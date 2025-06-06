using System;
using Xunit.Runner.Common;
using Xunit.Sdk;

namespace Xunit.Internal;

/// <summary>
/// INTERNAL CLASS. DO NOT USE.
/// </summary>
public sealed class FilterClassSimpleName(string filter) :
	ITestCaseFilter
{
	readonly Func<string?, bool> evaluator = QueryFilterParser.ToEvaluator(filter);

	/// <summary/>
	public bool Filter(
		string assemblyName,
		ITestCaseMetadata testCase) =>
			evaluator(Guard.ArgumentNotNull(testCase).TestClassSimpleName);
}
