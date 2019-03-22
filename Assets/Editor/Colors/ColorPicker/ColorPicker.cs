using UnityEditor;

namespace EditorTools
{
    public class ColorPicker : EditorWindow
    {
        [MenuItem("Tools/Color Picker")]
        static void Init()
        {
            var window = GetWindow<ColorPicker>(); // Get existing open window or if none, make a new one
            window.Show();
        }

        void OnGUI()
        {

        }
    }
}
