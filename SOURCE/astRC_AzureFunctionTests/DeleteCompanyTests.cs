using Hammock.AssetView.Platinum.RC.Azure;
using Hammock.AssetView.Platinum.RC.Azure.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;

namespace astRC_AzureFunctionTests
{
    [TestClass]
    public class DeleteCompanyTests
    {
        [TestMethod]
        public void DeleteCompanyFunctionTest()
        {
            try
            {
                var str = "Server=tcp:takahisa-ishikawa-relaydb01.database.windows.net,1433;Initial Catalog=takahisa_ishikawa_relaydb;Persist Security Info=False;User ID=astrelayadmin;Password=hPmM8izzVDtXDkA7;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

                using (TransactionScope ts = new TransactionScope())
                {
                    using (SqlConnection sqlConnection = new SqlConnection(str))
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            sqlConnection.Open();
                            cmd.Connection = sqlConnection;

                            // dummy data
                            StringBuilder sbAddDummyData = new StringBuilder();
                            sbAddDummyData.Append(" INSERT INTO T_PRC_RELAY_SERVICE_COMPANY_MASTER ( ");
                            sbAddDummyData.Append(" COMPANY_CODE ");
                            sbAddDummyData.Append(", PASSWORD ");
                            sbAddDummyData.Append(", DELETED_DATETIME ");
                            sbAddDummyData.Append(", CREATED_DATETIME ");
                            sbAddDummyData.Append(", UPDATED_DATETIME ");
                            sbAddDummyData.Append(" ) VALUES ( ");
                            sbAddDummyData.Append("  @COMPANY_CODE ");
                            sbAddDummyData.Append(", @PASSWORD ");
                            sbAddDummyData.Append($", DATEADD(day, {-AzureFunctionSettings.COMPANY_DELETED_DATETIME_DAY}, GETDATE()) ");
                            sbAddDummyData.Append($", DATEADD(day, {-AzureFunctionSettings.COMPANY_DELETED_DATETIME_DAY}, GETDATE()) ");
                            sbAddDummyData.Append(", GETDATE() ) ");

                            cmd.CommandText = sbAddDummyData.ToString();

                            cmd.Parameters.AddWithValue("@COMPANY_CODE", "CompanyCodeTest");
                            cmd.Parameters.AddWithValue("@PASSWORD", "PasswordTest");

                            var rows = cmd.ExecuteNonQuery();

                            if (rows == -1)
                            {
                                Assert.Fail("Can not insert dummy data to run unit test!");
                            }

                            Assert.AreEqual(DeleteCompany.RunAsync(str).Result, rows);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Assert.Fail($"Unexpected exception of type {e.GetType()} caught: {e.Message}");
            }
        }
    }
}