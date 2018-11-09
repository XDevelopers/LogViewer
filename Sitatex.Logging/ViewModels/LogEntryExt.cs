using Newtonsoft.Json;

namespace Sitatex.Logging.ViewModels
{
    public partial class LogEntry
    {
        public static LogEntry FromJson(string json)
        {
            return JsonConvert.DeserializeObject<LogEntry>(json, Sitatex.Logging.Extensions.LogConverter.Settings);
        }
    }
}
