using UnityEngine;
using UnityEngine.UI;
namespace JH
{
    public class UI_StatBar : MonoBehaviour
    {
        private Slider slider;
        private RectTransform rectTransform;
        // Variable to scale bar size depending on stat
        [Header("Bar Options")]
        [SerializeField] protected bool scaleBarLengthWithStats = true;
        [SerializeField] protected float widthScaleMultiplier = 1.0f;

        // TODO : secondary bar that indicates how much resource has been consumed/ depleted

        protected virtual void Awake()
        {
            slider = GetComponent<Slider>();
            rectTransform = GetComponent<RectTransform>();
        }

        public virtual void SetStat(float newValue)
        {
            slider.value = newValue;
        }

        public virtual void SetMaxStat(float maxValue)
        {
            slider.maxValue = maxValue;
            slider.value = maxValue;

            if (scaleBarLengthWithStats)
            {
                // scale transform of this object
                rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMultiplier, rectTransform.sizeDelta.y);
                // reset position of bar after resizing it based on their layout group settings
                PlayerUIManager.instance.playerHUDManager.RefreshHUD();
            }
        }
    }
}
