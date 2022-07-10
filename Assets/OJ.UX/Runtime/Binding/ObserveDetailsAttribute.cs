using System;
using UnityEngine;

namespace OJ.UX.Runtime.Binding
{
    public class ObserveDetailsAttribute : PropertyAttribute
    {
        public readonly Type[] AcceptableTypes;
        
        public ObserveDetailsAttribute(params Type[] acceptableTypes)
        {
            AcceptableTypes = acceptableTypes;
        }
    }
}