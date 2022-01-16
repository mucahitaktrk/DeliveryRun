using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FSpine
{
    /// <summary>
    /// FM: Main component for spine-chain-follow procedural animation
    /// </summary>
    [AddComponentMenu("FImpossible Creations/Spine Animator 2")]
    [DefaultExecutionOrder(-11)]
    public partial class FSpineAnimator : MonoBehaviour, UnityEngine.EventSystems.IDropHandler, IFHierarchyIcon
    {

        // THIS IS PARTIAL CLASS: REST OF THE CODE INSIDE "Scripts" directory


        #region Inspector Variables ---------------------------

        /// <summary> List of Spine Animator's bones used for animation </summary>
        public List<SpineBone> SpineBones;
        /// <summary> Transforms list is no more used, it remains here for bones import to new version. Use SpineBones[].Transform </summary>
        public List<Transform> SpineTransforms;

        #region Head bones Variables

        private HeadBone frontHead;
        private HeadBone backHead;
        private HeadBone headBone;

        #endregion


        [Tooltip("Main character object - by default it is game object to which Spine Animator is attached.\n\nYou can use it to control spine of character from different game object.")]
        public Transform BaseTransform;
        /// <summary> ForwardReference is no more used, it remains here for compatibility import to new version. Use BaseTransform now </summary>
        public Transform ForwardReference;

        [System.Obsolete("Use SpineAnimatorAmount instead, but remember that it works in reversed way -> SpineAnimatorAmount 1 = BlendToOriginal 0  and  SpineAnimatorAmount 0 = BlendToOriginal 1")]
        public float BlendToOriginal { get { return 1f - SpineAnimatorAmount; } set { SpineAnimatorAmount = 1f - value; } }


        [Tooltip("If your spine lead bone is in beggining of your hierarchy chain then toggle it.\n\nComponent's gizmos can help you out to define which bone should be leading (check head gizmo when you switch this toggle).")]
        public bool LastBoneLeading = true;
        public bool EndBoneIsHead { get { return LastBoneLeading; } set { LastBoneLeading = EndBoneIsHead; } }

        [Tooltip("Sometimes spine chain can face in different direction than desired or you want your characters to move backward with spine motion.")]
        public bool ReverseForward = false;


        //[Tooltip("If you want spine animator motion to be connected with keyframed animation motion, don't use this when your object isn't animated.")]
        //public bool SyncWithAnimator = true;

        //[Tooltip("If your skeleton have not animated keyframes in animation clip then bones would start doing circles with this option disabled.\n\nIn most cases all keyframes are filled, if you're sure for baked keyframes you can disable this option to avoid some not needed calculations.")]
        //public bool DetectZeroKeyframes = true;


        [Tooltip("If you're using 'Animate Physics' on animator you should set this variable to be enabled.")]
        public EFixedMode AnimatePhysics = EFixedMode.None;
        public enum EFixedMode { None, Basic, Late }

        /// <summary> AnchorRoot is no more used, it remains here for compatibility import to new version. Use HeadAnchor now </summary>
        public Transform AnchorRoot = null;
        [Tooltip("Connecting lead bone position to given transform, useful when it is tail and you already animating spine with other Spine Animator component.")]
        public Transform HeadAnchor = null;
        [Tooltip("Letting head anchor to animate rotation")]
        public bool AnimateAnchor = true;

        [Tooltip("If you need to offset leading bone rotation.")]
        public Vector3 LeadBoneRotationOffset = Vector3.zero;
        [Tooltip("If Lead Bone Rotation Offset should affect reference pose or bone rotation")]
        public bool LeadBoneOffsetReference = true;

        [Tooltip("List of bone positioning/rotation fixers if using paws positioning with IK controlls disconnected out of arms/legs in the hierarchy")]
        public List<SpineAnimator_FixIKControlledBones> BonesFixers = new List<SpineAnimator_FixIKControlledBones>();

        [Tooltip("Useful when you use few spine animators and want to rely on animated position and rotation by other spine animator.")]
        public bool UpdateAsLast = false;
        /// <summary> Queue to last udpate is no longer used, use "UpdateAsLast" now </summary>
        public bool QueueToLastUpdate = false;

        [Tooltip("If corrections should affect spine chain children.")]
        public bool ManualAffectChain = false;

        [Tooltip("Often when you drop model to scene, it's initial pose is much different than animations, which causes problems, this toggle solves it at start.")]
        public bool StartAfterTPose = true;

        [Tooltip("If you want spine animator to stop computing when choosed mesh is not visible in any camera view (editor's scene camera is detecting it too)")]
        public Renderer OptimizeWithMesh = null;

        [Tooltip("Delta Time for Spine Animator calculations")]
        public EFDeltaType DeltaType = EFDeltaType.SafeDelta;

        [Tooltip("Making update rate stable for target rate.\nIf this value is = 0 then update rate is unlimited.")]
        public float UpdateRate = 0f;

        [Tooltip("In some cases you need to use chain corrections, it will cost a bit more in performance, not much but always.")]
        public bool UseCorrections = false;

        [Tooltip("Sometimes offsetting model's pivot position gives better results using spine animator, offset forward axis so front legs are in centrum and see the difference (generating additional transform inside hierarchy)")]
        public Vector3 MainPivotOffset = new Vector3(0f, 0f, 0f);


        #region Animation settings and limitations

        [Range(0f, 1f)]
        [Tooltip("If animation of changing segments position should be smoothed - creating a little gumy effect.")]
        public float PosSmoother = 0f;
        [Range(0f, 1f)]
        [Tooltip("If animation of changing segments rotation should be smoothed - making it more soft, but don't overuse it!")]
        public float RotSmoother = 0f;
        [Range(0f, 1f)]
        [Tooltip("We stretching segments to bigger value than bones are by default to create some extra effect which looks good but sometimes it can stretch to much if you using position smoothing, you can adjust it here.")]
        public float MaxStretching = 1f;
        [Tooltip("Making algorithm referencing back to static rotation if value = 0f | at 1 motion have more range and is more slithery.")]
        [Range(0f, 1f)]
        public float Slithery = 1f;

        [Range(1f, 91f)]
        [Tooltip("Limiting rotation angle difference between each segment of spine.")]
        public float AngleLimit = 40f;
        [Range(0f, 1f)]
        [Tooltip("Smoothing how fast limiting should make segments go back to marginal pose.")]
        public float LimitSmoother = .35f;
        [Range(0f, 15f)]
        [Tooltip("How fast spine should be rotated to straight pose when your character moves.")]
        public float StraightenSpeed = 7.5f;
        public bool TurboStraighten = false;

        [Tooltip("Spine going back to straight position constantly with choosed speed intensity.")]
        [Range(0f, 1f)]
        public float GoBackSpeed = 0f;

        [Tooltip("Elastic spring effect good for tails to make them more 'meaty'.")]
        [Range(0f, 1f)]
        public float Springiness = 0.0f;


        [Tooltip("How much effect on spine chain should have character movement.")]
        [Range(0f, 1f)]
        public float MotionInfluence = 1f;
        [Tooltip("Useful when your creature jumps on moving platform, so when platform moves spine is not reacting, by default world space is used (null).")]
        public Transform MotionSpace;


        [SerializeField]
        [HideInInspector]
        private Transform mainPivotOffsetTransform;

        [Tooltip("<! Most models can not need this !> Offset for bones rotations, thanks to that animation is able to rotate to segments in a correct way, like from center of mass.")]
        public Vector3 SegmentsPivotOffset = new Vector3(0f, 0f, 0f);

        [Tooltip("Multiplies distance value between bones segments - can be useful for use with humanoid skeletons")]
        public float DistancesMultiplier = 1f;

        [Tooltip("Pushing segments in world direction (should have included ground collider to collide with).")]
        public Vector3 GravityPower = Vector3.zero;
        protected Vector3 gravityScale = Vector3.zero;

        #endregion


        #region Physics Experimental Stuff

        [Tooltip("[Experimental] Using some simple calculations to make spine bend on colliders.")]
        public bool UseCollisions = false;

        public List<Collider> IncludedColliders;
        protected List<FImp_ColliderData_Base> IncludedCollidersData;
        protected List<FImp_ColliderData_Base> CollidersDataToCheck;
        //public List<Vector3> CollidersOffsets;

        [Tooltip("If disabled Colliders can be offsetted a bit in wrong way - check pink spheres in scene view (playmode, with true positions disabled colliders are fitting to stiff reference pose) - but it gives more stable collision projection! But to avoid stuttery you can increase position smoothing.")]
        public bool UseTruePosition = false;
        public Vector3 OffsetAllColliders = Vector3.zero;

        public AnimationCurve CollidersScale = AnimationCurve.Linear(0, 1, 1, 1);
        public float CollidersScaleMul = 6.5f;
        [Range(0f, 1f)]
        public float DifferenceScaleFactor = 0f;

        [Tooltip("If you want to continue checking collision if segment collides with one collider (very useful for example when you using gravity power with ground)")]
        public bool DetailedCollision = true;


        #endregion


        #endregion


        // THIS IS PARTIAL CLASS: REST OF THE CODE INSIDE "Scripts" directory


        void Reset()
        {
            // Trying to find reference transform when adding component to object
            BaseTransform = FindBaseTransform();
        }


        void Start()
        {
            // This simple thing queues monobehaviour execution to be updates as last
            if (UpdateAsLast) { enabled = false; enabled = true; }
            if (BaseTransform == null) BaseTransform = transform;

            initialized = false;

            // Initializing in courutine after animator sets default animation
            if (!StartAfterTPose) Init(); else initAfterTPoseCounter = 0;
        }


        bool updateSpineAnimator = false;
        internal void Update()
        {
            #region Conditions to do any calculations within Spine Animator

            if (!initialized)
            {
                if (StartAfterTPose)
                {
                    initAfterTPoseCounter++;
                    if (initAfterTPoseCounter > 5)
                    {
                        Init();
                    }
                    else
                    { updateSpineAnimator = false; return; }
                }
                else
                { updateSpineAnimator = false; return; }
            }

            #region Debug "`" disable key for editor only

#if UNITY_EDITOR
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKey(KeyCode.BackQuote)) { updateSpineAnimator = false; return; }
#endif
#endif

#endregion

            if (OptimizeWithMesh != null) if (OptimizeWithMesh.isVisible == false) { updateSpineAnimator = false; return; }

            //#region Triggering Animate Physics Support

            //if (AnimatePhysics)
            //{
            //    if (!animatePhysicsWorking) StartCoroutine(AnimatePhysicsClock());
            //    if (!triggerAnimatePhysics) { updateSpineAnimator = false; return; } else triggerAnimatePhysics = false;
            //}

            //#endregion


            if (SpineBones.Count == 0)
            {
                Debug.LogError("[SPINE ANIMATOR] No spine bones defined in " + name + " !");
                initialized = false;
                updateSpineAnimator = false;
                return;
            }


            if (BaseTransform == null) BaseTransform = transform;


            UpdateChainIndexHelperVariables();


            // Resetting spine motion when disabling amount
            if (SpineAnimatorAmount <= 0.01f)
            {
                wasBlendedOut = true;
                updateSpineAnimator = false;
                return;
            }
            else if (wasBlendedOut)
            {
                ReposeSpine();
                wasBlendedOut = false;
            }

            updateSpineAnimator = true;

            #endregion

            if (AnimatePhysics != EFixedMode.None) return;
            PreCalibrateBones();
        }


        internal void FixedUpdate()
        {
            if (!updateSpineAnimator) return;
            if (AnimatePhysics != EFixedMode.Basic) return;
            PreCalibrateBones();
        }


        internal void LateUpdate()
        {
            if (!updateSpineAnimator) return;


            #region Support second solution for animate physics mode -----

            if (AnimatePhysics == EFixedMode.Late)
            {
                if (!lateFixedIsRunning) { StartCoroutine(LateFixed()); }
                if (fixedAllow) fixedAllow = false; else return;
            }
            else if (lateFixedIsRunning) { StopCoroutine(LateFixed()); lateFixedIsRunning = false; }

            #endregion


            CalibrateBones();
            DeltaTimeCalculations();

            #region Main update operations dependent on update rate

            if (UpdateRate > 0)
            {
                StableUpdateRateCalculations(); // Delta computation
                unifiedDelta = delta;

                if (UseCorrections)
                    if (ManualAffectChain)
                    {
                        if (updateLoops > 0) PreMotionNoHead();
                        PreMotionHead();
                    }

                RefreshReferencePose();
                BeginBaseBonesUpdate();

                for (int i = 0; i < updateLoops; i++)
                {
                    BeginPhysicsUpdate();
                    CalculateBonesCoordinates();
                }

                if (UseCorrections)
                    if (!ManualAffectChain)
                    {
                        if (updateLoops > 0) PostMotionNoHead();
                        PostMotionHead();
                    }
            }
            else
            {
                RefreshReferencePose();
                PreMotionBoneOffsets();

                BeginPhysicsUpdate();
                BeginBaseBonesUpdate();
                CalculateBonesCoordinates();

                PostMotionBoneOffsets();
            }

            #endregion


            ApplyNewBonesCoordinates();

            EndUpdate();

        }

    }
}