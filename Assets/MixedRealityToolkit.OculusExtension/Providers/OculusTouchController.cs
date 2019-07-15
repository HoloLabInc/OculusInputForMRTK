using System;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Oculus.Input
{
    /// <summary>
    /// A Oculus Touch Controller Instance.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.OculusTouch,
        new[] { Handedness.Left, Handedness.Right, Handedness.None },
        "StandardAssets/Textures/MotionController")]
    public sealed class OculusTouchController : BaseOculusSource
    {
        public OculusTouchController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null) 
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <inheritdoc />
        /// <remarks> Note, MUST use RAW button types as that is what the API works with, DO NOT use Virtual!</remarks>
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Trigger", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_9),
            new MixedRealityInteractionMapping(2, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, KeyCode.JoystickButton14),
            new MixedRealityInteractionMapping(3, "Trigger Near Touch", AxisType.Digital, DeviceInputType.TriggerNearTouch, ControllerMappingLibrary.AXIS_13),
            new MixedRealityInteractionMapping(4, "Trigger Press", AxisType.Digital, DeviceInputType.TriggerPress, ControllerMappingLibrary.AXIS_9),
            new MixedRealityInteractionMapping(5, "HandTrigger Press", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_11),
            new MixedRealityInteractionMapping(6, "Thumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerMappingLibrary.AXIS_1, ControllerMappingLibrary.AXIS_2),
            new MixedRealityInteractionMapping(7, "Thumbstick Touch", AxisType.Digital, DeviceInputType.ThumbStickTouch, KeyCode.JoystickButton16),            
            new MixedRealityInteractionMapping(8, "Thumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress, KeyCode.JoystickButton8),
            new MixedRealityInteractionMapping(9, "Thumb Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, ControllerMappingLibrary.AXIS_15),
            new MixedRealityInteractionMapping(10, "X Button Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton2),
            new MixedRealityInteractionMapping(11, "Y Button Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton3),
            new MixedRealityInteractionMapping(12, "X Button Touch", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton12),
            new MixedRealityInteractionMapping(13, "Y Button Touch", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton13),
            new MixedRealityInteractionMapping(14, "Start Press", AxisType.Digital, DeviceInputType.Menu, KeyCode.JoystickButton7),
            new MixedRealityInteractionMapping(15, "PrimaryThumbRest Touch", AxisType.Digital, DeviceInputType.ThumbTouch, KeyCode.JoystickButton18),
            new MixedRealityInteractionMapping(16, "PrimaryThumbRest Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, ControllerMappingLibrary.AXIS_17)
        };

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Trigger", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_10),
            new MixedRealityInteractionMapping(2, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, KeyCode.JoystickButton15),
            new MixedRealityInteractionMapping(3, "Trigger Near Touch", AxisType.Digital, DeviceInputType.TriggerNearTouch, ControllerMappingLibrary.AXIS_14),
            new MixedRealityInteractionMapping(4, "Trigger Press", AxisType.Digital, DeviceInputType.TriggerPress, ControllerMappingLibrary.AXIS_10),
            new MixedRealityInteractionMapping(5, "HandTrigger Press", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_12),
            new MixedRealityInteractionMapping(6, "Thumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerMappingLibrary.AXIS_4, ControllerMappingLibrary.AXIS_5),
            new MixedRealityInteractionMapping(7, "Thumbstick Touch", AxisType.Digital, DeviceInputType.ThumbStickTouch, KeyCode.JoystickButton17),
            new MixedRealityInteractionMapping(8, "Thumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress, KeyCode.JoystickButton9),
            new MixedRealityInteractionMapping(9, "Thumb Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, ControllerMappingLibrary.AXIS_16),
            new MixedRealityInteractionMapping(10, "A Button Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton0),
            new MixedRealityInteractionMapping(11, "B Button Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton1),
            new MixedRealityInteractionMapping(12, "X Button Touch", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton10),
            new MixedRealityInteractionMapping(13, "Y Button Touch", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton11),
            new MixedRealityInteractionMapping(14, "SecondaryThumbRest Touch", AxisType.Digital, DeviceInputType.ThumbTouch, KeyCode.JoystickButton19),
            new MixedRealityInteractionMapping(15, "SecondaryThumbRest Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, ControllerMappingLibrary.AXIS_18)
        };

        #region Update data functions

        /// <summary>
        /// Update the controller data from the provided platform state.
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        public override void UpdateController()
        {
            if (!Enabled) { return; }

            base.UpdateController();

            for (int i = 0; i < Interactions?.Length; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.None:
                        break;
                    #region ThumbSticks
                    case DeviceInputType.ThumbStick:
                        UpdateThumbstickData(Interactions[i]);
                        break;
                    case DeviceInputType.ThumbStickPress:
                        UpdateThumbstickPressData(Interactions[i]);
                        break;
                    case DeviceInputType.ThumbStickTouch:
                        UpdateThumbStickTouch(Interactions[i]);
                        break;
                    #endregion
                    case DeviceInputType.ThumbNearTouch:
                        UpdateThumbNearTouch(Interactions[i]);
                        break;
                    case DeviceInputType.Menu:
                    case DeviceInputType.ButtonPress:
                        UpdateButtonData(Interactions[i]);
                        break;
                    case DeviceInputType.ButtonTouch:
                        UpdateButtonTouchData(Interactions[i]);
                        break;
                }
            }
        }

        #region ThumbSticks
        private Vector2 dualAxisPosition = Vector2.zero;

        /// <summary>
        /// Update the thumbstick input from the device.
        /// </summary>
        /// <param name="interactionMapping"></param>
        private void UpdateThumbstickData(MixedRealityInteractionMapping interactionMapping)
        {
            {
                OculusApi.RawAxis2D interactionAxis2D = OculusApi.RawAxis2D.None;
                OculusInteractionMapping.TryParseRawAxis2D(interactionMapping, out interactionAxis2D);

                if (interactionAxis2D != OculusApi.RawAxis2D.None)
                {
                    switch (interactionAxis2D)
                    {
                        case OculusApi.RawAxis2D.LThumbstick:
                            dualAxisPosition.x = currentInputSourceState.LThumbstick.x;
                            dualAxisPosition.y = currentInputSourceState.LThumbstick.y;
                            dualAxisPosition = OculusApi.CalculateAbsMax(Vector2.zero, dualAxisPosition);
                            break;
                        case OculusApi.RawAxis2D.LTouchpad:
                            dualAxisPosition.x = currentInputSourceState.LTouchpad.x;
                            dualAxisPosition.y = currentInputSourceState.LTouchpad.y;
                            dualAxisPosition = OculusApi.CalculateAbsMax(Vector2.zero, dualAxisPosition);
                            break;
                        case OculusApi.RawAxis2D.RThumbstick:
                            dualAxisPosition.x = currentInputSourceState.RThumbstick.x;
                            dualAxisPosition.y = currentInputSourceState.RThumbstick.y;
                            dualAxisPosition = OculusApi.CalculateAbsMax(Vector2.zero, dualAxisPosition);
                            break;
                        case OculusApi.RawAxis2D.RTouchpad:
                            dualAxisPosition.x = currentInputSourceState.RTouchpad.x;
                            dualAxisPosition.y = currentInputSourceState.RTouchpad.y;
                            dualAxisPosition = OculusApi.CalculateAbsMax(Vector2.zero, dualAxisPosition);
                            break;
                    }
                }

                // Update the interaction data source
                interactionMapping.Vector2Data = dualAxisPosition;
                if (interactionMapping.Changed)
                {
                    InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.Vector2Data);
                }
            }

        }

        /// <summary>
        /// Update the thumbstick press input from the device.
        /// </summary>
        /// <param name="interactionMapping"></param>
        private void UpdateThumbstickPressData(MixedRealityInteractionMapping interactionMapping)
        {
            if (interactionMapping.InputType != DeviceInputType.ThumbStickPress)
                return;

            OculusApi.RawButton interactionButton = OculusApi.RawButton.None;
            OculusInteractionMapping.TryParseRawButton(interactionMapping, out interactionButton);

            if (interactionButton != OculusApi.RawButton.LThumbstick ||
                interactionButton != OculusApi.RawButton.RThumbstick) { return; }

            // Update the interaction data source
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
        }

        /// <summary>
        /// Update the thumbstick touch input from the device.
        /// </summary>
        /// <param name="interactionMapping"></param>
        private void UpdateThumbStickTouch(MixedRealityInteractionMapping interactionMapping)
        {
            OculusApi.RawTouch interactionTouch = OculusApi.RawTouch.None;
            OculusInteractionMapping.TryParseRawTouch(interactionMapping, out interactionTouch);

            if (interactionTouch != OculusApi.RawTouch.LThumbstick ||
                interactionTouch != OculusApi.RawTouch.RThumbstick) { return; }

            // Update the interaction data source
            if ((((OculusApi.RawTouch)currentInputSourceState.Touches & interactionTouch) == 0)
            && (((OculusApi.RawTouch)previousInputSourceState.Touches & interactionTouch) != 0))
            {
                interactionMapping.BoolData = false;
            }

            if ((((OculusApi.RawTouch)currentInputSourceState.Touches & interactionTouch) != 0)
            && (((OculusApi.RawTouch)previousInputSourceState.Touches & interactionTouch) == 0))
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
        }
        #endregion

        /// <summary>
        /// Update the thumbstick near touch input from the device.
        /// </summary>
        /// <param name="interactionMapping"></param>
        private void UpdateThumbNearTouch(MixedRealityInteractionMapping interactionMapping)
        {
            OculusApi.RawNearTouch interactionTouch = OculusApi.RawNearTouch.None;
            OculusInteractionMapping.TryParseRawNearTouch(interactionMapping, out interactionTouch);

            if (interactionTouch != OculusApi.RawNearTouch.LThumbButtons ||
                interactionTouch != OculusApi.RawNearTouch.RThumbButtons) { return; }

            // Update the interaction data source
            if ((((OculusApi.RawNearTouch)currentInputSourceState.NearTouches & interactionTouch) == 0)
            && (((OculusApi.RawNearTouch)previousInputSourceState.NearTouches & interactionTouch) != 0))
            {
                interactionMapping.BoolData = false;
            }

            if ((((OculusApi.RawNearTouch)currentInputSourceState.Touches & interactionTouch) != 0)
            && (((OculusApi.RawNearTouch)previousInputSourceState.Touches & interactionTouch) == 0))
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
        }

        #region Buttons
        /// <summary>
        /// Update the Button input from the device
        /// </summary>
        /// <param name="interactionMapping"></param>
        private void UpdateButtonData(MixedRealityInteractionMapping interactionMapping)
        {
            if (interactionMapping.InputType != DeviceInputType.ButtonPress)
                return;

            OculusApi.RawButton interactionButton = OculusApi.RawButton.None;
            OculusInteractionMapping.TryParseRawButton(interactionMapping, out interactionButton);

            if (interactionButton != OculusApi.RawButton.None) { return; }

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
        }

        /// <summary>
        /// Update the Button touch input from the device
        /// </summary>
        /// <param name="interactionMapping"></param>
        private void UpdateButtonTouchData(MixedRealityInteractionMapping interactionMapping)
        {
            if (interactionMapping.InputType != DeviceInputType.ButtonPress)
                return;

            OculusApi.RawTouch interactionButton = OculusApi.RawTouch.None;
            OculusInteractionMapping.TryParseRawTouch(interactionMapping, out interactionButton);

            if (interactionButton != OculusApi.RawTouch.None) { return; }

            // Update the interaction data source
            if ((((OculusApi.RawTouch)currentInputSourceState.Buttons & interactionButton) == 0)
            && (((OculusApi.RawTouch)previousInputSourceState.Buttons & interactionButton) != 0))
            {
                interactionMapping.BoolData = false;
            }

            if ((((OculusApi.RawTouch)currentInputSourceState.Buttons & interactionButton) != 0)
            && (((OculusApi.RawTouch)previousInputSourceState.Buttons & interactionButton) == 0))
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
        }
        #endregion
        #endregion
    }
}
