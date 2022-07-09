using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.FrameworkLibraryTests.Utils
{
    public static class Extensions
    {
        public static void InitConcreteSingletons(this ServiceCollection serviceCollection, IDictionary<Type, object> singletonDictionary)
        {
            if (serviceCollection == null || singletonDictionary == null)
            {
                return;
            }

            foreach (var keyValuePair in singletonDictionary)
            {
                serviceCollection.AddSingleton(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public static void InitSingletonMocks(this ServiceCollection serviceCollection, IEnumerable<Type> singletonTypes)
        {
            if (serviceCollection == null || singletonTypes == null)
            {
                return;
            }

            foreach (var singletonType in singletonTypes)
            {
                if (singletonType.IsInterface)
                {
                    serviceCollection.AddSingleton(singletonType, NSubstitute.Substitute.For(new[] { singletonType }, new object[] { }));
                }
                else
                {
                    serviceCollection.AddSingleton(singletonType);
                }
            }
        }

        public static void InitTransientMocks(this ServiceCollection serviceCollection, IEnumerable<Type> transientTypes)
        {
            if (serviceCollection == null || transientTypes == null)
            {
                return;
            }

            foreach (var transientType in transientTypes)
            {
                if (transientType.IsInterface)
                {
                    serviceCollection.AddTransient(transientType, s => NSubstitute.Substitute.For(new[] { transientType }, new object[] { }));
                }
                else
                {
                    serviceCollection.AddTransient(transientType);
                }
            }
        }
    }
}
