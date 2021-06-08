namespace Platform.Common
{
    /// <summary>
    ///     Handles the information about the deployment.
    /// </summary>
    public class Deployment
    {
        /// <summary>
        ///     Gets or sets Namespace name on K8S.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        ///     Gets or sets Microservice name.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        ///     Gets or sets the environment.
        /// </summary>
        /// <value>
        ///     The environment.
        /// </value>
        public string Environment { get; set; }

        /// <summary>
        ///     Gets or sets Build number.
        /// </summary>
        public string Build { get; set; }
    }
}