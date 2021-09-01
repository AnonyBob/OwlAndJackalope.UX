using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.Data.Serialized
{
    /// <summary>
    /// Scriptable object containing a SerializedReference. This can be used to pull out serialization data
    /// or to initialize Experience's ReferenceModule with some starting parameters.
    /// </summary>
    [CreateAssetMenu(fileName = "Reference Template", menuName = "UXMachine/Reference Template", order = 0)]
    public class ReferenceTemplate : ScriptableObject
    {
        [SerializeField]
        public BaseSerializedReference Reference;
    }
}