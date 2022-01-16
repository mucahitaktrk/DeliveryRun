using UnityEngine;

namespace FIMSpace.FSpine
{
    /// <summary>
    /// FM: Just simple class for drawing skeleton hierarchy inside unity editor scene window, helpful for debugging etc.
    /// </summary>
    [AddComponentMenu("FImpossible Creations/Spine Animator Utilities/FSpine Draw Skeleton")]
    public class FSpine_DrawSkeleton : MonoBehaviour
    {
#if UNITY_EDITOR
        public Transform RootBone;
        [Range(0f, 1f)]
        public float BoneStartRadius = 0.035f;
        [Range(0f, 1f)]
        public float OrientationsRayLength = 0f;
        public bool Coloful = false;
        [Range(1f, 15f)] public float ColorBias = 3f;
        [Range(0f, 1f)] public float Alpha = 0.7f;

        public bool Draw = true;

        private int recurrention = 0;
        private int recurrentionSegment = 0;


        void OnDrawGizmos()
        {
            if (!Draw) return;

            Color preCol = Gizmos.color;

            recurrention = 0;
            recurrentionSegment = 0;
            if (!RootBone) RootBone = transform;
            DrawBonesRecurent(RootBone);

            Gizmos.color = preCol;
        }

        public void DrawBonesRecurent(Transform start)
        {
            if (!Coloful)
                Gizmos.color = new Color(0.3f, 1f, 0.3f, Alpha);
            else
            {
                Color newCol = Color.HSVToRGB(0.5f + Mathf.Sin((float)recurrentionSegment / ColorBias) / 2f, 0.7f, 0.8f);
                newCol.a = Alpha;
                Gizmos.color = newCol;
            }

            if (UnityEditor.Selection.Contains(start.gameObject))
            {
                Gizmos.color = new Color(1f, 1f, 1f, Alpha * 1.5f);
                Gizmos.DrawWireSphere(start.position, BoneStartRadius);
            }

            recurrention++;

            for (int i = 0; i < start.childCount; i++)
            {
                DrawBoneLine(start.position, start.GetChild(i).position);
                recurrentionSegment++;

                if (OrientationsRayLength > 0f)
                {
                    Color preCol = Gizmos.color;

                    Gizmos.color = Color.blue;
                    Gizmos.DrawRay(start.position, start.forward * OrientationsRayLength);

                    Gizmos.color = Color.red;
                    Gizmos.DrawRay(start.position, start.right * OrientationsRayLength);

                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(start.position, start.up * OrientationsRayLength);

                    Gizmos.color = preCol;
                }

                DrawBonesRecurent(start.GetChild(i));
            }
        }

        private void DrawBoneLine(Vector3 origin, Vector3 pos)
        {
            Gizmos.DrawLine(origin + Vector3.forward * BoneStartRadius, pos);
            Gizmos.DrawLine(origin - Vector3.forward * BoneStartRadius, pos);
            Gizmos.DrawLine(origin - Vector3.right * BoneStartRadius, pos);
            Gizmos.DrawLine(origin + Vector3.right * BoneStartRadius, pos);
            Gizmos.DrawLine(origin + Vector3.up * BoneStartRadius, pos);
            Gizmos.DrawLine(origin - Vector3.up * BoneStartRadius, pos);
            Gizmos.DrawLine(origin, pos);

            Gizmos.DrawLine(origin + Vector3.forward * BoneStartRadius, origin);
            Gizmos.DrawLine(origin - Vector3.forward * BoneStartRadius, origin);
            Gizmos.DrawLine(origin - Vector3.right * BoneStartRadius, origin);
            Gizmos.DrawLine(origin + Vector3.right * BoneStartRadius, origin);
            Gizmos.DrawLine(origin + Vector3.up * BoneStartRadius, origin);
            Gizmos.DrawLine(origin - Vector3.up * BoneStartRadius, origin);
        }

#endif

    }
}
