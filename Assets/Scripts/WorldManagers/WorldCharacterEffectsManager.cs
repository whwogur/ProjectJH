using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class WorldCharacterEffectsManager : MonoBehaviour
    {
        private static WorldCharacterEffectsManager _instance;
        public static WorldCharacterEffectsManager instance => _instance;

        [Header("Damage")]
        public TakeDamageEffect takeDamageEffect;

        [SerializeField] List<InstantCharacterEffect> instantEffects;

        private void Awake()
        {
            if (null == _instance)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            GenerateEffectIDs();
        }

        private void GenerateEffectIDs()
        {
            for (int i = 0; i < instantEffects.Count; ++i)
            {
                instantEffects[i].instantEffectID = i; // TEMP
            }
            
        }
    }
}
