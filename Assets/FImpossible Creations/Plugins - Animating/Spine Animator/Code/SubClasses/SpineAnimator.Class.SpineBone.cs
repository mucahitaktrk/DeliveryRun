using System;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FSpine
{
    public partial class FSpineAnimator
    {
        /// <summary>
        /// FC: Class handling motion of each bone in spine chain
        /// </summary>
        [System.Serializable]
        public partial class SpineBone
        {
            /// <summary> Bone transform in world </summary>
            public Transform transform;

            /// <summary> Procedural position for bone </summary>
            public Vector3 ProceduralPosition;
            /// <summary> Procedural rotation for bone </summary>
            public Quaternion ProceduralRotation;

            /// <summary> Helper variable for calculating difference position </summary>
            public Vector3 HelperDiffPosition;
            /// <summary> Helper variable for calculating difference rotation </summary>
            public Quaternion HelperDiffRoation;

            /// <summary> For springiness animation support </summary>
            public Vector3 PreviousPosition;
            /// <summary> Preventing zero look direction </summary>
            public Vector3 DefaultForward;

            /// <summary> Variable which helps straigtening spine when character is moving in world space </summary>
            public float StraightenFactor = 0f;
            /// <summary> Variable which helps straigtening spine when character is moving in world space </summary>
            public float TargetStraightenFactor = 0f;

            /// <summary> Bone length - distance to child bone calculated at game start. SetDistanceForFrame...() used for directional definition </summary>
            public float BoneLength { get; private set; }
            private float boneLengthB = 0.1f;
            private float boneLengthF = 0.1f;

            /// <summary> Initial local bone position offset </summary>
            public Vector3 BoneLocalOffset { get; private set; }
            private Vector3 boneLocalOffsetB;
            private Vector3 boneLocalOffsetF;

            /// <summary> Bone rotation and position weight factor </summary>
            public float MotionWeight = 1f;

            /// <summary> Rotation value of target bone + offset or just target rotation if not syncing with animator </summary>
            public Quaternion FinalRotation;
            /// <summary> Target position for bone transforms with offsets etc. </summary>
            public Vector3 FinalPosition;

            /// <summary> Additional bone position offset mainly for correcting pose of character </summary>
            public Vector3 ManualPosOffset;
            /// <summary> Additional bone rotation offset mainly for correcting pose of character </summary>
            public Quaternion ManualRotOffset;


            /// <summary> Variable for refreshing bone position when working with unsynced model </summary>
            public Vector3 InitialLocalPosition { get; private set; }
            /// <summary> Variable for refreshing bone rotation when working with unsynced model </summary>
            public Quaternion InitialLocalRotation { get; private set; }

            public void UpdateReferencePosition(Vector3 pos)
            {
                PreviousReferencePosition = ReferencePosition;
                ReferencePosition = pos;
            }

            /// <summary> Straight forward reference position for pose difference calculations </summary>
            public Vector3 ReferencePosition;
            public Vector3 PreviousReferencePosition;
            /// <summary> Straight forward reference rotation for pose difference calculations </summary>
            public Quaternion ReferenceRotation;


            #region Zero keyframe detection


            /// <summary> Variable to detect if some bones are not keyframed in animation to avoid spinning glitch </summary>
            private Quaternion lastKeyframeRotation;
            /// <summary> Variable to detect if some bones are not keyframed in animation to avoid spinning glitch </summary>
            private Vector3 lastKeyframePosition;

            /// <summary> Variable to detect if some bones are not keyframed in animation to avoid spinning glitch </summary>
            private Vector3 lastFinalLocalPosition;
            /// <summary> Variable to detect if some bones are not keyframed in animation to avoid spinning glitch </summary>
            private Quaternion lastFinalLocalRotation;


            /// <summary>
            /// Checking for null keyframes to prevent spinning error
            /// </summary>
            public void ZeroKeyframeCheck()
            {
                // Null keyframe detected 
                if (FEngineering.QIsSame(lastFinalLocalRotation, transform.localRotation))
                    transform.localRotation = lastKeyframeRotation;
                else // Remembering rotation to check for null keyframe
                    lastKeyframeRotation = transform.localRotation;

                if (FEngineering.VIsSame(lastFinalLocalPosition, transform.localPosition))
                    transform.localPosition = lastKeyframePosition;
                else // Remembering position to check for null keyframe
                    lastKeyframePosition = transform.localPosition;
            }

            /// <summary>
            /// Remembering last final calculated pose
            /// </summary>
            public void RefreshFinalLocalPose()
            {
                lastFinalLocalPosition = transform.localPosition;
                lastFinalLocalRotation = transform.localRotation;
            }


            #endregion


            #region Additional helper rotation variables

            public Vector3 forward;
            public Vector3 right;
            public Vector3 up;

            #endregion


            /// <summary>
            /// Generating bones instance just for editor usage, use PrepareBone() for playmode setup
            /// </summary>
            public SpineBone(Transform t)
            {
                transform = t;
                ManualPosOffset = Vector3.zero;
                ColliderOffset = Vector3.zero;
                Collide = true;
                CollisionRadius = 1f;
            }


            #region Preparations


            /// <summary>
            /// Preparing bone for playmode usage
            /// </summary>
            public void PrepareBone(Transform baseTransform, List<SpineBone> bones, int index)
            {
                InitialLocalPosition = transform.localPosition;
                InitialLocalRotation = transform.localRotation;

                // Prepare two directional distances (bone length)
                Vector3 nextPos;

                if (index == bones.Count - 1) nextPos = bones[index - 1].transform.position; else nextPos = bones[index + 1].transform.position;
                if (index == 0) nextPos = bones[index + 1].transform.position; else nextPos = bones[index - 1].transform.position;
               
                float dist = Vector3.Distance(baseTransform.InverseTransformPoint(nextPos), baseTransform.InverseTransformPoint(bones[index].transform.position));
                if (dist < 0.01f)
                {
                    int nInd = index + 2;

                    if ( nInd < bones.Count)
                        DefaultForward = transform.InverseTransformPoint(bones[nInd].transform.position);
                    else
                        DefaultForward = transform.InverseTransformPoint(nextPos - baseTransform.position);

                    //Vector3 towards = baseTransform.InverseTransformVector(nextPos - bones[index].transform.position);
                    ////if ( index < bones.Count - 2) towards = 
                    //nextPos += towards * 1f;
                    //InitialLocalPosition -= towards * 1f;
                }
                else
                    DefaultForward = transform.InverseTransformPoint(nextPos);

                boneLengthB = (baseTransform.InverseTransformPoint(transform.position) - baseTransform.InverseTransformPoint(nextPos)).magnitude;
                boneLocalOffsetB = baseTransform.InverseTransformPoint(nextPos);

                boneLengthF = (baseTransform.InverseTransformPoint(transform.position) - baseTransform.InverseTransformPoint(nextPos)).magnitude;
                boneLocalOffsetF = baseTransform.InverseTransformPoint(nextPos);

                if (ManualPosOffset.sqrMagnitude == 0) ManualPosOffset = Vector3.zero;
                if (ManualRotOffset.eulerAngles.sqrMagnitude == 0) ManualRotOffset = Quaternion.identity;

                SetDistanceForFrameForward();

                // Prepare bone orientation references
                PrepareAxes(baseTransform, bones, index);
            }

            public void SetDistanceForFrameForward()
            {
                BoneLength = boneLengthF;
                BoneLocalOffset = boneLocalOffsetF;
            }

            public void SetDistanceForFrameBackward()
            {
                BoneLength = boneLengthB;
                BoneLocalOffset = boneLocalOffsetB;
            }

            public float GetUnscalledBoneLength()
            {
                if (boneLengthF > boneLengthB) return boneLengthF; else return boneLengthB;
            }


            /// <summary>
            /// Calculating axis orientation helper variables
            /// </summary>
            private void PrepareAxes(Transform baseTransform, List<SpineBone> bonesList, int index)
            {
                Transform parentTr, childTr;
                Vector3 childPos, parentPos;

                #region Detecting parent and child at the end of the array

                if (index == bonesList.Count - 1)
                {
                    if (transform.childCount == 1)
                    {
                        parentTr = transform;
                        childTr = transform.GetChild(0);
                        parentPos = parentTr.position;
                        childPos = childTr.position;
                    }
                    else
                    {
                        parentTr = transform;
                        childTr = transform;

                        parentPos = bonesList[index - 1].transform.position;
                        childPos = transform.position;
                    }
                }
                else
                {
                    parentTr = transform;
                    childTr = bonesList[index + 1].transform;
                    parentPos = parentTr.position;
                    childPos = childTr.position;
                }

                #endregion

                Vector3 forwardInBoneOrientation = parentTr.InverseTransformDirection(childPos) - parentTr.InverseTransformDirection(parentPos);
                Vector3 projectedUp = Vector3.ProjectOnPlane(baseTransform.up, transform.TransformDirection(forwardInBoneOrientation).normalized).normalized;
                Vector3 upInBoneOrientation = parentTr.InverseTransformDirection(parentPos + projectedUp) - parentTr.InverseTransformDirection(parentPos);
                Vector3 crossRight = Vector3.Cross(transform.TransformDirection(forwardInBoneOrientation), transform.TransformDirection(upInBoneOrientation));
                Vector3 rightInBoneOrientation = parentTr.InverseTransformDirection(parentPos + crossRight) - parentTr.InverseTransformDirection(parentPos);

                right = rightInBoneOrientation.normalized;
                up = upInBoneOrientation.normalized;
                forward = forwardInBoneOrientation.normalized;
            }


            #endregion

            #region Playmode Methods


            internal void CalculateDifferencePose(Vector3 upAxis, Vector3 rightAxis)
            {
                HelperDiffPosition = ProceduralPosition - ReferencePosition;

                Quaternion fixedBendRotation = ProceduralRotation * Quaternion.FromToRotation(up, upAxis) * Quaternion.FromToRotation(right, rightAxis);
                Quaternion fixedRefRotation = ReferenceRotation * Quaternion.FromToRotation(up, upAxis) * Quaternion.FromToRotation(right, rightAxis);

                HelperDiffRoation = fixedBendRotation * Quaternion.Inverse(fixedRefRotation);
            }

            /// <summary>
            /// Applying final coordinates from calculated difference to straight forward chain pose
            /// </summary>
            internal void ApplyDifferencePose()
            {
                FinalPosition = transform.position + HelperDiffPosition;
                FinalRotation = HelperDiffRoation * transform.rotation;
            }

            #endregion


            #region Editor Only

            public void Editor_SetLength(float length)
            {
                if (!Application.isPlaying) BoneLength = length;
            }


            #endregion


        }

    }
}