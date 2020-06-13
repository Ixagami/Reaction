using UnityEngine;
using UnityEngine.EventSystems;

public class TouchController : MonoBehaviour {
    public void OnClick(BaseEventData ed) {
        PointerEventData ev = ed as PointerEventData;

        var ray = Camera.main.ScreenPointToRay(ev.position);
        if (Physics.Raycast(ray, out var hit, 1000f)) {
            var fig = hit.collider.gameObject.GetComponent<Figure>();
            if (fig != null) {
                fig.Press();
            }
        }
        
    }
}