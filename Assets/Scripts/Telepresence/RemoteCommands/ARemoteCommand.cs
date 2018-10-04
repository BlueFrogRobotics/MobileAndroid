using BlueQuark.Remote;

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace BlueQuark.Remote
{
    /// <summary>
    /// Represents an embedded command sended remotely and executed on the robot.
    /// </summary>
    public abstract partial class ARemoteCommand
    {
        private bool mIsExecuted;

        /// <summary>
        /// Timestamp of the command execution
        /// </summary>
        public TimeStamp Timestamp { get; set; }

        /// <summary>
        /// Parameters of the command
        /// </summary>
        public RemoteCommandParameters Parameters { get; set; }

        /// <summary>
        /// Check if the command has been executed
        /// </summary>
        public bool IsExecuted { get { return mIsExecuted; } }

        /// <summary>
        /// Base constructor for deserialization
        /// </summary>
        public ARemoteCommand() { }

        /// <summary>
        /// Execute the command behaviour defined in ExecuteImpl
        /// </summary>
        public void Execute()
        {
            Timestamp = new TimeStamp();
            //Utils.LogI(LogModule.COMMAND, LogStatus.RUNNING, this);
            ExecuteImpl();
            mIsExecuted = true;
        }

        /// <summary>
        /// Execute the command behaviour.
        /// </summary>
        protected abstract void ExecuteImpl();

        /// <summary>
        /// Update params in existing command. Allows to avoid recreate a new command for the same purpose.
        /// </summary>
        /// <param name="iIntegers">Integers that will replace existing Integers in Parameters</param>
        /// <returns>The updated command</returns>
        public ARemoteCommand UpdateParams(params int[] iIntegers)
        {
            if (Parameters == null)
                Parameters = new RemoteCommandParameters();
            int lLength = iIntegers.Length;
            Parameters.Integers = new int[lLength];
            Array.Copy(iIntegers, Parameters.Integers, lLength);
            return this;
        }

        /// <summary>
        /// Update params in existing command. Allows to avoid recreate a new command for the same purpose.
        /// </summary>
        /// <param name="iSingles">Singles that will replace existing Singles in Parameters</param>
        /// <returns>The updated command</returns>
        public ARemoteCommand UpdateParams(params float[] iSingles)
        {
            if (Parameters == null)
                Parameters = new RemoteCommandParameters();
            int lLength = iSingles.Length;
            Parameters.Singles = new float[lLength];
            Array.Copy(iSingles, Parameters.Singles, lLength);
            return this;
        }

        /// <summary>
        /// Update params in existing command. Allows to avoid recreate a new command for the same purpose.
        /// </summary>
        /// <param name="iStrings">Strings that will replace existing Strings in Parameters</param>
        /// <returns>The updated command</returns>
        public ARemoteCommand UpdateParams(params string[] iStrings)
        {
            if (Parameters == null)
                Parameters = new RemoteCommandParameters();
            int lLength = iStrings.Length;
            Parameters.Strings = new string[lLength];
            Array.Copy(iStrings, Parameters.Strings, lLength);
            return this;
        }

        /// <summary>
        /// Create a custom byte array representing the command.
        /// </summary>
        /// <returns>The command serialization</returns>
        public byte[] Serialize()
        {
            string lClassName = GetType().Name;
            byte lNameArrayLength = (byte)lClassName.Length;

            byte[] lNameArray = Encoding.ASCII.GetBytes(lClassName);
            byte[] lParamArray = Parameters != null ? Parameters.Serialize() : new byte[0];
            byte[] lExecTimeArray = Timestamp != null ? Timestamp.Serialize() : new byte[0];

            int lParamArrayLength = lParamArray.Length;
            int lExecTimeArrayLength = lExecTimeArray.Length;

            byte[] oCompleteData = new byte[sizeof(byte)
                                            + lNameArrayLength
                                            + lParamArrayLength
                                            + lExecTimeArrayLength];

            oCompleteData[0] = lNameArrayLength;
            Array.Copy(lNameArray, 0, oCompleteData, sizeof(byte), lNameArrayLength);
            Array.Copy(lParamArray, 0, oCompleteData, sizeof(byte) + lNameArrayLength, lParamArrayLength);
            Array.Copy(lExecTimeArray, 0, oCompleteData, sizeof(byte) + lNameArrayLength + lParamArrayLength, lExecTimeArrayLength);

            return oCompleteData;
        }

        /// <summary>
        /// Create a command from a string containing successive bytes, separate by ","
        /// </summary>
        /// <param name="iBytes">The byte string</param>
        /// <param name="iSeparator">Separator character between each byte</param>
        /// <param name="iAssembly">The assembly where the command belongs</param>
        /// <returns>The command, created and init</returns>
        public static ARemoteCommand Deserialize(string iBytes, char iSeparator = ' ', string iAssembly = "OS")
        {
            string[] lSplit = iBytes.Split(iSeparator);
            int lNbBytes = lSplit.Length;
            byte[] lBytes = new byte[lNbBytes];
            for (int i = 0; i < lNbBytes; ++i)
                lBytes[i] = byte.Parse(lSplit[i]);
            return Deserialize(lBytes, iAssembly);
        }

        /// <summary>
        /// Create a command from a byte array
        /// </summary>
        /// <param name="iBytes">The byte array</param>
        /// <param name="iAssembly">The assembly where the command belongs</param>
        /// <returns>The command, created and init</returns>
        public static ARemoteCommand Deserialize(byte[] iBytes, string iAssembly = "OS")
        {
            byte lNameLength = iBytes[0];
            string lName = Encoding.ASCII.GetString(iBytes, sizeof(byte), lNameLength);
            ARemoteCommand oCommand = Activator.CreateInstance(iAssembly, "BlueQuark.Remote." + lName).Unwrap() as ARemoteCommand;
            byte[] lParamAndTime = iBytes.Skip(sizeof(byte) + lNameLength).ToArray();
            int lIndexTime = 0;
            oCommand.Parameters = RemoteCommandParameters.Deserialize(lParamAndTime, out lIndexTime);
            oCommand.Timestamp = (lParamAndTime.Length - lIndexTime) >= 9 ? TimeStamp.Deserialize(lParamAndTime, lIndexTime) : null;
            return oCommand;
        }

        /// <summary>
        /// String basic serialization of the command
        /// </summary>
        /// <returns>Command string value for debug</returns>
        public override string ToString()
        {
            string oString = base.ToString();

            if (Parameters != null)
                oString += " : " + Parameters.ToString();

            if (Timestamp != null)
                oString += " : " + Timestamp.ToString();

            return oString;
        }
    }
}