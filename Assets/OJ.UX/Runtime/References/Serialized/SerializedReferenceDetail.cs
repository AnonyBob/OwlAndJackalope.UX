using System;
using UnityEngine;

namespace OJ.UX.Runtime.References.Serialized
{
    [System.Serializable, SerializedDetailDisplay("Reference")]
    public class SerializedReferenceDetail : SerializedConversionValueDetail<SerializedReferenceTemplate, IReference>
    {
        protected override IReference ConvertToFinalValue(SerializedReferenceTemplate value)
        {
            return value.CreateReference();
        }

        protected override SerializedReferenceTemplate ConvertToStoredValue(IReference value)
        {
            throw new NotImplementedException();
        }
        
        public override void RespondToChangesInRuntimeDetail()
        {
            // if (RuntimeDetail != null && PreviousRuntimeVersion != RuntimeDetail.Version)
            // {
            //     PreviousRuntimeVersion = RuntimeDetail.Version;
            //     Value =  ConvertToStoredValue(RuntimeDetail.Value);
            // }
        }

        public override void ForceUpdateRuntimeDetail()
        {
            // if (CanMutateRuntimeDetail())
            // {
            //     MutableRuntimeDetail.Value =  ConvertToFinalValue(Value);
            // }
        }
    }
}