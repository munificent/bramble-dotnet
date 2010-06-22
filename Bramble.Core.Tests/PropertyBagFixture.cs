using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Bramble.Core;

namespace Bramble.Core.Tests
{
    [TestFixture]
    public class PropertyBagFixture
    {
        #region Constructor

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NameValueBase_ThrowsOnNullName()
        {
            PropertyBag dummy = new PropertyBag(null, "bar", new PropertyBag[] { new PropertyBag("base") });
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NameValueBase_ThrowsOnNullValue()
        {
            PropertyBag dummy = new PropertyBag("foo", null, new PropertyBag[] { new PropertyBag("base") });
        }

        [Test]
        public void Constructor_NameValueBase_DoesNotThrowOnNullBase()
        {
            PropertyBag dummy = new PropertyBag("foo", "bar", null);
        }

        [Test]
        public void Constructor_NameValueBase()
        {
            PropertyBag prop = new PropertyBag("foo", "bar", new PropertyBag[] { new PropertyBag("base") });

            Assert.AreEqual("foo", prop.Name);
            Assert.AreEqual("bar", prop.Value);
            Assert.AreEqual(0, prop.Count);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NameValue_ThrowsOnNullName()
        {
            PropertyBag dummy = new PropertyBag(null, "bar");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NameValue_ThrowsOnNullValue()
        {
            PropertyBag dummy = new PropertyBag("foo", (string)null);
        }

        [Test]
        public void Constructor_NameValue()
        {
            PropertyBag prop = new PropertyBag("foo", "bar");

            Assert.AreEqual("foo", prop.Name);
            Assert.AreEqual("bar", prop.Value);
            Assert.AreEqual(0, prop.Count);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NameBase_ThrowsOnNullName()
        {
            PropertyBag dummy = new PropertyBag(null, new PropertyBag[] { new PropertyBag("base") });
        }

        [Test]
        public void Constructor_NameBase_DoesNotThrowOnNullBase()
        {
            PropertyBag dummy = new PropertyBag("foo", (PropertyBag[])null);
        }

        [Test]
        public void Constructor_NameBase_DoesNotThrowOnEmptyBase()
        {
            PropertyBag dummy = new PropertyBag("foo", new PropertyBag[0]);
        }

        [Test]
        public void Constructor_NameBase()
        {
            PropertyBag prop = new PropertyBag("foo", new PropertyBag[] { new PropertyBag("base") });

            Assert.AreEqual("foo", prop.Name);
            Assert.AreEqual(String.Empty, prop.Value);
            Assert.AreEqual(0, prop.Count);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Name_ThrowsOnNull()
        {
            PropertyBag dummy = new PropertyBag(null);
        }

        [Test]
        public void Constructor_Name()
        {
            PropertyBag prop = new PropertyBag("foo");

            Assert.AreEqual("foo", prop.Name);
            Assert.AreEqual(String.Empty, prop.Value);
            Assert.AreEqual(0, prop.Count);
        }

        #endregion

        [Test]
        public void StringIndexer_FindsCorrectValue()
        {
            PropertyBag prop = new PropertyBag("foo");
            prop.Add(new PropertyBag("first", "one"));
            prop.Add(new PropertyBag("second", "two"));

            Assert.AreEqual("one", prop["first"].Value);
            Assert.AreEqual("two", prop["second"].Value);
        }

        [Test]
        public void StringIndexer_ReturnsNullOnMissing()
        {
            PropertyBag prop = new PropertyBag("foo");

            Assert.IsNull(prop["not found"]);
        }

        [Test]
        public void Add()
        {
            PropertyBag prop = new PropertyBag("foo", "bar");

            Assert.AreEqual(0, prop.Count);

            prop.Add(new PropertyBag("item", "value"));

            Assert.AreEqual(1, prop.Count);
            Assert.AreEqual("item", prop["item"].Name);
            Assert.AreEqual("value", prop["item"].Value);
        }

        [Test]
        public void Contains()
        {
            PropertyBag prop = new PropertyBag("foo", "bar");

            prop.Add(new PropertyBag("name", "value"));

            Assert.IsTrue(prop.Contains("name"));
            Assert.IsFalse(prop.Contains("not name"));
        }

        [Test]
        public void TryGetValue()
        {
            PropertyBag prop = new PropertyBag("foo", "bar");
            PropertyBag found = new PropertyBag("found", "value");
            prop.Add(found);

            // find one
            PropertyBag result;
            bool success = prop.TryGetValue("found", out result);
            Assert.IsTrue(success);
            Assert.AreSame(found, result);

            // fail to find one
            success = prop.TryGetValue("not found", out result);
            Assert.IsFalse(success);
            Assert.IsNull(result);
        }

        [Test]
        public void GetOrDefault_String()
        {
            PropertyBag prop = new PropertyBag("foo", "bar");

            prop.Add(new PropertyBag("name", "value"));

            Assert.AreEqual("value", prop.GetOrDefault("name", "default"));
            Assert.AreEqual("default", prop.GetOrDefault("not name", "default"));
        }

        [Test]
        public void GetOrDefault_Int()
        {
            PropertyBag prop = new PropertyBag("foo", "bar");

            prop.Add(new PropertyBag("name", "123"));

            Assert.AreEqual(123, prop.GetOrDefault("name", 666));
            Assert.AreEqual(666, prop.GetOrDefault("not name", 666));
        }

        [Test]
        public void GetOrDefault_Converter()
        {
            PropertyBag prop = new PropertyBag("foo", "bar");

            prop.Add(new PropertyBag("name", "value"));

            Assert.AreEqual("value (added)", prop.GetOrDefault("name", value => value + " (added)", "default"));
            Assert.AreEqual("default", prop.GetOrDefault("not name", "default"));
        }

        [Test]
        public void InheritValuesFromBases()
        {
            PropertyBag base1Prop = new PropertyBag("base1");
            base1Prop.Add(new PropertyBag("from base 1", "value 1"));

            PropertyBag base2Prop = new PropertyBag("base2");
            base2Prop.Add(new PropertyBag("from base 2", "value 2"));

            PropertyBag derivedProp = new PropertyBag("derived", new PropertyBag[] { base1Prop, base2Prop });
            derivedProp.Add(new PropertyBag("from derived", "value"));

            Assert.AreEqual(3, derivedProp.Count);
            Assert.AreEqual("value 1", derivedProp["from base 1"].Value);
            Assert.AreEqual("value 2", derivedProp["from base 2"].Value);
            Assert.AreEqual("value", derivedProp["from derived"].Value);
        }

        [Test]
        public void OverrideAcrossBases()
        {
            PropertyBag base1Prop = new PropertyBag("base1");
            base1Prop.Add(new PropertyBag("from base", "value 1"));

            PropertyBag base2Prop = new PropertyBag("base2");
            base2Prop.Add(new PropertyBag("from base", "value 2"));

            PropertyBag derivedProp = new PropertyBag("derived", new PropertyBag[] { base1Prop, base2Prop });

            Assert.AreEqual(1, derivedProp.Count);
            Assert.AreEqual("value 2", derivedProp["from base"].Value);
        }

        [Test]
        public void OverrideValueFromBase()
        {
            PropertyBag baseProp = new PropertyBag("base");
            baseProp.Add(new PropertyBag("from base", "value"));
            baseProp.Add(new PropertyBag("override", "base value"));

            PropertyBag derivedProp = new PropertyBag("derived", new PropertyBag[] { baseProp });
            derivedProp.Add(new PropertyBag("from derived", "value"));
            derivedProp.Add(new PropertyBag("override", "derived value"));

            Assert.AreEqual(3, derivedProp.Count);
            Assert.AreEqual("value", derivedProp["from base"].Value);
            Assert.AreEqual("derived value", derivedProp["override"].Value);
        }
    }
}