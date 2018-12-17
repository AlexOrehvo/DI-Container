using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyContainer
{
	public class Implementation
	{
		internal Lifetime Lifetime { get; set; }
		internal object SingletonInstance { get; set; }

		public Type ImplementationType { get; }

		public Implementation(Type implementationType, Lifetime lifetime)
		{
			this.ImplementationType = implementationType;
			this.Lifetime = lifetime;
		}
	}
}
