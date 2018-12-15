using System;
using System.Collections.Generic;

namespace DependencyContainer
{
	public class DependenciesConfiguration
	{
		Dictionary<Type, List<Implementation>> container;

		public void Register<TDependency, TImplementation>(Lifetime lifetime = Lifetime.instPerDependency)
		{
			Register(typeof(TDependency), typeof(TImplementation), lifetime);
		}

		public void Register(Type tDependency, Type tImplementation, Lifetime lifetime = Lifetime.instPerDependency)
		{
			Implementation implementation = new Implementation(tImplementation, lifetime);

			if (!container.ContainsKey(tDependency))
			{
				container[tDependency] = new List<Implementation>();
			}

			if (!container[tDependency].Contains(implementation))
			{
				container[tDependency].Add(implementation);
			}
		}
		
		public DependenciesConfiguration()
		{
			this.container = new Dictionary<Type, List<Implementation>>();
		}
	}
}
