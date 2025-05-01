using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class FullScreenHelper
{
    static FullScreenHelper()
    {
        EditorApplication.update += Update;
    }

    static void Update()
    {
#if UNITY_EDITOR
        if(EditorApplication.isPlaying && ShouldToggleMaximize())
        {
            EditorWindow.focusedWindow.maximized = !EditorWindow.focusedWindow.maximized;
        }
#endif
    }

    private static bool ShouldToggleMaximize()
    {
        return Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.LeftShift);
    }
}
