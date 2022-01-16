using UnityEngine;

namespace FIMSpace.FSpine
{
    public partial class FSpineAnimator
    {
        [FPD_Percentage(0f, 1f)]
        [Tooltip("You can use this variable to blend intensity of spine animator motion over skeleton animation\n\nValue = 1: Animation with spine Animator motion\nValue = 0: Only skeleton animation")]
        public float SpineAnimatorAmount = 1f;

        Quaternion Rotate180 = Quaternion.Euler(0f, 180f, 0f);
        //float scaleReference = 1f;
        //float smallScaleReference = 0.01f;


        /// <summary>
        /// Calculating spine-like movement animation logic for given transforms list
        /// </summary>
        void CalculateBonesCoordinates()
        {
            //scaleReference = (SpineBones[0].transform.position - SpineBones[SpineBones.Count - 1].transform.position).magnitude;

            if (LastBoneLeading)
            {
                // Predicted position -> rotation to predicted position then final position calculations? with assigning?
                for (int i = SpineBones.Count - 2; i >= 0; i--)
                {
                    CalculateTargetBoneRotation(i);
                    CalculateTargetBonePosition(i);

                    SpineBones[i].CalculateDifferencePose(ModelUpAxis, ModelRightAxis);
                    SpineBones[i].ApplyDifferencePose();
                }
            }
            else
            {
                for (int i = 1; i < SpineBones.Count; i++)
                {
                    CalculateTargetBoneRotation(i);
                    CalculateTargetBonePosition(i);

                    SpineBones[i].CalculateDifferencePose(ModelUpAxis, ModelRightAxis);
                    SpineBones[i].ApplyDifferencePose();
                }
            }
        }



        /// <summary>
        /// Calculation target animation position for bone
        /// </summary>
        void CalculateTargetBonePosition(int index)
        {
            SpineBone otherBone = SpineBones[index - chainIndexDirection];
            SpineBone currentBone = SpineBones[index];

            // Target position calculation
            Vector3 targetPosition = otherBone.ProceduralPosition - (currentBone.ProceduralRotation * ModelForwardAxisScaled) * (currentBone.BoneLength * DistancesMultiplier);
            if (currentBone.Collide) targetPosition += gravityScale;


            #region Springiness Stuff

            if (Springiness > 0f)
            {
                if (!LastBoneLeading)
                {
                    Vector3 backPosDiff = currentBone.ProceduralPosition - currentBone.PreviousPosition;
                    Vector3 newPos = currentBone.ProceduralPosition;
                    currentBone.PreviousPosition = currentBone.ProceduralPosition;
                    newPos += backPosDiff * (1 - Mathf.Lerp(.05f, .25f, Springiness));

                    float restDistance = (otherBone.ProceduralPosition - newPos).magnitude;

                    Matrix4x4 otherLocalToWorld = otherBone.transform.localToWorldMatrix;
                    otherLocalToWorld.SetColumn(3, otherBone.ProceduralPosition);
                    Vector3 restPos = otherLocalToWorld.MultiplyPoint3x4(currentBone.transform.localPosition);

                    Vector3 diffPosVector = restPos - newPos;
                    newPos += diffPosVector * Mathf.Lerp(0.05f, 0.2f, Springiness);

                    diffPosVector = restPos - newPos;
                    float distance = diffPosVector.magnitude;
                    float maxDistance = restDistance * (1 - Mathf.Lerp(0.0f, 0.2f, Springiness)) * 2;
                    if (distance > maxDistance) newPos += diffPosVector * ((distance - maxDistance) / distance);

                    if (MaxStretching < 1f)
                    {
                        float dist = Vector3.Distance(currentBone.ProceduralPosition, newPos);
                        if (dist > 0f)
                        {
                            float maxDist = currentBone.BoneLength * 4 * MaxStretching;
                            if (dist > maxDist) newPos = Vector3.Lerp(newPos, targetPosition, Mathf.InverseLerp(dist, 0f, maxDist));
                        }
                    }

                    targetPosition = Vector3.Lerp(targetPosition, newPos, Mathf.Lerp(0.3f, 0.9f, Springiness));
                }
            }

            #endregion


            // Target Position Stretching Prevention
            if (PosSmoother > 0f)
                if (MaxStretching < 1f)
                {
                    float dist = Vector3.Distance(currentBone.ProceduralPosition, targetPosition);
                    if (dist > 0f)
                    {
                        float maxDist = currentBone.BoneLength * 4 * MaxStretching;
                        if (dist > maxDist) currentBone.ProceduralPosition = Vector3.Lerp(currentBone.ProceduralPosition, targetPosition, Mathf.InverseLerp(dist, 0f, maxDist));
                    }
                }


            if (UseCollisions) if (currentBone.Collide) PushIfSegmentInsideCollider(currentBone, ref targetPosition);

            // Smoothing Position
            if (PosSmoother == 0f)
                currentBone.ProceduralPosition = targetPosition;
            else
                currentBone.ProceduralPosition = Vector3.LerpUnclamped(currentBone.ProceduralPosition, targetPosition, Mathf.LerpUnclamped(1f, unifiedDelta, PosSmoother));
        }


        /// <summary>
        /// Calculating rotation with limitations and stuff for single segment
        /// </summary>
        void CalculateTargetBoneRotation(int index)
        {
            // Preparation for calculations
            SpineBone otherBone = SpineBones[index - chainIndexDirection];
            SpineBone currentBone = SpineBones[index];

            Quaternion targetLookRotation, backRotationRef;


            // Slithery calculation
            if (Slithery >= 1f)
                backRotationRef = otherBone.ProceduralRotation;
            else
            if (Slithery > 0f) backRotationRef = Quaternion.LerpUnclamped(currentBone.ReferenceRotation, otherBone.ProceduralRotation, Slithery);
            else
                backRotationRef = currentBone.ReferenceRotation;


            // Initial target rotation calculation
            Vector3 towards = otherBone.ProceduralPosition - currentBone.ProceduralPosition;
            if (towards == Vector3.zero) towards = currentBone.transform.rotation * currentBone.DefaultForward;
            targetLookRotation = Quaternion.LookRotation(towards, otherBone.ProceduralRotation * ModelUpAxis);


            #region Calculations to limit rotations in order to adjust animation behaviour for project needs

            if (AngleLimit < 91)
            {
                float lookDiff = Quaternion.Angle(targetLookRotation, backRotationRef);

                // Limiting rotation to correct state with elastic range
                if (lookDiff > AngleLimit)
                {
                    float limiting = 0f;
                    limiting = Mathf.InverseLerp(0f, lookDiff, lookDiff - AngleLimit);

                    Quaternion limitRange = Quaternion.LerpUnclamped(targetLookRotation, backRotationRef, limiting);

                    float elasticPush = Mathf.Min(1f, lookDiff / (AngleLimit / 0.75f));
                    elasticPush = Mathf.Sqrt(Mathf.Pow(elasticPush, 4)) * elasticPush; // sqrt and power will make this value increase slower but reaching 1f at the end

                    if (LimitSmoother == 0f)
                        targetLookRotation = Quaternion.LerpUnclamped(targetLookRotation, limitRange, elasticPush);
                    else
                        //targetLookRotation = Quaternion.LerpUnclamped(targetLookRotation, limitRange, Mathf.LerpUnclamped(1f, unifiedDelta * elasticPush, LimitSmoother));
                        targetLookRotation = Quaternion.LerpUnclamped(targetLookRotation, limitRange, unifiedDelta * (1f - LimitSmoother) * 50f * elasticPush);
                }
            }

            if (GoBackSpeed <= 0f)
            {
                // When position in previous frame was different, we straigtening a little rotation of spine
                if (StraightenSpeed > 0f)
                {
                    //if (previousPos != RoundPosDiff(SpineBones[leadingBoneIndex].ProceduralPosition))
                    //    currentBone.TargetStraightenFactor = 1f;
                    //else
                    //    if (currentBone.TargetStraightenFactor > 0f)
                    //    currentBone.TargetStraightenFactor -= delta * (5f + StraightenSpeed );

                    //float baseF = currentBone.BoneLength * delta - (previousPos - SpineBones[leadingBoneIndex].ProceduralPosition).magnitude;
                    //currentBone.TargetStraightenFactor = baseF;
                    //if (currentBone.TargetStraightenFactor < 0f) currentBone.TargetStraightenFactor = -currentBone.TargetStraightenFactor;
                    //currentBone.TargetStraightenFactor *= 4f;
                    //if (currentBone.TargetStraightenFactor > 1f) currentBone.TargetStraightenFactor = 1f;

                    float diff = (currentBone.ReferencePosition - currentBone.PreviousReferencePosition).magnitude / currentBone.GetUnscalledBoneLength();
                    if (diff > 0.5f) diff = 0.5f;
                    float target = diff * (1f + StraightenSpeed / 5f);
                    
                    currentBone.StraightenFactor = Mathf.Lerp(currentBone.StraightenFactor, target, unifiedDelta * (7f + StraightenSpeed));

                    if (diff > 0.0001f)
                    {
                        targetLookRotation = Quaternion.Lerp(targetLookRotation, backRotationRef, unifiedDelta * currentBone.StraightenFactor * (StraightenSpeed + 5f) * (TurboStraighten ? 6f : 1f));
                    }
                }
            }
            else // When we set GoBackSpeed variable spine is going back to straight pose continously so diff would be detected all the time and we don't want this
            {
                // If we use straigtening at the same time when using GoBack variable
                float straightenVal = 0f;

                // When position in previous frame was different, we straigtening a little rotation of spine
                if (StraightenSpeed > 0f)
                {
                    if (previousPos != RoundPosDiff(SpineBones[leadingBoneIndex].ProceduralPosition))
                        currentBone.TargetStraightenFactor = 1f;
                    else
                        if (currentBone.TargetStraightenFactor > 0f)
                        currentBone.TargetStraightenFactor -= delta * (5f + StraightenSpeed);

                    //currentBone.StraightenFactor = Mathf.Lerp(currentBone.StraightenFactor, currentBone.TargetStraightenFactor, Mathf.LerpUnclamped(unifiedDelta, 1f, StraightenSpeed / 15f));
                    currentBone.StraightenFactor = Mathf.Lerp(currentBone.StraightenFactor, currentBone.TargetStraightenFactor, unifiedDelta * (1f + StraightenSpeed));

                    if (currentBone.StraightenFactor > 0.025f)
                        straightenVal = currentBone.StraightenFactor * StraightenSpeed * (TurboStraighten ? 6f : 1f);
                }

                //targetLookRotation = Quaternion.Lerp(targetLookRotation, backRotationRef, Mathf.LerpUnclamped(unifiedDelta + straightenVal, 1f, GoBackSpeed));
                targetLookRotation = Quaternion.Lerp(targetLookRotation, backRotationRef, unifiedDelta * (Mathf.Lerp(0f, 55f, GoBackSpeed) + straightenVal));
            }

            #endregion


            // If we want some smooth motion for follower
            if (RotSmoother == 0f) currentBone.ProceduralRotation = targetLookRotation; else currentBone.ProceduralRotation = Quaternion.LerpUnclamped(currentBone.ProceduralRotation, targetLookRotation, Mathf.LerpUnclamped(0f, Mathf.LerpUnclamped(1f, unifiedDelta, RotSmoother), MotionInfluence));
        }


        /// <summary>
        /// Updating pointers for reversed and basic spine lead direction
        /// </summary>
        void UpdateChainIndexHelperVariables()
        {
            if (chainReverseFlag != LastBoneLeading)
            {
                chainReverseFlag = LastBoneLeading;

                if (LastBoneLeading)
                {
                    leadingBoneIndex = SpineBones.Count - 1;
                    chainIndexDirection = -1;
                    chainIndexOffset = 1;
                    headBone = backHead;
                }
                else
                {
                    leadingBoneIndex = 0;
                    chainIndexDirection = 1;
                    chainIndexOffset = 0;
                    headBone = frontHead;
                }

                if (LastBoneLeading)
                {
                    for (int i = 0; i < SpineBones.Count; i++)
                        SpineBones[i].SetDistanceForFrameBackward();
                }
                else
                {
                    for (int i = 0; i < SpineBones.Count; i++)
                        SpineBones[i].SetDistanceForFrameForward();
                }

            }
        }


        /// <summary>
        /// Creating flat forward reference pose for animation correct spine motion
        /// </summary>
        void RefreshReferencePose()
        {
            if (HeadAnchor) if (!AnimateAnchor) SpineBones[leadingBoneIndex].transform.localRotation = SpineBones[leadingBoneIndex].InitialLocalRotation;


            if (LastBoneLeading) // Head bone is last bone in chain
            {
                headBone.SetCoordsForFrameBackward();

                if (!HeadAnchor)
                {
                    SpineBones[leadingBoneIndex].UpdateReferencePosition(headBone.SnapshotPosition);
                    SpineBones[leadingBoneIndex].ReferenceRotation = BaseTransform.rotation;
                }
                else
                {
                    SpineBones[leadingBoneIndex].UpdateReferencePosition(headBone.transform.position);
                    SpineBones[leadingBoneIndex].ReferenceRotation = BaseTransform.rotation;
                    //SpineBones[leadingBoneIndex].ReferenceRotation = headBone.GetLocalRotationDiff();
                }

                if (LeadBoneRotationOffset.sqrMagnitude != 0f) if (LeadBoneOffsetReference) SpineBones[leadingBoneIndex].ReferenceRotation *= Quaternion.Euler(LeadBoneRotationOffset);
                if (ReverseForward) SpineBones[leadingBoneIndex].ReferenceRotation *= Rotate180;

                for (int i = SpineBones.Count - 2; i >= 0; i--)
                {
                    SpineBones[i].ReferenceRotation = SpineBones[i + 1].ReferenceRotation;
                    SpineBones[i].UpdateReferencePosition(SpineBones[i + 1].ReferencePosition - (SpineBones[i].ReferenceRotation * ModelForwardAxis * (SpineBones[i].BoneLength * DistancesMultiplier * BaseTransform.lossyScale.x)));
                }
            }
            else // Head anchor bone is first bone in chain
            {
                headBone.SetCoordsForFrameForward();

                if (!HeadAnchor)
                {
                    SpineBones[leadingBoneIndex].UpdateReferencePosition(headBone.SnapshotPosition);
                    SpineBones[leadingBoneIndex].ReferenceRotation = BaseTransform.rotation;
                }
                else
                {
                    SpineBones[leadingBoneIndex].UpdateReferencePosition(headBone.transform.position);
                     SpineBones[leadingBoneIndex].ReferenceRotation = headBone.GetLocalRotationDiff();
                }

                if (LeadBoneRotationOffset.sqrMagnitude != 0f) if (LeadBoneOffsetReference) SpineBones[leadingBoneIndex].ReferenceRotation *= Quaternion.Euler(LeadBoneRotationOffset);
                if (ReverseForward) SpineBones[leadingBoneIndex].ReferenceRotation *= Rotate180;

                for (int i = 1; i < SpineBones.Count; i++)
                {
                    SpineBones[i].ReferenceRotation = SpineBones[i - 1].ReferenceRotation;
                    SpineBones[i].UpdateReferencePosition(SpineBones[i - 1].ReferencePosition - (SpineBones[i].ReferenceRotation * ModelForwardAxis * (SpineBones[i].BoneLength * DistancesMultiplier * BaseTransform.lossyScale.x)));
                }
            }


        }


        /// <summary>
        /// Restraightening spine motion pose
        /// </summary>
        void ReposeSpine()
        {
            UpdateChainIndexHelperVariables();
            RefreshReferencePose();

            for (int i = 0; i < SpineBones.Count; i++)
            {
                SpineBones[i].ProceduralPosition = SpineBones[i].ReferencePosition;
                SpineBones[i].ProceduralRotation = SpineBones[i].ReferenceRotation;

                SpineBones[i].PreviousPosition = SpineBones[i].ReferencePosition;

                SpineBones[i].FinalPosition = SpineBones[i].ReferencePosition;
                SpineBones[i].FinalRotation = SpineBones[i].ReferenceRotation;
            }
        }

        /// <summary>
        /// Updating head bone coordinates before other bones motion calculation
        /// </summary>
        void BeginBaseBonesUpdate()
        {
            if (HeadAnchor != null)
            {
                SpineBones[leadingBoneIndex].ProceduralRotation = headBone.GetLocalRotationDiff();
                SpineBones[leadingBoneIndex].ProceduralPosition = SpineBones[leadingBoneIndex].transform.position;
            }
            else // No anchor - default operation
            {
                SpineBones[leadingBoneIndex].ProceduralPosition = SpineBones[leadingBoneIndex].ReferencePosition;
                SpineBones[leadingBoneIndex].ProceduralRotation = SpineBones[leadingBoneIndex].ReferenceRotation;
            }

            if (LeadBoneRotationOffset.sqrMagnitude != 0f) if (!LeadBoneOffsetReference) SpineBones[leadingBoneIndex].ProceduralRotation *= Quaternion.Euler(LeadBoneRotationOffset);
            //if (ReverseForward) SpineBones[leadingBoneIndex].ProceduralRotation *= Rotate180;

            // Diff pose calculation for leading bone
            SpineBones[leadingBoneIndex].CalculateDifferencePose(ModelUpAxis, ModelRightAxis);
            SpineBones[leadingBoneIndex].ApplyDifferencePose();
        }

    }
}