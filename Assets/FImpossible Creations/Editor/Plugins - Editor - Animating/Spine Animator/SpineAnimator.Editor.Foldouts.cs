using FIMSpace.FEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class FSpineAnimator_Editor
{
    static bool drawSpineTransforms = false;
    void Fold_DrawSpineTransforms()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button(new GUIContent("  " + FGUI_Resources.GetFoldSimbol(drawSpineTransforms, 10, "►") + "  " + Lang("Spine Bones Chain") + " (" + Get.SpineBones.Count + ")", FGUI_Resources.Tex_Bone, "Adjust count of chain bones or limit single bones motion with sliders"), FGUI_Resources.FoldStyle, new GUILayoutOption[] { GUILayout.Height(24) })) drawSpineTransforms = !drawSpineTransforms;
        if (GUILayout.Button("Reset", new GUILayoutOption[2] { GUILayout.Width(62), GUILayout.Height(18) })) { Get.SpineBones.Clear(); serializedObject.Update(); serializedObject.ApplyModifiedProperties(); Get._GizmosRefreshChainList(true); return; }

        EditorGUILayout.EndHorizontal();

        if (drawSpineTransforms)
        {
            GUILayout.Space(3);

            int strt = 0; int cnt = Get.SpineBones.Count;
            if (!Get.LastBoneLeading) strt = 1; else cnt -= 1;


            if (!Get.LastBoneLeading)
            {
                GUILayout.Space(2);
                EditorGUIUtility.labelWidth = 90;
                GUI.enabled = false; EditorGUILayout.ObjectField(new GUIContent("Head Bone: ", "Using sliders you can limit motion of bones or remove with 'X' button"), Get.SpineBones[0].transform, typeof(Transform), true); GUI.enabled = true;
                EditorGUIUtility.labelWidth = 0;
                GUILayout.Space(6);

                if ( Get.SpineAnimatorAmount >= 1f) GUILayout.Space(3);
            }
            else
            {
                GUILayout.Space(3);
            }

            if (Get.SpineAnimatorAmount >= 1f)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.8f);
                EditorGUILayout.LabelField(new GUIContent( "Spine Amount must be < 100% for weight sliders", "When Spine Animator Amount is 99% or less it enables blending then you can use motion weight blending sliders below"), FGUI_Resources.HeaderStyle);
                GUILayout.Space(3); GUI.color = c;
            }

            for (int i = strt; i < cnt; i++)
            {
                EditorGUILayout.BeginHorizontal();

                Color cc = c; cc.a = Get.SpineBones[i].MotionWeight * 0.8f + 0.2f;
                GUI.color = cc;
                GUI.enabled = false; EditorGUILayout.ObjectField(new GUIContent("", "Using slider you can limit motion of this bone or remove it with 'X' button"), Get.SpineBones[i].transform, typeof(Transform), true); GUI.enabled = true;
                GUI.color = c;

                if ( Get.SpineAnimatorAmount >= 1f) GUI.enabled = false;
                Get.SpineBones[i].MotionWeight = GUILayout.HorizontalSlider(Get.SpineBones[i].MotionWeight, 0f, 1f, GUILayout.Width(40f));
                GUI.enabled = true;

                if (GUILayout.Button(new GUIContent("X", "Remove bone from chain with this button or limit it's motion weight with slider"), new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(14) }))
                {
                    Get.SpineBones.RemoveAt(i);
                    EditorUtility.SetDirty(target);
                    break;
                }

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(1);
            }


            if (Get.LastBoneLeading)
            {
                GUILayout.Space(5);
                EditorGUIUtility.labelWidth = 90;
                GUI.enabled = false; EditorGUILayout.ObjectField(new GUIContent("Head Bone: ", "Using sliders you can limit motion of bones or remove with 'X' button"), Get.SpineBones[Get.SpineBones.Count - 1].transform, typeof(Transform), true); GUI.enabled = true;
                EditorGUIUtility.labelWidth = 0;
            }


            GUILayout.Space(2f);
        }

        GUILayout.Space(2f);
    }

    bool drawAdditionalSetup = true;
    private void Fold_DrawAdditionalSetup()
    {
        GUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);

        if (GUILayout.Button(new GUIContent(FGUI_Resources.GetFoldSimbol(drawAdditionalSetup, 10, "►") + "  " + Lang("Optimization And More"), FGUI_Resources.TexAddIcon), FGUI_Resources.FoldStyle)) drawAdditionalSetup = !drawAdditionalSetup;

        if (drawAdditionalSetup)
        {
            //if (animatorAnimPhys && Get.AnimatePhysics == false)
            //{
            //    FGUI_Inspector.DrawWarning(" Unity's Animator is using 'Animate Physics'!");
            //    GUI.color = new Color(.9f, .9f, 0.6f, 1f);
            //}

            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();

            EditorGUIUtility.labelWidth = 80; EditorGUIUtility.fieldWidth = 0;
            EditorGUILayout.PropertyField(sp_DeltaType);

            EditorGUIUtility.labelWidth = 40;
            EditorGUIUtility.fieldWidth = Get.UpdateRate == 0 ? 10 : 30;
            EditorGUILayout.PropertyField(sp_UpdateRate, new GUIContent("Rate", sp_UpdateRate.tooltip), GUILayout.Width(Get.UpdateRate == 0 ? 60 : 70) );

            GUI.color = new Color(1f, 1f, 1f, 0.75f);
            EditorGUILayout.LabelField(Get.UpdateRate == 0 ? "Unlimited" : "FPS", GUILayout.Width(Get.UpdateRate == 0 ? 60 : 28) );
            GUI.color = c;

            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = 0; EditorGUIUtility.fieldWidth = 0;

            GUILayout.Space(6);

            EditorGUILayout.BeginHorizontal();

            if (animator) if (animator.updateMode == AnimatorUpdateMode.AnimatePhysics) GUI.color = new Color(0.6f, 1f, 0.6f, 1f);
            EditorGUILayout.PropertyField(sp_PhysicalClock); 
            GUI.color = c;
            GUILayout.Space(3);
            EditorGUILayout.PropertyField(sp_UpdateAsLast);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(3);
            EditorGUILayout.PropertyField(sp_StartAfterTPose);
            GUILayout.Space(5);
            El_DrawOptimizeWithMesh();

            GUILayout.Space(4f);
        }


        GUILayout.EndVertical();
        GUILayout.Space(-5);
    }
}