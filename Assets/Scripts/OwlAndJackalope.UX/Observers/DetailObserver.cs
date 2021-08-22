using OwlAndJackalope.UX.Data;

namespace OwlAndJackalope.UX.Observers
{
    [System.Serializable]
    public class DetailObserver : AbstractDetailObserver
    {
        public override IDetail Detail
        {
            get => _detail;
            set => _detail = value;
        }

        private IDetail _detail;
    }

    [System.Serializable]
    public class DetailObserver<T> : AbstractDetailObserver
    {
        public override IDetail Detail
        {
            get => _detail;
            set => _detail = value as IDetail<T>;
        }

        public T Value => IsSet ? _detail.GetValue() : default;

        private IDetail<T> _detail;
    }
}