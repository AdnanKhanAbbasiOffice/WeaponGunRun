// using Falcon.FalconAnalytics.Scripts.Message.DWH;
// using Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Model;
// using UnityEngine;
//
// namespace Falcon.FalconAnalytics.Scripts.Message.Wrapper
// {
//     public class ActionLogWrapper : DataWrapper
//     {
//         private static ActionLogConfig _config;
//
//         [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
//         public static void AssignConfigAfterInit()
//         {
//             FalconConfig.OnUpdateFromNet += (sender, args) =>
//             {
//                 _config = FalconConfig.Instance<ActionLogConfig>();
//             };
//         }
//         
//         public ActionLogWrapper(string messageType, string url, string data, int failCount) : base(messageType, url, data, failCount)
//         {
//         }
//
//         public ActionLogWrapper(DwhActionLog message, string url) : base(message, url)
//         {
//         }
//
//         public override void Send()
//         {
//             if (_config != null && _config.fCoreAnalyticActionLogEnable)
//             {
//                 base.Send();
//             }
//             else
//             {
//                 AnalyticLogger.Instance.Error("Action log not enabled, log not send to server");
//             }
//         }
//     }
// }