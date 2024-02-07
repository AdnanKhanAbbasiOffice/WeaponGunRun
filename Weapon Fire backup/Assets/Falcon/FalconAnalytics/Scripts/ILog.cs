using System;
using Falcon.FalconAnalytics.Scripts.Enum;

namespace Falcon.FalconAnalytics.Scripts
{
    /// <summary>
    ///     Basic Log interface, containing function requirements for sending data to the server for later analysis.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        ///     Log the event the player has just switch between two action/event/scene/panel/etc.
        ///     This is a type of log designed for order tracking and user behavior analysis.
        /// </summary>
        /// <param name="actionName">(1 ≤ strLen ≤ 50) The action the player switch from</param>
        void ActionLog(string actionName);

        /// <summary>
        ///     Log the event the player has just watched an advertisement.
        /// </summary>
        /// <param name="maxPassedLevel">(≥0) The max level that the player has passed</param>
        /// <param name="type">The type of the ad</param>
        /// <param name="adWhere">(1 ≤ strLen ≤ 50) Briefly describe the situation where the user watches the ad</param>
        void AdsLog(int maxPassedLevel, AdType type, string adWhere);

        /// <summary>
        ///     Log the event the player has just watched an advertisement.
        /// </summary>
        /// <param name="type">The type of the ad</param>
        /// <param name="adWhere">(1 ≤ strLen ≤ 50) Briefly describe the situation where the user watches the ad</param>
        void AdsLog(AdType type, string adWhere);

        /// <summary>
        /// Log the event the player has just watched an advertisement.
        /// </summary>
        /// <param name="type">The type of the ad</param>
        /// <param name="adWhere">(1 ≤ strLen ≤ 50) Briefly describe the situation where the user watches the ad</param>
        /// <param name="adPrecision"></param>
        /// <param name="adCountry"></param>
        /// <param name="adRev"></param>
        /// <param name="adNetwork"></param>
        /// <param name="adMediation"></param>
        void AdsLog(AdType type, string adWhere, string adPrecision, string adCountry, double adRev, string adNetwork,
            string adMediation);

        /// <summary>
        ///  Log any thing that is predefined
        /// </summary>
        /// <param name="type">Type of the log</param>
        /// <param name="jsonContent">The content of the log in json format</param>
        void DefineLog(string type, string jsonContent);

        /// <summary>
        ///     Log event the player achieves a certain achievement, feature, level, etc.
        ///     This is a type of log designed to classify users.
        /// </summary>
        /// <param name="maxPassedLevel">(≥0) The max level that the player has passed</param>
        /// <param name="funnelName">(1 ≤ strLen ≤ 200) The name of the funnel</param>
        /// <param name="action">(1 ≤ strLen ≤ 200) The level name in the funnel</param>
        /// <param name="priority">(≥0) The level order in the funnel</param>
        /// <remarks>
        ///     Funnel log execution on a device must be executed in the increasing order of priority, starting from 0;
        ///     this also means that the funnelName must be classified in a consecutive and non-duplicating integer order.
        ///     If the funnel log hasn't been executed in order, a debug.warning will be used instead of sending to server(not
        ///     throwing any exception).
        ///     The SDK will automatically store and check funnelName and priority, so no user pre-check needed.
        /// </remarks>
        void FunnelLog(int maxPassedLevel, string funnelName, string action, int priority);

        /// <summary>
        ///     Log event the player achieves a certain achievement, feature, level, etc.
        ///     This is a type of log designed to classify users.
        /// </summary>
        /// <param name="funnelName">(1 ≤ strLen ≤ 200) The name of the funnel</param>
        /// <param name="action">(1 ≤ strLen ≤ 200) The level name in the funnel</param>
        /// <param name="priority">(≥0) The level order in the funnel</param>
        /// <remarks>
        ///     Funnel log execution on a device must be executed in the increasing order of priority, starting from 0;
        ///     this also means that the funnelName must be classified in a consecutive and non-duplicating integer order.
        ///     If the funnel log hasn't been executed in order, a debug.warning will be used instead of sending to server(not
        ///     throwing any exception).
        ///     The SDK will automatically store and check funnelName and priority, so no user pre-check needed.
        /// </remarks>
        void FunnelLog(string funnelName, string action, int priority);

        /// <summary>
        ///     Log the event the player has purchase an in-app package.
        /// </summary>
        /// <param name="maxPassedLevel">(≥0) The max level that the player has passed</param>
        /// <param name="productId">(1 ≤ strLen ≤ 100) The self-define id of the package</param>
        /// <param name="currencyCode">(1 ≤ strLen ≤ 20) The currency the player used</param>
        /// <param name="price">(1 ≤ strLen ≤ 20) The price of the package</param>
        /// <param name="transactionId">The transactionId</param>
        /// <param name="purchaseToken">The token which Google Play return after the purchase, input "" for other platform</param>
        /// <param name="where">(1 ≤ strLen ≤ 200)Briefly describe the situation where the user purchase</param>
        void InAppLog(int maxPassedLevel, string productId, string currencyCode, string price, string transactionId,
            string purchaseToken, string where);

        /// <summary>
        ///     Log the event the player has purchase an in-app package.
        /// </summary>
        /// <param name="productId">(1 ≤ strLen ≤ 100) The self-define id of the package</param>
        /// <param name="currencyCode">(1 ≤ strLen ≤ 20) The currency the player used</param>
        /// <param name="price">(1 ≤ strLen ≤ 20) The price of the package</param>
        /// <param name="transactionId">The transactionId</param>
        /// <param name="purchaseToken">The token which Google Play return after the purchase, input "" for other platform</param>
        /// <param name="where">(1 ≤ strLen ≤ 200)Briefly describe the situation where the user purchase</param>
        [Obsolete(
        "Must ensure price in right format, use InAppLog(string productId, string currencyCode, decimal price, string transactionId,string purchaseToken, string where) instead")]
        void InAppLog(string productId, string currencyCode, string price, string transactionId,
            string purchaseToken, string where);
        
        /// <summary>
        ///     Log the event the player has purchase an in-app package.
        /// </summary>
        /// <param name="productId">(1 ≤ strLen ≤ 100) The self-define id of the package</param>
        /// <param name="currencyCode">(1 ≤ strLen ≤ 20) The currency the player used</param>
        /// <param name="price">(≥0) The price of the package</param>
        /// <param name="transactionId">The transactionId</param>
        /// <param name="purchaseToken">The token which Google Play return after the purchase, input "" for other platform</param>
        /// <param name="where">(1 ≤ strLen ≤ 200)Briefly describe the situation where the user purchase</param>
        void InAppLog(string productId, string currencyCode, decimal price, string transactionId,
            string purchaseToken, string where);

        /// <summary>
        ///     Log the event when the player end a level, whether fail or pass.
        /// </summary>
        /// <param name="level">(≥0) The level that the player is playing</param>
        /// <param name="duration">
        ///     (in second, 5secs ≤ value ≤ 2days) The amount of time that the player took in order to pass
        ///     the level,calculated by second
        /// </param>
        /// <param name="wave">(≥0) The number of waves the player has passed during the level(or 0, if the level has no wave) </param>
        /// <param name="difficulty">(1 ≤ strLen ≤ 20) The difficulty of the level</param>
        /// <param name="status">The status of the user when ending the level, either fail or pass</param>
        /// <remarks>
        ///     The Log only executed when maxPassedLevel is a new level.
        ///     If the user already passed the level before, a debug.error will be used instead of sending to server(not throwing
        ///     any exception).
        ///     The SDK will automatically store and check the maxPassedLevel, so no user pre-check needed.
        /// </remarks>
        [Obsolete(
            "Must ensure duration is in second, use LevelLog(int level, TimeSpan duration, int wave, string difficulty, LevelStatus status) instead")]
        void LevelLog(int level, int duration, int wave, string difficulty, LevelStatus status);

        /// <summary>
        ///     Log the event when the player end a level, whether fail or pass.
        /// </summary>
        /// <param name="level">(≥0) The level that the player is playing</param>
        /// <param name="duration">
        ///     (in second, 5secs ≤ value ≤ 2days) The amount of time that the player took in order to pass
        ///     the level,calculated by second
        /// </param>
        /// <param name="wave">(≥0) The number of waves the player has passed during the level(or 0, if the level has no wave) </param>
        /// <param name="difficulty">(1 ≤ strLen ≤ 20) The difficulty of the level</param>
        /// <param name="status">The status of the user when ending the level, either fail or pass</param>
        /// <remarks>
        ///     The Log only executed when maxPassedLevel is a new level.
        ///     If the user already passed the level before, a debug.error will be used instead of sending to server(not throwing
        ///     any exception).
        ///     The SDK will automatically store and check the maxPassedLevel, so no user pre-check needed.
        /// </remarks>
        void LevelLog(int level, TimeSpan duration, int wave, string difficulty, LevelStatus status);

        /// <summary>
        ///     Log the event when the user unlock an event/feature/item/etc.
        ///     This is a log type designed for users to dynamically define the values they want to aggregate.
        /// </summary>
        /// <param name="maxPassedLevel">(≥0) The max level that the player has passed</param>
        /// <param name="name">(1 ≤ strLen ≤ 50) The type name of the unlocked entity</param>
        /// <param name="value">(1 ≤ strLen ≤ 50) The unlocked entity value</param>
        /// <param name="priority">(≥0) The order of the priority value in the type </param>
        void PropertyLog(int maxPassedLevel, string name, string value, int priority);

        /// <summary>
        ///     Log the event when the user unlock an event/feature/item/etc.
        ///     This is a log type designed for users to dynamically define the values they want to aggregate.
        /// </summary>
        /// <param name="name">(1 ≤ strLen ≤ 50) The type name of the unlocked entity</param>
        /// <param name="value">(1 ≤ strLen ≤ 50) The unlocked entity value</param>
        /// <param name="priority">(≥0) The order of the priority value in the type </param>
        void PropertyLog(string name, string value, int priority);

        /// <summary>
        ///     Log the event when the player get/spend any type of resource.
        /// </summary>
        /// <param name="maxPassedLevel">(≥0) The max level that the player has passed</param>
        /// <param name="flowType">Source(get) or Sink(spend)</param>
        /// <param name="itemType">(1 ≤ strLen ≤ 50) The reason of the resource(in general)</param>
        /// <param name="itemId">(1 ≤ strLen ≤ 50) The reason of the resource(in specific)</param>
        /// <param name="currency">(1 ≤ strLen ≤ 50) The type of resource the user get/spend</param>
        /// <param name="amount">(≥0) The resource count the user get/spend</param>
        void ResourceLog(int maxPassedLevel, FlowType flowType, string itemType, string itemId, string currency,
            long amount);

        /// <summary>
        ///     Log the event when the player get/spend any type of resource.
        /// </summary>
        /// <param name="flowType">Source(get) or Sink(spend)</param>
        /// <param name="itemType">(1 ≤ strLen ≤ 50) The reason of the resource(in general)</param>
        /// <param name="itemId">(1 ≤ strLen ≤ 50) The reason of the resource(in specific)</param>
        /// <param name="currency">(1 ≤ strLen ≤ 50) The type of resource the user get/spend</param>
        /// <param name="amount">(≥0) The resource count the user get/spend</param>
        void ResourceLog(FlowType flowType, string itemType, string itemId, string currency,
            long amount);

        /// <summary>
        ///     Log the duration measured that the player used for a feature.
        /// </summary>
        /// <param name="maxPassedLevel">(≥0) The max level that the player has passed</param>
        /// <param name="sessionTime">(in second, 5 ≤ sessionTime ≤ 172800) The amount of time the user used for the feature</param>
        /// <param name="gameMode">The feature name</param>
        [Obsolete(
            "Must ensure sessionTime is in second, use SessionLog(int maxPassedLevel, TimeSpan sessionTime, string gameMode) instead")]
        void SessionLog(int maxPassedLevel, int sessionTime, string gameMode);

        /// <summary>
        ///     Log the duration measured that the player used for a feature.
        /// </summary>
        /// <param name="sessionTime">(in second, 5 ≤ sessionTime ≤ 172800) The amount of time the user used for the feature</param>
        /// <param name="gameMode">(1 ≤ strLen ≤ 100) The feature name</param>
        [Obsolete(
            "Must ensure sessionTime is in second, use SessionLog(int maxPassedLevel, TimeSpan sessionTime, string gameMode) instead")]
        void SessionLog(int sessionTime, string gameMode);

        /// <summary>
        ///     Log the duration measured that the player used for a feature.
        /// </summary>
        /// <param name="maxPassedLevel">(≥0) The max level that the player has passed</param>
        /// <param name="sessionTime">(5sec ≤ sessionTime ≤ 2days) The amount of time the user used for the feature</param>
        /// <param name="gameMode">(1 ≤ strLen ≤ 100) The feature name</param>
        void SessionLog(int maxPassedLevel, TimeSpan sessionTime, string gameMode);

        /// <summary>
        ///     Log the duration measured that the player used for a feature.
        /// </summary>
        /// <param name="sessionTime">(5sec ≤ sessionTime ≤ 2days) The amount of time the user used for the feature</param>
        /// <param name="gameMode">(1 ≤ strLen ≤ 100) The feature name</param>
        void SessionLog(TimeSpan sessionTime, string gameMode);
    }
}