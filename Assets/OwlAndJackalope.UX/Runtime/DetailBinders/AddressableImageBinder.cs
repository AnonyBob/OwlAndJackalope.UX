using System.Collections.Generic;
using OwlAndJackalope.UX.Runtime.Observers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace OwlAndJackalope.UX.Runtime.DetailBinders
{
    [RequireComponent(typeof(Image))]
    public class AddressableImageBinder : BaseDetailBinder
    {
        [SerializeField, DetailType(typeof(AssetReference))]
        private DetailObserver<AssetReference> _observer;

        private Image _image;
        private AssetReference _previousAssetReference;

        private void Start()
        {
            _image = GetComponent<Image>();
            _observer.Initialize(_referenceModule.Reference, HandleAssetLoading, false);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _previousAssetReference?.ReleaseAsset();
            _previousAssetReference = null;
        }

        private void HandleAssetLoading()
        {
            var assetRef = _observer.Value;
            _previousAssetReference?.ReleaseAsset();

            _previousAssetReference = assetRef;
            if (_previousAssetReference != null && _previousAssetReference.RuntimeKeyIsValid())
            {
                var handle = _previousAssetReference.LoadAssetAsync<Sprite>();
                handle.Completed += (handle) => LoadComplete(handle, _previousAssetReference);
            }
            else if(_image != null)
            {
                _image.overrideSprite = null;
            }
        }

        private void LoadComplete(AsyncOperationHandle<Sprite> handle, AssetReference assetReference)
        {
            if (_previousAssetReference != assetReference)
            {
                _previousAssetReference?.ReleaseAsset();
                return;
            }

            if (_image != null)
            {
                _image.overrideSprite = handle.Result;    
            }
        }
        
        protected override IEnumerable<AbstractDetailObserver> GetDetailObservers()
        {
            yield return _observer;
        }
    }
}