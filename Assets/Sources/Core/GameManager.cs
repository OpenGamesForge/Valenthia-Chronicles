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
        
        private List<IGameService> _services = new List<IGameService>();
        private bool _isInitialized = false;
        
        private async void Start()
        {
            try
            {
                await InitializeGame();
            }
            catch (Exception exc)
            {
                Debug.LogError($"Critical error during GameManager initialization: {exc.Message}\nStack trace: {exc.StackTrace}");
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
            }
        }

        private void OnApplicationQuit()
        {
            ShutdownGame().Forget();
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

        //private void InitializeServices()
        //{
            // TODO: Add future services initialization here
            // Example:
            // var audioService = AudioService.Instance;
            // await audioService.Initialize();
            // _services.Add(audioService);
        //}

        private async UniTaskVoid ShutdownGame()
        {
            try
            {
                OnGameShutdown?.Invoke();
                
                // Shutdown all services in reverse order of their initialization
                for (int i = _services.Count - 1; i >= 0; i--)
                {
                    await _services[i].Shutdown();
                }
                
                _services.Clear();
                _isInitialized = false;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during game shutdown: {e.Message}");
            }
        }

        public bool IsInitialized => _isInitialized;
    }

    public interface IGameService
    {
        UniTask Initialize();
        UniTask Shutdown();
    }
}
