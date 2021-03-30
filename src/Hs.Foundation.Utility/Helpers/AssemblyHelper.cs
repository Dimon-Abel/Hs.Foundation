using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Microsoft.Extensions.DependencyModel;

namespace Hs.Foundation.Utility
{
    #region 程序集相关助手类

    /// <summary>
    /// 程序集相关助手类
    /// </summary>
    public class AssemblyHelper
    {
        #region 获取全部程序集

        /// <summary>
        /// 获取全部程序集
        /// </summary>
        /// <returns></returns>
        public static IList<Assembly> GetAssemblies()
        {
            var libs = DependencyContext.Default.CompileLibraries.Where(lib => !lib.Serviceable);
            return libs.Select(lib => AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name)))
                .Where(assembly => assembly != default).ToList();
        }

        #endregion
    }

    #endregion
}
