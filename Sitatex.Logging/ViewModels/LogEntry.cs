namespace Sitatex.Logging.ViewModels
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public partial class LogEntry
    {
        [JsonProperty("EntryDate")]
        public string EntryDate { get; set; }

        [JsonProperty("Level")]
        public string Level { get; set; }

        [JsonProperty("Logger")]
        public string Logger { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("Location")]
        public string Location { get; set; }

        [JsonProperty("Exception")]
        public string Exception { get; set; }

        //"Properties":"{log4net:Identity=, log4net:UserName=DOM_QP\m.lourenco, log4net:HostName=CONDOR}",
        [JsonProperty("Properties")]
        public Dictionary<string, string> Properties { get; set; }
    }
}
