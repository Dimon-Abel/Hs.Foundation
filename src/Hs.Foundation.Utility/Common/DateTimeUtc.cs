using System;
using System.Runtime.CompilerServices;

namespace Hs.Foundation.Utility
{
    #region 自定义时间(UTC)

    /// <summary>
    /// 自定义时间(UTC)
    /// </summary>
    [Serializable]
    public struct DateTimeUtc : IComparable<DateTimeUtc>, IEquatable<DateTimeUtc>
    {
        #region 私有变量

        /// <summary>
        /// 当前的时间
        /// </summary>
        private DateTimeOffset? dateTime;

        #endregion

        #region 属性

        /// <summary>
        /// 当前的时间(UTC时间戳)
        /// </summary>
        public static DateTimeUtc Now => new DateTimeUtc(DateTimeOffset.UtcNow);

        /// <summary>
        /// 时间戳(秒)
        /// </summary>
        public long Timestamp
        {
            get => GetDateTime().ToUnixTimeSeconds();
            init => dateTime = ((DateTimeUtc)value).ToDateTimeOffset();
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="utcDateTime">UTC时间</param>
        public DateTimeUtc(DateTimeOffset utcDateTime) => dateTime = utcDateTime.UtcDateTime;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        public DateTimeUtc(long timestamp) => dateTime = ((DateTimeUtc)timestamp).ToDateTimeOffset();

        #endregion

        #region 获取时间

        /// <summary>
        /// 获取时间
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DateTimeOffset GetDateTime()
        {
            dateTime ??= DateTimeOffset.UtcNow;
            return dateTime.Value;
        }

        #endregion

        #region 转换为DateTimeOffset

        /// <summary>
        /// 转换为DateTimeOffset
        /// </summary>
        /// <returns></returns>
        public DateTimeOffset ToDateTimeOffset()
            => GetDateTime();

        #endregion

        #region 获取时间戳(毫秒)

        /// <summary>
        /// 获取时间戳(毫秒)
        /// </summary>
        /// <returns></returns>
        public long ToUnixTimeMilliseconds()
            => GetDateTime().ToUnixTimeMilliseconds();

        /// <summary>
        /// 获取当前时间的日期部分的时间戳(毫秒)
        /// </summary>
        /// <returns></returns>
        public long ToCurrentDayUnixTimeMilliseconds()
            => ToCurrentDayUnixTimeMilliseconds(DateTimeOffset.Now.Offset);

        /// <summary>
        /// 获取当前时间的日期部分的时间戳(毫秒)
        /// </summary>
        /// <param name="offset">与UTC时间的差</param>
        /// <returns></returns>
        public long ToCurrentDayUnixTimeMilliseconds(TimeSpan offset)
            => new DateTimeOffset(GetDateTime().Date, offset).ToUnixTimeMilliseconds();

        /// <summary>
        /// 获取当前时间的日期部分的的月的时间戳(毫秒)
        /// </summary>
        /// <returns></returns>
        public long ToCurrentMonthUnixTimeMilliseconds()
            => ToCurrentMonthUnixTimeMilliseconds(DateTimeOffset.Now.Offset);

        /// <summary>
        /// 获取当前时间的日期部分的的月的时间戳(毫秒)
        /// </summary>
        /// <param name="offset">与UTC时间的差</param>
        /// <returns></returns>
        public long ToCurrentMonthUnixTimeMilliseconds(TimeSpan offset)
        {
            var date = GetDateTime();
            return new DateTimeOffset(date.Year, date.Month, 1, 0, 0, 0, offset).ToUnixTimeMilliseconds();
        }

        #endregion

        #region 比较当前对象和参数对象的大小

        /// <summary>
        /// 比较当前对象和参数对象的大小
        /// </summary>
        /// <param name="other">要比较的对象</param>
        /// <returns></returns>
        public int CompareTo(DateTimeUtc other)
        {
            if (Timestamp > other.Timestamp)
                return 1;
            else if (Timestamp == other.Timestamp)
                return 0;
            else
                return -1;
        }

        #endregion

        #region 比较连个对象是否相等

        /// <summary>
        /// 比较连个对象是否相等
        /// </summary>
        /// <param name="other">要比较的对象</param>
        /// <returns></returns>
        public bool Equals(DateTimeUtc other)
        {
            if (Timestamp == other.Timestamp)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 比较两个UtcDate是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is DateTimeUtc utcDate))
                return false;

            return Equals(utcDate);
        }

        #endregion

        #region 获取对象的HashCode

        /// <summary>
        /// 获取对象的HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => Timestamp.GetHashCode();

        #endregion

        #region 转换为string

        /// <summary>
        /// 转换为string
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Timestamp.ToString();

        #endregion

        #region 重载== !=运算符

        /// <summary>
        /// 重载==运算符
        /// </summary>
        /// <param name="left">运算符左边对象</param>
        /// <param name="right">运算符右边对象</param>
        /// <returns>运算结果</returns>
        public static bool operator ==(DateTimeUtc left, DateTimeUtc right) => left.Equals(right);

        /// <summary>
        /// 重载!=运算符
        /// </summary>
        /// <param name="left">运算符左边对象</param>
        /// <param name="right">运算符右边对象</param>
        /// <returns>运算结果</returns>
        public static bool operator !=(DateTimeUtc left, DateTimeUtc right) => !(left == right);

        #endregion

        #region 重载> <运算符

        /// <summary>
        /// 重载>运算符
        /// </summary>
        /// <param name="left">运算符左边对象</param>
        /// <param name="right">运算符右边对象</param>
        /// <returns>运算结果</returns>
        public static bool operator >(DateTimeUtc left, DateTimeUtc right)
        {
            if (left.CompareTo(right) > 0)
                return true;
            else
                return false;

        }

        /// <summary>
        /// 重载运算符
        /// </summary>
        /// <param name="left">运算符左边对象</param>
        /// <param name="right">运算符右边对象</param>
        /// <returns>运算结果</returns>
        public static bool operator <(DateTimeUtc left, DateTimeUtc right) => !(left > right);

        #endregion

        #region 重载>= <=运算符

        /// <summary>
        /// 重载>=运算符
        /// </summary>
        /// <param name="left">运算符左边对象</param>
        /// <param name="right">运算符右边对象</param>
        /// <returns>运算结果</returns>
        public static bool operator >=(DateTimeUtc left, DateTimeUtc right)
        {
            if (left.CompareTo(right) >= 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 重载运算符
        /// </summary>
        /// <param name="left">运算符左边对象</param>
        /// <param name="right">运算符右边对象</param>
        /// <returns>运算结果</returns>
        public static bool operator <=(DateTimeUtc left, DateTimeUtc right) => !(left >= right);

        #endregion

        #region 整形隐式转换UtcDateTime

        /// <summary>
        /// long整形隐式转换UtcDateTime
        /// </summary>
        /// <param name="timeStamp">时间戳(秒)</param>
        public static implicit operator DateTimeUtc(long timeStamp)
        {
            if (timeStamp <= 9999999999)
                return new DateTimeUtc(DateTimeOffset.FromUnixTimeSeconds(timeStamp));
            else
                return new DateTimeUtc(DateTimeOffset.FromUnixTimeMilliseconds(timeStamp));
        }

        #endregion

        #region DateTimeOffset整形隐式转换UtcDateTime

        /// <summary>
        /// DateTimeOffset隐式转换UtcDateTime
        /// </summary>
        /// <param name="dateTime"></param>
        public static implicit operator DateTimeUtc(DateTimeOffset dateTime)
            => new DateTimeUtc(dateTime);

        #endregion

        #region UtcDateTime对象隐式转换为long

        /// <summary>
        /// UtcDateTime对象隐式转换为long
        /// </summary>
        /// <param name="utcDate"></param>
        public static implicit operator long(DateTimeUtc utcDate)
            => utcDate.Timestamp;

        #endregion

        #region UtcDate对象显式转换为DateTimeOffset

        /// <summary>
        /// UtcDateTime对象显式转换为DateTimeOffset
        /// </summary>
        /// <param name="utcDate"></param>
        public static explicit operator DateTimeOffset(DateTimeUtc utcDate)
            => utcDate.GetDateTime();

        #endregion
    }

    #endregion
}

