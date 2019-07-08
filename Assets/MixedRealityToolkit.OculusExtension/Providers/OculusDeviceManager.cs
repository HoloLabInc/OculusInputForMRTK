using Microsoft.MixedReality.Toolkit.Oculus;
using Microsoft.MixedReality.Toolkit.Oculus.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [MixedRealityDataProvider(
    typeof(IMixedRealityInputSystem),
    SupportedPlatforms.WindowsStandalone | SupportedPlatforms.Android,
    "Oculus Device Manager")]
    public class OculusDeviceManager : BaseInputDeviceManager
    {
        public OculusDeviceManager(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem, 
            string name = null, 
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(registrar, inputSystem, name, priority, profile)
        {
        }

        private const float DeviceRefreshInterval = 3.0f;

        /// <summary>
        /// Dictionary to capture all active controllers detected
        /// </summary>
        private readonly Dictionary<OculusApi.Controller, BaseOculusController> ActiveControllers = new Dictionary<OculusApi.Controller, BaseOculusController>();

        private float deviceRefreshTimer;
        private OculusApi.Controller lastDeviceList;

        private int fixedUpdateCount = 0;

        /// <inheritdoc/>
        public override IMixedRealityController[] GetActiveControllers()
        {
            var list = new List<IMixedRealityController>();

            foreach (var controller in ActiveControllers.Values)
            {
                list.Add(controller);
            }

            return list.ToArray();
        }


        /// <inheritdoc />
        //public override void Enable() {}

        /// <inheritdoc />
        public override void Update()
        {

            OculusApi.stepType = OculusApi.Step.Render;

            fixedUpdateCount = 0;

            deviceRefreshTimer += Time.unscaledDeltaTime;

            if (deviceRefreshTimer >= DeviceRefreshInterval)
            {

                deviceRefreshTimer = 0.0f;
                RefreshDevices();
            }

            foreach (var controller in ActiveControllers)
            {
                controller.Value?.UpdateController();
            }
        }

        //public override void FixedUpdate()
        //{
        //    base.FixedUpdate();
        //    OculusApi.stepType = OculusApi.Step.Physics;

        //    double predictionSeconds = (double)fixedUpdateCount * Time.fixedDeltaTime / Mathf.Max(Time.timeScale, 1e-6f);
        //    fixedUpdateCount++;

        //    OculusApi.UpdateNodePhysicsPoses(0, predictionSeconds);
        //}

        /// <inheritdoc />
        public override void Disable()
        {
            foreach (var activeController in ActiveControllers)
            {
                var controller = GetOrAddController(activeController.Key, false);

                if (controller != null)
                {
                    MixedRealityToolkit.InputSystem?.RaiseSourceLost(controller.InputSource, controller);
                }
            }

            ActiveControllers.Clear();
        }

        private BaseOculusController GetOrAddController(OculusApi.Controller controllerMask, bool addController = true)
        {
            //If a device is already registered with the ID provided, just return it.
            if (ActiveControllers.ContainsKey(controllerMask))
            {
                var controller = ActiveControllers[controllerMask];
                Debug.Assert(controller != null);
                return controller;
            }

            if (!addController) { return null; }

            var currentControllerType = GetCurrentControllerType(controllerMask);
            Type controllerType = null;

            switch (currentControllerType)
            {
                case SupportedControllerType.OculusTouch:
                    controllerType = typeof(NativeOculusTouchController);
                    break;
                //case SupportedControllerType.OculusGo:
                //    controllerType = typeof(OculusGoController);
                //    break;
                //case SupportedControllerType.OculusRemote:
                //    controllerType = typeof(OculusRemoteController);
                //    break;
            }

            var controllingHand = Handedness.Any;

            //Determine Handedness of the current controller
            switch (controllerMask)
            {
                case OculusApi.Controller.LTrackedRemote:
                case OculusApi.Controller.LTouch:
                    controllingHand = Handedness.Left;
                    break;
                case OculusApi.Controller.RTrackedRemote:
                case OculusApi.Controller.RTouch:
                    controllingHand = Handedness.Right;
                    break;
                case OculusApi.Controller.Touchpad:
                case OculusApi.Controller.Gamepad:
                case OculusApi.Controller.Remote:
                    controllingHand = Handedness.Both;
                    break;
            }

            //TODO: FIX
            //var pointers = RequestPointers(typeof(BaseOculusController), controllingHand);
            var pointers = RequestPointers(SupportedControllerType.OculusTouch, controllingHand);
            var inputSource = MixedRealityToolkit.InputSystem?.RequestNewGenericInputSource($"Oculus Controller {controllingHand}", pointers);
            var detectedController = new BaseOculusController(TrackingState.NotTracked, controllingHand, inputSource);
            detectedController.controllerType = controllerMask;
            switch (controllingHand)
            {
                case Handedness.Left:
                    detectedController.NodeType = OculusApi.Node.HandLeft;
                    break;
                case Handedness.Right:
                    detectedController.NodeType = OculusApi.Node.HandRight;
                    break;
            }

            if (!detectedController.SetupConfiguration(controllerType))
            {
                // Controller failed to be setup correctly.
                // Return null so we don't raise the source detected.
                Debug.LogError($"Failed to Setup {controllerType.Name} controller");
                return null;
            }

            for (int i = 0; i < detectedController.InputSource?.Pointers?.Length; i++)
            {
                detectedController.InputSource.Pointers[i].Controller = detectedController;
            }

            //TODO: this code is not include in mrtk
            //detectedController.TryRenderControllerModel(controllerType);

            ActiveControllers.Add(controllerMask, detectedController);
            return detectedController;
        }

        private void RefreshDevices()
        {
            // override locally derived active and connected controllers if plugin provides more accurate data
            OculusApi.connectedControllerTypes = (OculusApi.Controller)OculusApi.GetConnectedControllers();
            OculusApi.activeControllerType = (OculusApi.Controller)OculusApi.GetActiveController();

            //Noticed that the "active" controllers also mark the Tracked state.
            //Debug.LogError($"Connected =[{OculusApi.connectedControllerTypes}] - Active = [{OculusApi.activeControllerType}]");

            if (OculusApi.connectedControllerTypes == OculusApi.Controller.None) { return; }

            if (ActiveControllers.Count > 0)
            {
                OculusApi.Controller[] activeControllers = new OculusApi.Controller[ActiveControllers.Count];
                ActiveControllers.Keys.CopyTo(activeControllers, 0);

                if (lastDeviceList != OculusApi.Controller.None && OculusApi.connectedControllerTypes != lastDeviceList)
                {
                    foreach (var activeController in activeControllers)
                    {
                        if (activeController == OculusApi.Controller.Touch && ((OculusApi.Controller.LTouch & OculusApi.connectedControllerTypes) != OculusApi.Controller.LTouch))
                        {
                            RaiseSourceLost(OculusApi.Controller.LTouch);
                        }
                        else if (activeController == OculusApi.Controller.Touch && ((OculusApi.Controller.RTouch & OculusApi.connectedControllerTypes) != OculusApi.Controller.RTouch))
                        {
                            RaiseSourceLost(OculusApi.Controller.RTouch);
                        }
                        else if ((activeController & OculusApi.connectedControllerTypes) != activeController)
                        {
                            RaiseSourceLost(activeController);
                        }
                    }
                }
            }

            for (var i = 0; i < OculusApi.Controllers.Length; i++)
            {
                if (OculusApi.ShouldResolveController(OculusApi.Controllers[i].controllerType, OculusApi.connectedControllerTypes))
                {
                    if (OculusApi.Controllers[i].controllerType == OculusApi.Controller.Touch)
                    {
                        if (!ActiveControllers.ContainsKey(OculusApi.Controller.LTouch)) { RaiseSourceDetected(OculusApi.Controller.LTouch); }
                        if (!ActiveControllers.ContainsKey(OculusApi.Controller.RTouch)) { RaiseSourceDetected(OculusApi.Controller.RTouch); }
                    }
                    else if (!ActiveControllers.ContainsKey(OculusApi.Controllers[i].controllerType))
                    {
                        RaiseSourceDetected(OculusApi.Controllers[i].controllerType);
                    }
                }
            }

            lastDeviceList = OculusApi.connectedControllerTypes;
        }

        private void RaiseSourceDetected(OculusApi.Controller controllerType)
        {
            var controller = GetOrAddController(controllerType);

            if (controller != null)
            {
                MixedRealityToolkit.InputSystem?.RaiseSourceDetected(controller.InputSource, controller);
            }
        }

        private void RaiseSourceLost(OculusApi.Controller activeController)
        {
            var controller = GetOrAddController(activeController, false);

            if (controller != null)
            {
                MixedRealityToolkit.InputSystem?.RaiseSourceLost(controller.InputSource, controller);
            }

            ActiveControllers.Remove(activeController);
        }

        private SupportedControllerType GetCurrentControllerType(OculusApi.Controller controllerMask)
        {
            switch (controllerMask)
            {
                case OculusApi.Controller.LTouch:
                case OculusApi.Controller.RTouch:
                case OculusApi.Controller.Touch:
                    return SupportedControllerType.OculusTouch;
                case OculusApi.Controller.Remote:
                    return SupportedControllerType.OculusRemote;
                
                //TODO: Skip OculusGo
                //case OculusApi.Controller.LTrackedRemote:
                //case OculusApi.Controller.RTrackedRemote:
                //    Debug.LogError($"{controllerMask} found - assuming Go?");
                //    return SupportedControllerType.OculusGo;
                default:
                    break;
            }

            Debug.LogWarning($"{controllerMask} does not have a defined controller type, falling back to generic controller type");

            return SupportedControllerType.GenericOpenVR;
        }
    }
}
