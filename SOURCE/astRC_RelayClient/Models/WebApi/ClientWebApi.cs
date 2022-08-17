using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.Utils;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Models.V1.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.WebApi
{
    public class ClientWebApi : IClientWebApi
    {
        public string CloudUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public IEnumerable<GetSessionResultModel> GetSessionList()
        {
            Logger.Debug(string.Format("{0}: {1}", MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name));

            var request = (HttpWebRequest)System.Net.WebRequest.Create(CloudUrl + "api/v1/client/sessions/");
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(UserName + ":" + Password)));
            request.Timeout = 1000 * 10;
            request.ReadWriteTimeout = 1000 * 10;

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                {
                    return JsonUtils.FromJson<IEnumerable<GetSessionResultModel>>(responseStream);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
        }

        public void UpdateSessions(IEnumerable<UpdateSessionRequestModel> models)
        {
            Logger.Debug(string.Format("{0}: {1}", MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name));

            var request = (HttpWebRequest)System.Net.WebRequest.Create(CloudUrl + "api/v1/client/sessions/update");
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(UserName + ":" + Password)));
            request.Timeout = 1000 * 10;
            request.ReadWriteTimeout = 1000 * 10;

            using (var requestStream = request.GetRequestStream())
            {
                JsonUtils.ToJson(requestStream, models);
            }

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {

                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
        }

        public void DeleteSessions(IEnumerable<DeleteSessionRequestModel> models)
        {
            Logger.Debug(string.Format("{0}: {1}", MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name));

            var request = (HttpWebRequest)System.Net.WebRequest.Create(CloudUrl + "api/v1/client/sessions/delete");
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(UserName + ":" + Password)));
            request.Timeout = 1000 * 10;
            request.ReadWriteTimeout = 1000 * 10;

            using (var requestStream = request.GetRequestStream())
            {
                JsonUtils.ToJson(requestStream, models);
            }

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
        }

        public void DeleteSession(string sessionId)
        {
            Logger.Debug(string.Format("{0}: {1}", MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name));

            var request = (HttpWebRequest)System.Net.WebRequest.Create(CloudUrl + "api/v1/client/sessions?id=" + sessionId);
            request.Method = "DELETE";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(UserName + ":" + Password)));
            request.Timeout = 1000 * 10;
            request.ReadWriteTimeout = 1000 * 10;

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {

                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
        }
    }
}
