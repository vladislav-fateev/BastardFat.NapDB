using BastardFat.NapDB.Abstractions;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BastardFat.NapDB.Tests.Relations.OneToMany
{
    public class OneToManyTest_Company : IEntity<int>
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int PhoneId { get; set; }
        public virtual OneToManyTest_Phone Phone { get; set; }

        public virtual int ParentId { get; set; }
        public virtual OneToManyTest_Company Parent { get; set; }
    }

    public class OneToManyTest_Phone : IEntity<int>
    {
        public virtual int Id { get; set; }
        public virtual string Number { get; set; }
        public virtual string AdditionalNumber { get; set; }

        [XmlIgnore]
        public virtual IEnumerable<OneToManyTest_Company> Companies { get; set; }
    }
}
