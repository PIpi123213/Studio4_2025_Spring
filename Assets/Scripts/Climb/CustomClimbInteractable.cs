using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Interactable that can be climbed while selected.
    /// </summary>
    /// <seealso cref="CustomClimbProvider"/>
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [AddComponentMenu("XR/Custom Climb Interactable", 11)]

    public class CustomClimbInteractable : XRBaseInteractable
    {
        const float k_DefaultMaxInteractionDistance = 0.1f * 4f;

        [SerializeField]
        [Tooltip("The climb provider that performs locomotion while this interactable is selected. " +
                 "If no climb provider is configured, will attempt to find one.")]
        CustomClimbProvider m_ClimbProvider;

        /// <summary>
        /// The climb provider that performs locomotion while this interactable is selected.
        /// If no climb provider is configured, will attempt to find one.
        /// </summary>
        public CustomClimbProvider climbProvider
        {
            get => m_ClimbProvider;
            set => m_ClimbProvider = value;
        }

        [SerializeField]
        [Tooltip("Transform that defines the coordinate space for climb locomotion. " +
                 "Will use this GameObject's Transform by default.")]
        Transform m_ClimbTransform;

        /// <summary>
        /// Transform that defines the coordinate space for climb locomotion. Will use this GameObject's Transform by default.
        /// </summary>
        public Transform climbTransform
        {
            get
            {
                if (m_ClimbTransform == null)
                    m_ClimbTransform = transform;
                return m_ClimbTransform;
            }
            set => m_ClimbTransform = value;
        }

        [SerializeField]
        [Tooltip("Controls whether to apply a distance check when validating hover and select interaction.")]
        bool m_FilterInteractionByDistance = true;

        /// <summary>
        /// Controls whether to apply a distance check when validating hover and select interaction.
        /// </summary>
        /// <seealso cref="maxInteractionDistance"/>
        /// <seealso cref="XRBaseInteractable.distanceCalculationMode"/>
        public bool filterInteractionByDistance
        {
            get => m_FilterInteractionByDistance;
            set => m_FilterInteractionByDistance = value;
        }

        [SerializeField]
        [Tooltip("The maximum distance that an interactor can be from this interactable to begin hover or select.")]
        float m_MaxInteractionDistance = k_DefaultMaxInteractionDistance;

        /// <summary>
        /// The maximum distance that an interactor can be from this interactable to begin hover or select.
        /// Only applies when <see cref="filterInteractionByDistance"/> is <see langword="true"/>.
        /// </summary>
        /// <seealso cref="filterInteractionByDistance"/>
        /// <seealso cref="XRBaseInteractable.distanceCalculationMode"/>
        public float maxInteractionDistance
        {
            get => m_MaxInteractionDistance;
            set => m_MaxInteractionDistance = value;
        }

        [SerializeField]
        [Tooltip("Optional override of locomotion settings specified in the climb provider. " +
                 "Only applies as an override if set to Use Value or if the asset reference is set.")]
        ClimbSettingsDatumProperty m_ClimbSettingsOverride;

        /// <summary>
        /// Optional override of climb locomotion settings specified in the climb provider. Only applies as
        /// an override if <see cref="Unity.XR.CoreUtils.Datums.DatumProperty{TValue, TDatum}.Value"/> is not <see langword="null"/>.
        /// </summary>
        public ClimbSettingsDatumProperty climbSettingsOverride
        {
            get => m_ClimbSettingsOverride;
            set => m_ClimbSettingsOverride = value;
        }


        [Header("X Axis Constraints")]
        [Tooltip("���������˶��������루�� X ���򣩡�")]
        public float maxLeft = 0.35f*4f;
        [Tooltip("���������˶��������루�� X ���򣩡�")]
        public float maxRight = 0.35f*4f;

        [Header("Y Axis Constraints")]
        [Tooltip("���������˶��������루�� Y ���򣩡�")]
        public float maxUp = 0.35f * 4f;
        [Tooltip("���������˶��������루�� Y ���򣩡�")]
        public float maxDown = 0.5f*4f;

        [Header("Z Axis Constraints")]
        [Tooltip("������ǰ�˶��������루�� Z ���򣩡�")]
        public float maxForward = 0.1f*4f;
        [Tooltip("��������˶��������루�� Z ���򣩡�")]
        public float maxBackward = 0.3f * 4f;


        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnValidate()
        {
            if (m_ClimbTransform == null)
                m_ClimbTransform = transform;
        }

        /// <inheritdoc />
        protected override void Reset()
        {
            base.Reset();

            selectMode = InteractableSelectMode.Multiple;
            m_ClimbTransform = transform;
        }

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();


        }

        /// <inheritdoc />
        public override bool IsHoverableBy(IXRHoverInteractor interactor)
        {
            return base.IsHoverableBy(interactor) && (!m_FilterInteractionByDistance ||
                GetDistanceSqrToInteractor(interactor) <= m_MaxInteractionDistance * m_MaxInteractionDistance);
        }

        /// <inheritdoc />
        public override bool IsSelectableBy(IXRSelectInteractor interactor)
        {
            return base.IsSelectableBy(interactor) && (IsSelected(interactor) || !m_FilterInteractionByDistance ||
                GetDistanceSqrToInteractor(interactor) <= m_MaxInteractionDistance * m_MaxInteractionDistance);
        }

        /// <inheritdoc />
        public const string OnRockClimb = "OnRockClimb";
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            if (m_ClimbProvider != null )
                m_ClimbProvider.StartClimbGrab(this, args.interactorObject);

            if (!CharacterClimb.isStart)  CharacterClimb.isStart = true;
            if (ZipLine.isSliding&&ZipLine.isDone) ZipLine.isSliding = false;
            int randomIndex = UnityEngine.Random.Range(1, 8);
            if (transform.gameObject.tag == "Rock")
            {
                Debug.Log("Rock Climb");
                EventManager.Instance.Trigger(OnRockClimb, randomIndex);
            }
        }

        /// <inheritdoc />
        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            if (m_ClimbProvider != null)
                m_ClimbProvider.FinishClimbGrab(args.interactorObject);
        }
    }
}