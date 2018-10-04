using UnityEngine;

using System;
using System.Collections;

namespace BlueQuark.Remote
{
    /// <summary>
    /// When executed : Set the angular position of the motor at a specific speed
    /// </summary>
    public sealed class SetYesHingePositionCmd : ARemoteCommand
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public SetYesHingePositionCmd()
        {
        }

        /// <summary>
        /// When executed : Set the angular position of the motor at a specific speed
        /// </summary>
        /// <param name="iAngle">Angle in degrees of the angular position</param>
        /// <param name="iSpeed">Speed in degrees/sec of the motor</param>
        public SetYesHingePositionCmd(float iAngle, float iSpeed)
        {
            Parameters = new RemoteCommandParameters();
            Parameters.Singles = new float[] { iAngle, iSpeed };
        }

        /// <summary>
        /// Execute the command behaviour.
        /// </summary>
        protected override void ExecuteImpl()
        {
            float[] lInputFloats = Parameters.Singles;
            float lAngle = lInputFloats[0];
            float lSpeed = lInputFloats[1];

            //Buddy.Actuators.Head.Yes.SetPosition(lAngle, lSpeed);  // Embedded Only
        }
    }
}