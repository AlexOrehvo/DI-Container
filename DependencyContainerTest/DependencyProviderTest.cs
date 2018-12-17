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
			Assert.IsInstanceOfType(actual, typeof(Interface1));
		}

		[TestMethod]
		public void MultipleImplResolve()
		{
			configuration.Register<Interface1, Implementation1>();
			configuration.Register<Interface1, Implementation2>();
			provider = new DependencyContainer.DependencyProvider(configuration);

			var actual = provider.Resolve<IEnumerable<Interface1>>();
			Assert.IsInstanceOfType(actual, typeof(IEnumerable<Interface1>));
		}
	}
}
