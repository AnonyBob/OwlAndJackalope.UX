using System;
using UnityEngine;

namespace OwlAndJackalope.UX.Observers
{
    public class DetailTypeAttribute : PropertyAttribute
    {
        public readonly Type[] AcceptableTypes;

        public DetailTypeAttribute(params Type[] acceptableTypes)
        {
            AcceptableTypes = acceptableTypes;
        }
    }
}