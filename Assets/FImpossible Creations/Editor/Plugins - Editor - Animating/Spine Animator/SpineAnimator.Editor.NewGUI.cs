using FIMSpace.FEditor;
using UnityEditor;
using UnityEngine;

public partial class FSpineAnimator_Editor
{
    void DrawNewGUI()
    {
        #region Preparations for unity versions and skin

        c = Color.Lerp(GUI.color * new Color(0.8f, 0.8f, 0.8f, 0.7f), GUI.color, Mathf.InverseLerp(0f, 0.15f, Get.SpineAnimatorAmount));
        bc = GUI.backgroundColor;

        RectOffset zeroOff = new RectOffset(0, 0, 0, 0);
        float bgAlpha = 0.05f; if (EditorGUIUtility.isProSkin) bgAlpha = 0.1f;

#if UNITY_2019_3_OR_NEWER
        int headerHeight = 22;
#else
        int headerHeight = 25;
#endif

        if (Get.SpineBones == null) Get.SpineBones = new System.Collections.Generic.List<FIMSpace.FSpine.FSpineAnimator.SpineBone>();

        #endregion

        GUILayout.BeginVertical(FGUI_Resources.BGBoxStyle); GUILayout.Space(1f);


        // ------------------------------------------------------------------------

        // If spine setup is not finished, then not drawing rest of the inspector
        if (Get.SpineBones.Count <= 1)
        {
            Get._Editor_Category = FIMSpace.FSpine.FSpineAnimator.EFSpineEditorCategory.Setup;
            GUILayout.BeginVertical(FGUI_Inspector.Style(zeroOff, zeroOff, new Color(.7f, .7f, 0.7f, bgAlpha), Vector4.one * 3, 3));

            EditorGUILayout.BeginHorizontal(FGUI_Resources.HeaderBoxStyle);

            GUILayout.Label(new GUIContent(" "), GUILayout.Width(1));
            if (GUILayout.Button(new GUIContent(FGUI_Resources.Tex_GearSetup), EditorStyles.label, new GUILayoutOption[2] { GUILayout.Width(headerHeight), GUILayout.Height(headerHeight) })) { }
            if (GUILayout.Button(Lang("Prepare Spine Chain"), LangBig() ? FGUI_Resources.HeaderStyleBig : FGUI_Resources.HeaderStyle, GUILayout.Height(headerHeight))) { }
            if (GUILayout.Button(new GUIContent(FGUI_Resources.Tex_Repair), EditorStyles.label, new GUILayoutOption[2] { GUILayout.Width(headerHeight), GUILayout.Height(headerHeight) })) { }
            GUILayout.Label(new GUIContent(" "), GUILayout.Width(1));

            EditorGUILayout.EndHorizontal();

            Tab_DrawPreSetup();

            GUILayout.EndVertical();
            GUILayout.EndVertical();
            return;
        }


        GUILayout.Space(2);
        EditorGUILayout.BeginHorizontal();
        DrawCategoryButton(FIMSpace.FSpine.FSpineAnimator.EFSpineEditorCategory.Setup, FGUI_Resources.Tex_GearSetup, "Setup");
        DrawCategoryButton(FIMSpace.FSpine.FSpineAnimator.EFSpineEditorCategory.Tweak, FGUI_Resources.Tex_Sliders, "Tweak");
        DrawCategoryButton(FIMSpace.FSpine.FSpineAnimator.EFSpineEditorCategory.Adjust, FGUI_Resources.Tex_Repair, "Adjust");
        DrawCategoryButton(FIMSpace.FSpine.FSpineAnimator.EFSpineEditorCategory.Physical, FGUI_Resources.Tex_Collider, "Physical");
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(4);


        switch (Get._Editor_Category)
        {
            case FIMSpace.FSpine.FSpineAnimator.EFSpineEditorCategory.Setup:

                GUILayout.BeginVertical(FGUI_Inspector.Style(zeroOff, zeroOff, new Color(.7f, .7f, 0.7f, bgAlpha), Vector4.one * 3, 3));

                FGUI_Inspector.HeaderBox(Lang("Character Setup"), true, FGUI_Resources.Tex_GearSetup, headerHeight, headerHeight - 1, LangBig());
                Tab_DrawSetup();

                GUILayout.EndVertical();
                break;

            case FIMSpace.FSpine.FSpineAnimator.EFSpineEditorCategory.Tweak:
                GUILayout.BeginVertical(FGUI_Inspector.Style(zeroOff, zeroOff, new Color(.3f, .4f, 1f, bgAlpha), Vector4.one * 3, 3));
                FGUI_Inspector.HeaderBox(Lang("Tweak Animation"), true, FGUI_Resources.Tex_Sliders, headerHeight, headerHeight - 1, LangBig());

                Tab_DrawTweaking();

                GUILayout.EndVertical();
                break;

            case FIMSpace.FSpine.FSpineAnimator.EFSpineEditorCategory.Adjust:
                GUILayout.BeginVertical(FGUI_Inspector.Style(zeroOff, zeroOff, new Color(.825f, .825f, 0.225f, bgAlpha * 0.8f), Vector4.one * 3, 3));
                FGUI_Inspector.HeaderBox(Lang("Adjustements"), true, FGUI_Resources.Tex_Repair, headerHeight, headerHeight - 1, LangBig());

                Tab_DrawCorrections();

                GUILayout.EndVertical();
                break;

            case FIMSpace.FSpine.FSpineAnimator.EFSpineEditorCategory.Physical:
                GUILayout.BeginVertical(FGUI_Inspector.Style(zeroOff, zeroOff, new Color(.4f, 1f, .7f, bgAlpha), Vector4.one * 3, 3));
                FGUI_Inspector.HeaderBox(Lang("Physical Parameters"), true, FGUI_Resources.Tex_Physics, headerHeight, headerHeight - 1, LangBig());
                Tab_DrawPhysics();

                GUILayout.EndVertical();
                break;
        }


        GUILayout.EndVertical();
    }

    void DrawCategoryButton(FIMSpace.FSpine.FSpineAnimator.EFSpineEditorCategory target, Texture icon, string lang)
    {
        if (Get._Editor_Category == target) GUI.backgroundColor = new Color(0.1f, 1f, 0.2f, 1f);

        int height = 28;
        int lim = 360;
        if (choosedLang == ELangs.русский) lim = 390;

        if (EditorGUIUtility.currentViewWidth > lim)
        {
            if (GUILayout.Button(new GUIContent("  " + Lang(lang), icon), FGUI_Resources.ButtonStyle, GUILayout.Height(height))) Get._Editor_Category = target;
        }
        else
            if (GUILayout.Button(new GUIContent(icon, Lang(lang)), FGUI_Resources.ButtonStyle, GUILayout.Height(height))) Get._Editor_Category = target;
        
        GUI.backgroundColor = bc;
    }

}