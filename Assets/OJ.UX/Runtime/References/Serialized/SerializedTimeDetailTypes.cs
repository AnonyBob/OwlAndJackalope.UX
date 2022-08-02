using System;

namespace OJ.UX.Runtime.References.Serialized
{
    [Serializable, SerializedDetailDisplay("TimeSpan")]
    public class SerializedTimeSpanDetail : SerializedConversionValueDetail<long, TimeSpan>
    {
        protected override TimeSpan ConvertToFinalValue(long value)
        {
            return TimeSpan.FromTicks(value);
        }

        protected override long ConvertToStoredValue(TimeSpan value)
        {
            return value.Ticks;
        }
    }
    
    [Serializable, SerializedDetailDisplay("DateTime")]
    public class SerializedDateTimeDetail : SerializedConversionValueDetail<long, DateTime>
    {
        protected override DateTime ConvertToFinalValue(long value)
        {
            return new DateTime(value, DateTimeKind.Utc);
        }

        protected override long ConvertToStoredValue(DateTime value)
        {
            return value.Ticks;
        }
    }
}