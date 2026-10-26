using System;
using NUnit.Framework;
using VertigoWheel.Core;

namespace VertigoWheel.Tests
{
    public class SpinResolverTests
    {
        [Test]
        public void ResolveSliceIndex_ReturnsValueFromInjectedRandomProvider()
        {
            SpinResolver resolver = new SpinResolver(new QueueRandomProvider(5));

            Assert.AreEqual(5, resolver.ResolveSliceIndex(8));
        }

        [Test]
        public void ResolveSliceIndex_WheelWithoutSlices_Throws()
        {
            SpinResolver resolver = new SpinResolver(new QueueRandomProvider());

            Assert.Throws<ArgumentOutOfRangeException>(() => resolver.ResolveSliceIndex(0));
        }
    }
}