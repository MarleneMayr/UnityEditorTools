using CurvedUI;
using UnityEngine;


public class AppendComponent : ScriptableObject
{
    public AppendComponent()
    {

    }

    private void AppendComponentToObjects(GameObject[] selection)
    {
        foreach (GameObject gameObject in selection)
        {
            var component = gameObject.GetComponent<CurvedUITMP>();
            if (component != null)
            {
                gameObject.AddComponent<CurvedUITMP>();
            }
        }
    }
}
