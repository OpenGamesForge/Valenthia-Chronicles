
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Core
{
    public static class SceneLoader
    {
        private static readonly Dictionary<AssetReference, AsyncOperationHandle<SceneInstance>> LoadedScenes = new();

        public static async UniTask LoadScene(AssetReference sceneRef, bool setActive = false)
        {
            if (LoadedScenes.ContainsKey(sceneRef))
                return;

            var handle = Addressables.LoadSceneAsync(sceneRef, LoadSceneMode.Additive);
            await handle.Task;

            LoadedScenes[sceneRef] = handle;

            if (setActive && handle.Result.Scene.IsValid())
                SceneManager.SetActiveScene(handle.Result.Scene);
        }

        public static async UniTask UnloadScene(AssetReference sceneRef)
        {
            if (!LoadedScenes.TryGetValue(sceneRef, out var handle))
                return;

            await Addressables.UnloadSceneAsync(handle).Task;
            LoadedScenes.Remove(sceneRef);
        }
    }
}