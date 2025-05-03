using UnityEngine;

namespace JH
{
    public class WorldSoundFXManager : MonoBehaviour
    {
        private static WorldSoundFXManager _instance;
        public static WorldSoundFXManager instance => _instance;

        [Header("Action Sounds")]
        public AudioClip dodgeSFX;

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
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
