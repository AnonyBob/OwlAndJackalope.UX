#if USE_ADDRESSABLES
using UnityEngine;
using UnityEngine.UI;

namespace OwlAndJackalope.UX.Runtime.DetailBinders.AddressableBinders
{
    [RequireComponent(typeof(Image))]
    public class AddressableImageBinder : BaseAddressableBinder<Sprite>
    {
        [SerializeField]
        private Image _image;
        
        [SerializeField]
        private bool _useOverrideSprite;
        
        protected override void Start()
        {
            if (_image == null)
            {
                _image = GetComponent<Image>();    
            }
            base.Start();
        }
        
        protected override void UseAsset(Sprite asset)
        {
            if (_image != null)
            {
                if (_useOverrideSprite)
                {
                    _image.overrideSprite = asset;
                }
                else
                {
                    _image.sprite = asset;
                }
            }
        }
    }
}
#endif