using UnityEngine;

using System.Collections;
using System;

namespace BlueQuark.Remote
{
    /// <summary>
    /// When executed : Controls the speed of the wheels, stops after the timeout.
    /// </summary>
    public sealed class SetWheelsVelocitiesCmd : ARemoteCommand
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public SetWheelsVelocitiesCmd()
        {
        }

        /// <summary>
        /// When executed : Sets robot linear (meters / s) and angular (deg / s) velocities.
        /// </summary>
        /// <param name="iLinearVelocity">Rate of change (in meters by second) of an object position with in a certain time range<see href="https://en.wikipedia.org/wiki/Angular_velocity">Article</see></param>
        /// <param name="iAngularVelocity">Rate (in degrees by second) at which it rotates around a chosen center point: that is, the time rate of change of its angular displacement relative to the origin<see href="https://physics.tutorvista.com/motion/linear-velocity.html">Article</see></param>
        public SetWheelsVelocitiesCmd(float iLinearVelocity, float iAngularVelocity)
        {
            Parameters = new RemoteCommandParameters();
            Parameters.Singles = new float[] { iLinearVelocity, iAngularVelocity };
        }

        /// <summary>
        /// Execute the command behaviour.
        /// </summary>
        protected override void ExecuteImpl()
        {
            float[] lInputFloats = Parameters.Singles;

            float lLinearVelocity = lInputFloats[0];
            float lAngularVelocity = lInputFloats[1];

            //Buddy.Actuators.Wheels.SetVelocities(lLinearVelocity, lAngularVelocity); // Embedded Only
        }
    }
}