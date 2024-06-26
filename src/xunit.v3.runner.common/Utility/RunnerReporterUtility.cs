using System.Collections.Generic;

namespace Xunit.Runner.Common;

/// <summary>
/// A utility class for finding runner reporters.
/// </summary>
public static class RunnerReporterUtility
{
	/// <summary>
	/// Gets the list of reporters that are embedded inside xUnit.net itself. Does not include <see cref="DefaultRunnerReporter"/>.
	/// </summary>
	public static IReadOnlyList<IRunnerReporter> EmbeddedReporters { get; } =
	[
		new AppVeyorReporter(),
		new JsonReporter(),
		new QuietReporter(),
		new SilentReporter(),
		new TeamCityReporter(),
		new VerboseReporter(),
		new VstsReporter(),
	];

	/// <summary>
	/// Gets a list of runner reporters from DLLs in the given folder. NOTE: Runner reporter extensibility is not yet
	/// available for v3 projects, so this currently returns an empty list (plus the embedded reporters, if they are
	/// requested).
	/// </summary>
	/// <param name="folder">The folder to search for reporters in</param>
	/// <param name="includeEmbeddedReporters">A flag to indicate if the embedded reporters should be included. This
	/// is the list of reporters built into xUnit.net, except for <see cref="DefaultRunnerReporter"/>.</param>
	/// <param name="messages">Messages that were generated during discovery</param>
	/// <returns>List of available reporters</returns>
	public static List<IRunnerReporter> GetAvailableRunnerReporters(
		string folder,
		bool includeEmbeddedReporters,
		out List<string> messages)
	{
		var result = new List<IRunnerReporter>();
		messages = [];

		if (includeEmbeddedReporters)
			result.AddRange(EmbeddedReporters);

#if false
		string[] dllFiles;

		try
		{
			dllFiles = Directory.GetFiles(folder, "*reporters*.dll").Select(f => Path.Combine(folder, f)).ToArray();
		}
		catch (Exception ex)
		{
			messages.Add(string.Format(CultureInfo.CurrentCulture, "Exception thrown looking for reporters in folder '{0}':{1}{2}", folder, Environment.NewLine, ex));
			return result;
		}

		foreach (var dllFile in dllFiles)
		{
			Type?[] types;

			try
			{
				var assembly = Assembly.LoadFile(dllFile);
				types = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				types = ex.Types;
			}
			catch
			{
				continue;
			}

			foreach (var type in types)
			{
				if (type is null || type.IsAbstract || type.GetCustomAttribute<HiddenRunnerReporterAttribute>() is not null || type.GetInterfaces().All(t => t != typeof(IRunnerReporter)))
					continue;

				try
				{
					var ctor = type.GetConstructor(Type.EmptyTypes);
					if (ctor == null)
					{
						messages.Add(
							string.Format(
								CultureInfo.CurrentCulture,
								"Type '{0}' in assembly '{1}' appears to be a runner reporter, but does not have an empty constructor.",
								type.FullName ?? type.Name,
								dllFile
							)
						);

						continue;
					}

					result.Add((IRunnerReporter)ctor.Invoke(Array.Empty<object>()));
				}
				catch (Exception ex)
				{
					messages.Add(
						string.Format(
							CultureInfo.CurrentCulture,
							"Exception thrown while inspecting type '{0}' in assembly '{1}':{2}{3}",
							type.FullName ?? type.Name,
							dllFile,
							Environment.NewLine,
							ex
						)
					);
				}
			}
		}
#endif

		return result;
	}
}
