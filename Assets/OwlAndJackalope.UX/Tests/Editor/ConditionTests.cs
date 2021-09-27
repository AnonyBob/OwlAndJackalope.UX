using NUnit.Framework;
using OwlAndJackalope.UX.Runtime.Conditions;

namespace OwlAndJackalope.UX.Tests.Editor
{
    [TestFixture]
    public class ConditionTests
    {
        //bools
        [TestCase(Comparison.Equal, true, true, true)]
        [TestCase(Comparison.Equal, true, false, false)]
        [TestCase(Comparison.NotEqual, true, false, true)]
        [TestCase(Comparison.NotEqual, true, true, false)]
        
        //numbers
        [TestCase(Comparison.Equal, 1, 1, true)]
        [TestCase(Comparison.Equal, 1, 2, false)]
        [TestCase(Comparison.GreaterThan, 1, 2, true)]
        [TestCase(Comparison.GreaterThan, 1, 1, false)]
        [TestCase(Comparison.GreaterThanEqual, 1, 1, false)]
        [TestCase(Comparison.GreaterThanEqual, 1, 1, false)]
        [TestCase(Comparison.LessThan, 1, 1, false)]
        [TestCase(Comparison.LessThanEqual, 1, 1, false)]
        
        //timespans
        [TestCase(Comparison.Equal, 1, 1, true)]
        [TestCase(Comparison.Equal, 1, 2, false)]
        [TestCase(Comparison.GreaterThan, 1, 2, true)]
        [TestCase(Comparison.GreaterThan, 1, 1, false)]
        [TestCase(Comparison.GreaterThanEqual, 1, 1, false)]
        [TestCase(Comparison.GreaterThanEqual, 1, 1, false)]
        [TestCase(Comparison.LessThan, 1, 1, false)]
        [TestCase(Comparison.LessThanEqual, 1, 1, false)]
        
        //strings
        [TestCase(Comparison.Equal, 1, 1, true)]
        [TestCase(Comparison.Equal, 1, 2, false)]
        [TestCase(Comparison.GreaterThan, 1, 2, true)]
        [TestCase(Comparison.GreaterThan, 1, 1, false)]
        [TestCase(Comparison.GreaterThanEqual, 1, 1, false)]
        [TestCase(Comparison.GreaterThanEqual, 1, 1, false)]
        [TestCase(Comparison.LessThan, 1, 1, false)]
        [TestCase(Comparison.LessThanEqual, 1, 1, false)]
        public void DoComparisonCheck<TValue>(Comparison comparison, TValue one, TValue two, bool expectedResult)
        {
            
        }

        public void AndGroupWithNoConditionsIsAlwaysFalse()
        {
            
        }

        public void AndGroupWithOneConditionIsSameAsCondition()
        {
            
        }

        public void AndGroupWithMultipleConditionsIsTrueWhenAllAreTrue()
        {
            
        }

        public void AndGroupWithMultipleConditionsIsFalseWhenAnyAreFalse()
        {
            
        }

        public void ConvertToConditionWithValueTypeParameterTwo()
        {
            
        }

        public void ConvertToConditionWithDetailTypeParameterTwo()
        {
            
        }

        public void ConvertAndGroupCreatesSubConditions()
        {
            
        }
    }
}