using System.Linq;
using NUnit.Framework;
using OwlAndJackalope.UX.Runtime.Conditions;
using OwlAndJackalope.UX.Runtime.Conditions.Serialized;
using OwlAndJackalope.UX.Runtime.Data;
using OwlAndJackalope.UX.Runtime.Data.Extensions;

namespace OwlAndJackalope.UX.Tests.Editor
{
    [TestFixture]
    public class ConditionTests
    {
        private static object[] ComparisonCases = new object[]
        {
            //bools
            new object[] { Comparison.Equal, true, true, true },
            new object[] { Comparison.Equal, true, true, true},
            new object[] { Comparison.Equal, true, false, false},
            new object[] { Comparison.NotEqual, true, false, true},
            new object[] { Comparison.NotEqual, true, true, false},
            
            //numbers
            new object[] { Comparison.Equal, 1, 1, true},
            new object[] { Comparison.Equal, 1, 2, false},
            new object[] { Comparison.GreaterThan, 2, 1, true},
            new object[] { Comparison.GreaterThan, 1, 1, false},
            new object[] { Comparison.GreaterThan, 1, 2, false},
            new object[] { Comparison.GreaterThanEqual, 2, 1, true},
            new object[] { Comparison.GreaterThanEqual, 1, 1, true},
            new object[] { Comparison.GreaterThanEqual, 1, 2, false},
            new object[] { Comparison.LessThan, 1, 2, true},
            new object[] { Comparison.LessThan, 1, 1, false},
            new object[] { Comparison.LessThan, 2, 1, false},
            new object[] { Comparison.LessThanEqual, 1, 1, true},
            new object[] { Comparison.LessThanEqual, 2, 1, false},
            new object[] { Comparison.LessThanEqual, 1, 2, true},

            //strings
            new object[] { Comparison.Equal, "a", "a", true},
            new object[] { Comparison.Equal, "a", "b", false},
            new object[] { Comparison.GreaterThan, "b", "a", true},
            new object[] { Comparison.GreaterThan, "a", "b", false},
            new object[] { Comparison.GreaterThan, "a", "a", false},
            new object[] { Comparison.GreaterThanEqual, "b", "a", true},
            new object[] { Comparison.GreaterThanEqual, "b", "b", true},
            new object[] { Comparison.GreaterThanEqual, "a", "b", false},
            new object[] { Comparison.LessThan, "a", "b", true},
            new object[] { Comparison.LessThan, "a", "a", false},
            new object[] { Comparison.LessThan, "b", "a", false},
            new object[] { Comparison.LessThanEqual, "a", "b", true},
            new object[] { Comparison.LessThanEqual, "b", "a", false},
            new object[] { Comparison.LessThanEqual, "b", "b", true},
        };
        
        [TestCaseSource(nameof(ComparisonCases))]
        public void DoComparisonCheck<TValue>(Comparison comparison, TValue one, TValue two, bool expectedResult)
        {
            var condition = new BaseRuntimeCondition<TValue>(
                new Parameter() { Type = ParameterType.Detail, Name = "testing" },
                new BaseDetail<TValue>("", two),
                comparison);

            var reference = new BaseReference(new IDetail[] { new BaseDetail<TValue>("testing", one) });
            Assert.That(condition.IsMet(reference), Is.EqualTo(expectedResult));
        }

        [TestCaseSource(nameof(ComparisonCases))]
        public void DoComparisonCheckTwoDetails<TValue>(Comparison comparison, TValue one, TValue two,
            bool expectedResult)
        {
            var condition = new BaseRuntimeCondition<TValue>(
                new Parameter() { Type = ParameterType.Detail, Name = "testing" },
                new Parameter() {Type = ParameterType.Detail, Name = "testing2"},
                comparison);
            
            var reference = new BaseReference(new IDetail[]
            {
                new BaseDetail<TValue>("testing", one),
                new BaseDetail<TValue>("testing2", two),
            });
            Assert.That(condition.IsMet(reference), Is.EqualTo(expectedResult));
        }

        [Test]
        public void ConditionReturnsUsedDetails()
        {
            var condition = new BaseRuntimeCondition<int>(
                new Parameter() { Type = ParameterType.Detail, Name = "testing" },
                new BaseDetail<int>("", 1),
                Comparison.Equal);
            
            Assert.That(condition.GetUsedDetails().Count(), Is.EqualTo(1));
            Assert.That(condition.GetUsedDetails().First(), Is.EqualTo("testing"));
            
            var doubleCondition = new BaseRuntimeCondition<int>(
                new Parameter() { Type = ParameterType.Detail, Name = "testing" },
                new Parameter() { Type = ParameterType.Detail, Name = "testing2" },
                Comparison.Equal);
            
            Assert.That(doubleCondition.GetUsedDetails().Count(), Is.EqualTo(2));
            Assert.That(doubleCondition.GetUsedDetails().Contains("testing"), Is.True);
            Assert.That(doubleCondition.GetUsedDetails().Contains("testing2"), Is.True);
        }

        [Test]
        public void AndGroupWithNoConditionsIsAlwaysFalse()
        {
            var reference = new BaseReference(new IDetail[] { new BaseDetail<int>("testing", 1) });
            var andGroup = new AndConditionGroup(null);
            Assert.That(andGroup.IsMet(reference), Is.False);
        }

        [Test]
        public void AndGroupWithOneConditionIsSameAsCondition()
        {
            var reference = new BaseReference(new IDetail[] { new BaseDetail<int>("testing", 1) });
            var condition = new BaseRuntimeCondition<int>(
                new Parameter() { Type = ParameterType.Detail, Name = "testing" },
                new BaseDetail<int>("", 1),
                Comparison.Equal);
            
            var andGroup = new AndConditionGroup(new ICondition[] { condition });
            Assert.That(andGroup.IsMet(reference), Is.EqualTo(condition.IsMet(reference)));
        }

        [Test]
        public void AndGroupWithMultipleConditionsIsTrueWhenAllAreTrue()
        {
            var reference = new BaseReference(new IDetail[]
            {
                new BaseDetail<int>("testing", 1),
                new BaseDetail<int>("testing2", 10)
            });
            var condition = new BaseRuntimeCondition<int>(
                new Parameter() { Type = ParameterType.Detail, Name = "testing" },
                new BaseDetail<int>("", 1),
                Comparison.Equal);
            var condition1 = new BaseRuntimeCondition<int>(
                new Parameter() { Type = ParameterType.Detail, Name = "testing2" },
                new BaseDetail<int>("", 10),
                Comparison.Equal);
            
            var andGroup = new AndConditionGroup(new ICondition[] { condition, condition1 });
            Assert.That(andGroup.IsMet(reference), Is.True);
        }

        [Test]
        public void AndGroupWithMultipleConditionsIsFalseWhenAnyAreFalse()
        {
            var reference = new BaseReference(new IDetail[]
            {
                new BaseDetail<int>("testing", 1),
                new BaseDetail<int>("testing2", 5) //False
            });
            var condition = new BaseRuntimeCondition<int>(
                new Parameter() { Type = ParameterType.Detail, Name = "testing" },
                new BaseDetail<int>("", 1),
                Comparison.Equal);
            var condition1 = new BaseRuntimeCondition<int>(
                new Parameter() { Type = ParameterType.Detail, Name = "testing2" },
                new BaseDetail<int>("", 10),
                Comparison.Equal);
            
            var andGroup = new AndConditionGroup(new ICondition[] { condition, condition1 });
            Assert.That(andGroup.IsMet(reference), Is.False);
        }

        [Test]
        public void ConvertToConditionWithValueTypeParameterTwo()
        {
            var reference = new BaseReference(new IDetail[]
            {
                new BaseDetail<int>("testing", 1),
                new BaseDetail<int>("testing2", 5) //False
            });
            
            var serializedCondition = BaseSerializedCondition.Create("testing", 23, Comparison.Equal);
            var condition = serializedCondition.ConvertToCondition();
            Assert.That(condition.IsMet(reference), Is.False);
            
            reference.GetMutable<int>("testing").SetValue(23);
            Assert.That(condition.IsMet(reference), Is.True);
        }

        [Test]
        public void ConvertToConditionWithDetailTypeParameterTwo()
        {
            var reference = new BaseReference(new IDetail[]
            {
                new BaseDetail<int>("testing", 1),
                new BaseDetail<int>("testing2", 5) //False
            });
            
            var serializedCondition = BaseSerializedCondition.Create<int>("testing", "testing2", Comparison.Equal);
            var condition = serializedCondition.ConvertToCondition();
            Assert.That(condition.IsMet(reference), Is.False);
            
            reference.GetMutable<int>("testing").SetValue(5);
            Assert.That(condition.IsMet(reference), Is.True);
        }

        [Test]
        public void ConvertAndGroupCreatesSubConditions()
        {
            var reference = new BaseReference(new IDetail[]
            {
                new BaseDetail<int>("testing", 1),
                new BaseDetail<int>("testing2", 5) //False
            });

            var serializedCondition = new SerializedConditionAndGroup(new BaseSerializedCondition[]
            {
                BaseSerializedCondition.Create<int>("testing", 23, Comparison.Equal),
                BaseSerializedCondition.Create<int>("testing", "testing2", Comparison.Equal)
            });
            
            var condition = serializedCondition.ConvertToCondition();
            Assert.That(condition.IsMet(reference), Is.False);
            
            reference.GetMutable<int>("testing").SetValue(23);
            Assert.That(condition.IsMet(reference), Is.False);
            
            reference.GetMutable<int>("testing2").SetValue(23);
            Assert.That(condition.IsMet(reference), Is.True);
        }
    }
}