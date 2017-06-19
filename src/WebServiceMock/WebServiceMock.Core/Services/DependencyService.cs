using Microsoft.Practices.Unity;
using System;
using System.Reflection;
using System.Web.Http.Dependencies;
using Unity.WebApi;

namespace WebServiceMock.Core.Services
{
    /// <summary>
    /// Resolves types at runtime for Dependency Injection.
    /// <para>No need to unit test this (no business logic), only integration testing.</para>
    /// </summary>
    public class DependencyService : IDisposable
    {
        public IDependencyResolver DependencyResolver { get { return _dependencyResolver; } }

        private static readonly IUnityContainer _container = null;
        private static readonly IDependencyResolver _dependencyResolver = null;

        private static object _lock = new object();
        private static bool _isDisposed = false;

        static DependencyService()
        {
            _container = new UnityContainer();

            // Finds the interface "IExample" and maps it to the class "Example".
            _container.RegisterTypes(
                AllClasses.FromAssemblies(Assembly.GetExecutingAssembly()),
                WithMappings.FromMatchingInterface,
                WithName.Default
            );

            // To manually map more types:  _container.RegisterType<ITestService, TestService>();

            _dependencyResolver = new UnityDependencyResolver(_container);
        }

        /// <summary>Gets an implementation for the type.</summary>
        /// <typeparam name="T">The type to resolve (generally an interface).</typeparam>
        /// <returns>The concrete implementation of the requested type.</returns>
        /// <example>IExample example = new DependencyService.Resolve<IExample>();</example>
        public T Resolve<T>() where T : class
        {
           return (T)_dependencyResolver.GetService(typeof(T));
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                lock (_lock)
                {
                    if (!_isDisposed)
                    {
                        _container.Dispose();
                        _dependencyResolver.Dispose();
                        _isDisposed = true;
                    }
                }
            }
        }
    }
}
