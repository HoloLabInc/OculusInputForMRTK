using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Oculus
{
    /// <summary>
    /// This class Convert MixedRealitToolkit.InteractionMapping to OculusApi.Button
    /// </summary>
    public static class OculusInteractionMapping
    {
        public static bool TryParseRawAxis1D(MixedRealityInteractionMapping mapping, out OculusApi.RawAxis1D axis)
        {
            axis = OculusApi.RawAxis1D.None;
            if (mapping.AxisType != AxisType.SingleAxis)
                return false;

            if (mapping.InputType == DeviceInputType.Trigger)
            {
                if (mapping.AxisCodeX == ControllerMappingLibrary.AXIS_9)  { axis = OculusApi.RawAxis1D.LIndexTrigger; return true; }
                if (mapping.AxisCodeX == ControllerMappingLibrary.AXIS_10) { axis = OculusApi.RawAxis1D.RIndexTrigger; return true; }
                if (mapping.AxisCodeX == ControllerMappingLibrary.AXIS_11) { axis = OculusApi.RawAxis1D.LHandTrigger;  return true; }
                if (mapping.AxisCodeX == ControllerMappingLibrary.AXIS_12) { axis = OculusApi.RawAxis1D.RHandTrigger;  return true; }
            }
            return false;
        }

        public static bool TryParseRawAxis2D(MixedRealityInteractionMapping mapping, out OculusApi.RawAxis2D axis)
        {
            axis = OculusApi.RawAxis2D.None;
            if (mapping.AxisType != AxisType.DualAxis)
                return false;

            if (mapping.InputType == DeviceInputType.ThumbStick)
            {
                if (mapping.AxisCodeX == ControllerMappingLibrary.AXIS_1 || mapping.AxisCodeX == ControllerMappingLibrary.AXIS_2) { axis = OculusApi.RawAxis2D.LThumbstick; return true; }
                if (mapping.AxisCodeX == ControllerMappingLibrary.AXIS_4 || mapping.AxisCodeX == ControllerMappingLibrary.AXIS_5) { axis = OculusApi.RawAxis2D.RThumbstick; return true; }
            }
            return false;
        }

        public static bool TryParseRawTouch(MixedRealityInteractionMapping mapping, out OculusApi.RawTouch touch)
        {
            touch = OculusApi.RawTouch.None;
            if (mapping.AxisType != AxisType.Digital)
                return false;

            if (mapping.InputType == DeviceInputType.TriggerTouch)
            {
                if (mapping.KeyCode == KeyCode.JoystickButton14) { touch = OculusApi.RawTouch.LIndexTrigger; return true; }
                if (mapping.KeyCode == KeyCode.JoystickButton15) { touch = OculusApi.RawTouch.RIndexTrigger; return true; }
            }
            if (mapping.InputType == DeviceInputType.ThumbTouch)
            {
                if (mapping.KeyCode == KeyCode.JoystickButton16) { touch = OculusApi.RawTouch.LThumbstick; return true; }
                if (mapping.KeyCode == KeyCode.JoystickButton17) { touch = OculusApi.RawTouch.RThumbstick; return true; }
                if (mapping.KeyCode == KeyCode.JoystickButton18) { touch = OculusApi.RawTouch.LThumbRest; return true; }
                if (mapping.KeyCode == KeyCode.JoystickButton19) { touch = OculusApi.RawTouch.RThumbRest; return true; }
            }
            if (mapping.InputType == DeviceInputType.ButtonTouch)
            {
                if (mapping.KeyCode == KeyCode.JoystickButton10) { touch = OculusApi.RawTouch.A; return true; }
                if (mapping.KeyCode == KeyCode.JoystickButton11) { touch = OculusApi.RawTouch.B; return true; }
                if (mapping.KeyCode == KeyCode.JoystickButton12) { touch = OculusApi.RawTouch.X; return true; }
                if (mapping.KeyCode == KeyCode.JoystickButton13) { touch = OculusApi.RawTouch.Y; return true; }
            }
            return false;
        }

        public static bool TryParseRawNearTouch(MixedRealityInteractionMapping mapping, out OculusApi.RawNearTouch nearTouch)
        {
            nearTouch = OculusApi.RawNearTouch.None;

            if (mapping.AxisType != AxisType.Digital)
                return false;

            if (mapping.InputType == DeviceInputType.TriggerNearTouch)
            {
                if (mapping.AxisCodeX == ControllerMappingLibrary.AXIS_13) { nearTouch = OculusApi.RawNearTouch.LIndexTrigger; return true; }
                if (mapping.AxisCodeY == ControllerMappingLibrary.AXIS_14) { nearTouch = OculusApi.RawNearTouch.RIndexTrigger; return true; }
                if (mapping.AxisCodeX == ControllerMappingLibrary.AXIS_17) { nearTouch = OculusApi.RawNearTouch.LThumbButtons; return true; }
                if (mapping.AxisCodeY == ControllerMappingLibrary.AXIS_18) { nearTouch = OculusApi.RawNearTouch.RThumbButtons; return true; }
            }
            return false;
        }

        public static bool TryParseRawButton(MixedRealityInteractionMapping mapping, out OculusApi.RawButton button)
        {
            button = OculusApi.RawButton.None;
            if (mapping.InputType == DeviceInputType.ButtonPress)
            {
                if (mapping.KeyCode == KeyCode.JoystickButton0) { button = OculusApi.RawButton.A; return true; }
                if (mapping.KeyCode == KeyCode.JoystickButton1) { button = OculusApi.RawButton.B; return true; }
                if (mapping.KeyCode == KeyCode.JoystickButton2) { button = OculusApi.RawButton.X; return true; }
                if (mapping.KeyCode == KeyCode.JoystickButton3) { button = OculusApi.RawButton.Y; return true; }
                if (mapping.KeyCode == KeyCode.JoystickButton7) { button = OculusApi.RawButton.Start; return true; }
                if (mapping.KeyCode == KeyCode.JoystickButton8) { button = OculusApi.RawButton.LThumbstick; return true; }
                if (mapping.KeyCode == KeyCode.JoystickButton9) { button = OculusApi.RawButton.RThumbstick; return true; }
                return false;
            }
            if (mapping.InputType == DeviceInputType.TriggerPress)
            {
                if (mapping.AxisCodeX == ControllerMappingLibrary.AXIS_9)  { button = OculusApi.RawButton.LIndexTrigger; return true; }
                if (mapping.AxisCodeX == ControllerMappingLibrary.AXIS_10) { button = OculusApi.RawButton.RIndexTrigger; return true; }
                return false;
            }
            return false;
        }
    }
}
