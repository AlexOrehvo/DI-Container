using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyContainer
{
	public class DependenciesConfiguration
	{
		internal Dictionary<Type, List<Implementation>> Container { get; set; }

		public void Register<TDependency, TImplementation>(Lifetime lifetime = Lifetime.instPerDependency)
		{
			Register(typeof(TDependency), typeof(TImplementation), lifetime);
		}

		public void Register(Type tDependency, Type tImplementation, Lifetime lifetime = Lifetime.instPerDependency)
		{
			Implementation implementation = new Implementation(tImplementation, lifetime);

			if (tDependency.IsGenericType)
			{
				tDependency = tDependency.GetGenericTypeDefinition();
			}

			if (!Container.ContainsKey(tDependency))
			{
				Container[tDependency] = new List<Implementation>();
			}

			if (!Container[tDependency].Contains(implementation))
			{
				Container[tDependency].Add(implementation);
			}
		}

		public IEnumerable<Implementation> GetImplementations(Type type)
		{
			Type collectionType;

			if (type.IsGenericType)
			{
				collectionType = type.GetGenericTypeDefinition();
			}
			else
			{
				collectionType = type;
			}

			if (Container.TryGetValue(collectionType, out List<Implementation> dependencyImplementations))
			{
				IEnumerable<Implementation> result =
					new List<Implementation>(dependencyImplementations);
				if (type.IsGenericType)
				{
					result = result.Where((impl) => impl.ImplementationType.IsGenericTypeDefinition
													|| type.IsAssignableFrom(impl.ImplementationType));
				}

				return result;
			}
			else
			{
				return new List<Implementation>();
			}
		}

		public DependenciesConfiguration()
		{
			this.Container = new Dictionary<Type, List<Implementation>>();
		}
	}
}
