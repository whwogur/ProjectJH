using UnityEngine;
using System;
using System.IO;

namespace JH
{
    public class SaveGameDataWriter
    {
        public string saveDataDirectoryPath = "";
        public string saveFileName = "";

        /* before creating a new save file, check to see if one of the character slot already exists */
        public bool CheckIfFileAlreadyExists()
        {
            if (File.Exists(Path.Combine(saveDataDirectoryPath, saveFileName)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /* Used to delete character save files */
        public void DeleteSaveFile()
        {
            File.Delete(Path.Combine(saveDataDirectoryPath, saveFileName));
        }

        /* used to create a save file upon starting a new game */
        public void CreateNewCharacterSaveFile(CharacterSaveData characterData)
        {
            // make a path to save file
            string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

            try
            {
                // create directory the file will be written to if it does not already exist
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                Debug.Log("Creating Save File, at path: " + savePath);

                // Serialize C# Game Data Object to json file
                string dataToStore = JsonUtility.ToJson(characterData, true);

                // write the file to system
                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(dataToStore);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error whilst trying to save character data, game not saved!" + savePath + "\n" + ex);
            }
        }

        /* Used to load a save file upon loading a previous game */
        public CharacterSaveData LoadSaveFile()
        {
            CharacterSaveData characterData = null;
            // make a path to load the file
            string loadPath = Path.Combine(saveDataDirectoryPath, saveFileName);

            if (File.Exists(loadPath))
            {
                try
                {
                    string dataToLoad = "";
                    using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    // Deserialize the data from json back to unity
                    characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
                }
                catch (Exception ex)
                {
                    Debug.LogError("File is Blank!" + ex);
                }
            }

            return characterData;
        }
    }
}
