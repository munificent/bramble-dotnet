using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Bramble.Core;

namespace Bramble.Core.Tests
{
    [TestFixture]
    public class Array2DFixture
    {
        #region Constructors

        [Test]
        public void TestConstructor_WithWidthHeight()
        {
            var array = new Array2D<int>(2, 3);
            Assert.AreEqual(new Vec(2, 3), array.Size);
        }

        [Test]
        public void TestConstructor_WithSize()
        {
            var array = new Array2D<int>(new Vec(2, 3));
            Assert.AreEqual(new Vec(2, 3), array.Size);
        }

        [Test]
        public void TestConstructor_ZeroSizeIsOK()
        {
            var dummy1 = new Array2D<int>(0, 4);
            var dummy2 = new Array2D<int>(4, 0);
            var dummy3 = new Array2D<int>(0, 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestConstructor_NegativeWidthThrows()
        {
            new Array2D<int>(-1, 4);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestConstructor_NegativeHeightThrows()
        {
            new Array2D<int>(4, -1);
        }

        [Test]
        public void TestConstructor_ValueFillsWithDefault()
        {
            var array = new Array2D<int>(3, 4);
            Assert.AreEqual(0, array[0, 0]);
        }

        [Test]
        public void TestConstructor_ObjectFillsWithDefault()
        {
            var array = new Array2D<object>(3, 4);
            Assert.AreEqual(null, array[0, 0]);
        }

        #endregion

        #region Properties

        [Test]
        public void TestSize()
        {
            var array = new Array2D<string>(3, 4);
            Assert.AreEqual(new Vec(3, 4), array.Size);
        }

        [Test]
        public void TestBounds()
        {
            var array = new Array2D<string>(3, 4);
            Assert.AreEqual(new Rect(0, 0, 3, 4), array.Bounds);
        }

        [Test]
        public void TestWidth()
        {
            var array = new Array2D<string>(5, 4);
            Assert.AreEqual(5, array.Width);
        }

        [Test]
        public void TestHeight()
        {
            var array = new Array2D<string>(3, 6);
            Assert.AreEqual(6, array.Height);
        }

        #endregion

    }
}
