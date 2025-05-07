using UnityEngine;

namespace JH
{
    public class PlayerStatsManager : CharacterStatsManager
    {
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            CalculateHealthBasedOnVitalityLevel(character.characterNetworkManager.vitality.Value);
            CalculateStaminaBasedOnEnduranceLevel(character.characterNetworkManager.endurance.Value);
        }
    }
}
