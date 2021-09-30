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

        [Test]
        public void ClearRemovesElementsAndUpdatesVersion()
        {
            var list = new List<int>()
            {
                1, 2, 3
            };
            var detail = new BaseCollectionDetail<int>("detail", list, false);
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
            var detail = new BaseCollectionDetail<int>("detail", null, false);
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
            var serializedDetailType = typeof(BaseSerializedCollectionDetail);
            var typeField = serializedDetailType.GetField("_type", BindingFlags.Instance | BindingFlags.NonPublic);
            var enumField = serializedDetailType.GetField("_enumId", BindingFlags.Instance | BindingFlags.NonPublic);
            var detail = new BaseSerializedCollectionDetail("name", systemType);
            
            Assert.That(detail.Name, Is.EqualTo("name"));
            Assert.That(detail.Type, Is.EqualTo(typeof(List<>).MakeGenericType(systemType)));
            
            Assert.That(typeField.GetValue(detail), Is.EqualTo(detailType));
            Assert.That(enumField.GetValue(detail), Is.EqualTo(0));
        }
        
        [Test]
        public void TestDetailIsMadeFromDataAsEnum()
        {
            try
            {
                SerializedDetailEnumCache.AddEnumType(-2000, new EnumDetailCreator<TestEnumOne>(x => (TestEnumOne)x));
                
                var serializedDetailType = typeof(BaseSerializedCollectionDetail);
                var typeField = serializedDetailType.GetField("_type", BindingFlags.Instance | BindingFlags.NonPublic);
                var enumField = serializedDetailType.GetField("_enumId", BindingFlags.Instance | BindingFlags.NonPublic);
                var detail = new BaseSerializedCollectionDetail("name", typeof(TestEnumOne));
                
                Assert.That(detail.Name, Is.EqualTo("name"));
                Assert.That(detail.Type, Is.EqualTo(typeof(List<TestEnumOne>)));
               
                Assert.That(typeField.GetValue(detail), Is.EqualTo(DetailType.Enum));
                Assert.That(enumField.GetValue(detail), Is.EqualTo(-2000));
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
            var detailWrapper = new SerializedDetailWrapper<TValue>("detailName");
            detailWrapper.AddValue(value);
            
            var runTimeDetail = detailWrapper.Detail.ConvertToDetail();
            
            Assert.That(runTimeDetail is IDetail<List<TValue>>, Is.True);
            Assert.That(runTimeDetail is ICollectionDetail<TValue>, Is.True);
            Assert.That(runTimeDetail.Name, Is.EqualTo("detailName"));
            Assert.That(runTimeDetail.GetObject(), Is.EquivalentTo(new List<TValue>() { value }));
        }
        
        [Test]
        public void DetailIsCreatedFromSerializedVector2()
        {
            var detailWrapper = new SerializedDetailWrapper<Vector2>("detailName");
            var value = new Vector2(0.5f, 0.5f);
            detailWrapper.AddValue(value);
            
            var runTimeDetail = detailWrapper.Detail.ConvertToDetail();
            
            Assert.That(runTimeDetail is IDetail<List<Vector2>>, Is.True);
            Assert.That(runTimeDetail is ICollectionDetail<Vector2>, Is.True);
            Assert.That(runTimeDetail.Name, Is.EqualTo("detailName"));
            Assert.That(runTimeDetail.GetObject(), Is.EquivalentTo(new List<Vector2>() { value }));
        }
        
        [Test]
        public void DetailIsCreatedFromSerializedVector3()
        {
            var detailWrapper = new SerializedDetailWrapper<Vector3>("detailName");
            var value = new Vector3(0.5f, 0.5f, 0.5f);
            detailWrapper.AddValue(value);
            
            var runTimeDetail = detailWrapper.Detail.ConvertToDetail();
            
            Assert.That(runTimeDetail is IDetail<List<Vector3>>, Is.True);
            Assert.That(runTimeDetail is ICollectionDetail<Vector3>, Is.True);
            Assert.That(runTimeDetail.Name, Is.EqualTo("detailName"));
            Assert.That(runTimeDetail.GetObject(), Is.EquivalentTo(new List<Vector3>() { value }));
        }
        
        [Test]
        public void DetailIsCreatedFromSerializedColor()
        {
            var detailWrapper = new SerializedDetailWrapper<Color>("detailName");
            var value = new Color(0.5f, 0.5f, 0.5f, 1f);
            detailWrapper.AddValue(value);
            
            var runTimeDetail = detailWrapper.Detail.ConvertToDetail();
            
            Assert.That(runTimeDetail is IDetail<List<Color>>, Is.True);
            Assert.That(runTimeDetail is ICollectionDetail<Color>, Is.True);
            Assert.That(runTimeDetail.Name, Is.EqualTo("detailName"));
            Assert.That(runTimeDetail.GetObject(), Is.EquivalentTo(new List<Color>() { value }));
        }
        
        [Test]
        public void DetailIsCreatedFromSerializedTimeSpan()
        {
            var detailWrapper = new SerializedDetailWrapper<TimeSpan>("detailName");
            var value = new TimeSpan(1, 2, 3, 4);
            detailWrapper.AddValue(value);
            
            var runTimeDetail = detailWrapper.Detail.ConvertToDetail();
            
            Assert.That(runTimeDetail is IDetail<List<TimeSpan>>, Is.True);
            Assert.That(runTimeDetail is ICollectionDetail<TimeSpan>, Is.True);
            Assert.That(runTimeDetail.Name, Is.EqualTo("detailName"));
            Assert.That(runTimeDetail.GetObject(), Is.EquivalentTo(new List<TimeSpan>() { value }));
        }

        [Test]
        public void DetailIsCreatedFromGameObject()
        {
            var detailWrapper = new SerializedDetailWrapper<GameObject>("detailName");
            var go = new GameObject("testGO");
            detailWrapper.AddValue(go);
            
            var runTimeDetail = detailWrapper.Detail.ConvertToDetail();
            
            Assert.That(runTimeDetail is IDetail<List<GameObject>>, Is.True);
            Assert.That(runTimeDetail is ICollectionDetail<GameObject>, Is.True);
            Assert.That(runTimeDetail.Name, Is.EqualTo("detailName"));
            Assert.That(runTimeDetail.GetObject(), Is.EquivalentTo(new List<GameObject>() { go }));
        }
        
        [Test]
        public void DetailIsCreatedFromTexture()
        {
            var detailWrapper = new SerializedDetailWrapper<Texture2D>("detailName");
            var texture = new Texture2D(200, 200);
            detailWrapper.AddValue(texture);
            
            var runTimeDetail = detailWrapper.Detail.ConvertToDetail();
            
            Assert.That(runTimeDetail is IDetail<List<Texture2D>>, Is.True);
            Assert.That(runTimeDetail is ICollectionDetail<Texture2D>, Is.True);
            Assert.That(runTimeDetail.Name, Is.EqualTo("detailName"));
            Assert.That(runTimeDetail.GetObject(), Is.EquivalentTo(new List<Texture2D>() { texture }));
        }
        
        [Test]
        public void DetailIsCreatedFromSprite()
        {
            var detailWrapper = new SerializedDetailWrapper<Sprite>("detailName");
            var sprite = Sprite.Create(new Texture2D(200, 200), new Rect(0, 0, 200, 200), Vector2.zero);
            detailWrapper.AddValue(sprite);
            
            var runTimeDetail = detailWrapper.Detail.ConvertToDetail();
            
            Assert.That(runTimeDetail is IDetail<List<Sprite>>, Is.True);
            Assert.That(runTimeDetail is ICollectionDetail<Sprite>, Is.True);
            Assert.That(runTimeDetail.Name, Is.EqualTo("detailName"));
            Assert.That(runTimeDetail.GetObject(), Is.EquivalentTo(new List<Sprite>() { sprite }));
        }

        [Test]
        public void DetailIsCreatedFromTemplate()
        {
            var detailWrapper = new SerializedDetailWrapper<IReference>("detailName");
            var template = ScriptableObject.CreateInstance<ReferenceTemplate>();
            template.Reference = new BaseSerializedReference();
            template.Reference.UpdateSerializedDetails(new List<IDetail>()
            {
                new BaseDetail<int>("TestOne"),
                new BaseDetail<float>("TestTwo")
            });
            
            detailWrapper.AddValue(template);
            var runTimeDetail = detailWrapper.Detail.ConvertToDetail();
            
            Assert.That(runTimeDetail is IDetail<List<IReference>>, Is.True);
            Assert.That(runTimeDetail is ICollectionDetail<IReference>, Is.True);
            Assert.That(runTimeDetail.Name, Is.EqualTo("detailName"));

            var referenceValue = (runTimeDetail.GetObject() as List<IReference>)[0];
            Assert.That(referenceValue.GetDetail("TestOne"), Is.Not.Null);
            Assert.That(referenceValue.GetDetail<int>("TestOne"), Is.Not.Null);
            Assert.That(referenceValue.GetDetail("TestTwo"), Is.Not.Null);
            Assert.That(referenceValue.GetDetail<float>("TestTwo"), Is.Not.Null);
        }
        
        public class SerializedDetailWrapper<T>
        {
            public readonly BaseSerializedCollectionDetail Detail;

            public SerializedDetailWrapper(string name)
            {
                Detail = new BaseSerializedCollectionDetail(name, typeof(T));
            }

            public SerializedDetailWrapper(string name, List<T> values) : this(name)
            {
                foreach (var value in values)
                {
                    AddValue(value);
                }
            }
            
            public void AddValue(ReferenceTemplate template)
            {
                var newDetail = new BaseSerializedDetail("", typeof(T));
                newDetail.SetValue(template);
                
                AddToCollection(newDetail);
            }

            public void AddValue(T value)
            {
                var newDetail = new BaseSerializedDetail("", typeof(T));
                newDetail.SetValue(value);
                
               AddToCollection(newDetail);
            }

            private void AddToCollection(BaseSerializedDetail detail)
            {
                var collection = (List<BaseSerializedDetail>)typeof(BaseSerializedCollectionDetail).GetField("_collection", 
                    BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(Detail);
                collection?.Add(detail);
            }
        }
    }
}