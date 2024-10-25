using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace Gamekit2D.DanaChanges
{
    #region Serializable class.
    [Serializable]
    public class PlayerKeyData
    {
        public string[] keys = new string[3];
    }
    #endregion

    public class PlayerSaveAndLoadKeyDataUsingXML : MonoBehaviour
    {
        #region Variables.
        [SerializeField] private InventoryController _inventoryController;

        private string _folderPath;
        private string _saveFilePath;
        private KeyCode _e = KeyCode.E;
        private KeyCode _l = KeyCode.L;
        private KeyCode _leftCntrl = KeyCode.LeftControl;
        #endregion

        private void Start()
        {
            _folderPath = Path.Combine(Application.persistentDataPath, "KeyData");
            _saveFilePath = Path.Combine(_folderPath, "PlayerKeyData.xml");

            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
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
                SaveKeyData();
            }
            if (Input.GetKey(_firstKeyLoad) && Input.GetKeyDown(_secondKeyLoad))
            {
                LoadKeyData();
            }
        }
        #endregion

        #region Private Functions.

        #region Key Data Saver and Loader.
        /// <summary>
        /// Save the player's key data to XML by going through each Key in the Array
        /// and checking how many does the player have from them. Rellocate that data 
        /// into our PlayerKeyData class and serialize it to XML.
        /// </summary>
        private void SaveKeyData()
        {
            if (_inventoryController != null)
            {
                try
                {
                    PlayerKeyData keyData = new PlayerKeyData();
                    int index = 0;

                    if (KeyUI.Instance == null)
                    {
                        return;
                    }

                    foreach (var _keyName in KeyUI.Instance.keyNames)
                    {
                        if (_inventoryController.HasItem(_keyName))
                        {
                            if (index < keyData.keys.Length)
                            {
                                keyData.keys[index] = _keyName;
                                index++;
                            }
                        }
                    }

                    XmlSerializer _xmlSerializer = new XmlSerializer(typeof(PlayerKeyData));
                    using (StreamWriter _writer = new StreamWriter(_saveFilePath))
                    {
                        _xmlSerializer.Serialize(_writer, keyData);
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Load the player's key data from the xml file by serializing it
        /// for our KeyData script to read and store the amount of keys saved
        /// within the file. We access the game's inventory controller to add
        /// the amount of keys we saved into the inventory so that the players 
        /// can still use the keys to open the door.
        /// </summary>
        private void LoadKeyData()
        {
            if (_inventoryController != null)
            {
                if (File.Exists(_saveFilePath))
                {
                    try
                    {
                        XmlSerializer _serializer = new XmlSerializer(typeof(PlayerKeyData));
                        using (StreamReader _reader = new StreamReader(_saveFilePath))
                        {
                            PlayerKeyData _keyData = (PlayerKeyData)_serializer.Deserialize(_reader);

                            foreach (var _key in _keyData.keys)
                            {
                                if (!string.IsNullOrEmpty(_key))
                                {
                                    _inventoryController.AddItem(_key);
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
        #endregion
        #endregion
    }
}
