using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
//using UnityEngine.UI;

namespace EditorTools
{
    public class ColorConverter : EditorWindow
    {
        private List<AnimationClip> animationClips = new List<AnimationClip>();
        private Color inputColor;
        private int selectTargetClipIndex;

        [MenuItem("Tools/Color Converter")]
        static void Init()
        {
            var window = GetWindow<ColorConverter>(); // Get existing open window or if none, make a new one
            window.Show();
        }

        private void OnSelectionChange()
        {
            animationClips.Clear();
            var objects = Selection.objects.ToList();
            objects.Where(x => x.GetType() == typeof(AnimationClip)).ToList().ForEach(o => animationClips.Add(o as AnimationClip));
            Repaint();
        }

        #region GUI

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Selection", EditorStyles.boldLabel);

            foreach (var clip in animationClips)
            {
                EditorGUILayout.LabelField("Clip: ", clip.name);

                if (AnimationUtility.GetCurveBindings(clip).Any())
                {
                    var rgbaValues = GetRGBAValuesInClip(clip);
                    var rgbValues = GetRGBValuesInClip(clip);
                    GUIRGBSelectionValues("RGB", rgbValues);

                    var colorValuesConverted = rgbValues.Select(v => v = Mathf.Round(v * 255));
                    GUIRGBSelectionValues("RGB(A) converted", rgbValues);

                    GUIHEXFromRGB("HEX converted", rgbValues);
                }
                EditorGUILayout.Space();
            }

            //INPUT
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); //horizontal Line
            EditorGUILayout.Space();

            var inputRGB = GUIInputRGB();
            GUIRGBSelectionValues("RGB", inputRGB);

            GUISelectTarget();
            GUIApplyColorToClip();
        }

        private List<float> GUIInputRGB()
        {
            inputColor = EditorGUILayout.ColorField("Choose color", inputColor);
            var inputValues = new List<float>();
            inputValues.Add(inputColor.r);
            inputValues.Add(inputColor.g);
            inputValues.Add(inputColor.b);
            inputValues.Add(inputColor.a);
            return inputValues;
        }

        private void GUIRGBSelectionValues(string label, List<float> values)
        {
            var rgb = string.Join(", ", values);
            EditorGUILayout.TextField(label, rgb);
        }

        private void GUIHEXFromRGB(string label, List<float> values)
        {
            var hex = ColorUtility.ToHtmlStringRGB(new Color(values[0], values[1], values[2]));
            EditorGUILayout.TextField(label, hex);
        }

        private void GUIHEXFromRGBA(string label, List<float> values)
        {
            var hex = ColorUtility.ToHtmlStringRGB(new Color(values[0], values[1], values[2], values[3]));
            EditorGUILayout.TextField(label, hex);
        }

        private void GUISelectTarget()
        {
            var clipStrings = new string[animationClips.Count];
            for (var i = 0; i < animationClips.Count; i++)
            {
                clipStrings[i] = animationClips[i].name;
            }
            selectTargetClipIndex = EditorGUILayout.Popup(selectTargetClipIndex, clipStrings);
        }

        private void GUIApplyColorToClip()
        {
            if (animationClips.Count > 0)
            {
                var currentClip = animationClips[selectTargetClipIndex];
                if (GUILayout.Button($"Apply color to \"{currentClip.name}\".", GUILayout.MinHeight(35)))
                {
                    if (AnimationUtility.GetCurveBindings(currentClip).Any())
                    {
                        // ApplyColorToClip(inputColor, currentClip);
                    }
                    else
                    {
                        ShowNotification(new GUIContent("No bindings found on target clip."));
                    }
                }
            }
        }

        #endregion //GUI

        private List<float> GetRGBAValuesInClip(AnimationClip clip)
        {
            var colorValues = new List<float>();
            AnimationUtility.GetCurveBindings(clip).ToList().ForEach(binding => AnimationUtility.GetEditorCurve(clip, binding).keys.ToList().ForEach(k => colorValues.Add(k.value)));
            return colorValues;
        }

        private List<float> GetRGBValuesInClip(AnimationClip clip)
        {
            var colorValues = new List<float>();
            AnimationUtility.GetCurveBindings(clip).Take(3).ToList().ForEach(binding => AnimationUtility.GetEditorCurve(clip, binding).keys.ToList().ForEach(k => colorValues.Add(k.value)));
            return colorValues;
        }

        private void ApplyColorToClip(Color rgba, AnimationClip target)
        {
            var bindings = AnimationUtility.GetCurveBindings(target);

            bindings.ToList().ForEach(b => Debug.Log(b.propertyName == "m_Color.r"));
            ChangeKeys(rgba, bindings.Where(b => b.propertyName == "m_Color.r").ToList().Select(binding => AnimationUtility.GetEditorCurve(target, binding)));
        }

        private void ChangeKeys(Color rgba, IEnumerable<AnimationCurve> curves)
        {
            foreach (var curve in curves)
            {
                foreach (var key in curve.keys)
                {
                    //todo save values
                    //remove key    
                    //add new key with new value
                }
            }
        }
    }
}
