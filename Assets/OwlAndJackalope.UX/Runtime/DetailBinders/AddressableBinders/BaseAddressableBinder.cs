using System.Collections.Generic;
using OwlAndJackalope.UX.Runtime.Observers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace OwlAndJackalope.UX.Runtime.DetailBinders.AddressableBinders
{
    public abstract class BaseAddressableBinder<TAsset> : BaseDetailBinder where TAsset : class
    {
        public bool IsLoading => _isLoading;
        
        [SerializeField, DetailType(typeof(AssetReference))]
        private DetailObserver<AssetReference> _observer;
        
        private AssetReference _previousAssetReference;
        private bool _isLoading = false;

        protected virtual void Start()
        {
            _observer.Initialize(_referenceModule.Reference, HandleAssetLoading, false);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ClearPreviousAssetReference();
        }

        protected abstract void UseAsset(TAsset asset);

        private void HandleAssetLoading()
        {
            var assetRef = _observer.Value;
            ClearPreviousAssetReference();

            _previousAssetReference = assetRef;
            _isLoading = true;
            
            if (!_previousAssetReference.RuntimeKeyIsValid())
            {
                HandleLoadedAsset(null);
            }
            else if (!_previousAssetReference.OperationHandle.IsValid())
            {
                _previousAssetReference.LoadAssetAsync<TAsset>();
                _previousAssetReference.OperationHandle.Completed += HandleLoaded;  
            }
            else if (_previousAssetReference.OperationHandle.IsDone)
            {
                HandleLoaded(_previousAssetReference.OperationHandle);
            }
            else
            {
                _previousAssetReference.OperationHandle.Completed += HandleLoaded;    
            }
        }

        private void HandleLoaded(AsyncOperationHandle handle)
        {
            if (handle.Result is TAsset asset)
            {
                HandleLoadedAsset(asset);
            }
        }
        
        private void HandleLoadedAsset(TAsset asset)
        {
            UseAsset(asset);
            _isLoading = false;
        }

        private void ClearPreviousAssetReference()
        {
            if (_previousAssetReference != null && _previousAssetReference.OperationHandle.IsValid())
            {
                _previousAssetReference.OperationHandle.Completed -= HandleLoaded;
                _previousAssetReference.ReleaseAsset();
            }

            _previousAssetReference = null;
        }

        protected override IEnumerable<AbstractDetailObserver> GetDetailObservers()
        {
            yield return _observer;
        }
    }
}