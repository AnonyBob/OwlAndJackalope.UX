using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OwlAndJackalope.UX.Runtime.Data;
using OwlAndJackalope.UX.Runtime.Data.Extensions;
using OwlAndJackalope.UX.Runtime.Data.Serialized;

namespace OwlAndJackalope.UX.Tests.Editor
{
    [TestFixture]
    public class BaseReferenceTests
    {
        [Test]
        public void InitializeWithDetailsHasDetails()
        {
            var reference = new BaseReference(new IDetail[] { new BaseDetail<int>("DetailName") });
            
            Assert.That(reference.GetDetail("DetailName"), Is.Not.Null);
            Assert.That(reference.Count(), Is.EqualTo(1));
        }

        [Test]
        public void InitializeWithNoDetailsHasNoDetails()
        {
            var reference = new BaseReference();
            
            Assert.That(reference.GetDetail("DetailName"), Is.Null);
            Assert.That(reference.Count(), Is.EqualTo(0));
        }

        [Test]
        public void InitializeHasZeroVersion()
        {
            var reference = new BaseReference();
            Assert.That(reference.Version, Is.EqualTo(0));
            
            var withDetails = new BaseReference(new IDetail[] { new BaseDetail<int>("DetailName") });
            Assert.That(withDetails.Version, Is.EqualTo(0));
        }

        [Test]
        public void AddNewDetailChangesVersion()
        {
            var reference = new BaseReference();
            var changes = 0;
            reference.VersionChanged += () => changes++;

            Assert.That(reference.GetDetail("Detail"), Is.Null);
            
            reference.AddDetail(new BaseDetail<int>("Detail"));
            Assert.That(reference.Version, Is.EqualTo(1));
            Assert.That(changes, Is.EqualTo(1));
            
            Assert.That(reference.GetDetail("Detail"), Is.Not.Null);
        }

        [Test]
        public void AddExistingDetailWithOverrideChangesVersion()
        {
            var reference = new BaseReference(new IDetail[] { new BaseDetail<int>("Detail", 23) });
            var changes = 0;
            reference.VersionChanged += () => changes++;

            Assert.That(reference.GetDetail("Detail"), Is.Not.Null);
            Assert.That(reference.GetDetail("Detail").GetObject(), Is.EqualTo(23));
            
            reference.AddDetail(new BaseDetail<int>("Detail", 1), true);
            Assert.That(reference.Version, Is.EqualTo(1));
            Assert.That(changes, Is.EqualTo(1));
            
            Assert.That(reference.GetDetail("Detail"), Is.Not.Null);
            Assert.That(reference.GetDetail("Detail").GetObject(), Is.EqualTo(1));
        }

        [Test]
        public void AddExistingDetailWithNoOverrideDoesNothing()
        {
            var reference = new BaseReference(new IDetail[] { new BaseDetail<int>("Detail", 23) });
            var changes = 0;
            reference.VersionChanged += () => changes++;

            Assert.That(reference.GetDetail("Detail"), Is.Not.Null);
            Assert.That(reference.GetDetail("Detail").GetObject(), Is.EqualTo(23));
            
            reference.AddDetail(new BaseDetail<int>("Detail", 1), false);
            Assert.That(reference.Version, Is.EqualTo(0));
            Assert.That(changes, Is.EqualTo(0));
            
            Assert.That(reference.GetDetail("Detail"), Is.Not.Null);
            Assert.That(reference.GetDetail("Detail").GetObject(), Is.EqualTo(23));
            
        }
        
        [Test]
        public void AddNewDetailsChangesVersion()
        {
            var reference = new BaseReference();
            
            var changes = 0;
            reference.VersionChanged += () => changes++;

            var amount = reference.AddDetails(new IDetail[]
            {
                new BaseDetail<int>("DetailOne", 23),
                new BaseDetail<string>("DetailTwo", "example")
            });
            
            Assert.That(amount, Is.EqualTo(2));
            Assert.That(reference.Version, Is.EqualTo(1));
            Assert.That(changes, Is.EqualTo(1));

            Assert.That(reference.Count(), Is.EqualTo(2));
            Assert.That(reference.GetDetail("DetailOne").GetObject(), Is.EqualTo(23));
            Assert.That(reference.GetDetail("DetailTwo").GetObject(), Is.EqualTo("example"));
        }

        [Test]
        public void AddExistingDetailsWithOverrideChangesVersion()
        {
            var reference = new BaseReference(new IDetail[]
            {
                new BaseDetail<int>("DetailOne", 1),
                new BaseDetail<string>("DetailTwo", "bad")
            });
            
            var changes = 0;
            reference.VersionChanged += () => changes++;

            var amount = reference.AddDetails(new IDetail[]
            {
                new BaseDetail<int>("DetailOne", 23),
                new BaseDetail<string>("DetailTwo", "example")
            });
            
            Assert.That(amount, Is.EqualTo(2));
            Assert.That(reference.Version, Is.EqualTo(1));
            Assert.That(changes, Is.EqualTo(1));

            Assert.That(reference.Count(), Is.EqualTo(2));
            Assert.That(reference.GetDetail("DetailOne").GetObject(), Is.EqualTo(23));
            Assert.That(reference.GetDetail("DetailTwo").GetObject(), Is.EqualTo("example"));
        }

        [Test]
        public void AddExisitngDetailsWithNoOverrideDoesNothing()
        {
            var reference = new BaseReference(new IDetail[]
            {
                new BaseDetail<int>("DetailOne", 1),
                new BaseDetail<string>("DetailTwo", "bad")
            });
            
            var changes = 0;
            reference.VersionChanged += () => changes++;

            var amount = reference.AddDetails(new IDetail[]
            {
                new BaseDetail<int>("DetailOne", 23),
                new BaseDetail<string>("DetailTwo", "example")
            }, false);
            
            Assert.That(amount, Is.EqualTo(0));
            Assert.That(reference.Version, Is.EqualTo(0));
            Assert.That(changes, Is.EqualTo(0));

            Assert.That(reference.Count(), Is.EqualTo(2));
            Assert.That(reference.GetDetail("DetailOne").GetObject(), Is.EqualTo(1));
            Assert.That(reference.GetDetail("DetailTwo").GetObject(), Is.EqualTo("bad"));
        }

        [Test]
        public void RemoveDetailRemovesDetail()
        {
            var reference = new BaseReference(new IDetail[]
            {
                new BaseDetail<int>("DetailOne", 1),
                new BaseDetail<string>("DetailTwo", "bad")
            });
            
            var changes = 0;
            reference.VersionChanged += () => changes++;

            var removed = reference.RemoveDetail("DetailOne");
            
            Assert.That(removed, Is.True);
            Assert.That(reference.Version, Is.EqualTo(1));
            Assert.That(changes, Is.EqualTo(1));

            Assert.That(reference.Count(), Is.EqualTo(1));
            Assert.That(reference.GetDetail("DetailOne"), Is.Null);
            Assert.That(reference.GetDetail("DetailTwo").GetObject(), Is.EqualTo("bad"));
        }

        [Test]
        public void RemoveUnaddedDetailDoesNothing()
        {
            var reference = new BaseReference(new IDetail[]
            {
                new BaseDetail<string>("DetailTwo", "bad")
            });
            
            var changes = 0;
            reference.VersionChanged += () => changes++;

            var removed = reference.RemoveDetail("DetailOne");
            
            Assert.That(removed, Is.False);
            Assert.That(reference.Version, Is.EqualTo(0));
            Assert.That(changes, Is.EqualTo(0));

            Assert.That(reference.Count(), Is.EqualTo(1));
            Assert.That(reference.GetDetail("DetailOne"), Is.Null);
            Assert.That(reference.GetDetail("DetailTwo").GetObject(), Is.EqualTo("bad"));
        }

        [Test]
        public void GetDetailRetrievesDetail()
        {
            var reference = CreateReference();
            Assert.That(reference.GetDetail("DetailOne"), Is.Not.Null);
            Assert.That(reference.GetDetail("DetailOne").GetObject(), Is.EqualTo(1));
        }

        [Test]
        public void GetDetailThatDoesntExistReturnsNull()
        {
            var reference = CreateReference();
            Assert.That(reference.GetDetail("Flarp"), Is.Null);
        }

        [Test]
        public void GetDetailOfTypeRetrievesDetail()
        {
            var reference = CreateReference();
            Assert.That(reference.GetDetail<int>("DetailOne").GetValue(), Is.EqualTo(1));
            Assert.That(reference.GetDetail<string>("DetailTwo").GetValue(), Is.EqualTo("bad"));
            Assert.That(reference.GetCollection<long>("DetailThree").GetValue()[1], Is.EqualTo(2));
            Assert.That(reference.GetMap<string, string>("DetailFour").GetValue()["one"], Is.EqualTo("one"));
        }

        [Test]
        public void GetDetailOfTypeThatDoesntExistReturnsNull()
        {
            var reference = CreateReference();
            Assert.That(reference.GetDetail<long>("DetailOne"), Is.Null);
            Assert.That(reference.GetDetail<string>("DetailThree"), Is.Null);
            Assert.That(reference.GetCollection<double>("DetailDetail"), Is.Null);
            Assert.That(reference.GetMap<string, float>("DetailFour"), Is.Null);
        }

        [Test]
        public void EnumerationGoesThroughAllDetails()
        {
            var knownDetails = new Dictionary<string, int>()
            {
                {"DetailOne", 0},
                {"DetailTwo", 0},
                {"DetailThree", 0},
                {"DetailFour", 0}
            };
            
            var reference = CreateReference();
            foreach (var detail in reference)
            {
                knownDetails[detail.Name]++;
            }

            foreach (var knownDetail in knownDetails)
            {
                if (knownDetail.Value < 1)
                {
                    Assert.Fail($"{knownDetail.Key} was not enumerated!");
                }
            }
        }

        [Test]
        public void ConvertCreatesDetails()
        {
            var map = new Dictionary<string, long>() { { "bleh", 245 }, { "alright", 77705 } };
            var detail = new SerializedDetailTests.SerializedDetailWrapper<int>("Detail", 12);
            var collection = new BaseCollectionDetailTests.SerializedDetailWrapper<int>("Collection", new List<int>() { 1, 2, 3});
            var mapCollection = new BaseMapDetailTests.SerializedDetailWrapper<string, long>("Map", map);
            var serializedReference = new BaseSerializedReference(new [] { detail.Detail }, 
                new []{ collection.Detail }, 
                new [] { mapCollection.Detail } );

            var reference = serializedReference.ConvertToReference();
            Assert.That(reference.Count(), Is.EqualTo(3));
            Assert.That(reference.GetDetail<int>("Detail").GetValue(), Is.EqualTo(12));
            Assert.That(reference.GetCollection<int>("Collection").GetValue(), Is.EquivalentTo(new List<int>() { 1, 2, 3 }));
            Assert.That(reference.GetMap<string, long>("Map").GetValue(), Is.EquivalentTo(map));
        }

        [Test]
        public void UpdateSerializedDetailsAddsNewDetails()
        {
            var serializedReference = new BaseSerializedReference();
            var updatedReference = new BaseReference(new IDetail[]
            {
                new BaseDetail<int>("Detail"),
                new BaseCollectionDetail<int>("Collection", null, false),
                new BaseMapDetail<string, int>("Map", null, false),
            });
            
            serializedReference.UpdateSerializedDetails(updatedReference);
            var reference = serializedReference.ConvertToReference();
            Assert.That(reference.Count(), Is.EqualTo(3));
            Assert.That(reference.GetDetail<int>("Detail").GetValue(), Is.EqualTo(0));
            Assert.That(reference.GetCollection<int>("Collection").GetValue(), Is.EquivalentTo(new List<int>()));
            Assert.That(reference.GetMap<string, int>("Map").GetValue(), Is.EquivalentTo(new Dictionary<string, int>()));
        }

        [Test]
        public void UpdateSerializedDetailsDoesntRemoveExistingDetails()
        {
            var detail = new SerializedDetailTests.SerializedDetailWrapper<int>("OtherDetail", 12);
            var serializedReference = new BaseSerializedReference(new [] { detail.Detail }, null, null);
            var updatedReference = new BaseReference(new IDetail[]
            {
                new BaseDetail<int>("Detail"),
                new BaseCollectionDetail<int>("Collection", null, false),
                new BaseMapDetail<string, int>("Map", null, false),
            });
            
            serializedReference.UpdateSerializedDetails(updatedReference);
            var reference = serializedReference.ConvertToReference();
            Assert.That(reference.Count(), Is.EqualTo(4));
            Assert.That(reference.GetDetail<int>("Detail").GetValue(), Is.EqualTo(0));
            Assert.That(reference.GetDetail<int>("OtherDetail").GetValue(), Is.EqualTo(12));
            Assert.That(reference.GetCollection<int>("Collection").GetValue(), Is.EquivalentTo(new List<int>()));
            Assert.That(reference.GetMap<string, int>("Map").GetValue(), Is.EquivalentTo(new Dictionary<string, int>()));
        }

        private IReference CreateReference()
        {
            return new BaseReference(new IDetail[]
            {
                new BaseDetail<int>("DetailOne", 1),
                new BaseDetail<string>("DetailTwo", "bad"),
                new BaseCollectionDetail<long>("DetailThree", new List<long>() { 1, 2, 3}, false),
                new BaseMapDetail<string, string>("DetailFour", new Dictionary<string, string>()
                {
                    {"one", "one"},
                    {"two", "two"}
                }, false)
            });

        }
    }
}