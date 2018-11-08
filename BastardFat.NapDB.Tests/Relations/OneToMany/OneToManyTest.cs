using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BastardFat.NapDB.Tests.Relations.OneToMany
{
    [TestClass]
    public class OneToManyTest
    {
        public const string Path = @"C:\dev\BastardFat.NapDB\_databases\OneToManyTestDb\";

        [TestMethod]
        public void Simple()
        {
            if (Directory.Exists(Path))
                Directory.Delete(Path, true);

            var db = new OneToManyTestDb();
            var phone1 = db.Phones.Save(new OneToManyTest_Phone { Number = "1", AdditionalNumber = "111" });
            var phone2 = db.Phones.Save(new OneToManyTest_Phone { Number = "2", AdditionalNumber = "222" });

            var c1 = db.Companies.Save(new OneToManyTest_Company { Name = "AA", PhoneId = phone1.Id });
            var c2 = db.Companies.Save(new OneToManyTest_Company { Name = "AB", PhoneId = phone1.Id });
            var c3 = db.Companies.Save(new OneToManyTest_Company { Name = "BB", PhoneId = phone2.Id });
            var c4 = db.Companies.Save(new OneToManyTest_Company { Name = "CC" });

            Assert.AreEqual("111", c1.Phone.AdditionalNumber);
            Assert.AreEqual(phone1.Number, c2.Phone.Number);
            Assert.AreEqual(c2.Phone.Number, c1.Phone.Number);
            Assert.AreEqual(phone2.Id, c3.PhoneId);
            Assert.AreEqual(phone2.Id, c3.Phone.Id);
            Assert.AreEqual("2", c3.Phone.Number);
            Assert.IsNull(c4.Phone);
            Assert.AreEqual(0, c4.PhoneId);
        }
    }
}
