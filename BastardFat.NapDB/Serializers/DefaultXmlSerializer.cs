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
        public string GetSignature(byte[] content)
        {
            using (MD5 md5 = MD5.Create())
                return BitConverter.ToString(md5.ComputeHash(content)).Replace("-", "");
        }

        TEntity IEntitySerializer<TKey>.Deserialize<TEntity>(byte[] content)
        {
            var xmlstring = Encoding.Unicode.GetString(content);
            var serializer = new XmlSerializer(typeof(TEntity));
            using (var stringReader = new StringReader(xmlstring))
                return serializer.Deserialize(stringReader) as TEntity;
        }

        byte[] IEntitySerializer<TKey>.Serialize<TEntity>(TEntity model)
        {
            var serializer = new XmlSerializer(typeof(TEntity));
            using (var stringwriter = new StringWriter())
            {
                serializer.Serialize(stringwriter, model);
                return Encoding.Unicode.GetBytes(stringwriter.ToString());
            }
        }
    }
}
