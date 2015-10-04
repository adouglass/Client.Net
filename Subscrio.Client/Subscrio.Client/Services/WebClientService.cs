using System;
using System.Net;
using Newtonsoft.Json;
using Subscrio.Client.Models;

namespace Subscrio.Client.Services
{
    public class WebClientService
    {

        public WebClientService(string endpoint, string authorizationToken)
        {
            Endpoint = endpoint;
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
            using (var wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.Headers[HttpRequestHeader.Authorization] = AuthToken;
                try
                {
                    return wc.DownloadString(Endpoint + GET_SUBSCRIPTIONS);
                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
        }

        public virtual string GetConfiguration()
        {
            using (var wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.Headers[HttpRequestHeader.Authorization] = AuthToken;
                try
                {
                    return wc.DownloadString(Endpoint + GET_CONFIGURATION);
                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
        }

        public virtual string GetSubscriptionByKey(string key)
        {
            using (var wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.Headers[HttpRequestHeader.Authorization] = AuthToken;
                try
                {
                    return wc.DownloadString(Endpoint + GET_SUBSCRIPTION_BYKEY + "/" + key);
                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
        }

        public virtual string GetSubscriptionByApplicationId(string applicationId)
        {
            using (var wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.Headers[HttpRequestHeader.Authorization] = AuthToken;
                try
                {
                    return wc.DownloadString(Endpoint + GET_SUBSCRIPTION_BYAPPLICATIONID + "/" + applicationId);
                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
        }

        public virtual string CreateSubscription(SubscriberModel model)
        {
            using (var wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.Headers[HttpRequestHeader.Authorization] = AuthToken;
                try
                {
                    return wc.UploadString(Endpoint + CREATE_SUBSCRIPTION, "POST", JsonConvert.SerializeObject(model));
                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
        }

        public virtual string UpdateSubscription(SubscriberModel model)
        {
            using (var wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.Headers[HttpRequestHeader.Authorization] = AuthToken;
                try
                {
                    return wc.UploadString(Endpoint + UPDATE_SUBSCRIPTION, "PUT", JsonConvert.SerializeObject(model));
                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
        }
    }
}
