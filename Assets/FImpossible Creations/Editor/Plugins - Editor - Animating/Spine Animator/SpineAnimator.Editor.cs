using FIMSpace.FSpine;
using UnityEditor;
using UnityEngine;

[UnityEditor.CustomEditor(typeof(FSpineAnimator))]
/// <summary>
/// FM: Editor class component to enchance controll over component from inspector window
/// </summary>
[CanEditMultipleObjects]
public partial class FSpineAnimator_Editor : Editor
{
    [MenuItem("CONTEXT/FSpineAnimator/Switch displaying header bar")]
    private static void HideFImpossibleHeader(MenuCommand menuCommand)
    {
        int current = EditorPrefs.GetInt("FSpineHeader", 1);
        if (current == 1) current = 0; else current = 1;
        EditorPrefs.SetInt("FSpineHeader", current);
    }

    public override void OnInspectorGUI()
    {

        Undo.RecordObject(target, "Spine Animator Inspector");

        serializedObject.Update();

        FSpineAnimator Get = (FSpineAnimator)target;
        string title = drawDefaultInspector ? " Default Inspector" : " " + Get._editor_Title;
        if (!drawNewInspector) title = " Old GUI Version";

        if (EditorPrefs.GetInt("FSpineHeader", 1) == 1)
        {
            HeaderBoxMain(title, ref Get.DrawGizmos, ref drawDefaultInspector, _TexSpineAnimIcon, Get, 27);
            GUILayout.Space(4f);
        }
        else
        {
            GUILayout.Space(2f);
        }


        #region Default Inspector
        if (drawDefaultInspector)
        {
            // Draw default inspector without not needed properties
            DrawPropertiesExcluding(serializedObject, new string[0] { });
        }
        else
        #endregion
        {
            if (drawNewInspector)
            {
                GUILayout.Space(4f);
                DrawNewGUI();
            }
            else
            {
                DrawOldGUI();
            }
        }

        SetupLangs();

        // Apply changed parameters variables
        serializedObject.ApplyModifiedProperties();

    }


}
