using OwlAndJackalope.UX.Runtime.Data.Serialized;
using OwlAndJackalope.UX.Runtime.Data.Serialized.Enums;
using OwlAndJackalope.UX.Runtime.Testing;
using UnityEditor;
using UnityEngine.UI;

namespace OwlAndJackalope.UX.Editor.EnumExtensions
{
    /// <summary>
    /// Create your own and add any enums that you would like to appear in the serialized details here.
    /// This is used to keep the scope of enums limited only to ones that are useful in the tools.
    /// </summary>
    [InitializeOnLoad]
    public static class EnumTypeAdder
    {
        static EnumTypeAdder()
        {
            SerializedDetailEnumCache.AddEnumType(typeof(DetailType));
            SerializedDetailEnumCache.AddEnumType(typeof(Slider.Direction));
            SerializedDetailEnumCache.AddEnumType(typeof(EyeColor));
        }
    }
}