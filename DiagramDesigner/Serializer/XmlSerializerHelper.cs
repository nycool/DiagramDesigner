using System.IO;
using System.Xml;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;

namespace DiagramDesigner.Serializer
{
    internal static class XmlSerializerHelper
    {
        private static IExtendedXmlSerializer _serializer;

        private static IExtendedXmlSerializer SerializerInfo =>
            _serializer ??= new ConfigurationContainer().Create();


        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        private static string Serializer<T>(T t)
        {
            return SerializerInfo.Serialize(new XmlWriterSettings() { Indent = true }, t);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static T Deserialize<T>(string xml)
        {
            return SerializerInfo.Deserialize<T>(new XmlReaderSettings() { IgnoreWhitespace = false }, xml);
        }

        public static void SerializerToPath<T>(T t, string fileName)
        {
            string xml = Serializer(t);

            if (xml != default)
            {
                File.WriteAllText(fileName, xml);
            }
        }

        public static T DeserializeFromPath<T>(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(nameof(fileName));
            }

            string xml = File.ReadAllText(fileName);

            if (xml != default)
            {
                return Deserialize<T>(xml);
            }

            return default;
        }
    }
}