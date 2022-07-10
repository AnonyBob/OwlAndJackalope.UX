using System;
using System.Collections.Generic;

namespace OJ.UX.Runtime.Binders.Conditions
{
    public enum ComparisonType
    {
        Equal = 0,
        NotEqual = 1,
        GreaterThan = 2,
        GreaterThanEqual = 3,
        LessThan = 4,
        LessThanEqual = 5,
    }

    public static class ComparisonTypeUtility
    {
        public static bool DoComparison<TValue>(this TValue one, ComparisonType comparisonType, TValue two)
        {
            var compare = Comparer<TValue>.Default.Compare(one, two);
            switch (comparisonType)
            {
                case ComparisonType.Equal:
                    return compare == 0;
                case ComparisonType.NotEqual:
                    return compare != 0;
                case ComparisonType.GreaterThan:
                    return compare > 0;
                case ComparisonType.GreaterThanEqual:
                    return compare >= 0;
                case ComparisonType.LessThan:
                    return compare < 0;
                case ComparisonType.LessThanEqual:
                    return compare <= 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}