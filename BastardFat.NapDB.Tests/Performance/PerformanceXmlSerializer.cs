using BastardFat.NapDB.Serializers;

namespace BastardFat.NapDB.Tests.Performance
{
    public class PerformanceXmlSerializer<TKey> : DefaultXmlSerializer<TKey>
    {
        public PerformanceXmlSerializer() : base()
        {
            Measurer = new PerformanceMeasurer();
        }
        public PerformanceMeasurer Measurer { get; }
        public override TEntity Deserialize<TEntity>(byte[] content)
        {
            return Measurer.Measure(() => base.Deserialize<TEntity>(content));
        }

        public override string GetSignature(byte[] content)
        {
            return Measurer.Measure(() => base.GetSignature(content));
        }

        public override byte[] Serialize<TEntity>(TEntity model)
        {
            return Measurer.Measure(() => base.Serialize(model));
        }
    }
}
