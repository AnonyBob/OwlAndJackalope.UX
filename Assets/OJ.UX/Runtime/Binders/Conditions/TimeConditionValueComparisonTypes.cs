using System;

namespace OJ.UX.Runtime.Binders.Conditions
{
    [System.Serializable, ConditionDisplay("TimeSpan Comparison", "TimeSpan")]
    public class ConditionTimeSpanComparison : ConditionConversionValueComparison<long, TimeSpan>
    {
        protected override TimeSpan ConvertToFinalValue(long storedValue)
        {
            return TimeSpan.FromTicks(storedValue);
        }

        protected override long ConvertToStoredValue(TimeSpan timeSpan)
        {
            return timeSpan.Ticks;
        }
    }
}