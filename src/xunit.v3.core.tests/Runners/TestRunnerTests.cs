using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

public class TestRunnerTests
{
	public class Run
	{
		[Fact]
		public static async ValueTask Passing()
		{
			var test = TestData.XunitTest<ClassUnderTest>(nameof(ClassUnderTest.Passing));
			var runner = new SimpleXunitTestRunner(test);

			await runner.Run();

			Assert.Single(runner.MessageBus.Messages.OfType<ITestPassed>());
			var starting = Assert.Single(runner.MessageBus.Messages.OfType<ITestStarting>());
			Assert.Equal($"{typeof(ClassUnderTest).FullName}.Passing", starting.TestDisplayName);
		}

		[Fact]
		public static async ValueTask Failing()
		{
			var test = TestData.XunitTest<ClassUnderTest>(nameof(ClassUnderTest.Failing));
			var runner = new SimpleXunitTestRunner(test);

			await runner.Run();

			Assert.Single(runner.MessageBus.Messages.OfType<ITestFailed>());
			var starting = Assert.Single(runner.MessageBus.Messages.OfType<ITestStarting>());
			Assert.Equal($"{typeof(ClassUnderTest).FullName}.Failing", starting.TestDisplayName);
		}

		[Fact]
		public static async ValueTask TooManyParameterValues()
		{
			var test = TestData.XunitTest<ClassUnderTest>(nameof(ClassUnderTest.Passing), testMethodArguments: [42]);
			var runner = new SimpleXunitTestRunner(test);

			await runner.Run();

			var failed = Assert.Single(runner.MessageBus.Messages.OfType<ITestFailed>());
			Assert.Equal("The test method expected 0 parameter values, but 1 parameter value was provided.", failed.Messages.Single());
		}

		[Fact]
		public static async ValueTask NotEnoughParameterValues()
		{
			var test = TestData.XunitTest<ClassUnderTest>(nameof(ClassUnderTest.FactWithParameter));
			var runner = new SimpleXunitTestRunner(test);

			await runner.Run();

			var failed = Assert.Single(runner.MessageBus.Messages.OfType<ITestFailed>());
			Assert.Equal("The test method expected 1 parameter value, but 0 parameter values were provided.", failed.Messages.Single());
		}

		[Fact]
		public static async ValueTask CancelledTestDoesNotRun()
		{
			var test = TestData.XunitTest<ClassUnderTest>(nameof(ClassUnderTest.Failing));
			var runner = new SimpleXunitTestRunner(test);
			runner.TokenSource.Cancel();

			await runner.Run();

			Assert.Collection(
				runner.MessageBus.Messages,
				message => Assert.IsType<ITestStarting>(message, exactMatch: false),
				// No result message
				message => Assert.IsType<ITestFinished>(message, exactMatch: false)
			);
		}

		[Fact]
		public static async ValueTask TestWithPreExistingErrorsFailsWithPreExistingError()
		{
			var test = TestData.XunitTest<ClassUnderTest>(nameof(ClassUnderTest.Failing));
			var runner = new SimpleXunitTestRunner(test);
			runner.Aggregator.Add(new DivideByZeroException());

			await runner.Run();

			var failed = Assert.Single(runner.MessageBus.Messages.OfType<ITestFailed>());
			Assert.Equal(typeof(DivideByZeroException).FullName, failed.ExceptionTypes.Single());
		}

		class ClassUnderTest
		{
			[Fact]
			public static void StaticPassing() { }

			[Fact]
			public void Passing() { }

			[Fact]
			public void Failing()
			{
				Assert.True(false);
			}

#pragma warning disable xUnit1001 // Fact methods cannot have parameters

			[Fact]
			public void FactWithParameter(int _) { }

#pragma warning restore xUnit1001 // Fact methods cannot have parameters
		}
	}

	public class InvocationsAndMessages
	{
		[Fact]
		public static async ValueTask Passed()
		{
			var testClassInstance = new object();

			var runner = new TestableTestRunner
			{
				CreateTestClassInstance__Result = testClassInstance,
				InvokeTest__Lambda = () => Assert.Same(testClassInstance, TestContext.Current.TestClassInstance),
				PostInvoke__Lambda = () => Assert.Same(testClassInstance, TestContext.Current.TestClassInstance),
				PreInvoke__Lambda = () => Assert.Same(testClassInstance, TestContext.Current.TestClassInstance),
			};

			var summary = await runner.Run();

			VerifyRunSummary(summary);
			Assert.False(runner.Aggregator.HasExceptions);
			Assert.Collection(
				runner.Invocations,
				msg => Assert.Equal("SetTextContext(testStatus: Initializing, testState: null)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: Running, testState: null)", msg),
				msg => Assert.Equal("IsTestClassCreatable", msg),
				msg => Assert.Equal("OnTestClassConstructionStarting", msg),
				msg => Assert.Equal("CreateTestClassInstance", msg),
				msg => Assert.Equal("OnTestClassConstructionFinished", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: typeof(object), testState: null)", msg),
				msg => Assert.Equal("PreInvoke", msg),
				// UpdateTestContext
				msg => Assert.Equal("InvokeTest(testClassInstance: typeof(object))", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: typeof(object), testState: Passed)", msg),
				msg => Assert.Equal("PostInvoke", msg),
				// UpdateTestContext
				msg => Assert.Equal("IsTestClassDisposable", msg),
				msg => Assert.Equal("OnTestClassDisposeStarting", msg),
				msg => Assert.Equal("DisposeTestClassInstance", msg),
				msg => Assert.Equal("OnTestClassDisposeFinished", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: null, testState: Passed)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: CleaningUp, testState: Passed)", msg)
			);
			Assert.Collection(
				runner.MessageBus.Messages,
				msg => Assert.IsType<ITestStarting>(msg, exactMatch: false),
				msg => Assert.IsType<ITestClassConstructionStarting>(msg, exactMatch: false),
				msg => Assert.IsType<ITestClassConstructionFinished>(msg, exactMatch: false),
				// InvokeTest
				msg => Assert.IsType<ITestClassDisposeStarting>(msg, exactMatch: false),
				msg => Assert.IsType<ITestClassDisposeFinished>(msg, exactMatch: false),
				msg => Assert.IsType<ITestPassed>(msg, exactMatch: false),
				msg => Assert.IsType<ITestFinished>(msg, exactMatch: false)
			);
		}

		[Fact]
		public static async ValueTask Failed()
		{
			var runner = new TestableTestRunner
			{
				InvokeTest__Lambda = () => Assert.True(false),
				IsTestClassCreatable__Result = false,  // Turning off creation just to make sure we don't call things we don't need to
			};

			var summary = await runner.Run();

			VerifyRunSummary(summary, failed: 1);
			Assert.False(runner.Aggregator.HasExceptions);
			Assert.Collection(
				runner.Invocations,
				msg => Assert.Equal("SetTextContext(testStatus: Initializing, testState: null)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: Running, testState: null)", msg),
				msg => Assert.Equal("IsTestClassCreatable", msg),
				// OnTestClassConstructionStarting
				// CreateTestClassInstance
				// OnTestClassConstructionFinished
				msg => Assert.Equal("UpdateTestContext(testClassInstance: null, testState: null)", msg),
				msg => Assert.Equal("PreInvoke", msg),
				// UpdateTestContext
				msg => Assert.Equal("InvokeTest(testClassInstance: null)", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: null, testState: Failed)", msg),
				msg => Assert.Equal("PostInvoke", msg),
				// UpdateTestContext
				// IsTestClassDisposable
				// OnTestClassDisposeStarting
				// DisposeTestClassInstance
				// OnTestClassDisposeFinished
				msg => Assert.Equal("UpdateTestContext(testClassInstance: null, testState: Failed)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: CleaningUp, testState: Failed)", msg)
			);
			Assert.Collection(
				runner.MessageBus.Messages,
				msg => Assert.IsType<ITestStarting>(msg, exactMatch: false),
				// ITestClassConstructionStarting
				// ITestClassConstructionFinished
				// ITestClassDisposeStarting
				// ITestClassDisposeFinished
				msg => Assert.IsType<ITestFailed>(msg, exactMatch: false),
				msg => Assert.IsType<ITestFinished>(msg, exactMatch: false)
			);
		}

		[Fact]
		public static async ValueTask Skipped()
		{
			var runner = new TestableTestRunner();

			var summary = await runner.Run("Don't run me");

			VerifyRunSummary(summary, skipped: 1);
			Assert.False(runner.Aggregator.HasExceptions);
			Assert.Collection(
				runner.Invocations,
				msg => Assert.Equal("SetTextContext(testStatus: Initializing, testState: null)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: Running, testState: null)", msg),
				// IsTestClassCreatable
				// OnTestClassConstructionStarting
				// CreateTestClassInstance
				// OnTestClassConstructionFinished
				// UpdateTestContext
				// PreInvoke
				// UpdateTestContext
				// InvokeTest
				// UpdateTestContext
				// PostInvoke
				// UpdateTestContext
				// IsTestClassDisposable
				// OnTestClassDisposeStarting
				// DisposeTestClassInstance
				// OnTestClassDisposeFinished
				// UpdateTestContext
				msg => Assert.Equal("SetTextContext(testStatus: CleaningUp, testState: Skipped)", msg)
			);
			Assert.Collection(
				runner.MessageBus.Messages,
				msg => Assert.IsType<ITestStarting>(msg, exactMatch: false),
				// ITestClassConstructionStarting
				// ITestClassConstructionFinished
				// ITestClassDisposeStarting
				// ITestClassDisposeFinished
				msg => Assert.IsType<ITestSkipped>(msg, exactMatch: false),
				msg => Assert.IsType<ITestFinished>(msg, exactMatch: false)
			);
		}

		[Fact]
		public static async ValueTask NotRun()
		{
			var runner = new TestableTestRunner { ShouldTestRun__Result = false };

			var summary = await runner.Run();

			VerifyRunSummary(summary, notRun: 1);
			Assert.False(runner.Aggregator.HasExceptions);
			Assert.Collection(
				runner.Invocations,
				msg => Assert.Equal("SetTextContext(testStatus: Initializing, testState: null)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: Running, testState: null)", msg),
				// IsTestClassCreatable
				// OnTestClassConstructionStarting
				// CreateTestClassInstance
				// OnTestClassConstructionFinished
				// UpdateTestContext
				// PreInvoke
				// UpdateTestContext
				// InvokeTest
				// UpdateTestContext
				// PostInvoke
				// UpdateTestContext
				// IsTestClassDisposable
				// OnTestClassDisposeStarting
				// DisposeTestClassInstance
				// OnTestClassDisposeFinished
				// UpdateTestContext
				msg => Assert.Equal("SetTextContext(testStatus: CleaningUp, testState: NotRun)", msg)
			);
			Assert.Collection(
				runner.MessageBus.Messages,
				msg => Assert.IsType<ITestStarting>(msg, exactMatch: false),
				// ITestClassConstructionStarting
				// ITestClassConstructionFinished
				// ITestClassDisposeStarting
				// ITestClassDisposeFinished
				msg => Assert.IsType<ITestNotRun>(msg, exactMatch: false),
				msg => Assert.IsType<ITestFinished>(msg, exactMatch: false)
			);
		}
	}

	public class HandlerExceptions
	{
		[Fact]
		public static async ValueTask CreateTestClassInstance()
		{
			var runner = new TestableTestRunner { CreateTestClassInstance__Lambda = () => throw new DivideByZeroException() };

			var summary = await runner.Run();

			VerifyRunSummary(summary, failed: 1);
			Assert.False(runner.Aggregator.HasExceptions);
			Assert.Collection(
				runner.Invocations,
				msg => Assert.Equal("SetTextContext(testStatus: Initializing, testState: null)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: Running, testState: null)", msg),
				msg => Assert.Equal("IsTestClassCreatable", msg),
				msg => Assert.Equal("OnTestClassConstructionStarting", msg),
				msg => Assert.Equal("CreateTestClassInstance", msg),
				msg => Assert.Equal("OnTestClassConstructionFinished", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: null, testState: Failed)", msg),
				// PreInvoke
				// UpdateTestContext
				// InvokeTest
				// UpdateTestContext
				// PostInvoke
				// UpdateTestContext
				// IsTestClassDisposable
				// OnTestClassDisposeStarting
				// DisposeTestClassInstance
				// OnTestClassDisposeFinished
				// UpdateTestContext
				msg => Assert.Equal("SetTextContext(testStatus: CleaningUp, testState: Failed)", msg)
			);
			Assert.Contains(runner.MessageBus.Messages, msg => msg is ITestFailed);
		}

		[Fact]
		public static async ValueTask DisposeTestClassInstance()
		{
			var runner = new TestableTestRunner
			{
				CreateTestClassInstance__Result = new object(),
				DisposeTestClassInstance__Lambda = () => throw new DivideByZeroException(),
			};

			var summary = await runner.Run();

			VerifyRunSummary(summary, failed: 1);
			Assert.False(runner.TokenSource.IsCancellationRequested);
			Assert.False(runner.Aggregator.HasExceptions);
			Assert.Collection(
				runner.Invocations,
				msg => Assert.Equal("SetTextContext(testStatus: Initializing, testState: null)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: Running, testState: null)", msg),
				msg => Assert.Equal("IsTestClassCreatable", msg),
				msg => Assert.Equal("OnTestClassConstructionStarting", msg),
				msg => Assert.Equal("CreateTestClassInstance", msg),
				msg => Assert.Equal("OnTestClassConstructionFinished", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: typeof(object), testState: null)", msg),
				msg => Assert.Equal("PreInvoke", msg),
				// UpdateTestContext
				msg => Assert.Equal("InvokeTest(testClassInstance: typeof(object))", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: typeof(object), testState: Passed)", msg),
				msg => Assert.Equal("PostInvoke", msg),
				// UpdateTestContext
				msg => Assert.Equal("IsTestClassDisposable", msg),
				msg => Assert.Equal("OnTestClassDisposeStarting", msg),
				msg => Assert.Equal("DisposeTestClassInstance", msg),
				msg => Assert.Equal("OnTestClassDisposeFinished", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: null, testState: Failed)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: CleaningUp, testState: Failed)", msg)
			);
			Assert.Contains(runner.MessageBus.Messages, msg => msg is ITestFailed);
		}

		[Fact]
		public static async ValueTask IsTestClassCreatable()
		{
			var runner = new TestableTestRunner { IsTestClassCreatable__Lambda = () => throw new DivideByZeroException() };

			var summary = await runner.Run();

			VerifyRunSummary(summary, failed: 1);
			Assert.False(runner.Aggregator.HasExceptions);
			Assert.Collection(
				runner.Invocations,
				msg => Assert.Equal("SetTextContext(testStatus: Initializing, testState: null)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: Running, testState: null)", msg),
				msg => Assert.Equal("IsTestClassCreatable", msg),
				// OnTestClassConstructionStarting
				// CreateTestClassInstance
				// OnTestClassConstructionFinished
				msg => Assert.Equal("UpdateTestContext(testClassInstance: null, testState: Failed)", msg),
				// PreInvoke
				// UpdateTestContext
				// InvokeTest
				// UpdateTestContext
				// PostInvoke
				// UpdateTestContext
				// IsTestClassDisposable
				// OnTestClassDisposeStarting
				// DisposeTestClassInstance
				// OnTestClassDisposeFinished
				// UpdateTestContext
				msg => Assert.Equal("SetTextContext(testStatus: CleaningUp, testState: Failed)", msg)
			);
			Assert.Contains(runner.MessageBus.Messages, msg => msg is ITestFailed);
		}

		[Fact]
		public static async ValueTask IsTestClassDisposable()
		{
			var runner = new TestableTestRunner
			{
				CreateTestClassInstance__Result = new object(),
				IsTestClassDisposable__Lambda = () => throw new DivideByZeroException(),
			};

			var summary = await runner.Run();

			VerifyRunSummary(summary, failed: 1);
			Assert.False(runner.Aggregator.HasExceptions);
			Assert.Collection(
				runner.Invocations,
				msg => Assert.Equal("SetTextContext(testStatus: Initializing, testState: null)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: Running, testState: null)", msg),
				msg => Assert.Equal("IsTestClassCreatable", msg),
				msg => Assert.Equal("OnTestClassConstructionStarting", msg),
				msg => Assert.Equal("CreateTestClassInstance", msg),
				msg => Assert.Equal("OnTestClassConstructionFinished", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: typeof(object), testState: null)", msg),
				msg => Assert.Equal("PreInvoke", msg),
				// UpdateTestContext
				msg => Assert.Equal("InvokeTest(testClassInstance: typeof(object))", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: typeof(object), testState: Passed)", msg),
				msg => Assert.Equal("PostInvoke", msg),
				// UpdateTestContext
				msg => Assert.Equal("IsTestClassDisposable", msg),
				// OnTestClassDisposeStarting
				// DisposeTestClassInstance
				// OnTestClassDisposeFinished
				msg => Assert.Equal("UpdateTestContext(testClassInstance: null, testState: Failed)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: CleaningUp, testState: Failed)", msg)
			);
			Assert.Contains(runner.MessageBus.Messages, msg => msg is ITestFailed);
		}

		[Fact]
		public static async ValueTask OnTestClassConstructionFinished()
		{
			var runner = new TestableTestRunner { OnTestClassConstructionFinished__Lambda = () => throw new DivideByZeroException() };

			var summary = await runner.Run();

			VerifyRunSummary(summary, failed: 1);
			Assert.False(runner.Aggregator.HasExceptions);
			Assert.Collection(
				runner.Invocations,
				msg => Assert.Equal("SetTextContext(testStatus: Initializing, testState: null)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: Running, testState: null)", msg),
				msg => Assert.Equal("IsTestClassCreatable", msg),
				msg => Assert.Equal("OnTestClassConstructionStarting", msg),
				msg => Assert.Equal("CreateTestClassInstance", msg),
				msg => Assert.Equal("OnTestClassConstructionFinished", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: null, testState: Failed)", msg),
				// PreInvoke
				// UpdateTestContext
				// InvokeTest
				// UpdateTestContext
				// PostInvoke
				// UpdateTestContext
				// IsTestClassDisposable
				// OnTestClassDisposeStarting
				// DisposeTestClassInstance
				// OnTestClassDisposeFinished
				// UpdateTestContext
				msg => Assert.Equal("SetTextContext(testStatus: CleaningUp, testState: Failed)", msg)
			);
			Assert.Contains(runner.MessageBus.Messages, msg => msg is ITestFailed);
		}

		[Fact]
		public static async ValueTask OnTestClassConstructionStarting()
		{
			var runner = new TestableTestRunner { OnTestClassConstructionStarting__Lambda = () => throw new DivideByZeroException() };

			var summary = await runner.Run();

			VerifyRunSummary(summary, failed: 1);
			Assert.False(runner.Aggregator.HasExceptions);
			Assert.Collection(
				runner.Invocations,
				msg => Assert.Equal("SetTextContext(testStatus: Initializing, testState: null)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: Running, testState: null)", msg),
				msg => Assert.Equal("IsTestClassCreatable", msg),
				msg => Assert.Equal("OnTestClassConstructionStarting", msg),
				// CreateTestClassInstance
				// OnTestClassConstructionFinished
				msg => Assert.Equal("UpdateTestContext(testClassInstance: null, testState: Failed)", msg),
				// PreInvoke
				// UpdateTestContext
				// InvokeTest
				// UpdateTestContext
				// PostInvoke
				// UpdateTestContext
				// IsTestClassDisposable
				// OnTestClassDisposeStarting
				// DisposeTestClassInstance
				// OnTestClassDisposeFinished
				// UpdateTestContext
				msg => Assert.Equal("SetTextContext(testStatus: CleaningUp, testState: Failed)", msg)
			);
			Assert.Contains(runner.MessageBus.Messages, msg => msg is ITestFailed);
		}

		[Fact]
		public static async ValueTask OnTestClassDisposeFinished()
		{
			var runner = new TestableTestRunner
			{
				CreateTestClassInstance__Result = new object(),
				OnTestClassDisposeFinished__Lambda = () => throw new DivideByZeroException(),
			};

			var summary = await runner.Run();

			VerifyRunSummary(summary, failed: 1);
			Assert.False(runner.Aggregator.HasExceptions);
			Assert.Collection(
				runner.Invocations,
				msg => Assert.Equal("SetTextContext(testStatus: Initializing, testState: null)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: Running, testState: null)", msg),
				msg => Assert.Equal("IsTestClassCreatable", msg),
				msg => Assert.Equal("OnTestClassConstructionStarting", msg),
				msg => Assert.Equal("CreateTestClassInstance", msg),
				msg => Assert.Equal("OnTestClassConstructionFinished", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: typeof(object), testState: null)", msg),
				msg => Assert.Equal("PreInvoke", msg),
				// UpdateTestContext
				msg => Assert.Equal("InvokeTest(testClassInstance: typeof(object))", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: typeof(object), testState: Passed)", msg),
				msg => Assert.Equal("PostInvoke", msg),
				// UpdateTestContext
				msg => Assert.Equal("IsTestClassDisposable", msg),
				msg => Assert.Equal("OnTestClassDisposeStarting", msg),
				msg => Assert.Equal("DisposeTestClassInstance", msg),
				msg => Assert.Equal("OnTestClassDisposeFinished", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: null, testState: Failed)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: CleaningUp, testState: Failed)", msg)
			);
			Assert.Contains(runner.MessageBus.Messages, msg => msg is ITestFailed);
		}

		[Fact]
		public static async ValueTask OnTestClassDisposeStarting()
		{
			var runner = new TestableTestRunner
			{
				CreateTestClassInstance__Result = new object(),
				OnTestClassDisposeStarting__Lambda = () => throw new DivideByZeroException(),
			};

			var summary = await runner.Run();

			VerifyRunSummary(summary, failed: 1);
			Assert.False(runner.Aggregator.HasExceptions);
			Assert.Collection(
				runner.Invocations,
				msg => Assert.Equal("SetTextContext(testStatus: Initializing, testState: null)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: Running, testState: null)", msg),
				msg => Assert.Equal("IsTestClassCreatable", msg),
				msg => Assert.Equal("OnTestClassConstructionStarting", msg),
				msg => Assert.Equal("CreateTestClassInstance", msg),
				msg => Assert.Equal("OnTestClassConstructionFinished", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: typeof(object), testState: null)", msg),
				msg => Assert.Equal("PreInvoke", msg),
				// UpdateTestContext
				msg => Assert.Equal("InvokeTest(testClassInstance: typeof(object))", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: typeof(object), testState: Passed)", msg),
				msg => Assert.Equal("PostInvoke", msg),
				// UpdateTestContext
				msg => Assert.Equal("IsTestClassDisposable", msg),
				msg => Assert.Equal("OnTestClassDisposeStarting", msg),
				msg => Assert.Equal("DisposeTestClassInstance", msg),
				msg => Assert.Equal("OnTestClassDisposeFinished", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: null, testState: Failed)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: CleaningUp, testState: Failed)", msg)
			);
			Assert.Contains(runner.MessageBus.Messages, msg => msg is ITestFailed);
		}

		[Fact]
		public static async ValueTask PostInvoke()
		{
			var runner = new TestableTestRunner
			{
				CreateTestClassInstance__Result = new object(),
				PostInvoke__Lambda = () => throw new DivideByZeroException(),
			};

			var summary = await runner.Run();

			VerifyRunSummary(summary, failed: 1);
			Assert.False(runner.TokenSource.IsCancellationRequested);
			Assert.False(runner.Aggregator.HasExceptions);
			Assert.Collection(
				runner.Invocations,
				msg => Assert.Equal("SetTextContext(testStatus: Initializing, testState: null)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: Running, testState: null)", msg),
				msg => Assert.Equal("IsTestClassCreatable", msg),
				msg => Assert.Equal("OnTestClassConstructionStarting", msg),
				msg => Assert.Equal("CreateTestClassInstance", msg),
				msg => Assert.Equal("OnTestClassConstructionFinished", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: typeof(object), testState: null)", msg),
				msg => Assert.Equal("PreInvoke", msg),
				// UpdateTestContext
				msg => Assert.Equal("InvokeTest(testClassInstance: typeof(object))", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: typeof(object), testState: Passed)", msg),
				msg => Assert.Equal("PostInvoke", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: typeof(object), testState: Failed)", msg),
				msg => Assert.Equal("IsTestClassDisposable", msg),
				msg => Assert.Equal("OnTestClassDisposeStarting", msg),
				msg => Assert.Equal("DisposeTestClassInstance", msg),
				msg => Assert.Equal("OnTestClassDisposeFinished", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: null, testState: Failed)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: CleaningUp, testState: Failed)", msg)
			);
			Assert.Contains(runner.MessageBus.Messages, msg => msg is ITestFailed);
		}

		[Fact]
		public static async ValueTask PreInvoke()
		{
			var runner = new TestableTestRunner
			{
				CreateTestClassInstance__Result = new object(),
				PreInvoke__Lambda = () => throw new DivideByZeroException(),
			};

			var summary = await runner.Run();

			VerifyRunSummary(summary, failed: 1);
			Assert.False(runner.TokenSource.IsCancellationRequested);
			Assert.False(runner.Aggregator.HasExceptions);
			Assert.Collection(
				runner.Invocations,
				msg => Assert.Equal("SetTextContext(testStatus: Initializing, testState: null)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: Running, testState: null)", msg),
				msg => Assert.Equal("IsTestClassCreatable", msg),
				msg => Assert.Equal("OnTestClassConstructionStarting", msg),
				msg => Assert.Equal("CreateTestClassInstance", msg),
				msg => Assert.Equal("OnTestClassConstructionFinished", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: typeof(object), testState: null)", msg),
				msg => Assert.Equal("PreInvoke", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: typeof(object), testState: Failed)", msg),
				// InvokeTest
				// UpdateTestContext
				// PostInvoke
				// UpdateTestContext
				msg => Assert.Equal("IsTestClassDisposable", msg),
				msg => Assert.Equal("OnTestClassDisposeStarting", msg),
				msg => Assert.Equal("DisposeTestClassInstance", msg),
				msg => Assert.Equal("OnTestClassDisposeFinished", msg),
				msg => Assert.Equal("UpdateTestContext(testClassInstance: null, testState: Failed)", msg),
				msg => Assert.Equal("SetTextContext(testStatus: CleaningUp, testState: Failed)", msg)
			);
			Assert.Contains(runner.MessageBus.Messages, msg => msg is ITestFailed);
		}
	}

	static void VerifyRunSummary(
		RunSummary summary,
		int total = 1,
		int failed = 0,
		int notRun = 0,
		int skipped = 0) =>
			Assert.Equivalent(new { Total = total, Failed = failed, NotRun = notRun, Skipped = skipped }, summary);

	// This is a lightweight version of XunitTestRunner, with just enough implementation to allow it to
	// create instances of test classes (without concern for things like ctor arguments or IAsyncLifetime).
	class SimpleXunitTestRunner(IXunitTest test) :
		TestRunner<TestRunnerContext<IXunitTest>, IXunitTest>
	{
		public readonly ExceptionAggregator Aggregator = new();
		public readonly SpyMessageBus MessageBus = new();
		public readonly CancellationTokenSource TokenSource = new();

		protected override ValueTask<(object? Instance, SynchronizationContext? SyncContext, ExecutionContext? ExecutionContext)> CreateTestClassInstance(TestRunnerContext<IXunitTest> ctxt) =>
			new((Activator.CreateInstance(ctxt.Test.TestCase.TestClass.Class, []), SynchronizationContext.Current, ExecutionContext.Capture()));

		protected override bool IsTestClassCreatable(TestRunnerContext<IXunitTest> ctxt) =>
			!ctxt.Test.TestCase.TestMethod.Method.IsStatic;

		public async ValueTask<RunSummary> Run()
		{
			await using var ctxt = new TestRunnerContext<IXunitTest>(
				test,
				MessageBus,
				test.TestCase.SkipReason,
				ExplicitOption.Off,
				Aggregator,
				TokenSource,
				test.TestMethod.Method,
				test.TestMethodArguments
			);
			await ctxt.InitializeAsync();

			return await Run(ctxt);
		}
	}

	// This is an inspectable version of TestRunner, which logs extensibility method calls and assumes that the provided test is
	// a mock, so it points to a NOOP method for its test method to be invoked.
	class TestableTestRunner(ITest? test = null) :
		TestRunner<TestRunnerContext<ITest>, ITest>
	{
		public readonly ExceptionAggregator Aggregator = new();
		public readonly List<string> Invocations = [];
		public readonly SpyMessageBus MessageBus = new();
		public readonly ITest Test = test ?? Mocks.Test();
		public readonly MethodInfo TestMethod = typeof(TestableTestRunner).GetMethod(nameof(_TestMethod), BindingFlags.NonPublic | BindingFlags.Static) ?? throw new InvalidOperationException("Could not find TestableTestRunner._TestMethod");
		public readonly CancellationTokenSource TokenSource = new();

		public Action? CreateTestClassInstance__Lambda;
		public object? CreateTestClassInstance__Result;

		protected override ValueTask<(object? Instance, SynchronizationContext? SyncContext, ExecutionContext? ExecutionContext)> CreateTestClassInstance(TestRunnerContext<ITest> ctxt)
		{
			Invocations.Add("CreateTestClassInstance");

			CreateTestClassInstance__Lambda?.Invoke();

			return new((CreateTestClassInstance__Result, SynchronizationContext.Current, ExecutionContext.Capture()));
		}

		public Action? DisposeTestClassInstance__Lambda;

		protected override ValueTask DisposeTestClassInstance(
			TestRunnerContext<ITest> ctxt,
			object testClassInstance)
		{
			Invocations.Add("DisposeTestClassInstance");

			DisposeTestClassInstance__Lambda?.Invoke();

			return default;
		}

		public Action? InvokeTest__Lambda;
		public TimeSpan InvokeTest__Result = TimeSpan.Zero;

		protected override ValueTask<TimeSpan> InvokeTest(
			TestRunnerContext<ITest> ctxt,
			object? testClassInstance)
		{
			Assert.Same(CreateTestClassInstance__Result, testClassInstance);

			Invocations.Add($"InvokeTest(testClassInstance: {TypeName(testClassInstance)})");

			InvokeTest__Lambda?.Invoke();

			return new(InvokeTest__Result);
		}

		public Action? IsTestClassCreatable__Lambda;
		public bool IsTestClassCreatable__Result = true;

		protected override bool IsTestClassCreatable(TestRunnerContext<ITest> ctxt)
		{
			Invocations.Add("IsTestClassCreatable");

			IsTestClassCreatable__Lambda?.Invoke();

			return IsTestClassCreatable__Result;
		}

		public Action? IsTestClassDisposable__Lambda;
		public bool IsTestClassDisposable__Result = true;

		protected override bool IsTestClassDisposable(
			TestRunnerContext<ITest> ctxt,
			object testClassInstance)
		{
			Invocations.Add("IsTestClassDisposable");

			IsTestClassDisposable__Lambda?.Invoke();

			return IsTestClassDisposable__Result;
		}

		public Action? OnTestClassConstructionFinished__Lambda;
		public bool OnTestClassConstructionFinished__Result = true;

		protected override async ValueTask<bool> OnTestClassConstructionFinished(TestRunnerContext<ITest> ctxt)
		{
			Invocations.Add("OnTestClassConstructionFinished");

			OnTestClassConstructionFinished__Lambda?.Invoke();

			await base.OnTestClassConstructionFinished(ctxt);

			return OnTestClassConstructionFinished__Result;
		}

		public Action? OnTestClassConstructionStarting__Lambda;
		public bool OnTestClassConstructionStarting__Result = true;

		protected override async ValueTask<bool> OnTestClassConstructionStarting(TestRunnerContext<ITest> ctxt)
		{
			Invocations.Add("OnTestClassConstructionStarting");

			OnTestClassConstructionStarting__Lambda?.Invoke();

			await base.OnTestClassConstructionStarting(ctxt);

			return OnTestClassConstructionStarting__Result;
		}

		public Action? OnTestClassDisposeFinished__Lambda;
		public bool OnTestClassDisposeFinished__Result = true;

		protected override async ValueTask<bool> OnTestClassDisposeFinished(TestRunnerContext<ITest> ctxt)
		{
			Invocations.Add("OnTestClassDisposeFinished");

			OnTestClassDisposeFinished__Lambda?.Invoke();

			await base.OnTestClassDisposeFinished(ctxt);

			return OnTestClassDisposeFinished__Result;
		}

		public Action? OnTestClassDisposeStarting__Lambda;
		public bool OnTestClassDisposeStarting__Result = true;

		protected override async ValueTask<bool> OnTestClassDisposeStarting(TestRunnerContext<ITest> ctxt)
		{
			Invocations.Add("OnTestClassDisposeStarting");

			OnTestClassDisposeStarting__Lambda?.Invoke();

			await base.OnTestClassDisposeStarting(ctxt);

			return OnTestClassDisposeStarting__Result;
		}

		public Action? PostInvoke__Lambda;

		protected override void PostInvoke(TestRunnerContext<ITest> ctxt)
		{
			Invocations.Add("PostInvoke");

			PostInvoke__Lambda?.Invoke();
		}

		public Action? PreInvoke__Lambda;

		protected override void PreInvoke(TestRunnerContext<ITest> ctxt)
		{
			Invocations.Add("PreInvoke");

			PreInvoke__Lambda?.Invoke();
		}

		public async ValueTask<RunSummary> Run(string? skipReason = null)
		{
			await using var ctxt = new TestRunnerContext<ITest>(Test, MessageBus, skipReason, ExplicitOption.Off, Aggregator, TokenSource, TestMethod, []);
			await ctxt.InitializeAsync();

			return await Run(ctxt);
		}

		protected override void SetTestContext(
			TestRunnerContext<ITest> ctxt,
			TestEngineStatus testStatus,
			TestResultState? testState = null,
			object? testClassInstance = null)
		{
			Invocations.Add($"SetTextContext(testStatus: {testStatus}, testState: {testState?.Result.ToString() ?? "null"})");

			base.SetTestContext(ctxt, testStatus, testState, testClassInstance);
		}

		public bool ShouldTestRun__Result = true;

		protected override bool ShouldTestRun(TestRunnerContext<ITest> ctxt) =>
			ShouldTestRun__Result;

		static string TypeName(object? value) =>
			value is null ? "null" : $"typeof({ArgumentFormatter.FormatTypeName(value.GetType())})";

		protected override void UpdateTestContext(
			object? testClassInstance,
			TestResultState? testState = null)
		{
			Invocations.Add($"UpdateTestContext(testClassInstance: {TypeName(testClassInstance)}, testState: {testState?.Result.ToString() ?? "null"})");

			base.UpdateTestContext(testClassInstance, testState);
		}

		static void _TestMethod() { }
	}
}
