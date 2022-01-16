using System;
using UnityEngine;

namespace FIMSpace.FSpine
{
    public partial class FSpineAnimator
    {
        void PreCalibrateBones()
        {
            for (int i = 0; i < SpineBones.Count; i++)
            {
                SpineBones[i].transform.localPosition = SpineBones[i].InitialLocalPosition;
                SpineBones[i].transform.localRotation = SpineBones[i].InitialLocalRotation;
            }

            if (BonesFixers.Count > 0) for (int i = 0; i < BonesFixers.Count; i++) BonesFixers[i].Calibration();
        }

        /// <summary>
        /// Preparing bones for animation
        /// </summary>
        void CalibrateBones()
        {
            if (BonesFixers.Count > 0) for (int i = 0; i < BonesFixers.Count; i++) BonesFixers[i].UpdateOnAnimator();

            // Updating bone connectors before spine motion
            if (connectors != null) for (int i = 0; i < connectors.Count; i++) connectors[i].RememberAnimatorState();

            ModelForwardAxisScaled = Vector3.Scale(ModelForwardAxis, BaseTransform.localScale);
            ModelUpAxisScaled = Vector3.Scale(ModelUpAxis, BaseTransform.localScale);
            //ModelRightAxisScaled = Vector3.Scale(ModelRightAxis, BaseTransform.localScale);
        }


        /// <summary>
        /// Just defining delta time for component operations
        /// </summary>
        void DeltaTimeCalculations()
        {
            switch (DeltaType)
            {
                case EFDeltaType.SafeDelta: delta = Mathf.Lerp(delta, GetClampedSmoothDelta(), 0.05f); break;
                case EFDeltaType.DeltaTime: delta = Time.deltaTime; break;
                case EFDeltaType.SmoothDeltaTime: delta = Time.smoothDeltaTime; break;
                case EFDeltaType.UnscaledDeltaTime: delta = Time.unscaledDeltaTime; break;
                case EFDeltaType.FixedDeltaTime: delta = Time.fixedDeltaTime; break;
            }

            unifiedDelta = Mathf.Pow(delta, 0.1f) * 0.04f;
        }


        /// <summary>
        /// Calculating how many update loops should be done in this frame according to target update rate and elapsed deltaTime
        /// </summary>
        void StableUpdateRateCalculations()
        {
            updateLoops = 1;

            float targetDelta = 1.0f / UpdateRate;
            elapsedDeltaHelper += delta;
            updateLoops = 0;

            while (elapsedDeltaHelper >= targetDelta)
            {
                elapsedDeltaHelper -= targetDelta;
                if (++updateLoops >= 3) { elapsedDeltaHelper = 0; break; }
            }
        }


        /// <summary>
        /// Applying new coordinates for transforms and refreshing hierarchy
        /// </summary>
        void ApplyNewBonesCoordinates()
        {
            // Set new coords without blending to keyframed animation
            if (SpineAnimatorAmount >= 1f)
            {
                SpineBones[leadingBoneIndex].transform.position = SpineBones[leadingBoneIndex].FinalPosition;
                SpineBones[leadingBoneIndex].transform.rotation = SpineBones[leadingBoneIndex].FinalRotation;

                for (int i = 1 - chainIndexOffset; i < SpineBones.Count - chainIndexOffset; i++)
                {
                    SpineBones[i].transform.position = SpineBones[i].FinalPosition;
                    SpineBones[i].transform.rotation = SpineBones[i].FinalRotation;

                    SpineBones[i].RefreshFinalLocalPose();
                }

                SpineBones[leadingBoneIndex].RefreshFinalLocalPose();

            }
            else // Blending amount of spine animator motion with keyframed animation
            {
                SpineBones[leadingBoneIndex].transform.position = Vector3.LerpUnclamped(SpineBones[leadingBoneIndex].transform.position, SpineBones[leadingBoneIndex].FinalPosition, SpineAnimatorAmount * SpineBones[leadingBoneIndex].MotionWeight);
                SpineBones[leadingBoneIndex].transform.rotation = Quaternion.LerpUnclamped(SpineBones[leadingBoneIndex].transform.rotation, SpineBones[leadingBoneIndex].FinalRotation, SpineAnimatorAmount * SpineBones[leadingBoneIndex].MotionWeight);

                for (int i = 1 - chainIndexOffset; i < SpineBones.Count - chainIndexOffset; i++)
                {
                    SpineBones[i].transform.position = Vector3.LerpUnclamped(SpineBones[i].transform.position, SpineBones[i].FinalPosition, SpineAnimatorAmount * SpineBones[i].MotionWeight);
                    SpineBones[i].transform.rotation = Quaternion.LerpUnclamped(SpineBones[i].transform.rotation, SpineBones[i].FinalRotation, SpineAnimatorAmount * SpineBones[i].MotionWeight);

                    SpineBones[i].RefreshFinalLocalPose();
                }

                SpineBones[leadingBoneIndex].RefreshFinalLocalPose();

            }

        }


        /// <summary>
        /// Last chain of update order for Spine Animator
        /// </summary>
        void EndUpdate()
        {
            previousPos = SpineBones[leadingBoneIndex].ProceduralPosition;//RoundPosDiff(SpineBones[leadingBoneIndex].ProceduralPosition);

            // Syncing bone connectors after spine motion
            if (connectors != null) for (int i = 0; i < connectors.Count; i++) connectors[i].RefreshAnimatorState();
            if (BonesFixers.Count > 0) for (int i = 0; i < BonesFixers.Count; i++) BonesFixers[i].UpdateAfterProcedural();
        }
    }
}