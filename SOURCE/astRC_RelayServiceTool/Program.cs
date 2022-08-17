using Hammock.AssetView.Platinum.Tools.RC.RelayService.Common;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Dao;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Field;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Enums;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Model;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.ExtensionMethod;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService
{
    internal static class Program
    {
        private static string _settingFilePath;

        private const string COMPANIES_FILE_NAME = @"astRC_RelayService_Companies.esf";
        private static string companies_file_path = string.Empty;

        public static IXmlSerializeDao SerializeDao { get; set; }

        private const string DB_NAME = "HammockActivationDB";

        private static readonly ViewerSettingManager _viewerSettingManager = new ViewerSettingManager();

        /// <summary>
        /// アプリケーション開始時の処理。
        /// </summary>
        public static void Main()
        {
            try
            {
                // Xmlファイル読み込み
                string executeFolder = Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf(@"\") + 1);
                _settingFilePath = Path.Combine(executeFolder, Constants.LOCAL_SETTINIG_FILE_NAME);

                SerializeDao = new XmlSerializeDao();

                Variables.InitializeVariables();

                //ログライターを初期化する。
                ProgramLogWriter.Init(Path.Combine(executeFolder, "Logs"), "astRC_RelayServiceTool");
                ProgramLogWriter.WriteVersion();
                ProgramLogWriter.Write(1, "astRC_RelayServiceTool START");

                //二重起動をチェックする
                if (!MutexManager.CreateMutex())
                {
                    ApplicationController.ShowMessage("RCリレーサービスツールは既に起動されています。", MessageBoxImage.Information);
                    ApplicationController.ExitApplication(ExitCodes.Duplicate);
                    return;
                }

                //INIファイル読み込み
                _viewerSettingManager.ReadIniFile();

                if (!_viewerSettingManager.DBConnectionSettings.Any())
                {
                    ApplicationController.ExitApplication(ExitCodes.InvalidCondition);
                    return;
                }

                var dbConnectionSetting = _viewerSettingManager.DBConnectionSettings.FirstOrDefault();
                if (dbConnectionSetting == null)
                {
                    // データベース接続設定が取得できないためエラー
                    ProgramLogWriter.Write(1, @"データベース接続設定を取得できませんでした。
接続設定ファイルをご確認ください。");
                    ApplicationController.ExitApplication(ExitCodes.Exception);
                    return;
                }
                else
                {
                    // コマンドタイムアウト変更対応
                    Variables.CommandTimeOut = dbConnectionSetting.DBCommandTimeOut;
                    ProgramLogWriter.Write(1, string.Format("CommandTimeOut:{0}", Variables.CommandTimeOut));
                }

                //接続文字列初期化
                Variables.PlatinumConnectionStringBuilder = CreateConnectionSettingBuilder(dbConnectionSetting);

                var mainLogic = new MainLogic();
                var companyList = mainLogic.GetCompanyList();
                if (!string.IsNullOrEmpty(mainLogic.Message))
                {
                    ProgramLogWriter.Write(1, mainLogic.Message);
                    ApplicationController.ExitApplication(ExitCodes.Exception);
                    return;
                }

                var field = SerializeDao.Read<LocalSettingField>(_settingFilePath);
                companies_file_path = Path.Combine(string.IsNullOrEmpty(field.DefaultOutputFilePath) ? executeFolder : field.DefaultOutputFilePath, COMPANIES_FILE_NAME);

                var wkList = new List<CompanyModel>();
                foreach (var company in companyList)
                {
                    wkList.Add(new CompanyModel(company.Id.Trim(), company.Password.Decrypt().Trim()));
                }

                var bkFolder = Path.Combine(executeFolder, $"bk\\{DateTime.Now:yyyyMMddHHmmss}");
                if (!Directory.Exists(bkFolder))
                {
                    Directory.CreateDirectory(bkFolder);
                }
                var bk_companies_file_path = Path.Combine(bkFolder, COMPANIES_FILE_NAME);
                SettingsFramework.Save<IEnumerable<CompanyModel>>(bk_companies_file_path, wkList);

                if (File.Exists(companies_file_path))
                {
                    // 出力ファイルと使用中ファイルを比較する
                    string sourceContent = Encrypt.DecryptFromFile(bk_companies_file_path);
                    if (string.IsNullOrEmpty(sourceContent))
                    {
                        // 復号できないためエラー
                        var message = $"出力ファイルを復号できませんでした。:{bk_companies_file_path}";
                        SendMessage(field, message);
                        ProgramLogWriter.Write(1, message);
                        ApplicationController.ExitApplication(ExitCodes.Exception);
                        return;
                    }
                    string destinateContent = Encrypt.DecryptFromFile(companies_file_path);
                    if (string.IsNullOrEmpty(destinateContent))
                    {
                        // 復号できないためエラー
                        var message = $"使用中ファイルを復号できませんでした。:{companies_file_path}";
                        SendMessage(field, message);
                        ProgramLogWriter.Write(1, message);
                        ApplicationController.ExitApplication(ExitCodes.Exception);
                        return;
                    }
                    // 内容が一致してればファイルを変更しない
                    if (sourceContent == destinateContent)
                    {
                        var message = $"データに変更がないため処理を終了します。";
                        SendMessage(field, message);
                        ProgramLogWriter.Write(1, message);
                        ApplicationController.ExitApplication(ExitCodes.Success);
                        return;
                    }
                    else
                    {
                        File.Copy(bk_companies_file_path, companies_file_path, true);
                    }
                }
                else
                {
                    // ファイルが存在しない場合は無条件でコピー
                    File.Copy(bk_companies_file_path, companies_file_path);
                }
                ProgramLogWriter.Write(1, $"ファイルコピーしました。");

                ApplicationController.ExitApplication(ExitCodes.Success);
            }
            catch (Exception ex)
            {
                Program.Application_DispatcherUnhandledException(ex);
                throw;
            }
        }

        private static void SendMessage(LocalSettingField field, string body)
        {
            var sb = new StringBuilder();
            if (!field.StopTeams)
            {
                sb.AppendLine($"■{DateTime.Now:yyyy/MM/dd HH:mm:ss} RCリレーサービス登録結果");
                sb.AppendLine(body);
                // Teamsへ通知
                SendMessageTeams(field.MicrosoftWebhookUrl, sb.ToString());
            }
        }

        #region 接続文字列を生成する

        /// <summary>
        ///   DB接続文字列情報を生成する
        /// </summary>
        /// <param name="dbConnectionSetting"></param>
        private static MySqlConnectionStringBuilder CreateConnectionSettingBuilder(DBConnectionSetting dbConnectionSetting)
        {
            return new MySqlConnectionStringBuilder
            {
                Server = dbConnectionSetting.DBServerName,
                Port = dbConnectionSetting.DBServerPort.HasValue ? (uint)dbConnectionSetting.DBServerPort.Value : 0, // 文字列チェック済なのでパースで変換
                UserID = dbConnectionSetting.DBUserName,
                Password = dbConnectionSetting.DBPassword,
                Database = DB_NAME,
                ConnectionTimeout = (uint)dbConnectionSetting.DBConnectionTimeOut,
                SslMode = dbConnectionSetting.DBMySqlSslMode,
            };
        }

        #endregion

        private static void SendMessageTeams(string microsoftWebhookUrl, string postdata)
        {
            using (var httpClient = new HttpClient())
            {
                var param = new Hashtable();
                param["Text"] = postdata.Replace("\r\n", "<br>");
                var json = JsonConvert.SerializeObject(param);

                using (var content = new StringContent(json))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    var response = httpClient.PostAsync(microsoftWebhookUrl, content);
                    response.Wait();
                }
            }
        }

        /// <summary>
        /// 集約例外ハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Application_DispatcherUnhandledException(Exception ex)
        {
            // 例外の内容をログに記述する。
            ProgramLogWriter.Write(1, ex);

            //例外メッセージを表示する。
            var message = string.Format("{0}\r\n例外が発生したため処理を中断しました。", ex.Message);
            ApplicationController.ShowMessage(message, MessageBoxImage.Exclamation);

            //引数の解釈が完了していないか、または画面無しで実行している場合はアプリケーションを終了する。
            ApplicationController.ExitApplication(ExitCodes.Exception);
        }

        /// <summary>
        /// アプリケーション終了時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Application_Exit(object sender, ExitEventArgs e)
        {
            //終了ログを出力する。
            ProgramLogWriter.Write(1, "astRC_RelayServiceTool END");

            //MUTEXを解放する。
            MutexManager.ReleaseMutex();
        }
    }
}
