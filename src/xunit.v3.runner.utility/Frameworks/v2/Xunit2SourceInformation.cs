using System;
using Xunit.Abstractions;
using Xunit.Internal;

namespace Xunit.Runner.v2;

/// <summary>
/// Default implementation of <see cref="ISourceInformation"/>.
/// </summary>
public class Xunit2SourceInformation : MarshalByRefObject, ISourceInformation
{
	/// <inheritdoc/>
	public string? FileName { get; set; }

	/// <inheritdoc/>
	public int? LineNumber { get; set; }

	/// <inheritdoc/>
	public void Serialize(IXunitSerializationInfo info)
	{
		Guard.ArgumentNotNull(info);

		info.AddValue("FileName", FileName);
		info.AddValue("LineNumber", LineNumber);
	}

	/// <inheritdoc/>
	public void Deserialize(IXunitSerializationInfo info)
	{
		Guard.ArgumentNotNull(info);

		FileName = info.GetValue<string>("FileName");
		LineNumber = info.GetValue<int?>("LineNumber");
	}

#if NETFRAMEWORK
	/// <inheritdoc/>
	[System.Security.SecurityCritical]
	public override sealed object InitializeLifetimeService() => null!;
#endif
}
