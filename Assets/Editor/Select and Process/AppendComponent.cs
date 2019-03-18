using System;
using System.Linq;
using UnityEngine;

namespace EditorTools
{
    [CreateAssetMenu(fileName = "AppendComponent", menuName = "EditorToolProcessSelected/AppendComponent", order = 1)]
    public class AppendComponent : BaseProcessing
    {
        [SerializeField] private string typeString;

        internal override void processSelectedObjects(GameObject[] selectedGameObjects)
        {
            var type = Type.GetType(typeString);
            AssetDatabaseUtil.AllAssetsOfType(type);
            Debug.Log(type);
            selectedGameObjects.Where(o => o.GetComponent(type) == null).ToList().ForEach(o => o.AddComponent(type));
        }
    }
}
