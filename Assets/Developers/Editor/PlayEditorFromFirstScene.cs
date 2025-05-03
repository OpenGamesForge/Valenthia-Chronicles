using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

// ReSharper disable CheckNamespace
public static class PlayEditorFromFirstScene
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnPlay() 
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            SceneManager.LoadScene(0);
        }
    }
}