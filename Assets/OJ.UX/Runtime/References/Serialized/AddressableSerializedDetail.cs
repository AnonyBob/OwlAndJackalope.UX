#if ADDRESSABLES_ACTIVE
using System;
using UnityEngine.AddressableAssets;

namespace OJ.UX.Runtime.References.Serialized
{
    [Serializable, SerializedDetailDisplay("AssetReference")]
    public class AddressableSerializedDetail : SerializedValueDetail<AssetReference>
    {
    }
}
    
#endif