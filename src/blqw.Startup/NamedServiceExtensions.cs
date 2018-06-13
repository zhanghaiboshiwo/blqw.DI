﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blqw
{
    /// <summary>
    /// 命名服务扩展方法
    /// </summary>
    public static class NamedServiceExtensions
    {
        #region Scoped
        public static IServiceCollection AddNamedScoped<TService>(this IServiceCollection services, string name)
          where TService : class
        {
            services.Add(new ServiceDescriptor(new NamedType(name, typeof(TService)), typeof(TService), ServiceLifetime.Scoped));
            return services;
        }
        public static IServiceCollection AddNamedScoped(this IServiceCollection services, string name, Func<IServiceProvider, object> implementationFactory)
        {
            services.Add(new ServiceDescriptor(new NamedType(name, null), implementationFactory, ServiceLifetime.Scoped));
            return services;
        }
        public static IServiceCollection AddNamedScoped(this IServiceCollection services, string name, Type serviceType)
        {
            services.Add(new ServiceDescriptor(new NamedType(name, serviceType), serviceType, ServiceLifetime.Scoped));
            return services;
        }
        #endregion

        #region Singleton
        public static IServiceCollection AddNamedSingleton<TService>(this IServiceCollection services, string name)
    where TService : class
        {
            services.Add(new ServiceDescriptor(new NamedType(name, typeof(TService)), typeof(TService), ServiceLifetime.Singleton));
            return services;
        }
        public static IServiceCollection AddNamedSingleton(this IServiceCollection services, string name, Func<IServiceProvider, object> implementationFactory)
        {
            services.Add(new ServiceDescriptor(new NamedType(name, null), implementationFactory, ServiceLifetime.Singleton));
            return services;
        }
        public static IServiceCollection AddNamedSingleton(this IServiceCollection services, string name, Type serviceType)
        {
            services.Add(new ServiceDescriptor(new NamedType(name, serviceType), serviceType, ServiceLifetime.Singleton));
            return services;
        }
        public static IServiceCollection AddNamedSingleton(this IServiceCollection services, string name, object service)
        {
            services.Add(new ServiceDescriptor(new NamedType(name, null), service));
            return services;
        }
        #endregion

        #region Transient
        public static IServiceCollection AddNamedTransient<TService>(this IServiceCollection services, string name)
            where TService : class
        {
            services.Add(new ServiceDescriptor(new NamedType(name, typeof(TService)), typeof(TService), ServiceLifetime.Transient));
            return services;
        }
        public static IServiceCollection AddNamedTransient(this IServiceCollection services, string name, Func<IServiceProvider, object> implementationFactory)
        {
            services.Add(new ServiceDescriptor(new NamedType(name, null), implementationFactory, ServiceLifetime.Transient));
            return services;
        }
        public static IServiceCollection AddNamedTransient(this IServiceCollection services, string name, Type serviceType)
        {
            services.Add(new ServiceDescriptor(new NamedType(name, serviceType), serviceType, ServiceLifetime.Transient));
            return services;
        }
        #endregion

        #region GetService
        public static object GetNamedService(this IServiceProvider provider, string name) =>
            provider.GetService(new NamedType(name));
        public static object GetRequiredNamedService(this IServiceProvider provider, string name) =>
            provider.GetRequiredService(new NamedType(name));
        public static object GetRequiredNamedService(this IServiceProvider provider, string name, Type serviceType) =>
            provider.GetServices(new NamedType(name)).LastOrDefault(serviceType.IsInstanceOfType);
        public static T GetRequiredNamedService<T>(this IServiceProvider provider, string name) =>
            provider.GetServices(new NamedType(name)).OfType<T>().Last<T>();
        public static T GetNamedService<T>(this IServiceProvider provider, string name) =>
            provider.GetServices(new NamedType(name)).OfType<T>().LastOrDefault<T>();
        public static IEnumerable<T> GetNamedServices<T>(this IServiceProvider provider, string name) =>
            provider.GetServices(new NamedType(name)).OfType<T>().ToArray();
        public static IEnumerable<object> GetNamedServices(this IServiceProvider provider, string name, Type serviceType) =>
            provider.GetServices(new NamedType(name)).Where(serviceType.IsInstanceOfType).ToArray();
        #endregion
    }
}