using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRProject
{
    public class Packet
    {
        private List<byte> dataList;

        private byte[] data = null;

        private int position = 0;

        public byte[] Data
        {
            get
            {
                if (data == null)
                    data = dataList.ToArray();
                return data;
            }

            set
            {
                data = Data;
            }
        }

        #region Constructors
        public Packet()
        {
            this.dataList = new List<byte>();
        }

        public Packet(byte[] data)
        {
            this.dataList = new List<byte>(data);
        }
        #endregion

        #region Adders
        public void AddInt16(Int16 value)
        {
            dataList.AddRange(BitConverter.GetBytes(value));
            data = null;
        }
        public void AddInt32(Int32 value)
        {
            dataList.AddRange(BitConverter.GetBytes(value));
            data = null;
        }
        public void AddInt64(Int64 value)
        {
            dataList.AddRange(BitConverter.GetBytes(value));
            data = null;
        }
        public void AddUInt16(UInt16 value)
        {
            dataList.AddRange(BitConverter.GetBytes(value));
            data = null;
        }
        public void AddUInt32(UInt32 value)
        {
            dataList.AddRange(BitConverter.GetBytes(value));
            data = null;
        }
        public void AddUInt64(UInt64 value)
        {
            dataList.AddRange(BitConverter.GetBytes(value));
            data = null;
        }
        public void AddFloat(float value)
        {
            dataList.AddRange(BitConverter.GetBytes(value));
            data = null;
        }
        public void AddDouble(double value)
        {
            dataList.AddRange(BitConverter.GetBytes(value));
            data = null;
        }
        public void AddString(String message)
        {
            dataList.AddRange(BitConverter.GetBytes(message.Length));
            dataList.AddRange(Encoding.UTF8.GetBytes(message));
            data = null;
        }
        public void AddBool(bool value)
        {
            dataList.AddRange(BitConverter.GetBytes(value));
            data = null;
        }
        #endregion

        #region Getters
        public Int16 NextInt16()
        {
            Int16 result = BitConverter.ToInt16(Data, position);
            position += sizeof(Int16);
            return result;
        }
        public Int32 NextInt32()
        {
            Int32 result = BitConverter.ToInt32(Data, position);
            position += sizeof(Int32);
            return result;
        }
        public Int64 NextInt64()
        {
            Int64 result = BitConverter.ToInt64(Data, position);
            position += sizeof(Int64);
            return result;
        }
        public UInt16 NextUInt16()
        {
            UInt16 result = BitConverter.ToUInt16(Data, position);
            position += sizeof(UInt16);
            return result;
        }
        public UInt32 NextUInt32()
        {
            UInt32 result = BitConverter.ToUInt32(Data, position);
            position += sizeof(UInt32);
            return result;
        }
        public UInt64 NextUInt64()
        {
            UInt64 result = BitConverter.ToUInt64(Data, position);
            position += sizeof(UInt64);
            return result;
        }
        public float NextFloat()
        {
            float result = BitConverter.ToSingle(Data, position);
            position += sizeof(float);
            return result;
        }
        public double NextDouble()
        {
            double result = BitConverter.ToDouble(Data, position);
            position += sizeof(double);
            return result;
        }
        public String NextString()
        {
            int length = BitConverter.ToInt32(Data, position);
            position += sizeof(int);
            String result = UTF8Encoding.ASCII.GetString(Data, position, length);
            position += length;
            return result;
        }
        public bool NextBool()
        {
            bool result = BitConverter.ToBoolean(Data, position);
            position += sizeof(bool);
            return result;
        }
        #endregion

        #region Position Modifiers
        public void Skip(int numBytes)
        {
            position += numBytes;
        }

        public void Reset()
        {
            position = 0;
        }

        public void JumpTo(int numBytes)
        {
            position = numBytes;
        }
        #endregion
    }
}
