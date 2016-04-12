using System;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using SubscriptionApp.Client.Models;

namespace SubscriptionApp.Client.Services
{
    public class WebClientService
    {

        public WebClientService(string endpoint, string authorizationToken)
        {
            Endpoint = endpoint.EndsWith("/") ? endpoint : endpoint + "/";
            AuthToken = authorizationToken;
        }

        public WebClientService()
        {
            
        }

        private readonly string Endpoint;
        private readonly string AuthToken;
        private const string GET_SUBSCRIPTION_BYKEY = "api/client/getsubscriptionbykey";
        private const string GET_SUBSCRIPTION_BYAPPLICATIONID = "api/client/getsubscriptionbyapplicationid";
        private const string GET_SUBSCRIPTIONS = "api/client/getsubscriptions";
        private const string GET_CONFIGURATION = "api/client/getconfiguration";
        private const string CREATE_SUBSCRIPTION = "api/client/createsubscription";
        private const string UPDATE_SUBSCRIPTION = "api/client/updatesubscription";


        public virtual string GetSubscriptions()
        {
            using (var wc = new WebClient { Encoding = Encoding.UTF8 })
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.Headers[HttpRequestHeader.Authorization] = AuthToken;
                try
                {
                    return wc.DownloadString(Endpoint + GET_SUBSCRIPTIONS);
                }
                catch (WebException ex)
                {
                    return wc.DownloadString(Endpoint + GET_SUBSCRIPTIONS);
                }
            }
        }

        public virtual string GetConfiguration()
        {
            using (var wc = new WebClient { Encoding = Encoding.UTF8 })
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.Headers[HttpRequestHeader.Authorization] = AuthToken;
                try
                {
                    return wc.DownloadString(Endpoint + GET_CONFIGURATION);
                }
                catch (WebException ex)
                {
                    return wc.DownloadString(Endpoint + GET_CONFIGURATION);
                }
            }
        }

        public virtual string GetSubscriptionByKey(string key)
        {
            using (var wc = new WebClient { Encoding = Encoding.UTF8 })
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.Headers[HttpRequestHeader.Authorization] = AuthToken;
                try
                {
                    return wc.DownloadString(Endpoint + GET_SUBSCRIPTION_BYKEY + "/" + key);
                }
                catch (WebException ex)
                {
                    var webResponse = (HttpWebResponse)ex.Response;
                    if (webResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        return string.Empty;
                    }
                    return wc.DownloadString(Endpoint + GET_SUBSCRIPTION_BYKEY + "/" + key);
                }
            }
        }

        public virtual string GetSubscriptionByApplicationId(string applicationId)
        {
            using (var wc = new WebClient { Encoding = Encoding.UTF8 })
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.Headers[HttpRequestHeader.Authorization] = AuthToken;
                try
                {
                    return wc.DownloadString(Endpoint + GET_SUBSCRIPTION_BYAPPLICATIONID + "/" + applicationId);
                }
                catch (WebException ex)
                {
                    var webResponse = (HttpWebResponse)ex.Response;
                    if (webResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        return string.Empty;
                    }

                    return wc.DownloadString(Endpoint + GET_SUBSCRIPTION_BYAPPLICATIONID + "/" + applicationId);
                }
            }
        }

        public virtual string CreateSubscription(SubscriberModel model)
        {
            using (var wc = new WebClient { Encoding = Encoding.UTF8 })
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.Headers[HttpRequestHeader.Authorization] = AuthToken;
                try
                {
                    return wc.UploadString(Endpoint + CREATE_SUBSCRIPTION, "POST", JsonConvert.SerializeObject(model));
                }
                catch (WebException ex)
                {
                    return wc.UploadString(Endpoint + CREATE_SUBSCRIPTION, "POST", JsonConvert.SerializeObject(model));
                }
            }
        }

        public virtual string UpdateSubscription(SubscriberModel model)
        {
            using (var wc = new WebClient { Encoding = Encoding.UTF8 })
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.Headers[HttpRequestHeader.Authorization] = AuthToken;
                try
                {
                    return wc.UploadString(Endpoint + UPDATE_SUBSCRIPTION, "PUT", JsonConvert.SerializeObject(model));
                }
                catch (WebException ex)
                {
                    return wc.UploadString(Endpoint + UPDATE_SUBSCRIPTION, "PUT", JsonConvert.SerializeObject(model));
                }
            }
        }
    }
}
