using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Hammock.AssetView.Platinum.RC.Azure.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Hammock.AssetView.Platinum.RC.Azure
{
    public static class DeleteLogTable
    {
        [FunctionName("DeleteLogTable")]
        public static async Task Run([TimerTrigger("0 0 0 * * *")] TimerInfo myTimer, ILogger log)
        {
            try
            {
                log.LogInformation($"DeleteLogTable executed at: { DateTime.Now} ");
                await RunAsync(Environment.GetEnvironmentVariable("sqldb_connection"));
            }
            catch (Exception ex)
            {
                log.LogInformation($"Exception: { ex.StackTrace }");
            }
            finally
            {
                log.LogInformation($"DeleteLogTable finished at: { DateTime.Now }");
            }
        }

        public static async Task<int> RunAsync(string sqldb_connection)
        {
            int iRows = 0;

            using (SqlConnection sqlConnection = new SqlConnection(sqldb_connection))
            {
                sqlConnection.Open();

                StringBuilder sb = new StringBuilder();
                sb.Append($" SELECT TABLE_NAME FROM information_schema.tables  ");
                sb.Append($" WHERE TABLE_NAME LIKE 'T_PRC_RELAY_SERVICE_LOG_%' ");
                sb.Append($" AND GETDATE() >= DATEADD(day, { AzureFunctionSettings.LOG_TABLE_DETELE_DAY }, CONVERT(date, SUBSTRING(TABLE_NAME, LEN(TABLE_NAME) - 7, 8))) ");
                sb.Append(" ORDER BY TABLE_NAME ");

                List<string> lstTable = new List<string>();

                using (SqlCommand cmd = new SqlCommand(sb.ToString(), sqlConnection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                lstTable.Add(reader.GetString(0));
                            }
                        }
                    }
                }

                foreach (string strTableName in lstTable)
                {
                    sb = new StringBuilder();
                    sb.Append($"DROP TABLE dbo.{ strTableName }");

                    using (SqlCommand cmd2 = new SqlCommand(sb.ToString(), sqlConnection))
                    {
                        iRows += await cmd2.ExecuteNonQueryAsync();
                    }
                }
            }

            return iRows;
        }
    }
}
