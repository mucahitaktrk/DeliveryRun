using UnityEngine;

namespace FIMSpace.FSpine
{
    public partial class FSpineAnimator
    {
        /// <summary>
        /// Searching for second spine animator in parent, if there is not then transform of this game object is returned
        /// </summary>
        public Transform FindBaseTransform()
        {
            Transform target = transform;

            Transform p;
            Transform c = transform.parent;
            FSpineAnimator mySpine = null;

            if (c != null)
                for (int i = 0; i < 32; i++)
                {
                    p = c.parent;
                    mySpine = c.GetComponent<FSpineAnimator>();
                    if (mySpine) break;
                    c = p;
                    if (p == null) break;
                }

            if (mySpine != null)
            {
                if (mySpine.BaseTransform != null) target = mySpine.BaseTransform; else target = mySpine.transform;
                if (mySpine.transform != transform) UpdateAsLast = true;
            }

            return target;
        }


        /// <summary>
        /// Returning current lead bone corresponding to chain settings
        /// </summary>
        public SpineBone GetLeadingBone()
        {
            if (SpineBones == null || SpineBones.Count == 0) return null;
            if (LastBoneLeading) return SpineBones[SpineBones.Count - 1]; return SpineBones[0];
        }

        /// <summary>
        /// Returning current end bone corresponding to chain settings
        /// </summary>
        public SpineBone GetEndBone()
        {
            if (SpineBones == null || SpineBones.Count == 0) return null;
            if (LastBoneLeading) return SpineBones[0]; return SpineBones[SpineBones.Count - 1];
        }

        /// <summary>
        /// Gizmos needed method
        /// </summary>
        public Transform GetHeadBone()
        {
            if (SpineBones.Count <= 0) return transform;
            if (LastBoneLeading) return SpineBones[SpineBones.Count - 1].transform;
            return SpineBones[0].transform;
        }

        /// <summary>
        /// Gizmos needed method
        /// </summary>
        public SpineBone GetLeadBone()
        {
            if (LastBoneLeading) return SpineBones[SpineBones.Count - 1];
            return SpineBones[0];
        }

        public Transform GetBaseTransform()
        {
            if (BaseTransform == null) return transform; else return BaseTransform;
        }
    }
}