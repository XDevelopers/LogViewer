namespace Sitatex.Logging.Extensions
{
    using Newtonsoft.Json;
    using Sitatex.Logging.ViewModels;

    public static class SerializeLog
    {
        public static string ToJson(this LogEntry self)
        {
            return JsonConvert.SerializeObject(self, Sitatex.Logging.Extensions.LogConverter.Settings);
        }
    }
}
