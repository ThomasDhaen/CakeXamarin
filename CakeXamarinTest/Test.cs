using NUnit.Framework;
using System;
using CakeXamarin;
namespace CakeXamarinTest
{
    [TestFixture()]
    public class Test
    {
        private Calculator _calc;

        [SetUp]
        public void Setup()
        {
            _calc = new Calculator();
        }

        [Test]
        public void Add()
        {
            var sum = _calc.AddUp(2, 6);
            Assert.AreEqual(8, sum);
        }

        [Test]
        public void Subtract()
        {
            var sum = _calc.Subtract(2, 6);
            Assert.AreEqual(-4, sum);
        }

        [Test]
        public void Multiply()
        {
            var sum = _calc.Multiply(2, 6);
            Assert.AreEqual(12, sum);
        }

        [Test]
        public void Divide()
        {
            var sum = _calc.Divide(2, 6);
            Assert.AreEqual(0, sum);
        }
    }
}
