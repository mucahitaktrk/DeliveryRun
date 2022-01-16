using FIMSpace.FEditor;
using UnityEditor;
using UnityEngine;

public partial class FSpineAnimator_Editor
{
    private bool drawHeaderFoldout = false;
    private void HeaderBoxMain(string title, ref bool drawGizmos, ref bool defaultInspector, Texture2D scrIcon, MonoBehaviour target, int height = 22)
    {
        EditorGUILayout.BeginVertical(FGUI_Resources.HeaderBoxStyle);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button(new GUIContent(scrIcon), EditorStyles.label, new GUILayoutOption[2] { GUILayout.Width(height - 2), GUILayout.Height(height - 2) }))
        {
            MonoScript script = MonoScript.FromMonoBehaviour(target);
            if (script) EditorGUIUtility.PingObject(script);
            drawHeaderFoldout = !drawHeaderFoldout;
        }

        if (GUILayout.Button(title, FGUI_Resources.GetTextStyle(14, true, TextAnchor.MiddleLeft), GUILayout.Height(height)))
        {
            MonoScript script = MonoScript.FromMonoBehaviour(target);
            if (script) EditorGUIUtility.PingObject(script);
            drawHeaderFoldout = !drawHeaderFoldout;
        }

        if (EditorGUIUtility.currentViewWidth > 326)
            // Youtube channel button
            if (GUILayout.Button(new GUIContent(FGUI_Resources.Tex_Tutorials, "Open FImpossible Creations Channel with tutorial videos in your web browser"), FGUI_Resources.ButtonStyle, new GUILayoutOption[2] { GUILayout.Width(height), GUILayout.Height(height) }))
            {
                Application.OpenURL("https://www.youtube.com/c/FImpossibleCreations");
            }

        if (EditorGUIUtility.currentViewWidth > 292)
            // Store site button
            if (GUILayout.Button(new GUIContent(FGUI_Resources.Tex_Website, "Open FImpossible Creations Asset Store Page inside your web browser"), FGUI_Resources.ButtonStyle, new GUILayoutOption[2] { GUILayout.Width(height), GUILayout.Height(height) }))
            {
                Application.OpenURL("https://assetstore.unity.com/publishers/37262");
            }

        // Manual file button
        if (_manualFile == null) _manualFile = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(target))) + "/Spine Animator User Manual.pdf");
        if (_manualFile)
            if (GUILayout.Button(new GUIContent(FGUI_Resources.Tex_Manual, "Open .PDF user manual file for Spine Animator"), FGUI_Resources.ButtonStyle, new GUILayoutOption[2] { GUILayout.Width(height), GUILayout.Height(height) }))
            {
                EditorGUIUtility.PingObject(_manualFile);
                Application.OpenURL(Application.dataPath + "/" + AssetDatabase.GetAssetPath(_manualFile).Replace("Assets/", ""));
            }

        FGUI_Inspector.DrawSwitchButton(ref drawGizmos, FGUI_Resources.Tex_GizmosOff, FGUI_Resources.Tex_Gizmos, "Toggle drawing gizmos on character in scene window", height, height, true);
        FGUI_Inspector.DrawSwitchButton(ref drawHeaderFoldout, FGUI_Resources.Tex_LeftFold, FGUI_Resources.Tex_DownFold, "Toggle to view additional options for foldouts", height, height);

        EditorGUILayout.EndHorizontal();

        if (drawHeaderFoldout)
        {
            FGUI_Inspector.DrawUILine(0.07f, 0.1f, 1, 4, 0.99f);

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            choosedLang = (ELangs)EditorGUILayout.EnumPopup(choosedLang, new GUIStyle(EditorStyles.layerMaskField) { fixedHeight = 0 }, new GUILayoutOption[2] { GUILayout.Width(80), GUILayout.Height(22) });
            if (EditorGUI.EndChangeCheck())
            {
                PlayerPrefs.SetInt("FimposLang", (int)choosedLang);
                SetupLangs();
            }

            GUILayout.FlexibleSpace();

            bool hierSwitchOn = PlayerPrefs.GetInt("AnimsH", 1) == 1;
            FGUI_Inspector.DrawSwitchButton(ref hierSwitchOn, FGUI_Resources.Tex_HierSwitch, null, "Switch drawing small icons in hierarchy", height, height, true);
            PlayerPrefs.SetInt("AnimsH", hierSwitchOn ? 1 : 0);

            if (GUILayout.Button(new GUIContent(FGUI_Resources.Tex_Rename, "Change component title to yours (current: '" + Get._editor_Title + "'"), FGUI_Resources.ButtonStyle, new GUILayoutOption[2] { GUILayout.Width(height), GUILayout.Height(height) }))
            {
                string filename = EditorUtility.SaveFilePanelInProject("Type your title (no file will be created)", Get._editor_Title, "", "Type your title (no file will be created)");
                if (!string.IsNullOrEmpty(filename))
                {
                    filename = System.IO.Path.GetFileNameWithoutExtension(filename);
                    if (!string.IsNullOrEmpty(filename))
                    { Get._editor_Title = filename; serializedObject.ApplyModifiedProperties(); }
                }
            }

            // Default inspector switch
            FGUI_Inspector.DrawSwitchButton(ref drawNewInspector, FGUI_Resources.Tex_AB, null, "Switch GUI Style to old / new", height, height, true);
            if (!drawNewInspector && drawDefaultInspector) drawDefaultInspector = false;

            // Old new UI Button
            FGUI_Inspector.DrawSwitchButton(ref defaultInspector, FGUI_Resources.Tex_Default, null, "Toggle inspector view to default inspector.\n\nIf you ever edit source code of Look Animator and add custom variables, you can see them by entering this mode, also sometimes there can be additional/experimental variables to play with.", height, height);
            if (!drawNewInspector && drawDefaultInspector) drawNewInspector = false;

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }


    void El_BaseTransformsSetup()
    {
        GUILayout.Space(4);
        EditorGUILayout.BeginHorizontal();

        if (startBone == null) startBone = Get.SpineBones[0].transform;
        if (endBone == null) startBone = Get.SpineBones[Get.SpineBones.Count - 1].transform;

        EditorGUIUtility.labelWidth = 42f;
        GUIStyle style = Get.LastBoneLeading ? EditorStyles.label : EditorStyles.boldLabel;
        EditorGUILayout.LabelField(new GUIContent("Start", "Base parent bone of whole spine chain"), style, GUILayout.Width(38));
        Get._gizmosEditorStartPreview = (Transform)EditorGUILayout.ObjectField(Get._gizmosEditorStartPreview, typeof(Transform), true);

        style = Get.LastBoneLeading ? EditorStyles.boldLabel : EditorStyles.label;
        EditorGUILayout.LabelField(new GUIContent("End", "Last parent bone of whole spine chain"), style, GUILayout.Width(35));
        Get._gizmosEditorEndPreview = (Transform)EditorGUILayout.ObjectField(Get._gizmosEditorEndPreview, typeof(Transform), true);
        EditorGUIUtility.labelWidth = 0f;

        bool canRefresh = true;
        if (Get.SpineBones.Count > 1)
            if (Get._gizmosEditorStartPreview == Get.SpineBones[0].transform)
                if (Get._gizmosEditorEndPreview == Get.SpineBones[Get.SpineBones.Count - 1].transform)
                    canRefresh = false;

        if (canRefresh) GUI.color = new Color(0.1f, 1f, 0.1f, 0.95f); else GUI.enabled = false;

        if (GUILayout.Button(new GUIContent("Get"), new GUILayoutOption[2] { GUILayout.MaxWidth(36), GUILayout.MaxHeight(14) }))
        {
            Get.CreateSpineChain(Get._gizmosEditorStartPreview, Get._gizmosEditorEndPreview);
        }

        GUI.enabled = true;
        GUI.color = c;

        EditorGUILayout.EndHorizontal();

    }



    bool drawBending = true;
    void El_DrawBending()
    {
        GUILayout.BeginVertical(FGUI_Resources.BGInBoxLightStyle);

        if (GUILayout.Button(new GUIContent(" " + FGUI_Resources.GetFoldSimbol(drawBending, 8, "►") + "  " + Lang("Limiting Angles"), FGUI_Resources.Tex_Knob), FGUI_Resources.FoldStyle, GUILayout.Height(21))) drawBending = !drawBending;

        if (drawBending)
        {
            GUILayout.Space(3f);
            //GUI.color = new Color(1f, 1f, 1f, 0.85f); EditorGUILayout.LabelField("Bending", FGUI_Resources.HeaderStyle); GUI.color = c;

            EditorGUILayout.PropertyField(sp_AngleLimit);
            if (Get.AngleLimit < 91) EditorGUILayout.PropertyField(sp_LimitingAngleSmoother);
            GUILayout.Space(3f);
            EditorGUILayout.PropertyField(sp_Slithery);
            GUILayout.Space(3f);
        }

        GUILayout.EndVertical();
    }

    bool drawStraigtening = true;
    void El_DrawStraigtening()
    {
        GUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);

        if (GUILayout.Button(new GUIContent(" " + FGUI_Resources.GetFoldSimbol(drawStraigtening, 8, "►") + "  " + Lang("Straigtening Rules"), FGUI_Resources.Tex_Limits), FGUI_Resources.FoldStyle, GUILayout.Height(21))) drawStraigtening = !drawStraigtening;

        if (drawStraigtening)
        {
            GUILayout.Space(3f);

            EditorGUILayout.PropertyField(sp_StraighteningSpeed/*, new GUIContent("When Moving", sp_StraighteningSpeed.tooltip)*/, true);

            if (Get.StraightenSpeed > 0f)
            {
                EditorGUILayout.PropertyField(sp_TurboStraighten, true);
            }

            GUILayout.Space(3f);
            if (!Get.LastBoneLeading) if (Get.Springiness > 0) if (Get.GoBackSpeed <= 0) GUI.color = new Color(0.55f, 0.9f, 1f, 1f);
            EditorGUILayout.PropertyField(sp_GoBackSpeed/*, new GUIContent("Constantly", sp_GoBackSpeed.tooltip)*/, true); GUI.color = c;

            //GUILayout.Space(3f);
            GUILayout.Space(3f);
        }

        GUILayout.EndVertical();
    }

    bool drawSmoothing = true;
    void El_DrawSmoothing()
    {
        GUILayout.BeginVertical(FGUI_Resources.BGInBoxLightStyle);

        if (GUILayout.Button(new GUIContent(" " + FGUI_Resources.GetFoldSimbol(drawSmoothing, 8, "►") + "   " + Lang("Smoothing Motion"), FGUI_Resources.TexSmallOptimizeIcon), FGUI_Resources.FoldStyle, GUILayout.Height(21))) drawSmoothing = !drawSmoothing;

        if (drawSmoothing)
        {
            GUILayout.Space(6f);
            EditorGUILayout.PropertyField(sp_PositionsSmoother);

            if (Get.PosSmoother > 0f)
            {
                if (!Get.LastBoneLeading) if (Get.Springiness > 0) if (Get.MaxStretching > 0.4f) GUI.color = new Color(0.75f, 0.9f, 1f, 0.95f);
                EditorGUILayout.PropertyField(sp_MaxStretching);
            }

            GUILayout.Space(4f);
            EditorGUILayout.PropertyField(sp_RotationsSmoother); GUILayout.Space(1f);
            if ( Get.RotSmoother > 0f) EditorGUILayout.PropertyField(sp_MotionInfluence);

            GUI.color = c;
            GUILayout.Space(3f);
        }

        GUILayout.EndVertical();

        if (!Get.LastBoneLeading)
        {
            GUILayout.Space(3f);
            GUILayout.BeginVertical(FGUI_Resources.BGInBoxBlankStyle);

            EditorGUILayout.PropertyField(sp_Springiness, true);
            GUILayout.EndVertical();
            GUILayout.Space(3f);
        }
    }

    private void El_DrawOptimizeWithMesh()
    {
        // Drawing box informing if spine animator is working by mesh visibility factor
        if (Get.OptimizeWithMesh)
        {
            if (Application.isPlaying)
            {
                GUI.color = new Color(1f, 1f, 1f, .5f);
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

                if (Get.OptimizeWithMesh.isVisible)
                    EditorGUILayout.LabelField("Spine Animator Is Active", FGUI_Resources.HeaderStyle);
                else
                {
                    GUI.enabled = false;
                    EditorGUILayout.LabelField("Spine Animator Is Inactive", FGUI_Resources.HeaderStyle);
                    GUI.enabled = true;
                }

                EditorGUILayout.EndHorizontal();
                GUI.color = c;
            }
        }


        EditorGUILayout.BeginHorizontal();

        EditorGUIUtility.labelWidth = 144;
        EditorGUILayout.PropertyField(sp_OptimizeWithMesh);
        EditorGUIUtility.labelWidth = 0;

        if (Get.OptimizeWithMesh == null)
        {
            if (GUILayout.Button("Find", new GUILayoutOption[1] { GUILayout.Width(44) }))
            {
                if (Get.OptimizeWithMesh == null)
                {
                    Get.OptimizeWithMesh = Get.transform.GetComponent<Renderer>();
                    if (!Get.OptimizeWithMesh) Get.OptimizeWithMesh = Get.transform.GetComponentInChildren<Renderer>();
                    if (!Get.OptimizeWithMesh) if (Get.transform.parent != null) Get.OptimizeWithMesh = Get.transform.parent.GetComponentInChildren<Renderer>();
                    if (!Get.OptimizeWithMesh) if (Get.transform.parent != null) if (Get.transform.parent.parent != null) Get.OptimizeWithMesh = Get.transform.parent.parent.GetComponentInChildren<Renderer>();
                    if (!Get.OptimizeWithMesh) if (Get.transform.parent != null) if (Get.transform.parent.parent != null) if (Get.transform.parent.parent.parent != null) Get.OptimizeWithMesh = Get.transform.parent.parent.parent.GetComponentInChildren<Renderer>();
                }
            }
        }
        else
        {
            if (GUILayout.Button(hideSkin ? "Show" : "Hide", new GUILayoutOption[1] { GUILayout.Width(44) }))
            {
                hideSkin = !hideSkin;

                if (hideSkin)
                    for (int i = 0; i < skins.Count; i++) skins[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                else
                    for (int i = 0; i < skins.Count; i++) skins[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }

        EditorGUILayout.EndHorizontal();
    }


    bool drawFullChainCorrs = false;
    void El_DrawFullChainCorrections()
    {
        GUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);

        GUILayout.BeginHorizontal();

        if (Get.UseCorrections)
        {
            if (GUILayout.Button(new GUIContent(" " + FGUI_Resources.GetFoldSimbol(drawFullChainCorrs, 8, "►") + "  " + Lang("Spine Chain Corrections"), FGUI_Resources.Tex_GearMain, sp_UseCorrections.tooltip), FGUI_Resources.FoldStyle, GUILayout.Height(21))) drawFullChainCorrs = !drawFullChainCorrs;
        }
        else
            if (GUILayout.Button(new GUIContent("  Spine Chain Corrections", FGUI_Resources.Tex_GearMain, sp_UseCorrections.tooltip), FGUI_Resources.FoldStyle, GUILayout.Height(21))) { Get.UseCorrections = !Get.UseCorrections; serializedObject.ApplyModifiedProperties(); }

        GUILayout.FlexibleSpace();
        EditorGUILayout.PropertyField(sp_UseCorrections, new GUIContent("", sp_UseCorrections.tooltip), GUILayout.Width(22));

        GUILayout.EndHorizontal();


        if (drawFullChainCorrs && Get.UseCorrections)
        {
            GUILayout.Space(4f);
            EditorGUIUtility.labelWidth = 154f;
            EditorGUILayout.PropertyField(sp_SegmentsPivotOffset);
            EditorGUIUtility.labelWidth = 0f;
            GUILayout.Space(4f);
            GUILayout.EndVertical();

            El_DrawManualChainCorrections();
        }
        else
            GUILayout.EndVertical();
    }


    //bool drawManualChainCorrs = false;
    void El_DrawManualChainCorrections()
    {
        GUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);

        //if (GUILayout.Button(new GUIContent(" " + FGUI_Resources.GetFoldSimbol(drawManualChainCorrs, 8, "►") + "  Manual Chain Corrections"), FGUI_Resources.FoldStyle, GUILayout.Height(21))) drawManualChainCorrs = !drawManualChainCorrs;
        ////if (GUILayout.Button(new GUIContent(" " + FGUI_Resources.GetFoldSimbol(drawManualChainCorrs, 8, "►") + "  Manual Chain Corrections", FGUI_Resources.Tex_Gear), FGUI_Resources.FoldStyle, GUILayout.Height(21))) drawManualChainCorrs = !drawManualChainCorrs;

        //if (drawManualChainCorrs)
        {
            GUILayout.Space(4f);
            EditorGUIUtility.labelWidth = 154f;
            EditorGUILayout.PropertyField(sp_ManualAffects);
            EditorGUIUtility.labelWidth = 0f;


            //int strt = 0; int cnt = Get.SpineBones.Count;
            //if (!Get.LastBoneLeading) strt = 1; else cnt -= 1;

            if (!Get.LastBoneLeading)
            {
                for (int i = 0; i < Get.SpineBones.Count; i++)
                //for (int i = strt; i < cnt; i++)
                {
                    EditorGUILayout.BeginVertical(i % 2 == 0 ? FGUI_Resources.BGInBoxLightStyle : FGUI_Resources.BGInBoxStyle);
                    GUI.enabled = false; EditorGUILayout.ObjectField(Get.SpineBones[i].transform, typeof(Transform), true); GUI.enabled = true;
                    Get.SpineBones[i].ManualPosOffset = EditorGUILayout.Vector3Field("Position", Get.SpineBones[i].ManualPosOffset);
                    Get.SpineBones[i].ManualRotOffset = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation (degrees)", Get.SpineBones[i].ManualRotOffset.eulerAngles));
                    EditorGUILayout.EndVertical();
                }
            }
            else
                for (int i = Get.SpineBones.Count - 1; i >= 0; i--)
                //for (int i = cnt - 1; i >= strt; i--)
                {
                    EditorGUILayout.BeginVertical(i % 2 == 0 ? FGUI_Resources.BGInBoxLightStyle : FGUI_Resources.BGInBoxStyle);
                    GUI.enabled = false; EditorGUILayout.ObjectField(Get.SpineBones[i].transform, typeof(Transform), true); GUI.enabled = true;
                    Get.SpineBones[i].ManualPosOffset = EditorGUILayout.Vector3Field("Position", Get.SpineBones[i].ManualPosOffset);
                    Get.SpineBones[i].ManualRotOffset = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation (degrees)", Get.SpineBones[i].ManualRotOffset.eulerAngles));
                    EditorGUILayout.EndVertical();
                }


            GUILayout.Space(4f);
        }

        GUILayout.EndVertical();
    }


    bool anchoringWarning = false;
    bool drawAnchoring = false;
    void El_DrawAnchoring()
    {

        GUILayout.BeginVertical(FGUI_Resources.BGInBoxLightStyle);
        
        if (GUILayout.Button(new GUIContent(" " + FGUI_Resources.GetFoldSimbol(drawAnchoring, 8, "►") + "  " + Lang("Lead Bone Behaviour"), FGUI_Resources.TexMotionIcon), FGUI_Resources.FoldStyle, GUILayout.Height(21))) drawAnchoring = !drawAnchoring;
        if (drawAnchoring)
        {
            GUILayout.Space(4f);

            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(sp_LeadBoneRotationOffset, true);

            EditorGUIUtility.labelWidth = 14;
            EditorGUILayout.PropertyField(sp_LeadBoneOffsetReference, new GUIContent("A", sp_LeadBoneOffsetReference.tooltip), GUILayout.Width(38));
            EditorGUIUtility.labelWidth = 0;

            GUILayout.EndHorizontal();

            GUILayout.Space(4f);

            GUILayout.BeginHorizontal();
            if (Application.isPlaying) GUI.enabled = false;
            if (anchoringWarning) if (Get.HeadAnchor == null) GUI.color = new Color(1f, 1f, 0f, 1f); else GUI.color = new Color(.7f, 1f, .7f, 1f);

            if (Get.HeadAnchor == null && !anchoringWarning)
            {
                EditorGUIUtility.labelWidth = 150; EditorGUILayout.PropertyField(sp_HeadAnchor, new GUIContent("Head Anchor (Optional)", sp_HeadAnchor.tooltip)); EditorGUIUtility.labelWidth = 0;
            }
            else
                EditorGUILayout.PropertyField(sp_HeadAnchor);

            GUI.enabled = true; GUI.color = c;

            if (Get.SpineBones.Count > 0)
            {
                if (Get.HeadAnchor != Get.GetLeadingBone().transform)
                    if (GUILayout.Button("Use Head", new GUILayoutOption[] { GUILayout.Width(70), GUILayout.Height(16) })) { Get.HeadAnchor = Get.GetLeadingBone().transform; Get.UpdateAsLast = true; serializedObject.ApplyModifiedProperties(); }
            }

            if (Get.HeadAnchor)
            {
                EditorGUIUtility.labelWidth = 14;
                EditorGUILayout.PropertyField(sp_AnimateAnchor, new GUIContent("A", sp_AnimateAnchor.tooltip), GUILayout.Width(38));
                EditorGUIUtility.labelWidth = 0;
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(4f);
        }

        GUILayout.EndVertical();

        GUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);
        GUILayout.Space(4f);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(sp_FixersList, true);
        EditorGUI.indentLevel--;
        GUILayout.Space(4f);
        GUILayout.EndVertical();

    }



    bool drawRareCorrs = false;
    void El_DrawRareCorrections()
    {
        GUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);

        if (GUILayout.Button(new GUIContent(" " + FGUI_Resources.GetFoldSimbol(drawRareCorrs, 8, "►") + "  " + Lang("Rare Corrections"), FGUI_Resources.Tex_HiddenIcon), FGUI_Resources.FoldStyle, GUILayout.Height(21))) drawRareCorrs = !drawRareCorrs;

        if (drawRareCorrs)
        {
            GUILayout.Space(5f);
            EditorGUILayout.PropertyField(sp_ModelForwardAxis);
            EditorGUILayout.PropertyField(sp_ModelUpAxis);
            GUILayout.Space(5f);
        }

        GUILayout.EndVertical();
    }


    bool drawSelectiveColliders = false;
    void El_DrawSelectiveCollision()
    {
        GUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);

        if (GUILayout.Button(new GUIContent(" " + FGUI_Resources.GetFoldSimbol(drawSelectiveColliders, 8, "►") + "  " + Lang("Segments Collision"), FGUI_Resources.Tex_Gear), FGUI_Resources.FoldStyle, GUILayout.Height(21))) drawSelectiveColliders = !drawSelectiveColliders;
        ////if (GUILayout.Button(new GUIContent(" " + FGUI_Resources.GetFoldSimbol(drawManualChainCorrs, 8, "►") + "  Manual Chain Corrections", FGUI_Resources.Tex_Gear), FGUI_Resources.FoldStyle, GUILayout.Height(21))) drawManualChainCorrs = !drawManualChainCorrs;

        if (drawSelectiveColliders)
        {
            GUILayout.Space(7);
            //for (int i = 0; i < Get.SpineBones.Count; i++)
            int strt = 0; int cnt = Get.SpineBones.Count;
            if (!Get.LastBoneLeading) strt = 1; else cnt -= 1;
            for (int i = strt; i < cnt; i++)
            {
                EditorGUILayout.BeginVertical(i % 2 == 0 ? FGUI_Resources.BGInBoxLightStyle : FGUI_Resources.BGInBoxStyle);
                EditorGUILayout.BeginHorizontal();
                Get.SpineBones[i].Collide = EditorGUILayout.Toggle("", Get.SpineBones[i].Collide, GUILayout.Width(20));
                GUI.enabled = false; EditorGUILayout.ObjectField(Get.SpineBones[i].transform, typeof(Transform), true); GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                if (!Get.SpineBones[i].Collide) GUI.enabled = false;
                Get.SpineBones[i].ColliderOffset = EditorGUILayout.Vector3Field("", Get.SpineBones[i].ColliderOffset); GUI.enabled = true;
                EditorGUILayout.EndVertical();
            }
        }

        EditorGUILayout.EndVertical();
    }

    void El_DrawDragAndDropCollidersBox()
    {
        GUILayout.Space(3);

        var drop = GUILayoutUtility.GetRect(0f, 38f, new GUILayoutOption[1] { GUILayout.ExpandWidth(true) });
        GUI.color = new Color(0.5f, 1f, 0.5f, 0.9f);
        GUI.Box(drop, "Drag & Drop New Colliders Here", new GUIStyle(EditorStyles.helpBox) { alignment = TextAnchor.MiddleCenter, fixedHeight = 38 });
        GUI.color = c;
        var dropEvent = Event.current;

        switch (dropEvent.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!drop.Contains(dropEvent.mousePosition)) break;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (dropEvent.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (var dragged in DragAndDrop.objectReferences)
                    {
                        GameObject draggedObject = dragged as GameObject;

                        if (draggedObject)
                        {
                            Collider[] coll = draggedObject.GetComponents<Collider>();
                            for (int ci = 0; ci < coll.Length; ci++) Get.AddCollider(coll[ci]);
                            EditorUtility.SetDirty(target);
                        }
                    }

                }

                Event.current.Use();
                break;
        }

        //GUILayout.Space(3);
    }

}