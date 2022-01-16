using UnityEngine;

namespace FIMSpace.FSpine
{
    public partial class FSpineAnimator
    {
        public partial class SpineBone
        {
            public bool Collide = true;
            public float CollisionRadius = 1f;
            public Vector3 ColliderOffset = Vector3.zero;

            public float GetCollisionRadiusScaled()
            {
                return CollisionRadius * transform.lossyScale.x;
            }
        }
    }
}