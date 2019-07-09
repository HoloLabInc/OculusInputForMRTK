// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in https://github.com/XRTK/XRTK-Core/blob/upm/LICENSE.md for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Oculus.Input
{
    public class BaseOculusController : BaseController
    {
        public BaseOculusController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
            NodeType = controllerHandedness == Handedness.Left ? OculusApi.Node.HandLeft : OculusApi.Node.HandRight;
        }

        public OculusApi.Controller controllerType = OculusApi.Controller.None;
        public OculusApi.Node NodeType = OculusApi.Node.None;

        public OculusApi.ControllerState4 previousState = new OculusApi.ControllerState4();
        public OculusApi.ControllerState4 currentState  = new OculusApi.ControllerState4();

        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultInteractions);
        }

        //internal MLInputController MlControllerReference { get; set; }
        //internal LuminControllerGestureSettings ControllerGestureSettings { get; set; }

        internal bool IsHomePressed = false;

        private MixedRealityPose currentPointerPose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose lastControllerPose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose currentControllerPose = MixedRealityPose.ZeroIdentity;
        private float singleAxisValue = 0.0f;
        private Vector2 dualAxisPosition = Vector2.zero;

        /// <summary>
        /// Updates the controller's interaction mappings and ready the current input values.
        /// </summary>
        public void UpdateController()
        {
            if (!Enabled) { return; }

            UpdateControllerData();

            if (Interactions == null)
            {
                Debug.LogError($"No interaction configuration for Windows Mixed Reality Motion Controller {ControllerHandedness}");
                Enabled = false;
            }

            for (int i = 0; i < Interactions?.Length; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.SpatialPointer:
                        UpdatePoseData(Interactions[i]);
                        break;
                    case DeviceInputType.Select:
                    case DeviceInputType.ButtonPress:
                    case DeviceInputType.TriggerPress:
                    case DeviceInputType.ThumbStickPress:
                        UpdateButtonDataPress(Interactions[i]);
                        break;
                    case DeviceInputType.ButtonTouch:
                    case DeviceInputType.TriggerTouch:
                    case DeviceInputType.ThumbTouch:
                    case DeviceInputType.TouchpadTouch:
                    case DeviceInputType.ThumbStickTouch:
                        UpdateButtonDataTouch(Interactions[i]);
                        break;

                    case DeviceInputType.ButtonNearTouch:
                    case DeviceInputType.TriggerNearTouch:
                    case DeviceInputType.ThumbNearTouch:
                    case DeviceInputType.TouchpadNearTouch:
                    case DeviceInputType.ThumbStickNearTouch:
                        UpdateButtonDataNearTouch(Interactions[i]);
                        break;
                    case DeviceInputType.Trigger:
                        UpdateSingleAxisData(Interactions[i]);
                        break;
                    case DeviceInputType.ThumbStick:
                    case DeviceInputType.Touchpad:
                        UpdateDualAxisData(Interactions[i]);
                        break;
                    default:
                        Debug.LogError($"Input [{Interactions[i].InputType}] is not handled for this controller [{GetType().Name}]");
                        break;
                }
            }
        }

        private void UpdateControllerData()
        {
            var lastState = TrackingState;
            lastControllerPose = currentControllerPose;
            previousState = currentState;

            currentState = OculusApi.GetControllerState4((uint)controllerType);

            if (currentState.LIndexTrigger >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentState.Buttons |= (uint)OculusApi.RawButton.LIndexTrigger;

            if (currentState.LHandTrigger >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentState.Buttons |= (uint)OculusApi.RawButton.LHandTrigger;

            if (currentState.LThumbstick.y >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentState.Buttons |= (uint)OculusApi.RawButton.LThumbstickUp;

            if (currentState.LThumbstick.y <= -OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentState.Buttons |= (uint)OculusApi.RawButton.LThumbstickDown;

            if (currentState.LThumbstick.x <= -OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentState.Buttons |= (uint)OculusApi.RawButton.LThumbstickLeft;

            if (currentState.LThumbstick.x >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentState.Buttons |= (uint)OculusApi.RawButton.LThumbstickRight;

            if (currentState.RIndexTrigger >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentState.Buttons |= (uint)OculusApi.RawButton.RIndexTrigger;

            if (currentState.RHandTrigger >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentState.Buttons |= (uint)OculusApi.RawButton.RHandTrigger;

            if (currentState.RThumbstick.y >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentState.Buttons |= (uint)OculusApi.RawButton.RThumbstickUp;

            if (currentState.RThumbstick.y <= -OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentState.Buttons |= (uint)OculusApi.RawButton.RThumbstickDown;

            if (currentState.RThumbstick.x <= -OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentState.Buttons |= (uint)OculusApi.RawButton.RThumbstickLeft;

            if (currentState.RThumbstick.x >= OculusApi.AXIS_AS_BUTTON_THRESHOLD)
                currentState.Buttons |= (uint)OculusApi.RawButton.RThumbstickRight;

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

            var pose = OculusApi.GetNodePose(NodeType, OculusApi.Step.Render);

            currentControllerPose = pose.ToMixedRealityPose();

            // Raise input system events if it is enabled.
            if (lastState != TrackingState)
            {
                MixedRealityToolkit.InputSystem?.RaiseSourceTrackingStateChanged(InputSource, this, TrackingState);
            }

            if (TrackingState == TrackingState.Tracked && lastControllerPose != currentControllerPose)
            {
                if (IsPositionAvailable && IsRotationAvailable)
                {
                    MixedRealityToolkit.InputSystem?.RaiseSourcePoseChanged(InputSource, this, currentControllerPose);
                }
                else if (IsPositionAvailable && !IsRotationAvailable)
                {
                    MixedRealityToolkit.InputSystem?.RaiseSourcePositionChanged(InputSource, this, currentControllerPose.Position);
                }
                else if (!IsPositionAvailable && IsRotationAvailable)
                {
                    MixedRealityToolkit.InputSystem?.RaiseSourceRotationChanged(InputSource, this, currentControllerPose.Rotation);
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

        private void UpdateButtonDataPress(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.Digital);

            OculusApi.RawButton interactionButton = OculusApi.RawButton.None;
            OculusInteractionMapping.TryParseRawButton(interactionMapping, out interactionButton);
            //Enum.TryParse<OculusApi.RawButton>(interactionMapping.InputName, out interactionButton);
            //TODO: SHould the "ShouldResolveController" function be used here?
            if (interactionButton != OculusApi.RawButton.None)
            {
                if ((((OculusApi.RawButton)currentState.Buttons & interactionButton) == 0)
                    && (((OculusApi.RawButton)previousState.Buttons & interactionButton) != 0))
                {
                    interactionMapping.BoolData = false;
                }

                if ((((OculusApi.RawButton)currentState.Buttons & interactionButton) != 0)
                    && (((OculusApi.RawButton)previousState.Buttons & interactionButton) == 0))
                {
                    interactionMapping.BoolData = true;
                }

                //interactionMapping.UpdateInteractionMappingBool(InputSource, ControllerHandedness);

                if (interactionMapping.Changed)
                {
                    Debug.Log("On Changed");

                    if (interactionMapping.BoolData)
                    {
                        InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                    }
                    else
                    {
                        InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                    }
                }

                if (interactionButton == OculusApi.RawButton.LHandTrigger ||
                    interactionButton == OculusApi.RawButton.RIndexTrigger)
                {
                    InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, MixedRealityPose.ZeroIdentity);
                }
            }
        }

        private void UpdateButtonDataTouch(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.Digital);

            OculusApi.RawTouch interactionButton = OculusApi.RawTouch.None;
            OculusInteractionMapping.TryParseRawTouch(interactionMapping, out interactionButton);
            //Enum.TryParse<OculusApi.RawTouch>(interactionMapping.InputName, out interactionButton);

            if (interactionButton != OculusApi.RawTouch.None)
            {
                if (((OculusApi.RawTouch)previousState.Touches & interactionButton) != 0)
                {
                    interactionMapping.BoolData = false;
                }

                if ((((OculusApi.RawTouch)currentState.Touches & interactionButton) != 0)
                    && (((OculusApi.RawTouch)previousState.Touches & interactionButton) == 0))
                {
                    interactionMapping.BoolData = true;
                }

                //interactionMapping.UpdateInteractionMappingBool(InputSource, ControllerHandedness);

                //if (interactionMapping.Changed)
                //{
                //    if (interactionMapping.BoolData)
                //    {
                //        InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                //    }
                //    else
                //    {
                //        InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                //    }
                //}
            }
        }



        private void UpdateButtonDataNearTouch(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.Digital);

            OculusApi.RawNearTouch interactionButton = OculusApi.RawNearTouch.None;
            OculusInteractionMapping.TryParseRawNearTouch(interactionMapping, out interactionButton);
            //Enum.TryParse<OculusApi.RawNearTouch>(interactionMapping.InputName, out interactionButton);

            if (interactionButton != OculusApi.RawNearTouch.None)
            {
                if (((OculusApi.RawNearTouch)previousState.NearTouches & interactionButton) != 0)
                {
                    interactionMapping.BoolData = false;
                }

                if ((((OculusApi.RawNearTouch)currentState.NearTouches & interactionButton) != 0)
                    && (((OculusApi.RawNearTouch)previousState.NearTouches & interactionButton) == 0))
                {
                    interactionMapping.BoolData = true;
                }

                //interactionMapping.UpdateInteractionMappingBool(InputSource, ControllerHandedness);

                //if (interactionMapping.Changed)
                //{
                //    if (interactionMapping.BoolData)
                //    {
                //        InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                //    }
                //    else
                //    {
                //        InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                //    }
                //}
            }

        }

        private void UpdateSingleAxisData(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SingleAxis);

            OculusApi.RawAxis1D interactionAxis1D = OculusApi.RawAxis1D.None;
            OculusInteractionMapping.TryParseRawAxis1D(interactionMapping, out interactionAxis1D);
            //Enum.TryParse<OculusApi.RawAxis1D>(interactionMapping.InputName, out interactionAxis1D);

            if (interactionAxis1D != OculusApi.RawAxis1D.None)
            {
                switch (interactionAxis1D)
                {
                    case OculusApi.RawAxis1D.LIndexTrigger:
                        singleAxisValue = currentState.LIndexTrigger;

                        //if (shouldApplyDeadzone)
                        //    singleAxisValue = OculusApi.CalculateDeadzone(singleAxisValue, OculusApi.AXIS_DEADZONE_THRESHOLD);

                        singleAxisValue = OculusApi.CalculateAbsMax(0, singleAxisValue);
                        break;
                    case OculusApi.RawAxis1D.LHandTrigger:
                        singleAxisValue = currentState.LHandTrigger;

                        //if (shouldApplyDeadzone)
                        //    singleAxisValue = OculusApi.CalculateDeadzone(singleAxisValue, OculusApi.AXIS_DEADZONE_THRESHOLD);

                        singleAxisValue = OculusApi.CalculateAbsMax(0, singleAxisValue);
                        break;

                    case OculusApi.RawAxis1D.RIndexTrigger:
                        singleAxisValue = currentState.RIndexTrigger;

                        //if (shouldApplyDeadzone)
                        //    singleAxisValue = OculusApi.CalculateDeadzone(singleAxisValue, OculusApi.AXIS_DEADZONE_THRESHOLD);

                        singleAxisValue = OculusApi.CalculateAbsMax(0, singleAxisValue);
                        break;

                    case OculusApi.RawAxis1D.RHandTrigger:
                        singleAxisValue = currentState.RHandTrigger;

                        //if (shouldApplyDeadzone)
                        //    singleAxisValue = OculusApi.CalculateDeadzone(singleAxisValue, OculusApi.AXIS_DEADZONE_THRESHOLD);

                        singleAxisValue = OculusApi.CalculateAbsMax(0, singleAxisValue);
                        break;
                }
            }

            // Update the interaction data source
            interactionMapping.FloatData = singleAxisValue;

            //interactionMapping.UpdateInteractionMappingFloat(InputSource, ControllerHandedness);
            if (interactionMapping.Changed)
            {
                InputSystem?.RaiseFloatInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.FloatData);
            }
        }

        private void UpdateDualAxisData(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.DualAxis);

            OculusApi.RawAxis2D interactionAxis2D = OculusApi.RawAxis2D.None;
            OculusInteractionMapping.TryParseRawAxis2D(interactionMapping, out interactionAxis2D);
            //Enum.TryParse<OculusApi.RawAxis2D>(interactionMapping.InputName, out interactionAxis2D);

            if (interactionAxis2D != OculusApi.RawAxis2D.None)
            {
                switch (interactionAxis2D)
                {
                    case OculusApi.RawAxis2D.LThumbstick:
                        dualAxisPosition.x = currentState.LThumbstick.x;
                        dualAxisPosition.y = currentState.LThumbstick.y;

                        //if (shouldApplyDeadzone)
                        //    dualAxisPosition = OculusApi.CalculateDeadzone(dualAxisPosition, OculusApi.AXIS_DEADZONE_THRESHOLD);

                        dualAxisPosition = OculusApi.CalculateAbsMax(Vector2.zero, dualAxisPosition);
                        break;

                    case OculusApi.RawAxis2D.LTouchpad:
                        dualAxisPosition.x = currentState.LTouchpad.x;
                        dualAxisPosition.y = currentState.LTouchpad.y;

                        //if (shouldApplyDeadzone)
                        //    dualAxisPosition = OculusApi.CalculateDeadzone(dualAxisPosition, OculusApi.AXIS_DEADZONE_THRESHOLD);

                        dualAxisPosition = OculusApi.CalculateAbsMax(Vector2.zero, dualAxisPosition);
                        break;

                    case OculusApi.RawAxis2D.RThumbstick:

                        dualAxisPosition.x = currentState.RThumbstick.x;
                        dualAxisPosition.y = currentState.RThumbstick.y;

                        //if (shouldApplyDeadzone)
                        //    dualAxisPosition = OculusApi.CalculateDeadzone(dualAxisPosition, OculusApi.AXIS_DEADZONE_THRESHOLD);

                        dualAxisPosition = OculusApi.CalculateAbsMax(Vector2.zero, dualAxisPosition);
                        break;

                    case OculusApi.RawAxis2D.RTouchpad:

                        dualAxisPosition.x = currentState.RTouchpad.x;
                        dualAxisPosition.y = currentState.RTouchpad.y;

                        //if (shouldApplyDeadzone)
                        //    dualAxisPosition = OculusApi.CalculateDeadzone(dualAxisPosition, OculusApi.AXIS_DEADZONE_THRESHOLD);

                        dualAxisPosition = OculusApi.CalculateAbsMax(Vector2.zero, dualAxisPosition);
                        break;
                }
            }

            // Update the interaction data source
            interactionMapping.Vector2Data = dualAxisPosition;
            //interactionMapping.UpdateInteractionMappingVector2(InputSource, ControllerHandedness);
            if (interactionMapping.Changed)
            {
                InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.Vector2Data);
            }
        }

        private void UpdatePoseData(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SixDof);

            if (interactionMapping.InputType != DeviceInputType.SpatialPointer)
            {
                Debug.LogError($"Input [{interactionMapping.InputType}] is not handled for this controller [{GetType().Name}]");
                return;
            }

            //if (interactionMapping.InputType == DeviceInputType.SpatialPointer)
            //{
            //    pointerOffsetPose.Position = CurrentControllerPose.Position;
            //    pointerOffsetPose.Rotation = CurrentControllerPose.Rotation * Quaternion.AngleAxis(PointerOffsetAngle, Vector3.left);

            //    // Update the interaction data source
            //    interactionMapping.PoseData = pointerOffsetPose;
            //}

            //else if (interactionMapping.InputType == DeviceInputType.SpatialGrip)
            //{
            //    // Update the interaction data source
            //    interactionMapping.PoseData = CurrentControllerPose;
            //}
            //else
            //{
            //    Debug.LogWarning($"Unhandled Interaction {interactionMapping.Description}");
            //    return;
            //}

            currentPointerPose = currentControllerPose;

            // Update the interaction data source
            interactionMapping.PoseData = currentPointerPose;

            //It is not defined in MRTK
            //interactionMapping.UpdateInteractionMappingPose(InputSource, ControllerHandedness);

            if (interactionMapping.Changed)
            {
                InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.PoseData);
            }
        }
    }
}
