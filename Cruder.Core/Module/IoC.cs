using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Cruder.Core.ExceptionHandling;
using System;

namespace Cruder.Core.Module
{
    public static class IoC
    {
        public static IWindsorContainer Container { get; private set; }
        public static AssemblyFilter AssemblyFilter { get; private set; }
        public static FromAssemblyDescriptor AssemblyDescriptor { get; private set; }

        public static void Bootstrap(string path, Predicate<System.Reflection.Assembly> filter = null)
        {
            Container = new WindsorContainer();

            AssemblyFilter = new AssemblyFilter(path);

            if (filter != null)
            {
                AssemblyFilter.FilterByAssembly(filter);
            }

            AssemblyDescriptor = Classes.FromAssemblyInDirectory(AssemblyFilter);
            
            Container.Install(FromAssembly.InDirectory(AssemblyFilter));
        }

        public static void Register(params IRegistration[] registrations)
        {
            Container.Register(registrations);
        }

        public static T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public static object Resolve(Type type)
        {
            object result;

            try
            {
                result = Container.Resolve(type);
            }
            catch (Exception e)
            {
                var exception = new IoCException("IoC.Resolve<>()", "An exception occurred while resolving a type.", e);
                exception.Data.Add("type", type);
                throw exception;
            }

            return result;
        }

        public static void Release(object instance)
        {
            try
            {
                Container.Release(instance);
            }
            catch (Exception e)
            {
                var exception = new IoCException("IoC.Release()", "An exception occurred while releasing an instance.", e);
                exception.Data.Add("instance", instance);
                throw exception;
            }
        }

    }
}
