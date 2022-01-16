using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace FIMSpace.FEditor
{
    [CustomPropertyDrawer(typeof(FPD_TabAttribute))]
    public class FD_Tab : PropertyDrawer
    {
        FPD_TabAttribute Attribute { get { return ((FPD_TabAttribute)base.attribute); } }

        public static GUIStyle HeaderStyle { get { if (_headerStyle == null) { _headerStyle = new GUIStyle(EditorStyles.helpBox); _headerStyle.fontStyle = FontStyle.Bold; _headerStyle.alignment = TextAnchor.MiddleCenter; _headerStyle.fontSize = 11; } return _headerStyle; } }
        private static GUIStyle _headerStyle;

        private Texture2D icon;
        //private bool isUnfolded = false;

        private SerializedProperty foldProp; 

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Color c = GUI.color;
            FPD_TabAttribute att = (FPD_TabAttribute)base.attribute;
            position.height = Attribute.IconSize + 4;

            if (icon == null)
            {
                if (string.IsNullOrEmpty(att.IconContent) == false)
                {
                    GUIContent gc = EditorGUIUtility.IconContent(att.IconContent);
                    if (gc != null) if (gc.image != null) icon = (Texture2D)gc.image;
                }

                if (string.IsNullOrEmpty(att.ResourcesIconPath) == false)
                {
                    icon = Resources.Load<Texture2D>(att.ResourcesIconPath);
                }
            }

            Rect pos = position; pos.y += 2; pos.height = 28 + att.IconSize - 24;

            GUI.color = new Color(att.R, att.G, att.B);
            GUI.BeginGroup(pos, FGUI_Resources.HeaderBoxStyle);
            GUI.EndGroup();
            GUI.color = c;

            if (!string.IsNullOrEmpty(att.FoldVariable)) if ( foldProp == null ) foldProp = property.serializedObject.FindProperty(att.FoldVariable);

            bool folded = foldProp != null;
            string f = folded ? FGUI_Resources.GetFoldSimbol(foldProp.boolValue) : "";
            string header = folded ? (f + "    " + Attribute.HeaderText + "    " + f) : Attribute.HeaderText;
            //if (folded) isUnfolded = foldProp.boolValue;

            if (icon != null) if (GUI.Button(new Rect(pos.x + 4, pos.y + 3, att.IconSize, att.IconSize), new GUIContent(icon), EditorStyles.label)) { if (foldProp != null) foldProp.boolValue = !foldProp.boolValue; property.serializedObject.ApplyModifiedProperties(); }
            if (GUI.Button(pos, new GUIContent(header), FGUI_Resources.HeaderStyle)) { if (foldProp != null) foldProp.boolValue = !foldProp.boolValue; property.serializedObject.ApplyModifiedProperties(); }
            if (icon != null) if (GUI.Button(new Rect(pos.width - att.IconSize + 9, pos.y + 3, att.IconSize, att.IconSize), new GUIContent(icon), EditorStyles.label)) { if (foldProp != null) foldProp.boolValue = !foldProp.boolValue; property.serializedObject.ApplyModifiedProperties(); }

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!string.IsNullOrEmpty(Attribute.FoldVariable)) if (foldProp == null) foldProp = property.serializedObject.FindProperty(Attribute.FoldVariable);

            //if ( foldProp != null)
            //    isUnfolded = foldProp.boolValue;

            //if (isUnfolded)
                return base.GetPropertyHeight(property, label) + Attribute.IconSize - 8;
            //else
            //    return Attribute.IconSize - 8;
        }
    }
}

