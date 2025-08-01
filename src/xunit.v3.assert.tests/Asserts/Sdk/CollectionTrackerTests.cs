using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;

public class CollectionTrackerTests
{
	public class FormatIndexedMismatch_IEnumerable
	{
		[Fact]
		public static void ExceededDepth()
		{
			var tracker = new[] { 42, 2112 }.AsTracker();

			var result = tracker.FormatIndexedMismatch(2600, out var pointerIndent, ArgumentFormatter.MaxEnumerableLength + 1);

			Assert.Equal($"[{ArgumentFormatter.Ellipsis}]", result);
			//             - ^
			Assert.Equal(1, pointerIndent);
		}

		[Fact]
		public static void SmallCollection_Start()
		{
			var tracker = new[] { 42, 2112 }.AsTracker();

			var result = tracker.FormatIndexedMismatch(0, out var pointerIndent);

			Assert.Equal("[42, 2112]", result);
			//            -^
			Assert.Equal(1, pointerIndent);
		}

		[Fact]
		public static void LargeCollection_Start()
		{
			var tracker = new[] { 1, 2, 3, 4, 5, 6, 7 }.AsTracker();

			var result = tracker.FormatIndexedMismatch(1, out var pointerIndent);

			Assert.Equal($"[1, 2, 3, 4, 5, {ArgumentFormatter.Ellipsis}]", result);
			//             ----^
			Assert.Equal(4, pointerIndent);
		}

		[Fact]
		public static void LargeCollection_Mid()
		{
			var tracker = new[] { 1, 2, 3, 4, 5, 6, 7 }.AsTracker();

			var result = tracker.FormatIndexedMismatch(3, out var pointerIndent);

			Assert.Equal($"[{ArgumentFormatter.Ellipsis}, 2, 3, 4, 5, 6, {ArgumentFormatter.Ellipsis}]", result);
			//             - ---                        |----|--^
			Assert.Equal(12, pointerIndent);
		}

		[Fact]
		public static void LargeCollection_End()
		{
			var tracker = new[] { 1, 2, 3, 4, 5, 6, 7 }.AsTracker();

			var result = tracker.FormatIndexedMismatch(6, out var pointerIndent);

			Assert.Equal($"[{ArgumentFormatter.Ellipsis}, 3, 4, 5, 6, 7]", result);
			//             - ---                        |----|----|---^
			Assert.Equal(18, pointerIndent);
		}
	}

	public class FormatIndexedMismatch_Span
	{
		[Fact]
		public static void ExceededDepth()
		{
			var span = new[] { 42, 2112 }.AsSpan();

			var result = CollectionTracker<int>.FormatIndexedMismatch(span, 2600, out var pointerIndent, ArgumentFormatter.MaxEnumerableLength + 1);

			Assert.Equal($"[{ArgumentFormatter.Ellipsis}]", result);
			//             - ^
			Assert.Equal(1, pointerIndent);
		}

		[Fact]
		public static void SmallCollection_Start()
		{
			var span = new[] { 42, 2112 }.AsSpan();

			var result = CollectionTracker<int>.FormatIndexedMismatch(span, 0, out var pointerIndent);

			Assert.Equal("[42, 2112]", result);
			//            -^
			Assert.Equal(1, pointerIndent);
		}

		[Fact]
		public static void LargeCollection_Start()
		{
			var span = new[] { 1, 2, 3, 4, 5, 6, 7 }.AsSpan();

			var result = CollectionTracker<int>.FormatIndexedMismatch(span, 1, out var pointerIndent);

			Assert.Equal($"[1, 2, 3, 4, 5, {ArgumentFormatter.Ellipsis}]", result);
			//             ----^
			Assert.Equal(4, pointerIndent);
		}

		[Fact]
		public static void LargeCollection_Mid()
		{
			var span = new[] { 1, 2, 3, 4, 5, 6, 7 }.AsSpan();

			var result = CollectionTracker<int>.FormatIndexedMismatch(span, 3, out var pointerIndent);

			Assert.Equal($"[{ArgumentFormatter.Ellipsis}, 2, 3, 4, 5, 6, {ArgumentFormatter.Ellipsis}]", result);
			//             - ---                        |----|--^
			Assert.Equal(12, pointerIndent);
		}

		[Fact]
		public static void LargeCollection_End()
		{
			var span = new[] { 1, 2, 3, 4, 5, 6, 7 }.AsSpan();

			var result = CollectionTracker<int>.FormatIndexedMismatch(span, 6, out var pointerIndent);

			Assert.Equal($"[{ArgumentFormatter.Ellipsis}, 3, 4, 5, 6, 7]", result);
			//             - ---                        |----|----|---^
			Assert.Equal(18, pointerIndent);
		}
	}

	public class FormatStart_IEnumerable_Tracked
	{
		[Fact]
		public static void Empty()
		{
			var tracker = Array.Empty<object>().AsTracker();

			Assert.Equal("[]", tracker.FormatStart());
		}

		[Fact]
		public static void ExceededDepth()
		{
			var tracker = Array.Empty<object>().AsTracker();

			Assert.Equal($"[{ArgumentFormatter.Ellipsis}]", tracker.FormatStart(ArgumentFormatter.MaxEnumerableLength + 1));
		}

		[CulturedFact(["en-US", "fr-FR"])]
		public static void Short()
		{
			var tracker = new object[] { 1, 2.3M, "Hello, world!" }.AsTracker();

			Assert.Equal($"[1, {2.3M}, \"Hello, world!\"]", tracker.FormatStart());
		}

		[CulturedFact(["en-US", "fr-FR"])]
		public static void Long()
		{
			var tracker = new object[] { 1, 2.3M, "Hello, world!", 42, 2112, new() }.AsTracker();

			Assert.Equal($"[1, {2.3M}, \"Hello, world!\", 42, 2112, {ArgumentFormatter.Ellipsis}]", tracker.FormatStart());
		}
	}

	public class FormatStart_IEnumerable_Untracked
	{
		[Fact]
		public static void Empty()
		{
			IEnumerable<object> collection = [];

			Assert.Equal("[]", CollectionTracker<object>.FormatStart(collection));
		}

		[Fact]
		public static void ExceededDepth()
		{
			IEnumerable<object> collection = [];

			Assert.Equal($"[{ArgumentFormatter.Ellipsis}]", CollectionTracker<object>.FormatStart(collection, ArgumentFormatter.MaxEnumerableLength + 1));
		}

		[CulturedFact(["en-US", "fr-FR"])]
		public static void Short()
		{
			IEnumerable<object> collection = [1, 2.3M, "Hello, world!"];

			Assert.Equal($"[1, {2.3M}, \"Hello, world!\"]", CollectionTracker<object>.FormatStart(collection));
		}

		[CulturedFact(["en-US", "fr-FR"])]
		public static void Long()
		{
			IEnumerable<object> collection = [1, 2.3M, "Hello, world!", 42, 2112, new object()];

			Assert.Equal($"[1, {2.3M}, \"Hello, world!\", 42, 2112, {ArgumentFormatter.Ellipsis}]", CollectionTracker<object>.FormatStart(collection));
		}
	}

	public class FormatStart_Span
	{
		[Fact]
		public static void Empty()
		{
			var span = Array.Empty<object>().AsSpan();

			Assert.Equal("[]", CollectionTracker<object>.FormatStart(span));
		}

		[Fact]
		public static void ExceededDepth()
		{
			var span = Array.Empty<object>().AsSpan();

			Assert.Equal($"[{ArgumentFormatter.Ellipsis}]", CollectionTracker<object>.FormatStart(span, ArgumentFormatter.MaxEnumerableLength + 1));
		}

		[CulturedFact(["en-US", "fr-FR"])]
		public static void Short()
		{
			var span = new object[] { 1, 2.3M, "Hello, world!" }.AsSpan();

			Assert.Equal($"[1, {2.3M}, \"Hello, world!\"]", CollectionTracker<object>.FormatStart(span));
		}

		[CulturedFact(["en-US", "fr-FR"])]
		public static void Long()
		{
			var span = new object[] { 1, 2.3M, "Hello, world!", 42, 2112, new() }.AsSpan();

			Assert.Equal($"[1, {2.3M}, \"Hello, world!\", 42, 2112, {ArgumentFormatter.Ellipsis}]", CollectionTracker<object>.FormatStart(span));
		}
	}
}
