using Newtonsoft.Json.Linq;

namespace Platform.Common
{
    public static class JTokenExtensions
    {
        public static bool IsNullOrEmpty(this JToken token)
        {
            return token == null ||
                   token.Type == JTokenType.Array && !token.HasValues ||
                   token.Type == JTokenType.Object && !token.HasValues ||
                   token.Type == JTokenType.String && token.ToString() == string.Empty ||
                   token.Type == JTokenType.Null;
        }

        public static bool IsNullOrEmptyString(this JToken token)
        {
            return token == null || string.IsNullOrWhiteSpace(token.ToString());
        }
    }
}