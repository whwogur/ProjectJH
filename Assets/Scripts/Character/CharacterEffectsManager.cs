using UnityEngine;

namespace JH
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        CharacterManager character;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }
        // process instant effects ( taking damage, healing.. etc )
        public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
        {
            effect.ProcessEffect(character);
        }
        // process timed effects ( DOT, build up damage.. etc )

        // process static effects ( adding / removing buffs.. etc )
    }
}
