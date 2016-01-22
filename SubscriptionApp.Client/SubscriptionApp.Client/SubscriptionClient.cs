﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SubscriptionApp.Client.Models;
using SubscriptionApp.Client.Services;

namespace SubscriptionApp.Client
{
    public class SubscriptionClient
    {
        private readonly WebClientService _webClientService;

        public IStorageMethod StorageMethod
        {
            get { return _storageMethod ?? new FakeStorageMethod(); }
            set { _storageMethod = value; }
        }

        private IStorageMethod _storageMethod;
        private readonly Cache _cache;
        private readonly string _cacheKey = "subscriber_key_";
        private readonly string _cacheAppId = "subscriber_appid_";
        private readonly string _configCacheKey = "subscrio_configuraion";

        public SubscriptionClient(string endpoint, string authorizationToken)
        {
            _webClientService = new WebClientService(endpoint, authorizationToken);
            _cache = new Cache();
        }

        public SubscriptionClient(WebClientService webClientService)
        {
            _webClientService = webClientService;
            _cache = new Cache();
        }


        public dynamic GetSubscriptionByKey(string key)
        {
            var subscriber = _cache.Get(_cacheKey + key) as SubscriberModel;

            if (subscriber != null) return subscriber.ToDynamic();

            subscriber = StorageMethod.GetByKey(key);
            if (subscriber != null)
            {
                _cache.Add(_cacheKey + key, subscriber, MyCachePriority.Default);
                return subscriber.ToDynamic();
            }

            var result = _webClientService.GetSubscriptionByKey(key);
            subscriber = JsonConvert.DeserializeObject<SubscriberModel>(result);

            if (subscriber == null) return null;
            _cache.Add(_cacheKey + key, subscriber, MyCachePriority.Default);
            StorageMethod.AddOrUpdateSubscriber(subscriber);
            return subscriber.ToDynamic();
        }

        public List<dynamic> GetSubscriptions()
        {
            var subscribers = StorageMethod.GetAllSubscriptions();

            if (subscribers != null) return subscribers.ToDynamics();

            subscribers = JsonConvert.DeserializeObject<List<SubscriberModel>>(_webClientService.GetSubscriptions());
            if (subscribers == null) return null;
            StorageMethod.AddOrUpdateSubscribers(subscribers);

            return subscribers.ToDynamics();
        }

        private SubscriberModel GetSubscriberModelByKey(string key)
        {
            var subscriber = _cache.Get(_cacheKey + key) as SubscriberModel;
            if (subscriber != null) return subscriber;

            subscriber = StorageMethod.GetByKey(key);
            if (subscriber != null)
            {
                _cache.Add(_cacheKey + key, subscriber, MyCachePriority.Default);
                return subscriber.ToDynamic();
            }

            var result = _webClientService.GetSubscriptionByKey(key);
            subscriber = JsonConvert.DeserializeObject<SubscriberModel>(result);

            if (subscriber == null) return null;
            _cache.Add(_cacheKey + key, subscriber, MyCachePriority.Default);
            StorageMethod.AddOrUpdateSubscriber(subscriber);
            return subscriber;
        }

        public dynamic GetSubscriptionByApplicationId(string applicationId)
        {
            var subscriber = _cache.Get(_cacheAppId + applicationId) as SubscriberModel;

            if (subscriber != null) return subscriber.ToDynamic();

            subscriber = StorageMethod.GetByApplicationId(applicationId);
            if (subscriber != null)
            {
                _cache.Add(_cacheAppId + applicationId, subscriber, MyCachePriority.Default);
                return subscriber.ToDynamic();
            }

            var result = _webClientService.GetSubscriptionByApplicationId(applicationId);
            subscriber = JsonConvert.DeserializeObject<SubscriberModel>(result);
            if (subscriber == null) return null;
            _cache.Add(_cacheAppId + applicationId, subscriber, MyCachePriority.Default);
            StorageMethod.AddOrUpdateSubscriber(subscriber);
            return subscriber.ToDynamic();
        }

        public dynamic CreateSubscription(int subscriptionTypeId, string applicationIdentifier, string name = null, int? billingSystemType = null, string billingSystemIdentifier = null)
        {
            var config = GetConfiguration();
            if (config?.SubscriptionTypes == null || config.SubscriptionTypes.Count == 0)
            {
                throw new Exception("Missing configuration or subscription types, no subscription types found");
            }
            var subscriptionType = config.SubscriptionTypes.FirstOrDefault(x => x.Id == subscriptionTypeId);
            if (subscriptionType == null)
            {
                throw new Exception($"Subscription type with id of: {subscriptionTypeId} was not found");
            }

            var model = new SubscriberModel
            {
                ApplicationId = applicationIdentifier,
                BillingSystemType = billingSystemType ?? subscriptionType.BillingSystemType,
                BillingSystemIdentifier = billingSystemIdentifier,
                CompanyId = config.Company.Id,
                DefaultGracePeriod = subscriptionType.DefaultGracePeriod,
                DefaultNeverExpire = subscriptionType.DefaultNeverExpire,
                DefaultResetFeaturesOnRenewal = subscriptionType.DefaultResetFeaturesOnRenewal,
                DefaultRevertOnExpiration = subscriptionType.DefaultRevertOnExpiration,
                DefaultRevertTo = subscriptionType.DefaultRevertTo,
                Name = name,
                SubscriptionTypeId = subscriptionType.Id,
                Features = subscriptionType.Features,
                ExpirationDate = DateTime.UtcNow.AddTicks(subscriptionType.TimeToExpireTicks.GetValueOrDefault())
            };

            var result = _webClientService.CreateSubscription(model);
            var newSubscriber = JsonConvert.DeserializeObject<SubscriberModel>(result);
            if (newSubscriber == null) return null;
            StorageMethod.AddOrUpdateSubscriber(newSubscriber);
            _cache.Add(_cacheAppId + newSubscriber.ApplicationId, newSubscriber, MyCachePriority.Default);
            _cache.Add(_cacheKey + newSubscriber.Key, newSubscriber, MyCachePriority.Default);
            return newSubscriber.ToDynamic();
        }

        public dynamic UpdateSubscription(dynamic subscriber)
        {
            var originalSubscriber = GetSubscriberModelByKey(subscriber.Key);
            var model = Extensions.AsSubscriberModel(subscriber, originalSubscriber);
            var result = _webClientService.UpdateSubscription(model);
            SubscriberModel updatedSubscriber = JsonConvert.DeserializeObject<SubscriberModel>(result);
            if (updatedSubscriber != null)
            {
                StorageMethod.AddOrUpdateSubscriber(updatedSubscriber);

                _cache.Remove(_cacheKey + updatedSubscriber.Key);
                _cache.Remove(_cacheAppId + updatedSubscriber.ApplicationId);

                _cache.Add(_cacheAppId + updatedSubscriber.ApplicationId, updatedSubscriber, MyCachePriority.Default);
                _cache.Add(_cacheKey + updatedSubscriber.Key, updatedSubscriber, MyCachePriority.Default);
                return updatedSubscriber.ToDynamic();
            }
            return null;
        }

        public void UpdateLocalSubscription(dynamic subscriber)
        {
            var originalSubscriber = GetSubscriberModelByKey(subscriber.Key);
            var model = Extensions.AsSubscriberModel(subscriber, originalSubscriber);
            StorageMethod.AddOrUpdateSubscriber(model);

            _cache.Remove(_cacheKey + model.Key);
            _cache.Remove(_cacheAppId + model.ApplicationId);

            _cache.Add(_cacheAppId + model.ApplicationId, model, MyCachePriority.Default);
            _cache.Add(_cacheKey + model.Key, model, MyCachePriority.Default);

        }

        public void SubscriptionUpdated(SubscriberModel model)
        {
            if (model != null)
            {
                StorageMethod.AddOrUpdateSubscriber(model);

                _cache.Remove(_cacheKey + model.Key);
                _cache.Remove(_cacheAppId + model.ApplicationId);

                _cache.Add(_cacheAppId + model.ApplicationId, model, MyCachePriority.Default);
                _cache.Add(_cacheKey + model.Key, model, MyCachePriority.Default);
            }
        }

        public void SubscriptionRemoved(SubscriberModel model)
        {
            if (model != null)
            {
                StorageMethod.SubscriberRemoved(model);

                _cache.Remove(_cacheKey + model.Key);
                _cache.Remove(_cacheAppId + model.ApplicationId);
            }
        }

        public void WebhookUpdate(SubscriberWebhookModel subscriberWebhookModel)
        {
            if (subscriberWebhookModel.Action == "add" || subscriberWebhookModel.Action == "update")
            {
                var model = JsonConvert.DeserializeObject<SubscriberModel>(subscriberWebhookModel.Subscriber);
                SubscriptionUpdated(model);
            }

            if (subscriberWebhookModel.Action == "delete")
            {
                var model = JsonConvert.DeserializeObject<SubscriberModel>(subscriberWebhookModel.Subscriber);
                SubscriptionRemoved(model);
            }

            if (subscriberWebhookModel.Action == "config")
            {
                var config = JsonConvert.DeserializeObject<Configuration>(_webClientService.GetConfiguration());
                if (config != null)
                {
                    StorageMethod.UpdateConfiguration(config);
                    _cache.Add(_configCacheKey, config, MyCachePriority.Default);
                }
            }
        }

        public Configuration GetConfiguration()
        {
            var config = _cache.Get(_configCacheKey) as Configuration;

            if (config != null) return config;
            config = StorageMethod.GetConfiguration();

            if (config != null) return config;
            config = JsonConvert.DeserializeObject<Configuration>(_webClientService.GetConfiguration());

            if (config != null)
            {
                StorageMethod.UpdateConfiguration(config);
                _cache.Add(_configCacheKey, config, MyCachePriority.Default);
            }

            return config;
        }
    }

    public static class Extensions
    {
        public static dynamic ToDynamic(this SubscriberModel subscriber)
        {
            var dic = subscriber.Features.ToDictionary(ft => ft.PropertyName, GetDerivedValue);
            dic.Add("SubscriptionTypeId", subscriber.SubscriptionTypeId);
            dic.Add("Id", subscriber.Id);
            dic.Add("Name", subscriber.Name);
            dic.Add("ApplicationId", subscriber.ApplicationId);
            dic.Add("Key", subscriber.Key);
            dic.Add("ExpirationDate", subscriber.ExpirationDate);
            dic.Add("BillingSystemIdentifier", subscriber.BillingSystemIdentifier);
            dic.Add("BillingSystemType", subscriber.BillingSystemType);
            dic.Add("IsExpired", !subscriber.DefaultNeverExpire && (subscriber.ExpirationDate.AddDays(subscriber.DefaultGracePeriod) < DateTime.UtcNow));
            return new DynamicDictionary(dic);
        }

        public static List<dynamic> ToDynamics(this List<SubscriberModel> subscribers)
        {
            return subscribers.Select(subscriberModel => subscriberModel.ToDynamic()).ToList();
        }

        public static SubscriberModel AsSubscriberModel(dynamic dynamic, SubscriberModel subscriber)
        {
            if (subscriber == null) throw new Exception("Cannot update subscription, original subscribption not found");
            if (dynamic == null) throw new ArgumentNullException(nameof(dynamic));
            var subscriberProps = subscriber.GetType().GetProperties();
            foreach (var property in dynamic.GetDynamicMemberNames())
            {
                if (subscriberProps.All(sp => property != sp.Name))
                {
                    var feature = subscriber.Features.FirstOrDefault(x => x.PropertyName == property);
                    if (feature == null) continue;
                    feature.Value = dynamic.GetMemberValue(property)?.ToString();
                }
                else
                {
                    var subscriberProp = subscriberProps.FirstOrDefault(x => x.Name == property);
                    subscriberProp?.SetValue(subscriber, dynamic.GetMemberValue(property));
                }
            }
            return subscriber;
        }

        public static T As<T>(this DynamicDictionary dynamic)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(dynamic));
        }

        private static object GetDerivedValue(FeatureModel feature)
        {
            if (feature.Value == null) return null;

            switch (feature.DataType)
            {
                case 0:
                    return bool.Parse(feature.Value);
                case 1:
                    return int.Parse(feature.Value);
                case 2:
                    return DateTime.Parse(feature.Value);
                case 3:
                    return feature.Value;
                case 4:
                    return decimal.Parse(feature.Value);
                default:
                    return null;
            }
        }

        public static void Configure(this SubscriptionClient client, IStorageMethod storageMethod)
        {
            client.StorageMethod = storageMethod;
        }

    }
}