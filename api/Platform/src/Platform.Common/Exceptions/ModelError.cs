using Newtonsoft.Json;

namespace Platform.Common
{
    public class ModelError
    {
        public ModelError(string type, string message) : this(0, type, message)
        {
        }

        public ModelError(int code, string message) : this(code, null, message)
        {
        }

        public ModelError(int code, string type, string message)
        {
            Code = code;
            Type = type;
            Message = message;
        }

        /// <summary>
        ///     Gets the Internal code exception.
        /// </summary>
        [JsonProperty]
        public int Code { get; set; }

        /// <summary>
        ///     Gets the type of exception.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        /// <summary>
        ///     Gets the reason of the exception.
        /// </summary>
        [JsonProperty]
        public string Message { get; set; }
    }
}