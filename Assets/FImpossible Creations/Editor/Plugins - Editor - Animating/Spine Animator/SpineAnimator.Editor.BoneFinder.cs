using System.Collections.Generic;
using UnityEngine;

public partial class FSpineAnimator_Editor
{
    /// <summary>
    /// Getting last bone in hierarhy going up by first children
    /// </summary>
    void GetLastBoneInHierarchy()
    {
        if (startBone == null)
        {
            Debug.LogWarning("Start bone is not defined in " + target.name);
            return;
        }

        Transform c = startBone;

        // Try to find bones with spine word in it and go through deepest found with this name
        Transform spine = null;
        foreach (Transform t in c.GetComponentsInChildren<Transform>())
        {
            if (t.name.ToLower().Contains("spine"))
            {
                spine = t;
                break;
            }
        }

        if (spine != null) c = spine;

        // I'm scared of while() loops so I just put here iterator to limit in some case
        for (int i = 0; i < 1000; i++)
        {
            if (c.childCount > 0)
            {
                for (int j = 0; j < c.childCount; j++)
                    if (c.GetChild(j).name.ToLower().Contains("spine"))
                    {
                        c = c.GetChild(j);
                        break;
                    }

                c = c.GetChild(0);
            }
            else break;
        }

        endBone = c;
    }


    public List<SkinnedMeshRenderer> skins;
    SkinnedMeshRenderer largestSkin;
    Animator animator;
    Animation animation;

    Transform startBone { get { return Get._gizmosEditorStartPreview; } set { Get._gizmosEditorStartPreview = value; } }
    Transform endBone { get { return Get._gizmosEditorEndPreview; } set { Get._gizmosEditorEndPreview = value; } }

    /// <summary>
    /// Trying to deep find skinned mesh renderer
    /// </summary>
    private void FindComponents()
    {
        if (skins == null) skins = new List<SkinnedMeshRenderer>();

        foreach (var t in Get.GetBaseTransform().GetComponentsInChildren<Transform>())
        {
            SkinnedMeshRenderer s = t.GetComponent<SkinnedMeshRenderer>(); if (s) skins.Add(s);
            if (!animator) animator = t.GetComponent<Animator>();
            if (!animator) if (!animation) animation = t.GetComponent<Animation>();
        }

        if ((skins != null && largestSkin != null) && (animator != null || animation != null)) return;

        if (Get.GetBaseTransform() != Get.transform)
        {
            foreach (var t in Get.transform.GetComponentsInChildren<Transform>())
            {
                SkinnedMeshRenderer s = t.GetComponent<SkinnedMeshRenderer>(); if (!skins.Contains(s)) if (s) skins.Add(s);
                if (!animator) animator = t.GetComponent<Animator>();
                if (!animator) if (!animation) animation = t.GetComponent<Animation>();
            }
        }

        // Searching in parent
        if ( skins.Count == 0 )
        {
            Transform lastParent = Get.transform;

            while(lastParent != null)
            {
                if (lastParent.parent == null) break;
                lastParent = lastParent.parent;
            }

            foreach (var t in lastParent.GetComponentsInChildren<Transform>())
            {
                SkinnedMeshRenderer s = t.GetComponent<SkinnedMeshRenderer>(); if (!skins.Contains(s)) if (s) skins.Add(s);
                if (!animator) animator = t.GetComponent<Animator>();
                if (!animator) if (!animation) animation = t.GetComponent<Animation>();
            }
        }

        if (skins.Count > 1)
        {
            largestSkin = skins[0];
            for (int i = 1; i < skins.Count; i++)
                if (skins[i].bones.Length > largestSkin.bones.Length)
                    largestSkin = skins[i];
        }
        else
            if (skins.Count > 0) largestSkin = skins[0];

    }



    /// <summary>
    /// Automatically finding start and end bone 
    /// </summary>
    void AutoSuggestChain()
    {
        Transform preStart = startBone;
        Transform preEnd = endBone;

        TryFindStartBone();
        TryFindEndBone();

        //if (preStart != startBone || preEnd != endBone)
        //    _autoFound = true;
        //else
        //    _autoFound = false;
    }

    /// <summary>
    /// Trying to find right start bone for spine chain
    /// </summary>
    void TryFindStartBone()
    {
        Transform start = null;

        for (int i = 0; i < skins.Count; i++)
        {
            if (start) break;

            foreach (var t in skins[i].bones)
            {
                if (start) break;
                if (t.name.ToLower().Contains("pelv")) start = t;
                else if (t.name.ToLower().Contains("root")) start = t;
                else if (t.name.ToLower().Contains("spine")) start = t;
            }
        }

        if (!start)
        {
            Transform[] children = Get.GetComponentsInChildren<Transform>();

            foreach (Transform t in children)
            {
                if (t.name.ToLower().Contains("pelv")) start = t;
                if (t.name.ToLower().Contains("root")) start = t;
                if (t.name.ToLower().Contains("spine")) start = t;
            }
        }

        if (start) startBone = start;
    }


    /// <summary>
    /// Trying to find right end bone for spine chain
    /// </summary>
    void TryFindEndBone()
    {
        Transform end = null;

        for (int i = 0; i < skins.Count; i++)
        {
            if (end) break;

            foreach (var t in skins[i].bones)
            {
                if (end) break;
                if (t.name.ToLower().Contains("neck")) end = t;
                else if (t.name.ToLower().Contains("head")) end = t;
                else if (t.name.ToLower().Contains("chest")) end = t;
            }
        }

        if (!end)
        {
            Transform[] children = Get.GetComponentsInChildren<Transform>();

            foreach (Transform t in children)
            {
                if (t.name.ToLower().Contains("neck")) end = t;
                if (t.name.ToLower().Contains("head")) end = t;
                if (t.name.ToLower().Contains("chest")) end = t;
            }
        }

        if (end) endBone = end;


    }
}