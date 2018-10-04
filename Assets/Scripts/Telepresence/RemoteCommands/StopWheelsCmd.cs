using UnityEngine;

using System.Collections;
using System;

namespace BlueQuark.Remote
{
    /// <summary>
    /// When executed : Stops wheels motors.
    /// </summary>
    public sealed class StopWheelsCmd : ARemoteCommand
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public StopWheelsCmd()
        {
        }

        /// <summary>
        /// Execute the command behaviour.
        /// </summary>
        protected override void ExecuteImpl()
        {
            //Buddy.Actuators.Wheels.Stop(); // Embedded only
        }
    }
}