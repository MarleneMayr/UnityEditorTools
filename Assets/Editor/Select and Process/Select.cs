using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EditorTools
{
    class Select
    {
        // GUI
        internal static int selectTypeIndex;
        internal static string selectPhrase;

        // Select Filters
        internal static bool typeSelection, phraseSelection = false;
        internal static bool activeFilter, childrenFilter = false;
        internal static bool sceneObjectsFilter, assetsFilter = true;

        // Types
        private static System.Type[] types;
        private static string[] typesAsStrings;


        public static System.Type[] Types => types;
        public static string[] TypesAsStrings => typesAsStrings;

        internal static void SelectGameObjects()
        {
            // CollapseHierarchy();
            var allObjects = FindAllGameObjects();
            FilterSelection(allObjects);
        }

        internal static void ApplyFilterToSelection()
        {
            var selection = Selection.gameObjects;
            FilterSelection(new List<GameObject>(selection));
        }

        // find and return all active, inactive objects containing of type text, including assets, prefabs etc.
        private static List<GameObject> FindAllGameObjects()
        {
            var allgameObjects = new List<GameObject>();
            var rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var gameObject in rootGameObjects)
            {
                var children = gameObject.GetComponentsInChildren<Transform>(true);
                foreach (var child in children)
                {
                    allgameObjects.Add(child.gameObject);
                }
            }
            return allgameObjects;
        }

        #region ReferenceAllTypesInProject

        internal static void ReferenceAllTypesInProject()
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

        #endregion // ReferenceAllTypesInProject

        private static void FilterSelection(List<GameObject> currentSelection)
        {
            var newSelection = new List<GameObject>();
            foreach (var gameObject in currentSelection)
            {
                if (childrenFilter == true)
                {
                    var children = gameObject.GetComponentsInChildren<Transform>(true);
                    foreach (var child in children)
                    {
                        if (newSelection.Any(obj => obj.GetInstanceID() == child.gameObject.GetInstanceID()) == false)
                        {
                            var isMatching = GameObjectMatchesCriteria(child.gameObject);
                            if (isMatching)
                            {
                                newSelection.Add(child.gameObject);
                            }
                        }
                    }
                }
                else
                {
                    if (GameObjectMatchesCriteria(gameObject))
                    {
                        newSelection.Add(gameObject);
                    }
                }
            }

            if (newSelection.Count > 0)
            {
                Selection.objects = newSelection.ToArray();
                //Debug.Log($"{newSelection.Count} objects selected.");
            }
            else
            {
                Debug.Log("Selection criteria match no objects.");
            }
        }

        private static bool GameObjectMatchesCriteria(GameObject gameObject)
        {
            bool phraseMatch = phraseSelection ? gameObject.name.ToLower().Contains(selectPhrase.ToLower()) : true; // check for phrase in name only if phraseSelection is true
            bool typeMatch = typeSelection ? (gameObject).GetComponent(types[selectTypeIndex]) : true; // check for type in components only if typeSelection is true
            bool activeMatch = activeFilter ? (gameObject).activeSelf : true; // check for active gameObject only if activeGOFilter is true

            return phraseMatch && typeMatch && activeMatch;
        }

        private void CollapseHierarchy()
        {
            // Todo implement this
        }
    }
}
