using CommonServiceLocator;
using FrameworkLibrary.Interfaces;
using FrameworkLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests.FrameworkLibraryTests.Utils
{
    public class MockUtils<T> where T : class
    {
        public MockUtils(List<Type>? singletonTypes = null, List<Type>? transientTypes = null, List<object>? singletonObjects = null)
        {
            SingletonTypes = singletonTypes ?? new List<Type>();
            TransientTypes = transientTypes ?? new List<Type>();
            SingletonObjects = singletonObjects?.ToDictionary(x => x.GetType(), y => y) ?? new Dictionary<Type, object>();
        }

        public List<Type> SingletonTypes { get; }

        public List<Type> TransientTypes { get; }

        public Dictionary<Type, object> SingletonObjects { get; }

        private IServiceProvider? ServiceProvider { get; set; }

        private IServiceLocator? ServiceLocator { get; set; }

        /// <summary>
        /// Add a new type as a transient type (typically the class and not the interface)
        /// </summary>
        /// <typeparam name="T1">Type</typeparam>
        /// <returns></returns>
        public MockUtils<T> AddTransient<T1>() where T1 : class
        {
            var t = typeof(T1);

            ValidateNotBuilt();

            ValidateUniqueType(t);

            if (!TransientTypes.Contains(t))
            {
                TransientTypes.Add(t);
            }

            return this;
        }

        /// <summary>
        /// Add a new type as a singleton
        /// </summary>
        /// <typeparam name="T1">Type</typeparam>
        /// <returns></returns>
        public MockUtils<T> AddSingleton<T1>() where T1 : class
        {
            var t = typeof(T1);

            ValidateNotBuilt();

            ValidateUniqueType(t);

            if (!SingletonTypes.Contains(t))
            {
                SingletonTypes.Add(t);
            }

            return this;
        }

        /// <summary>
        /// Add an implementation of the type as a singleton
        /// </summary>
        /// <typeparam name="T1">Type</typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public MockUtils<T> AddSingleton<T1>(T1 item) where T1 : class
        {
            var t = typeof(T1);

            ValidateNotBuilt();

            ValidateUniqueType(t);

            if (!SingletonObjects.ContainsKey(t))
            {
                SingletonObjects.Add(t, item);
            }

            return this;
        }

        private TDesiredService InternalGetService<TDesiredService>() where TDesiredService : class
        {
            var desiredService = ServiceProvider?.GetService<TDesiredService>();

            if (desiredService is IServiceLocatorClient serviceLocatorClient)
            {
                serviceLocatorClient.ServiceLocator = EnsureServiceLocator();
            }

            return desiredService
                ?? throw new InvalidOperationException("Could not create instance of service.");
        }

        /// <summary>
        /// Ensures the service provider is built and then returns a service from the service provider.
        /// </summary>
        /// <typeparam name="TDesiredService">Desired service</typeparam>
        /// <returns></returns>
        public TDesiredService GetService<TDesiredService>() where TDesiredService : class
        {
            EnsureServiceProvider();

            return InternalGetService<TDesiredService>();
        }

        /// <summary>
        /// Returns a mock from the service provider if it has already been built,
        /// otherwise add the new type internally as a singleton and return a new mock
        /// for the type.
        /// </summary>
        /// <typeparam name="TDesiredMockType">Desired mock type</typeparam>
        /// <returns></returns>
        public TDesiredMockType GetMock<TDesiredMockType>() where TDesiredMockType : class
        {
            if (ServiceProvider != null)
            {
                return InternalGetService<TDesiredMockType>();
            }

            TDesiredMockType? item;

            var mockType = typeof(TDesiredMockType) 
                ?? throw new InvalidOperationException("Unexpected exception retrieving type info.");

            if (SingletonObjects.ContainsKey(mockType))
            {
                item = (TDesiredMockType)SingletonObjects[mockType];
            }
            else
            {
                SingletonTypes.Remove(mockType);

                item = mockType.IsInterface 
                    ? NSubstitute.Substitute.For<TDesiredMockType>()
                    : (TDesiredMockType)(Activator.CreateInstance(mockType) ?? throw new InvalidOperationException("Could not create instance of mocking type."));

                if (item == null)
                {
                    throw new InvalidOperationException("Could not create desired mock.");
                }

                AddSingleton(item);
            }

            return item;
        }

        /// <summary>
        /// Ensures the service provider has been built and returns the main instance of the
        /// generic type that should be used in the test.
        /// </summary>
        /// <returns></returns>
        public T GetMainInstance()
        {
            EnsureServiceProvider();

            return InternalGetService<T>();
        }

        /// <summary>
        /// Build MockUtils helper
        /// </summary>
        public void Build()
        {
            AddKnownTypes();
            ServiceProvider = GetServiceCollection().BuildServiceProvider();
        }

        private bool IsRegistered(Type t) => SingletonTypes.Contains(t) || TransientTypes.Contains(t) || SingletonObjects.ContainsKey(t);

        /// <summary>
        /// Using the types in this instance of MockUtils, create a service collection
        /// </summary>
        /// <returns></returns>
        private ServiceCollection GetServiceCollection()
        {
            var services = new ServiceCollection();

            services.InitSingletonMocks(SingletonTypes);
            services.InitTransientMocks(TransientTypes);
            services.InitConcreteSingletons(SingletonObjects);

            return services;
        }

        /// <summary>
        /// Add any type dependencies for all types which were added explicitly
        /// </summary>
        private void AddKnownTypes()
        {
            foreach (var type in SingletonTypes.ToList())
            {
                FillTypesList(type);
            }

            foreach (var type in TransientTypes.ToList())
            {
                FillTypesList(type);
            }

            FillTypesList(typeof(T));
        }

        /// <summary>
        /// ****************** RECURSIVE *********************
        /// For the given type, ensure it is added to our internal transient/singleton type list and also check all constructors
        /// for the given type and ensure that any types for the constructor parameters are also added to our internal transient/singleton
        /// type list
        /// </summary>
        /// <param name="type"></param>
        private void FillTypesList(Type type)
        {
            if (!IsRegistered(type))
            {
                var isInterface = type.IsInterface;

                if (isInterface)
                {
                    SingletonTypes.Add(type);
                }
                else
                {
                    TransientTypes.Add(type);
                }
            }

            if (SingletonObjects.ContainsKey(type))
            {
                return;
            }

            if (!type.IsClass)
            {
                return;
            }

            var ctors = type.GetConstructors();

            if (ctors.Length == 0)
            {
                return;
            }

            foreach (var ctor in ctors)
            {
                foreach (var param in ctor.GetParameters())
                {
                    if (param.ParameterType.Namespace == "System")
                    {
                        continue;
                    }

                    FillTypesList(param.ParameterType);
                }
            }
        }

        private void ValidateNotBuilt()
        {
            if (ServiceProvider != null)
            {
                throw new Exception("Invalid operation.  MockUtils internal ServiceProvider has already been built.");
            }
        }

        private void ValidateUniqueType(Type t)
        {
            if (IsRegistered(t))
            {
                throw new InvalidOperationException("Type may only be registered one time.");
            }
        }

        private IServiceProvider EnsureServiceProvider()
        {
            if (ServiceProvider == null)
            {
                Build();
            }

            return ServiceProvider ??
                throw new InvalidOperationException("ServiceProvider cannot be null.");
        }        

        private IServiceLocator EnsureServiceLocator()
        {
            if (ServiceLocator == null)
            {
                ServiceLocator = new ServiceLocatorService(EnsureServiceProvider());
            }

            return ServiceLocator
                ?? throw new InvalidOperationException("ServiceLocator cannot be null.");
        }
    }
}
