using Hammock.AssetView.Platinum.Tools.RC.RelayService.Common;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.ExtensionMethod;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Field;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService
{
    /// <summary>
    ///   管理コンソールログイン用のINIファイル管理
    /// </summary>
    public class ViewerSettingManager
    {
        /// <summary>
        /// 設定ファイルの名前を指定する。
        /// </summary>
        private const string SETTING_FILE_NAME = "hmkActivationLogin.ini";

        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        private const string SECTION_NAME = "DBServerInfo";

        /// <summary>
        /// 未設定数値
        /// </summary>
        private const int UNSET_VALUE = -1;

        /// <summary>
        /// データベース接続のタイムアウト値（秒）
        /// </summary>
        private const int DEFAULT_CONNECTION_TIMEOUT = 60;

        /// <summary>
        /// 設定ファイルの配置場所を指定する。
        /// </summary>
        private readonly string _settingFolderPath = string.Empty;

        /// <summary>
        /// メッセージを取得する。
        /// </summary>
        public string Message { get; private set; }

        public IList<DBConnectionSetting> DBConnectionSettings { get; private set; }
        private int _dbConnectionTimeOut;

        public int DBConnectionTimeOut
        {
            get { return _dbConnectionTimeOut < 0 ? DEFAULT_CONNECTION_TIMEOUT : _dbConnectionTimeOut; }
            private set { _dbConnectionTimeOut = value; }
        }

        public int DBCommandTimeOut { get; private set; }

        public string DBConnectionTimeOutText
        {
            get { return DBConnectionTimeOut < 0 ? DEFAULT_CONNECTION_TIMEOUT.ToString() : DBConnectionTimeOut.ToString(); }
        }

        public string DBCommandTimeOutText
        {
            get { return DBCommandTimeOut < 0 ? Variables.CommandTimeOut.ToString() : DBCommandTimeOut.ToString(); }
        }

        public ViewerSettingManager()
        {
            DBConnectionSettings = new List<DBConnectionSetting>();
            DBConnectionTimeOut = UNSET_VALUE;
            DBCommandTimeOut = UNSET_VALUE;
            _settingFolderPath = Path.Combine(Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf(@"\") + 1), @"Setting");
        }

        /// <summary>
        ///   ログイン用のイニシャライズファイルの読み込み
        /// </summary>
        public void ReadIniFile()
        {
            DBConnectionSettings.Clear();
            DBConnectionTimeOut = UNSET_VALUE;
            DBCommandTimeOut = UNSET_VALUE;

            var filePath = Path.Combine(_settingFolderPath, SETTING_FILE_NAME);
            if (string.IsNullOrEmpty(filePath))
            {
                Message = string.Format("指定されたパスに{0}が見つかりません。", SETTING_FILE_NAME);
                return;
            }

            var fileFullPath = Path.GetFullPath(filePath);
            if (string.IsNullOrEmpty(fileFullPath))
            {
                Message = string.Format("指定されたパスに{0}が見つかりません。", SETTING_FILE_NAME);
                return;
            }

            if (!File.Exists(fileFullPath))
            {
                Message = string.Format("指定されたパスに{0}が見つかりません。", SETTING_FILE_NAME);
                return;
            }

            var ini = new IniFileParser();

            DBConnectionTimeOut = ini.GetValue(SECTION_NAME, "ConnectionTimeOut", String.Empty, fileFullPath).ToIntWithDefault(UNSET_VALUE);
            DBCommandTimeOut = ini.GetValue(SECTION_NAME, "CommandTimeOut", String.Empty, fileFullPath).ToIntWithDefault(UNSET_VALUE);

            int index = 1;
            while (true)
            {
                if (ini.GetValue(SECTION_NAME, string.Format("DBServerName{0}", index), String.Empty, fileFullPath).Decrypt() == "")
                {
                    break; // 最後まで読み込んだらループを抜けて終了
                }
                var dbServerPortText = ini.GetValue(SECTION_NAME, string.Format("DBServerPort{0}", index), String.Empty, fileFullPath);
                int dbServerPort;
                _ = int.TryParse(dbServerPortText, out dbServerPort);
                // リストに追加
                DBConnectionSetting setting = new DBConnectionSetting
                {
                    SettingIndex = index,
                    DBServerName = ini.GetValue(SECTION_NAME, string.Format("DBServerName{0}", index), String.Empty, fileFullPath).Decrypt(),
                    DBServerPort = dbServerPort,
                    DBName = ini.GetValue(SECTION_NAME, string.Format("DBUserName{0}", index), String.Empty, fileFullPath).Decrypt(),
                    DBUserName = ini.GetValue(SECTION_NAME, string.Format("DBUserName{0}", index), String.Empty, fileFullPath).Decrypt(),
                    DBPassword = ini.GetValue(SECTION_NAME, string.Format("DBPassword{0}", index), String.Empty, fileFullPath).Decrypt(),
                    DBConnectionTimeOut = DBConnectionTimeOut < 0 ? DEFAULT_CONNECTION_TIMEOUT : DBConnectionTimeOut,
                    DBCommandTimeOut = DBCommandTimeOut < 0 ? Variables.CommandTimeOut : DBCommandTimeOut
                };

                DBConnectionSettings.Add(setting);
                index++;
            }
        }

        /// <summary>
        ///   ログイン用のイニシャライズファイルの書き込み
        /// </summary>
        public void WriteIniFile(string dbConnectionTimeOut, string dbCommandTimeOut)
        {
            // フォルダがなければ作成しておく
            if (!Directory.Exists(_settingFolderPath))
            {
                Directory.CreateDirectory(_settingFolderPath);
            }

            var filePath = Path.Combine(_settingFolderPath, SETTING_FILE_NAME);
            var fileFullPath = Path.GetFullPath(filePath);

            var writer = new IniFileWriter();
            // 削除
            writer.SetValue(SECTION_NAME, null, null, fileFullPath);

            // 全データを保存（文字列を暗号化する）
            int index = 0;
            foreach (var setting in DBConnectionSettings.OrderBy(p => p.SettingIndex))
            {
                writer.SetValue(SECTION_NAME, string.Format("DBServerName{0}", index + 1), setting.DBServerName.Encrypt(), fileFullPath);
                writer.SetValue(SECTION_NAME, string.Format("DBName{0}", index + 1), setting.DBName.Encrypt(), fileFullPath);
                writer.SetValue(SECTION_NAME, string.Format("DBUserName{0}", index + 1), setting.DBUserName.Encrypt(), fileFullPath);
                writer.SetValue(SECTION_NAME, string.Format("DBPassword{0}", index + 1), setting.DBPassword.Encrypt(), fileFullPath);
                index++;
            }
            // 元のファイルにDBのタイムアウト値があれば値を設定する
            if (0 <= DBConnectionTimeOut)
            {
                writer.SetValue(SECTION_NAME, "ConnectionTimeOut", dbConnectionTimeOut, fileFullPath);
            }
            if (0 <= DBCommandTimeOut)
            {
                writer.SetValue(SECTION_NAME, "CommandTimeOut", dbCommandTimeOut, fileFullPath);
            }
        }
    }
}
