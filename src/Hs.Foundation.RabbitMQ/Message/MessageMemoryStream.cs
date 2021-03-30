using System;
using System.Buffers;
using System.IO;

namespace Hs.Foundation.RabbitMQ.Message
{
    #region 消息内存流

    /// <summary>
    /// 消息内存流
    /// </summary>
    public class MessageMemoryStream : Stream
    {
        #region 私有变量

        /// <summary>
        /// 数组池
        /// </summary>
        private ArrayPool<byte> arrayPool;

        /// <summary>
        /// 当前的字节数组
        /// </summary>
        private byte[] currentbuffer;

        /// <summary>
        /// 当前流是否可写
        /// </summary>
        private readonly bool canWrite;

        /// <summary>
        /// 流数据的长度
        /// </summary>
        private long length;

        /// <summary>
        /// 当前流操作的位置
        /// </summary>
        private long position;

        #endregion 私有变量

        #region 属性

        /// <summary>
        /// 流是否能读
        /// </summary>
        public override bool CanRead => true;

        /// <summary>
        /// </summary>
        public override bool CanSeek => true;

        /// <summary>
        /// 流是否能写
        /// </summary>
        public override bool CanWrite => canWrite;

        /// <summary>
        /// 流的长度
        /// </summary>
        public override long Length => length;

        /// <summary>
        /// 当前的位置
        /// </summary>
        public override long Position { get => position; set => position = value; }

        #endregion 属性

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        public MessageMemoryStream() : this(ArrayPool<byte>.Shared) { }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="pool"> 数组池 </param>
        public MessageMemoryStream(ArrayPool<byte> pool) : this(pool, 40960) { }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="pool"> 数组池 </param>
        /// <param name="size"> 池大小 </param>
        public MessageMemoryStream(ArrayPool<byte> pool, int size)
        {
            arrayPool = pool;
            currentbuffer = arrayPool.Rent(size);
            length = 0;
            canWrite = true;
            position = 0;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"> 放入流中的字节数组 </param>
        public MessageMemoryStream(byte[] data)
        {
            arrayPool = null;
            currentbuffer = data;
            length = data.Length;
            canWrite = false;
        }

        #endregion 初始化

        #region 冲洗缓冲区

        /// <summary>
        /// 冲洗缓冲区
        /// </summary>
        public override void Flush() { }

        #endregion 冲洗缓冲区

        #region 读取流数据

        /// <summary>
        /// 读取流数据
        /// </summary>
        /// <param name="buffer"> 要存放数据的byte数组 </param>
        /// <param name="offset"> 读取的位置 </param>
        /// <param name="count">  读取的字节数 </param>
        /// <returns> </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int readLength = count > (int)(length - position) ? (int)(length - position) : count;
            if (readLength > 0)
            {
                Buffer.BlockCopy(currentbuffer, (int)position, buffer, offset, readLength);
                position += readLength;
                return readLength;
            }
            else
                return 0;
        }

        #endregion 读取流数据

        #region 设定流的操作位置

        /// <summary>
        /// 设定流的操作位置
        /// </summary>
        /// <param name="offset"> 操作位置 </param>
        /// <param name="origin"> 位置类型 </param>
        /// <returns> </returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            long oldValue = position;
            switch ((int)origin)
            {
                case (int)SeekOrigin.Begin:
                    position = offset;
                    break;

                case (int)SeekOrigin.End:
                    position = length - offset;
                    break;

                case (int)SeekOrigin.Current:
                    position += offset;
                    break;

                default:
                    throw new InvalidOperationException("unknown SeekOrigin");
            }
            if (position < 0 || position > length)
            {
                position = oldValue;
                throw new IndexOutOfRangeException();
            }
            return position;
        }

        #endregion 设定流的操作位置

        #region 设置流的长度

        /// <summary>
        /// 设置流的长度
        /// </summary>
        /// <param name="value"> 长度 </param>
        public override void SetLength(long value)
        {
            if (!canWrite)
                throw new NotSupportedException("stream is readonly");

            if (value > int.MaxValue)
                throw new IndexOutOfRangeException("overflow");

            if (value < 0)
                throw new IndexOutOfRangeException("underflow");

            length = value;
            if (currentbuffer.Length < length)
                ReallocateBuffer((int)length);
        }

        #endregion 设置流的长度

        #region 写入流

        /// <summary>
        /// 写入流
        /// </summary>
        /// <param name="buffer"> </param>
        /// <param name="offset"> </param>
        /// <param name="count">  </param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!canWrite)
                throw new InvalidOperationException("stream is readonly");

            long endOffset = position + count;
            if (endOffset > currentbuffer.Length)
                ReallocateBuffer((int)(endOffset) * 2);

            Buffer.BlockCopy(buffer, offset, currentbuffer, (int)position, count);

            if (endOffset > length)
                length = endOffset;

            position = endOffset;
        }

        #endregion 写入流

        #region 转为字节数组

        /// <summary>
        /// 转为字节数组
        /// </summary>
        /// <returns> </returns>
        public virtual byte[] ToArray()
        {
            var bytes = new byte[length];
            Buffer.BlockCopy(currentbuffer, 0, bytes, 0, (int)length);
            return bytes;
        }

        #endregion 转为字节数组

        #region 重新分配缓存区

        /// <summary>
        /// 重新分配缓存区
        /// </summary>
        /// <param name="minimumRequired"> </param>
        private void ReallocateBuffer(int minimumRequired)
        {
            var bytes = arrayPool.Rent(minimumRequired);
            Buffer.BlockCopy(currentbuffer, 0, bytes, 0, currentbuffer.Length);
            arrayPool.Return(currentbuffer);
            currentbuffer = bytes;
        }

        #endregion 重新分配缓存区
    }

    #endregion 消息内存流
}