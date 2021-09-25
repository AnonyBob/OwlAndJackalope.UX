using OwlAndJackalope.UX.Runtime.Data.Serialized.Enums;
using UnityEngine;

namespace OwlAndJackalope.UX.Samples.UserScene
{
    public static class EnumTypeAdder
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        public static void InitializeEnums()
        {
            SerializedDetailEnumCache.AddEnumType(1, new EnumDetailCreator<EyeColor>(x => (EyeColor)x));
        }
    }
}


