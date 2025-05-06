using UnityEngine;
using TMPro;

namespace JH
{
    public class UI_Character_SaveSlot : MonoBehaviour
    {
        SaveGameDataWriter saveDataWriter;

        [Header("Game Slot")]
        public CharacterSlot characterSlot;

        [Header("Character Info")]
        public TextMeshProUGUI characterName;
        public TextMeshProUGUI timePlayed;

        private void OnEnable()
        {
            LoadSaveSlot();
        }

        private void LoadSaveSlot()
        {
            saveDataWriter = new SaveGameDataWriter();
            saveDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
            saveDataWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlot(characterSlot);

            if (saveDataWriter.CheckIfFileAlreadyExists(saveDataWriter.saveDataDirectoryPath, saveDataWriter.saveFileName))
            {
                characterName.text = WorldSaveGameManager.instance.GetCharacterName(characterSlot);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void LoadGameFromCharacterSlot()
        {
            WorldSaveGameManager.instance.currentCharacterSlot = characterSlot;
            WorldSaveGameManager.instance.LoadGame();
        }

        public void SelectCurrentSlot()
        {
            TitleScreenManager.instance.SelectCharacterSlot(characterSlot);
        }
    }
}