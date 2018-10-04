using UnityEngine;

using System.Collections;
using System;

namespace BlueQuark.Remote
{
    /// <summary>
    /// When executed : Set robot mood.
    /// </summary>
    public sealed class StartAppCmd : ARemoteCommand
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public StartAppCmd(string iAppName)
        {
            Parameters = new RemoteCommandParameters();
            Parameters.Strings = new string[] { iAppName };
        }

        /// <summary>
        /// Execute the command behaviour.
        /// </summary>
        protected override void ExecuteImpl()
        {
            //Buddy.Platform.Application.StartApp(Parameters.Strings[0]);
        }
    }
}