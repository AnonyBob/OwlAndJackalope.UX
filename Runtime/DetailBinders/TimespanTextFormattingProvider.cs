using System;
using OwlAndJackalope.UX.Runtime.Data.Extensions;

namespace OwlAndJackalope.UX.Runtime.DetailBinders
{
    public class TimespanTextFormattingProvider : TextFormattingProvider
    {
        private TimespanFormatProvider _provider;
        
        public override IFormatProvider GetProvider()
        {
            if (_provider == null)
            {
                _provider = new TimespanFormatProvider();
            }

            return _provider;
        }
    }
}