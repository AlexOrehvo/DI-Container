using DependencyContainer;
using DependencyContainerTest.Implementations;
using DependencyContainerTest.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyContainerTest
{
	[TestClass]
	public class DependencyProviderTest
	{
		private DependencyContainer.DependenciesConfiguration configuration;
		private DependencyContainer.DependencyProvider provider;

		[TestInitialize]
		public void Initialize()
		{
			configuration = new DependencyContainer.DependenciesConfiguration();
		}

		[TestMethod]
		public void SingleImplResolve()
		{
			configuration.Register<Interface1, Implementation1>();
			provider = new DependencyContainer.DependencyProvider(configuration);

			var actual = provider.Resolve<Interface1>();

			var expected = new List<Type>
			{
				typeof(Implementation1)
			};

			CollectionAssert.AreEquivalent(expected,
                actual.Select((instance) => instance.GetType()).ToList());
		}

		[TestMethod]
		public void MultipleImplResolve()
		{
			configuration.Register<Interface1, Implementation1>();
			configuration.Register<Interface1, Implementation2>();
			provider = new DependencyContainer.DependencyProvider(configuration);

			var actual = provider.Resolve<Interface1>();

			var expected = new List<Type>
			{
				typeof(Implementation1),
				typeof(Implementation2),
			};

			CollectionAssert.AreEquivalent(expected,
				actual.Select((instance) => instance.GetType()).ToList());
		}

		[TestMethod]
		public void LifetimeTest()
		{
			configuration.Register<Interface1, Implementation1>(Lifetime.singleton);
			provider = new DependencyProvider(configuration);

			var instance1 = provider.Resolve<Interface1>().First();
			var instance2 = provider.Resolve<Interface1>().First();

			Assert.AreSame(instance1, instance2);
		}
	}
}
