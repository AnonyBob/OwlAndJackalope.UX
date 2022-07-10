using System;
using UnityEngine;

namespace OJ.UX.Runtime.Binders
{
    public abstract class FormattingProvider : MonoBehaviour
    {
        public abstract IFormatProvider GetFormatter();
    }
}