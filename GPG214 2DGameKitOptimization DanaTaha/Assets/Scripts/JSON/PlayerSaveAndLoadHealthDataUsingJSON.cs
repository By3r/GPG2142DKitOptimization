using System;
using System.IO;
using UnityEngine;

namespace Gamekit2D.DanaChanges
{
    #region Serializable struct.
    [Serializable]
    public struct PlayerHealthData
    {
        public int currentHealth;
    }
    #endregion

    public class PlayerSaveAndLoadHealthDataUsingJSON : MonoBehaviour
    {
        #region variables
        [SerializeField] private Damageable _damageable;

        private string _folderPath;
        private string _saveFilePath;
        private KeyCode _e = KeyCode.E;
        private KeyCode _l = KeyCode.L;
        private KeyCode _leftCntrl = KeyCode.LeftControl;

        #endregion

        private void Start()
        {
            _folderPath = Path.Combine(Application.persistentDataPath, "HealthData");
            _saveFilePath = Path.Combine(_folderPath, "PlayerHealthData.json");
            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
                Debug.Log("Created folder at: " + _folderPath);
            }
        }

        private void Update()
        {
            PlayerSaveAndLoadInput(_leftCntrl, _e, _leftCntrl, _l);
        }

        #region Public Functions.
        public void PlayerSaveAndLoadInput(KeyCode _firstKeySave, KeyCode _secondKeySave, KeyCode _firstKeyLoad, KeyCode _secondKeyLoad)
        {
            if (Input.GetKey(_firstKeySave) && Input.GetKeyDown(_secondKeySave))
            {
                SaveHealthData();
            }
            if (Input.GetKey(_firstKeyLoad) && Input.GetKeyDown(_secondKeyLoad))
            {
                LoadHealthData();
            }
        }
        #endregion

        #region Private Functions.

        #region Health Data Saver and Loader.
        /// <summary>
        /// Store the health data in a serializable Class or Struct to be converted into JSON.
        /// Save the now JSON file data into the text file.
        /// </summary>
        private void SaveHealthData()
        {
            if (_damageable != null)
            {
                try
                {
                    PlayerHealthData _healthData = new PlayerHealthData
                    {
                        currentHealth = _damageable.CurrentHealth
                    };
                    string _json = JsonUtility.ToJson(_healthData, true);
                    File.WriteAllText(_saveFilePath, _json);
                    Debug.Log("Saved health data into: " + _saveFilePath);
                }
                catch
                {
                    Debug.Log("Couldn't save health data.");
                }
            }
            else
            {
                Debug.Log("You forgot to refernce Damageable script in the blanks!");
            }
        }

        /// <summary>
        /// Load JSON file by reading its content and converting them back into data
        /// within the PlayerHealthData struct. Change the player's health in the
        /// Damageable script according to the loaded data's data?? content?? you get it.
        /// </summary>
        private void LoadHealthData()
        {
            if (_damageable != null)
            {
                if (File.Exists(_saveFilePath))
                {
                    try
                    {
                        string _json = File.ReadAllText(_saveFilePath);
                        PlayerHealthData _healthData = JsonUtility.FromJson<PlayerHealthData>(_json);
                        _damageable.SetHealth(_healthData.currentHealth);
                        Debug.Log("Successfully loaded health data from " + _saveFilePath);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("Couldn't load the data: " + ex.Message);
                    }
                }
                else
                {
                    Debug.Log("File not found.. File path: " + _saveFilePath);
                }
            }
            else
            {
                Debug.Log("You forgot to refernce Damageable script in the blanks!");
            }
        }
        #endregion
        #endregion
    }
}
