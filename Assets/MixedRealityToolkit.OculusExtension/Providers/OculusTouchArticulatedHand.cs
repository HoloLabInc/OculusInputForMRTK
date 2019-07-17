using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Oculus.Input
{
    [MixedRealityController(
        SupportedControllerType.ArticulatedHand,
        new[] { Handedness.Left, Handedness.Right} 
        )]
    public class OculusTouchArticulatedHand : BaseOculusSource, IMixedRealityHand
    {
        public OculusTouchArticulatedHand(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null) : 
            base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <summary>
        /// The Windows Mixed Reality Controller default interactions.
        /// </summary>
        /// <remarks>A single interaction mapping works for both left and right controllers.</remarks>
        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Select", AxisType.Digital, DeviceInputType.Select, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(3, "Grab", AxisType.SingleAxis, DeviceInputType.TriggerPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(4, "Index Finger Pose", AxisType.SixDof, DeviceInputType.IndexFinger, MixedRealityInputAction.None)
        };

        public bool TryGetJoint(TrackedHandJoint joint, out MixedRealityPose pose)
        {
            return jointPoses.TryGetValue(joint, out pose);
        }

        protected static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;
        protected readonly Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();
        
        /// <summary>
        /// Update the controller data from the provided platform state
        /// </summary>
        public override void UpdateController()
        {
            if (!Enabled) { return; }

            base.UpdateController();

            UpdateHandPose();

            for (int i = 0; i < Interactions?.Length; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.IndexFinger:
                        UpdateIndexFingerData(Interactions[i]);
                        break;
                }
            }
        }

        private void UpdateHandPose()
        {
            if(ControllerHandedness == Handedness.Right)
            {
                UpdateRightHandPose();
            }
            else
            {
                UpdateLeftHandPose();
            }
        }

        private void UpdateRightHandPose()
        {
            var button = (OculusApi.RawButton)currentInputSourceState.Buttons;
            var pose = new ArticulatedHandPose();

            if (((button & OculusApi.RawButton.RIndexTrigger) == 0) &&
                ((button & OculusApi.RawButton.RHandTrigger) != 0))
            {
                pose = ArticulatedHandPose.GetGesturePose(ArticulatedHandPose.GestureId.Poke);
            }
            else if (
                ((button & OculusApi.RawButton.RIndexTrigger) != 0) &&
                ((button & OculusApi.RawButton.RHandTrigger) != 0) &&
                ((button & OculusApi.RawButton.RThumbstick) == 0))
            {
                pose = ArticulatedHandPose.GetGesturePose(ArticulatedHandPose.GestureId.ThumbsUp);
            }
            else if (
                ((button & OculusApi.RawButton.RIndexTrigger) != 0) &&
                ((button & OculusApi.RawButton.RHandTrigger) != 0) &&
                ((button & OculusApi.RawButton.RThumbstick) != 0))
            {
                pose = ArticulatedHandPose.GetGesturePose(ArticulatedHandPose.GestureId.Grab);
            }
            else
            {
                pose = ArticulatedHandPose.GetGesturePose(ArticulatedHandPose.GestureId.Open);
            }
        }

        private void UpdateLeftHandPose()
        {
            var button = (OculusApi.RawButton)currentInputSourceState.Buttons;
            var pose = new ArticulatedHandPose();

            if (((button & OculusApi.RawButton.LIndexTrigger) == 0) &&
                ((button & OculusApi.RawButton.LHandTrigger) != 0))
            {
                pose = ArticulatedHandPose.GetGesturePose(ArticulatedHandPose.GestureId.Poke);
            }
            else if (
                ((button & OculusApi.RawButton.LIndexTrigger) != 0) &&
                ((button & OculusApi.RawButton.LHandTrigger) != 0) &&
                ((button & OculusApi.RawButton.LThumbstick) == 0))
            {
                pose = ArticulatedHandPose.GetGesturePose(ArticulatedHandPose.GestureId.ThumbsUp);
            }
            else if (
                ((button & OculusApi.RawButton.LIndexTrigger) != 0) &&
                ((button & OculusApi.RawButton.LHandTrigger) != 0) &&
                ((button & OculusApi.RawButton.LThumbstick) != 0))
            {
                pose = ArticulatedHandPose.GetGesturePose(ArticulatedHandPose.GestureId.Grab);
            }
            else
            {
                pose = ArticulatedHandPose.GetGesturePose(ArticulatedHandPose.GestureId.Open);
            }
        }

        private MixedRealityPose currentIndexPose = MixedRealityPose.ZeroIdentity;

        private void UpdateIndexFingerData(MixedRealityInteractionMapping interactionMapping)
        {
            // Update the interaction data source
            interactionMapping.PoseData = currentIndexPose;

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system Event if it enabled
                InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, currentIndexPose);
            }
        }

        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultInteractions);
        }
    }
}
