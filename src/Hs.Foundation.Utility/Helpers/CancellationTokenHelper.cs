using System.Threading;

namespace Hs.Foundation.Utility
{
    #region CancellationToken助手类

    /// <summary>
    /// CancellationToken助手类
    /// </summary>
    public class CancellationTokenHelper
    {
        #region 创建CancellationToken

        /// <summary>
        /// 创建CancellationToken
        /// </summary>
        /// <param name="millisecondsDelay"></param>
        /// <returns></returns>
        public static CancellationToken CreateCancellationToken(int millisecondsDelay)
            => new CancellationTokenSource().Token;

        #endregion
    }

    #endregion
}
