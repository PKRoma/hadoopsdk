// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License.  You may obtain a copy
// of the License at http://www.apache.org/licenses/LICENSE-2.0
// 
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED
// WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT.
// 
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

namespace Microsoft.WindowsAzure.Management.Framework.InversionOfControl
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    ///     The main service locator for any application.
    /// </summary>
    public class IocServiceLocator : IIocServiceLocator
    {
        private static readonly IocServiceLocator SingletonInstance = new IocServiceLocator();
        private readonly Dictionary<Type, object> runtimeServices = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> testRunServices = new Dictionary<Type, object>();
        private Dictionary<Type, object> individualTestServices = new Dictionary<Type, object>();
        private IocTestMockingLevel mockingLevel = IocTestMockingLevel.ApplyFullMocking;

        /// <summary>
        ///     Gets the singleton instance of the IocServiceLocator.
        /// </summary>
        public static IIocServiceLocator Instance
        {
            get { return SingletonInstance; }
        }

        /// <summary>
        ///     Prevents a default instance of the IocServiceLocator class from being created.
        /// </summary>
        private IocServiceLocator()
        {
            this.runtimeServices.Add(typeof(IIocServiceLocationRuntimeManager),
                                     new IocServiceLocationRuntimeManager(this));
            this.runtimeServices.Add(typeof(IIocServiceLocationTestRunManager),
                                     new IocServiceLocationTestRunManager(this));
            this.runtimeServices.Add(typeof(IIocServiceLocationIndividualTestManager),
                                     new IocServiceLocationIndividualTestManager(this));
            this.runtimeServices.Add(typeof(IIocServiceLocator),
                                     this);
        }

        internal static void ThrowIfNullInstance([ValidatedNotNull] Type type, [ValidatedNotNull] object implementation)
        {
            if (ReferenceEquals(type,
                                null))
            {
                string msg = string.Format(CultureInfo.InvariantCulture,
                                           "An attempt was made to register a null type as service");
                throw new InvalidOperationException(msg);
            }
            if (ReferenceEquals(implementation,
                                null))
            {
                string msg = string.Format(
                    CultureInfo.InvariantCulture,
                    "An attempt was made to register or override a service with a null implementation '{0}'",
                    type.FullName);
                throw new InvalidOperationException(msg);
            }
        }

        internal static void ThrowIfInvalidRegistration(Type type, Type implementation)
        {
            ThrowIfNullInstance(type,
                                implementation);
            if (type == typeof(IIocServiceLocationRuntimeManager) || type == typeof(IIocServiceLocationTestRunManager) ||
                type == typeof(IIocServiceLocationIndividualTestManager) || type == typeof(IIocServiceLocator))
            {
                string msg = string.Format(
                    CultureInfo.InvariantCulture,
                    "An attempt was made to register or override the restrictive service '{0}'",
                    type.FullName);
                throw new InvalidOperationException(msg);
            }
            if (!type.IsInterface)
            {
                string msg = string.Format(
                    CultureInfo.InvariantCulture,
                    "An attempt was made to register or override a service for the type '{0}' that was not an interface",
                    type.FullName);
                throw new InvalidOperationException(msg);
            }
            if (!type.IsAssignableFrom(implementation))
            {
                string msg = string.Format(
                    CultureInfo.InvariantCulture,
                    "An attempt was made to register or override the service '{0}' for the type '{1}' which was not derived from the service",
                    implementation.FullName,
                    type.FullName);
                throw new InvalidOperationException(msg);
            }
        }

        private abstract class IocServiceLocationManager : IIocServiceLocationManager
        {
            /// <inheritdoc />
            public void RegisterInstance<TService>(TService instance)
            {
                this.RegisterInstance(typeof(TService),
                                      instance);
            }

            /// <inheritdoc />
            public abstract void RegisterInstance(Type type, object instance);

            /// <inheritdoc />
            public void RegisterType<T>(Type registrationValue)
            {
                this.RegisterType(typeof(T),
                                  registrationValue);
            }

            /// <inheritdoc />
            public void RegisterType<TInterface, TConcreate>() where TConcreate : class, TInterface
            {
                this.RegisterType(typeof(TInterface), typeof(TConcreate));
            }

            /// <inheritdoc />
            public void RegisterType(Type type, Type registrationValue)
            {
                // TODO: see if an injection is necessary here (for Service Locator)
                ThrowIfInvalidRegistration(type, registrationValue);
                object obj = Activator.CreateInstance(registrationValue);
                this.RegisterInstance(type,
                                      obj);
            }
        }

        private class InternalRuntimeRegistrationManager : IocServiceLocationManager, IIocServiceLocationRuntimeManager
        {
            private readonly Dictionary<Type, object> discovered = new Dictionary<Type, object>();

            public override void RegisterInstance(Type serviceType, object instance)
            {
                var registering = instance as IRegisteringService;
                object discoveredInstance;
                if (this.discovered.TryGetValue(serviceType,
                                                out discoveredInstance))
                {
                    if (!ReferenceEquals(discoveredInstance,
                                         instance) && !ReferenceEquals(registering,
                                                                       null))
                    {
                        this.discovered[serviceType] = instance;
                        registering.Register(this);
                    }
                }
                else
                {
                    this.discovered[serviceType] = instance;
                    if (!ReferenceEquals(registering,
                                         null))
                    {
                        registering.Register(this);
                    }
                }
            }

            public IEnumerable<KeyValuePair<Type, object>> GetDiscovered()
            {
                return this.discovered;
            }
        }

        private class IocServiceLocationRuntimeManager : IocServiceLocationManager, IIocServiceLocationRuntimeManager
        {
            private readonly IocServiceLocator locator;

            public IocServiceLocationRuntimeManager(IocServiceLocator locator)
            {
                this.locator = locator;
            }

            /// <inheritdoc />
            public override void RegisterInstance(Type type, object instance)
            {
                ThrowIfNullInstance(type,
                                    instance);
                ThrowIfInvalidRegistration(type,
                                           instance.GetType());
                var internalManager = new InternalRuntimeRegistrationManager();
                internalManager.RegisterInstance(type,
                                                 instance);
                foreach (KeyValuePair<Type, object> keyValuePair in internalManager.GetDiscovered())
                {
                    this.locator.runtimeServices[keyValuePair.Key] = keyValuePair.Value;
                }
            }
        }

        private class IocServiceLocationTestRunManager : IocServiceLocationManager, IIocServiceLocationTestRunManager
        {
            private readonly IocServiceLocator locator;

            public IocServiceLocationTestRunManager(IocServiceLocator locator)
            {
                this.locator = locator;
            }

            /// <inheritdoc />
            public override void RegisterInstance(Type type, object instance)
            {
                ThrowIfNullInstance(type,
                                    instance);
                ThrowIfInvalidRegistration(type,
                                           instance.GetType());
                this.locator.testRunServices[type] = instance;
            }

            public IocTestMockingLevel MockingLevel
            {
                get { return this.locator.mockingLevel; }
                set { this.locator.mockingLevel = value; }
            }
        }

        private class IocServiceLocationIndividualTestManager : IIocServiceLocationIndividualTestManager
        {
            private readonly IocServiceLocator locator;

            public IocServiceLocationIndividualTestManager(IocServiceLocator locator)
            {
                this.locator = locator;
            }

            public void Override<T>(T overrideValue)
            {
                this.Override(typeof(T),
                              overrideValue);
            }

            public void Override(Type type, object overrideValue)
            {
                ThrowIfNullInstance(type,
                                    overrideValue);
                ThrowIfInvalidRegistration(type,
                                           overrideValue.GetType());
                this.locator.individualTestServices.Add(type,
                                           overrideValue);
            }

            public void Reset()
            {
                this.locator.individualTestServices = new Dictionary<Type, object>();
            }
        }

        /// <inheritdoc />
        public T Locate<T>()
        {
            return (T)this.Locate(typeof(T));
        }

        /// <inheritdoc />
        public object Locate(Type type)
        {
            if (ReferenceEquals(type,
                                null))
            {
                throw new ArgumentNullException("type");
            }
            object runtimeVersion = null;
            object fakeVersion = null;
            object overrideVersion = null;

                // First try to get a IndiviudalTest Mock
            if (!((this.mockingLevel == IocTestMockingLevel.ApplyFullMocking ||
                   this.mockingLevel == IocTestMockingLevel.ApplyIndividualTestMockingOnly) && 
                   this.individualTestServices.TryGetValue(type, out overrideVersion)) && 
                // Next try to get a TestRun Mock
                !((this.mockingLevel == IocTestMockingLevel.ApplyFullMocking ||
                   this.mockingLevel == IocTestMockingLevel.ApplyTestRunMockingOnly) && 
                   this.testRunServices.TryGetValue(type, out fakeVersion)) &&
                // Finally try to get the actual service
                !this.runtimeServices.TryGetValue(type, out runtimeVersion))
            {
                string message = string.Format(CultureInfo.InvariantCulture,
                                               "Attempt to locate a service '{0}' that has not been registered",
                                               type.FullName);
                throw new InvalidOperationException(message);
            }
            object retval = overrideVersion;
            if (retval.IsNull())
            {
                retval = fakeVersion;
                if (retval.IsNull())
                {
                    retval = runtimeVersion;
                }
            }
            return retval;
        }
    }
}