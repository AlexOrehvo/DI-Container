using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyContainer
{
	internal class Implementation
	{
		private Lifetime Lifetime { get; set; }
		private Type ImplementationType { get; }

		public Implementation(Type implementationType, Lifetime lifetime)
		{
			this.ImplementationType = implementationType;
			this.Lifetime = lifetime;
		}
	}
}
