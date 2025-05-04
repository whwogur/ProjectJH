using UnityEngine;
using UnityEngine.UI;
namespace JH
{
    public class UI_StatBar : MonoBehaviour
    {
        private Slider slider;
        // Variable to scale bar size depending on stat
        // secondary bar that indicates how much resource has been consumed/ depleted

        protected virtual void Awake()
        {
            if (null == slider)
            {
                slider = GetComponent<Slider>();
            }
        }

        public virtual void SetStat(float newValue)
        {
            slider.value = newValue;
        }

        public virtual void SetMaxStat(float maxValue)
        {
            slider.maxValue = maxValue;
            slider.value = maxValue;
        }
    }
}
