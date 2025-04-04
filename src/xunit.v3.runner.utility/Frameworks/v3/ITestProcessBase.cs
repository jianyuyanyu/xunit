using System;

namespace Xunit.v3;

/// <summary>
/// Represents a v3 test process that has been launched. When the process is disposed,
/// it should be allowed to clean up and exit within an appropriate amount of time,
/// and then killed if it will not stop cleanly.
/// </summary>
/// <remarks>
/// The use of the word "process" here is an abstraction across the default implementation
/// which does launch v3 tests as a separate local operating-system process.
/// </remarks>
public interface ITestProcessBase : IDisposable
{
	/// <summary>
	/// Gets a flag to indicate whether the test process has exited or not yet.
	/// </summary>
	bool HasExited { get; }

	/// <summary>
	/// Cancels the test pipeline, forcefully if necessary.
	/// </summary>
	/// <param name="forceCancellation">When set to <c>false</c>, this should request graceful termination
	/// of the test pipeline; when set to <c>true</c>, the test process should be forcefully shut down as
	/// quickly as possible.</param>
	/// <remarks>
	/// Note that repeated calls to this method with <paramref name="forceCancellation"/> set to <c>false</c>
	/// may be possible, since it may be dispatched every time a remote runner returns <c>false</c> from
	/// a message sink/message bus call. For out of process runners using Ctrl+C via standard input, it
	/// should only send Ctrl+C the first time this is called (since double Ctrl+C is the forceful
	/// cancellation signal).
	/// </remarks>
	void Cancel(bool forceCancellation);

	/// <summary>
	/// Wait for the specified number of milliseconds for the test process to exit.
	/// </summary>
	/// <param name="milliseconds">The amount of time, in milliseconds, to wait</param>
	/// <returns>Returns <c>true</c> if the process exited; <c>false</c>, otherwise</returns>
	bool WaitForExit(int milliseconds);
}
