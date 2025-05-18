/*---------------------------------------------------------------------------------------------
 *  Copyright (c) OpenGamesCore Project. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameSession : LazySingleton<GameSession>, IGameService
    {
        public UniTask Initialize()
        {
            Debug.Log("GameSession initialized.");
            return UniTask.CompletedTask;
        }
        
        public async UniTask NewGame()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync("SC_DevMap", LoadSceneMode.Additive);
            await UniTask.WaitUntil(() => loadOperation is { isDone: true });

            SceneManager.SetActiveScene(SceneManager.GetSceneByName("SC_DevMap"));
            
            _ = SceneManager.UnloadSceneAsync("SC_MainMenu");
        }

        public UniTask Shutdown()
        {
            OnDestroy();
            return UniTask.CompletedTask;
        }
    }
}
