using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OJ.UX.Runtime.References.Serialized
{
    [System.Serializable, SerializedDetailDisplay("Reference")]
    public class SerializedReferenceDetail : SerializedConversionValueDetail<SerializedReferenceTemplate, IReference>
    {
        protected override IReference ConvertToFinalValue(SerializedReferenceTemplate value)
        {
            if (value == null)
            {
                return new Reference();
            }
            return value.CreateReference();
        }

        protected override SerializedReferenceTemplate ConvertToStoredValue(IReference value)
        {
            return Value;
        }
        
        public override bool CanMutateRuntimeDetail() => false;
        
        public override void RespondToChangesInRuntimeDetail()
        {
            //Do nothing right now.
        }

        public override void ForceUpdateRuntimeDetail()
        {
            //Do nothing right now.
        }
    }
    
    [System.Serializable, SerializedDetailDisplay("Reference[]", "Lists")]
    public class SerializedReferenceListValueDetail : SerializedConversionValueDetail<List<SerializedReferenceTemplate>, List<IReference>>
    {
        protected override List<IReference> ConvertToFinalValue(List<SerializedReferenceTemplate> value)
        {
            return new List<IReference>(value.Select(v =>
            {
                if (v == null)
                {
                    return new Reference();
                }
                return v.CreateReference();
            }));
        }

        protected override List<SerializedReferenceTemplate> ConvertToStoredValue(List<IReference> value)
        {
            return Value;
        }

        public override bool CanMutateRuntimeDetail() => false;

        public override void RespondToChangesInRuntimeDetail()
        {
            //Do nothing right now.
        }

        public override void ForceUpdateRuntimeDetail()
        {
            //Do nothing right now.
        }
    }
}