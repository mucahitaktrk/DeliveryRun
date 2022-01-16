using FIMSpace.FEditor;
using FIMSpace.FSpine;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class FSpineAnimator_Editor
{
    static bool drawNewInspector = true;


    #region Serialized properties definition


    //protected SerializedProperty sp_spines;
    protected SerializedProperty sp_forw;
    //protected SerializedProperty sp_drawg;

    protected SerializedProperty sp_SpineAnimatorAmount;
    protected SerializedProperty sp_LastBoneLeading;
    protected SerializedProperty sp_ReverseForward;
    //protected SerializedProperty sp_SyncWithAnimator;
    //protected SerializedProperty sp_DetectZeroKeyframes;
    protected SerializedProperty sp_BackwardMovement;
    protected SerializedProperty sp_PhysicalClock;
    protected SerializedProperty sp_OptimizeWithMesh;
    protected SerializedProperty sp_LeadBoneOffsetReference;
    //protected SerializedProperty sp_SafeDeltaTime;
    //protected SerializedProperty sp_AnchoredSpine;
    //protected SerializedProperty sp_AutoAnchor;
    protected SerializedProperty sp_HeadAnchor;
    protected SerializedProperty sp_AnimateAnchor;
    protected SerializedProperty sp_UpdateAsLast;
    protected SerializedProperty sp_MotionInfluence;
    protected SerializedProperty sp_MotionSpace;
    //protected SerializedProperty sp_PositionsNotAnimated;
    //protected SerializedProperty sp_SelectivePosNotAnimated;
    //protected SerializedProperty sp_RotationsNotAnimated;
    //protected SerializedProperty sp_SelectiveRotNotAnimated;
    protected SerializedProperty sp_ManualAffects;
    //protected SerializedProperty sp_ManualRotationOffsets;
    //protected SerializedProperty sp_ManualPositionOffsets;
    protected SerializedProperty sp_ModelForwardAxis;
    protected SerializedProperty sp_ModelUpAxis;
    protected SerializedProperty sp_RoundCorrection;
    protected SerializedProperty sp_UnifyCorrection;
    protected SerializedProperty sp_StartAfterTPose;
    protected SerializedProperty sp_InversedVerticalRotation;
    protected SerializedProperty sp_OrientationReference;
    protected SerializedProperty sp_DeltaType;

    protected SerializedProperty sp_PositionsSmoother;
    protected SerializedProperty sp_RotationsSmoother;
    protected SerializedProperty sp_MaxStretching;
    protected SerializedProperty sp_Slithery;

    protected SerializedProperty sp_AngleLimit;
    protected SerializedProperty sp_LimitingAngleSmoother;
    protected SerializedProperty sp_StraighteningSpeed;
    protected SerializedProperty sp_TurboStraighten;
    protected SerializedProperty sp_GoBackSpeed;
    protected SerializedProperty sp_Springiness;
    protected SerializedProperty sp_SegmentsPivotOffset;
    protected SerializedProperty sp_UseCorrections;
    protected SerializedProperty sp_DistancesMul;
    //protected SerializedProperty sp_AnimateLeadingBone;
    //protected SerializedProperty sp_LeadingAnimateAfterMotion;
    protected SerializedProperty sp_LeadBoneRotationOffset;
    //protected SerializedProperty sp_CustomAnchorRotationOffset;


    //protected SerializedProperty sp_DrawDebug;
    //protected SerializedProperty sp_DebugAlpha;
    //protected SerializedProperty sp_AddDebugAlpha;
    protected SerializedProperty sp_MainPivotOffset;

    protected SerializedProperty sp_UseCollisions;
    protected SerializedProperty sp_IncludedColliders;
    protected SerializedProperty sp_CollidersScale;
    protected SerializedProperty sp_CollidersScaleMul;
    protected SerializedProperty sp_CollidersAutoCurve;
    //protected SerializedProperty sp_CollidersOffsets;
    protected SerializedProperty sp_GravityPower;

    protected SerializedProperty sp_AllCollidersOffset;
    protected SerializedProperty sp_UseTruePosition;
    protected SerializedProperty sp_DetailedCollision;
    protected SerializedProperty sp_UpdateRate;
    protected SerializedProperty sp_FixersList;
    //protected SerializedProperty sp_SegmentCollision;

    public bool hideSkin = false;

    #endregion


    protected virtual void OnEnable()
    {
        //sp_spines = serializedObject.FindProperty("SpineTransforms");
        sp_forw = serializedObject.FindProperty("BaseTransform");
        //sp_drawg = serializedObject.FindProperty("drawGizmos");

        sp_SpineAnimatorAmount = serializedObject.FindProperty("SpineAnimatorAmount");
        sp_LastBoneLeading = serializedObject.FindProperty("LastBoneLeading");
        sp_ReverseForward = serializedObject.FindProperty("ReverseForward");
        //sp_SyncWithAnimator = serializedObject.FindProperty("SyncWithAnimator");
        //sp_DetectZeroKeyframes = serializedObject.FindProperty("DetectZeroKeyframes");
        sp_BackwardMovement = serializedObject.FindProperty("BackwardMovement");
        sp_PhysicalClock = serializedObject.FindProperty("AnimatePhysics");
        sp_OptimizeWithMesh = serializedObject.FindProperty("OptimizeWithMesh");
        sp_LeadBoneOffsetReference = serializedObject.FindProperty("LeadBoneOffsetReference");
        sp_UseCorrections = serializedObject.FindProperty("UseCorrections");
        sp_HeadAnchor = serializedObject.FindProperty("HeadAnchor");
        sp_AnimateAnchor = serializedObject.FindProperty("AnimateAnchor");
        sp_UpdateAsLast = serializedObject.FindProperty("UpdateAsLast");
        sp_MotionInfluence = serializedObject.FindProperty("MotionInfluence");
        sp_MotionSpace = serializedObject.FindProperty("MotionSpace");
        sp_ManualAffects = serializedObject.FindProperty("ManualAffectChain");
        sp_ModelForwardAxis = serializedObject.FindProperty("ModelForwardAxis");
        sp_ModelUpAxis = serializedObject.FindProperty("ModelUpAxis");
        sp_RoundCorrection = serializedObject.FindProperty("RoundCorrection");
        sp_UnifyCorrection = serializedObject.FindProperty("UnifyCorrection");
        sp_StartAfterTPose = serializedObject.FindProperty("StartAfterTPose");
        sp_InversedVerticalRotation = serializedObject.FindProperty("InversedVerticalRotation");
        sp_OrientationReference = serializedObject.FindProperty("OrientationReference");
        sp_DeltaType = serializedObject.FindProperty("DeltaType");

        sp_PositionsSmoother = serializedObject.FindProperty("PosSmoother");
        sp_RotationsSmoother = serializedObject.FindProperty("RotSmoother");
        sp_MaxStretching = serializedObject.FindProperty("MaxStretching");
        sp_Slithery = serializedObject.FindProperty("Slithery");
        sp_AngleLimit = serializedObject.FindProperty("AngleLimit");
        sp_LimitingAngleSmoother = serializedObject.FindProperty("LimitSmoother");
        sp_StraighteningSpeed = serializedObject.FindProperty("StraightenSpeed");
        sp_TurboStraighten = serializedObject.FindProperty("TurboStraighten");
        sp_GoBackSpeed = serializedObject.FindProperty("GoBackSpeed");
        sp_Springiness = serializedObject.FindProperty("Springiness");
        sp_SegmentsPivotOffset = serializedObject.FindProperty("SegmentsPivotOffset");
        sp_MainPivotOffset = serializedObject.FindProperty("MainPivotOffset");
        sp_DistancesMul = serializedObject.FindProperty("DistancesMultiplier");
        sp_LeadBoneRotationOffset = serializedObject.FindProperty("LeadBoneRotationOffset");

        sp_UseCollisions = serializedObject.FindProperty("UseCollisions");
        sp_IncludedColliders = serializedObject.FindProperty("IncludedColliders");
        sp_CollidersScale = serializedObject.FindProperty("CollidersScale");
        sp_CollidersScaleMul = serializedObject.FindProperty("CollidersScaleMul");
        sp_CollidersAutoCurve = serializedObject.FindProperty("DifferenceScaleFactor");
        sp_GravityPower = serializedObject.FindProperty("GravityPower");

        sp_AllCollidersOffset = serializedObject.FindProperty("OffsetAllColliders");
        sp_UseTruePosition = serializedObject.FindProperty("UseTruePosition");
        sp_DetailedCollision = serializedObject.FindProperty("DetailedCollision");
        sp_UpdateRate = serializedObject.FindProperty("UpdateRate");
        sp_FixersList = serializedObject.FindProperty("BonesFixers");

        BackCompatibilityConversion();
        FGUI_Finders.ResetFinders(false);
        FindComponents();
        AutoSuggestChain();

        if (Get.SpineBones != null)
            if (Get.SpineBones.Count > 0)
            {
                if (Get.SpineBones[Get.SpineBones.Count - 1].transform)
                    if (Get.SpineBones[Get.SpineBones.Count - 1].transform != Get._gizmosEditorStartPreview)
                        Get._gizmosEditorEndPreview = Get.SpineBones[Get.SpineBones.Count - 1].transform;

                if (Get.SpineBones[0].transform)
                    if (Get.SpineBones[0].transform != Get._gizmosEditorEndPreview)
                        Get._gizmosEditorStartPreview = Get.SpineBones[0].transform;
            }

        if (hideSkin) for (int i = 0; i < skins.Count; i++)
            {
                skins[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }


        // Detecting if using few spine animators
        if (Get.BaseTransform != Get.transform)
        {
            if (Get.BaseTransform != null)
            {
                if (Get.BaseTransform.GetComponent<FSpineAnimator>())
                {
                    anchoringWarning = true;
                    //drawCorrecting = true;
                    drawAnchoring = true;
                }
            }
        }

        //if (Get.UseCollisions) drawPhysics = true;

        if (Get.SpineBones == null) Get.SpineBones = new List<FSpineAnimator.SpineBone>();
        if (Get.SpineBones.Count < 1)
        {
            if (SceneView.lastActiveSceneView)
                SceneView.lastActiveSceneView.FrameSelected();
        }

        if (animator)
        {
            if (animator.updateMode == AnimatorUpdateMode.AnimatePhysics)
            {
                if (Get.AnimatePhysics == FSpineAnimator.EFixedMode.None)
                {
                    //drawSetup = true;
                    drawAdditionalSetup = true;
                }
            }
        }
    }

    private void BackCompatibilityConversion()
    {
        List<Transform> tr = Get.GetOldSpineTransforms();

        if (Get.SpineBones != null)
            if (Get.SpineBones.Count <= 1)
                if (tr != null)
                {
                    if (tr.Count > 1)
                    {
                        Get.CreateSpineChain(tr[0], tr[tr.Count - 1]);
                        Get.ClearOldSpineTransforms();
                        Get.UpdateAsLast = Get.QueueToLastUpdate;
                        Get.BaseTransform = Get.ForwardReference;
                        Get.HeadAnchor = Get.AnchorRoot;

                        if (Get.SegmentsPivotOffset != Vector3.zero) Get.UseCorrections = true;

                        serializedObject.Update();
                        serializedObject.ApplyModifiedProperties();

                        Debug.Log("[Spine Animator] Successfully converted old bones setup to new version of Spine Animator (" + Get.name + ") If it is prefab please apply this changes.");
                    }
                }
    }

    void OnDisable()
    {
        if (hideSkin) for (int i = 0; i < skins.Count; i++)
            {
                skins[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
    }
}