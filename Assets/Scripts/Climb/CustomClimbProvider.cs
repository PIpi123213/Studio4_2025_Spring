using System.Collections.Generic;
using UnityEngine.Assertions;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Locomotion provider that allows the user to climb a <see cref="ClimbInteractable"/> by selecting it.
    /// Climb locomotion moves the XR Origin counter to movement of the last selecting interactor, with optional
    /// movement constraints along each axis of the interactable.
    /// </summary>
    /// <seealso cref="ClimbInteractable"/>
    [AddComponentMenu("XR/Locomotion/Custom Climb Provider", 11)]
    public class CustomClimbProvider : LocomotionProvider
    {
        readonly List<IXRSelectInteractor> m_GrabbingInteractors = new List<IXRSelectInteractor>();
        readonly List<CustomClimbInteractable> m_GrabbedClimbables = new List<CustomClimbInteractable>();

        public int Climb_res = 2;
        public float Climb_offest = 0.05f;
        private Vector3 m_InteractorAnchorWorldPosition;
        private Vector3 m_InteractorAnchorClimbSpacePosition;
        private Vector3 totalClimbOffset = Vector3.zero;

        [SerializeField]
        [Tooltip("Climb locomotion settings. Can be overridden by the Climb Interactable used for locomotion.")]
        ClimbSettingsDatumProperty m_ClimbSettings = new ClimbSettingsDatumProperty(new ClimbSettings());

        /// <summary>
        /// Climb locomotion settings. Can be overridden by the <see cref="CustomClimbInteractable"/> used for locomotion.
        /// </summary>
        public ClimbSettingsDatumProperty climbSettings
        {
            get => m_ClimbSettings;
            set => m_ClimbSettings = value;
        }

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();
            if (m_ClimbSettings == null || m_ClimbSettings.Value == null)
                m_ClimbSettings = new ClimbSettingsDatumProperty(new ClimbSettings());
        }

        public void StartClimbGrab(CustomClimbInteractable climbInteractable, IXRSelectInteractor interactor)
        {
            var xrOrigin = system.xrOrigin?.Origin;
            if (xrOrigin == null)
                return;

            totalClimbOffset = Vector3.zero;
            m_GrabbingInteractors.Add(interactor);
            m_GrabbedClimbables.Add(climbInteractable);
            UpdateClimbAnchor(climbInteractable, interactor);

            if (locomotionPhase != LocomotionPhase.Moving)
                locomotionPhase = LocomotionPhase.Started;
        }

        public void FinishClimbGrab(IXRSelectInteractor interactor)
        {
            var interactionIndex = m_GrabbingInteractors.IndexOf(interactor);
            if (interactionIndex < 0)
                return;

            Assert.AreEqual(m_GrabbingInteractors.Count, m_GrabbedClimbables.Count);

            if (interactionIndex > 0 && interactionIndex == m_GrabbingInteractors.Count - 1)
            {
                var newLastIndex = interactionIndex - 1;
                UpdateClimbAnchor(m_GrabbedClimbables[newLastIndex], m_GrabbingInteractors[newLastIndex]);
            }

            if (m_GrabbingInteractors.Count == 0)
            {
                totalClimbOffset = Vector3.zero;
            }

            m_GrabbingInteractors.RemoveAt(interactionIndex);
            m_GrabbedClimbables.RemoveAt(interactionIndex);
        }

        void UpdateClimbAnchor(CustomClimbInteractable climbInteractable, IXRInteractor interactor)
        {
            var climbTransform = climbInteractable.climbTransform;
            m_InteractorAnchorWorldPosition = interactor.transform.position;
            m_InteractorAnchorClimbSpacePosition = climbTransform.InverseTransformPoint(m_InteractorAnchorWorldPosition);
        }

        protected virtual void Update()
        {
            if (locomotionPhase == LocomotionPhase.Done)
            {
                locomotionPhase = LocomotionPhase.Idle;
                return;
            }

            if (m_GrabbingInteractors.Count > 0)
            {
                if (locomotionPhase != LocomotionPhase.Moving)
                {
                    if (!BeginLocomotion())
                        return;

                    locomotionPhase = LocomotionPhase.Moving;
                }

                Assert.AreEqual(m_GrabbingInteractors.Count, m_GrabbedClimbables.Count);

                var lastIndex = m_GrabbingInteractors.Count - 1;
                var currentInteractor = m_GrabbingInteractors[lastIndex];
                var currentClimbInteractable = m_GrabbedClimbables[lastIndex];
                if (currentInteractor == null || currentClimbInteractable == null)
                {
                    FinishLocomotion();
                    return;
                }

                StepClimbMovement(currentClimbInteractable, currentInteractor);
            }
            else if (locomotionPhase != LocomotionPhase.Idle)
            {
                FinishLocomotion();
            }
        }

        void StepClimbMovement(CustomClimbInteractable currentClimbInteractable, IXRSelectInteractor currentInteractor)
        {
            var xrOrigin = system.xrOrigin?.Origin;
            if (xrOrigin != null)
            {
                var activeClimbSettings = GetActiveClimbSettings(currentClimbInteractable);
                var allowFreeXMovement = activeClimbSettings.allowFreeXMovement;
                var allowFreeYMovement = activeClimbSettings.allowFreeYMovement;
                var allowFreeZMovement = activeClimbSettings.allowFreeZMovement;
                var rigTransform = xrOrigin.transform;
                var interactorWorldPosition = currentInteractor.transform.position;
                Vector3 movement;

                var climbTransform = currentClimbInteractable.climbTransform;
                var interactorClimbSpacePosition = climbTransform.InverseTransformPoint(interactorWorldPosition);
                var movementInClimbSpace = m_InteractorAnchorClimbSpacePosition - interactorClimbSpacePosition;

                // 计算每个轴的累计位移
                if (allowFreeXMovement)
                {
                    if (movementInClimbSpace.x > 0) // Moving right
                    {
                        if (totalClimbOffset.x >= currentClimbInteractable.maxRight + Climb_offest)
                        {
                            movementInClimbSpace.x = -movementInClimbSpace.x / (1 + 200);  // No movement if exceeds maxRight
                        }
                        else if (totalClimbOffset.x >= currentClimbInteractable.maxRight)
                        {
                            float normalizedClimbOffsetX = Mathf.InverseLerp(0, Mathf.Abs(currentClimbInteractable.maxRight), Mathf.Abs(totalClimbOffset.x));

                            // Square function to enhance the effect for larger values
                            float squaredEffectX = Mathf.Pow(normalizedClimbOffsetX, Climb_res); // You can adjust the exponent to control the effect

                            // Finally, adjust movementInClimbSpace.x based on squaredEffectX
                            movementInClimbSpace.x = movementInClimbSpace.x / (100 + squaredEffectX + Climb_res);
                        }
                        else
                        {
                            if (totalClimbOffset.x < 0)
                            {
                                movementInClimbSpace.x = movementInClimbSpace.x / 2;
                            }
                            else
                            {
                                float normalizedClimbOffsetX = Mathf.InverseLerp(0, Mathf.Abs(currentClimbInteractable.maxRight), Mathf.Abs(totalClimbOffset.x));

                                // Square function to enhance the effect for larger values
                                float squaredEffectX = Mathf.Exp(normalizedClimbOffsetX * Climb_res); // You can adjust the exponent to control the effect

                                // Finally, adjust movementInClimbSpace.x based on squaredEffectX
                                movementInClimbSpace.x = movementInClimbSpace.x / (2 + squaredEffectX);
                            }
                           
                        }
                    }
                    else if (movementInClimbSpace.x < 0) // Moving left
                    {
                        if (totalClimbOffset.x <= -currentClimbInteractable.maxLeft - Climb_offest)
                        {
                            movementInClimbSpace.x = -movementInClimbSpace.x / (1 + 200); // No movement if exceeds maxLeft
                        }
                        else if (totalClimbOffset.x <= -currentClimbInteractable.maxLeft)
                        {
                            float normalizedClimbOffsetX = Mathf.InverseLerp(0, Mathf.Abs(currentClimbInteractable.maxLeft), Mathf.Abs(totalClimbOffset.x));

                            // Square function to enhance the effect for larger values
                            float squaredEffectX = Mathf.Pow(normalizedClimbOffsetX, Climb_res);

                            // Finally, adjust movementInClimbSpace.x based on squaredEffectX
                            movementInClimbSpace.x = movementInClimbSpace.x / (100 + squaredEffectX + Climb_res);
                        }
                        else
                        {
                            if (totalClimbOffset.x > 0)
                            {
                                movementInClimbSpace.x = movementInClimbSpace.x / 2 ;
                            }
                            else
                            {
                                float normalizedClimbOffsetX = Mathf.InverseLerp(0, Mathf.Abs(currentClimbInteractable.maxLeft), Mathf.Abs(totalClimbOffset.x));

                                // Square function to enhance the effect for larger values
                                float squaredEffectX = Mathf.Exp(normalizedClimbOffsetX * Climb_res);

                                // Finally, adjust movementInClimbSpace.x based on squaredEffectX
                                movementInClimbSpace.x = movementInClimbSpace.x / (2 + squaredEffectX);
                            }
                           
                        }
                    }

                    totalClimbOffset.x += movementInClimbSpace.x;
                   
                }
            
                else
                {
                    movementInClimbSpace.x = 0f; // Lock X movement
                }

                // Handle Y-axis movement with different limits for up and down
                if (allowFreeYMovement)
                {
                   
                    if (movementInClimbSpace.y > 0) // Moving upward
                    {
                        if (totalClimbOffset.y >= currentClimbInteractable.maxUp + Climb_offest)
                        {
                            movementInClimbSpace.y = -movementInClimbSpace.y / (1 + 200);  // No movement if exceeds maxUp
                        }

                        else if (totalClimbOffset.y >= currentClimbInteractable.maxUp)
                        {
                            float normalizedClimbOffset = Mathf.InverseLerp(0, Mathf.Abs(currentClimbInteractable.maxUp), Mathf.Abs(totalClimbOffset.y));

                            // 使用平方函数来增强较大值的影响
                            float squaredEffect = Mathf.Pow(normalizedClimbOffset, Climb_res);  // 你可以调整平方的指数来控制效果

                            // 最后根据 squaredEffect 来调整 movementInClimbSpace.x
                            movementInClimbSpace.y = movementInClimbSpace.y / (100 + squaredEffect + Climb_res);
                        }
                        else
                        {
                            if (totalClimbOffset.y < 0)
                            {
                                movementInClimbSpace.y = movementInClimbSpace.y / 2;

                            }
                            else
                            {
                                float normalizedClimbOffset = Mathf.InverseLerp(0, Mathf.Abs(currentClimbInteractable.maxUp), Mathf.Abs(totalClimbOffset.y));

                                // 使用平方函数来增强较大值的影响
                                float squaredEffect = Mathf.Exp(normalizedClimbOffset * Climb_res); // 你可以调整平方的指数来控制效果

                                // 最后根据 squaredEffect 来调整 movementInClimbSpace.x
                                movementInClimbSpace.y = movementInClimbSpace.y / (2 + squaredEffect);
                            }
                           

                        }
                    }
                    else if (movementInClimbSpace.y < 0) // Moving downward
                    {
                        if (totalClimbOffset.y <= -currentClimbInteractable.maxDown - Climb_offest)
                        {
                            //movementInClimbSpace.y = -movementInClimbSpace.y / (1 + 200);
                        }
                        else if (totalClimbOffset.y <= -currentClimbInteractable.maxDown)
                        {
                            float normalizedClimbOffset = Mathf.InverseLerp(0, Mathf.Abs(currentClimbInteractable.maxDown), Mathf.Abs(totalClimbOffset.y));

                            // 使用平方函数来增强较大值的影响
                            float squaredEffect = Mathf.Pow(normalizedClimbOffset, Climb_res);  // 你可以调整平方的指数来控制效果

                            // 最后根据 squaredEffect 来调整 movementInClimbSpace.
                            movementInClimbSpace.y = movementInClimbSpace.y / (100 + squaredEffect + Climb_res);
                        }
                        else
                        {
                            if (totalClimbOffset.y > 0)
                            {
                                movementInClimbSpace.y = movementInClimbSpace.y / 2;

                            }
                            else
                            {
                                float normalizedClimbOffset = Mathf.InverseLerp(0, Mathf.Abs(currentClimbInteractable.maxDown), Mathf.Abs(totalClimbOffset.y));

                                // 使用平方函数来增强较大值的影响

                                float squaredEffect = Mathf.Exp(normalizedClimbOffset * Climb_res);
                                // 最后根据 squaredEffect 来调整 movementInClimbSpace.x
                                movementInClimbSpace.y = movementInClimbSpace.y / (2 + squaredEffect);
                            }


                        }
                    }

                    totalClimbOffset.y += movementInClimbSpace.y;
                }
                else
                {
                    movementInClimbSpace.y = 0f; // Lock Y movement
                }

                // Handle Z-axis movement with different limits for forward and backward
                if (allowFreeZMovement)
                {
                    if (movementInClimbSpace.z > 0) // Moving forward
                    {
                        if (totalClimbOffset.z >= currentClimbInteractable.maxForward + Climb_offest)
                        {
                            movementInClimbSpace.z = -movementInClimbSpace.z / (1 + 200);  // No movement if exceeds maxForward
                        }
                        else if (totalClimbOffset.z >= currentClimbInteractable.maxForward)
                        {
                            float normalizedClimbOffsetZ = Mathf.InverseLerp(0, Mathf.Abs(currentClimbInteractable.maxForward), Mathf.Abs(totalClimbOffset.z));

                            // Square function to enhance the effect for larger values
                            float squaredEffectZ = Mathf.Pow(normalizedClimbOffsetZ, Climb_res);

                            // Finally, adjust movementInClimbSpace.z based on squaredEffectZ
                            movementInClimbSpace.z = movementInClimbSpace.z / (100 + squaredEffectZ + Climb_res);
                        }
                        else
                        {
                            if (totalClimbOffset.z > 0)
                            {
                                movementInClimbSpace.z = movementInClimbSpace.z / 2;
                            }
                            else
                            {
                                float normalizedClimbOffsetZ = Mathf.InverseLerp(0, Mathf.Abs(currentClimbInteractable.maxForward), Mathf.Abs(totalClimbOffset.z));

                                // Square function to enhance the effect for larger values
                                float squaredEffectZ = Mathf.Exp(normalizedClimbOffsetZ * Climb_res);

                                // Finally, adjust movementInClimbSpace.z based on squaredEffectZ
                                movementInClimbSpace.z = movementInClimbSpace.z / (2 + squaredEffectZ);
                            }
                           
                        }
                    }
                    else if (movementInClimbSpace.z < 0) // Moving backward
                    {
                        if (totalClimbOffset.z <= -currentClimbInteractable.maxBackward - Climb_offest)
                        {
                            movementInClimbSpace.z = -movementInClimbSpace.z / (1 + 200); // No movement if exceeds maxBackward
                        }
                        else if (totalClimbOffset.z <= currentClimbInteractable.maxBackward)
                        {
                            float normalizedClimbOffsetZ = Mathf.InverseLerp(0, Mathf.Abs(currentClimbInteractable.maxBackward), Mathf.Abs(totalClimbOffset.z));

                            // Square function to enhance the effect for larger values
                            float squaredEffectZ = Mathf.Pow(normalizedClimbOffsetZ, Climb_res);

                            // Finally, adjust movementInClimbSpace.z based on squaredEffectZ
                            movementInClimbSpace.z = movementInClimbSpace.z / (100 + squaredEffectZ + Climb_res);
                        }
                        else
                        {
                            if (totalClimbOffset.z < 0)
                            {
                                movementInClimbSpace.z = movementInClimbSpace.z / 2;
                            }
                            else
                            {
                                float normalizedClimbOffsetZ = Mathf.InverseLerp(0, Mathf.Abs(currentClimbInteractable.maxBackward), Mathf.Abs(totalClimbOffset.z));

                                // Square function to enhance the effect for larger values
                                float squaredEffectZ = Mathf.Exp(normalizedClimbOffsetZ * Climb_res);

                                // Finally, adjust movementInClimbSpace.z based on squaredEffectZ
                                movementInClimbSpace.z = movementInClimbSpace.z / (2 + squaredEffectZ);
                            }
                           
                        }
                    }

                    totalClimbOffset.z += movementInClimbSpace.z;
                }
                else
                {
                    movementInClimbSpace.z = 0f; // Lock Z movement
                }

                // Apply movement
                movement = climbTransform.TransformVector(movementInClimbSpace);
                rigTransform.position += movement;
            }
        }

        void FinishLocomotion()
        {
            EndLocomotion();
            totalClimbOffset = Vector3.zero;
            locomotionPhase = LocomotionPhase.Done;
            m_GrabbingInteractors.Clear();
            m_GrabbedClimbables.Clear();
        }

        ClimbSettings GetActiveClimbSettings(CustomClimbInteractable climbInteractable)
        {
            if (climbInteractable.climbSettingsOverride.Value != null)
                return climbInteractable.climbSettingsOverride;

            return m_ClimbSettings;
        }
    }
}