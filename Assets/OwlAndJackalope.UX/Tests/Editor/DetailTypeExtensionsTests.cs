using NUnit.Framework;
using OwlAndJackalope.UX.Runtime.Data.Serialized;
using System;
using System.Linq;
using OwlAndJackalope.UX.Runtime.Data;
using OwlAndJackalope.UX.Runtime.Data.Serialized.Enums;
using UnityEngine;

namespace OwlAndJackalope.UX.Tests.Editor
{
    public enum TestEnumOne
    {
        Hey,
        Hello
    }

    public enum TestEnumTwo
    {
        Bye,
        SeeYa
    }
    
    [TestFixture]
    public class DetailTypeExtensionsTests
    {
        [TestCase(DetailType.Bool, typeof(bool))]
        [TestCase(DetailType.Integer, typeof(int))]
        [TestCase(DetailType.Long, typeof(long))]
        [TestCase(DetailType.Float, typeof(float))]
        [TestCase(DetailType.Double, typeof(double))]
        [TestCase(DetailType.String, typeof(string))]
        [TestCase(DetailType.Reference, typeof(IReference))]
        [TestCase(DetailType.Vector2, typeof(Vector2))]
        [TestCase(DetailType.Vector3, typeof(Vector3))]
        [TestCase(DetailType.Color, typeof(Color))]
        [TestCase(DetailType.GameObject, typeof(GameObject))]
        [TestCase(DetailType.Texture, typeof(Texture2D))]
        [TestCase(DetailType.Sprite, typeof(Sprite))]
        [TestCase(DetailType.TimeSpan, typeof(TimeSpan))]
        public void ConvertEnumToType(DetailType type, Type expected)
        {
            Assert.That(type.ConvertToType(0), Is.EqualTo(expected));
            Assert.That(expected.ConvertToEnum(), Is.EqualTo(type));
        }

        [Test]
        public void CheckEnumsGoToEnumType()
        {
            Assert.That(typeof(DetailType).ConvertToEnum(), Is.EqualTo(DetailType.Enum));
            Assert.That(typeof(TestEnumOne).ConvertToEnum(), Is.EqualTo(DetailType.Enum));
            Assert.That(typeof(TestEnumTwo).ConvertToEnum(), Is.EqualTo(DetailType.Enum));
        }

        [Test]
        public void TestEnumRegistration()
        {
            try
            {
                SerializedDetailEnumCache.AddEnumType(-2000, new EnumDetailCreator<TestEnumOne>(x => (TestEnumOne)x));
                SerializedDetailEnumCache.AddEnumType(-3000, new EnumDetailCreator<TestEnumTwo>(x => (TestEnumTwo)x));

                var typeOne = (DetailType.Enum).ConvertToType(-2000);
                var typeTwo = (DetailType.Enum).ConvertToType(-3000);
                var typeThree = (DetailType.Enum).ConvertToType(-4000);

                Assert.That(typeOne, Is.EqualTo(typeof(TestEnumOne)));
                Assert.That(typeTwo, Is.EqualTo(typeof(TestEnumTwo)));
                Assert.That(typeThree, Is.EqualTo(null));
            }
            finally
            {
                SerializedDetailEnumCache.RemoveEnumType(-2000);
                SerializedDetailEnumCache.RemoveEnumType(-3000);
            }
        }

        [Test]
        public void TestEnumsAreEnumerable()
        {
            try
            {
                SerializedDetailEnumCache.AddEnumType(-2000, new EnumDetailCreator<TestEnumOne>(x => (TestEnumOne)x));
                SerializedDetailEnumCache.AddEnumType(-3000, new EnumDetailCreator<TestEnumTwo>(x => (TestEnumTwo)x));
                
                Assert.That(SerializedDetailEnumCache.EnumTypeNames.Length, Is.GreaterThanOrEqualTo(3));
                Assert.That(SerializedDetailEnumCache.EnumTypeNames.Contains("TestEnumOne"), Is.True);
                Assert.That(SerializedDetailEnumCache.EnumTypeNames.Contains("TestEnumTwo"), Is.True);
                Assert.That(SerializedDetailEnumCache.EnumTypeNames.Contains("DetailType"), Is.True);
            }
            finally
            {
                SerializedDetailEnumCache.RemoveEnumType(-2000);
                SerializedDetailEnumCache.RemoveEnumType(-3000);
            }
        }

        [Test]
        public void TestGetCreatorOfAddedById()
        {
            try
            {
                SerializedDetailEnumCache.AddEnumType(-2000, new EnumDetailCreator<TestEnumOne>(x => (TestEnumOne)x));
                var creator = SerializedDetailEnumCache.GetCreator(-2000);
                
                Assert.That(creator.EnumName, Is.EqualTo(nameof(TestEnumOne)));
                Assert.That(creator.EnumType, Is.EqualTo(typeof(TestEnumOne)));
                
                Assert.That(SerializedDetailEnumCache.GetCreator(-3000), Is.EqualTo(null));
            }
            finally
            {
                SerializedDetailEnumCache.RemoveEnumType(-2000);
            }
        }

        [Test]
        public void TestGetCreatorOfAddedByName()
        {
            try
            {
                SerializedDetailEnumCache.AddEnumType(-2000, new EnumDetailCreator<TestEnumOne>(x => (TestEnumOne)x));
                var enumIdAndCreator = SerializedDetailEnumCache.GetCreator(nameof(TestEnumOne));
                
                Assert.That(enumIdAndCreator.Creator.EnumName, Is.EqualTo(nameof(TestEnumOne)));
                Assert.That(enumIdAndCreator.Creator.EnumType, Is.EqualTo(typeof(TestEnumOne)));
                Assert.That(enumIdAndCreator.EnumId, Is.EqualTo(-2000));
                
                Assert.That(SerializedDetailEnumCache.GetCreator(nameof(TestEnumTwo)).Creator, Is.EqualTo(null));
            }
            finally
            {
                SerializedDetailEnumCache.RemoveEnumType(-2000);
            }
        }
    }
}