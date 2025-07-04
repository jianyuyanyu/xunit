using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Internal;
using Xunit.Sdk;

namespace Xunit.v3;

// Extensibility points related to test methods

public static partial class ExtensibilityPointFactory
{
	/// <summary>
	/// Gets the <see cref="IBeforeAfterTestAttribute"/>s attached to the given method.
	/// </summary>
	/// <param name="testMethod">The test method</param>
	/// <param name="classBeforeAfterAttributes">The before after attributes from the test class,
	/// to be merged into the result.</param>
	public static IReadOnlyCollection<IBeforeAfterTestAttribute> GetMethodBeforeAfterTestAttributes(
		MethodInfo testMethod,
		IReadOnlyCollection<IBeforeAfterTestAttribute> classBeforeAfterAttributes)
	{
		var warnings = new List<string>();

		try
		{
			return
				Guard.ArgumentNotNull(classBeforeAfterAttributes)
					.Concat(Guard.ArgumentNotNull(testMethod).GetMatchingCustomAttributes<IBeforeAfterTestAttribute>(warnings))
					.CastOrToReadOnlyCollection();
		}
		finally
		{
			foreach (var warning in warnings)
				TestContext.Current.SendDiagnosticMessage(warning);
		}
	}

	/// <summary>
	/// Gets the <see cref="IDataAttribute"/>s attached to the given test method.
	/// </summary>
	/// <param name="testMethod">The test method</param>
	public static IReadOnlyCollection<IDataAttribute> GetMethodDataAttributes(MethodInfo testMethod)
	{
		var warnings = new List<string>();

		try
		{
			var result = Guard.ArgumentNotNull(testMethod).GetMatchingCustomAttributes<IDataAttribute>(warnings);

			foreach (var typeAwareAttribute in result.OfType<ITypeAwareDataAttribute>())
				typeAwareAttribute.MemberType ??= testMethod.ReflectedType;

			return result;
		}
		finally
		{
			foreach (var warning in warnings)
				TestContext.Current.SendDiagnosticMessage(warning);
		}
	}

	/// <summary>
	/// Gets the <see cref="IFactAttribute"/>s attached to the given test method.
	/// </summary>
	/// <param name="testMethod">The test method</param>
	public static IReadOnlyCollection<IFactAttribute> GetMethodFactAttributes(MethodInfo testMethod)
	{
		var warnings = new List<string>();

		try
		{
			return Guard.ArgumentNotNull(testMethod).GetMatchingCustomAttributes<IFactAttribute>(warnings);
		}
		finally
		{
			foreach (var warning in warnings)
				TestContext.Current.SendDiagnosticMessage(warning);
		}
	}

	/// <summary>
	/// Gets the traits that are attached to the test method via <see cref="ITraitAttribute"/>s.
	/// </summary>
	/// <param name="testMethod">The test method</param>
	/// <param name="testClassTraits">The traits inherited from the test class</param>
	public static IReadOnlyDictionary<string, IReadOnlyCollection<string>> GetMethodTraits(
		MethodInfo testMethod,
		IReadOnlyDictionary<string, IReadOnlyCollection<string>>? testClassTraits)
	{
		Guard.ArgumentNotNull(testMethod);

		var warnings = new List<string>();

		try
		{
			var result = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

			if (testClassTraits is not null)
				foreach (var trait in testClassTraits)
					result.AddOrGet(trait.Key).AddRange(trait.Value);

			foreach (var traitAttribute in testMethod.GetMatchingCustomAttributes<ITraitAttribute>(warnings))
				foreach (var kvp in traitAttribute.GetTraits())
					result.AddOrGet(kvp.Key).Add(kvp.Value);

			return result.ToReadOnly();
		}
		finally
		{
			foreach (var warning in warnings)
				TestContext.Current.SendDiagnosticMessage(warning);
		}
	}
}
