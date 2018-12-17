	using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace DependencyContainer
{
	public class DependencyProvider
	{
		private DependenciesConfiguration configuration;
		private Dictionary<int, Stack<Type>> recursionControl;

		public TDependency Resolve<TDependency>()
		{
			Type dependency = typeof(TDependency);
			if (dependency.IsGenericTypeDefinition)
			{
				throw new ArgumentException("Generic type definition resolving is not supproted");
			}
			if (recursionControl.TryGetValue(Thread.CurrentThread.ManagedThreadId, out Stack<Type> types))
			{
				types.Clear();
			}
			else
			{
				recursionControl[Thread.CurrentThread.ManagedThreadId] = new Stack<Type>();
			}

			return (TDependency)Resolve(dependency);
		}

		internal object Resolve(Type dependency)
		{
				if (dependency.IsGenericType || dependency.IsGenericTypeDefinition)
			{
				return ResolveGeneric(dependency);
			}
				else
			{
				return ResolveNonGeneric(dependency);
			}
		}

		private object ResolveGeneric(Type dependency)
		{
			List<object> result = new List<object>(); // Ошибка приведения типа
			IEnumerable<Implementation> implementations = configuration.GetImplementations(dependency)
				.Where((impl) => !recursionControl[Thread.CurrentThread.ManagedThreadId].Contains(impl.ImplementationType));

			object instance;
			foreach(Implementation implementation in implementations)
			{
				instance = Create(implementation.ImplementationType.GetGenericTypeDefinition()
					.MakeGenericType(dependency.GenericTypeArguments));

				if (instance != null)
				{
					result.Add(instance);
				}
			}

			if (result.Count == 1)
			{
				return result[0];
			}
			else
			{
				return result;
			}
			
		}

		private object ResolveNonGeneric(Type dependency)
		{
			IEnumerable<Implementation> implementations =
				configuration.GetImplementations(dependency)
				.Where((impl) => !recursionControl[Thread.CurrentThread.ManagedThreadId].Contains(impl.ImplementationType));

			List<object> result = new List<object>();
			object dependencyInstance = null;

			foreach (Implementation implementation in implementations)
			{
				if (implementation.Lifetime == Lifetime.singleton)
				{
					if (implementation.SingletonInstance == null)
					{
						lock(implementation)
						{
							if (implementation.SingletonInstance == null)
							{
								implementation.SingletonInstance = Create(implementation.ImplementationType);
							}
						}
					}
				} 
				else
				{
					dependencyInstance = Create(implementation.ImplementationType);
				}

				if (dependencyInstance != null)
				{
					result.Add(dependencyInstance);
				}
			}

			if (result.Count == 1)
			{
				return result[0];
			}
			else
			{
				return result;
			}
		}

		private object Create(Type type)
		{
			ConstructorInfo[] constructors = type.GetConstructors().
				OrderBy((constructor) => constructor.GetParameters().Length).ToArray();
			object instance = null;
			List<object> parameters = new List<object>();
			recursionControl[Thread.CurrentThread.ManagedThreadId].Push(type);

			for (int i = 0; (i < constructors.Length) && (instance == null); ++i)
			{
				try
				{
					foreach (ParameterInfo parameter in constructors[i].GetParameters())
					{
						parameters.Add(Resolve(parameter.ParameterType));
					}
					instance = constructors[i].Invoke(parameters.ToArray());
				} catch
				{
					parameters.Clear();
				}
			}

			recursionControl[Thread.CurrentThread.ManagedThreadId].Pop();
			return instance;
		}

		private void ValidateConfiguration()
		{
			foreach(Type dependency in configuration.Container.Keys)
			{
				List<Implementation> implementations;
				if (configuration.Container.TryGetValue(dependency, out implementations))
				{
					foreach (Implementation impl in implementations)
					{
						if (dependency.IsGenericTypeDefinition ^ impl.ImplementationType.IsGenericTypeDefinition)
						{
							ValidatePairOfTypes(dependency, impl.ImplementationType, impl.Lifetime);
						}
					}
				}
			}
		}

		private void ValidatePairOfTypes(Type dependency, Type implementation, Lifetime lifetime)
		{
			if (dependency.IsGenericTypeDefinition ^ implementation.IsGenericTypeDefinition)
			{
				throw new ArgumentException("Open generics register should be with both open generic types");
			}

			if (dependency.IsGenericTypeDefinition)
			{
				if (lifetime == Lifetime.singleton)
				{
					throw new ArgumentException("Open generic cannot be singleton");
				}
			}
			else
			{
				if (!dependency.IsClass && !dependency.IsAbstract && !dependency.IsInterface
					|| (!implementation.IsClass || implementation.IsAbstract))
				{
					throw new ArgumentException("Wrong types");
				}

				if (!dependency.IsAssignableFrom(implementation))
				{
					throw new ArgumentException("Dependency is not assignable from implementation");
				}
			}
		}

		public DependencyProvider(DependenciesConfiguration configuration)
		{
			this.configuration = configuration;
			ValidateConfiguration();
			recursionControl = new Dictionary<int, Stack<Type>>();
		}
	}
}
