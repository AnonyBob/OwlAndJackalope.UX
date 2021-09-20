using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.Data.Serialized.Enums
{
    /// <summary>
    /// Create your own and add any enums that you would like to appear in the serialized details here.
    /// This is used to keep the scope of enums limited only to ones that are useful in the tools.
    /// </summary>
    public static class EnumTypeAdder
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        public static void InitializeEnums()
        {
            SerializedDetailEnumCache.AddEnumType(0, new EnumDetailCreator<DetailType>(x => (DetailType)x));
        }
    }
}