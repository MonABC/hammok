using System;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Hammock.AssetView.Platinum.RC.Azure.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Hammock.AssetView.Platinum.RC.Azure
{
    public static class CreateLogTable
    {
        [FunctionName("CreateLogTable")]
        public static async Task Run([TimerTrigger("0 0 0 * * *")] TimerInfo myTimer, ILogger log)
        {
            try
            {
                log.LogInformation($"CreateLogTable executed at: { DateTime.Now }");
                await RunAsync(Environment.GetEnvironmentVariable("sqldb_connection"));
            }
            catch (Exception ex)
            {
                log.LogInformation($"Exception: { ex.StackTrace }");
            }
            finally
            {
                log.LogInformation($"CreateLogTable finished at: { DateTime.Now }");
            }
        }

        public static async Task<int> RunAsync(string sqldb_connection)
        {
            int iRows = 0;

            using (SqlConnection sqlConnection = new SqlConnection(sqldb_connection))
            {
                sqlConnection.Open();

                DateTime now = DateTime.Now;

                for (int i = 0; i < AzureFunctionSettings.LOG_TABLE_CREATE_DAY; i++)
                {
                    string strTableName = $"T_PRC_RELAY_SERVICE_LOG_{ now.ToString("yyyyMMdd") }";

                    StringBuilder sb = new StringBuilder();
                    sb.Append($" IF OBJECT_ID('{ strTableName }', 'U') IS NULL ");
                    sb.Append($" BEGIN ");
                    sb.Append($"  CREATE TABLE { strTableName }");
                    sb.Append("   ( ");
                    sb.Append("    DateTime datetime NOT NULL default current_timestamp, ");
                    sb.Append("    COMPANY_CODE varchar(50), ");
                    sb.Append("    LONG_SESSION_ID varchar(30) NOT NULL, ");
                    sb.Append("    SERVER_IPADDRESS varchar(50), ");
                    sb.Append("    SERVER_HOSTNAME varchar(300), ");
                    sb.Append("    SERVER_USERNAME varchar(300), ");
                    sb.Append("    CLIENT_IPADDRESS varchar(50), ");
                    sb.Append("    CLIENT_HOSTNAME varchar(300), ");
                    sb.Append("    CLIENT_USERNAME varchar(300), ");
                    sb.Append("    SESSION_CREATED datetime, ");
                    sb.Append("    CONNECTED datetime, ");
                    sb.Append("    DISCONNECTED datetime, ");
                    sb.Append("    SESSION_DELETED datetime, ");
                    sb.Append("    WEBAPI_TYPE int NOT NULL");
                    sb.Append("   ) ");
                    sb.Append($" END ");

                    using (SqlCommand cmd = new SqlCommand(sb.ToString(), sqlConnection))
                    {
                        iRows += await cmd.ExecuteNonQueryAsync();
                    }

                    now = now.AddDays(1);
                }
            }

            return iRows;
        }
    }
}