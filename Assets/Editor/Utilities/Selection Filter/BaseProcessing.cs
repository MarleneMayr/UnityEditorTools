using UnityEngine;

namespace EditorTools
{
    [System.Serializable]
    public abstract class BaseProcessing : ScriptableObject
    {
        abstract internal void processSelectedObjects(GameObject[] selectedGameObjects);
    }
}
