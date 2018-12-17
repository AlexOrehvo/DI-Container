using DependencyContainerTest.Implementations;
using DependencyContainerTest.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyContainerTest
{
	[TestClass]
	public class DependencyContainerTest
	{
		DependencyContainer.DependenciesConfiguration configuration;
		DependencyContainer.DependencyProvider provider;

		[TestInitialize]
		public void Initialize()
		{
			configuration = new DependencyContainer.DependenciesConfiguration();
		}

		[TestMethod]
		public void NonGenericTypeRegisterTest()
		{
			configuration.Register<Interface1, Implementation1>();
			configuration.Register<Interface1, Implementation2>();
			var implementations = configuration.GetImplementations(typeof(Interface1)).ToList();
			Assert.AreEqual(2, implementations.Count);

			List<Type> expectedTypes = new List<Type> { typeof(Implementation1), typeof(Implementation2) };

			List<Type> actualTypes = implementations.Select((impl) => impl.ImplementationType).ToList();

			CollectionAssert.AreEquivalent(expectedTypes, actualTypes);
		}

		[TestMethod]
		public void GenericTypeRegisterTest()
		{
			configuration.Register<GenericInterface1<Interface1>, GenericImplementation1<Interface1>>();
			configuration.Register<GenericInterface1<Interface1>, GenericImplementation2<Interface1>>();
			var implementations = configuration.GetImplementations(typeof(GenericInterface1<Interface1>)).ToList();
			Assert.AreEqual(2, implementations.Count);

			List<Type> expectedTypes = new List<Type>
			{ typeof(GenericImplementation1<Interface1>), typeof(GenericImplementation2<Interface1>) };

			List<Type> actualTypes = implementations.Select((impl) => impl.ImplementationType).ToList();

			CollectionAssert.AreEquivalent(expectedTypes, actualTypes);
		}

		[TestMethod]
		public void OpenGenericRegisterTest()
		{
			configuration.Register(typeof(GenericInterface1<>), typeof(GenericImplementation1<>));
			configuration.Register(typeof(GenericInterface1<>), typeof(GenericImplementation2<>));
			var implementations = configuration.GetImplementations(typeof(GenericInterface1<>)).ToList();
			Assert.AreEqual(2, implementations.Count);

			List<Type> expectedTypes = new List<Type>
			{ typeof(GenericImplementation1<>), typeof(GenericImplementation2<>) };


			List<Type> actualTypes = implementations.Select((impl) => impl.ImplementationType).ToList();

			CollectionAssert.AreEquivalent(expectedTypes, actualTypes);
		}
	}
}
