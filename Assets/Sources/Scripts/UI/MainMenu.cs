/*---------------------------------------------------------------------------------------------
 *  Copyright (c) OpenGamesCore Project. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Core;
using System.Threading.Tasks;
using UnityEngine;


namespace Scripts.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private float fadeInDuration = 1f;
        
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                Debug.LogError("CanvasGroup component is missing on MainMenu object!");
                _ = GameManager.Instance.GameShutdown();
            }
        }

        private void Start()
        {
            if (!_canvasGroup)
                return;
            
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            GameManager.Instance.OnGameInitialized += OnGameManagerInitialized;
        }

        private void OnGameManagerInitialized()
        {
            DisplayUI();
        }

        private async void DisplayUI()
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            float elapsedTime = 0;
            while (elapsedTime < fadeInDuration)
            {
                _canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeInDuration);
                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }
            _canvasGroup.alpha = 1;
        }
        
        public void PlayGame()
        {
            _ = GameSession.Instance.NewGame();
        }
        
        public void QuitGame()
        {
            _ = GameManager.Instance.GameShutdown();
        }
        
        public void DisplaySettings()
        {
            Debug.Log("Settings Menu");
        }
    }
}
