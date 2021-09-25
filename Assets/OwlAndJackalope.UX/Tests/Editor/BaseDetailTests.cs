using System;
using NUnit.Framework;
using OwlAndJackalope.UX.Runtime.Data;

namespace OwlAndJackalope.UX.Tests.Editor
{
    [TestFixture]
    public class BaseDetailTests
    {
        [Test]
        public void GetObjectAndGetValueAreEqualWithValueType()
        {
            var detail = new BaseDetail<int>("Detail", 12);
            Assert.That(detail.GetValue(), Is.EqualTo(detail.GetObject()));
        }
        
        [Test]
        public void GetObjectAndGetValueAreEqualWithReferenceType()
        {
            var detail = new BaseDetail<IReference>("Detail", new BaseReference());
            Assert.That(detail.GetValue(), Is.EqualTo(detail.GetObject()));
        }

        [Test]
        public void DetailInitialVersionIsZero()
        {
            var detail = new BaseDetail<int>("Detail");
            Assert.That(detail.Version, Is.EqualTo(0));
            
            var detail1 = new BaseDetail<int>("Detail1", 12);
            Assert.That(detail1.Version, Is.EqualTo(0));
        }

        [Test]
        public void DetailVersionNumberUpdatesOnChange()
        {
            var detail = new BaseDetail<int>("Detail", 12);
            
            detail.SetValue(13);
            Assert.That(detail.Version, Is.EqualTo(1));

            detail.SetObject(14);
            Assert.That(detail.Version, Is.EqualTo(2));
        }
        
        [Test]
        public void DetailVersionNumberDoesntChangeIfEqualOnChange()
        {
            var detail = new BaseDetail<TimeSpan>("Detail", TimeSpan.FromDays(12));
            
            detail.SetValue(TimeSpan.FromDays(12));
            Assert.That(detail.Version, Is.EqualTo(0));

            detail.SetObject(TimeSpan.FromDays(12));
            Assert.That(detail.Version, Is.EqualTo(0));
        }

        [Test]
        public void VersionChangedEventIsFiredOnChange()
        {
            var fired = 0;
            var detail = new BaseDetail<int>("Detail", 12);
            detail.VersionChanged += () => fired++;
            
            detail.SetValue(13);
            Assert.That(fired, Is.EqualTo(1));

            detail.SetObject(14);
            Assert.That(fired, Is.EqualTo(2));
        }
        
        [Test]
        public void VersionedChangedEventIsNotFiredOnSameChange()
        {
            var fired = 0;
            var detail = new BaseDetail<int>("Detail", 12);
            detail.VersionChanged += () => fired++;
            
            detail.SetValue(12);
            Assert.That(fired, Is.EqualTo(0));

            detail.SetObject(12);
            Assert.That(fired, Is.EqualTo(0));
        }

        [Test]
        public void GetValueChangesAfterSetValue()
        {
            var detail = new BaseDetail<int>("Detail", 12);
            Assert.That(detail.GetValue(), Is.EqualTo(12));
            Assert.That(detail.GetObject(), Is.EqualTo(12));

            detail.SetValue(13);
            Assert.That(detail.GetValue(), Is.EqualTo(13));
            Assert.That(detail.GetObject(), Is.EqualTo(13));
            
            detail.SetObject(14);
            Assert.That(detail.GetValue(), Is.EqualTo(14));
            Assert.That(detail.GetObject(), Is.EqualTo(14));
        }

        [Test]
        public void GetTypeReturnsTypeOfDetail()
        {
            var intDetail = new BaseDetail<int>("Detail");
            var stringDetail = new BaseDetail<string>("Detail");
            var referenceDetail = new BaseDetail<IReference>("Detail");

            Assert.That(intDetail.GetObjectType(), Is.EqualTo(typeof(int)));
            Assert.That(stringDetail.GetObjectType(), Is.EqualTo(typeof(string)));
            Assert.That(referenceDetail.GetObjectType(), Is.EqualTo(typeof(IReference)));
        }
    }
}