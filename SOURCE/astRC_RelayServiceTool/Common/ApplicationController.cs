using Hammock.AssetView.Platinum.Tools.RC.RelayService.Enums;
using System;
using System.Windows;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common
{
    /// <summary>
    /// アプリケーション全体に関わる制御を行うクラス。
    /// </summary>
    public class ApplicationController
    {
        /// <summary>
        /// メッセージを出力する。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="image"></param>
        public static void ShowMessage(string message, MessageBoxImage image)
        {
            //メッセージボックスを表示する。
            //MessageBox.Show(message, "RCリレーサービスツール", MessageBoxButton.OK, image);
            ProgramLogWriter.Write(1, message);
        }

        /// <summary>
        /// 終了コードを指定してアプリケーションを終了する。
        /// 終了コードに対応したログを出力する。
        /// </summary>
        /// <param name="exitCode"></param>
        public static void ExitApplication(ExitCodes exitCode)
        {
            //終了コードによるログメッセージを設定する。
            var message = string.Empty;
            switch (exitCode)
            {
                case ExitCodes.InvalidArgument:
                    message = "引数が不正なため終了しました。";
                    break;
                case ExitCodes.Fail:
                    message = "RCリレーサービスアカウントファイル作成処理に失敗しました。";
                    break;
                case ExitCodes.Exception:
                    message = "例外が発生したため終了しました。";
                    break;
            }

            if (!string.IsNullOrEmpty(message))
            {
                ProgramLogWriter.Write(1, message);
            }

            if (Application.Current != null)
            {
                Application.Current.Shutdown((int)exitCode);
            }
            else
            {
                Environment.Exit((int)exitCode);
            }
        }
    }
}