namespace Falcon.FalconAnalytics.Scripts
{
    public static class DwhConstants
    {
        private const string Domain = "https://dwhapi.data4game.com";
        // private const string Domain = "http://localhost:8389";

        public const string ActionApi = Domain + "/action-log";
        public const string AdApi = Domain + "/ads-log";
        public const string FunnelApi = Domain + "/funnel-log";
        public const string InAppApi = Domain + "/iap-log";
        public const string LevelApi = Domain + "/level-log";
        public const string PropertyApi = Domain + "/property-log";
        public const string ResourceApi = Domain + "/resource-log";
        public const string RetentionApi = Domain + "/retention-log";
        public const string SessionApi = Domain + "/session-log";
        
        public const string ExceptionApi = Domain + "/exception-log";
        public const string DefineLogApi = Domain + "/define-log";
    }
}
