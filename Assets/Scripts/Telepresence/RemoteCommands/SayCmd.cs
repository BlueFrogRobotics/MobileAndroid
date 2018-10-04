using UnityEngine;

using System.Collections;
using System;

namespace BlueQuark.Remote
{
    /// <summary>
    /// When executed : Set robot mood.
    /// </summary>
    public sealed class SayCmd : ARemoteCommand
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public SayCmd(string iUtterance)
        {
            Parameters = new RemoteCommandParameters();
            Parameters.Strings = new string[] { iUtterance };
        }

        /// <summary>
        /// Execute the command behaviour.
        /// </summary>
        protected override void ExecuteImpl()
        {
            //Buddy.Vocal.Say(Parameters.Strings[0]);
        }
    }
}