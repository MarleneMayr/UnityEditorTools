using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace EditorTools
{
    [CreateAssetMenu(fileName = "FindAssetUsesFromFolder", menuName = "EditorToolProcessSelected/FindAssetUsesFromFolder", order = 1)]
    public class FindAssetUsesFromFolder : BaseProcessing
    {
        [SerializeField] private static string path;

        internal override void processSelectedObjects(GameObject[] selectedGameObjects)
        {
            LogAllImagespritesUsedInSelectedObjects(selectedGameObjects);
        }

        private static void LogAllImagespritesUsedInSelectedObjects(GameObject[] selection)
        {
            foreach (var gameObject in selection)
            {
                var sprite = gameObject.GetComponent<Image>().sprite;
                if (sprite != null)
                {
                    Directory.GetFiles(path).Where(f => f.Contains(sprite.texture.name)).ToList().ForEach(i => Debug.Log(i));
                }
            }
        }
    }
}
