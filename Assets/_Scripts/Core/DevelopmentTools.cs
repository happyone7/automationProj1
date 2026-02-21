using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
public class DevelopmentTools : Editor
{
    // 메뉴에 추가됨
    [MenuItem("Tools/Open Project Folder")]
    public static void OpenProjectFolder()
    {
        EditorUtility.RevealInFinder(Application.dataPath);
    }

    [MenuItem("Tools/Clear Console")]
    public static void ClearConsole()
    {
        var assembly = System.Reflection.Assembly.GetAssembly(typeof(SceneView));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        method.Invoke(null, null);
    }

    [MenuItem("Tools/Force Refresh Assets")]
    public static void ForceRefreshAssets()
    {
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Open Unity Preferences")]
    public static void OpenPreferences()
    {
        // Preferences 폴더 열기
        string prefPath = Path.GetDirectoryName(EditorApplication.applicationPath);
        prefPath = Path.Combine(prefPath, "Data", "Resources");
        if (Directory.Exists(prefPath))
        {
            EditorUtility.RevealInFinder(prefPath);
        }
    }

    [MenuItem("Tools/List All Buildings")]
    public static void ListAllBuildings()
    {
        string[] guids = AssetDatabase.FindAssets("t:BuildingData");
        Debug.Log($"Found {guids.Length} BuildingData assets:");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log($"  - {path}");
        }
    }

    [MenuItem("Tools/List All Items")]
    public static void ListAllItems()
    {
        string[] guids = AssetDatabase.FindAssets("t:Item");
        Debug.Log($"Found {guids.Length} Item assets:");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log($"  - {path}");
        }
    }

    // Unity 에디터에서 플레이 모드 시작/종료 시 자동 호출
    [InitializeOnLoad]
    public class PlayModeStateChanged
    {
        static PlayModeStateChanged()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                Debug.Log("▶ Play Mode Started");
            }
            else if (state == PlayModeStateChange.ExitingPlayMode)
            {
                Debug.Log("◀ Play Mode Ended");
            }
        }
    }
}
#endif
