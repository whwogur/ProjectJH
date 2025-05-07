using UnityEngine;

namespace JH
{
    public class PlayerEffectsManager : CharacterEffectsManager
    {
        [Header("Debug Delete Later")]
        [SerializeField] InstantCharacterEffect effectTest;
        [SerializeField] bool processEffect = false;

        private void Update()
        {
            if (processEffect)
            {
                processEffect = false;
                /* make instantiated dynamic effect */
                InstantCharacterEffect effect = Instantiate(effectTest);
                ProcessInstantEffect(effect);
            }
        }
    }
}
