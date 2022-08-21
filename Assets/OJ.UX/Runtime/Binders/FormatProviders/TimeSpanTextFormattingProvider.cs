using System;

namespace OJ.UX.Runtime.Binders.FormatProviders
{
    public class TimeSpanTextFormattingProvider : FormattingProvider
    {
        private TimeSpanFormatProvider _provider;

        public override IFormatProvider GetFormatter()
        {
            if (_provider == null)
            {
                _provider = new TimeSpanFormatProvider();
            }

            return _provider;
        }
    }
}