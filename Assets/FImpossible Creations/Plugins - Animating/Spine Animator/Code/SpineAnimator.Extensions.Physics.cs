using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FIMSpace.FSpine
{
    public partial class FSpineAnimator
    {
        bool collisionInitialized = false;
        bool forceRefreshCollidersData = false;

        /// <summary>
        /// Initial operations for handling collisions
        /// </summary>
        void BeginPhysicsUpdate()
        {
            gravityScale = GravityPower * delta;

            if (!UseCollisions) return;

            if (!collisionInitialized) InitColliders();
            else
                RefreshCollidersDataList();

            // Letting every tail segment check only enabled colliders by game object
            CollidersDataToCheck.Clear();

            for (int i = 0; i < IncludedCollidersData.Count; i++)
            {
                if (IncludedCollidersData[i].Collider == null) { forceRefreshCollidersData = true; break; }
                if (IncludedCollidersData[i].Collider.gameObject.activeInHierarchy)
                {
                    IncludedCollidersData[i].RefreshColliderData();
                    CollidersDataToCheck.Add(IncludedCollidersData[i]);
                }
            }
        }



        #region Editor Only

#if UNITY_EDITOR

        void _Gizmos_DrawColliders()
        {
            if (_Editor_Category != EFSpineEditorCategory.Physical) return;

            if (UseCollisions)
            {
                Color c = Gizmos.color; Color sphColor;
                int strt = 0; int cnt = SpineBones.Count;
                if (!LastBoneLeading) strt = 1; else cnt -= 1;
                float al = Application.isPlaying ? al = 0.265f : 0.2f;

                // True Positions
                sphColor = new Color(1f, 0f, 1f, 1f); if (!UseTruePosition) sphColor = new Color(1f, 0f, 1f, al);
                Gizmos.color = sphColor;

                for (int i = strt; i < cnt; i++)
                {
                    if (!SpineBones[i].Collide) Gizmos.color *= new Color(1f, 1f, 1f, 0.2f);
                    Vector3 pos = SpineBones[i].transform.position + SpineBones[i].transform.TransformVector(SpineBones[i].ColliderOffset + OffsetAllColliders);
                    Gizmos.DrawWireSphere(pos, GetColliderSphereRadiusFor(i));
                    Gizmos.color = sphColor;
                }


                // Procedural Positions
                sphColor = new Color(.2f, 1f, .2f, 1f); if (UseTruePosition) sphColor *= new Color(.2f, 1f, .2f, al);
                Gizmos.color = sphColor;

                for (int i = strt; i < cnt; i++)
                {
                    if (!SpineBones[i].Collide) Gizmos.color *= new Color(1f, 1f, 1f, 0.2f);
                    Vector3 pos = SpineBones[i].ProceduralPosition + SpineBones[i].transform.TransformVector(SpineBones[i].ColliderOffset + OffsetAllColliders);
                    Gizmos.DrawWireSphere(pos, GetColliderSphereRadiusFor(i));
                    Gizmos.color = sphColor;
                }

                Gizmos.color = c;
            }
        }

#endif

        #endregion



        /// <summary>
        /// Refreshing colliders data for included colliders
        /// </summary>
        public void RefreshCollidersDataList()
        {
            if (IncludedColliders.Count != IncludedCollidersData.Count || forceRefreshCollidersData)
            {
                IncludedCollidersData.Clear();

                for (int i = IncludedColliders.Count - 1; i >= 0; i--)
                {
                    if (IncludedColliders[i] == null)
                    {
                        IncludedColliders.RemoveAt(i);
                        continue;
                    }

                    FImp_ColliderData_Base colData = FImp_ColliderData_Base.GetColliderDataFor(IncludedColliders[i]);
                    IncludedCollidersData.Add(colData);
                }

                forceRefreshCollidersData = false;
            }
        }


        /// <summary>
        /// Calculating automatically scale for colliders on tail, which will be automatically assigned after initialization
        /// </summary>
        float GetColliderSphereRadiusFor(int i)
        {
            int backBone = i - 1;
            if (LastBoneLeading)
            {
                if (i == SpineBones.Count - 1) return 0f;
                backBone = i + 1;
            }
            else
                if (i == 0) return 0f;

            float refDistance = 1f;
            if (SpineBones.Count > 1) refDistance = Vector3.Distance(SpineBones[1].transform.position, SpineBones[0].transform.position);

            float singleScale = Mathf.Lerp(refDistance, (SpineBones[i].transform.position - SpineBones[backBone].transform.position).magnitude * 0.5f, DifferenceScaleFactor);
            float div = SpineBones.Count - 1;
            if (div <= 0f) div = 1f;
            float step = 1f / div;

            return 0.5f * singleScale * CollidersScaleMul * CollidersScale.Evaluate(step * (float)i);
        }


        /// <summary>
        /// Adding collider to included colliders list
        /// </summary>
        public void AddCollider(Collider collider)
        {
            if (IncludedColliders.Contains(collider)) return;
            IncludedColliders.Add(collider);
        }


        /// <summary>
        /// Initializing collider helper list
        /// </summary>
        void InitColliders()
        {
            for (int i = 0; i < SpineBones.Count; ++i)
            {
                SpineBones[i].CollisionRadius = GetColliderSphereRadiusFor(i);
            }

            IncludedCollidersData = new List<FImp_ColliderData_Base>();
            RefreshCollidersDataList();

            collisionInitialized = true;
        }

        /// <summary>
        /// Checking if colliders list don't have duplicates
        /// </summary>
        public void CheckForColliderDuplicates()
        {
            for (int i = 0; i < IncludedColliders.Count; i++)
            {
                Collider col = IncludedColliders[i];
                int count = IncludedColliders.Count(o => o == col);

                if (count > 1)
                {
                    IncludedColliders.RemoveAll(o => o == col);
                    IncludedColliders.Add(col);
                }
            }
        }


        /// <summary>
        /// Pushing spine segment from detected collider
        /// </summary>
        public void PushIfSegmentInsideCollider(SpineBone bone, ref Vector3 targetPoint)
        {
            // We must translate phantom/reference skeleton positions to true position in world space for collisions
            Vector3 offset;

            if (UseTruePosition)
            {
                Vector3 theTarget = targetPoint;
                Vector3 truePosition = bone.FinalPosition;
                offset = truePosition - theTarget + bone.transform.TransformVector(bone.ColliderOffset + OffsetAllColliders);
            }
            else
                offset = bone.transform.TransformVector(bone.ColliderOffset + OffsetAllColliders);

            if (!DetailedCollision)
            {
                for (int i = 0; i < CollidersDataToCheck.Count; i++)
                    if (CollidersDataToCheck[i].PushIfInside(ref targetPoint, bone.GetCollisionRadiusScaled(), offset)) return;
            }
            else
            {
                for (int i = 0; i < CollidersDataToCheck.Count; i++)
                    CollidersDataToCheck[i].PushIfInside(ref targetPoint, bone.GetCollisionRadiusScaled(), offset);
            }
        }


    }
}