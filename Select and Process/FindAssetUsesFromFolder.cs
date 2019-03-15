using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class FindAssetUsesFromFolder : BaseProcessSelected
{
    public override void ProcessSelectedObjects(GameObject[] selection)
    {
        var path = "Assets/_Common/Images/Ionicons_White/";
        LogAllImagespritesUsedInSelectedObjects(path, selection);
    }

    private static void LogAllImagespritesUsedInSelectedObjects(string path, GameObject[] selection)
    {
        foreach (var gameObject in selection)
        {
            var sprite = gameObject.GetComponent<Image>().sprite;
            if (sprite != null)
            {
                var spriteName = sprite.texture.name;
                foreach (var file in Directory.GetFiles(path))
                {
                    if (file.Contains(spriteName))
                    {
                        Debug.Log(spriteName);
                    }
                }
            }
        }
    }
}
