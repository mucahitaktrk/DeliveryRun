using UnityEngine;

namespace FIMSpace.FSpine
{
    /// <summary>
    /// FM: Class to sync bones when for example legs aren't connected with spine bone or so inside your model's bones hierarchy
    /// </summary>
    [AddComponentMenu("FImpossible Creations/Spine Animator Utilities/FSpine Bone Connector")]
    public class FSpineBoneConnector : MonoBehaviour, UnityEngine.EventSystems.IDropHandler, IFHierarchyIcon
    {
        public string EditorIconPath { get { return "Spine Animator/Spine Bone Connector Icon"; } }
        public void OnDrop(UnityEngine.EventSystems.PointerEventData data) { }

        [Space(5f)]
        public FSpineAnimator TargetSpineAnimator;
        public Transform TargetBone;
        [Space(3f)]
        [FPD_Width(130)]
        public bool PositionAnimated = false;
        [FPD_Width(130)]
        public bool RotationAnimated = true;
        [Space(3f)]
        public Vector3 RotationCorrection;
        public bool Mirror = false;

        private Vector3 animatorStatePosition;
        private Quaternion animatorStateRotation;
        private Quaternion targetBoneStateRotation;

        private void Start()
        {
            if (!TargetBone)
            {
                Debug.LogError("No target bone in " + name + " for Spine Bone Connector Component!");
                Destroy(this);
            }

            if (TargetSpineAnimator)
            {
                TargetSpineAnimator.AddConnector(this);
            }
            else
            {
                Debug.LogError("No target SpineAnimator in " + name + " for Spine Bone Connector Component!");
                Destroy(this);
            }

            if (!PositionAnimated) animatorStatePosition = TargetBone.InverseTransformVector(transform.position - TargetBone.position);
            if (!RotationAnimated) animatorStateRotation = transform.localRotation;
            targetBoneStateRotation = TargetBone.localRotation;
        }

        internal void RememberAnimatorState()
        {
            if (PositionAnimated) animatorStatePosition = TargetBone.InverseTransformVector(transform.position - TargetBone.position);
            if (RotationAnimated) animatorStateRotation = transform.localRotation;
        }

        internal void RefreshAnimatorState()
        {
            if (!enabled) return;
            transform.position = TargetBone.position + TargetBone.TransformVector(animatorStatePosition);
            transform.rotation = TargetBone.rotation * (animatorStateRotation * (Mirror ? targetBoneStateRotation : Quaternion.Inverse(targetBoneStateRotation))) * Quaternion.Euler(RotationCorrection);
        }
    }


#if UNITY_EDITOR
    [UnityEditor.CanEditMultipleObjects]
    [UnityEditor.CustomEditor(typeof(FSpineBoneConnector))]
    public class FSpineBoneConnectorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            FSpineBoneConnector targetScript = (FSpineBoneConnector)target;
            DrawDefaultInspector();
            GUILayout.Space(5f);

            UnityEditor.EditorGUILayout.HelpBox("Component will try to sync this object with spine animator motion (Use this component if bone's hierarchy is wrong connected", UnityEditor.MessageType.Info);
        }
    }
#endif

}