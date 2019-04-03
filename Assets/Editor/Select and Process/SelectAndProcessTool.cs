using UnityEditor;
using UnityEngine;

namespace EditorTools
{
    class SelectAndProcessTool : EditorWindow
    {
        private Vector2 scrollPos;

        bool showTypeSelection = true;
        bool showPhraseSelection = true;


        [MenuItem("Tools/Select and Process")]
        static void Init()
        {
            Select.ReferenceAllTypesInProject();
            var window = GetWindow<SelectAndProcessTool>(); // Get existing open window or if none, make a new one
            window.Show();
        }

        void OnGUI()
        {
            EditorGUIUtility.labelWidth = 200;

            // HEADER
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Selection", EditorStyles.boldLabel);

            // Selection criteria
            GUICriteriaComponentType();
            GUICriteriaSearchPhrase();
            GUICriteriaActiveOnly();
            GUICriteriaChildren();

            GUIButtonFilterSelection();

            // HEADER
            /*
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); //horizontal Line
            EditorGUILayout.LabelField("Processing", EditorStyles.boldLabel);
            GUIScriptField();
            GUIButtonProcessObjects();
            */
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

        private void GUICriteriaComponentType()
        {
            showTypeSelection = EditorGUILayout.Foldout(showTypeSelection, "select objects which contain a component of type...");
            if (showTypeSelection)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    Select.typeSelection = EditorGUILayout.BeginToggleGroup("enable/disable", Select.typeSelection);
                    if (Select.Types == null)
                    {
                        Select.ReferenceAllTypesInProject();
                    }
                    Select.selectTypeIndex = EditorGUILayout.Popup(Select.selectTypeIndex, Select.TypesAsStrings);

                    using (new EditorGUILayout.HorizontalScope("Refresh Button", GUILayout.MaxHeight(10)))
                    {
                        EditorGUILayout.Space();
                        GUIButtonRefreshTypes();
                    }
                    EditorGUILayout.EndToggleGroup();
                }
            }
        }

        private void GUICriteriaSearchPhrase()
        {
            showPhraseSelection = EditorGUILayout.Foldout(showPhraseSelection, "object's name contains...");
            if (showPhraseSelection)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    Select.phraseSelection = EditorGUILayout.BeginToggleGroup("enable/disable", Select.phraseSelection);
                    Select.selectPhrase = EditorGUILayout.TextField("Phrase: ", Select.selectPhrase);
                    EditorGUILayout.EndToggleGroup();
                }
            }
        }
        private void GUICriteriaActiveOnly()
        {
            // select only active gameObjects
            Select.activeFilter = EditorGUILayout.Toggle("Only active objects", Select.activeFilter);
        }

        private void GUICriteriaSceneObjects()
        {
            // select only active gameObjects
            Select.sceneObjectsFilter = EditorGUILayout.Toggle("Objects in Scene", Select.sceneObjectsFilter);
        }

        private void GUICriteriaAssets()
        {
            // select only active gameObjects
            Select.assetsFilter = EditorGUILayout.Toggle("Objects in Assets", Select.assetsFilter);
        }

        private void GUICriteriaChildren()
        {
            // select only active gameObjects
            Select.childrenFilter = EditorGUILayout.Toggle("Select in objects' children", Select.childrenFilter);
        }

        private void GUIScriptField()
        {
            EditorGUILayout.Space();
            var label = "Process script:";
            Process.processScript = EditorGUILayout.ObjectField(label, Process.processScript, typeof(BaseProcessing), false) as BaseProcessing;
        }

        private void GUIButtonFilterSelection()
        {
            if (GUILayout.Button("Apply filter on SCENE", GUILayout.MinHeight(40)))
            {
                if (Selection.gameObjects.Length == 0)
                {
                    Select.SelectGameObjects();
                }
                else
                {
                    Select.ApplyFilterToSelection();
                }
            }
        }
        private void GUIButtonProcessObjects()
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("Process selected objects", GUILayout.MinHeight(40)))
            {
                if (Process.processScript == null)
                {
                    ShowNotification(new GUIContent("No processing script selected."));
                }
                else
                {
                    Process.processScript.processSelectedObjects(Selection.gameObjects);
                }
            }
        }
        private void GUIButtonRefreshTypes()
        {
            if (GUILayout.Button("Refresh Types", GUILayout.ExpandWidth(false)))
            {
                Select.ReferenceAllTypesInProject();
            }
        }
    }
}
