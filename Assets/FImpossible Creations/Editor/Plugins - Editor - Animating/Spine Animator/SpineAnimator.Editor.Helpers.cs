using FIMSpace.FSpine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class FSpineAnimator_Editor
{
    // RESOURCES ----------------------------------------

    public static Texture2D _TexSpineAnimIcon { get { if (__texSpineAnimIcon != null) return __texSpineAnimIcon; __texSpineAnimIcon = Resources.Load<Texture2D>("Spine Animator/SpineAnimator_SmallIcon"); return __texSpineAnimIcon; } }
    private static Texture2D __texSpineAnimIcon = null;
    public static Texture2D _TexLeadingRIcon { get { if (__texLeadingRIcon != null) return __texLeadingRIcon; __texLeadingRIcon = Resources.Load<Texture2D>("Spine Animator/LeadingR"); return __texLeadingRIcon; } }
    private static Texture2D __texLeadingRIcon = null;
    public static Texture2D _TexLeadingLIcon { get { if (__texLeadingLIcon != null) return __texLeadingLIcon; __texLeadingLIcon = Resources.Load<Texture2D>("Spine Animator/LeadingL"); return __texLeadingLIcon; } }
    private static Texture2D __texLeadingLIcon = null;
    public static Texture2D _TexSpineWideIcon { get { if (__texSpideWideIcon != null) return __texSpideWideIcon; __texSpideWideIcon = Resources.Load<Texture2D>("Spine Animator/SpineWide"); return __texSpideWideIcon; } }
    private static Texture2D __texSpideWideIcon = null;
    //public static Texture2D _TexBirdIcon { get { if (__texBirdIcon != null) return __texBirdIcon; __texBirdIcon = Resources.Load<Texture2D>("Look Animator/BirdMode"); return __texBirdIcon; } }
    //private static Texture2D __texBirdIcon = null;
    //public static Texture2D _TexEyesIcon { get { if (__texEyeIcon != null) return __texEyeIcon; __texEyeIcon = Resources.Load<Texture2D>("Look Animator/LookEyes"); return __texEyeIcon; } }
    //private static Texture2D __texEyeIcon = null;


    private static UnityEngine.Object _manualFile;


    // HELPER VARIABLES ----------------------------------------
    private FSpineAnimator Get { get { if (_get == null) _get = target as FSpineAnimator; return _get; } }
    private FSpineAnimator _get;

    static bool drawDefaultInspector = false;
    //private Color unchangedC = new Color(1f, 1f, 1f, 0.65f);
    //private Color limitsC = new Color(1f, 1f, 1f, 0.88f);
    private Color c;
    private Color bc;




    protected bool CheckForAnimator(FSpineAnimator spineAnimator)
    {
        Animation animation = null;
        Animator animator = spineAnimator.GetComponentInChildren<Animator>();
        if (!animator) if (spineAnimator.transform.parent) if (spineAnimator.transform.parent.parent) animator = spineAnimator.transform.parent.parent.GetComponentInChildren<Animator>(); else animator = spineAnimator.transform.parent.GetComponent<Animator>();

        if (!animator)
        {
            animation = spineAnimator.GetComponentInChildren<Animation>();
            if (!animation) if (spineAnimator.transform.parent) if (spineAnimator.transform.parent.parent) animation = spineAnimator.transform.parent.parent.GetComponentInChildren<Animation>(); else animation = spineAnimator.transform.parent.GetComponent<Animation>();
        }

        if (!animator && !animation)
        {
            if (spineAnimator.BaseTransform)
            {
                animator = spineAnimator.BaseTransform.GetComponentInChildren<Animator>();
                if (!animator) animation = spineAnimator.BaseTransform.GetComponentInChildren<Animation>();
            }

            if (!animator && !animation)
                if (spineAnimator.SpineBones.Count > 0)
                    if (spineAnimator.SpineBones[0].transform.parent)
                    {
                        animator = spineAnimator.SpineBones[0].transform.parent.GetComponentInChildren<Animator>();
                        if (!animator) animation = spineAnimator.SpineBones[0].transform.parent.GetComponentInChildren<Animation>();
                    }
        }

        //if (spineAnimator.SyncWithAnimator)
        //    if (animator)
        //    {
        //        if (animator.runtimeAnimatorController == null)
        //        {
        //            EditorGUILayout.HelpBox("No 'Animator Controller' inside Animator", MessageType.Warning);
        //            animator = null;
        //        }
        //    }


        if (animator != null || animation != null)
        {
            if (animator) if (!animator.enabled) return false;
            if (animation) if (!animation.enabled) return false;
            return true;
        }
        else return false;
    }










}