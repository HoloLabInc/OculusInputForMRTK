using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Oculus.Input
{
    public abstract class BaseOculusSource : BaseController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        protected BaseOculusSource(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null) 
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => DefaultInteractions;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => DefaultInteractions;

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultInteractions);
        }

        private MixedRealityPose currentPointerPose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose currentGripPose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose lastSourcePose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose currentSourcePose = MixedRealityPose.ZeroIdentity;
        
        public OculusApi.Node NodeType = OculusApi.Node.None;
        public OculusApi.Controller controllerType = OculusApi.Controller.None;
        protected OculusApi.ControllerState4 currentInputSourceState  = new OculusApi.ControllerState4();
        protected OculusApi.ControllerState4 previousInputSourceState = new OculusApi.ControllerState4();

        /// <summary>
        /// Update the source data from the provided platform state.
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform.</param>
        public virtual void UpdateController()
        {
            if (!Enabled) { return; }

            UpdateSourceData();

            if (Interactions == null)
            {
                Debug.LogError($"No interaction configuration for Windows Mixed Reality {ControllerHandedness} Source");
                Enabled = false;
            }

            for (int i = 0; i < Interactions?.Length; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.None:
                        break;
                    case DeviceInputType.SpatialPointer:
                        UpdatePointerData(Interactions[i]);
                        break;
                    case DeviceInputType.Select:
                    case DeviceInputType.TriggerPress:
                        UpdateTriggerData(Interactions[i]);
                        break;
                    case DeviceInputType.SpatialGrip:
                        UpdateGripData(Interactions[i]);
                        break;
                }
            }
        }

        /// <summary>
        /// Update the source input from the device.
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform.</param>
        private void UpdateSourceData()
        {
            var lastState  = TrackingState;
            lastSourcePose = currentSourcePose;

            previousInputSourceState = currentInputSourceState;
            currentInputSourceState  = OculusApi.GetControllerState4((uint)controllerType);

            #region Update Current State of Controller Data
            if (currentInputSourceState.LIndexTrigger >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentInputSourceState.Buttons |= (uint)OculusApi.RawButton.LIndexTrigger;

            if (currentInputSourceState.LHandTrigger >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentInputSourceState.Buttons |= (uint)OculusApi.RawButton.LHandTrigger;

            if (currentInputSourceState.LThumbstick.y >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentInputSourceState.Buttons |= (uint)OculusApi.RawButton.LThumbstickUp;

            if (currentInputSourceState.LThumbstick.y <= -OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentInputSourceState.Buttons |= (uint)OculusApi.RawButton.LThumbstickDown;

            if (currentInputSourceState.LThumbstick.x <= -OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentInputSourceState.Buttons |= (uint)OculusApi.RawButton.LThumbstickLeft;

            if (currentInputSourceState.LThumbstick.x >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentInputSourceState.Buttons |= (uint)OculusApi.RawButton.LThumbstickRight;

            if (currentInputSourceState.RIndexTrigger >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentInputSourceState.Buttons |= (uint)OculusApi.RawButton.RIndexTrigger;

            if (currentInputSourceState.RHandTrigger >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentInputSourceState.Buttons |= (uint)OculusApi.RawButton.RHandTrigger;

            if (currentInputSourceState.RThumbstick.y >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentInputSourceState.Buttons |= (uint)OculusApi.RawButton.RThumbstickUp;

            if (currentInputSourceState.RThumbstick.y <= -OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentInputSourceState.Buttons |= (uint)OculusApi.RawButton.RThumbstickDown;

            if (currentInputSourceState.RThumbstick.x <= -OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentInputSourceState.Buttons |= (uint)OculusApi.RawButton.RThumbstickLeft;

            if (currentInputSourceState.RThumbstick.x >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentInputSourceState.Buttons |= (uint)OculusApi.RawButton.RThumbstickRight;
            #endregion

            #region Update Position & Rotation Data
            if (IsTrackedController(controllerType))
            {
                // The source is either a hand or a controller that supports pointing.
                // We can now check for position and rotation.
                IsPositionAvailable = OculusApi.GetNodePositionTracked(NodeType);

                if (IsPositionAvailable)
                {
                    IsPositionApproximate = OculusApi.GetNodePositionValid(NodeType);
                }
                else
                {
                    IsPositionApproximate = false;
                }

                IsRotationAvailable = OculusApi.GetNodeOrientationTracked(NodeType);

                // Devices are considered tracked if we receive position OR rotation data from the sensors.
                TrackingState = (IsPositionAvailable || IsRotationAvailable) ? TrackingState.Tracked : TrackingState.NotTracked;
            }
            else
            {
                // The input source does not support tracking.
                TrackingState = TrackingState.NotApplicable;
            }

            var pose = OculusApi.GetNodePose(NodeType, OculusApi.Step.Render).ToMixedRealityPose();
            currentSourcePose = pose;
            #endregion

            // Raise input system events if it is enabled.
            if (lastState != TrackingState)
            {
                InputSystem?.RaiseSourceTrackingStateChanged(InputSource, this, TrackingState);
            }

            if (TrackingState == TrackingState.Tracked && lastSourcePose != currentSourcePose)
            {
                if (IsPositionAvailable && IsRotationAvailable)
                {
                    InputSystem?.RaiseSourcePoseChanged(InputSource, this, currentSourcePose);
                }
                else if (IsPositionAvailable && !IsRotationAvailable)
                {
                    InputSystem?.RaiseSourcePositionChanged(InputSource, this, currentSourcePose.Position);
                }
                else if (!IsPositionAvailable && IsRotationAvailable)
                {
                    InputSystem?.RaiseSourceRotationChanged(InputSource, this, currentSourcePose.Rotation);
                }
            }
        }

        private bool IsTrackedController(OculusApi.Controller controller)
        {
            return controller == OculusApi.Controller.LTouch ||
                controller == OculusApi.Controller.LTrackedRemote ||
                controller == OculusApi.Controller.RTouch ||
                controller == OculusApi.Controller.RTrackedRemote ||
                controller == OculusApi.Controller.Touch;
        }

        /// <summary>
        /// Update the spatial pointer input from the device.
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform.</param>
        /// <param name="interactionMapping"></param>
        private void UpdatePointerData(MixedRealityInteractionMapping interactionMapping)
        {
            if(interactionMapping.AxisType == AxisType.SixDof &&
                interactionMapping.InputType == DeviceInputType.SpatialPointer)
            {
                currentPointerPose = currentSourcePose;
                
                // Update the interaction data source
                interactionMapping.PoseData = currentPointerPose;

                // If our value changed raise it.
                if (interactionMapping.Changed)
                {
                    // Raise input system Event if it enabled
                    InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, currentPointerPose);
                }
            }
        }

        /// <summary>
        /// Update the spatial grip input from the device.
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform.</param>
        /// <param name="interactionMapping"></param>
        private void UpdateGripData(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.AxisType)
            {
                case AxisType.SixDof:
                    {
                        currentGripPose = currentSourcePose;

                        // Update the interaction data source
                        interactionMapping.PoseData = currentGripPose;

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            // Raise input system Event if it enabled
                            InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, currentGripPose);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Update the trigger and grasped input from the device.
        /// </summary>
        /// <param name="interactionMapping"></param>
        private void UpdateTriggerData(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.TriggerPress:
                {
                    OculusApi.RawButton interactionButton = OculusApi.RawButton.None;
                    OculusInteractionMapping.TryParseRawButton(interactionMapping, out interactionButton);

                    if (interactionButton != OculusApi.RawButton.LIndexTrigger ||
                        interactionButton != OculusApi.RawButton.RIndexTrigger) { return; }

                    // Update the interaction data source
                    if ((((OculusApi.RawButton)currentInputSourceState.Buttons & interactionButton) == 0)
                    && (((OculusApi.RawButton)previousInputSourceState.Buttons & interactionButton) != 0))
                    {
                        interactionMapping.BoolData = false;
                    }

                    if ((((OculusApi.RawButton)currentInputSourceState.Buttons & interactionButton) != 0)
                    && (((OculusApi.RawButton)previousInputSourceState.Buttons & interactionButton) == 0))
                    {
                        interactionMapping.BoolData = true;
                    }

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // Raise input system Event if it enabled
                        if (interactionMapping.BoolData)
                        {
                            InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                        else
                        {
                            InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                    }
                    break;
                }
            }
        }
    }
}
