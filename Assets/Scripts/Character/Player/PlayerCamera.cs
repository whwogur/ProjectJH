using UnityEngine;

namespace JH
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera Instance;
        public Camera cameraObject;

        private void Awake()
        {
            if (null == Instance)
            {
                Instance = this;
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

