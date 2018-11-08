using BastardFat.NapDB.Abstractions;
using System;

namespace BastardFat.NapDB.Tests.Smoke
{
    public class SmokeTestEntity : IEntity<int>
    {
        public virtual int Id { get; set; }
        public virtual string Data { get; set; }
        public virtual DateTime Created { get; set; }
    }
}
