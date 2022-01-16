using FIMSpace;
using FIMSpace.FEditor;
using FIMSpace.FSpine;
using UnityEditor;
using UnityEngine;

public partial class FSpineAnimator_Editor
{
    static bool drawMain = true;
    static bool drawAnimationOptions = true;
    static bool drawQuickCorrection = false;
    static bool drawAdvancedCorrection = false;
    static bool drawDebug = false;
    static bool drawPreciseAutoCorr = false;

    private Transform headBone;

    void DrawOldGUI()
    {
        GUILayout.Space(5f);
        Color preCol = GUI.color;

        EditorGUILayout.BeginVertical(FEditor_Styles.Style(FColorMethods.ChangeColorAlpha(Color.white, 0.25f)));
        EditorGUI.indentLevel++;

        drawMain = EditorGUILayout.Foldout(drawMain, "Main Parameters", true);


        #region Main Tab

        if (drawMain)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (Get.SpineBones == null || Get.SpineBones.Count < 1)
            {
                GUILayout.BeginHorizontal(FEditor_Styles.YellowBackground);
                EditorGUILayout.HelpBox("Put here two marginal bones from hierarchy and click 'Get' to create spine chain of section you want to animate with spine animator", MessageType.Info);
                GUILayout.EndHorizontal();


            }

            EditorGUILayout.BeginVertical(FEditor_Styles.LGrayBackground);

            if (Get.SpineBones == null || Get.SpineBones.Count < 1)
            {
                GUIStyle smallStyle = new GUIStyle(EditorStyles.miniLabel) { fontStyle = FontStyle.Italic, fontSize = 9 };
                GUI.color = new Color(1f, 1f, 1f, 0.7f);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("             Enter for tooltip", "If you rigging quadroped or other animal, start bone should be pelvis bone with back legs and tail inside the hierarchy"), smallStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label(new GUIContent("Enter for tooltip                       ", "If you rigging quadroped or other animal, end bone should be chest bone / neck bone or head bone, depends of your needs and model structure"), smallStyle);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(1f);
            }

            EditorGUILayout.BeginHorizontal();
            int wrong = 0;
            if (Get.SpineBones != null)
            {
                if (Get.SpineBones.Count < 2) wrong = 2;
                else
                {
                    if (startBone != Get.SpineBones[0].transform || endBone != Get.SpineBones[Get.SpineBones.Count - 1].transform)
                    {
                        wrong = 3;
                    }
                }
            }
            else wrong = 1;

            if (wrong == 1) GUI.color = new Color(1f, 0.3f, 0.3f, 0.85f);
            if (wrong == 2) GUI.color = new Color(1f, 0.7f, 0.2f, 0.85f);

            EditorGUI.indentLevel--;

            EditorGUIUtility.labelWidth = 42f;
            startBone = Get._gizmosEditorStartPreview;
            endBone = Get._gizmosEditorEndPreview;
            if (startBone == null) TryFindStartBone();
            if (endBone == null) TryFindEndBone();

            startBone = (Transform)EditorGUILayout.ObjectField(new GUIContent("Start", "Put here first bone in hierarchy depth for automatically get chain of bones to end one"), startBone, typeof(Transform), true);
            endBone = (Transform)EditorGUILayout.ObjectField(new GUIContent("End", "Put here last bone in hierarchy depth for automatically get chain of bones from start one"), endBone, typeof(Transform), true);

            EditorGUIUtility.labelWidth = 0f;

            if (GUILayout.Button(new GUIContent("L", "Automatically get last bone in hierarchy - it depends of children placement, then sometimes last bone can be found wrong, whne you have arms/legs bones inside, if they're higher, algorithm will go through them"), new GUILayoutOption[2] { GUILayout.MaxWidth(24), GUILayout.MaxHeight(14) })) GetLastBoneInHierarchy();

            if (wrong == 3) GUI.color = new Color(0.2f, 1f, 0.4f, 0.85f);
            if (startBone != null && endBone != null)
            {
                GUI.color = new Color(0.3f, 1f, 0.4f, 0.8f);

                if (Get.SpineBones != null)
                {
                    if (Get.SpineBones.Count > 0)
                    {
                        if (startBone != Get.SpineBones[0].transform || endBone != Get.SpineBones[Get.SpineBones.Count - 1].transform) wrong = 3; else GUI.color = FColorMethods.ChangeColorAlpha(preCol, 0.7f);
                    }
                }
            }

            if (GUILayout.Button(new GUIContent("Get"), new GUILayoutOption[2] { GUILayout.MaxWidth(36), GUILayout.MaxHeight(14) }))
            {
                Get.CreateSpineChain(Get._gizmosEditorStartPreview, Get._gizmosEditorEndPreview);
                //GetBonesChainFromStartToEnd();
                //Get.TryAutoCorrect(null, false);
                EditorUtility.SetDirty(target);
            }


            GUI.color = preCol;

            EditorGUILayout.EndHorizontal();


            if (Get.SpineBones == null || Get.SpineBones.Count < 1)
            {
                GUIStyle smallStyle = new GUIStyle(EditorStyles.miniLabel) { fontStyle = FontStyle.Italic, fontSize = 9 };
                GUI.color = new Color(1f, 1f, 1f, 0.7f);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("        Pelvis bone with Legs", "If you rigging quadroped or other animal, start bone should be pelvis bone with back legs and tail inside the hierarchy"), smallStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label(new GUIContent("Chest/Neck/Head bone             ", "If you rigging quadroped or other animal, end bone should be chest bone / neck bone or head bone, depends of your needs and model structure"), smallStyle);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(1f);
            }


            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel++;

            if (Get.SpineBones == null || Get.SpineBones.Count < 1)
            {
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
                return;
            }

            EditorGUIUtility.labelWidth = 148f;
            EditorGUILayout.PropertyField(sp_forw);
            EditorGUIUtility.labelWidth = 0;

            if (Get.SpineBones.Count < 1)
                EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);
            //EditorGUILayout.BeginHorizontal(FEditor_Styles.Style(new Color32(99, 50, 166, 45)));
            else
                EditorGUILayout.BeginVertical(FEditor_Styles.LGrayBackground);
            //EditorGUILayout.BeginHorizontal(FEditor_Styles.Style(new Color32(10, 66, 175, 25)));

            //EditorGUILayout.PropertyField(sp_spines, true);

            drawSpineTransforms = EditorGUILayout.Foldout(drawSpineTransforms, new GUIContent("Spine Transforms", "Spine chain transforms"), true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });

            if (drawSpineTransforms)
            {
                EditorGUIUtility.labelWidth = 120;
                for (int i = 0; i < Get.SpineBones.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    GUI.enabled = false;
                    EditorGUILayout.ObjectField("Spine Bone [" + i + "]", Get.SpineBones[i].transform, typeof(Transform), true);
                    if (i != 0 && i != Get.SpineBones.Count - 1) GUI.enabled = true;

                    if (GUILayout.Button("X", new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(14) }))
                    {
                        Get.SpineBones.RemoveAt(i);
                        EditorUtility.SetDirty(target);
                        break;
                    }

                    GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUIUtility.labelWidth = 0;
            }

            EditorGUILayout.EndVertical();

            EditorGUIUtility.labelWidth = 124f;
            EditorGUI.indentLevel--;

            EditorGUILayout.BeginVertical(FEditor_Styles.Style(new Color32(0, 200, 100, 22)));
            EditorGUILayout.BeginVertical(FEditor_Styles.Style(new Color32(0, 200, 100, 0)));
            GUILayout.Space(2f);
            EditorGUILayout.PropertyField(sp_SpineAnimatorAmount, true);
            GUILayout.Space(3f);

            EditorGUIUtility.labelWidth = 128f;

            GUI.color = new Color(0.3f, 1f, 0.4f, 0.8f);
            EditorGUILayout.PropertyField(sp_LastBoneLeading, true);
            GUI.color = preCol;

            GUILayout.Space(2f);

            //bool animatorDetected = CheckForAnimator(Get);
            //if (!animatorDetected && Get.SyncWithAnimator) GUI.color = new Color(1f, 0.2f, 0.2f, 0.8f);
            //else if (animatorDetected && Get.SyncWithAnimator == false) GUI.color = new Color(1f, 1f, 0.35f, 0.8f);

            //EditorGUIUtility.labelWidth = 163f;
            //EditorGUILayout.PropertyField(sp_SyncWithAnimator, true);
            GUI.color = preCol;

            GUILayout.Space(2f);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
            GUILayout.Space(2f);

            EditorGUIUtility.labelWidth = 0f;

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel++;
        }

        #endregion

        #region Animation Options

        EditorGUILayout.BeginVertical(FEditor_Styles.LGrayBackground);
        drawAnimationOptions = EditorGUILayout.Foldout(drawAnimationOptions, "Animation Options", true);

        if (drawAnimationOptions)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUI.indentLevel--;
            GUI.color = new Color(0.55f, 0.75f, 0.9f, 0.85f);

            GUILayout.BeginVertical(FEditor_Styles.Style(new Color32(33, 130, 200, 24)));

            EditorGUIUtility.labelWidth = 105f;
            GUILayout.Space(4f);

            Color preCc = GUI.color;

            if (Get.UseCollisions) if (Get.UseTruePosition)
                {
                    if (Get.PosSmoother < 0.075f) GUI.color = new Color(0.9f, 0.5f, 0.5f); else if (Get.PosSmoother < 0.225f) GUI.color = Color.Lerp(new Color(0.9f, 0.6f, 0.6f), preCc, Mathf.InverseLerp(0.075f, 0.225f, Get.PosSmoother));
                }

            EditorGUILayout.PropertyField(sp_PositionsSmoother, true);
            GUI.color = preCc;

            EditorGUILayout.PropertyField(sp_RotationsSmoother, true);
            if (Get.PosSmoother > 0.1f || Get.Springiness > 0f) EditorGUILayout.PropertyField(sp_MaxStretching, true);
            GUILayout.Space(5f);

            EditorGUILayout.EndVertical();

            GUILayout.BeginVertical(FEditor_Styles.Style(new Color32(33, 130, 230, 58)));

            EditorGUILayout.PropertyField(sp_AngleLimit, true);
            EditorGUILayout.PropertyField(sp_LimitingAngleSmoother, true);

            EditorGUILayout.EndVertical();

            GUILayout.BeginVertical(FEditor_Styles.Style(new Color32(33, 130, 230, 58)));

            EditorGUIUtility.labelWidth = 120f;
            EditorGUILayout.PropertyField(sp_StraighteningSpeed, true);
            EditorGUIUtility.labelWidth = 0f;

            if (Get.StraightenSpeed > 0f)
            {
                EditorGUI.indentLevel++;
                EditorGUIUtility.labelWidth = 121f;
                EditorGUILayout.PropertyField(sp_TurboStraighten, true);
                EditorGUI.indentLevel--;
                GUILayout.Space(3f);
                EditorGUIUtility.labelWidth = 105f;
            }

            EditorGUILayout.EndVertical();

            GUILayout.BeginVertical(FEditor_Styles.LBlueBackground);

            EditorGUIUtility.labelWidth = 105f;

            if (!Get.LastBoneLeading) if (Get.Springiness > 0) if (Get.GoBackSpeed <= 0) GUI.color = new Color(0.55f, 0.9f, 1f, 1f);
            EditorGUILayout.PropertyField(sp_GoBackSpeed, true);
            EditorGUILayout.PropertyField(sp_Slithery, true);

            GUI.color = new Color(0.55f, 0.75f, 0.9f, 0.85f);


            if (!Get.LastBoneLeading)
            {
                if (Get.Springiness <= 0)
                    if (Get.GoBackSpeed > 0.15f)
                        GUI.color = new Color(0.55f, 0.9f, 1f, 0.95f);

                EditorGUILayout.PropertyField(sp_Springiness, true);
            }


            EditorGUILayout.EndVertical();

            EditorGUIUtility.labelWidth = 0f;
            EditorGUILayout.EndVertical();
            GUILayout.Space(5f);
            EditorGUI.indentLevel++;

            GUI.color = preCol;
        }

        EditorGUILayout.EndVertical();

        #endregion

        #region Basic Correction

        EditorGUILayout.BeginVertical(FEditor_Styles.LGrayBackground);

        EditorGUILayout.BeginHorizontal();
        drawQuickCorrection = EditorGUILayout.Foldout(drawQuickCorrection, "Main Tuning Options", true);

        GUI.color = preCol;

        EditorGUILayout.EndHorizontal();

        if (drawPreciseAutoCorr)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.indentLevel--;
            EditorGUIUtility.labelWidth = 74f;

            if (!headBone) GUI.color = new Color(0.9f, 0.3f, 0.3f, 0.9f);
            headBone = (Transform)EditorGUILayout.ObjectField(new GUIContent("Head bone", "Head bone or some bone before, it's important to be in front of spine and not included in spine animator's chain"), headBone, typeof(Transform), true);
            GUI.color = preCol;

            EditorGUI.indentLevel++;
            EditorGUIUtility.labelWidth = 0f;

            EditorGUILayout.EndHorizontal();
        }

        if (drawQuickCorrection)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(3f);

            EditorGUIUtility.labelWidth = 146f;

            if (!Application.isPlaying)
            {
                EditorGUILayout.PropertyField(sp_StartAfterTPose, true);
            }

            GUILayout.Space(3f);
            EditorGUILayout.PropertyField(sp_MainPivotOffset, true);

            EditorGUIUtility.labelWidth = 0f;
            GUILayout.Space(5f);

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();

        #endregion

        #region Advanced correction

        EditorGUILayout.BeginVertical(FEditor_Styles.LGrayBackground);
        drawAdvancedCorrection = EditorGUILayout.Foldout(drawAdvancedCorrection, "Advanced Options", true);

        if (drawAdvancedCorrection)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Space(4f);
            EditorGUIUtility.labelWidth = 144f;
            if (!Application.isPlaying) EditorGUILayout.PropertyField(sp_PhysicalClock, true);
            EditorGUILayout.PropertyField(sp_DeltaType);

            EditorGUIUtility.labelWidth = 166f;

            if (Get.AnchorRoot && !Get.UpdateAsLast) GUI.color = new Color(0.5f, 1f, 0.65f, 0.85f);

            if (!Application.isPlaying) EditorGUILayout.PropertyField(sp_UpdateAsLast, true);
            GUI.color = preCol;

            FEditor_Styles.DrawUILine(new Color(0.6f, 0.6f, 0.6f, 0.4f));

            GUILayout.BeginVertical(FEditor_Styles.Style(new Color32(33, 200, 130, 24)));
            EditorGUILayout.PropertyField(sp_LeadBoneRotationOffset, true);

            serializedObject.ApplyModifiedProperties();

            GUILayout.EndVertical();

            GUILayout.Space(4f);

            FEditor_Styles.DrawUILine(new Color(0.6f, 0.6f, 0.6f, 0.4f));

            EditorGUILayout.PropertyField(sp_SegmentsPivotOffset, true);
            EditorGUILayout.PropertyField(sp_DistancesMul, true);

            FEditor_Styles.DrawUILine(new Color(0.6f, 0.6f, 0.6f, 0.4f));

            GUILayout.Space(3f);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(sp_ManualAffects, true);
            GUILayout.Space(5f);
            EditorGUI.indentLevel--;

            EditorGUIUtility.labelWidth = 0f;
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();

        #endregion


        #region Debug Options

        EditorGUILayout.BeginVertical(FEditor_Styles.LGrayBackground);
        drawDebug = EditorGUILayout.Foldout(drawDebug, "Debugging", true);

        if (drawDebug)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginHorizontal(FEditor_Styles.LBlueBackground);
            EditorGUILayout.HelpBox("When 'DrawDebug' is toggled, you can use button '~' to instantly deactivate SpineAnimator's motion for time you hold this button (not on build)", MessageType.None);
            GUILayout.EndHorizontal();

            GUILayout.Space(5f);
            GUILayout.Space(5f);

            EditorGUIUtility.labelWidth = 0f;
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();

        #endregion


        EditorGUILayout.EndVertical();
        EditorGUI.indentLevel--;

        DrawPhysicalOptionsTab(Get);

    }


    #region Physical Experimental Stuff

    protected static bool drawCollisionParams = false;

    protected virtual void DrawPhysicalOptionsTab(FSpineAnimator spine)
    {
        EditorGUIUtility.labelWidth = 130;

        EditorGUILayout.BeginVertical(FEditor_Styles.Style(new Color(0.9f, 0.9f, 0.9f, 0.15f)));

        EditorGUI.indentLevel++;

        GUILayout.BeginHorizontal(FEditor_Styles.LGrayBackground);
        drawCollisionParams = EditorGUILayout.Foldout(drawCollisionParams, "Collisions (Experimental)", true);
        GUILayout.EndHorizontal();

        if (drawCollisionParams)
        {
            GUILayout.Space(3f);
            DrawPhysicsStuff(spine);
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        EditorGUIUtility.labelWidth = 0;
    }

    private void DrawPhysicsStuff(FSpineAnimator spine)
    {
        EditorGUIUtility.labelWidth = 140;

        EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);

        EditorGUILayout.PropertyField(sp_UseCollisions);

        if (spine.UseCollisions)
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Collision support is experimental and not working fully correct yet.", MessageType.Info);

                if (spine.IncludedColliders.Count == 0)
                    EditorGUILayout.BeginVertical(FEditor_Styles.RedBackground);
                else
                    EditorGUILayout.BeginVertical(FEditor_Styles.Emerald);

                Color c = GUI.color;
                GUILayout.BeginVertical();
                if (ActiveEditorTracker.sharedTracker.isLocked) GUI.color = new Color(0.44f, 0.44f, 0.44f, 0.8f); else GUI.color = new Color(0.95f, 0.95f, 0.99f, 0.9f);
                if (GUILayout.Button(new GUIContent("Lock Inspector for Drag & Drop Colliders", "Drag & drop colliders to 'Included Colliders' List from the hierarchy"), EditorStyles.toolbarButton)) { GameObject act = Get.gameObject;  ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked; Selection.activeObject = act; }
                GUI.color = c;
                GUILayout.EndVertical();

                EditorGUILayout.PropertyField(sp_IncludedColliders, true);
                EditorGUILayout.EndVertical();

                FEditor_Styles.DrawUILine(new Color(0.5f, 0.5f, 0.5f, 0.6f));

                EditorGUILayout.PropertyField(sp_CollidersScaleMul, new GUIContent("Scale Multiplier"));
                EditorGUILayout.PropertyField(sp_CollidersScale, new GUIContent("Scale Curve"));
                EditorGUILayout.PropertyField(sp_CollidersAutoCurve, new GUIContent("Auto Curve"));
                FEditor_Styles.DrawUILine(new Color(0.6f, 0.6f, 0.6f, 0.4f));
                EditorGUILayout.PropertyField(sp_AllCollidersOffset, true);

                if (spine.UseTruePosition)
                    EditorGUILayout.BeginVertical(FEditor_Styles.RedBackground);
                else
                    EditorGUILayout.BeginVertical(FEditor_Styles.YellowBackground);

                EditorGUILayout.PropertyField(sp_UseTruePosition, true);
                EditorGUILayout.EndVertical();

                Color preCol = GUI.color;
                if (spine.GravityPower != Vector3.zero) if (!spine.DetailedCollision) GUI.color = new Color(1f, 1f, 0.35f, 0.8f);
                GUI.color = preCol;

                EditorGUILayout.PropertyField(sp_DetailedCollision, true);


                FEditor_Styles.DrawUILine(new Color(0.6f, 0.6f, 0.6f, 0.4f));
                EditorGUILayout.PropertyField(sp_GravityPower, true);

                GUILayout.Space(3f);
            }
            else // In Playmode
            {
                EditorGUILayout.BeginVertical(FEditor_Styles.Emerald);

                Color c = GUI.color;
                GUILayout.BeginVertical();
                if (ActiveEditorTracker.sharedTracker.isLocked) GUI.color = new Color(0.44f, 0.44f, 0.44f, 0.8f); else GUI.color = new Color(0.95f, 0.95f, 0.99f, 0.9f);
                if (GUILayout.Button(new GUIContent("Lock Inspector for Drag & Drop Colliders", "Drag & drop colliders to 'Included Colliders' List from the hierarchy"), EditorStyles.toolbarButton)) ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
                GUI.color = c;
                GUILayout.EndVertical();

                EditorGUILayout.PropertyField(sp_IncludedColliders, true);
                EditorGUILayout.EndVertical();
                // EditorGUI.indentLevel--;

                EditorGUILayout.HelpBox("Rescalling in playmode available only in editor not in build", MessageType.Warning);
                EditorGUILayout.PropertyField(sp_CollidersScaleMul, new GUIContent("Scale Multiplier"));
                EditorGUILayout.PropertyField(sp_CollidersScale, new GUIContent("Scale Curve"));
                EditorGUILayout.PropertyField(sp_CollidersAutoCurve, new GUIContent("Auto Curve"));
                FEditor_Styles.DrawUILine(new Color(0.6f, 0.6f, 0.6f, 0.4f));
                EditorGUILayout.PropertyField(sp_AllCollidersOffset, true);

                if (spine.UseTruePosition)
                    EditorGUILayout.BeginVertical(FEditor_Styles.RedBackground);
                else
                    EditorGUILayout.BeginVertical(FEditor_Styles.YellowBackground);

                EditorGUILayout.PropertyField(sp_UseTruePosition, true);
                EditorGUILayout.EndVertical();

                Color preCol = GUI.color;
                if (spine.GravityPower != Vector3.zero) if (!spine.DetailedCollision) GUI.color = new Color(1f, 1f, 0.35f, 0.8f);
                GUI.color = preCol;

                EditorGUILayout.PropertyField(sp_DetailedCollision, true);

                FEditor_Styles.DrawUILine(new Color(0.6f, 0.6f, 0.6f, 0.4f));
                EditorGUILayout.PropertyField(sp_GravityPower, true);

                GUILayout.Space(3f);
            }
        }
        else
        {
            FEditor_Styles.DrawUILine(new Color(0.6f, 0.6f, 0.6f, 0.4f));
            EditorGUILayout.PropertyField(sp_GravityPower, true);
            GUILayout.Space(3f);
        }

        EditorGUILayout.EndVertical();
    }

    #endregion


}