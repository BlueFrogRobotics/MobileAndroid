using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace BlueQuark.Remote
{
    /// <summary>
    /// Represents parameters for command. Contains it own serialization implementation
    /// </summary>
    public sealed class RemoteCommandParameters
    {
        /// <summary>
        /// Integer storage.
        /// </summary>
        public int[] Integers { get; set; }

        /// <summary>
        /// Float storage.
        /// </summary>
        public float[] Singles { get; set; }

        /// <summary>
        /// Unicode string storage.
        /// </summary>
        public string[] Strings { get; set; }

        /// <summary>
        /// Serialization to byte array of the current param
        /// </summary>
        /// <returns>Parameter byte array equivalent</returns>
        internal byte[] Serialize()
        {
            List<byte[]> lDataBytes = new List<byte[]>();

            if (Integers != null) {
                byte lIntegersLength = (byte)Integers.Length;
                byte[] lCompleteArray = new byte[sizeof(byte) + sizeof(int) * lIntegersLength];
                lCompleteArray[0] = lIntegersLength;

                int lIndexInt = sizeof(byte);
                for (byte i = 0; i < lIntegersLength; ++i) {
                    Array.Copy(BitConverter.GetBytes(Integers[i]), 0, lCompleteArray, lIndexInt, sizeof(int));
                    lIndexInt += sizeof(int);
                }

                lDataBytes.Add(lCompleteArray);
            } else {
                lDataBytes.Add(new byte[] { 0 });
            }

            if (Singles != null) {
                byte lSinglesLength = (byte)Singles.Length;
                byte[] lCompleteArray = new byte[sizeof(byte) + sizeof(float) * lSinglesLength];
                lCompleteArray[0] = lSinglesLength;

                int lIndexFloat = sizeof(byte);
                for (byte i = 0; i < lSinglesLength; ++i) {
                    Array.Copy(BitConverter.GetBytes(Singles[i]), 0, lCompleteArray, lIndexFloat, sizeof(float));
                    lIndexFloat += sizeof(float);
                }

                lDataBytes.Add(lCompleteArray);
            } else {
                lDataBytes.Add(new byte[] { 0 });
            }

            if (Strings != null) {
                byte lStringsLength = (byte)Strings.Length;
                int lCompleteArraySize = sizeof(byte);

                for (byte i = 0; i < lStringsLength; ++i) {
                    string lString = Strings[i];
                    if (lString != null)
                        lCompleteArraySize += sizeof(short) + UnicodeEncoding.Unicode.GetByteCount(lString);
                    else {
                        --i;
                        --lStringsLength;
                    }
                }

                if (lStringsLength != 0) {
                    byte[] lCompleteArray = new byte[lCompleteArraySize];

                    lCompleteArray[0] = lStringsLength;

                    int lIndexString = sizeof(byte);
                    for (byte i = 0; i < lStringsLength; ++i) {
                        string lString = Strings[i];

                        byte[] lStringSizeArray = BitConverter.GetBytes((short)UnicodeEncoding.Unicode.GetByteCount(lString));
                        Array.Copy(lStringSizeArray, 0, lCompleteArray, lIndexString, lStringSizeArray.Length);
                        lIndexString += lStringSizeArray.Length;

                        byte[] lStringArray = UnicodeEncoding.Unicode.GetBytes(lString);
                        Array.Copy(lStringArray, 0, lCompleteArray, lIndexString, lStringArray.Length);
                        lIndexString += lStringArray.Length;
                    }
                    lDataBytes.Add(lCompleteArray);
                } else
                    lDataBytes.Add(new byte[] { 0 });
            } else {
                lDataBytes.Add(new byte[] { 0 });
            }

            byte[] oCompleteData = new byte[lDataBytes.Sum(x => x.Length)];

            int lIndex = 0;
            for (int i = 0; i < lDataBytes.Count; ++i) {
                byte[] lByteArray = lDataBytes[i];
                Array.Copy(lByteArray, 0, oCompleteData, lIndex, lByteArray.Length);
                lIndex += lByteArray.Length;
            }

            return oCompleteData;
        }

        /// <summary>
        /// Create a new CommandParam object from the input byte array
        /// </summary>
        /// <param name="iBytes">The byte array</param>
        /// <param name="ioIndex">The index at the end of the parameter area</param>
        /// <returns>The new CommandParam</returns>
        internal static RemoteCommandParameters Deserialize(byte[] iBytes, out int ioIndex)
        {
            if (iBytes.Length > 0) {
                RemoteCommandParameters oParam = new RemoteCommandParameters();

                int lIndex = 0;
                byte lNbInteger = iBytes[lIndex];
                lIndex += sizeof(byte);

                if (lNbInteger != 0) {
                    oParam.Integers = new int[lNbInteger];
                    for (byte i = 0; i < lNbInteger; ++i) {
                        oParam.Integers[i] = BitConverter.ToInt32(iBytes, lIndex);
                        lIndex += sizeof(int);
                    }
                }

                byte lNbFloat = iBytes[lIndex];
                lIndex += sizeof(byte);
                if (lNbFloat != 0) {
                    oParam.Singles = new float[lNbFloat];
                    for (byte i = 0; i < lNbFloat; ++i) {
                        oParam.Singles[i] = BitConverter.ToSingle(iBytes, lIndex);
                        lIndex += sizeof(float);
                    }
                }

                byte lNbString = iBytes[lIndex];
                lIndex += sizeof(byte);
                if (lNbString != 0) {
                    oParam.Strings = new string[lNbString];
                    for (byte i = 0; i < lNbString; ++i) {
                        short lStringLength = BitConverter.ToInt16(iBytes, lIndex);
                        lIndex += sizeof(short);
                        oParam.Strings[i] = Encoding.Unicode.GetString(iBytes, lIndex, lStringLength);
                        lIndex += lStringLength;
                    }
                }

                ioIndex = lIndex;
                return oParam;
            }
            ioIndex = 0;
            return null;
        }
    }
}