using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using OwlAndJackalope.UX.Runtime.Data;
using OwlAndJackalope.UX.Runtime.Data.Extensions;
using OwlAndJackalope.UX.Runtime.Data.Serialized;
using OwlAndJackalope.UX.Runtime.Data.Serialized.Enums;
using UnityEngine;

namespace OwlAndJackalope.UX.Tests.Editor
{
    [TestFixture]
    public class BaseMapDetailTests
    {
         [Test]
        public void GetObjectAndGetValueAreEqual()
        {
            var detail = new BaseMapDetail<string, int>("detail", new Dictionary<string, int>()
            {
                {"one", 1},
                {"two", 2},
                {"three", 3},
            }, false);
            
            Assert.That(detail.GetValue(), Is.EqualTo(detail.GetObject()));
        }

        [Test]
        public void CreateDetailWithDuplicateDoesntUseInitialObject()
        {
            var dict = new Dictionary<string, int>()
            {
                { "one", 1 },
                { "two", 2 },
                { "three", 3 },
            };
            var detail = new BaseMapDetail<string, int>("detail", dict, true);
            
            Assert.That(detail.GetValue() == dict, Is.False);
            Assert.That(detail.GetValue(), Is.EquivalentTo(dict));
        }
        
        [Test]
        public void CreateDetailWithoutDuplicateDoesntUseInitialObject()
        {
            var dict = new Dictionary<string, int>()
            {
                { "one", 1 },
                { "two", 2 },
                { "three", 3 },
            };
            var detail = new BaseMapDetail<string, int>("detail", dict, false);
            
            Assert.That(detail.GetValue() == dict, Is.True);
            Assert.That(detail.GetValue(), Is.EquivalentTo(dict));
        }
        
        [Test]
        public void SetValueWithDifferentValueUpdatesValues()
        {
            var dict = new Dictionary<string, int>()
            {
                { "one", 1 },
                { "two", 2 },
                { "three", 3 },
            };
            var detail = new BaseMapDetail<string, int>("detail", dict, false);
            var newDict = new Dictionary<string, int>()
            {
                { "four", 4 },
                { "five", 5 },
                { "six", 6 },
            };

            var changed = 0;
            detail.VersionChanged += () => changed++;
            detail.SetValue(newDict);

            Assert.That(detail.GetValue(), Is.EqualTo(detail.GetObject()));
            Assert.That(detail.GetValue(), Is.EqualTo(newDict));
            Assert.That(detail.Version, Is.EqualTo(1));
            Assert.That(changed, Is.EqualTo(1));
        }
        
        [Test]
        public void SetObjectWithDifferentValueUpdatesValues()
        {
            var dict = new Dictionary<string, int>()
            {
                { "one", 1 },
                { "two", 2 },
                { "three", 3 },
            };
            var detail = new BaseMapDetail<string, int>("detail", dict, false);
            var newDict = new Dictionary<string, int>()
            {
                { "four", 4 },
                { "five", 5 },
                { "six", 6 },
            };

            var changed = 0;
            detail.VersionChanged += () => changed++;
            detail.SetObject(newDict);

            Assert.That(detail.GetValue(), Is.EqualTo(detail.GetObject()));
            Assert.That(detail.GetValue(), Is.EqualTo(newDict));
            Assert.That(detail.Version, Is.EqualTo(1));
            Assert.That(changed, Is.EqualTo(1));
        }
        
        [Test]
        public void SetValueWithSameValueDoesNothing()
        {
            var dict = new Dictionary<string, int>()
            {
                { "one", 1 },
                { "two", 2 },
                { "three", 3 },
            };
            var detail = new BaseMapDetail<string, int>("detail", dict, false);

            var changed = 0;
            detail.VersionChanged += () => changed++;
            detail.SetValue(dict);

            Assert.That(detail.GetValue(), Is.EqualTo(detail.GetObject()));
            Assert.That(detail.GetValue(), Is.EqualTo(dict));
            Assert.That(detail.Version, Is.EqualTo(0));
            Assert.That(changed, Is.EqualTo(0));
        }
        
        [Test]
        public void SetObjectWithSameValueDoesNothing()
        {
            var dict = new Dictionary<string, int>()
            {
                { "one", 1 },
                { "two", 2 },
                { "three", 3 },
            };
            var detail = new BaseMapDetail<string, int>("detail", dict, false);

            var changed = 0;
            detail.VersionChanged += () => changed++;
            detail.SetObject(dict);

            Assert.That(detail.GetValue(), Is.EqualTo(detail.GetObject()));
            Assert.That(detail.GetValue(), Is.EqualTo(dict));
            Assert.That(detail.Version, Is.EqualTo(0));
            Assert.That(changed, Is.EqualTo(0));
        }
        
        [Test]
        public void LengthIsEqualToListLength()
        {
            var dict = new Dictionary<string, int>()
            {
                { "one", 1 },
                { "two", 2 },
                { "three", 3 },
            };
            var detail = new BaseMapDetail<string, int>("detail", dict, false);
            Assert.That(detail.Count, Is.EqualTo(3));
            
            dict.Add("twenty-three", 23);
            Assert.That(detail.Version, Is.EqualTo(0));
            Assert.That(detail.Count, Is.EqualTo(4));
        }
        
        [Test]
        public void NullListReturns0Length()
        {
            var detail = new BaseMapDetail<string, int>("detail", null, false);
            Assert.That(detail.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void AddElementChangesVersion()
        {
            var dict = new Dictionary<string, int>()
            {
                { "one", 1 },
                { "two", 2 },
                { "three", 3 },
            };
            var detail = new BaseMapDetail<string, int>("detail", dict, false);
            var changed = 0;
            detail.VersionChanged += () => changed++;
            
            detail.Add("four", 4);
            detail.Add("five", 5);
            detail.Add("six", 6);

            Assert.That(dict.Count, Is.EqualTo(6));
            Assert.That(detail.Count, Is.EqualTo(6));
            Assert.That(detail.Version, Is.EqualTo(3));
            Assert.That(changed, Is.EqualTo(3));

            Assert.That(detail.ContainsKey("one"), Is.True);
            Assert.That(detail.ContainsKey("two"), Is.True);
            Assert.That(detail.ContainsKey("three"), Is.True);
            Assert.That(detail.ContainsKey("four"), Is.True);
            Assert.That(detail.ContainsKey("five"), Is.True);
            Assert.That(detail.ContainsKey("six"), Is.True);
        }
        
        [Test]
        public void AddOnNullListCreatesLists()
        {
            var detail = new BaseMapDetail<string, int>("detail", null, false);
            Assert.That(detail.Count, Is.EqualTo(0));
            
            detail.Add("four", 4);
            Assert.That(detail.GetValue(), Is.Not.Null);
            Assert.That(detail.Count, Is.EqualTo(1));
        }
        
        [Test]
        public void RemoveChangesElement()
        {
            var dict = new Dictionary<string, int>()
            {
                { "one", 1 },
                { "two", 2 },
                { "three", 3 },
            };
            var detail = new BaseMapDetail<string, int>("detail", dict, false);
            var changed = 0;
            detail.VersionChanged += () => changed++;
            
            detail.Remove("two");
            detail.Remove("three");

            Assert.That(dict.Count, Is.EqualTo(1));
            Assert.That(detail.Count, Is.EqualTo(1));
            Assert.That(detail.Version, Is.EqualTo(2));
            Assert.That(changed, Is.EqualTo(2));

            Assert.That(detail.ContainsKey("one"), Is.True);
            Assert.That(detail.ContainsKey("two"), Is.False);
            Assert.That(detail.ContainsKey("three"), Is.False);
        }

        [Test]
        public void RemoveOnNullDoesNothing()
        {
            var detail = new BaseMapDetail<string, int>("detail", null, false);
            var changed = 0;
            detail.VersionChanged += () => changed++;
            
            detail.Remove("two");

            Assert.That(detail.Count, Is.EqualTo(0));
            Assert.That(detail.Version, Is.EqualTo(0));
            Assert.That(changed, Is.EqualTo(0));
        }

        [Test]
        public void RemoveNonContainedObjectDoesNothing()
        {
            var dict = new Dictionary<string, int>()
            {
                { "one", 1 },
                { "two", 2 },
                { "three", 3 },
            };
            var detail = new BaseMapDetail<string, int>("detail", dict, false);
            var changed = 0;
            detail.VersionChanged += () => changed++;
            
            detail.Remove("four");

            Assert.That(detail.Count, Is.EqualTo(3));
            Assert.That(detail.Version, Is.EqualTo(0));
            Assert.That(changed, Is.EqualTo(0));
        }

        [Test]
        public void ClearRemovesElementsAndUpdatesVersion()
        {
            var dict = new Dictionary<string, int>()
            {
                { "one", 1 },
                { "two", 2 },
                { "three", 3 },
            };
            var detail = new BaseMapDetail<string, int>("detail", dict, false);
            var changed = 0;
            detail.VersionChanged += () => changed++;
            
            detail.Clear();

            Assert.That(detail.Count, Is.EqualTo(0));
            Assert.That(detail.Version, Is.EqualTo(1));
            Assert.That(changed, Is.EqualTo(1));
        }

        [Test]
        public void ClearOnNullDoesNothing()
        {
            var detail = new BaseMapDetail<string, int>("detail", null, false);
            var changed = 0;
            detail.VersionChanged += () => changed++;
            
            detail.Clear();

            Assert.That(detail.Count, Is.EqualTo(0));
            Assert.That(detail.Version, Is.EqualTo(0));
            Assert.That(changed, Is.EqualTo(0));
        }
        
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
        public void TestDetailIsMadeFromData(DetailType detailType, Type systemType)
        {
            var serializedDetailType = typeof(BaseSerializedMapDetail);
            var keyType = serializedDetailType.GetField("_keyType", BindingFlags.Instance | BindingFlags.NonPublic);
            var keyEnum = serializedDetailType.GetField("_keyEnumId", BindingFlags.Instance | BindingFlags.NonPublic);
            var valueType = serializedDetailType.GetField("_valueType", BindingFlags.Instance | BindingFlags.NonPublic);
            var valueEnum = serializedDetailType.GetField("_valueEnumId", BindingFlags.Instance | BindingFlags.NonPublic);
            var detail = new BaseSerializedMapDetail("name", systemType, systemType);
            
            Assert.That(detail.Name, Is.EqualTo("name"));
            Assert.That(detail.Type, Is.EqualTo(typeof(Dictionary<,>).MakeGenericType(systemType, systemType)));
            
            Assert.That(keyType.GetValue(detail), Is.EqualTo(detailType));
            Assert.That(keyEnum.GetValue(detail), Is.EqualTo(0));
            
            Assert.That(valueType.GetValue(detail), Is.EqualTo(detailType));
            Assert.That(valueEnum.GetValue(detail), Is.EqualTo(0));
        }
        
        [Test]
        public void TestDetailIsMadeFromDataAsEnum()
        {
            try
            {
                SerializedDetailEnumCache.AddEnumType(-2000, new EnumDetailCreator<TestEnumOne>(x => (TestEnumOne)x));
                
                var serializedDetailType = typeof(BaseSerializedMapDetail);
                var keyType = serializedDetailType.GetField("_keyType", BindingFlags.Instance | BindingFlags.NonPublic);
                var keyEnum = serializedDetailType.GetField("_keyEnumId", BindingFlags.Instance | BindingFlags.NonPublic);
                var valueType = serializedDetailType.GetField("_valueType", BindingFlags.Instance | BindingFlags.NonPublic);
                var valueEnum = serializedDetailType.GetField("_valueEnumId", BindingFlags.Instance | BindingFlags.NonPublic);
                var detail = new BaseSerializedMapDetail("name", typeof(TestEnumOne), typeof(TestEnumOne));
                
                Assert.That(detail.Name, Is.EqualTo("name"));
                Assert.That(detail.Type, Is.EqualTo(typeof(Dictionary<TestEnumOne, TestEnumOne>)));
               
                Assert.That(keyType.GetValue(detail), Is.EqualTo(DetailType.Enum));
                Assert.That(keyEnum.GetValue(detail), Is.EqualTo(-2000));
            
                Assert.That(valueType.GetValue(detail), Is.EqualTo(DetailType.Enum));
                Assert.That(valueEnum.GetValue(detail), Is.EqualTo(-2000));
            }
            finally
            {
                SerializedDetailEnumCache.RemoveEnumType(-2000);
            }
        }
        
        [TestCase(false)]
        [TestCase(true)]
        [TestCase(12)]
        [TestCase(-12)]
        [TestCase(12L)]
        [TestCase(-12L)]
        [TestCase(12.5f)]
        [TestCase(-12.5f)]
        [TestCase(12.5)]
        [TestCase(-12.5)]
        [TestCase("")]
        [TestCase("some value")]
        [TestCase(DetailType.Float)]
        public void DetailIsCreatedFromSerialized<TValue>(TValue value)
        {
            var detailWrapper = new SerializedDetailWrapper<string, TValue>("detailName");
            detailWrapper.AddValue("testing", value);
            
            var runTimeDetail = detailWrapper.Detail.ConvertToDetail();
            
            Assert.That(runTimeDetail is IDetail<Dictionary<string, TValue>>, Is.True);
            Assert.That(runTimeDetail is IMapDetail<string, TValue>, Is.True);
            Assert.That(runTimeDetail.Name, Is.EqualTo("detailName"));
            Assert.That(runTimeDetail.GetObject(), Is.EquivalentTo(new Dictionary<string, TValue>() { {"testing", value} }));
        }

        private void CreateFromObjectTest<T>(T value)
        {
            var detailWrapper = new SerializedDetailWrapper<string, T>("detailName");
            detailWrapper.AddValue("testing", value);
            
            var runTimeDetail = detailWrapper.Detail.ConvertToDetail();
            
            Assert.That(runTimeDetail is IDetail<Dictionary<string, T>>, Is.True);
            Assert.That(runTimeDetail is IMapDetail<string, T>, Is.True);
            Assert.That(runTimeDetail.Name, Is.EqualTo("detailName"));
            Assert.That(runTimeDetail.GetObject(), Is.EquivalentTo(new Dictionary<string, T>() { {"testing", value} }));
        }
        
        [Test]
        public void DetailIsCreatedFromSerializedVector2()
        {
            CreateFromObjectTest(new Vector2(0.5f, 0.5f));
        }
        
        [Test]
        public void DetailIsCreatedFromSerializedVector3()
        {
            CreateFromObjectTest(new Vector3(0.5f, 0.5f, 0.5f));
        }
        
        [Test]
        public void DetailIsCreatedFromSerializedColor()
        {
            CreateFromObjectTest(new Color(0.5f, 0.5f, 0.5f, 1f));
        }
        
        [Test]
        public void DetailIsCreatedFromSerializedTimeSpan()
        {
            CreateFromObjectTest(new TimeSpan(1, 2, 3, 4));
        }

        [Test]
        public void DetailIsCreatedFromGameObject()
        {
            CreateFromObjectTest(new GameObject("mapDetailGO"));
        }
        
        [Test]
        public void DetailIsCreatedFromTexture()
        {
            CreateFromObjectTest(new Texture2D(200, 200));
        }
        
        [Test]
        public void DetailIsCreatedFromSprite()
        {
            var sprite = Sprite.Create(new Texture2D(200, 200), new Rect(0, 0, 200, 200), Vector2.zero);
            CreateFromObjectTest(sprite);
        }

        [Test]
        public void DetailIsCreatedFromTemplate()
        {
            var detailWrapper = new SerializedDetailWrapper<string, IReference>("detailName");
            var template = ScriptableObject.CreateInstance<ReferenceTemplate>();
            template.Reference = new BaseSerializedReference();
            template.Reference.UpdateSerializedDetails(new List<IDetail>()
            {
                new BaseDetail<int>("TestOne"),
                new BaseDetail<float>("TestTwo")
            });
            
            detailWrapper.AddValue("testing", template);
            var runTimeDetail = detailWrapper.Detail.ConvertToDetail();
            
            Assert.That(runTimeDetail is IDetail<Dictionary<string, IReference>>, Is.True);
            Assert.That(runTimeDetail is IMapDetail<string, IReference>, Is.True);
            Assert.That(runTimeDetail.Name, Is.EqualTo("detailName"));

            var referenceValue = (runTimeDetail.GetObject() as Dictionary<string, IReference>)["testing"];
            Assert.That(referenceValue.GetDetail("TestOne"), Is.Not.Null);
            Assert.That(referenceValue.GetDetail<int>("TestOne"), Is.Not.Null);
            Assert.That(referenceValue.GetDetail("TestTwo"), Is.Not.Null);
            Assert.That(referenceValue.GetDetail<float>("TestTwo"), Is.Not.Null);
        }
        
        private class SerializedDetailWrapper<TKey, TValue>
        {
            public readonly BaseSerializedMapDetail Detail;

            public SerializedDetailWrapper(string name)
            {
                Detail = new BaseSerializedMapDetail(name, typeof(TKey), typeof(TValue));
            }
            
            public void AddValue(string key, ReferenceTemplate template)
            {
                var newKey = new BaseSerializedDetail("", typeof(TKey));
                newKey.SetValue(key);
                
                var newValue = new BaseSerializedDetail("", typeof(TValue));
                newValue.SetValue(template);
                
                AddToCollection(newKey, newValue);
            }

            public void AddValue(TKey key, TValue value)
            {
                var newKey = new BaseSerializedDetail("", typeof(TKey));
                newKey.SetValue(key);
                
                var newValue = new BaseSerializedDetail("", typeof(TValue));
                newValue.SetValue(value);
                
                AddToCollection(newKey, newValue);
            }

            private void AddToCollection(BaseSerializedDetail key, BaseSerializedDetail value)
            {
                var keyCollection = (List<BaseSerializedDetail>)typeof(BaseSerializedMapDetail).GetField("_keyCollection", 
                    BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(Detail);
                keyCollection?.Add(key);
                
                var valCollection = (List<BaseSerializedDetail>)typeof(BaseSerializedMapDetail).GetField("_valueCollection", 
                    BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(Detail);
                valCollection?.Add(value);
            }
        }
    }
}