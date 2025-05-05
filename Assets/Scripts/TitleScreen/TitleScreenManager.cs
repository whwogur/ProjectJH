using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

namespace JH
{
    public class TitleScreenManager : MonoBehaviour
    {
        public static TitleScreenManager instance;

        [Header("Menu Objects")]
        [SerializeField] GameObject titleScreenMainMenu;
        [SerializeField] GameObject titleScreenLoadMenu;

        [Header("Buttons")]
        [SerializeField] Button mainMenuNewGameButton;
        [SerializeField] Button returnToMainMenuButton;
        [SerializeField] Button mainMenuLoadGameButton;

        [Header("Popups")]
        [SerializeField] GameObject noCharacterSlotsPopup;
        [SerializeField] Button noCharacterSlotsOK;

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
        }
        public void StartNetworkAsHost()
        {
            NetworkManager.Singleton.StartHost();
        }

        public void StartNewGame()
        {
            WorldSaveGameManager.instance.AttemptCreateNewGame();
        }

        public void OpenLoadGameMenu()
        {
            // close main menu
            titleScreenMainMenu.SetActive(false);
            // open load menu
            titleScreenLoadMenu.SetActive(true);

            // Select the RETURN TO MAIN MENU as default
            returnToMainMenuButton.Select();
        }

        public void CloseLoadGameMenu()
        {
            // close load menu
            titleScreenLoadMenu.SetActive(false);
            // open main menu
            titleScreenMainMenu.SetActive(true);

            // Select the LoadMenu button as default
            mainMenuLoadGameButton.Select();
        }

        public void DisplayNoFreeCharacterSlotsPopup()
        {
            noCharacterSlotsPopup.SetActive(true);
            noCharacterSlotsOK.Select();
        }

        public void CloseNoFreeCharacterSlotsPopup()
        {
            noCharacterSlotsPopup.SetActive(false);
            mainMenuNewGameButton.Select();
        }
    }
}

