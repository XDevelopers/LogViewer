using System.Xml.Serialization;

namespace Sitatex.Logging.Models
{
    [XmlRoot(ElementName = "staticLogFileName")]
    public class StaticLogFileName
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

}
