using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FSpine
{
    public partial class FSpineAnimator
    {
        public class HeadBone
        {
            public Transform baseTransform;
            public Transform transform;

            /// <summary> Variable for refreshing bone position when working with unsynced model </summary>
            public Vector3 InitialLocalPosition { get; private set; }
            /// <summary> Variable for refreshing bone rotation when working with unsynced model </summary>
            public Quaternion InitialLocalRotation { get; private set; }


            public HeadBone(Transform t)
            {
                transform = t;
            }


            public void PrepareBone(Transform baseTransform, List<SpineBone> bones, int index)
            {
                // Prepare local offsets in baseTransform transform space
                TakePoseSnapshot(baseTransform, bones, index);

                InitialLocalPosition = transform.localPosition;
                InitialLocalRotation = transform.localRotation;
            }


            internal Quaternion GetLocalRotationDiff()
            {
                return (transform.rotation) * Quaternion.Inverse(snapshotPoseLocalRotation);
            }


            #region Pose Snapshot


            private Vector3 snapshotPoseBaseTrSpacePosition;

            /// <summary> Snapshot of position in base transform space to transpone it during gameplay to world space </summary>
            public Vector3 SnapshotPosition;

            private Quaternion snapshotPoseBaseTrSpaceRotationF;
            private Quaternion snapshotPoseBaseTrSpaceRotationB;
            public Quaternion snapshotPoseLocalRotation;

            /// <summary> Snapshot of rotation in base transform space to transpone it during gameplay to world space </summary>
            public Quaternion SnapshotRotation;

            public void SetCoordsForFrameForward()
            {
                //BoneLength = boneLengthF;
                SnapshotPosition = baseTransform.TransformPoint(snapshotPoseBaseTrSpacePosition);
                SnapshotRotation = baseTransform.rotation * snapshotPoseBaseTrSpaceRotationF;
            }

            public void SetCoordsForFrameBackward()
            {
                SnapshotPosition = baseTransform.TransformPoint(snapshotPoseBaseTrSpacePosition);
                SnapshotRotation = baseTransform.rotation * snapshotPoseBaseTrSpaceRotationB;
            }

            /// <summary>
            /// Taking reference pose snapshot
            /// </summary>
            public void TakePoseSnapshot(Transform targetSpace, List<SpineBone> bones, int index)
            {
                baseTransform = targetSpace;
                snapshotPoseBaseTrSpacePosition = targetSpace.InverseTransformPoint(transform.position);

                // Target direction position for different conditions
                Vector3 targetPosF;
                Vector3 targetPosB;

                if (index == bones.Count - 1)
                {
                    Vector3 backDir = targetSpace.InverseTransformPoint(transform.position) - targetSpace.InverseTransformPoint(bones[index - 1].transform.position);
                    targetPosF = snapshotPoseBaseTrSpacePosition + backDir;
                    targetPosB = targetSpace.InverseTransformPoint(bones[index - 1].transform.position);
                }
                else if (index == 0)
                {
                    targetPosF = targetSpace.InverseTransformPoint(bones[index + 1].transform.position);

                    Vector3 backDir = targetSpace.InverseTransformPoint(transform.position) - targetSpace.InverseTransformPoint(bones[index + 1].transform.position);
                    targetPosB = snapshotPoseBaseTrSpacePosition + backDir;
                }
                else
                {
                    targetPosF = targetSpace.InverseTransformPoint(bones[index + 1].transform.position);
                    targetPosB = targetSpace.InverseTransformPoint(bones[index - 1].transform.position);
                }

                snapshotPoseBaseTrSpaceRotationF = Quaternion.Inverse(targetSpace.rotation) * Quaternion.LookRotation(targetPosF - snapshotPoseBaseTrSpacePosition);// Quaternion.Inverse(targetSpace.rotation) * transform.rotation;
                snapshotPoseBaseTrSpaceRotationB = Quaternion.Inverse(targetSpace.rotation) * Quaternion.LookRotation(targetPosB - snapshotPoseBaseTrSpacePosition);// Quaternion.Inverse(targetSpace.rotation) * transform.rotation;
                snapshotPoseLocalRotation = Quaternion.Inverse(targetSpace.rotation) * transform.rotation;
            }


            #endregion


        }
    }
}