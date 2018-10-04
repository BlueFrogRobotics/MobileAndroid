using UnityEngine;

using System.Collections;
using System;

namespace BlueQuark.Remote
{
    /// <summary>
    /// When executed : Set robot mood.
    /// </summary>
    public sealed class SetMoodCmd : ARemoteCommand
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public SetMoodCmd(string iMood)
        {
            Parameters = new RemoteCommandParameters();
            Parameters.Strings = new string[] { iMood };
        }

        /// <summary>
        /// Execute the command behaviour.
        /// </summary>
        protected override void ExecuteImpl()
        {
            //int lIdx = Parameters.Integers[0];

            //Buddy.Behaviour.SetMood((Mood)lIdx);
        }
    }
}