using System;
using System.Runtime.CompilerServices;

namespace Hs.Foundation.Utility
{
    #region 生成全局唯一值

    /// <summary>
    /// 生成全局唯一值
    /// </summary>
    public class UniqueObjectHelper
    {
        #region 时间随机相关变量

        /// <summary>
        /// 开始的值
        /// </summary>
        private static int startValue = 0;

        /// <summary>
        /// 开始的时间字符串
        /// </summary>
        private static string startDateTimeString = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss");

        /// <summary>
        /// 开始的时间整形
        /// </summary>
        private static long startDateTimeNumber = long.Parse(startDateTimeString);

        /// <summary>
        /// 要生成的字符串的总长度
        /// </summary>
        private const int stringLength = 20;

        /// <summary>
        /// 累到最大值
        /// </summary>
        private const int maxValue = 999999;

        /// <summary>
        /// 同步对象
        /// </summary>
        private static readonly object lockDateObject = new object();

        #endregion

        #region 雪花算法随机数相关变量

        /// <summary>
        /// 开始时间截(2018-08-08 08:08:08)
        /// </summary>
        private const long startTimestamp = 1533686888000;

        /// <summary>
        /// 机器id所占的位数
        /// </summary>
        private const int machineIdBitLength = 5;

        /// <summary>
        /// 数据中心id所占的位数
        /// </summary>
        private const int datacenterIdBitLength = 5;

        /// <summary>
        /// 序列在id中占的位数
        /// </summary>
        private const int sequenceBitLenth = 12;

        /// <summary>
        /// 机器ID的最大值
        /// </summary>
        private const long maxMachineId = -1 ^ (-1 << machineIdBitLength);

        /// <summary>
        /// 数据中心ID的最大值
        /// </summary>
        private const long maxDatacenterId = -1 ^ (-1 << datacenterIdBitLength);

        /// <summary>
        /// 机器ID向左移12位
        /// </summary>
        private const int machineIdLeft = sequenceBitLenth;

        /// <summary>
        /// 数据中心ID左移17
        /// </summary>
        private const int datacenterIdIdLeft = sequenceBitLenth + machineIdBitLength;

        /// <summary>
        /// 时间截向左移22位(5+5+12)
        /// </summary>
        private const int timestampLeft = sequenceBitLenth + machineIdBitLength + datacenterIdBitLength;

        /// <summary>
        /// 一微秒内可以产生计数，如果达到该值则等到下一微妙在进行生成
        /// </summary>
        private const long maxSequence = -1L ^ (-1L << sequenceBitLenth);

        /// <summary>
        /// 毫秒内序列
        /// </summary>
        private static long sequence = 0;

        /// <summary>
        /// 上次生成ID的时间截
        /// </summary>
        private static long lastTimestamp = -1;

        /// <summary>
        /// 同步对象
        /// </summary>
        private static readonly object lockSnowObject = new object();

        #endregion

        #region 获取下一毫秒时间戳

        /// <summary>
        /// 获取下一毫秒时间戳
        /// </summary>
        /// <param name="lastTimestamp"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long NextTimeMilliseconds(long lastTimestamp)
        {
            long timestamp = GetTimestamp();
            while (timestamp <= lastTimestamp)
                timestamp = GetTimestamp();

            return timestamp;
        }

        #endregion

        #region 生成当前时间戳

        /// <summary>
        /// 生成当前时间戳
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long GetTimestamp()
            => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        #endregion

        #region 根据时间生成唯一ID

        /// <summary>
        /// 根据时间生成唯一ID,长度固定为20，理论上每秒可以用生成最多999999个随机数(不同机器相同时间可能产生重复,同一台机器保证不重复)
        /// </summary>
        /// <returns></returns>
        public static string NewIdWithDate()
        {
            lock (lockDateObject) { return NewId(); }

            string NewId()
            {
                var nowDateTimeString = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss");
                var nowDateTimeNumber = long.Parse(nowDateTimeString);

                if (nowDateTimeNumber > startDateTimeNumber)
                {
                    startDateTimeString = nowDateTimeString;
                    startDateTimeNumber = nowDateTimeNumber;
                    startValue = 0;
                }

                var builder = new Span<char>(new char[stringLength]);
                var value = ++startValue;
                if (value <= maxValue)
                {
                    var valueString = value.ToString().PadLeft(maxValue.ToString().Length, '0');
                    nowDateTimeString.AsSpan().CopyTo(builder);

                    for (int i = nowDateTimeString.Length; i < stringLength; i++)
                        builder[i] = valueString[^(stringLength - i)];

                    return builder.ToString();
                }
                else
                    return NewId();
            }
        }

        #endregion

        #region 生成分布式唯一ID(雪花算法)

        /// <summary>
        /// 生成分布式唯一ID(雪花算法)
        /// </summary>
        /// <param name="machineId">机器ID(0~31)</param>
        /// <param name="datacenterId">数据中心ID(0~31)</param>
        /// <returns></returns>
        public static long NewIdWithSnowFlake(long machineId, long datacenterId)
        {
            if (machineId < 0 || machineId > maxMachineId)
                throw new ArgumentException($"machine Id can't be greater than {maxMachineId} or less than 0");

            if (datacenterId < 0 || datacenterId > maxDatacenterId)
                throw new ArgumentException($"datacenter Id can't be greater than {maxDatacenterId} or less than 0");

            lock (lockSnowObject) { return NewId(); }
            long NewId()
            {
                long nowTimestamp = GetTimestamp();

                //如果当前时间戳比上一次生成ID时时间戳还小，抛出异常，因为不能保证现在生成的ID之前没有生成过
                if (nowTimestamp < lastTimestamp)
                    throw new Exception($"Clock moved backwards.  Refusing to generate id for { lastTimestamp - nowTimestamp} milliseconds");

                if (lastTimestamp == nowTimestamp)
                {
                    //同一毫秒中生成ID,用&运算计算该毫秒内产生的计数是否已经到达上限
                    sequence = ++sequence & maxSequence;
                    if (sequence == 0)
                        //一毫秒内产生的ID计数已达上限，等待下一毫秒
                        nowTimestamp = NextTimeMilliseconds(lastTimestamp);
                }
                else
                    //不同毫秒生成ID，计数清0
                    sequence = 0;

                lastTimestamp = nowTimestamp;
                return ((nowTimestamp - startTimestamp) << timestampLeft) | (datacenterId << datacenterIdIdLeft) | (machineId << machineIdLeft) | sequence;
            }
        }

        #endregion

        #region 根据GUID生成唯一ID

        /// <summary>
        /// 根据GUID生成唯一ID
        /// </summary>
        /// <returns></returns>
        public static string NewIdWithGuid()
        {
            var bytes = Guid.NewGuid().ToByteArray();
            long i = 1;
            foreach (byte b in bytes)
                i *= (b + 1);
            
            return (i - DateTime.Now.Ticks).ToString("X");
        }

        #endregion
    }

    #endregion
}
