/*---------------------------------------------------------------------------------------------
 *  Copyright (c) OpenGamesCore Project. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Common;

namespace Core
{
    public class GameManager : LazySingleton<GameManager>
    {
        public event Action OnGameInitialized;
        public event Action OnGameShutdown;
        
        private readonly List<IGameService> _services = new List<IGameService>();
        private bool _isInitialized;
        
        private async void Start()
        {
            try
            {
                await InitializeGame();
            }
            catch (Exception exc)
            {
                Debug.LogError($"Critical error during GameManager initialization: {exc.Message}\nStack trace: {exc.StackTrace}");
                
                QuitGame();
            }
        }

        private async UniTask InitializeGame()
        {
            try
            {
                var operation = SceneManager.LoadSceneAsync("SC_MainMenu", LoadSceneMode.Additive);

                if (operation == null)
                {
                    throw new Exception("Failed to load the main menu.");
                }
                
                operation.allowSceneActivation = true;
                await operation;
                
                await InitializeServices();
                
                await Awaitable.WaitForSecondsAsync(5f);
                
                // Notify that initialization is complete
                _isInitialized = true;
                Debug.Log("Game initialization complete, triggering OnGameInitialized");
                OnGameInitialized?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during game initialization: {e.Message}");
                throw; // Re-throw to be caught by Start()
            }
        }

        private async UniTask InitializeServices()
        {
            await GameSession.Instance.Initialize();
            _services.Add(GameSession.Instance);
        }

        public async UniTask GameShutdown()
        {
            try
            {
                OnGameShutdown?.Invoke();
                
                // Shutdown all services in reverse order of their initialization
                for (int i = _services.Count - 1; i >= 0; i--)
                {
                    Debug.Log($"Shutting down service: {_services[i].GetType().Name}");
                    await _services[i].Shutdown();
                }
                
                _services.Clear();
                _isInitialized = false;
                
                Debug.Log("All services shut down successfully.");
                
                QuitGame();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during game shutdown: {e.Message}");
                throw; // Re-throw to allow proper error handling by the caller
            }
        }
        
        private void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        public bool IsInitialized => _isInitialized;
    }

    public interface IGameService
    {
        UniTask Initialize();
        UniTask Shutdown();
    }
}
