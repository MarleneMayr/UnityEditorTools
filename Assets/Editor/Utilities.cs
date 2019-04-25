using UnityEditor;

namespace EditorTools
{
    class Utilities : EditorWindow
    {
        private static EditorWindow selectionTool;
        private static EditorWindow quickOverview;
        private static EditorWindow colorTool;

        /** Initialization **/

        private static void InitWindows()
        {
            selectionTool = GetWindow<SelectAndProcessTool>("Selection");
            quickOverview = GetWindow<QuickOverview>("Overview", typeof(SelectAndProcessTool));
            colorTool = GetWindow<ColorTool>("Colors", typeof(QuickOverview));
        }

        private static void InitTools()
        {
            SelectAndProcessTool.Init(selectionTool);
            QuickOverview.Init();
            ColorConverter.Init();
        }

        /** Menu Items **/

        [MenuItem("Tools/Utilites/Selection")]
        public static void ShowSelection()
        {
            InitWindows();
            InitTools();

            selectionTool.ShowTab();
        }

        [MenuItem("Tools/Utilites/Overview")]
        public static void ShowOverview()
        {
            InitWindows();
            InitTools();

            quickOverview.ShowTab();
        }

        [MenuItem("Tools/Utilites/Colors")]
        public static void ShowColors()
        {
            InitWindows();
            InitTools();

            colorTool.ShowTab();
        }
    }
}
