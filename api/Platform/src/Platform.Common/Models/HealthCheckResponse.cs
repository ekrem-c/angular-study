namespace Platform.Common
{
    public class HealthCheckResponse
    {
        public string ResourceName { get; set; }

        public HealthCheckStatus Status { get; set; }

        public string ErrorStackTrace { get; set; }
    }
}