using UnityEngine;
namespace Mkey {
    [ExecuteInEditMode]
    public class SpriteText : MonoBehaviour
    {
        void Start()
        {
            //  var parent = transform.parent;
            //  var parentRenderer = parent.GetComponent<Renderer>();
            var renderer = GetComponent<Renderer>();
            renderer.sortingOrder = SortingOrder.LinesButton; //  renderer.sortingLayerID = parentRenderer.sortingLayerID;

            // var spriteTransform = parent.transform;
            // var text = GetComponent<TextMesh>();
            // var pos = spriteTransform.position;
            // text.text = string.Format("{0}, {1}", pos.x, pos.y);
        }
    }
}
