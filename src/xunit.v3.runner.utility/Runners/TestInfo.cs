using System;
using System.Collections.Generic;
using Xunit.Internal;

namespace Xunit.Runners;

/// <summary>
/// A base class which contains information about a test.
/// </summary>
[Obsolete("Please use the TestInfo class from the Xunit.SimpleRunner namespace. This class will be removed in the next major release.")]
public abstract class TestInfo
{
	/// <summary/>
	protected TestInfo(
		string typeName,
		string methodName,
		IReadOnlyDictionary<string, IReadOnlyCollection<string>>? traits,
		string testDisplayName,
		string testCollectionDisplayName)
	{
		Guard.ArgumentNotNull(typeName);
		Guard.ArgumentNotNull(methodName);
		Guard.ArgumentNotNull(testDisplayName);
		Guard.ArgumentNotNull(testCollectionDisplayName);

		TypeName = typeName;
		MethodName = methodName;
		Traits = traits ?? new Dictionary<string, IReadOnlyCollection<string>>(StringComparer.OrdinalIgnoreCase);
		TestDisplayName = testDisplayName;
		TestCollectionDisplayName = testCollectionDisplayName;
	}

	/// <summary>
	/// The fully qualified type name of the class that contains the test.
	/// </summary>
	public string TypeName { get; }

	/// <summary>
	/// The name of the method that contains the test.
	/// </summary>
	public string MethodName { get; }

	/// <summary>
	/// The traits associated with the test.
	/// </summary>
	public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Traits { get; }

	/// <summary>
	/// The display name for the test.
	/// </summary>
	public string TestDisplayName { get; }

	/// <summary>
	/// The display name of the test collection the test belongs to.
	/// </summary>
	public string TestCollectionDisplayName { get; }
}
