using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Falcon.FalconAnalytics.Scripts.Message.DWH;
using Falcon.FalconAnalytics.Scripts.Message.Wrapper;
using Falcon.FalconCore.Scripts;
using Falcon.FalconCore.Scripts.Entities;
using Falcon.FalconCore.Scripts.Utils.Data;
using Falcon.FalconCore.Scripts.Utils.FActions.Variances.Ends;
using Falcon.FalconCore.Scripts.Utils.Singletons;

namespace Falcon.FalconAnalytics.Scripts
{
    public class MessageSender : FSingleton<MessageSender>
    {
        private const string CachedRequestList = "Request_Queue";

        private readonly RepeatAction flushing ;

        private readonly FLimitQueue<DataWrapper> waitingQueue = new FLimitQueue<DataWrapper>(100);

        public MessageSender()
        {
            flushing = new RepeatAction(FlushQueue, TimeSpan.FromSeconds(5));
            flushing.Schedule();
            LoadRequests();
            FalconMain.Instance.OnGameStop += (a,b) => SaveRequest();
            FalconMain.Instance.OnGameContinue += (a,b) => LoadRequests();
        }

        private void FlushQueue()
        {
            if (waitingQueue.Count > 0)
                AnalyticLogger.Instance.Info(waitingQueue.Count + " requests is waiting in the main execute queue");

            var failCount = 0;
            DataWrapper data;
            while (waitingQueue.TryDequeue(out data) && failCount < 3)
                try
                {
                    data.Send();
                    failCount = 0;
                }
                catch (Exception e)
                {
                    failCount++;
                    if (e is AggregateException || e is TaskCanceledException)
                        AnalyticLogger.Instance.Warning("Sending message to server fail counting " + failCount);
                    else
                        AnalyticLogger.Instance.Error(e);
                    data.OnSendFail();
                    if (data.CanRetry()) waitingQueue.Enqueue(data);
                }

            if (failCount >= 3)
            {
                AnalyticLogger.Instance.Warning(
                    "Sending message fail 3 times, slowing the message send duration to 30 sec/flush");
                flushing.TimeSpan = TimeSpan.FromSeconds(30);
            }
        }

        private void SaveRequest()
        {
            var unsentData = waitingQueue.Clear();
            FData.Instance.Save(CachedRequestList, unsentData);

            AnalyticLogger.Instance.Info("Unsent requests Save success : " + unsentData.Count + " requests");
        }

        private void LoadRequests()
        {
            
            FData.Instance.ComputeIfPresent<List<DataWrapper>>(CachedRequestList, data =>
            {
                if (data != null)
                {
                    waitingQueue.EnqueueAll(data);
                    AnalyticLogger.Instance.Info("Unsent requests Load success : " + data.Count + " requests");
                }
                return null;
            });
        }

        public void Enqueue(DwhMessage message)
        {
            waitingQueue.Enqueue(message.Wrap());
        }
    }
}