using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor Script for selecting and processing Objects and Components
/// </summary>

class SelectAndProcessTool : EditorWindow
{
    // GUI
    private TMP_FontAsset target;
    private int selectTypeIndex;
    private string selectPhrase;
    private bool activeGOFilter = false;
    private bool typeSelection, phraseSelection = false;
    private Vector2 scrollPos;
    private BaseProcessSelected processScript;

    // Types
    private static System.Type[] types;
    private static string[] typesAsStrings;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Tools/Select and Process")]
    static void Init()
    {
        ReferenceAllTypesInProject();
        var window = GetWindow<SelectAndProcessTool>(); // Get existing open window or if none, make a new one
        window.Show();
    }

    #region GUI

    void OnGUI()
    {
        // HEADER
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Selection", EditorStyles.boldLabel);

        // Selection criteria
        GUICriteriaComponentType();
        GUICriteriaSearchPhrase();
        GUICriteriaActiveGOOnly();

        using (new EditorGUILayout.HorizontalScope("Selection Buttons", GUILayout.MaxHeight(60)))
        {
            GUIButtonSelectObjects();
            GUIButtonFilterSelection();

        }

        // HEADER
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Processing", EditorStyles.boldLabel);
        GUIScriptField();
        GUIButtonProcessObjects();
    }

    private void GUIShowSelectedGameObjects()
    {
        if (Selection.objects.Length > 0)
        {
            EditorGUILayout.LabelField("Selected objects in scene:", EditorStyles.wordWrappedLabel);

            string currentSelection = "";
            foreach (GameObject o in Selection.gameObjects)
            {
                currentSelection += o.name + "\n";
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
            EditorGUILayout.TextArea(currentSelection, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }
    }

    bool showTypeSelection;
    private void GUICriteriaComponentType()
    {
        showTypeSelection = EditorGUILayout.Foldout(showTypeSelection, "select objects which contain a component of type...");
        if (showTypeSelection)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                typeSelection = EditorGUILayout.BeginToggleGroup("enable/disable", typeSelection);
                if (types == null)
                {
                    ReferenceAllTypesInProject();
                }
                selectTypeIndex = EditorGUILayout.Popup(selectTypeIndex, typesAsStrings);

                using (new EditorGUILayout.HorizontalScope("Refresh Button", GUILayout.MaxHeight(10)))
                {
                    EditorGUILayout.Space();
                    GUIButtonRefreshTypes();
                }
                EditorGUILayout.EndToggleGroup();
            }
        }
    }

    bool showPhraseSelection;
    private void GUICriteriaSearchPhrase()
    {
        showPhraseSelection = EditorGUILayout.Foldout(showPhraseSelection, "object's name contains...");
        if (showPhraseSelection)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                phraseSelection = EditorGUILayout.BeginToggleGroup("enable/disable", phraseSelection);
                selectPhrase = EditorGUILayout.TextField("Phrase: ", selectPhrase);
                EditorGUILayout.EndToggleGroup();
            }
        }
    }
    private void GUICriteriaActiveGOOnly()
    {
        // select only active gameObjects
        activeGOFilter = EditorGUILayout.Toggle("Only active GameObjects", activeGOFilter);
    }

    private void GUIScriptField()
    {
        EditorGUILayout.Space();
        var label = "Select process script:";
        processScript = EditorGUILayout.ObjectField(label, processScript, typeof(BaseProcessSelected), false) as BaseProcessSelected;
    }

    private void GUIButtonSelectObjects()
    {
        if (GUILayout.Button("Select filtered objects in Scene", GUILayout.MinHeight(40)))
        {
            if (!typeSelection && !phraseSelection)
            {
                ShowNotification(new GUIContent("Criteria required."));
            }
            else
            {
                SelectGameObjects();
            }
        }
    }
    private void GUIButtonFilterSelection()
    {
        if (GUILayout.Button("Apply filter to Selection", GUILayout.MinHeight(40)))
        {
            if (Selection.objects.Length == 0)
            {
                ShowNotification(new GUIContent("Nothing Selected."));
            }
            else
            {
                ApplyFilterToSelection();
            }
        }
    }
    private void GUIButtonProcessObjects()
    {
        EditorGUILayout.Space();
        if (GUILayout.Button("Process selected objects", GUILayout.MinHeight(40)))
        {
            if (processScript == null)
            {
                ShowNotification(new GUIContent("No processing script selected."));
            }
            else
            {
                processScript.processSelectedObjects(Selection.gameObjects);
            }
        }
    }
    private void GUIButtonRefreshTypes()
    {
        if (GUILayout.Button("Refresh Types", GUILayout.ExpandWidth(false)))
        {
            ReferenceAllTypesInProject();
        }
    }

    #endregion // GUI

    private void SelectGameObjects()
    {
        CollapseHierarchy();
        var allObjects = FindAllGameObjects();
        var objects = allObjects.ToArray();
        FilterSelection(objects);
    }

    // find and return all active, inactive objects containing of type text, including assets, prefabs etc.
    private static List<GameObject> FindAllGameObjects()
    {
        var allgameObjects = new List<GameObject>();
        var rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var gameObject in rootGameObjects)
        {
            var childTransforms = gameObject.GetComponentsInChildren<Transform>(true);
            foreach (var child in childTransforms)
            {
                allgameObjects.Add(child.gameObject);
            }
        }
        return allgameObjects;
    }

    #region ReferenceAllTypesInProject

    private static void ReferenceAllTypesInProject()
    {
        var typesList = CreateListOfAllTypes();

        types = typesList.ToArray();
        System.Array.Sort(types, delegate (System.Type type1, System.Type type2)
        {
            return type1.Name.CompareTo(type2.Name);
        });

        CreateTypesAsStringsArray();
    }

    private static List<System.Type> CreateListOfAllTypes()
    {
        var typesList = new List<System.Type>();
        var gameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject gameObject in gameObjects)
        {
            var components = gameObject.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var component in components)
            {
                var type = component.GetType();
                if (!typesList.Contains(type))
                {
                    typesList.Add(type);
                }
            }
        }
        return typesList;
    }

    private static void CreateTypesAsStringsArray()
    {
        typesAsStrings = new string[types.Length];
        for (var i = 0; i < types.Length; i++)
        {
            var name = types[i].Name;
            typesAsStrings[i] = $"{name[0]}/{name}";
        }
    }

    private void ApplyFilterToSelection()
    {
        var selection = Selection.objects;
        FilterSelection(selection);
    }

    #endregion // ReferenceAllTypesInProject

    private void FilterSelection(Object[] currentSelection)
    {
        var newSelection = new List<Object>();
        foreach (var gameObject in currentSelection)
        {
            bool phraseMatch = phraseSelection ? gameObject.name.ToLower().Contains(selectPhrase.ToLower()) : true; // check for phrase in name only if phraseSelection is true
            bool typeMatch = typeSelection ? ((GameObject)gameObject).GetComponent(types[selectTypeIndex]) : true; // check for type in components only if typeSelection is true
            bool activeMatch = activeGOFilter ? ((GameObject)gameObject).activeSelf : true; // check for active gameObject only if activeGOFilter is true
            if (phraseMatch && typeMatch && activeMatch)
            {
                newSelection.Add(gameObject);
            }
        }

        if (newSelection.Count > 0)
        {
            Selection.objects = newSelection.ToArray();
            Debug.Log($"{newSelection.Count} objects selected.");
        }
        else
        {
            Debug.Log("Selection criteria match no objects.");
        }
    }

    private void CollapseHierarchy()
    {
        // Todo implement this
    }
}
