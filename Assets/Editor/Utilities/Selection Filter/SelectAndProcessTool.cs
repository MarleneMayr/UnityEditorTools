using UnityEditor;
using UnityEngine;

namespace EditorTools
{
    class SelectAndProcessTool : EditorWindow
    {
        int tab;

        private Vector2 scrollPos;

        bool showAdvancedFilters;

        string propertyName;
        PropertyOperators propertyOperator;
        enum PropertyOperators
        {
            equal,
            notEqual
        };
        int propertyTypeIndex = 0;
        string[] PropertyTypes = new string[] { "string", "int", "float", "Vector2", "Vector3", "bool", "object type" };
        string propertyPhrase;
        int propertyInt;
        float propertyFloat;
        Vector2 propertyVector2;
        Vector3 propertyVector3;
        bool propertyBool;
        int propertyObjectTypeIndex;
        string propertyObjectName;


        public static void Init(EditorWindow selectWindow)
        {
            Select.ReferenceAllTypesInProject();
            Select.Window = selectWindow;
        }

        void OnGUI()
        {
            EditorGUIUtility.labelWidth = 180;

            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope(EditorStyles.inspectorFullWidthMargins))
            {
                GUILayout.FlexibleSpace();
                tab = GUILayout.Toolbar(tab, new string[] { "Scene", "Assets" }, GUILayout.MaxWidth(400), GUILayout.MaxHeight(25));
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.Space();

            switch (tab)
            {
                case 0:
                    GUISceneFilter();
                    break;
                case 1:
                    GUIAssetFilter();
                    break;
            }
        }

        private void GUISceneFilter()
        {
            // Selection criteria
            GUICriteriaComponentType();
            GUICriteriaSearchPhrase();

            EditorGUILayout.Space();
            GUICriteriaActiveOnly();
            GUICriteriaChildren();

            // Advanced Filters
            EditorGUILayout.Space();
            // GUIAdvancedFilters();

            GUIButtonFilterSelection();
            GUIButtonSearchScene();

            // Current selection
            GUILayout.FlexibleSpace();
            GUIShowSelectedGameObjects();
        }

        private void GUIAssetFilter()
        {
            EditorGUILayout.LabelField("Not implemented yet.", EditorStyles.miniLabel);
        }

        private void GUIShowSelectedGameObjects()
        {
            var selectionCount = Selection.gameObjects.Length;
            if (selectionCount > 0)
            {
                EditorGUILayout.LabelField($"{selectionCount} selected objects in scene:", EditorStyles.wordWrappedLabel);

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
            Select.typeSelection = EditorGUILayout.BeginToggleGroup("Object has component of type:", Select.typeSelection);
            using (new EditorGUILayout.HorizontalScope(EditorStyles.inspectorFullWidthMargins, GUILayout.MaxHeight(10)))
            {
                if (Select.Types == null)
                {
                    Select.ReferenceAllTypesInProject();
                }

                Select.selectTypeIndex = EditorGUILayout.Popup(Select.selectTypeIndex, Select.TypesAsStrings);
                GUIButtonRefreshTypes();
            }
            EditorGUILayout.EndToggleGroup();
        }

        private void GUICriteriaSearchPhrase()
        {
            Select.phraseSelection = EditorGUILayout.BeginToggleGroup("Object name contains phrase:", Select.phraseSelection);
            Select.selectPhrase = EditorGUILayout.TextField("Phrase: ", Select.selectPhrase);
            EditorGUILayout.EndToggleGroup();
        }

        private void GUIAdvancedFilters()
        {
            showAdvancedFilters = EditorGUILayout.Foldout(showAdvancedFilters, "Advanced Filters");
            if (showAdvancedFilters)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    GUIPropertyInComponent();
                }
            }
        }

        private void GUIPropertyInComponent()
        {
            Select.propertyInComponent = EditorGUILayout.BeginToggleGroup("Property in Component", Select.propertyInComponent);
            using (new EditorGUI.IndentLevelScope())
            {
                using (new EditorGUILayout.HorizontalScope(EditorStyles.inspectorFullWidthMargins, GUILayout.MaxHeight(10)))
                {
                    EditorGUILayout.PrefixLabel("Property of type:");
                    propertyTypeIndex = EditorGUILayout.Popup(propertyTypeIndex, PropertyTypes);
                }

                EditorGUILayout.LabelField("Property name", EditorStyles.miniLabel);
                using (new EditorGUILayout.HorizontalScope(EditorStyles.inspectorFullWidthMargins, GUILayout.MaxHeight(10)))
                {
                    propertyName = EditorGUILayout.TextField(propertyName);
                    EditorGUIUtility.labelWidth = 180;

                    ChoosePropertyType();
                }
            }
            EditorGUILayout.EndToggleGroup();
        }

        private void ChoosePropertyType()
        {
            switch (PropertyTypes[propertyTypeIndex])
            {
                case "string":
                    propertyOperator = (PropertyOperators)EditorGUILayout.EnumPopup(propertyOperator, GUILayout.MaxWidth(100.0f));
                    propertyPhrase = EditorGUILayout.TextField(propertyPhrase);
                    break;
                case "int":
                    propertyOperator = (PropertyOperators)EditorGUILayout.EnumPopup(propertyOperator, GUILayout.MaxWidth(100.0f));
                    propertyInt = EditorGUILayout.IntField(propertyInt);
                    break;
                case "float":
                    propertyOperator = (PropertyOperators)EditorGUILayout.EnumPopup(propertyOperator, GUILayout.MaxWidth(100.0f));
                    propertyFloat = EditorGUILayout.FloatField(propertyFloat);
                    break;
                case "Vector2":
                    propertyOperator = (PropertyOperators)EditorGUILayout.EnumPopup(propertyOperator, GUILayout.MaxWidth(100.0f));
                    propertyVector2 = EditorGUILayout.Vector2Field("", propertyVector2);
                    break;
                case "Vector3":
                    propertyOperator = (PropertyOperators)EditorGUILayout.EnumPopup(propertyOperator, GUILayout.MaxWidth(100.0f));
                    propertyVector3 = EditorGUILayout.Vector3Field("", propertyVector3);
                    break;
                case "bool":
                    propertyBool = EditorGUILayout.Toggle("", propertyBool);
                    break;
                case "object type":
                    propertyObjectTypeIndex = EditorGUILayout.Popup(propertyObjectTypeIndex, Select.TypesAsStrings);
                    propertyObjectName = EditorGUILayout.TextField(propertyObjectName);
                    break;
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

        private void GUIButtonFilterSelection()
        {
            if (GUILayout.Button("Apply filter on Scene", GUILayout.MinHeight(45)))
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

        private void GUIButtonSearchScene()
        {
            if (GUILayout.Button("Search whole Scene"))
            {
                Select.SelectGameObjects();
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
