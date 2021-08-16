using System;
using UnityEngine;

namespace OwlAndJackalope.UX.Observers
{
    public class DetailNameAttribute : PropertyAttribute
    {
        public readonly Type[] AcceptableTypes;

        public DetailNameAttribute(params Type[] acceptableTypes)
        {
            AcceptableTypes = acceptableTypes;
        }
    }
}