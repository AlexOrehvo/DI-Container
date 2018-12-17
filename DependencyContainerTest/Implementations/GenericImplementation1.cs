using DependencyContainerTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyContainerTest.Implementations
{
	class GenericImplementation1<T> : GenericInterface1<T>
	{
		public T dependency;

		public GenericImplementation1(T dep)
		{
			this.dependency = dep;
		}
	}
}
