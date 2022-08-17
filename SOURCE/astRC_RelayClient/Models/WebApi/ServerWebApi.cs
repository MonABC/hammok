using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.Utils;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Models.V1.Server;
using System;
using System.Net;
using System.Reflection;
using System.Text;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.WebApi
{
    internal class ServerWebApi
    {
        private readonly string _cloudUrl;
        private readonly string _userName;
        private readonly string _password;

        public ServerWebApi(string url, string userName, string password)
        {
            _cloudUrl = url;
            _userName = userName;
            _password = password;
        }

        public CreateSessionResultModel CreateSession(CreateSessionRequestModel model)
        {
            Logger.Debug(string.Format("{0}: {1}", MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name));

            var request = (HttpWebRequest)System.Net.WebRequest.Create(_cloudUrl + "api/v1/server/sessions/");
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(_userName + ":" + _password)));
            request.Timeout = 1000 * 10;
            request.ReadWriteTimeout = 1000 * 10;

            using (var requestStream = request.GetRequestStream())
            {
                JsonUtils.ToJson(requestStream, model);
            }

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                {
                    return JsonUtils.FromJson<CreateSessionResultModel>(responseStream);
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

            var request = (HttpWebRequest)System.Net.WebRequest.Create(_cloudUrl + "api/v1/server/sessions?id=" + sessionId);
            request.Method = "DELETE";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(_userName + ":" + _password)));
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
