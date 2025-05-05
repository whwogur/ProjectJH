using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

namespace JH
{
    public class TitleScreenManager : MonoBehaviour
    {
        [Header("Menu Objects")]
        [SerializeField] GameObject titleScreenMainMenu;
        [SerializeField] GameObject titleScreenLoadMenu;

        [Header("Buttons")]
        [SerializeField] Button returnToMainMenuButton;
        [SerializeField] Button mainMenuLoadGameButton;

        public void StartNetworkAsHost()
        {
            NetworkManager.Singleton.StartHost();
        }

        public void StartNewGame()
        {
            WorldSaveGameManager.instance.CreateNewGame();
            StartCoroutine(WorldSaveGameManager.instance.LoadWorldScene());
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
    }
}

