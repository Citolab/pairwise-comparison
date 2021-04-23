using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Cito.Cat.Core.Helpers
{
    public class GenericXmlSerializer<T> : XmlSerializer where T : class

    {
        public GenericXmlSerializer() : base(typeof(T))
        {
        }

        public new T Deserialize(Stream stream)
        {
            return base.Deserialize(stream) as T;
        }

        public T Deserialize(string input)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
            return base.Deserialize(stream) as T;
        }
    }
}