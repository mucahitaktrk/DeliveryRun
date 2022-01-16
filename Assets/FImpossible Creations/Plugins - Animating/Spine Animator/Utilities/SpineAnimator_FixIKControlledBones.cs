using FIMSpace.FEditor;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FIMSpace.FSpine
{
    [DefaultExecutionOrder(-12)]
    [AddComponentMenu("FImpossible Creations/Spine Animator Utilities/Spine Animator IK Controlled Bone Fixer")]
    public class SpineAnimator_FixIKControlledBones : MonoBehaviour
    {
        public Transform SkeletonParentBone;
        [Tooltip("If bones are twisting with this option off you should turn it on (calibrating bone if it's animation don't use keyframes)")]
        public bool Calibrate = true;

        Quaternion initLocalRot;
        Vector3 initLocalPos;

        private void Start()
        {
            initLocalPos = transform.localPosition;
            initLocalRot = transform.localRotation;
        }

        Quaternion localRotation;
        Vector3 localPosition;

        public void Calibration()
        {
            if (!Calibrate) return;
            transform.localPosition = initLocalPos;
            transform.localRotation = initLocalRot;
        }

        public void UpdateOnAnimator()
        {
            if (!enabled) return;
            localRotation = FEngineering.QToLocal(SkeletonParentBone.rotation, transform.rotation);
            localPosition = SkeletonParentBone.InverseTransformPoint(transform.position);
        }

        public void UpdateAfterProcedural()
        {
            if (!enabled) return;
            transform.rotation = FEngineering.QToWorld(SkeletonParentBone.rotation, localRotation);
            transform.position = SkeletonParentBone.TransformPoint(localPosition);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Handles.color = new Color(0.1f, 1f, 0.2f, 0.5f);
            if (SkeletonParentBone)
            FGUI_Handles.DrawBoneHandle(SkeletonParentBone.position, transform.position);
        }
#endif

    }


#if UNITY_EDITOR
    /// <summary>
    /// FM: Editor class component to enchance controll over component from inspector window
    /// </summary>
    [UnityEditor.CanEditMultipleObjects]
    [UnityEditor.CustomEditor(typeof(SpineAnimator_FixIKControlledBones))]
    public class SpineAnimator_FixIKControlledBonesEditor : UnityEditor.Editor
    {
        public SpineAnimator_FixIKControlledBones Get { get { if (_get == null) _get = (SpineAnimator_FixIKControlledBones)target; return _get; } }
        private SpineAnimator_FixIKControlledBones _get;

        public override void OnInspectorGUI()
        {
            UnityEditor.EditorGUILayout.HelpBox("1: Add it to bone which position was controlled by IK Controls in Animation Software", UnityEditor.MessageType.Info);
            UnityEditor.EditorGUILayout.HelpBox("2: Define which bone is itended to be parent of this one (like if it is paw then  LowerLeg bone / Wrist Bone  should be parent)", UnityEditor.MessageType.Info);
            UnityEditor.EditorGUILayout.HelpBox("3: Add this object to the list under 'Tweak' tab inside SpineAnimator inspector window, find 'Bones Fixers' field", UnityEditor.MessageType.Info);

            serializedObject.Update();

            GUILayout.Space(4f);
            DrawPropertiesExcluding(serializedObject, "m_Script");

            serializedObject.ApplyModifiedProperties();

        }
    }
#endif


}
