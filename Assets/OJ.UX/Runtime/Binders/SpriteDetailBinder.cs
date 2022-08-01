using OJ.UX.Runtime.Binding;
using UnityEngine;
using UnityEngine.UI;

namespace OJ.UX.Runtime.Binders
{
    public class SpriteDetailBinder : AbstractDetailBinder
    {
        [SerializeField]
        private Observer<Sprite> _sprite;

        [SerializeField]
        private Image _image;

        [SerializeField]
        private bool _useOverride;

        private void Start()
        {
            if (_image == null)
            {
                _image = GetComponent<Image>();
            }
            
            _sprite.Initialize(HandleUpdate);
        }

        private void HandleUpdate()
        {
            if (_image != null)
            {
                if (_useOverride)
                    _image.overrideSprite = _sprite.Value;
                else
                    _image.sprite = _sprite.Value;
            }
        }

        private void OnDestroy()
        {
            _sprite.Destroy();
        }
    }
}