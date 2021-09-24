using System;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.DetailBinders
{
    public abstract class TextFormattingProvider : MonoBehaviour
    {
        public abstract IFormatProvider GetProvider();
    }
}


