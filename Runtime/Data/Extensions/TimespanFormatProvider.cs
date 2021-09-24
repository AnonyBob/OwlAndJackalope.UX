using System;
using System.Text;

namespace OwlAndJackalope.UX.Runtime.Data.Extensions
{
    /// <summary>
    /// Custom Timespan formatter to handle time like this:
    /// 2d 3h 2m 20s
    ///
    /// Uses format of S[1-4] where the number indicates how many units we want to display.
    /// Ex: 2 days 3 hours 4 minutes 28 seconds (S2) displays 2d 3h because those are the largest units.
    /// Ex: 13 hours 8 minutes 13 seconds (S2) displays 13h 8m because those are the largest units.
    /// </summary>
    public class TimespanFormatProvider : IFormatProvider, ICustomFormatter
    {
        private const int MAX = 4;
        private const int MIN = 1;
        
        private readonly string _dayString;
        private readonly string _hourString;
        private readonly string _minuteString;
        private readonly string _secondString;
        private readonly StringBuilder _builder;

        public TimespanFormatProvider() : this("d", "h", "m", "s")
        {
            
        }

        public TimespanFormatProvider(string dayString, string hourString, string minuteString, string secondString)
        {
            _builder = new StringBuilder();
            _dayString = dayString;
            _hourString = hourString;
            _minuteString = minuteString;
            _secondString = secondString;
        }
        
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }

            return null;
        }
        
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg == null)
            {
                return string.Empty;
            }

            if (format.StartsWith("S"))
            {
                if (arg is TimeSpan timespan)
                {
                    return ApplyFormat(timespan, GetNumberOfUnits(format));
                }
            }

            if (arg is IFormattable formattable)
            {
                return formattable.ToString(format, formatProvider);
            }
            return arg.ToString();
        }

        private string ApplyFormat(TimeSpan timeSpan, int numberOfUnits)
        {
            _builder.Clear();
            var remainingUnits = numberOfUnits;
            
            AppendUnit(timeSpan.Days, _dayString, numberOfUnits, ref remainingUnits);
            AppendUnit(timeSpan.Hours, _hourString, numberOfUnits, ref remainingUnits);
            AppendUnit(timeSpan.Minutes, _minuteString, numberOfUnits, ref remainingUnits);
            AppendUnit(timeSpan.Seconds, _secondString, numberOfUnits, ref remainingUnits);

            if (remainingUnits == numberOfUnits)
                AppendUnit(0, _secondString);

            return _builder.ToString();
        }

        private int GetNumberOfUnits(string format)
        {
            var numberString = format.Substring(1, format.Length - 1);
            if (int.TryParse(numberString, out var numberOfUnits) && numberOfUnits >= MIN && numberOfUnits <= MAX)
            {
                return numberOfUnits;
            }

            return MAX;
        }

        private void AppendUnit(int unitValue, string unitString, int startingUnits, ref int remaining)
        {
            if (remaining > 0 && unitValue > 0)
            {
                if (startingUnits > remaining)
                    _builder.Append(" ");

                _builder.Append(unitValue);
                _builder.Append(unitString);

                remaining--;
            }
        }

        private void AppendUnit(int unitValue, string unitString)
        {
            _builder.Append(unitValue);
            _builder.Append(unitString);
        }
    }
}