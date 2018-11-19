using System.Collections.Generic;
using G42.Safer.Xss.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Safer.Tests.Unit
{
    [TestFixture]
    public class ExtensionTests
    {
        [Test]
        public void Can_Make_Objects_Safe()
        {
            var unsafeString = "<script>alert('hello');</script><a href='#' onClick='javascript:void();'>Foo</a>";

            var testObj = new TestObject
            {
                Foo = unsafeString,
                Bar = 123,
                Fubar = new List<string>
                {
                    unsafeString,
                    "bar"
                },
                FubarNumerable = new List<string>
                {
                    unsafeString
                },
                NestedTestObject = new TestObject
                {
                    Foo = unsafeString,
                    Bar = 123,
                    Fubar = new List<string>
                    {
                        unsafeString,
                        "bar"
                    }
                },
                Fooable = new FooThing
                {
                    Blah = unsafeString
                }
            };

            var testObj2 = new TestObject
            {
                Foo = unsafeString,
                Bar = 123,
                Fubar = new List<string>
                {
                    "foo",
                    null
                },
                NestedTestObject = testObj
            };

            var testObj3 = new TestObject
            {

            };

            var testObj4 = new TestObject
            {
                DictionaryTester = new Dictionary<int, string>
                {
                    { 1, unsafeString }
                }
            };

            var result1 = testObj.ToSaferObject();

            var serializedResult1 = JsonConvert.SerializeObject(result1);

            Assert.That(!serializedResult1.Contains(unsafeString));

            var result2 = testObj2.ToSaferObject();

            var serializedResult2 = JsonConvert.SerializeObject(result2);

            Assert.That(!serializedResult2.Contains(unsafeString));

            var result3 = testObj3.ToSaferObject();

            var serializedResult3 = JsonConvert.SerializeObject(result3);

            Assert.That(!serializedResult3.Contains(unsafeString));

            //this has a dictionary that will not be touched
            var result4 = testObj4.ToSaferObject();

            var serializedResult4 = JsonConvert.SerializeObject(result4);

            Assert.That(serializedResult4.Contains(unsafeString));
        }

        [Test]
        public void Can_Make_String_Safer()
        {
            var unsafeString = "<script>alert('hello');</script><a href='#' onClick='javascript:void();'>Foo</a>";

            var saferString = unsafeString.ToSaferString();

            Assert.AreEqual("&lt;script&gt;alert(&#39;hello&#39;);&lt;/script&gt;&lt;a href=&#39;#&#39; onClick=&#39;javascript:void();&#39;&gt;Foo&lt;/a&gt;", saferString);
        }

        public class TestObject
        {
            public Dictionary<int, string> DictionaryTester { get; set; }
            public string Foo { get; set; }
            public int Bar { get; set; }
            public List<string> Fubar { get; set; }
            public IEnumerable<string> FubarNumerable { get; set; }
            public TestObject NestedTestObject { get; set; }
            public IFoo Fooable { get; set; }
        }

        public class FooThing : IFoo
        {
            public string Blah { get; set; }
        }

        public interface IFoo
        {
            string Blah { get; set; }
        }
    }
}
