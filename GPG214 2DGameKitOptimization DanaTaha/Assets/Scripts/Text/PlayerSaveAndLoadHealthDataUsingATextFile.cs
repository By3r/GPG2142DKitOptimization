using Gamekit2D;
using System.IO;
using UnityEngine;

#region Serializable struct.
[System.Serializable]
public struct PlayerGeneralData
{
    /// <summary>
    /// Contains general character data which tweak
    /// gameplay behavior.
    /// </summary>
    public float jumpSpeed;
    public float gravity;
    public float bulletSpeed;
}
#endregion

public class PlayerSaveAndLoadHealthDataUsingATextFile : MonoBehaviour
{
    #region Variables
    [SerializeField] private PlayerCharacter _playerCharacterScript;

    private string _filePath;
    private KeyCode _leftCntrl = KeyCode.LeftControl;
    private KeyCode _saveKey = KeyCode.E;
    private KeyCode _loadKey = KeyCode.L;
    #endregion

    private void Start()
    {
        _filePath = Path.Combine(Application.persistentDataPath, "PlayerGeneralData.txt");
    }

    private void Update()
    {
        KeyBoardInputTracker();
    }

    #region Public Functions.
    /// <summary>
    /// Tracks player keyboard inputs for saving, loading, and modifying player data.
    /// </summary>
    public void KeyBoardInputTracker()
    {
        PlayerSaveAndLoadInput(_leftCntrl, _saveKey, _loadKey);
        DecreaseDataValues(KeyCode.M, KeyCode.G, KeyCode.B, KeyCode.J);
        IncreaseDataValues(KeyCode.G, KeyCode.M, KeyCode.B, KeyCode.J);
    }

    /// <summary>
    /// Handles player input to save and load the player's movement data.
    /// </summary>
    public void PlayerSaveAndLoadInput(KeyCode _lCntrl, KeyCode _saveKey, KeyCode _loadKey)
    {
        if (Input.GetKey(_lCntrl) && Input.GetKeyDown(_saveKey))
        {
            SavePlayerGeneralData();
        }

        if (Input.GetKey(_lCntrl) && Input.GetKeyDown(_loadKey))
        {
            LoadGeneralPlayerData();
        }
    }
    #endregion

    #region Player General data saver and loader.
    /// <summary>
    /// Parses player data into a formatted string and saves it to a text file.
    /// </summary>
    private void SavePlayerGeneralData()
    {
        string _formattedData = $"jumpspeed,{_playerCharacterScript.jumpSpeed}||gravity,{_playerCharacterScript.gravity}||bulletSpeed,{_playerCharacterScript.bulletSpeed}";
        File.WriteAllText(_filePath, _formattedData);
    }

    /// <summary>
    /// Loads player data from the text file, parses it, and applies it to the PlayerCharacter script.
    /// </summary>
    private void LoadGeneralPlayerData()
    {
        if (File.Exists(_filePath))
        {
            string _formattedData = File.ReadAllText(_filePath);
            string[] _dataParts = _formattedData.Split(new string[] { "||" }, System.StringSplitOptions.None);

            foreach (var part in _dataParts)
            {
                string[] keyValue = part.Split(',');
                if (keyValue.Length == 2)
                {
                    string key = keyValue[0].Trim();
                    float value = float.Parse(keyValue[1].Trim());

                    switch (key.ToLower())
                    {
                        case "jumpspeed":
                            _playerCharacterScript.jumpSpeed = value;
                            break;
                        case "gravity":
                            _playerCharacterScript.gravity = value;
                            break;
                        case "bulletspeed":
                            _playerCharacterScript.bulletSpeed = value;
                            break;
                    }
                }
            }
        }
    }
    #endregion

    #region Data value increasers and decreasers. (Basically data tweakers)
    /// <summary>
    /// Accesses the player character script's variables and tweaks them based on player key inputs.
    /// </summary>
    private void IncreaseDataValues(KeyCode _gravityIncreaserKey, KeyCode _minusKey, KeyCode _bulletSpeedIncreaserKey, KeyCode _jumpSpeedIncreaserKey)
    {
        // Increases gravity.
        if (Input.GetKeyDown(_gravityIncreaserKey) && !Input.GetKey(_minusKey))
        {
            _playerCharacterScript.gravity += 0.2f;
            Debug.Log("Increased Gravity, it is now " + _playerCharacterScript.gravity);
        }

        // Increases bullet speed.
        if (Input.GetKeyDown(_bulletSpeedIncreaserKey) && !Input.GetKey(_minusKey))
        {
            _playerCharacterScript.bulletSpeed += 0.2f;
            Debug.Log("Increased Bullet Speed, it is now " + _playerCharacterScript.bulletSpeed);
        }

        // Increases jump speed.
        if (Input.GetKeyDown(_jumpSpeedIncreaserKey) && !Input.GetKey(_minusKey))
        {
            _playerCharacterScript.jumpSpeed += 0.5f;
            Debug.Log("Increased Jump Speed, it is now " + _playerCharacterScript.jumpSpeed);
        }
    }

    /// <summary>
    /// Decreases gravity, bullet speed, or jump speed based on player key inputs.
    /// </summary>
    private void DecreaseDataValues(KeyCode _minusKey, KeyCode _gravityKey, KeyCode _bulletSpeedKey, KeyCode _jumpSpeedKey)
    {
        // Decreases gravity.
        if (Input.GetKey(_minusKey) && Input.GetKeyDown(_gravityKey))
        {
            _playerCharacterScript.gravity -= 0.2f;
            Debug.Log("Decreased Gravity, it is now " + _playerCharacterScript.gravity);
        }

        // Decreases bullet speed.
        if (Input.GetKey(_minusKey) && Input.GetKeyDown(_bulletSpeedKey))
        {
            _playerCharacterScript.bulletSpeed -= 0.2f;
            Debug.Log("Decreased Bullet Speed, it is now " + _playerCharacterScript.bulletSpeed);
        }

        // Decreases jump speed.
        if (Input.GetKey(_minusKey) && Input.GetKeyDown(_jumpSpeedKey))
        {
            _playerCharacterScript.jumpSpeed -= 0.5f;
            Debug.Log("Decreased Jump Speed, it is now " + _playerCharacterScript.jumpSpeed);
        }
    }
    #endregion
}
