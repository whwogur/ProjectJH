using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JH
{
    public class UI_Adjust_ScrollWheel : MonoBehaviour
    {
        [SerializeField] GameObject currentSelected;
        [SerializeField] GameObject previousSelected;
        [SerializeField] RectTransform currentSelectedTransform;

        [SerializeField] RectTransform contentPanel;
        [SerializeField] ScrollRect scrollRect;


        private void Update()
        {
            currentSelected = EventSystem.current.currentSelectedGameObject;
            if (null != currentSelected && previousSelected != currentSelected)
            {
                previousSelected = currentSelected;
                currentSelectedTransform = currentSelected.GetComponent<RectTransform>();
                SnapTo(currentSelectedTransform);
            }
        }

        private void SnapTo(RectTransform targetRectTransform)
        {
            Canvas.ForceUpdateCanvases();

            Vector2 newPos = 
                (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position) 
                - (Vector2)scrollRect.transform.InverseTransformPoint(targetRectTransform.position);

            // only lock the position on the Y AXIS
            newPos.x = 0;

            contentPanel.anchoredPosition = newPos;
        }
    }
}
