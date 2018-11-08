using BastardFat.NapDB.Abstractions;
using System;

namespace BastardFat.NapDB.Tests.Relations.OneToMany
{
    public class OneToManyTest_Company : IEntity<int>
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int PhoneId { get; set; }
        public virtual OneToManyTest_Phone Phone { get; set; }
    }

    public class OneToManyTest_Phone : IEntity<int>
    {
        public virtual int Id { get; set; }
        public virtual string Number { get; set; }
        public virtual string AdditionalNumber { get; set; }
    }
}
