using Hammock.AssetView.Platinum.Tools.RC.RelayService.Common;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Dao
{
    /// <summary>
    /// PLATINUMのDBにアクセスするための基底クラス。
    /// </summary>
    public abstract class PlatinumDaoBase : DaoBase
    {
        /// <summary>
        ///   コンストラクタ
        /// </summary>
        protected PlatinumDaoBase()
            : base(Variables.PlatinumConnectionStringBuilder.ConnectionString, Variables.CommandTimeOut)
        {
        }
    }
}