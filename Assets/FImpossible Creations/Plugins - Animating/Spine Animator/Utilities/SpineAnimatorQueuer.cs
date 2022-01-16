using FIMSpace.FEditor;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FIMSpace.FSpine
{
    [DefaultExecutionOrder(-12)]
    [AddComponentMenu("FImpossible Creations/Spine Animator Utilities/Spine Animator Queuer")]
    public class SpineAnimatorQueuer : MonoBehaviour
    {
        [Tooltip("Can be used to fade out all spine animators")]
        [FPD_Suffix(0f, 1f)]
        public float SpineAnimatorsAmount = 1f;

        [SerializeField]
        internal List<FSpineAnimator> updateOrder;

        void Update()
        {
            for (int i = updateOrder.Count - 1; i >= 0; i--)
            {
                if (updateOrder[i] == null)
                {
                    updateOrder.RemoveAt(i);
                }
                else
                {
                    if (updateOrder[i].enabled) updateOrder[i].enabled = false;
                    updateOrder[i].Update();
                }
            }
        }

        void FixedUpdate()
        {
            for (int i = updateOrder.Count - 1; i >= 0; i--)
            {
                if (updateOrder[i] == null)
                {
                    updateOrder.RemoveAt(i);
                }
                else
                {
                    if (updateOrder[i].enabled) updateOrder[i].enabled = false;
                    updateOrder[i].FixedUpdate();
                }
            }
        }

        void LateUpdate()
        {
            for (int i = 0; i < updateOrder.Count; i++)
            {
                if (SpineAnimatorsAmount < 1f) updateOrder[i].SpineAnimatorAmount = SpineAnimatorsAmount;
                updateOrder[i].LateUpdate();
            }
        }
    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(SpineAnimatorQueuer))]
    [UnityEditor.CanEditMultipleObjects]
    public class SpineAnimatorQueuerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            SpineAnimatorQueuer targetScript = (SpineAnimatorQueuer)target;

            if (targetScript.updateOrder == null) targetScript.updateOrder = new List<FSpineAnimator>();

            serializedObject.Update();

            if (!Application.isPlaying)
                EditorGUILayout.HelpBox("Spine Animators will be disabled in playmode but this component will update them in defined update order internally", MessageType.Info);

            Color preC = GUI.color;
            GUI.color = new Color(0.7f, 0.7f, 0.975f, .85f);
            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyleH); GUI.color = preC;
            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxBlankStyle);


            EditorGUIUtility.labelWidth = 150;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SpineAnimatorsAmount"));
            EditorGUIUtility.labelWidth = 0;
            GUILayout.Space(6);

            EditorGUILayout.BeginHorizontal(FGUI_Resources.BGInBoxStyle);
            EditorGUILayout.LabelField("Define Execution Order", EditorStyles.boldLabel);

            if (GUILayout.Button("+", GUILayout.Width(32)))
            {
                targetScript.updateOrder.Add(null);
                EditorUtility.SetDirty(targetScript);
                serializedObject.Update();
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(4);

            for (int i = 0; i < targetScript.updateOrder.Count; i++)
            {
                GUILayout.Space(6);
                EditorGUILayout.BeginHorizontal();
                FSpineAnimator pre = targetScript.updateOrder[i];

                string title = "[" + (i + 1) + "] ";

                if (pre != null)
                {
                    if (pre._editor_Title != "Spine Animator 2")
                        title += pre._editor_Title;

                    GUIContent t = new GUIContent(title);
                    EditorGUILayout.LabelField(t, GUILayout.Width(EditorStyles.label.CalcSize(t).x + 4));
                }

                FSpineAnimator drag = (FSpineAnimator)EditorGUILayout.ObjectField(targetScript.updateOrder[i], typeof(FSpineAnimator), true);

                if (drag != pre)
                {
                    if (!targetScript.updateOrder.Contains(drag))
                    {
                        targetScript.updateOrder[i] = drag;
                        EditorUtility.SetDirty(targetScript);
                        serializedObject.Update();
                    }
                }

                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginVertical();

                int h = 12;
                if (i == 0 || i == targetScript.updateOrder.Count - 1) h = 16;

                if (i > 0)
                    if (GUILayout.Button("▲", FGUI_Resources.ButtonStyle, new GUILayoutOption[2] { GUILayout.Width(22), GUILayout.Height(h) }))
                    {
                        FSpineAnimator toMove = targetScript.updateOrder[i];
                        FSpineAnimator toSwap = targetScript.updateOrder[i - 1];

                        targetScript.updateOrder[i - 1] = toMove;
                        targetScript.updateOrder[i] = toSwap;

                        EditorUtility.SetDirty(targetScript);
                        serializedObject.Update();
                        serializedObject.ApplyModifiedProperties();
                        return;
                    }

                if (i < targetScript.updateOrder.Count - 1)
                    if (GUILayout.Button("▼", FGUI_Resources.ButtonStyle, new GUILayoutOption[2] { GUILayout.Width(22), GUILayout.Height(h) }))
                    {
                        FSpineAnimator toMove = targetScript.updateOrder[i];
                        FSpineAnimator toSwap = targetScript.updateOrder[i + 1];

                        targetScript.updateOrder[i + 1] = toMove;
                        targetScript.updateOrder[i] = toSwap;

                        EditorUtility.SetDirty(targetScript);
                        serializedObject.Update();
                        serializedObject.ApplyModifiedProperties();
                        return;
                    }

                EditorGUILayout.EndVertical();

                if (GUILayout.Button("X", FGUI_Resources.ButtonStyle, new GUILayoutOption[2] { GUILayout.Width(22), GUILayout.Height(16) }))
                {
                    targetScript.updateOrder.RemoveAt(i);
                    EditorUtility.SetDirty(targetScript);
                    serializedObject.Update();
                    serializedObject.ApplyModifiedProperties();
                    return;
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(2);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
            GUILayout.Space(2);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

}
