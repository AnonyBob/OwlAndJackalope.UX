using System.Collections.Generic;
using NUnit.Framework;
using OwlAndJackalope.UX.Runtime.Data;

namespace OwlAndJackalope.UX.Tests.Editor
{
    [TestFixture]
    public class BaseCollectionDetailTests
    {
        [Test]
        public void GetObjectAndGetValueAreEqual()
        {
            var detail = new BaseCollectionDetail<int>("detail", new List<int>()
            {
                1, 2, 3
            }, false);
            
            Assert.That(detail.GetValue(), Is.EqualTo(detail.GetObject()));
        }

        [Test]
        public void CreateDetailWithDuplicateDoesntUseInitialObject()
        {
            var list = new List<int>()
            {
                1, 2, 3
            };
            var detail = new BaseCollectionDetail<int>("detail", list, true);
            
            Assert.That(detail.GetValue() == list, Is.False);
            Assert.That(detail.GetValue(), Is.EquivalentTo(list));
        }
        
        [Test]
        public void CreateDetailWithoutDuplicateDoesntUseInitialObject()
        {
            var list = new List<int>()
            {
                1, 2, 3
            };
            var detail = new BaseCollectionDetail<int>("detail", list, false);
            
            Assert.That(detail.GetValue() == list, Is.True);
            Assert.That(detail.GetValue(), Is.EquivalentTo(list));
        }
        
        [Test]
        public void SetValueWithDifferentValueUpdatesValues()
        {
            var list = new List<int>()
            {
                1, 2, 3
            };
            var detail = new BaseCollectionDetail<int>("detail", list, false);
            var newList = new List<int>()
            {
                4, 5, 6
            };

            var changed = 0;
            detail.VersionChanged += () => changed++;
            detail.SetValue(newList);

            Assert.That(detail.GetValue(), Is.EqualTo(detail.GetObject()));
            Assert.That(detail.GetValue(), Is.EqualTo(newList));
            Assert.That(detail.Version, Is.EqualTo(1));
            Assert.That(changed, Is.EqualTo(1));
        }
        
        [Test]
        public void SetObjectWithDifferentValueUpdatesValues()
        {
            var list = new List<int>()
            {
                1, 2, 3
            };
            var detail = new BaseCollectionDetail<int>("detail", list, false);
            var newList = new List<int>()
            {
                4, 5, 6
            };

            var changed = 0;
            detail.VersionChanged += () => changed++;
            detail.SetObject(newList);

            Assert.That(detail.GetValue(), Is.EqualTo(detail.GetObject()));
            Assert.That(detail.GetValue(), Is.EqualTo(newList));
            Assert.That(detail.Version, Is.EqualTo(1));
            Assert.That(changed, Is.EqualTo(1));
        }
        
        [Test]
        public void SetValueWithSameValueDoesNothing()
        {
            var list = new List<int>()
            {
                1, 2, 3
            };
            var detail = new BaseCollectionDetail<int>("detail", list, false);

            var changed = 0;
            detail.VersionChanged += () => changed++;
            detail.SetValue(list);

            Assert.That(detail.GetValue(), Is.EqualTo(detail.GetObject()));
            Assert.That(detail.GetValue(), Is.EqualTo(list));
            Assert.That(detail.Version, Is.EqualTo(0));
            Assert.That(changed, Is.EqualTo(0));
        }
        
        [Test]
        public void SetObjectWithSameValueDoesNothing()
        {
            var list = new List<int>()
            {
                1, 2, 3
            };
            var detail = new BaseCollectionDetail<int>("detail", list, false);

            var changed = 0;
            detail.VersionChanged += () => changed++;
            detail.SetObject(list);

            Assert.That(detail.GetValue(), Is.EqualTo(detail.GetObject()));
            Assert.That(detail.GetValue(), Is.EqualTo(list));
            Assert.That(detail.Version, Is.EqualTo(0));
            Assert.That(changed, Is.EqualTo(0));
        }
        
        [Test]
        public void LengthIsEqualToListLength()
        {
            var list = new List<int>()
            {
                1, 2, 3
            };
            var detail = new BaseCollectionDetail<int>("detail", list, false);
            Assert.That(detail.Count, Is.EqualTo(3));
            
            list.Add(23);
            Assert.That(detail.Version, Is.EqualTo(0));
            Assert.That(detail.Count, Is.EqualTo(4));
        }
        
        [Test]
        public void NullListReturns0Length()
        {
            var detail = new BaseCollectionDetail<int>("detail", null, false);
            Assert.That(detail.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void AddElementChangesVersion()
        {
            var list = new List<int>()
            {
                1, 2, 3
            };
            var detail = new BaseCollectionDetail<int>("detail", list, false);
            var changed = 0;
            detail.VersionChanged += () => changed++;
            
            detail.Add(4);
            detail.Add(5);
            detail.Add(6);

            Assert.That(list.Count, Is.EqualTo(6));
            Assert.That(detail.Count, Is.EqualTo(6));
            Assert.That(detail.Version, Is.EqualTo(3));
            Assert.That(changed, Is.EqualTo(3));

            Assert.That(detail.Contains(1), Is.True);
            Assert.That(detail.Contains(2), Is.True);
            Assert.That(detail.Contains(3), Is.True);
            Assert.That(detail.Contains(4), Is.True);
            Assert.That(detail.Contains(5), Is.True);
            Assert.That(detail.Contains(6), Is.True);
        }
        
        [Test]
        public void AddOnNullListCreatesLists()
        {
            var detail = new BaseCollectionDetail<int>("detail", null, false);
            Assert.That(detail.Count, Is.EqualTo(0));
            
            detail.Add(4);
            Assert.That(detail.GetValue(), Is.Not.Null);
            Assert.That(detail.Count, Is.EqualTo(1));
        }
        
        [Test]
        public void RemoveChangesElement()
        {
            var list = new List<int>()
            {
                1, 2, 3
            };
            var detail = new BaseCollectionDetail<int>("detail", list, false);
            var changed = 0;
            detail.VersionChanged += () => changed++;
            
            detail.Remove(2);
            detail.Remove(3);

            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(detail.Count, Is.EqualTo(1));
            Assert.That(detail.Version, Is.EqualTo(2));
            Assert.That(changed, Is.EqualTo(2));

            Assert.That(detail.Contains(1), Is.True);
            Assert.That(detail.Contains(2), Is.False);
            Assert.That(detail.Contains(3), Is.False);
        }

        [Test]
        public void RemoveOnNullDoesNothing()
        {
            var detail = new BaseCollectionDetail<int>("detail", null, false);
            var changed = 0;
            detail.VersionChanged += () => changed++;
            
            detail.Remove(2);

            Assert.That(detail.Count, Is.EqualTo(0));
            Assert.That(detail.Version, Is.EqualTo(0));
            Assert.That(changed, Is.EqualTo(0));
        }

        [Test]
        public void RemoveNonContainedObjectDoesNothing()
        {
            var list = new List<int>()
            {
                1, 2, 3
            };
            var detail = new BaseCollectionDetail<int>("detail", list, false);
            var changed = 0;
            detail.VersionChanged += () => changed++;
            
            detail.Remove(4);

            Assert.That(detail.Count, Is.EqualTo(3));
            Assert.That(detail.Version, Is.EqualTo(0));
            Assert.That(changed, Is.EqualTo(0));
        }
    }
}