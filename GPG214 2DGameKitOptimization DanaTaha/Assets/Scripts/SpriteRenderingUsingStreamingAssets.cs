using System.IO;
using UnityEngine;

namespace Gamekit2D.DanaChanges
{
    /// <summary>
    /// Attach this to the dropship GameObject!
    /// The sprite is rendered when the player is in range and it gets unloaded when they are no longer in range.
    /// </summary>
    public class SpriteRenderingUsingDistanceCheck : MonoBehaviour
    {
        #region Variables
        [SerializeField] private string spriteFileName = "Dropship.png";
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float loadiingAssetDistance = 23f;
        [SerializeField] private float playerDistanceCheckerDelay = 5f;
        private string _spriteFilePath;
        private SpriteRenderer _spriteRenderer;
        private Sprite _loadedSprite;
        private bool _spriteLoaded = false;
        #endregion

        private void Start()
        {
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            _spriteFilePath = Path.Combine(Application.streamingAssetsPath, spriteFileName);
            LoadSpriteFromStreamingAssets();
        }

        private void Update()
        {
            Invoke("CheckPlayerDistance", playerDistanceCheckerDelay);
        }

        #region Private Functions.
        private void CheckPlayerDistance()
        {
            float _distance = Vector2.Distance(playerTransform.position, transform.position);

            if (_distance <= loadiingAssetDistance)
            {
                if (!_spriteLoaded)
                {
                    LoadSpriteFromStreamingAssets();
                }
            }
            else
            {
                if (_spriteLoaded)
                {
                    UnloadSprite();
                }
            }
        }

        /// <summary>
        /// Load the sprite from StreamingAssets.
        /// </summary>
        private void LoadSpriteFromStreamingAssets()
        {
            if (File.Exists(_spriteFilePath))
            {
                byte[] _fileData = File.ReadAllBytes(_spriteFilePath);
                Texture2D texture = new Texture2D(2, 2);

                if (texture.LoadImage(_fileData))
                {
                    _loadedSprite = CreateSpriteFromTexture(texture);
                    _spriteRenderer.sprite = _loadedSprite;
                    _spriteLoaded = true;
                    Debug.Log("Successfully loaded the sprite.");
                }
                else
                {
                    Debug.Log("texture didn;t load from the file.");
                }
            }
            else
            {
                Debug.Log("file isn't in " + _spriteFilePath);
            }
        }

        /// <summary>
        /// Unload the sprite by setting the SpriteRenderer's sprite to null.
        /// </summary>
        private void UnloadSprite()
        {
            if (_spriteRenderer.sprite != null)
            {
                _spriteRenderer.sprite = null;
                if (_loadedSprite != null)
                {
                    Destroy(_loadedSprite.texture);
                    _loadedSprite = null;
                }
                _spriteLoaded = false;
                Debug.Log("Sprite unloaded.");
            }
        }

        /// <summary>
        /// Creates a 2D sprite from the Texture provided to it.
        /// </summary>
        private Sprite CreateSpriteFromTexture(Texture2D _texture)
        {
            return Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f));
        }
        #endregion
    }
}
