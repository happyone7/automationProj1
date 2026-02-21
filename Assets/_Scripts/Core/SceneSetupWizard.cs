using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;

public class SceneSetupWizard : EditorWindow
{
    [MenuItem("Setup/Scene Setup Wizard")]
    public static void ShowWindow()
    {
        GetWindow<SceneSetupWizard>("Scene Setup");
    }

    bool createGridManager = true;
    bool createGameManager = true;
    bool createBuildingPlacer = true;
    bool createConveyorSystem = true;
    bool createCamera = true;
    bool createTilemap = true;

    void OnGUI()
    {
        GUILayout.Label("Unity Scene Setup Wizard", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        createGridManager = EditorGUILayout.Toggle("GridManager", createGridManager);
        createGameManager = EditorGUILayout.Toggle("GameManager", createGameManager);
        createBuildingPlacer = EditorGUILayout.Toggle("BuildingPlacer", createBuildingPlacer);
        createConveyorSystem = EditorGUILayout.Toggle("ConveyorSystem", createConveyorSystem);
        createCamera = EditorGUILayout.Toggle("Main Camera", createCamera);
        createTilemap = EditorGUILayout.Toggle("Tilemap", createTilemap);

        EditorGUILayout.Space();

        if (GUILayout.Button("Setup Scene"))
        {
            SetupScene();
        }
    }

    void SetupScene()
    {
        // GridManager
        if (createGridManager)
        {
            GameObject gridMgr = GameObject.Find("GridManager");
            if (gridMgr == null)
            {
                gridMgr = new GameObject("GridManager");
                gridMgr.AddComponent<GridManager>();
            }
        }

        // GameManager
        if (createGameManager)
        {
            GameObject gameMgr = GameObject.Find("GameManager");
            if (gameMgr == null)
            {
                gameMgr = new GameObject("GameManager");
                gameMgr.AddComponent<GameManager>();
            }
        }

        // BuildingPlacer
        if (createBuildingPlacer)
        {
            GameObject placer = GameObject.Find("BuildingPlacer");
            if (placer == null)
            {
                placer = new GameObject("BuildingPlacer");
                var script = placer.AddComponent<BuildingPlacer>();
                
                // Camera 찾기
                Camera cam = Camera.main;
                if (cam != null)
                {
                    script.mainCamera = cam;
                }
                
                // Grid 찾기
                Grid grid = FindObjectOfType<Grid>();
                if (grid != null)
                {
                    script.grid = grid;
                }
            }
        }

        // ConveyorSystem
        if (createConveyorSystem)
        {
            GameObject conveyor = GameObject.Find("ConveyorSystem");
            if (conveyor == null)
            {
                conveyor = new GameObject("ConveyorSystem");
                conveyor.AddComponent<ConveyorSystem>();
            }
        }

        // Main Camera
        if (createCamera)
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                GameObject cameraObj = new GameObject("Main Camera");
                cam = cameraObj.AddComponent<Camera>();
                cameraObj.tag = "MainCamera";
                cam.orthographic = true;
                cam.orthographicSize = 10;
                cam.transform.position = new Vector3(0, 0, -10);
            }
        }

        // Tilemap
        if (createTilemap)
        {
            Grid grid = FindObjectOfType<Grid>();
            if (grid == null)
            {
                GameObject gridObj = new GameObject("Grid");
                gridObj.AddComponent<Grid>();
                
                GameObject tilemapObj = new GameObject("Tilemap");
                tilemapObj.transform.parent = gridObj.transform;
                tilemapObj.AddComponent<Tilemap>();
                tilemapObj.AddComponent<TilemapRenderer>();
                tilemapObj.AddComponent<TilemapCollider2D>();
            }
        }

        Debug.Log("Scene Setup Complete!");
        Close();
    }
}
#endif
