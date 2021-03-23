using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Soundboard.Model
{
    public class Sound
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Path")]
        public string FilePath { get; set; }

        public Key KeyOne;

        public Key KeyTwo;
    }
}
