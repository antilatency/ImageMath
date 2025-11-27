#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.CompilerServices;



public class CreateTestSceneWindow : EditorWindow {
    private string sceneName = "NewTest";

    [MenuItem("Assets/Create/Test Scene")]
    public static void ShowWindow() {
        GetWindow<CreateTestSceneWindow>("Create Test Scene");
    }

    private void OnGUI() {
        GUILayout.Label("Enter Test Scene Name", EditorStyles.boldLabel);
        sceneName = EditorGUILayout.TextField("Name", sceneName);

        if (GUILayout.Button("Create")) {
            if (string.IsNullOrEmpty(sceneName)) {
                EditorUtility.DisplayDialog("Error", "Please enter a valid name.", "OK");
                return;
            }

            string selectedPath = GetSelectedPath();

            CreateTestScene(selectedPath, sceneName);
            Close();
        }
    }

    string GetSelectedPath() {
        string selectedPath = "Assets";
        Object activeObject = Selection.activeObject;
        if (activeObject != null) {
            string path = AssetDatabase.GetAssetPath(activeObject);
            if (AssetDatabase.IsValidFolder(path))
                selectedPath = path;
            else
                selectedPath = Path.GetDirectoryName(path);
        }
        return selectedPath;
    }

    private static void CreateTestScene(string selectedPath, string name) {

        string directory = Path.Combine(selectedPath, name);

        Directory.CreateDirectory(directory);

        // Create script
        string scriptPath = Path.Combine(directory, $"{name}Test.cs");
        File.WriteAllText(scriptPath, GetScriptCode($"{name}Test"));
        AssetDatabase.Refresh();

        // Create new scene
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        RenderSettings.skybox = null;

        // Create GameObject and add script
        GameObject go = new GameObject($"{name}Test");
        var agent = go.AddComponent<CreateTestSceneAgent>();
        agent.scriptFullName = $"{name}Test";

        /*MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
        if (monoScript != null) {
            go.AddComponent(monoScript.GetClass());
        }*/

        // Save scene
        string scenePath = Path.Combine(directory, $"{name}Test.unity");
        EditorSceneManager.SaveScene(scene, scenePath);
        //EditorSceneManager.OpenScene(scenePath);
    }

    static string GetTemplate([CallerFilePath] string filePath = "") {
        string templatePath = Path.Combine(Path.GetDirectoryName(filePath), "CreateTestScene.template");
        if (!File.Exists(templatePath)) {
            Debug.LogError($"Template file not found: {templatePath}");
            return string.Empty;
        }
        return File.ReadAllText(templatePath);
    }

    static string GetScriptCode(string className) {
        string template = GetTemplate();
        if (string.IsNullOrEmpty(template)) {
            Debug.LogError("Template is empty. Cannot create script.");
            return string.Empty;
        }
        return template.Replace("@ClassName", className);
    }

    /*private static string GetScriptTemplate(string name) {
        return
$@"using UnityEngine;

public class {name}Test : MonoBehaviour
{{
    void Start()
    {{
        Debug.Log(""{name}Test started."");
    }}
}}";
    }*/
}
#endif