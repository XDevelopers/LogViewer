namespace Sitatex.Logging.Models
{
    public class LogConfig
    {
        public int MaximumFileSize { get; set; } = 2;

        public string FilenamePattern { get; set; } = "SitatexIP-%date{yyyy-MM}.log";

        public string FilePath { get; set; } = $@"LogFiles\";

        public string PatternLayout { get; set; } = "%date | [%thread] | %level | %logger - %message%newline";
    }
}
