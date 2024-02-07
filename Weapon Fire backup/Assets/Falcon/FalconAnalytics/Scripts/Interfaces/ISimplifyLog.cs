using System;
using Falcon.FalconAnalytics.Scripts.Enum;

namespace Falcon.FalconAnalytics.Scripts.Interfaces
{
    /// <summary>
    /// Simplified Log interface, containing functions corresponding to <see cref="ILog"/>
    /// but some value is collected during the log process and therefore no longer needed to be manually input.
    /// </summary>
    public interface ISimplifyLog
    {
        /// <summary>
        /// Log the event the player has just switch between two action/event/scene/panel/etc.
        /// This is a type of log designed for order tracking and user behavior analysis.
        /// </summary>
        /// <param name="from">The action the player switch from</param>
        /// <param name="to">The action the player switch to</param>
        /// <param name="time">(in second)The amount of time the player spent for the from action before switching</param>
        /// <param name="priority">The priority of the action(For the server to sort and set up diagram)</param>
        /// <remarks>The time must be in second</remarks>
        void ActionLog(string from, string to, int time, int priority);
        
        /// <summary>
        /// Log the event the player has just switch between two action/event/scene/panel/etc.
        /// This is a type of log designed for order tracking and user behavior analysis.
        /// </summary>
        /// <param name="from">The action the player switch from</param>
        /// <param name="to">The action the player switch to</param>
        /// <param name="time">The amount of time the player spent for the from action before switching</param>
        /// <param name="priority">The priority of the action(For the server to sort and set up diagram)</param>
        void ActionLog(string from, string to, TimeSpan time, int priority);
        
        /// <summary>
        /// Log the event the player has just watched an advertisement
        /// </summary>
        /// <param name="type">The type of the ad</param>
        /// <param name="adWhere">(2&lt;=strLen&lt;=50) Briefly describe the situation where the user watches the ad</param>
        void AdsLog(AdType type, string adWhere);
        
        /// <summary>
        /// Log event the player achieves a certain achievement, feature, level, etc.
        /// This is a type of log designed to classify users.
        /// </summary>
        /// <param name="funnelName">(2&lt;=strLen&lt;=200) The name of the funnel</param>
        /// <param name="action">(2&lt;=strLen&lt;=200) The level name in the funnel</param>
        /// <param name="priority"> The level order in the funnel</param>
        /// <remarks>
        /// Funnel log execution on a device must be executed in the increasing order of priority, starting from 0;
        /// this also means that the funnelName must be classified in a consecutive and non-duplicating integer order.
        /// If the funnel log hasn't been executed in order, a debug.warning will be used instead of sending to server(not throwing any exception).
        /// The SDK will automatically store and check funnelName and priority, so no user pre-check needed.
        /// </remarks>
        void FunnelLog(string funnelName, string action, int priority);
        
        /// <summary>
        /// Log the event the player has purchase an in-app package
        /// </summary>
        /// <param name="productId">The self-define id of the package</param>
        /// <param name="currencyCode">The currency the player used</param>
        /// <param name="price">The price of the package</param>
        /// <param name="transactionId">The transactionId</param>
        /// <param name="purchaseToken">The token which Google Play return after the purchase, input "" for other platform</param>
        /// <param name="where">Briefly describe the situation where the user purchase</param>
        void InAppLog(string productId, string currencyCode, string price, string transactionId,
            string purchaseToken, string where);
        
        /// <summary>
        /// Log the event when the player end a level, whether fail or pass.
        /// </summary>
        /// <param name="level">The level that the player is playing</param>
        /// <param name="duration">(in second, 5secs&lt;=value&lt;=2days)The amount of time that the player took in order to pass the level,calculated by second</param>
        /// <param name="wave">The number of waves the player has passed during the level(or 0, if the level has no wave) </param>
        /// <param name="difficulty">(2&lt;=strLen&lt;=20)The difficulty of the level</param>
        /// <param name="status">The status of the user when ending the level, either fail or pass</param>
        /// <remarks>
        /// The Log only executed when maxPassedLevel is a new level.
        /// If the user already passed the level before, a debug.warning will be used instead of sending to server(not throwing any exception).
        /// The SDK will automatically store and check the maxPassedLevel, so no user pre-check needed.
        /// </remarks>
        void LevelLog(int level, int duration, int wave, string difficulty, LevelStatus status);

        /// <summary>
        /// Log the event when the player end a level, whether fail or pass.
        /// </summary>
        /// <param name="level">The level that the player is playing</param>
        /// <param name="duration">(5secs&lt;=value&lt;=2days)The amount of time that the player took in order to pass the level,calculated by second</param>
        /// <param name="wave">The number of waves the player has passed during the level(or 0, if the level has no wave) </param>
        /// <param name="difficulty">(2&lt;=strLen&lt;=20)The difficulty of the level</param>
        /// <param name="status">The status of the user when ending the level, either fail or pass</param>
        /// <remarks>
        /// The Log only executed when maxPassedLevel is a new level.
        /// If the user already passed the level before, a debug.warning will be used instead of sending to server(not throwing any exception).
        /// The SDK will automatically store and check the maxPassedLevel, so no user pre-check needed.
        /// </remarks>
        void LevelLog(int level, TimeSpan duration, int wave, string difficulty, LevelStatus status);
        
        /// <summary>
        /// Log the event when the user unlock an event/feature/item/etc.
        /// This is a log type designed for users to dynamically define the values they want to aggregate.
        /// </summary>
        /// <param name="name">The type name of the unlocked entity</param>
        /// <param name="value">The unlocked entity value</param>
        /// <param name="priority">The order of the priority value in the type </param>
        void PropertyLog(string name, string value, int priority);

        /// <summary>
        /// Log the event when the player get/spend any type of resource 
        /// </summary>
        /// <param name="flowType">Source(get) or Sink(spend)</param>
        /// <param name="itemType">(2&lt;=strLen&lt;=20)The reason of the resource(in general)</param>
        /// <param name="itemId">(2&lt;=strLen&lt;=20)The reason of the resource(in specific)</param>
        /// <param name="currency">(2&lt;=strLen&lt;=20)The type of resource the user get/spend</param>
        /// <param name="amount">The resource count the user get/spend</param>
        void ResourceLog(FlowType flowType, string itemType, string itemId, string currency, long amount);
        
        /// <summary>
        /// Log the duration measured that the player used for a feature
        /// </summary>
        /// <param name="sessionTime">(in second) The amount of time the user used for the feature</param>
        /// <param name="gameMode">The feature name</param>
        void SessionLog(int sessionTime, string gameMode);

        /// <summary>
        /// Log the duration measured that the player used for a feature
        /// </summary>
        /// <param name="sessionTime">The amount of time the user used for the feature</param>
        /// <param name="gameMode">The feature name</param>
        void SessionLog(TimeSpan sessionTime, string gameMode);
    }
}