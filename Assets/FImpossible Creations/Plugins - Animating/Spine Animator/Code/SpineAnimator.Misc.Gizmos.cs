#if UNITY_EDITOR
using UnityEngine;
using FIMSpace.FEditor;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FIMSpace.FSpine
{
    public partial class FSpineAnimator
    {
        public bool DrawGizmos = true;
        public bool DebugGizmos = false;

        #region Drawing Gizmos


        float _gizmosDist = 0.1f;
        public bool _gizmosDrawMaxDist = true;
        Vector3 _gizmosPreForw = Vector3.zero;
        Vector3 _gizmosPreUp = Vector3.zero;
        [Range(0f, 1f)]
        public float gizmosAlpha = 0.85f;
        float _editor_arrowsAlpha = 2f;
        public string _editor_Title = "Spine Animator 2";

        void OnDrawGizmos()
        {
            Color gC = Gizmos.color;
            Color hC = Handles.color;

            if (SpineBones != null)
                if (SpineBones.Count > 1) _gizmosDist = Vector3.Distance(SpineBones[0].transform.position, SpineBones[1].transform.position);

            if (_editor_arrowsAlpha < 2f) _editor_arrowsAlpha += 0.005f;
        }

        void OnDrawGizmosSelected()
        {
            if (DrawGizmos == false) return;
            if (gizmosAlpha <= 0f) return;

            Color gC = Gizmos.color;
            Color hC = Handles.color;

            if (SpineBones.Count > 1) _gizmosDist = Vector3.Distance(SpineBones[0].transform.position, SpineBones[1].transform.position);
            if (_gizmosPreForw != ModelForwardAxis || _gizmosPreUp != ModelUpAxis) _editor_arrowsAlpha = 1.5f;

            Gizmos_DrawArrowGuide();

            if (SpineBones.Count < 2)
                Gizmos_DrawSetupChain();
            else
            {
                if (Application.isPlaying)
                {
                    Gizmos_DrawBonesChainPlayMode();
                }
                else
                {
                    Gizmos_DrawBonesChainEditorMode();
                    GizmosDrawEditorHelper();
                }
            }

            Handles.color = hC;
            if (_gizmosPreForw != ModelForwardAxis || _gizmosPreUp != ModelUpAxis) _editor_arrowsAlpha = 1.5f;

            _gizmosPreUp = ModelUpAxis;
            _gizmosPreForw = ModelForwardAxis;

            Gizmos.color = gC;
            Handles.color = hC;

            _Gizmos_DrawColliders();
        }

        void GizmosDrawEditorHelper()
        {
            Handles.color = new Color(1f, 1f, 1f, 0.35f);
            Handles.DrawWireDisc(GetBaseTransform().position, GetBaseTransform().TransformDirection(ModelUpAxis), _gizmosDist);
            Vector3 fwd = GetBaseTransform().position + GetBaseTransform().TransformDirection(ModelForwardAxis.normalized) * _gizmosDist;
            Vector3 rght = GetBaseTransform().TransformDirection(Vector3.Cross(ModelForwardAxis, ModelUpAxis));
            Handles.DrawLine(GetBaseTransform().position, fwd);
            Handles.DrawLine(fwd, Vector3.LerpUnclamped(GetBaseTransform().position, fwd, 0.5f) + rght * _gizmosDist * 0.2f);
            Handles.DrawLine(fwd, Vector3.LerpUnclamped(GetBaseTransform().position, fwd, 0.5f) - rght * _gizmosDist * 0.2f);
            Handles.DrawLine(GetBaseTransform().position, GetBaseTransform().position + GetBaseTransform().TransformDirection(ModelUpAxis) * _gizmosDist);
            Handles.color = new Color(1f, 1f, 1f, 0.05f);
            Handles.DrawSolidDisc(GetBaseTransform().position, GetBaseTransform().TransformDirection(ModelUpAxis), _gizmosDist);

            if (SpineBones.Count > 1)
            {
                Handles.color = new Color(1f, 1f, 1f, 0.25f);
                Handles.DrawDottedLine(GetBaseTransform().position, SpineBones[0].transform.position, 2f);
                Handles.DrawDottedLine(GetBaseTransform().position, SpineBones[SpineBones.Count - 1].transform.position, 2f);
            }
        }


        #endregion


        void Gizmos_DrawArrowGuide()
        {
            if (SpineBones.Count <= 0) return;
            Transform LeadBone = GetHeadBone();
            Transform BaseTransform = GetBaseTransform();

            if (_editor_arrowsAlpha > 0f)
            {
                float d = Vector3.Distance(LeadBone.position, GetBaseTransform().position);
                Vector3 arrowStart = Vector3.Lerp(GetBaseTransform().position, LeadBone.position, 0.75f);

                Handles.color = new Color(0.05f, 0.225f, 1f, 0.9f * _editor_arrowsAlpha);
                FGUI_Handles.DrawArrow(GetBaseTransform().TransformDirection(ModelForwardAxis) * d * 1.25f + arrowStart, Quaternion.LookRotation(GetBaseTransform().TransformDirection(ModelForwardAxis), BaseTransform.TransformDirection(ModelUpAxis)), d * 0.8f);

                Handles.color = new Color(0.05f, 0.8f, 0.05f, 0.75f * _editor_arrowsAlpha);
                arrowStart = LeadBone.position + GetBaseTransform().TransformDirection(ModelUpAxis) * d * .95f;
                FGUI_Handles.DrawArrow(arrowStart, Quaternion.LookRotation(GetBaseTransform().TransformDirection(ModelUpAxis), GetBaseTransform().TransformDirection(ModelForwardAxis)), d * 0.7f, 4f, 0.5f);
            }

            if (_editor_arrowsAlpha > -0.1f) _editor_arrowsAlpha -= 0.01f;
        }


        void Gizmos_DrawHead(Vector3 position, Transform baseTr, Vector3 fwd, Vector3 up, float scale)
        {
            Vector3 front = position + baseTr.TransformDirection(ModelForwardAxis) * scale * 0.125f;
            Handles.DrawWireDisc(front, baseTr.TransformDirection(ModelUpAxis), scale * 0.15f);
            Handles.DrawWireDisc(front, baseTr.TransformDirection(ModelUpAxis), scale * 0.16f);
            Handles.DrawSolidDisc(front, baseTr.TransformDirection(ModelUpAxis), scale * 0.065f);

            Vector3 r = baseTr.TransformDirection(Vector3.Cross(fwd, up));
            Vector3 f = baseTr.TransformDirection(fwd);
            Handles.DrawLine(front + r * scale * 0.05f, front + r * scale * 0.2f + f * scale * 0.25f);
            Handles.DrawLine(front - r * scale * 0.05f, front - r * scale * 0.2f + f * scale * 0.25f);

            Handles.DrawLine(front + r * scale * 0.2f + f * scale * 0.25f, front + r * scale * 0.1f + f * scale * 0.28f);
            Handles.DrawLine(front - r * scale * 0.2f + f * scale * 0.25f, front - r * scale * 0.1f + f * scale * 0.28f);

            Handles.DrawLine(front + r * scale * 0.15f + f * scale * 0.17f, front + r * scale * 0.1f + f * scale * 0.2f);
            Handles.DrawLine(front - r * scale * 0.15f + f * scale * 0.17f, front - r * scale * 0.1f + f * scale * 0.2f);
        }

        //bool prepared = false;
        void Gizmos_DrawBonesChainEditorMode()
        {
            if (LastBoneLeading == false)
            {
                for (int i = 0; i < SpineBones.Count; i++) SpineBones[i].ProceduralPosition = SpineBones[i].transform.position;

                for (int i = 1; i < SpineBones.Count; i++)
                {
                    SpineBones[i].Editor_SetLength((SpineBones[i].transform.position - SpineBones[i - 1].transform.position).magnitude);
                    SpineBones[i].ProceduralRotation = Quaternion.LookRotation(SpineBones[i].transform.position - SpineBones[i - 1].transform.position, GetBaseTransform().up);

                    Vector3 dir = SpineBones[i].transform.position - SpineBones[i - 1].transform.position; Quaternion look = Quaternion.LookRotation(dir.normalized, GetBaseTransform().up);

                    // To bone position
                    SpineBones[i].ProceduralPosition = SpineBones[i - 1].ProceduralPosition + dir.normalized * SpineBones[i].BoneLength * DistancesMultiplier /* SpineBones[i].transform.lossyScale.z*/;

                    if (ManualAffectChain) SpineBones[i].ProceduralPosition += look * (SpineBones[i].ManualPosOffset) * SpineBones[i].BoneLength;
                }

                // Backrotation
                SpineBones[0].ProceduralRotation = SpineBones[1].ProceduralRotation;
                SpineBones[0].Editor_SetLength(SpineBones[1].BoneLength);

                if (!ManualAffectChain)
                    for (int i = 0; i < SpineBones.Count; i++) SpineBones[i].ProceduralPosition += SpineBones[i].ProceduralRotation * (SpineBones[i].ManualPosOffset) * SpineBones[i].BoneLength;

                for (int i = 0; i < SpineBones.Count; i++)
                    SpineBones[i].ProceduralPosition += SpineBones[i].ProceduralRotation * -SegmentsPivotOffset * SpineBones[i].BoneLength;
            }
            else
            {

                for (int i = 0; i < SpineBones.Count; i++) SpineBones[i].ProceduralPosition = SpineBones[i].transform.position;

                for (int i = SpineBones.Count - 2; i >= 0; i--)
                {
                    SpineBones[i].Editor_SetLength((SpineBones[i].transform.position - SpineBones[i + 1].transform.position).magnitude);
                    Vector3 target = SpineBones[i].transform.position - SpineBones[i + 1].transform.position;
                    SpineBones[i].ProceduralRotation = target.sqrMagnitude == 0 ? Quaternion.identity : Quaternion.LookRotation(target, GetBaseTransform().up);

                    Vector3 dir = SpineBones[i].transform.position - SpineBones[i + 1].transform.position; Quaternion look = dir.sqrMagnitude == 0 ? Quaternion.identity : Quaternion.LookRotation(dir.normalized, GetBaseTransform().up);

                    // To bone position
                    SpineBones[i].ProceduralPosition = SpineBones[i + 1].ProceduralPosition + dir.normalized * SpineBones[i].BoneLength * DistancesMultiplier /* SpineBones[i].transform.lossyScale.z*/;

                    if (ManualAffectChain) SpineBones[i].ProceduralPosition += look * (SpineBones[i].ManualPosOffset) * SpineBones[i].BoneLength;
                }

                // Backrotation
                SpineBones[SpineBones.Count - 1].ProceduralRotation = SpineBones[SpineBones.Count - 2].ProceduralRotation;
                SpineBones[SpineBones.Count - 1].Editor_SetLength(SpineBones[SpineBones.Count - 2].BoneLength);

                if (!ManualAffectChain)
                    for (int i = SpineBones.Count - 1; i >= 0; i--)
                        SpineBones[i].ProceduralPosition += SpineBones[i].ProceduralRotation * (SpineBones[i].ManualPosOffset) * SpineBones[i].BoneLength;

                for (int i = SpineBones.Count - 1; i >= 0; i--)
                    SpineBones[i].ProceduralPosition += SpineBones[i].ProceduralRotation * -SegmentsPivotOffset * SpineBones[i].BoneLength;
            }


            if (_Editor_Category != EFSpineEditorCategory.Setup) return;

            Color c = Handles.color;
            Color boneColor = new Color(0.075f, .85f, 0.3f, gizmosAlpha * 1f);
            Handles.color = boneColor;

            //if (!prepared)
            //    for (int i = 0; i < SpineBones.Count; i++)
            //        SpineBones[i].PrepareBone(GetBaseTransform(), SpineBones, i);

            //if (MainPivotOffset != Vector3.zero)
            //    Handles.matrix = Matrix4x4.Translate(GetBaseTransform().TransformDirection(MainPivotOffset));

            // Calculating preview bone chain
            if (LastBoneLeading == false)
            {
                // Drawing preview spine chain
                for (int i = 1; i < SpineBones.Count; i++)
                {
                    Handles.color = new Color(0.075f, .85f, 0.3f, gizmosAlpha * SpineBones[i].MotionWeight);
                    FGUI_Handles.DrawBoneHandle(SpineBones[i].ProceduralPosition, SpineBones[i - 1].ProceduralPosition, GetBaseTransform().TransformDirection(ModelUpAxis), 1.0f);
                }
            }
            else
            {
                // Drawing preview spine chain
                for (int i = SpineBones.Count - 2; i >= 0; i--)
                {
                    Handles.color = new Color(0.075f, .85f, 0.3f, gizmosAlpha * SpineBones[i].MotionWeight);
                    FGUI_Handles.DrawBoneHandle(SpineBones[i].ProceduralPosition, SpineBones[i + 1].ProceduralPosition, GetBaseTransform().TransformDirection(ModelUpAxis), 1.0f);
                }
            }

            Handles.color = boneColor;
            Gizmos_DrawHead(GetLeadBone().ProceduralPosition, GetBaseTransform(), ModelForwardAxis * (ReverseForward ? -1f : 1f), ModelUpAxis, _gizmosDist * 1.1f);

            Handles.matrix = Matrix4x4.identity;
            Handles.color = c;
        }


        void Gizmos_DrawBonesChainPlayMode()
        {
            if (_Editor_Category != EFSpineEditorCategory.Setup) return;

            Color c = Handles.color;
            Color boneColor = new Color(0.075f, .85f, 0.3f, gizmosAlpha * 0.2f);
            Handles.color = boneColor;

            if (LastBoneLeading == false)
            {
                Handles.color = new Color(.9f, .9f, .9f, gizmosAlpha * 0.2f);
                for (int i = 1; i < SpineBones.Count; i++) FGUI_Handles.DrawBoneHandle(SpineBones[i].ReferencePosition, SpineBones[i - 1].ReferencePosition, GetBaseTransform().TransformDirection(ModelUpAxis), 0.6f);

                for (int i = 1; i < SpineBones.Count; i++)
                {
                    Handles.color = new Color(0.075f, .85f, 0.3f, gizmosAlpha * 0.5f * SpineBones[i].MotionWeight);
                    FGUI_Handles.DrawBoneHandle(SpineBones[i].ProceduralPosition, SpineBones[i - 1].ProceduralPosition, GetBaseTransform().TransformDirection(ModelUpAxis), 1.2f);
                }
            }
            else
            {
                Handles.color = new Color(.9f, .9f, .9f, gizmosAlpha * 0.2f);
                for (int i = SpineBones.Count - 2; i >= 0; i--) FGUI_Handles.DrawBoneHandle(SpineBones[i].ReferencePosition, SpineBones[i + 1].ReferencePosition, GetBaseTransform().TransformDirection(ModelUpAxis), 0.6f);

                for (int i = SpineBones.Count - 2; i >= 0; i--)
                {
                    Handles.color = new Color(0.075f, .85f, 0.3f, gizmosAlpha * 0.5f * SpineBones[i].MotionWeight);
                    FGUI_Handles.DrawBoneHandle(SpineBones[i].ProceduralPosition, SpineBones[i + 1].ProceduralPosition, GetBaseTransform().TransformDirection(ModelUpAxis), 1.2f);
                }
            }

            if (DebugGizmos)
            {
                for (int i = 0; i < SpineBones.Count; i++)
                {
                    Gizmos.color = new Color(1f, .1f, 0.1f, gizmosAlpha * 0.75f);
                    Gizmos.DrawRay(SpineBones[i].ProceduralPosition, SpineBones[i].ProceduralRotation * Vector3.right);

                    Gizmos.color = new Color(.1f, 1f, 0.1f, gizmosAlpha * 0.75f);
                    Gizmos.DrawRay(SpineBones[i].ProceduralPosition, SpineBones[i].ProceduralRotation * Vector3.up);

                    Gizmos.color = new Color(.1f, .1f, 1f, gizmosAlpha * 0.75f);
                    Gizmos.DrawRay(SpineBones[i].ProceduralPosition, SpineBones[i].ProceduralRotation * Vector3.forward);
                }
            }

            Handles.color = boneColor;
            Gizmos_DrawHead(GetLeadBone().ProceduralPosition, GetBaseTransform(), ModelForwardAxis * (ReverseForward ? -1f : 1f), ModelUpAxis, _gizmosDist * 1.1f);

            Handles.color = c;
        }


        void Gizmos_DrawSetupChain()
        {
            Handles.color = new Color(0.1f, 1f, 0.9f, gizmosAlpha * 0.5f);
            float distRef = _gizmosDist;

            if (_gizmosChainList == null)
                _GizmosRefreshChainList();

            // Draw Target Chain Ghost
            if (_gizmosEditorStartPreview && _gizmosEditorEndPreview)
            {
                distRef = Vector3.Distance(_gizmosEditorStartPreview.transform.position, _gizmosEditorEndPreview.transform.position) / 7.5f;


                {
                    if (_gizmosChainList.Count == 0) _GizmosRefreshChainList();
                    else
                    {
                        if (_gizmosChainList.Count > 0) if (_gizmosEditorStartPreview != _gizmosChainList[0]) _GizmosRefreshChainList();
                        if (_gizmosChainList.Count > 1) if (_gizmosEditorEndPreview != _gizmosChainList[_gizmosChainList.Count - 1]) _GizmosRefreshChainList();
                    }
                }

                Handles.color = new Color(.1f, .9f, .8f, gizmosAlpha * 0.25f);
                for (int i = 1; i < _gizmosChainList.Count; i++)
                {
                    FGUI_Handles.DrawBoneHandle(_gizmosChainList[i - 1].position, _gizmosChainList[i].position, GetBaseTransform().TransformDirection(ModelUpAxis), 1.2f);
                }
            }

            Handles.color = new Color(0.1f, 1f, 0.9f, gizmosAlpha * 0.5f);

            if (_gizmosEditorStartPreview)
            {
                Handles.color = new Color(0.1f, 1f, 0.9f, gizmosAlpha * 0.1f);
                for (int i = 0; i < _gizmosEditorStartPreview.childCount; i++)
                {
                    if (_gizmosChainList.Count > 1)
                        if (_gizmosEditorStartPreview == null) continue; 
                        else if (_gizmosEditorStartPreview.childCount == 0) continue; 
                        else if (_gizmosChainList.Count < 2) continue; 
                        else if (_gizmosEditorStartPreview.GetChild(i) == _gizmosChainList[1]) continue;

                    Handles.DrawDottedLine(_gizmosEditorStartPreview.position, _gizmosEditorStartPreview.GetChild(i).position, 2f);
                    Handles.SphereHandleCap(0, _gizmosEditorStartPreview.GetChild(i).position, Quaternion.identity, distRef * 0.2f, EventType.Repaint);
                }

                Handles.color = new Color(0.1f, 1f, 0.9f, gizmosAlpha * 0.5f);
                Handles.SphereHandleCap(0, _gizmosEditorStartPreview.position, Quaternion.identity, distRef * 0.425f, EventType.Repaint);
                Handles.color = new Color(1f, 1f, 1f, gizmosAlpha * 0.5f);
                string title = _gizmosEditorStartPreview.childCount <= 2 ? "Chain Start (No Legs?)" : "Chain Start";
                Handles.Label(_gizmosEditorStartPreview.position + GetBaseTransform().TransformDirection(ModelUpAxis) * _gizmosDist, new GUIContent(title), FGUI_Resources.HeaderStyle);
            }

            if (_gizmosEditorEndPreview)
            {
                Handles.color = new Color(0.1f, 1f, 0.9f, gizmosAlpha * 0.1f);
                for (int i = 0; i < _gizmosEditorEndPreview.childCount; i++)
                {
                    if (_gizmosChainList.Count > 2) if (_gizmosEditorEndPreview.GetChild(i) == _gizmosChainList[_gizmosChainList.Count - 2]) continue;
                    Handles.DrawDottedLine(_gizmosEditorEndPreview.position, _gizmosEditorEndPreview.GetChild(i).position, 2f);
                    Handles.SphereHandleCap(0, _gizmosEditorEndPreview.GetChild(i).position, Quaternion.identity, distRef * 0.2f, EventType.Repaint);
                }

                Handles.color = new Color(0.15f, 1f, 0.85f, gizmosAlpha * 0.5f);
                Handles.SphereHandleCap(0, _gizmosEditorEndPreview.position, Quaternion.identity, distRef * 0.425f, EventType.Repaint);
                Handles.color = new Color(1f, 1f, 1f, gizmosAlpha * 0.5f);
                Handles.Label(_gizmosEditorEndPreview.position + GetBaseTransform().TransformDirection(ModelUpAxis) * _gizmosDist, new GUIContent("Chain End"), FGUI_Resources.HeaderStyle);
            }


        }

        public Transform _gizmosEditorStartPreview;
        public Transform _gizmospesp;
        public Transform _gizmosEditorEndPreview;
        public Transform _gizmospeep;
        List<Transform> _gizmosChainList;


        public void _GizmosRefreshChainList(bool hard = false)
        {
            if (!hard)
                if (_gizmospesp == _gizmosEditorStartPreview && _gizmospeep == _gizmosEditorEndPreview) return;

            _gizmospesp = _gizmosEditorStartPreview;
            _gizmospeep = _gizmosEditorEndPreview;

            if (_gizmosChainList == null) _gizmosChainList = new List<Transform>();

            if (_gizmosEditorStartPreview && _gizmosEditorEndPreview)
            {
                _gizmosChainList.Clear();

                Transform p = _gizmosEditorEndPreview;
                _gizmosChainList.Add(p);

                while (p.parent != null)
                {
                    p = p.parent;
                    if (p == null) break;
                    _gizmosChainList.Add(p);
                    if (p == _gizmosEditorStartPreview) break;
                }

                _gizmosChainList.Reverse();
            }
        }

    }
}

#endif
