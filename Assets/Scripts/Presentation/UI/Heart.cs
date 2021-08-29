using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Presentation.UI
{
    public class Heart : MonoBehaviour
    {
        public Color valid, invalid;

        public Image image;

        public class Factory : PlaceholderFactory<Heart>
        {
            public override Heart Create()
            {
                return base.Create();
            }
            public Heart Create(Transform transform)
            {
                var instance =  base.Create();
                
                instance.transform.SetParent(transform);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.rotation = Quaternion.identity;
                instance.transform.localScale = Vector3.one;

                return instance;
            }
        }

        public void SetValid(bool valid)
        {
            image.color = valid ? this.valid : invalid;
        }
    }
}