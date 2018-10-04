using System;
using System.Linq;
using System.Collections.Generic;

namespace BlueQuark.Remote
{
    /// <summary>
    /// Represents a timestamp data
    /// </summary>
    public sealed class TimeStamp
    {
        public short Millisecond { get; set; }

        public byte Second { get; set; }

        public byte Minute { get; set; }

        public byte Hour { get; set; }

        public byte Day { get; set; }

        public byte Month { get; set; }

        public short Year { get; set; }

        internal TimeStamp()
        {
            DateTime lTime = DateTime.Now;
            Millisecond = (short)lTime.Millisecond;
            Second = (byte)lTime.Second;
            Minute = (byte)lTime.Minute;
            Hour = (byte)lTime.Hour;
            Day = (byte)lTime.Day;
            Month = (byte)lTime.Month;
            Year = (short)lTime.Year;
        }

        internal byte[] Serialize()
        {
            IEnumerable<byte> oBytes =
                BitConverter.GetBytes(Millisecond)
                .Concat(new byte[] { Second })
                .Concat(new byte[] { Minute })
                .Concat(new byte[] { Hour })
                .Concat(new byte[] { Day })
                .Concat(new byte[] { Month })
                .Concat(BitConverter.GetBytes(Year));

            return oBytes.ToArray();
        }

        internal static TimeStamp Deserialize(byte[] iBytes, int iIndex = 0)
        {
            TimeStamp oTime = new TimeStamp();

            oTime.Millisecond = BitConverter.ToInt16(iBytes, iIndex);
            iIndex += sizeof(short);
            oTime.Second = iBytes[iIndex];
            iIndex += sizeof(byte);
            oTime.Minute = iBytes[iIndex];
            iIndex += sizeof(byte);
            oTime.Hour = iBytes[iIndex];
            iIndex += sizeof(byte);
            oTime.Day = iBytes[iIndex];
            iIndex += sizeof(byte);
            oTime.Month = iBytes[iIndex];
            iIndex += sizeof(byte);
            oTime.Year = BitConverter.ToInt16(iBytes, iIndex);

            return oTime;
        }
    }
}