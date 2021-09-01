using System;

namespace OwlAndJackalope.UX.Runtime.Data.Extensions
{
    public class TransformDetail<TIn, TOut> : IDetail<TOut>
    {
        public string Name { get; }
        
        public event Action VersionChanged;

        public long Version => _watchDetail.Version;

        private IDetail<TIn> _watchDetail;
        private Func<TIn, TOut> _transform;
        
        private TOut _cachedValue;
        private long _lastWatchVersion = -1; //Use negative 1 to ensure there no matches.
        
        public TransformDetail(string name, IDetail<TIn> initialDetail, Func<TIn, TOut> transform)
        {
            Name = name;
            _watchDetail = initialDetail;
            _transform = transform;

            _watchDetail.VersionChanged += HandleVersionUpdate;
        }

        ~TransformDetail()
        {
            _watchDetail.VersionChanged -= HandleVersionUpdate;
        }
        
        public TOut GetValue()
        {
            if (_lastWatchVersion != _watchDetail.Version)
            {
                _cachedValue = _transform.Invoke(_watchDetail.GetValue());
                _lastWatchVersion = _watchDetail.Version;
            }
            return _cachedValue;
        }
        
        public object GetObject()
        {
            return GetValue();
        }

        public Type GetObjectType()
        {
            return typeof(TOut);
        }

        private void HandleVersionUpdate()
        {
            VersionChanged?.Invoke();
        }
    }
    
    public class TransformDetail<TInOne, TInTwo, TOut> : IDetail<TOut>
    {
        public string Name { get; }

        public event Action VersionChanged;

        public long Version => _version;

        private IDetail<TInOne> _watchDetailOne;
        private IDetail<TInTwo> _watchDetailTwo;
        private Func<TInOne, TInTwo, TOut> _transform;
        
        private TOut _cachedValue;
        private long _lastOneVersion = -1; //Use negative 1 to ensure there no matches.
        private long _lastTwoVersion = -1; //Use negative 1 to ensure there no matches.

        private long _version;

        public TransformDetail(string name, IDetail<TInOne> initialDetailOne, IDetail<TInTwo> initialDetailTwo, 
            Func<TInOne, TInTwo, TOut> transform)
        {
            Name = name;
            _watchDetailOne = initialDetailOne;
            _watchDetailTwo = initialDetailTwo;
            
            _watchDetailOne.VersionChanged += HandleVersionUpdate;
            _watchDetailTwo.VersionChanged += HandleVersionUpdate;
            
            _transform = transform;
        }

        ~TransformDetail()
        {
            _watchDetailOne.VersionChanged -= HandleVersionUpdate;
            _watchDetailTwo.VersionChanged -= HandleVersionUpdate;
        }
        
        public TOut GetValue()
        {
            if (_lastOneVersion != _watchDetailOne.Version || _lastTwoVersion != _watchDetailTwo.Version)
            {
                _cachedValue = _transform.Invoke(_watchDetailOne.GetValue(), _watchDetailTwo.GetValue());
                _lastOneVersion = _watchDetailOne.Version;
                _lastTwoVersion = _watchDetailTwo.Version;
            }
            return _cachedValue;
        }
        
        public object GetObject()
        {
            return GetValue();
        }

        public Type GetObjectType()
        {
            return typeof(TOut);
        }

        private void HandleVersionUpdate()
        {
            _version++;
            VersionChanged?.Invoke();
        }
    }

    public static partial class DetailExtensions
    {
        /// <summary>
        /// Transforms a given detail into another data type using the provided transformation function.
        /// </summary>
        public static IDetail<TOut> Transform<TIn, TOut>(this IDetail<TIn> detail, 
            string name, Func<TIn, TOut> transform)
        {
            return new TransformDetail<TIn, TOut>(name, detail, transform);
        }
        
        /// <summary>
        /// Combines two different details into a new single output using a provided transformation function.
        /// </summary>
        public static IDetail<TOut> Combine<TInOne, TInTwo, TOut>(this IDetail<TInOne> detail, IDetail<TInTwo> other, 
            string name, Func<TInOne, TInTwo, TOut> transform)
        {
            return new TransformDetail<TInOne, TInTwo, TOut>(name, detail, other, transform);
        }
    }
}