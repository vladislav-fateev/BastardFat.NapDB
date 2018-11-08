using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Metadatas;

namespace BastardFat.NapDB.Tests.Keytypes
{
    public class GuidKeyTestEntity : IEntity<string>
    {
        public virtual string Id { get; set; } = GuidMetadata.GetDefaultId();

        public virtual string Data { get; set; }
    }
}
