using Microsoft.Extensions.Configuration;

namespace Hs.Foundation.Utility
{
    #region 配置对象助手类

    /// <summary>
    /// 配置对象助手类
    /// </summary>
    public class ConfigurationHelper
    {
        #region 获取配置对象

        /// <summary>
        /// 获取配置对象
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IConfiguration GetConfiguration(IConfiguration configuration, string key)
        {
            var cfg = configuration.GetSection(key);
            if (cfg.Exists())
                return cfg;
            else
                return configuration;
        }

        #endregion
    }

    #endregion
}
