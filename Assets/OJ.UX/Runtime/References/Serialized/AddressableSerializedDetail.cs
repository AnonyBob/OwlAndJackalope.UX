using System;
using UnityEngine.AddressableAssets;

namespace OJ.UX.Runtime.References.Serialized
{
#if ADDRESSABLES_ACTIVE
    
    [Serializable, SerializedDetailDisplay("AssetReference")]
    public class AddressableSerializedDetail : SerializedValueDetail<AssetReference>
    {
    }
    
#endif
}