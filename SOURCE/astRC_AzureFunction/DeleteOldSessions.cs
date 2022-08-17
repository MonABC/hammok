using System;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Hammock.AssetView.Platinum.RC.Azure.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Hammock.AssetView.Platinum.RC.Azure
{
    public static class DeleteOldSessions
    {
        [FunctionName("DeleteOldSessions")]
        public static async Task Run([TimerTrigger("0 0 0 * * *")] TimerInfo myTimer, ILogger log)
        {
            try
            {
                log.LogInformation($"DeleteOldSessions executed at: { DateTime.Now }");
                await RunAsync(Environment.GetEnvironmentVariable("sqldb_connection"));
            }
            catch (Exception ex)
            {
                log.LogInformation($"Exception: { ex.StackTrace }");
            }
            finally
            {
                log.LogInformation($"DeleteOldSessions finished at: { DateTime.Now }");
            }
        }

        public static async Task<int> RunAsync(string sqldb_connection)
        {
            int iRows = 0;

            using (SqlConnection sqlConnection = new SqlConnection(sqldb_connection))
            {
                sqlConnection.Open();

                StringBuilder sb = new StringBuilder();
                sb.Append(" DELETE T_PRC_RELAY_SERVICE_SESSION_MASTER ");
                sb.Append($" WHERE (CONNECTED_FLAG = '{ AzureFunctionSettings.SESSION_CONNECTED_FLAG_WAITING}' AND GETDATE() >= DATEADD(hour, { AzureFunctionSettings.SESSION_CONNECTED_FLAG_WAITING_HOUR }, SESSION_CREATED)) ");
                sb.Append($" OR (CONNECTED_FLAG = '{ AzureFunctionSettings.SESSION_CONNECTED_FLAG_CONNECTING }' AND GETDATE() >= DATEADD(hour, { AzureFunctionSettings.SESSION_CONNECTED_FLAG_CONNECTING_HOUR }, SESSION_CREATED)) ");

                using (SqlCommand cmd = new SqlCommand(sb.ToString(), sqlConnection))
                {
                    iRows = await cmd.ExecuteNonQueryAsync();
                }
            }

            return iRows;
        }
    }
}
