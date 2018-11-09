using System.Xml.Serialization;

namespace Sitatex.Logging.Models
{
    [XmlRoot(ElementName = "maxSizeRollBackups")]
    public class MaxSizeRollBackups
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

}
