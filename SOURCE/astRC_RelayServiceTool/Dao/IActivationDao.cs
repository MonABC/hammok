using Hammock.AssetView.Platinum.Tools.RC.RelayService.Field;
using System.Collections.Generic;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Dao
{
    /// <summary>
    /// アクティベーション情報等にアクセスするためのクラス。
    /// </summary>
    public interface IActivationDao : IDaoBase
    {
        /// <summary>
        /// DBに接続できるか確認する。
        /// </summary>
        /// <returns></returns>
        bool CanConnect();

        /// <summary>
        /// アカウントのリストを取得する。
        /// </summary>
        /// <param name="serchInfo">検索条件</param>
        /// <returns></returns>
        IList<CompanyListField> GetCompanyList(CompanyListField serchInfo = null);
    }
}