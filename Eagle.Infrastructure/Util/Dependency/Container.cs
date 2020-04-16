using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Util.Helpers;

namespace Util.Dependency {
    /// <summary>
    /// Autofac对象容器
    /// </summary>
    internal class Container : IContainer {
        /// <summary>
        /// 容器
        /// </summary>
        private Autofac.IContainer _container;

        /// <summary>
        /// 设置被代理的容器
        /// </summary>
        /// <param name="container"></param>
        public void SetProxy(Autofac.IContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            _container = container as Autofac.IContainer;
        }

        /// <summary>
        /// 创建集合
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">服务名称</param>
        public List<T> CreateList<T>( string name = null ) {
            var result = CreateList( typeof( T ), name );
            if( result == null )
                return new List<T>();
            return ( (IEnumerable<T>)result ).ToList();
        }

        /// <summary>
        /// 创建集合
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="name">服务名称</param>
        public object CreateList( Type type, string name = null ) {
            Type serviceType = typeof( IEnumerable<> ).MakeGenericType( type );
            return Create( serviceType, name );
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">服务名称</param>
        public T Create<T>( string name = null ) {
            return (T)Create( typeof( T ), name );
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="name">服务名称</param>
        public object Create( Type type, string name = null ) {
            return Web.HttpContext?.RequestServices != null ? GetServiceFromHttpContext( type, name ) : GetService( type, name );
        }


        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="obj">获取的对象</param>
        /// <param name="name">服务名称</param>
        public bool TryCreate(Type type, out object obj, string name = null)
        {
            return Web.HttpContext?.RequestServices != null ? TryGetServiceFromHttpContext(type, name, out obj) : TryGetService(type, name, out obj);
        }

        /// <summary>
        /// 从HttpContext获取服务
        /// </summary>
        private object GetServiceFromHttpContext( Type type, string name ) {
            var serviceProvider = Web.HttpContext.RequestServices;
            if( name == null )
                return serviceProvider.GetService( type );
            var context = serviceProvider.GetService<IComponentContext>();
            return context.ResolveNamed( name, type );
        }

        /// <summary>
        /// Tries the get service from HTTP context.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="obj">The object.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool TryGetServiceFromHttpContext(Type type, string name, out object obj)
        {
            var serviceProvider = Web.HttpContext.RequestServices;
            if (name == null)
            {
                try
                {
                    obj = serviceProvider.GetService(type);
                }
                catch
                {
                    obj = null;
                    return false;
                }
            }
            var context = serviceProvider.GetService<IComponentContext>();
            return context.TryResolveNamed(name, type, out obj);
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        private object GetService( Type type, string name ) {
            if( name == null )
                return _container.Resolve( type );
            return _container.ResolveNamed( name, type ); 
        }

        /// <summary>
        /// Tries the get service.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="obj">The object.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool TryGetService(Type type, string name, out object obj)
        {
            if (name == null)
                return _container.TryResolve(type, out obj);
            return _container.TryResolveNamed(name, type, out obj);
        }

        /// <summary>
        /// 作用域开始
        /// </summary>
        public IScope BeginScope() {
            return new Scope( _container.BeginLifetimeScope() );
        }

        /// <summary>
        /// 注册依赖
        /// </summary>
        /// <param name="configs">依赖配置</param>
        public void Register( params IConfig[] configs ) {
            Register( null, null, configs );
        }

        /// <summary>
        /// 注册依赖
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configs">依赖配置</param>
        public IServiceProvider Register( IServiceCollection services, params IConfig[] configs ) {
            return Register( services, null, configs );
        }

        /// <summary>
        /// 注册依赖
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="actionBefore">注册前操作</param>
        /// <param name="configs">依赖配置</param>
        public IServiceProvider Register( IServiceCollection services, Action<ContainerBuilder> actionBefore, params IConfig[] configs ) {
            var builder = CreateBuilder( services, actionBefore, configs );
            _container = builder.Build();
            return new AutofacServiceProvider( _container );
        }

        /// <summary>
        /// 创建容器生成器
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="actionBefore">注册前执行的操作</param>
        /// <param name="configs">依赖配置</param>
        public ContainerBuilder CreateBuilder( IServiceCollection services, Action<ContainerBuilder> actionBefore, params IConfig[] configs ) {
            var builder = new ContainerBuilder();
            actionBefore?.Invoke( builder );
            if ( configs != null ) {
                foreach ( var config in configs )
                    builder.RegisterModule( config );
            }
            if( services != null )
                builder.Populate( services );
            return builder;
        }

        /// <summary>
        /// 释放容器
        /// </summary>
        public void Dispose() {
            _container.Dispose();
        }
    }
}