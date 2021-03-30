using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Hs.Foundation.Utility
{
    #region Hash相关助手类

    /// <summary>
    /// Hash相关助手类
    /// </summary>
    public class HashHelper
    {
        #region 私有变量

        /// <summary>
        /// 固定值
        /// </summary>
        private const uint m = 0x5bd1e995;

        /// <summary>
        /// 固定值
        /// </summary>
        private const int r = 24;

        #endregion

        #region byte转Uint的数据存放对象

        /// <summary>
        /// byte转Uint的数据存放对象
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        struct ByteToUintObject
        {
            /// <summary>
            /// 字节数组
            /// </summary>
            [FieldOffset(0)]
            public byte[] Bytes;

            /// <summary>
            /// 存放整形数组
            /// </summary>
            [FieldOffset(0)]
            public uint[] UInts;
        }

        #endregion

        #region 计算Hash值

        /// <summary>
        /// 计算Hash值
        /// </summary>
        /// <param name="data">计算的数据</param>
        /// <returns></returns>
        public static uint Hash(string data)
            => Hash(Encoding.UTF8.GetBytes(data));

        /// <summary>
        /// 计算Hash值
        /// </summary>
        /// <param name="data">计算的数据</param>
        /// <param name="seed">种子值</param>
        /// <returns></returns>
        public static uint Hash(string data, uint seed)
           => Hash(Encoding.UTF8.GetBytes(data), seed);

        /// <summary>
        /// 计算Hash值
        /// </summary>
        /// <param name="data">计算的数据</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Hash(byte[] data)
            => Hash(data, 0xc58f1a7b);

        /// <summary>
        /// 计算Hash值
        /// </summary>
        /// <param name="data">计算的数据</param>
        /// <param name="seed">种子值</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Hash(byte[] data, uint seed)
        {
            int length = data.Length;
            if (length == 0)
                return 0;

            uint h = seed ^ (uint)length;
            int currentIndex = 0;
            uint[] hackArray = new ByteToUintObject { Bytes = data }.UInts;
            while (length >= 4)
            {
                uint k = hackArray[currentIndex++];
                k *= m;
                k ^= k >> r;
                k *= m;

                h *= m;
                h ^= k;
                length -= 4;
            }
            currentIndex *= 4;
            switch (length)
            {
                case 3:
                    h ^= (ushort)(data[currentIndex++] | data[currentIndex++] << 8);
                    h ^= (uint)data[currentIndex] << 16;
                    h *= m;
                    break;
                case 2:
                    h ^= (ushort)(data[currentIndex++] | data[currentIndex] << 8);
                    h *= m;
                    break;
                case 1:
                    h ^= data[currentIndex];
                    h *= m;
                    break;
                default:
                    break;
            }

            h ^= h >> 13;
            h *= m;
            h ^= h >> 15;

            return h;
        }

        #endregion
    }

    #endregion
}
