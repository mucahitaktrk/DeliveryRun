using FIMSpace.FEditor;
using System;
using UnityEditor;
using UnityEngine;

public partial class FSpineAnimator_Editor
{

    private void Tab_DrawSetup()
    {
        FGUI_Inspector.VSpace(-2, -4);
        GUILayout.BeginVertical(FGUI_Resources.ViewBoxStyle);

        if (Get.SpineBones == null) Get.SpineBones = new System.Collections.Generic.List<FIMSpace.FSpine.FSpineAnimator.SpineBone>();

        El_BaseTransformsSetup();

        GUILayout.Space(2f);

        GUILayout.BeginVertical(FGUI_Resources.BGInBoxLightStyle);
        GUILayout.Space(2f);
        EditorGUIUtility.labelWidth = 182f;
        EditorGUILayout.PropertyField(sp_forw, new GUIContent("Base Transform (Optional)", sp_forw.tooltip));
        EditorGUIUtility.labelWidth = 0f;
        GUILayout.Space(2f);
        GUILayout.EndVertical();

        GUILayout.Space(3f);
        GUILayout.BeginVertical(FGUI_Resources.BGInBoxBlankStyle);
        Fold_DrawSpineTransforms();
        GUILayout.EndVertical();

        GUILayout.BeginVertical(FGUI_Resources.BGInBoxLightStyle);
        GUILayout.Space(6f);
        GUILayout.BeginHorizontal();

        EditorGUIUtility.labelWidth = 135f;
        EditorGUILayout.PropertyField(sp_LastBoneLeading, new GUIContent(Get.LastBoneLeading ? sp_LastBoneLeading.displayName : "Start Bone Is Head", sp_LastBoneLeading.tooltip));
        if (GUILayout.Button(new GUIContent(Get.LastBoneLeading ? _TexLeadingRIcon : _TexLeadingLIcon), EditorStyles.label, new GUILayoutOption[] { GUILayout.Height(24), GUILayout.Width(96) })) Get.LastBoneLeading = !Get.LastBoneLeading;
        GUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(sp_ReverseForward);
        GUILayout.Space(5f);
        EditorGUIUtility.labelWidth = 0f;

        GUILayout.EndVertical();
        //GUILayout.Space(3f);

        //GUILayout.Space(1f);
        //GUILayout.BeginVertical(FGUI_Resources.BGInBoxBlankStyle);
        //El_DrawAnimatorSetup();
        //GUILayout.EndVertical();

        Fold_DrawAdditionalSetup();

        GUILayout.EndVertical();
    }


    private void Tab_DrawPreSetup()
    {
        FGUI_Inspector.VSpace(-2, -4);
        GUILayout.BeginVertical(FGUI_Resources.ViewBoxStyle);

        GUILayout.Space(7f);
        GUI.color = new Color(1f, 1f, 1f, 0.7f); EditorGUILayout.LabelField(Lang("Select right bones of your model"), FGUI_Resources.HeaderStyle); GUI.color = c;

        EditorGUIUtility.labelWidth = 42f;
        if (startBone == null) TryFindStartBone();
        if (endBone == null) TryFindEndBone();

        if (startBone != null && endBone != null)
        {
            if (!IsChildOf(endBone, startBone))
            {
                EditorGUILayout.HelpBox("! '" + startBone.name + "' is not child of '" + endBone.name + "' !", MessageType.Error);
            }
        }

        GUILayout.Space(12f);

        // START CHAIN BONE
        EditorGUILayout.BeginHorizontal();
        startBone = (Transform)EditorGUILayout.ObjectField(new GUIContent("Start", "Put here first bone in hierarchy depth for automatically get chain of bones to end one"), startBone, typeof(Transform), true);

        // Select bone button
        if (largestSkin) El_DrawBoneSelectionButton(true, ref Get._gizmosEditorStartPreview, Get.GetBaseTransform());

        // Go left / right in hierarchy icon
        if (startBone != null)
        {
            if (GUILayout.Button(new GUIContent("◄", "Get Parent Bone Of Current Selected"), FGUI_Resources.ButtonStyle, new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(16) }))
            { startBone = startBone.parent; EditorGUIUtility.PingObject(startBone); }

            if (startBone)
                if (startBone.childCount > 0)
                    if (GUILayout.Button(new GUIContent("►", "Get Child Bone Of Current Selected"), FGUI_Resources.ButtonStyle, new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(16) }))
                    { startBone = startBone.GetChild(0); EditorGUIUtility.PingObject(startBone); }
        }

        GUI.enabled = false; EditorGUILayout.LabelField(new GUIContent("(Pelvis + legs)", "Start bone should be pelvis bone which's children are legs"), GUILayout.Width(91)); GUI.enabled = true;

        EditorGUILayout.EndHorizontal();


        // MIDDLE SPINE CHAIN ICON
        GUILayout.Space(7f);
        if (GUILayout.Button(new GUIContent(_TexSpineWideIcon, skins.Count > 0 ? "Click to toggle meshes visibility" : ""), FGUI_Resources.HeaderStyle, GUILayout.Height(24)))
        {
            hideSkin = !hideSkin;

            // Switching mesh visibility if can
            if (hideSkin)
                for (int i = 0; i < skins.Count; i++)
                    skins[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            else
                for (int i = 0; i < skins.Count; i++)
                    skins[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
        //EditorGUILayout.LabelField(new GUIContent(_TexSpineWideIcon), FGUI_Resources.HeaderStyle, GUILayout.Height(24));
        GUILayout.Space(10f);


        // END CHAIN BONE
        EditorGUILayout.BeginHorizontal();
        endBone = (Transform)EditorGUILayout.ObjectField(new GUIContent("End", "Put here last bone in hierarchy depth for automatically get chain of bones from start one"), endBone, typeof(Transform), true);

        // Select bone button
        if (largestSkin) El_DrawBoneSelectionButton(false, ref Get._gizmosEditorEndPreview, startBone);

        // Go left / right in hierarchy icon
        if (endBone != null)
        {
            if (startBone != null)
                if (endBone.childCount > 0)
                {
                    if (GUILayout.Button(new GUIContent("L"), FGUI_Resources.ButtonStyle, new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(16) }))
                    { endBone = GetLastChild(startBone); EditorGUIUtility.PingObject(endBone); }
                }


            if (GUILayout.Button(new GUIContent("◄", "Get Parent Bone Of Current Selected"), FGUI_Resources.ButtonStyle, new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(16) }))
            { endBone = endBone.parent; EditorGUIUtility.PingObject(endBone); }


            if (endBone)
                if (endBone.childCount > 0)
                    if (GUILayout.Button(new GUIContent("►", "Get Child Bone Of Current Selected"), FGUI_Resources.ButtonStyle, new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(16) }))
                    { endBone = endBone.GetChild(0); EditorGUIUtility.PingObject(endBone); }
        }
        else
        {
            if (startBone != null)
            {
                if (GUILayout.Button(new GUIContent("L"), FGUI_Resources.ButtonStyle, new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(16) }))
                { endBone = GetLastChild(startBone); EditorGUIUtility.PingObject(endBone); }
            }
        }

        GUI.enabled = false; EditorGUILayout.LabelField(new GUIContent("(Neck/Head/Chest)", "End bone should be head/neck/chest bone of your model - depends what motion you want to achieve"), GUILayout.Width(120)); GUI.enabled = true;

        EditorGUILayout.EndHorizontal();




        GUILayout.Space(12f);

        if (startBone == null || endBone == null) GUI.enabled = false;

        if (GUILayout.Button(new GUIContent(Lang("Create Spine Chain") + " (Get Bones)")))
        {
            if (!IsChildOf(endBone, startBone))
            {
                Debug.LogError("! '" + startBone.name + "' is not child of '" + endBone.name + "' !");
            }
            else
            {
                Get.CreateSpineChain(Get._gizmosEditorStartPreview, Get._gizmosEditorEndPreview);
                EditorUtility.SetDirty(target);
            }

            if (Application.isPlaying) Get.Init();
        }

        GUI.enabled = true;

        GUILayout.Space(3f);
        EditorGUILayout.HelpBox(Lang("Lang_SpineCreateInfo"), MessageType.Info);
        //EditorGUILayout.HelpBox("After creating spine chain you will unlock rest of the parameters (You will be able to adjust it again after that)", MessageType.Info);

        GUILayout.EndVertical();
    }

    private Transform GetLastChild(Transform startBone)
    {
        if (startBone == null) return null;

        Transform child = startBone;

        while (child.childCount > 0)
        {
            child = child.GetChild(0);
        }

        return child;
    }

    void El_DrawBoneSelectionButton(bool startOrEnd, ref Transform target, Transform childOf)
    {
        if (GUILayout.Button(new GUIContent(FGUI_Resources.TexTargetingIcon, "Display bones from your renderer to choose"), FGUI_Resources.ButtonStyle, new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(16) }))
        {
            GenericMenu bonesMenu = new GenericMenu();

            for (int i = 0; i < largestSkin.bones.Length; i++)
            {
                GUIContent title = new GUIContent(largestSkin.bones[i].name); Transform tgt = largestSkin.bones[i];

                bool current = false;
                if (startOrEnd) { if (tgt == startBone) current = true; }
                else { if (tgt == endBone) current = true; }

                if (childOf != null) if (!IsChildOf(tgt, childOf)) continue;
                bonesMenu.AddItem(title, current, () => { SelectSpineChainBone(tgt, startOrEnd); });
            }

            bonesMenu.ShowAsContext();
        }
    }

    bool IsChildOf(Transform child, Transform parent)
    {
        Transform p = child.parent;
        while (p != null)
        {
            if (p == parent) return true;
            p = p.parent;
        }

        return false;
    }

    void SelectSpineChainBone(Transform t, bool startBone)
    {
        EditorGUIUtility.PingObject(t);
        if (startBone) this.startBone = t;
        else
            endBone = t;
    }

    private void Tab_DrawTweaking()
    {
        FGUI_Inspector.VSpace(-2, -4);
        GUILayout.BeginVertical(FGUI_Resources.ViewBoxStyle);

        GUILayout.Space(7f);
        EditorGUIUtility.labelWidth = 160f;
        EditorGUILayout.PropertyField(sp_SpineAnimatorAmount); EditorGUIUtility.labelWidth = 0f;
        GUILayout.Space(4f);
        GUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);
        GUILayout.Space(2f);
        EditorGUILayout.BeginHorizontal();

        if (Get._Editor_PivotoffsetXYZ)
        {
            EditorGUILayout.PropertyField(sp_MainPivotOffset);
            if (GUILayout.Button(new GUIContent("Z", "If you need only Z axis for pivot adjustement"), FGUI_Resources.ButtonStyle, new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(16) })) Get._Editor_PivotoffsetXYZ = !Get._Editor_PivotoffsetXYZ;
        }
        else
        {
            EditorGUI.BeginChangeCheck();
            Get.MainPivotOffset.z = EditorGUILayout.FloatField(new GUIContent(sp_MainPivotOffset.displayName, sp_MainPivotOffset.tooltip), Get.MainPivotOffset.z);
            Get.MainPivotOffset.x = 0f; Get.MainPivotOffset.y = 0f;
            if (EditorGUI.EndChangeCheck()) { serializedObject.ApplyModifiedProperties(); Get.UpdatePivotOffsetState(); }
            if (GUILayout.Button(new GUIContent("XYZ", "If you want to adjust pivot offset with all axes - x,y,z"), FGUI_Resources.ButtonStyle, new GUILayoutOption[2] { GUILayout.Width(36), GUILayout.Height(16) })) Get._Editor_PivotoffsetXYZ = !Get._Editor_PivotoffsetXYZ;
        }

        EditorGUILayout.EndHorizontal();
#if UNITY_2018_4_OR_NEWER
        if (Application.isPlaying == false)
        {
            // Not in islotated scene mode
            if (Get.transform.gameObject.scene.rootCount != 1)
            {
                SerializedProperty p = serializedObject.FindProperty("mainPivotOffsetTransform");
                if (p.objectReferenceValue == null)
                {
                    GameObject prefabed;
                    prefabed = PrefabUtility.GetNearestPrefabInstanceRoot(Get);
                    if (PrefabUtility.GetPrefabAssetType(Get) == PrefabAssetType.Model) prefabed = null;
                    if (prefabed != null) EditorGUILayout.HelpBox("Pivot offset can be generated only inside prefab mode!", MessageType.None);
                }
            }
        }
#endif

        GUILayout.Space(3f);
        GUILayout.EndVertical();

        El_DrawBending();
        El_DrawStraigtening();
        El_DrawSmoothing();

        GUILayout.Space(-5f);

        GUILayout.EndVertical();
    }


    private void Tab_DrawCorrections()
    {
        FGUI_Inspector.VSpace(-2, -4);

        GUILayout.BeginVertical(FGUI_Resources.ViewBoxStyle);

        GUILayout.Space(5f); EditorGUIUtility.labelWidth = 154f;
        EditorGUILayout.PropertyField(sp_DistancesMul);
        GUILayout.Space(5f); EditorGUIUtility.labelWidth = 0f;

        El_DrawFullChainCorrections();

        El_DrawAnchoring();

        //El_DrawRareCorrections();

        GUILayout.Space(-4f);
        GUILayout.EndVertical();
    }

    bool drawColl = true;
    bool drawCollSetup = true;
    bool drawInclud = true;
    private void Tab_DrawPhysics()
    {
        FGUI_Inspector.VSpace(-2, -4);

        GUILayout.BeginVertical(FGUI_Resources.ViewBoxStyle);


        // Gravity power
        GUILayout.Space(7f);
        EditorGUILayout.PropertyField(sp_GravityPower, true);
        GUILayout.Space(5f);


        // Physics switch button
        GUILayout.BeginVertical(FGUI_Resources.BGInBoxLightStyle);
        GUILayout.Space(2f);

        // Tab to switch collisions
        GUILayout.BeginHorizontal();

        if (Get.UseCollisions)
        {
            if (GUILayout.Button(new GUIContent(" " + FGUI_Resources.GetFoldSimbol(drawColl, 8, "►") + "  " + Lang("Use Collisions") + " (Experimental)", FGUI_Resources.Tex_Collider, sp_UseCollisions.tooltip), FGUI_Resources.FoldStyle, GUILayout.Height(21))) drawColl = !drawColl;
        }
        else
            if (GUILayout.Button(new GUIContent("  " + Lang("Use Collisions") + " (Experimental)", FGUI_Resources.Tex_Collider, sp_UseCollisions.tooltip), FGUI_Resources.FoldStyle, GUILayout.Height(21))) { Get.UseCollisions = !Get.UseCollisions; serializedObject.ApplyModifiedProperties(); }

        GUILayout.FlexibleSpace();
        EditorGUILayout.PropertyField(sp_UseCollisions, new GUIContent("", sp_UseCollisions.tooltip), GUILayout.Width(22));

        GUILayout.EndHorizontal();


        // 
        if (Get.UseCollisions && drawColl)
        {
            GUILayout.Space(5f);
            GUI.color = new Color(0.85f, 1f, 0.85f, 1f);
            EditorGUILayout.BeginHorizontal(FGUI_Resources.HeaderBoxStyleH);
            string f = FGUI_Resources.GetFoldSimbol(drawInclud); int iconSize = 24; int inclC = Get.IncludedColliders.Count;
            GUI.color = c;

            GUILayout.Label(new GUIContent(" "), GUILayout.Width(1));
            if (GUILayout.Button(new GUIContent(" " + f + "  " + Lang("Collide With") + " (" + (inclC == 0 ? "0 !!!)" : inclC + ")"), FGUI_Resources.TexBehaviourIcon), FGUI_Resources.FoldStyle, GUILayout.Height(24))) drawInclud = !drawInclud;

            if (drawInclud)
            {
                if (GUILayout.Button("+", new GUILayoutOption[2] { GUILayout.MaxWidth(24), GUILayout.MaxHeight(22) }))
                {
                    Get.IncludedColliders.Add(null);
                    serializedObject.Update();
                    serializedObject.ApplyModifiedProperties();
                }
            }

            EditorGUILayout.EndHorizontal();

            if (drawInclud)
            {
                FGUI_Inspector.VSpace(-3, -5);
                GUI.color = new Color(0.6f, .9f, 0.6f, 1f);
                EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyleH);
                GUI.color = c;
                GUILayout.Space(5f);


                // Drawing colliders from list
                if (Get.IncludedColliders.Count == 0)
                {
                    EditorGUILayout.LabelField("Please add here colliders", FGUI_Resources.HeaderStyle);
                    GUILayout.Space(2f);
                }
                else
                {

                    EditorGUI.BeginChangeCheck();
                    for (int i = 0; i < Get.IncludedColliders.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        Get.IncludedColliders[i] = (Collider)EditorGUILayout.ObjectField(Get.IncludedColliders[i], typeof(Collider), true);
                        if (GUILayout.Button("X", new GUILayoutOption[2] { GUILayout.MaxWidth(22), GUILayout.MaxHeight(16) }))
                        {
                            Get.IncludedColliders.RemoveAt(i);
                            serializedObject.Update();
                            serializedObject.ApplyModifiedProperties();
                            return;
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        Get.CheckForColliderDuplicates();
                        serializedObject.Update();
                        serializedObject.ApplyModifiedProperties();
                    }
                }

                GUILayout.Space(6f);

                // Lock button
                GUILayout.BeginVertical();
                if (ActiveEditorTracker.sharedTracker.isLocked) GUI.color = new Color(0.44f, 0.44f, 0.44f, 0.8f); else GUI.color = new Color(0.95f, 0.95f, 0.99f, 0.9f);
                if (GUILayout.Button(new GUIContent("Lock Inspector for Drag & Drop Colliders", "Drag & drop colliders to 'Included Colliders' List from the hierarchy"), FGUI_Resources.ButtonStyle, GUILayout.Height(18))) ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
                GUI.color = c;
                GUILayout.EndVertical();

                // Drag and drop box
                El_DrawDragAndDropCollidersBox();

                GUILayout.Space(3f);
                EditorGUILayout.EndVertical();
            }


            GUILayout.Space(5f);

            f = FGUI_Resources.GetFoldSimbol(drawCollSetup);
            GUI.color = new Color(1f, .85f, .85f, 1f);
            EditorGUILayout.BeginHorizontal(FGUI_Resources.HeaderBoxStyleH); iconSize = 22;
            GUI.color = c;
            GUILayout.Label(new GUIContent(" "), GUILayout.Width(1));
            if (GUILayout.Button(FGUI_Resources.Tex_GearSetup, EditorStyles.label, new GUILayoutOption[2] { GUILayout.Width(iconSize), GUILayout.Height(iconSize) })) { drawCollSetup = !drawCollSetup; }
            if (GUILayout.Button(f + "     " + "Colliders Setup" + "     " + f, FGUI_Resources.HeaderStyle, GUILayout.Height(24))) drawCollSetup = !drawCollSetup;
            if (GUILayout.Button(FGUI_Resources.Tex_GearSetup, EditorStyles.label, new GUILayoutOption[2] { GUILayout.Width(iconSize), GUILayout.Height(iconSize) })) { drawCollSetup = !drawCollSetup; }
            GUILayout.Label(new GUIContent(" "), GUILayout.Width(1));
            EditorGUILayout.EndHorizontal();

            if (drawCollSetup)
            {
                FGUI_Inspector.VSpace(-3, -5);
                GUI.color = new Color(1f, .55f, .55f, 1f);
                EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyleH);
                GUI.color = c;
                GUILayout.Space(6f);
                EditorGUILayout.PropertyField(sp_CollidersScaleMul, new GUIContent("Scale Multiplier"));
                EditorGUILayout.PropertyField(sp_CollidersScale, new GUIContent("Scale Curve"));
                EditorGUILayout.PropertyField(sp_CollidersAutoCurve, new GUIContent("Auto Curve"));
                EditorGUILayout.PropertyField(sp_AllCollidersOffset, true);
                GUILayout.Space(6f);


                El_DrawSelectiveCollision();

                GUILayout.Space(3f);
                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(5f);

            if (Get.UseTruePosition) GUI.color = new Color(1f, 0.7f, 0.7f, 1f);
            else GUI.color = new Color(1f, 1f, 0.7f, 1f);
            GUILayout.BeginVertical(FGUI_Resources.BGInBoxLightStyle);
            GUILayout.Space(2f); EditorGUILayout.PropertyField(sp_UseTruePosition, true); GUILayout.Space(2f);
            GUILayout.EndVertical(); GUI.color = c;

            GUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);
            GUILayout.Space(2f); EditorGUILayout.PropertyField(sp_DetailedCollision, true); GUILayout.Space(2f);
            GUILayout.EndVertical();
            GUILayout.Space(2f);
        }


        GUILayout.Space(2f);
        GUILayout.EndVertical();
        GUILayout.Space(-5f);
        GUILayout.EndVertical();
    }



}