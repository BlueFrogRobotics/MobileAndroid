using UnityEngine;
using System.Collections;
using System;

namespace Buddy.Command
{
    /// <summary>
    /// When executed : launches a BML with the given category.
    /// </summary>
    public class LaunchRandomBMLCmd : ACommand
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public LaunchRandomBMLCmd()
        {

        }

        /// <summary>
        /// When executed : launches a BML with the given category.
        /// </summary>
        /// <param name="iCategory">The category to launch from.</param>
        public LaunchRandomBMLCmd(string iCategory)
        {
            Parameters = new CommandParam();
            Parameters.Strings = new string[] { iCategory };
        }

        /// <summary>
        /// When executed : launches a BML with the given category.
        /// </summary>
        /// <param name="iType">The mood type category to launch from.</param>
        public LaunchRandomBMLCmd(MoodType iType)
        {
            Parameters = new CommandParam();
            Parameters.Strings = new string[] { iType.ToString().ToLower() };
        }

        /// <summary>
        /// Execute the command behaviour.
        /// </summary>
        protected override void ExecuteImpl()
        {

        }
    }
}