using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Soundboard.Model
{
    class SoundList
    {
        public List<Sound> MakeSoundList()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Sounds.xml"))
            {
                try
                {
                    TextReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "Sounds.xml");
                    XmlSerializer deserializer = new XmlSerializer(typeof(List<Sound>));
                    object obj = deserializer.Deserialize(reader);
                    reader.Close();
                    return (List<Sound>)obj;
                }
                catch (System.InvalidOperationException e)
                {
                    System.Console.WriteLine(e);
                    return new List<Sound>();
                }
            }
            return new List<Sound>();
        }

        public void Serialize(List<Sound> sounds)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Sound>));
            using (TextWriter writer = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Sounds.xml"))
            {
                serializer.Serialize(writer, sounds);
            }
        }
    }
}
