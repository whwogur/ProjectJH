using UnityEngine;

namespace JH
{
    public class PlayerUI_HUDManager : MonoBehaviour
    {
        public UI_StatBar staminaBar;

        public void SetNewStaminaValue(float oldValue, float newValue)
        {
            staminaBar.SetStat(newValue);
        }

        public void SetMaxStaminaValue(float maxStamina)
        {
            staminaBar.SetMaxStat(maxStamina);
        }
    }
}
