using OJ.UX.Runtime.Binding;
using OJ.UX.Runtime.References;
using UnityEngine;

namespace OJ.UX.Runtime.Binders.ListBinders
{
    [CreateAssetMenu(menuName = "OJ.UX/Prefab Provider", fileName = "Simple Providers")]
    public class SimpleReferencePrefabProvider : AbstractPrefabProvider<IReference, ReferenceModule>
    {
        [SerializeField]
        private ReferenceModule _prefab;
        
        public override bool IsReady() => true;

        public override void Load(MonoBehaviour behavior)
        {
        }

        public override void Unload(MonoBehaviour behavior)
        {
        }

        public override ReferenceModule CreateElement(IReference reference, MonoBehaviour parent)
        {
            var parentTransform = parent != null ? parent.transform : null;
            var instance = Instantiate(_prefab, parentTransform);
            instance.Reference = reference;
            return instance;
        }
    }
}