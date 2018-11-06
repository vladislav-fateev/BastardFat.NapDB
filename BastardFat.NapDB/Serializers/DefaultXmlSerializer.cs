using BastardFat.NapDB.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BastardFat.NapDB.Serializers
{
    public class DefaultXmlSerializer<TKey> : IEntitySerializer<TKey>
    {
        public virtual string GetSignature(byte[] content)
        {
            using (MD5 md5 = MD5.Create())
                return BitConverter.ToString(md5.ComputeHash(content)).Replace("-", "");
        }

        public virtual TEntity Deserialize<TEntity>(byte[] content)
            where TEntity : class, IEntity<TKey>, new()
        {
            var xmlstring = Encoding.Unicode.GetString(content);
            var serializer = GetXmlSerializer(typeof(TEntity));
            using (var stringReader = new StringReader(xmlstring))
                return serializer.Deserialize(stringReader) as TEntity;
        }

        public virtual byte[] Serialize<TEntity>(TEntity model)
            where TEntity : class, IEntity<TKey>, new()
        {
            var serializer = GetXmlSerializer(typeof(TEntity));
            using (var stringwriter = new StringWriter())
            {
                serializer.Serialize(stringwriter, model);
                return Encoding.Unicode.GetBytes(stringwriter.ToString());
            }
        }

        private static Dictionary<Type, XmlSerializer> _serializerInstances = new Dictionary<Type, XmlSerializer>();
        private XmlSerializer GetXmlSerializer(Type type)
        {
            if (!_serializerInstances.ContainsKey(type))
                lock (_serializerInstances)
                    if (!_serializerInstances.ContainsKey(type))
                        _serializerInstances[type] = new XmlSerializer(type);
            return _serializerInstances[type];
        }
    }
}
