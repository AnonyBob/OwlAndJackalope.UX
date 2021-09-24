using OwlAndJackalope.UX.Runtime.Data;

namespace OwlAndJackalope.UX.Runtime.Observers
{
    [System.Serializable]
    public class DetailObserver : AbstractDetailObserver
    {
        public override IDetail Detail
        {
            get => _detail;
            protected set => _detail = value;
        }

        private IDetail _detail;
    }

    [System.Serializable]
    public class DetailObserver<T> : AbstractDetailObserver
    {
        public override IDetail Detail
        {
            get => _detail;
            protected set => _detail = value as IDetail<T>;
        }

        public T Value => IsSet ? _detail.GetValue() : default;

        private IDetail<T> _detail;
    }
    
    [System.Serializable]
    public class MutableDetailObserver<T> : AbstractDetailObserver
    {
        public override IDetail Detail
        {
            get => _mutableDetail;
            protected set => _mutableDetail = value as IMutableDetail<T>;
        }

        public T Value
        {
            get => IsSet ? _mutableDetail.GetValue() : default;
            set
            {
                if (IsSet)
                {
                    _mutableDetail.SetValue(value);
                }
            }
        } 

        private IMutableDetail<T> _mutableDetail;
    }
}