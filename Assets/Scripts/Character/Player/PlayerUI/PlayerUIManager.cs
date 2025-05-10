using UnityEngine;
using Unity.Netcode;

namespace JH
{
    public class PlayerUIManager : MonoBehaviour
    {
        public static PlayerUIManager instance;
        [Header("NETWORK JOIN")]
        [SerializeField] bool startGameAsClient;

        [HideInInspector] public PlayerUI_HUDManager playerHUDManager;
        [HideInInspector] public PlayerUI_PopupManager playerPopupManager;

        private void Awake()
        {
            if (null == instance)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            playerHUDManager = GetComponentInChildren<PlayerUI_HUDManager>();
            playerPopupManager = GetComponentInChildren<PlayerUI_PopupManager>();
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (startGameAsClient)
            {
                startGameAsClient = false;
                NetworkManager.Singleton.Shutdown();

                NetworkManager.Singleton.StartClient();
            }
        }
    }
}
