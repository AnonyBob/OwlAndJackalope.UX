using UnityEngine;

namespace OJ.UX.Runtime.Binders.ListBinders
{
    [CreateAssetMenu(menuName = "OJ.UX/String Prefab Provider", fileName = "Simple Providers")]
    public class SimpleStringPrefabProvider : AbstractPrefabProvider<string, StringInitializationObject>
    {
        [SerializeField]
        private StringInitializationObject _prefab;
        
        public override bool IsReady() => true;

        public override void Load(MonoBehaviour behavior)
        {
        }

        public override void Unload(MonoBehaviour behavior)
        {
        }

        public override StringInitializationObject CreateElement(string value, MonoBehaviour parent)
        {
            var parentTransform = parent != null ? parent.transform : null;
            var instance = Instantiate(_prefab, parentTransform);
            instance.Initialize(value);
            return instance;
        }
    }
}