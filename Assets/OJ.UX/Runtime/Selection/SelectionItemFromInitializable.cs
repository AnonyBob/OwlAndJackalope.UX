using OJ.UX.Runtime.Versions;

namespace OJ.UX.Runtime.Selection
{
    public abstract class SelectionItemFromInitializable<TValue> : SelectionItem<TValue>
    {
        private IInitializableGameObject<TValue> _target;

        protected override void Awake()
        {
            base.Awake();
            _target = GetComponent<IInitializableGameObject<TValue>>();
        }
        
        public override TValue GetValue()
        {
            if (_target != null)
                return _target.Value;

            return default(TValue);
        }
    }
}