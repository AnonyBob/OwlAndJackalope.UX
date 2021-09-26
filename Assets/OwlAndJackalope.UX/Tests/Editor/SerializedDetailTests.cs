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
    public class SerializedDetailTests
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
        public void TestDetailIsMadeFromData(DetailType detailType, Type systemType)
        {
            var serializedDetailType = typeof(BaseSerializedDetail);
            var typeField = serializedDetailType.GetField("_type", BindingFlags.Instance | BindingFlags.NonPublic);
            var enumField = serializedDetailType.GetField("_enumId", BindingFlags.Instance | BindingFlags.NonPublic);
            var detail = new BaseSerializedDetail("name", systemType);
            
            Assert.That(detail.Name, Is.EqualTo("name"));
            Assert.That(detail.Type, Is.EqualTo(systemType));
            
            Assert.That(typeField.GetValue(detail), Is.EqualTo(detailType));
            Assert.That(enumField.GetValue(detail), Is.EqualTo(0));
        }

        [Test]
        public void TestDetailIsMadeFromDataAsEnum()
        {
            try
            {
                SerializedDetailEnumCache.AddEnumType(12, new EnumDetailCreator<TestEnumOne>(x => (TestEnumOne)x));
                
                var serializedDetailType = typeof(BaseSerializedDetail);
                var typeField = serializedDetailType.GetField("_type", BindingFlags.Instance | BindingFlags.NonPublic);
                var enumField = serializedDetailType.GetField("_enumId", BindingFlags.Instance | BindingFlags.NonPublic);
                var detail = new BaseSerializedDetail("name", typeof(TestEnumOne));
                
                Assert.That(detail.Name, Is.EqualTo("name"));
                Assert.That(detail.Type, Is.EqualTo(typeof(TestEnumOne)));
               
                Assert.That(typeField.GetValue(detail), Is.EqualTo(DetailType.Enum));
                Assert.That(enumField.GetValue(detail), Is.EqualTo(12));
            }
            finally
            {
                SerializedDetailEnumCache.RemoveEnumType(12);
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
            detailWrapper.SetValue(value);

            var detailValue = detailWrapper.GetValue();
            var runTimeDetail = detailWrapper.Detail.ConvertToDetail();
            
            Assert.That(runTimeDetail is IDetail<TValue>, Is.True);
            Assert.That(runTimeDetail.Name, Is.EqualTo("detailName"));
            Assert.That(runTimeDetail.GetObject(), Is.EqualTo(detailValue));
        }

        [Test]
        public void DetailIsCreatedFromSerializedVector2()
        {
            var detailWrapper = new SerializedDetailWrapper<Vector2>("detailName");
            detailWrapper.SetValue(new Vector2(0.5f, 0.5f));

            var detailValue = detailWrapper.GetValue();
            var runTimeDetail = detailWrapper.Detail.ConvertToDetail();
            
            Assert.That(runTimeDetail is IDetail<Vector2>, Is.True);
            Assert.That(runTimeDetail.Name, Is.EqualTo("detailName"));
            Assert.That(runTimeDetail.GetObject(), Is.EqualTo(detailValue));
        }
        
        [Test]
        public void DetailIsCreatedFromSerializedVector3()
        {
            var detailWrapper = new SerializedDetailWrapper<Vector3>("detailName");
            detailWrapper.SetValue(new Vector3(0.5f, 0.5f, 1.5f));

            var detailValue = detailWrapper.GetValue();
            var runTimeDetail = detailWrapper.Detail.ConvertToDetail();
            
            Assert.That(runTimeDetail is IDetail<Vector3>, Is.True);
            Assert.That(runTimeDetail.Name, Is.EqualTo("detailName"));
            Assert.That(runTimeDetail.GetObject(), Is.EqualTo(detailValue));
        }
        
        [Test]
        public void DetailIsCreatedFromSerializedColor()
        {
            var detailWrapper = new SerializedDetailWrapper<Color>("detailName");
            detailWrapper.SetValue(new Color(0.5f, 0.5f, 0.5f, 1f));

            var detailValue = detailWrapper.GetValue();
            var runTimeDetail = detailWrapper.Detail.ConvertToDetail();
            
            Assert.That(runTimeDetail is IDetail<Color>, Is.True);
            Assert.That(runTimeDetail.Name, Is.EqualTo("detailName"));
            Assert.That(runTimeDetail.GetObject(), Is.EqualTo(detailValue));
        }
        
        [Test]
        public void DetailIsCreatedFromSerializedTimeSpan()
        {
            var detailWrapper = new SerializedDetailWrapper<TimeSpan>("detailName");
            detailWrapper.SetValue(new TimeSpan(2, 3, 4, 5));

            var detailValue = detailWrapper.GetValue();
            var runTimeDetail = detailWrapper.Detail.ConvertToDetail();
            
            Assert.That(runTimeDetail is IDetail<TimeSpan>, Is.True);
            Assert.That(runTimeDetail.Name, Is.EqualTo("detailName"));
            Assert.That(runTimeDetail.GetObject(), Is.EqualTo(detailValue));
        }

        [Test]
        public void DetailIsCreatedFromGameObject()
        {
            var detailWrapper = new SerializedDetailWrapper<GameObject>("detailName");
            detailWrapper.SetValue(new GameObject("detailNameGO"));

            var detailValue = detailWrapper.GetValue();
            var runTimeDetail = detailWrapper.Detail.ConvertToDetail();
            
            Assert.That(runTimeDetail is IDetail<GameObject>, Is.True);
            Assert.That(runTimeDetail.Name, Is.EqualTo("detailName"));
            Assert.That(runTimeDetail.GetObject(), Is.EqualTo(detailValue));
        }
        
        [Test]
        public void DetailIsCreatedFromTexture()
        {
            var detailWrapper = new SerializedDetailWrapper<Texture2D>("detailName");
            detailWrapper.SetValue(new Texture2D(200, 200));

            var detailValue = detailWrapper.GetValue();
            var runTimeDetail = detailWrapper.Detail.ConvertToDetail();
            
            Assert.That(runTimeDetail is IDetail<Texture2D>, Is.True);
            Assert.That(runTimeDetail.Name, Is.EqualTo("detailName"));
            Assert.That(runTimeDetail.GetObject(), Is.EqualTo(detailValue));
        }
        
        [Test]
        public void DetailIsCreatedFromSprite()
        {
            var detailWrapper = new SerializedDetailWrapper<Sprite>("detailName");
            detailWrapper.SetValue(Sprite.Create(new Texture2D(200, 200), new Rect(0, 0, 200, 200), Vector2.zero));

            var detailValue = detailWrapper.GetValue();
            var runTimeDetail = detailWrapper.Detail.ConvertToDetail();
            
            Assert.That(runTimeDetail is IDetail<Sprite>, Is.True);
            Assert.That(runTimeDetail.Name, Is.EqualTo("detailName"));
            Assert.That(runTimeDetail.GetObject(), Is.EqualTo(detailValue));
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
            
            detailWrapper.SetValue(template);
            var detailValue = detailWrapper.GetValue();
            var runTimeDetail = detailWrapper.Detail.ConvertToDetail();
            
            Assert.That(runTimeDetail is IDetail<IReference>, Is.True);
            Assert.That(runTimeDetail.Name, Is.EqualTo("detailName"));

            var referenceValue = (IReference)runTimeDetail.GetObject();
            Assert.That(referenceValue.GetDetail("TestOne"), Is.Not.Null);
            Assert.That(referenceValue.GetDetail<int>("TestOne"), Is.Not.Null);
            Assert.That(referenceValue.GetDetail("TestTwo"), Is.Not.Null);
            Assert.That(referenceValue.GetDetail<float>("TestTwo"), Is.Not.Null);
        }

        private class SerializedDetailWrapper<T>
        {
            public readonly BaseSerializedDetail Detail;

            public SerializedDetailWrapper(string name)
            {
                Detail = new BaseSerializedDetail(name, typeof(T));
            }

            public T GetValue()
            {
                return Detail.GetValue<T>();
            }

            public void SetValue(ReferenceTemplate template)
            {
                var detailType = typeof(BaseSerializedDetail);
                var referenceField = detailType.GetField("_referenceValue", BindingFlags.Instance | BindingFlags.NonPublic);
                referenceField.SetValue(Detail, template);
            }
            
            public void SetValue(T value)
            {
                var detailType = typeof(BaseSerializedDetail);
                var valueField = detailType.GetField("_value", BindingFlags.Instance | BindingFlags.NonPublic);
                var stringField = detailType.GetField("_stringValue", BindingFlags.Instance | BindingFlags.NonPublic);
                var referenceField = detailType.GetField("_referenceValue", BindingFlags.Instance | BindingFlags.NonPublic);
                var vectorField = detailType.GetField("_vectorValue", BindingFlags.Instance | BindingFlags.NonPublic);
                
                var type = Detail.GetType();
                if (value is bool boolValue)
                {
                    valueField.SetValue(Detail, boolValue ? 1 : 0);
                }
                else if (value is int intValue)
                {
                    valueField.SetValue(Detail, intValue + 0.1);
                }
                else if (value is long longValue)
                {
                    valueField.SetValue(Detail, longValue + 0.1);
                }
                else if (value is float floatValue)
                {
                    valueField.SetValue(Detail, floatValue);
                }
                else if (value is double doubleValue)
                {
                    valueField.SetValue(Detail, doubleValue);
                }
                else if (type.IsEnum)
                {
                    var enumValue = Convert.ToInt32(value);
                    valueField.SetValue(Detail, enumValue + 0.1);
                }
                else if (value is string stringValue)
                {
                    stringField.SetValue(Detail, stringValue);
                }
                else if (value is UnityEngine.Object objectField) //References, Gameobject, Sprite, Texture
                {
                    referenceField.SetValue(Detail, objectField);
                }
                else if (value is Vector4 vectorValue)
                {
                    vectorField.SetValue(Detail, vectorValue);
                }
                else if (value is TimeSpan timeSpan)
                {
                    valueField.SetValue(Detail, timeSpan.Ticks + 0.1);
                }
            }
        }
    }
}