using Hammock.AssetView.Platinum.Tools.RC.RelayService.Dao;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.ExtensionMethod;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Field;
using System;
using System.Collections.Generic;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Model
{
    public class MainLogic
    {
        public Func<IActivationDao> CreateActivationDao { get; set; }

        /// <summary>
        /// メッセージを取得する。
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public MainLogic()
        {
            //各クラスの生成ロジックを登録する。
            CreateActivationDao = () => new ActivationDao();
        }

        /// <summary>
        /// アカウントのリストを取得する。
        /// </summary>
        /// <returns></returns>
        public IList<CompanyListField> GetCompanyList()
        {
            var result = new List<CompanyListField>();
            Message = string.Empty;

            using (var activationDao = CreateActivationDao())
            {
                //DBに接続できなければ失敗とする。
                if (!activationDao.CanConnect())
                {
                    Message = "データベースに接続できません。";
                    return result;
                }

                return activationDao.GetCompanyList();
            }
        }
    }
}
