using System.Xml.Serialization;

namespace Sitatex.Logging.Models
{
    [XmlRoot(ElementName = "appender")]
    public class Appender
    {
        [XmlElement(ElementName = "file")]
        public LogFile File { get; set; }
        [XmlElement(ElementName = "appendToFile")]
        public AppendToFile AppendToFile { get; set; }
        [XmlElement(ElementName = "rollingStyle")]
        public RollingStyle RollingStyle { get; set; }
        [XmlElement(ElementName = "maxSizeRollBackups")]
        public MaxSizeRollBackups MaxSizeRollBackups { get; set; }
        [XmlElement(ElementName = "maximumFileSize")]
        public MaximumFileSize MaximumFileSize { get; set; }
        [XmlElement(ElementName = "staticLogFileName")]
        public StaticLogFileName StaticLogFileName { get; set; }
        [XmlElement(ElementName = "layout")]
        public Layout Layout { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }
}
