using System;
using Falcon.FalconAnalytics.Scripts;
using Falcon.FalconAnalytics.Scripts.Counters;
using Falcon.FalconAnalytics.Scripts.Enum;
using Falcon.FalconAnalytics.Scripts.Message.DWH;
using Falcon.FalconAnalytics.Scripts.Message.Exceptions;
using Falcon.FalconCore.Scripts;
using Falcon.FalconCore.Scripts.Utils;
using Falcon.FalconCore.Scripts.Utils.Data;
using Falcon.FalconCore.Scripts.Utils.Singletons;
using Newtonsoft.Json;


namespace Falcon
{
    /// <summary>
    ///     Basic Log class for FalconAnalytic.
    ///     Users need to call functions at certain times in the game to send information to the data analysis server, then can
    ///     view the analyzed data at https://data4game.com.
    ///     Instructions for viewing detailed metrics are available at
    ///     https://falcon-game-studio.gitbook.io/falcon-bigdata/giai-thich-bieu-do/tong-quan.
    /// </summary>
    /// <remarks>
    ///     <li>Parameters in functions, if not specifically noted, all need to enter a non-null value</li>
    ///     <li>strLen in the note (if any) is the length of the input string value</li>
    ///     <li>
    ///         If the player does not have an internet connection when playing the game, the log cannot be sent to the server,
    ///         but instead saved to the local device and be sent when available.
    ///         The saving is max at 100 messages, if exceeded will delete the oldest request in the Cache
    ///     </li>
    /// </remarks>
    public class DWHLog : FSingleton<DWHLog>, ILog
    {
        public static ILog Log => Instance;

        public void ActionLog(string actionName)
        {
            long now = FTime.CurrentTimeMillis();
            PrintCacheRequest(nameof(ActionLog), actionName);
            WaitInit(() => new DwhActionLog(actionName, now));
        }

        public void AdsLog(int maxPassedLevel, AdType type, string adWhere)
        {
            PlayerParams.MaxPassedLevel = maxPassedLevel;
            AdsLog(type, adWhere);
        }
        
        public void AdsLog(AdType type, string adWhere)
        {
            if (type.Equals(AdType.Interstitial))
            {
                PlayerParams.InterstitialAdCount++;
            }
            else
            {
                PlayerParams.RewardAdCount++;
            }
            long now = FTime.CurrentTimeMillis();
            PrintCacheRequest(nameof(AdsLog), type.ToString(), adWhere);
            WaitInit(() => new DwhAdLog(type, adWhere, now));
        }
        
        public void AdsLog(AdType type, string adWhere, string adPrecision, string adCountry, double adRev, string adNetwork, string adMediation)
        {
            if (type.Equals(AdType.Interstitial))
            {
                PlayerParams.InterstitialAdCount++;
            }
            else
            {
                PlayerParams.RewardAdCount++;
            }
            long now = FTime.CurrentTimeMillis();

            PrintCacheRequest(nameof(AdsLog), type.ToString(), adWhere, adPrecision, adCountry, adRev, adNetwork, adMediation);
            WaitInit(() => new DwhAdLog(type, adWhere, adPrecision, adCountry, adRev, adNetwork, adMediation, now));
        }

        public void DefineLog(string type, string jsonContent)
        {
            long now = FTime.CurrentTimeMillis();
            PrintCacheRequest(nameof(AdsLog), type, jsonContent);
            WaitInit(() => new DwhDefineLog(type, jsonContent, now));
        }

        public void FunnelLog(int maxPassedLevel, string funnelName, string action, int priority)
        {
            PlayerParams.MaxPassedLevel = maxPassedLevel;
            FunnelLog(funnelName, action, priority);
        }
        
        public void FunnelLog(string funnelName, string action, int priority)
        {
            long now = FTime.CurrentTimeMillis();
            PrintCacheRequest(nameof(FunnelLog), funnelName, action, priority);
            WaitInit(() => new DwhFunnelLog(funnelName, action, priority, now));
        }

        public void InAppLog(int maxPassedLevel, string productId, string currencyCode, string price,
            string transactionId, string purchaseToken, string where)
        {
            PlayerParams.MaxPassedLevel = maxPassedLevel;
            InAppLog(productId, currencyCode, price, transactionId, purchaseToken, where);
        }
        
        public void InAppLog(string productId, string currencyCode, string price, string transactionId,
            string purchaseToken, string where)
        {
            long now = FTime.CurrentTimeMillis();
            PrintCacheRequest(nameof(InAppLog), productId, currencyCode, price, transactionId, purchaseToken, where);
            WaitInit(() => new DwhInAppLog(productId, currencyCode, price, transactionId, purchaseToken, where,now));
        }
        
        public void InAppLog(string productId, string currencyCode, decimal price, string transactionId,
            string purchaseToken, string where)
        {
            long now = FTime.CurrentTimeMillis();
            PrintCacheRequest(nameof(InAppLog), productId, currencyCode, price, transactionId, purchaseToken, where);
            WaitInit(() => new DwhInAppLog(productId, currencyCode, price, transactionId, purchaseToken, where, now));
        }

        public void LevelLog(int level, int duration, int wave, string difficulty,
            LevelStatus status)
        {
            LevelLog(level, TimeSpan.FromSeconds(duration), wave, difficulty, status);
        }
        
        public void LevelLog(int level, TimeSpan duration, int wave, string difficulty, LevelStatus status)
        {
            long now = FTime.CurrentTimeMillis();
            PrintCacheRequest(nameof(LevelLog), level, (int)duration.TotalSeconds, wave, difficulty, status.ToString());
            WaitInit(() => new DwhLevelLog(level, (int)duration.TotalSeconds, wave, difficulty, status, now));
        }

        public void PropertyLog(int maxPassedLevel, string name, string value, int priority)
        {
            PlayerParams.MaxPassedLevel = maxPassedLevel;
            PropertyLog(name, value, priority);
        }
        
        public void PropertyLog(string name, string value, int priority)
        {
            long now = FTime.CurrentTimeMillis();
            PrintCacheRequest(nameof(PropertyLog), name, value, priority);
            WaitInit(() => new DwhPropertyLog(name, value, priority, now));
        }

        public void ResourceLog(int maxPassedLevel, FlowType flowType, string itemType, string itemId, string currency,
            long amount)
        {
            PlayerParams.MaxPassedLevel = maxPassedLevel;
            ResourceLog(flowType, itemType, itemId, currency, amount);
        }

        public void ResourceLog(FlowType flowType, string itemType, string itemId, string currency, long amount)
        {
            PrintCacheRequest(nameof(ResourceLog), flowType.ToString(), itemType, itemId, currency, amount);
            ResourceCounter.Instance.Summarize(flowType, itemType, itemId, currency, amount);
        }
        
        public void SessionLog(int maxPassedLevel, int sessionTime, string gameMode)
        {
            PlayerParams.MaxPassedLevel = maxPassedLevel;
            SessionLog(TimeSpan.FromSeconds(sessionTime), gameMode);
        }

        public void SessionLog(int sessionTime, string gameMode)
        {
            SessionLog(TimeSpan.FromSeconds(sessionTime), gameMode);
        }

        public void SessionLog(int maxPassedLevel, TimeSpan sessionTime, string gameMode)
        {
            PlayerParams.MaxPassedLevel = maxPassedLevel;
            SessionLog(sessionTime, gameMode);
        }

        public void SessionLog(TimeSpan sessionTime, string gameMode)
        {
            long now = FTime.CurrentTimeMillis();
            PrintCacheRequest(nameof(SessionLog), sessionTime.TotalSeconds, gameMode);
            WaitInit(() => new DwhSessionLog((int)sessionTime.TotalSeconds, gameMode, now));
        }

        private void WaitInit(Func<DwhMessage> actionMessage)
        {
            new WaitInit(() =>
            {
                try
                {
                    MessageSender.Instance.Enqueue(actionMessage.Invoke());
                }
                catch (DwhMessageException e)
                {
                    AnalyticLogger.Instance.Error(e);
                }
            }).Schedule();
        }

        private void PrintCacheRequest(string function, params object[] args)
        {
            AnalyticLogger.Instance.Info("DwhLog => " + function + " " + JsonConvert.SerializeObject(args));
        }
    }
}