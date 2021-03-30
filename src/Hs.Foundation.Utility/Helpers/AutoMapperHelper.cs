using AutoMapper;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hs.Foundation.Utility
{
    #region AutoMapper助手类

    /// <summary>
    /// AutoMapper助手类
    /// </summary>
    public class AutoMapperHelper
    {
        #region 私有字段

        /// <summary>
        /// AutoMapper缓存
        /// </summary>
        private static readonly Dictionary<KeyValuePair<Type, Type>, IMapper> mapperCache = new Dictionary<KeyValuePair<Type, Type>, IMapper>();

        #endregion

        #region 获取Mapper对象

        /// <summary>
        /// 获取Mapper对象
        /// </summary>
        /// <typeparam name="TSource">映射源类型</typeparam>
        /// <typeparam name="TDestination">映射的目标类型</typeparam>
        /// <param name="configure">配置委托</param>
        /// <param name="serviceCtor">转换委托</param>
        /// <param name="refreshCache">是否刷新缓存</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IMapper GetMapper<TSource, TDestination>(Action<IMapperConfigurationExpression> configure, Func<Type, object> serviceCtor, bool refreshCache = false)
        {
            var keyType = new KeyValuePair<Type, Type>(typeof(TSource), typeof(TDestination));
            if (refreshCache)
                mapperCache.Remove(keyType);

            if (!mapperCache.ContainsKey(keyType))
            {
                IConfigurationProvider mapperConfig;
                if (configure == null)
                    mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<TSource, TDestination>());
                else
                {
                    mapperConfig = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<TSource, TDestination>();
                        configure.Invoke(cfg);
                    });
                }
                if (serviceCtor is null)
                    mapperCache.Add(keyType, mapperConfig.CreateMapper());
                else
                    mapperCache.Add(keyType, mapperConfig.CreateMapper(serviceCtor));
            }

            return mapperCache[keyType];
        }

        /// <summary>
        /// 获取Mapper对象
        /// </summary>
        /// <typeparam name="TSource">映射源类型</typeparam>
        /// <typeparam name="TDestination">映射的目标类型</typeparam>
        /// <param name="configure">配置委托</param>
        /// <param name="serviceCtor">转换委托</param>
        /// <param name="refreshCache"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IMapper GetMapper<TSource, TDestination>(Action<IMapperConfigurationExpression, IMappingExpression<TSource, TDestination>> configure, Func<Type, object> serviceCtor, bool refreshCache = false)
        {
            var keyType = new KeyValuePair<Type, Type>(typeof(TSource), typeof(TDestination));
            if (refreshCache)
                mapperCache.Remove(keyType);

            if (!mapperCache.ContainsKey(keyType))
            {
                IConfigurationProvider mapperConfig;
                if (configure == null)
                    mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<TSource, TDestination>());
                else
                    mapperConfig = new MapperConfiguration(cfg => configure.Invoke(cfg, cfg.CreateMap<TSource, TDestination>()));

                if (serviceCtor is null)
                    mapperCache.Add(keyType, mapperConfig.CreateMapper());
                else
                    mapperCache.Add(keyType, mapperConfig.CreateMapper(serviceCtor));
            }

            return mapperCache[keyType];
        }

        /// <summary>
        /// 获取Mapper对象
        /// </summary>
        /// <typeparam name="TSource">映射源类型</typeparam>
        /// <typeparam name="TDestination">映射的目标类型</typeparam>
        /// <param name="serviceCtor">转换委托</param>
        /// <param name="refreshCache">是否刷新缓存</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IMapper GetMapper<TSource, TDestination>(Func<Type, object> serviceCtor, bool refreshCache = false)
            => GetMapper<TSource, TDestination>(e => { }, serviceCtor, refreshCache);

        /// <summary>
        /// 获取Mapper对象
        /// </summary>
        /// <typeparam name="TSource">映射源类型</typeparam>
        /// <typeparam name="TDestination">映射的目标类型</typeparam>
        /// <param name="configure">配置委托</param>
        /// <param name="refreshCache">是否刷新缓存</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IMapper GetMapper<TSource, TDestination>(Action<IMapperConfigurationExpression> configure, bool refreshCache = false)
            => GetMapper<TSource, TDestination>(configure, null, refreshCache);

        /// <summary>
        /// 获取Mapper对象
        /// </summary>
        /// <typeparam name="TSource">映射源类型</typeparam>
        /// <typeparam name="TDestination">映射的目标类型</typeparam>
        /// <param name="configure">配置委托</param>
        /// <param name="refreshCache">是否刷新缓存</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IMapper GetMapper<TSource, TDestination>(Action<IMapperConfigurationExpression, IMappingExpression<TSource, TDestination>> configure, bool refreshCache = false)
            => GetMapper(configure, null, refreshCache);

        /// <summary>
        /// 获取Mapper对象
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="refreshCache">是否刷新缓存</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IMapper GetMapper<TSource, TDestination>(bool refreshCache = false)
            => GetMapper<TSource, TDestination>(e => { }, null, refreshCache);

        #endregion

        #region 映射对象

        /// <summary>
        /// 映射对象
        /// </summary>
        /// <typeparam name="TSource">映射源类型</typeparam>
        /// <typeparam name="TDestination">映射的目标类型</typeparam>
        /// <param name="source">源类型对象</param>
        /// <param name="refreshCache">是否刷新缓存</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TDestination Mapper<TSource, TDestination>(TSource source, bool refreshCache = false)
            => GetMapper<TSource, TDestination>(refreshCache).Map<TDestination>(source);

        /// <summary>
        /// 映射对象
        /// </summary>
        /// <typeparam name="TSource">映射源类型</typeparam>
        /// <typeparam name="TDestination">映射的目标类型</typeparam>
        /// <param name="source">源类型对象</param>
        /// <param name="opts">自定义映射配置</param>
        /// <param name="refreshCache">是否刷新缓存</param>
        /// <returns></returns>
        public static TDestination Mapper<TSource, TDestination>(object source, Action<IMappingOperationOptions<object, TDestination>> opts, bool refreshCache = false)
            => GetMapper<TSource, TDestination>(refreshCache).Map(source, opts);

        /// <summary>
        /// 映射对象
        /// </summary>
        /// <typeparam name="TSource">映射源类型</typeparam>
        /// <typeparam name="TDestination">映射的目标类型</typeparam>
        /// <param name="source">源类型对象</param>
        /// <param name="opts">自定义映射配置</param>
        /// <param name="refreshCache">是否刷新缓存</param>
        /// <returns></returns>
        public static TDestination Mapper<TSource, TDestination>(TSource source, Action<IMappingOperationOptions<TSource, TDestination>> opts, bool refreshCache = false)
           => GetMapper<TSource, TDestination>(refreshCache).Map(source, opts);

        /// <summary>
        /// 映射对象
        /// </summary>
        /// <typeparam name="TSource">映射源类型</typeparam>
        /// <typeparam name="TDestination">映射的目标类型</typeparam>
        /// <param name="source">源类型对象</param>
        /// <param name="destination">目标对象</param>
        /// <param name="opts">自定义映射配置</param>
        /// <param name="refreshCache">是否刷新缓存</param>
        /// <returns></returns>
        public static TDestination Mapper<TSource, TDestination>(TSource source, TDestination destination, Action<IMappingOperationOptions<TSource, TDestination>> opts, bool refreshCache = false)
           => GetMapper<TSource, TDestination>(refreshCache).Map(source, opts);

        #endregion
    }

    #endregion
}
