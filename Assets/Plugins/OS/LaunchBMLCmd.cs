using UnityEngine;
using System.Collections;
using System;

namespace Buddy.Command
{
    /// <summary>
    /// When executed : launches a BML with the given name.
    /// </summary>
    public class LaunchBMLCmd : ACommand
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public LaunchBMLCmd()
        {

        }

        /// <summary>
        /// When executed : launches a BML with the given name.
        /// </summary>
        /// <param name="iName">Name of the BML.</param>
        public LaunchBMLCmd(string iName)
        {
            Parameters = new CommandParam();
            Parameters.Strings = new string[] { iName };
        }

        /// <summary>
        /// Execute the command behaviour.
        /// </summary>
        protected override void ExecuteImpl()
        {

        }
    }
}