using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

public class DisplayNameFormatterTests
{
	[Theory]
	[MemberData(nameof(ClassAndMethodWithoutOptions))]
	public void FormatShouldReturnExpectedDisplayNameFromClassAndMethodWithoutAnyOptions(
		string name,
		string expected)
	{
		var formatter = new DisplayNameFormatter(display: TestMethodDisplay.ClassAndMethod, displayOptions: TestMethodDisplayOptions.None);
		var actual = formatter.Format(name);

		Assert.Equal(expected, actual);
	}

	[Theory]
	[MemberData(nameof(MethodWithoutOptions))]
	public void FormatShouldReturnExpectedDisplayNameFromMethodWithoutAnyOptions(
		string name,
		string expected)
	{
		var formatter = new DisplayNameFormatter(display: TestMethodDisplay.Method, displayOptions: TestMethodDisplayOptions.None);
		var actual = formatter.Format(name);

		Assert.Equal(expected, actual);
	}

	[Theory]
	[MemberData(nameof(ClassAndMethodWithAllOptions))]
	public void FormatShouldReturnExpectedDisplayNameFromClassAndMethodWithAllOptions(
		string name,
		string expected)
	{
		var formatter = new DisplayNameFormatter(display: TestMethodDisplay.ClassAndMethod, displayOptions: TestMethodDisplayOptions.All);
		var actual = formatter.Format(name);

		Assert.Equal(expected, actual);
	}

	[Theory]
	[MemberData(nameof(MethodWithAllOptions))]
	public void FormatShouldReturnExpectedDisplayNameFromMethodWithAllOptions(
		string name,
		string expected)
	{
		var formatter = new DisplayNameFormatter(display: TestMethodDisplay.Method, displayOptions: TestMethodDisplayOptions.All);
		var actual = formatter.Format(name);

		Assert.Equal(expected, actual);
	}

	[Theory]
	[MemberData(nameof(ClassAndMethodWithReplaceUnderscoreOption))]
	public void FormatShouldReturnExpectedDisplayNameFromClassAndMethodWithSpacesInsteadOfUnderscores(
		string name,
		string expected)
	{
		var formatter = new DisplayNameFormatter(display: TestMethodDisplay.ClassAndMethod, displayOptions: TestMethodDisplayOptions.ReplaceUnderscoreWithSpace);
		var actual = formatter.Format(name);

		Assert.Equal(expected, actual);
	}

	[Theory]
	[MemberData(nameof(MethodWithReplaceUnderscoreOption))]
	public void FormatShouldReturnExpectedDisplayNameFromMethodWithSpacesInsteadOfUnderscores(
		string name,
		string expected)
	{
		var formatter = new DisplayNameFormatter(display: TestMethodDisplay.Method, displayOptions: TestMethodDisplayOptions.ReplaceUnderscoreWithSpace);
		var actual = formatter.Format(name);

		Assert.Equal(expected, actual);
	}

	[Theory]
	[MemberData(nameof(ClassAndMethodWithReplaceUnderscoreAndOperatorOption))]
	public void FormatShouldReturnExpectedDisplayNameFromClassAndMethodWithReplacedSpacesAndOperators(
		string name,
		string expected)
	{
		var formatter = new DisplayNameFormatter(display: TestMethodDisplay.ClassAndMethod, displayOptions: TestMethodDisplayOptions.ReplaceUnderscoreWithSpace | TestMethodDisplayOptions.UseOperatorMonikers);
		var actual = formatter.Format(name);

		Assert.Equal(expected, actual);
	}

	[Theory]
	[MemberData(nameof(MethodWithReplaceUnderscoreAndOperatorOption))]
	public void FormatShouldReturnExpectedDisplayNameFromMethodWithReplacedSpacesAndOperators(
		string name,
		string expected)
	{
		var formatter = new DisplayNameFormatter(display: TestMethodDisplay.Method, displayOptions: TestMethodDisplayOptions.ReplaceUnderscoreWithSpace | TestMethodDisplayOptions.UseOperatorMonikers);
		var actual = formatter.Format(name);

		Assert.Equal(expected, actual);
	}

	[Theory]
	[MemberData(nameof(ClassAndMethodWithReplaceUnderscoreAndEscapeSequenceOption))]
	public void FormatShouldReturnExpectedDisplayNameFromClassAndMethodWithReplacedSpacesAndEscapeSequences(
		string name,
		string expected)
	{
		var formatter = new DisplayNameFormatter(display: TestMethodDisplay.ClassAndMethod, displayOptions: TestMethodDisplayOptions.ReplaceUnderscoreWithSpace | TestMethodDisplayOptions.UseEscapeSequences);
		var actual = formatter.Format(name);

		Assert.Equal(expected, actual);
	}

	[Theory]
	[MemberData(nameof(MethodWithReplaceUnderscoreAndEscapeSequenceOption))]
	public void FormatShouldReturnExpectedDisplayNameFromMethodWithReplacedSpacesAndEscapeSequences(
		string name,
		string expected)
	{
		var formatter = new DisplayNameFormatter(display: TestMethodDisplay.Method, displayOptions: TestMethodDisplayOptions.ReplaceUnderscoreWithSpace | TestMethodDisplayOptions.UseEscapeSequences);
		var actual = formatter.Format(name);

		Assert.Equal(expected, actual);
	}

	public static IEnumerable<TheoryDataRow<string, string>> ClassAndMethodWithoutOptions => NoOptionsDisplayData(TestMethodDisplay.ClassAndMethod);

	public static IEnumerable<TheoryDataRow<string, string>> MethodWithoutOptions => NoOptionsDisplayData(TestMethodDisplay.Method);

	public static IEnumerable<TheoryDataRow<string, string>> ClassAndMethodWithAllOptions => AllOptionsDisplayData(TestMethodDisplay.ClassAndMethod);

	public static IEnumerable<TheoryDataRow<string, string>> MethodWithAllOptions => AllOptionsDisplayData(TestMethodDisplay.Method);

	public static IEnumerable<TheoryDataRow<string, string>> ClassAndMethodWithReplaceUnderscoreOption => ReplaceUnderscoreOnlyDisplayData(TestMethodDisplay.ClassAndMethod);

	public static IEnumerable<TheoryDataRow<string, string>> MethodWithReplaceUnderscoreOption => ReplaceUnderscoreOnlyDisplayData(TestMethodDisplay.Method);

	public static IEnumerable<TheoryDataRow<string, string>> ClassAndMethodWithReplaceUnderscoreAndOperatorOption => ReplaceUnderscoreAndOperatorDisplayData(TestMethodDisplay.ClassAndMethod);

	public static IEnumerable<TheoryDataRow<string, string>> MethodWithReplaceUnderscoreAndOperatorOption => ReplaceUnderscoreAndOperatorDisplayData(TestMethodDisplay.Method);

	public static IEnumerable<TheoryDataRow<string, string>> ClassAndMethodWithReplaceUnderscoreAndEscapeSequenceOption => ReplaceUnderscoreAndEscapeSequenceDisplayData(TestMethodDisplay.ClassAndMethod);

	public static IEnumerable<TheoryDataRow<string, string>> MethodWithReplaceUnderscoreAndEscapeSequenceOption => ReplaceUnderscoreAndEscapeSequenceDisplayData(TestMethodDisplay.Method);

	private static string NameOf(Action testMethod) => $"{testMethod.Method.DeclaringType?.FullName}.{testMethod.Method.Name}";

	private static IEnumerable<TheoryDataRow<string, string>> NoOptionsDisplayData(TestMethodDisplay methodDisplay)
	{
		if (methodDisplay == TestMethodDisplay.ClassAndMethod)
		{
			yield return new(NameOf(FormattedDisplayNameExample.unit_tests_are_awesomeX21_U263A), NameOf(FormattedDisplayNameExample.unit_tests_are_awesomeX21_U263A));
			yield return new(NameOf(FormattedDisplayNameExample.api_version_1_eq_1X2E0), NameOf(FormattedDisplayNameExample.api_version_1_eq_1X2E0));
			yield return new(NameOf(FormattedDisplayNameExample.api_version_should_be_greater_than_1), NameOf(FormattedDisplayNameExample.api_version_should_be_greater_than_1));
			yield return new(NameOf(FormattedDisplayNameExample.api_version_2_should_be_gt_than_1), NameOf(FormattedDisplayNameExample.api_version_2_should_be_gt_than_1));
			yield return new(NameOf(FormattedDisplayNameExample.api_version_2_should_be_ge_than_1), NameOf(FormattedDisplayNameExample.api_version_2_should_be_ge_than_1));
			yield return new(NameOf(FormattedDisplayNameExample.api_version_1_should_be_lt_than_2), NameOf(FormattedDisplayNameExample.api_version_1_should_be_lt_than_2));
			yield return new(NameOf(FormattedDisplayNameExample.api_version_1_should_be_le_than_2), NameOf(FormattedDisplayNameExample.api_version_1_should_be_le_than_2));
			yield return new(NameOf(FormattedDisplayNameExample.api_version_1X2E0_should_ne_2U002E0), NameOf(FormattedDisplayNameExample.api_version_1X2E0_should_ne_2U002E0));
			yield return new(NameOf(FormattedDisplayNameExample.lt_0_should_be_an_error), NameOf(FormattedDisplayNameExample.lt_0_should_be_an_error));
			yield return new(NameOf(FormattedDisplayNameExample.equals_operator_overload_should_be_same_as_eq_eq), NameOf(FormattedDisplayNameExample.equals_operator_overload_should_be_same_as_eq_eq));
			yield return new(NameOf(FormattedDisplayNameExample.X3DX3D_operator_overload_should_be_same_as_equals_method), NameOf(FormattedDisplayNameExample.X3DX3D_operator_overload_should_be_same_as_equals_method));
			yield return new(NameOf(FormattedDisplayNameExample.masculine_super_heroes_should_be_buffed), NameOf(FormattedDisplayNameExample.masculine_super_heroes_should_be_buffed));
			yield return new(NameOf(FormattedDisplayNameExample.termination_date_should_be_updated_when_employee_is_axed), NameOf(FormattedDisplayNameExample.termination_date_should_be_updated_when_employee_is_axed));
			yield return new(NameOf(FormattedDisplayNameExample.total_amount_should_be_updated_when_order_is_taxed), NameOf(FormattedDisplayNameExample.total_amount_should_be_updated_when_order_is_taxed));
			yield return new(NameOf(FormattedDisplayNameExample.X27stuffedX27_should_not_be_ambiguous_with_X27stUFFEDX27), NameOf(FormattedDisplayNameExample.X27stuffedX27_should_not_be_ambiguous_with_X27stUFFEDX27));
			yield return new(NameOf(FormattedDisplayNameExample.X27maxed_outX27_should_not_be_ambiguous_with_X27maXED_outX27), NameOf(FormattedDisplayNameExample.X27maxed_outX27_should_not_be_ambiguous_with_X27maXED_outX27));
			yield return new(NameOf(FormattedDisplayNameExample.TestNameShouldRemainUnchanged), NameOf(FormattedDisplayNameExample.TestNameShouldRemainUnchanged));
			yield return new(NameOf(FormattedDisplayNameExample.TestX20NameX20WithU0020Spaces), NameOf(FormattedDisplayNameExample.TestX20NameX20WithU0020Spaces));
			yield return new(NameOf(FormattedDisplayNameExample.Parameter_X22RETURN_VALUEX22_is_always_added), NameOf(FormattedDisplayNameExample.Parameter_X22RETURN_VALUEX22_is_always_added));
			yield return new(NameOf(FormattedDisplayNameExample.Parameter_X22RETURN_VALUE_X22_is_always_added), NameOf(FormattedDisplayNameExample.Parameter_X22RETURN_VALUE_X22_is_always_added));
			yield return new(NameOf(Given_a_version_number.when_it_equals_1.then_it_should_be_less_than_2), NameOf(Given_a_version_number.when_it_equals_1.then_it_should_be_less_than_2));
		}
		else
		{
			yield return new(nameof(FormattedDisplayNameExample.unit_tests_are_awesomeX21_U263A), nameof(FormattedDisplayNameExample.unit_tests_are_awesomeX21_U263A));
			yield return new(nameof(FormattedDisplayNameExample.api_version_1_eq_1X2E0), nameof(FormattedDisplayNameExample.api_version_1_eq_1X2E0));
			yield return new(nameof(FormattedDisplayNameExample.api_version_should_be_greater_than_1), nameof(FormattedDisplayNameExample.api_version_should_be_greater_than_1));
			yield return new(nameof(FormattedDisplayNameExample.api_version_2_should_be_gt_than_1), nameof(FormattedDisplayNameExample.api_version_2_should_be_gt_than_1));
			yield return new(nameof(FormattedDisplayNameExample.api_version_2_should_be_ge_than_1), nameof(FormattedDisplayNameExample.api_version_2_should_be_ge_than_1));
			yield return new(nameof(FormattedDisplayNameExample.api_version_1_should_be_lt_than_2), nameof(FormattedDisplayNameExample.api_version_1_should_be_lt_than_2));
			yield return new(nameof(FormattedDisplayNameExample.api_version_1_should_be_le_than_2), nameof(FormattedDisplayNameExample.api_version_1_should_be_le_than_2));
			yield return new(nameof(FormattedDisplayNameExample.api_version_1X2E0_should_ne_2U002E0), nameof(FormattedDisplayNameExample.api_version_1X2E0_should_ne_2U002E0));
			yield return new(nameof(FormattedDisplayNameExample.lt_0_should_be_an_error), nameof(FormattedDisplayNameExample.lt_0_should_be_an_error));
			yield return new(nameof(FormattedDisplayNameExample.equals_operator_overload_should_be_same_as_eq_eq), nameof(FormattedDisplayNameExample.equals_operator_overload_should_be_same_as_eq_eq));
			yield return new(nameof(FormattedDisplayNameExample.X3DX3D_operator_overload_should_be_same_as_equals_method), nameof(FormattedDisplayNameExample.X3DX3D_operator_overload_should_be_same_as_equals_method));
			yield return new(nameof(FormattedDisplayNameExample.masculine_super_heroes_should_be_buffed), nameof(FormattedDisplayNameExample.masculine_super_heroes_should_be_buffed));
			yield return new(nameof(FormattedDisplayNameExample.termination_date_should_be_updated_when_employee_is_axed), nameof(FormattedDisplayNameExample.termination_date_should_be_updated_when_employee_is_axed));
			yield return new(nameof(FormattedDisplayNameExample.total_amount_should_be_updated_when_order_is_taxed), nameof(FormattedDisplayNameExample.total_amount_should_be_updated_when_order_is_taxed));
			yield return new(nameof(FormattedDisplayNameExample.X27stuffedX27_should_not_be_ambiguous_with_X27stUFFEDX27), nameof(FormattedDisplayNameExample.X27stuffedX27_should_not_be_ambiguous_with_X27stUFFEDX27));
			yield return new(nameof(FormattedDisplayNameExample.X27maxed_outX27_should_not_be_ambiguous_with_X27maXED_outX27), nameof(FormattedDisplayNameExample.X27maxed_outX27_should_not_be_ambiguous_with_X27maXED_outX27));
			yield return new(nameof(FormattedDisplayNameExample.TestNameShouldRemainUnchanged), nameof(FormattedDisplayNameExample.TestNameShouldRemainUnchanged));
			yield return new(nameof(FormattedDisplayNameExample.TestX20NameX20WithU0020Spaces), nameof(FormattedDisplayNameExample.TestX20NameX20WithU0020Spaces));
			yield return new(nameof(FormattedDisplayNameExample.Parameter_X22RETURN_VALUEX22_is_always_added), nameof(FormattedDisplayNameExample.Parameter_X22RETURN_VALUEX22_is_always_added));
			yield return new(nameof(FormattedDisplayNameExample.Parameter_X22RETURN_VALUE_X22_is_always_added), nameof(FormattedDisplayNameExample.Parameter_X22RETURN_VALUE_X22_is_always_added));
			yield return new(nameof(Given_a_version_number.when_it_equals_1.then_it_should_be_less_than_2), nameof(Given_a_version_number.when_it_equals_1.then_it_should_be_less_than_2));
		}
	}

	private static IEnumerable<TheoryDataRow<string, string>> AllOptionsDisplayData(TestMethodDisplay methodDisplay)
	{
		if (methodDisplay == TestMethodDisplay.ClassAndMethod)
		{
			yield return new(NameOf(FormattedDisplayNameExample.unit_tests_are_awesomeX21_U263A), "FormattedDisplayNameExample, unit tests are awesome! \u263A");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_1_eq_1X2E0), "FormattedDisplayNameExample, api version 1 = 1.0");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_should_be_greater_than_1), "FormattedDisplayNameExample, api version should be greater than 1");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_2_should_be_gt_than_1), "FormattedDisplayNameExample, api version 2 should be > than 1");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_2_should_be_ge_than_1), "FormattedDisplayNameExample, api version 2 should be >= than 1");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_1_should_be_lt_than_2), "FormattedDisplayNameExample, api version 1 should be < than 2");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_1_should_be_le_than_2), "FormattedDisplayNameExample, api version 1 should be <= than 2");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_1X2E0_should_ne_2U002E0), "FormattedDisplayNameExample, api version 1.0 should != 2.0");
			yield return new(NameOf(FormattedDisplayNameExample.lt_0_should_be_an_error), "FormattedDisplayNameExample, < 0 should be an error");
			yield return new(NameOf(FormattedDisplayNameExample.equals_operator_overload_should_be_same_as_eq_eq), "FormattedDisplayNameExample, equals operator overload should be same as = =");
			yield return new(NameOf(FormattedDisplayNameExample.X3DX3D_operator_overload_should_be_same_as_equals_method), "FormattedDisplayNameExample, == operator overload should be same as equals method");
			yield return new(NameOf(FormattedDisplayNameExample.masculine_super_heroes_should_be_buffed), "FormattedDisplayNameExample, masculine super heroes should be buffed");
			yield return new(NameOf(FormattedDisplayNameExample.termination_date_should_be_updated_when_employee_is_axed), "FormattedDisplayNameExample, termination date should be updated when employee is axed");
			yield return new(NameOf(FormattedDisplayNameExample.total_amount_should_be_updated_when_order_is_taxed), "FormattedDisplayNameExample, total amount should be updated when order is taxed");
			yield return new(NameOf(FormattedDisplayNameExample.X27stuffedX27_should_not_be_ambiguous_with_X27stUFFEDX27), "FormattedDisplayNameExample, 'stuffed' should not be ambiguous with 'st￭'");
			yield return new(NameOf(FormattedDisplayNameExample.X27maxed_outX27_should_not_be_ambiguous_with_X27maXED_outX27), "FormattedDisplayNameExample, 'maxed out' should not be ambiguous with 'maí out'");
			yield return new(NameOf(FormattedDisplayNameExample.TestNameShouldRemainUnchanged), "FormattedDisplayNameExample, TestNameShouldRemainUnchanged");
			yield return new(NameOf(FormattedDisplayNameExample.TestX20NameX20WithU0020Spaces), "FormattedDisplayNameExample, Test Name With Spaces");
			yield return new(NameOf(FormattedDisplayNameExample.Parameter_X22RETURN_VALUEX22_is_always_added), "FormattedDisplayNameExample, Parameter \"RETURN VALUE\" is always added");
			yield return new(NameOf(FormattedDisplayNameExample.Parameter_X22RETURN_VALUE_X22_is_always_added), "FormattedDisplayNameExample, Parameter \"RETURN VALUE \" is always added");
			yield return new(NameOf(Given_a_version_number.when_it_equals_1.then_it_should_be_less_than_2), "Given a version number, when it equals 1, then it should be less than 2");
		}
		else
		{
			yield return new(nameof(FormattedDisplayNameExample.unit_tests_are_awesomeX21_U263A), "unit tests are awesome! \u263A");
			yield return new(nameof(FormattedDisplayNameExample.api_version_1_eq_1X2E0), "api version 1 = 1.0");
			yield return new(nameof(FormattedDisplayNameExample.api_version_should_be_greater_than_1), "api version should be greater than 1");
			yield return new(nameof(FormattedDisplayNameExample.api_version_2_should_be_gt_than_1), "api version 2 should be > than 1");
			yield return new(nameof(FormattedDisplayNameExample.api_version_2_should_be_ge_than_1), "api version 2 should be >= than 1");
			yield return new(nameof(FormattedDisplayNameExample.api_version_1_should_be_lt_than_2), "api version 1 should be < than 2");
			yield return new(nameof(FormattedDisplayNameExample.api_version_1_should_be_le_than_2), "api version 1 should be <= than 2");
			yield return new(nameof(FormattedDisplayNameExample.api_version_1X2E0_should_ne_2U002E0), "api version 1.0 should != 2.0");
			yield return new(nameof(FormattedDisplayNameExample.lt_0_should_be_an_error), "< 0 should be an error");
			yield return new(nameof(FormattedDisplayNameExample.equals_operator_overload_should_be_same_as_eq_eq), "equals operator overload should be same as = =");
			yield return new(nameof(FormattedDisplayNameExample.X3DX3D_operator_overload_should_be_same_as_equals_method), "== operator overload should be same as equals method");
			yield return new(nameof(FormattedDisplayNameExample.masculine_super_heroes_should_be_buffed), "masculine super heroes should be buffed");
			yield return new(nameof(FormattedDisplayNameExample.termination_date_should_be_updated_when_employee_is_axed), "termination date should be updated when employee is axed");
			yield return new(nameof(FormattedDisplayNameExample.total_amount_should_be_updated_when_order_is_taxed), "total amount should be updated when order is taxed");
			yield return new(nameof(FormattedDisplayNameExample.X27stuffedX27_should_not_be_ambiguous_with_X27stUFFEDX27), "'stuffed' should not be ambiguous with 'st￭'");
			yield return new(nameof(FormattedDisplayNameExample.X27maxed_outX27_should_not_be_ambiguous_with_X27maXED_outX27), "'maxed out' should not be ambiguous with 'maí out'");
			yield return new(nameof(FormattedDisplayNameExample.TestNameShouldRemainUnchanged), "TestNameShouldRemainUnchanged");
			yield return new(nameof(FormattedDisplayNameExample.TestX20NameX20WithU0020Spaces), "Test Name With Spaces");
			yield return new(nameof(FormattedDisplayNameExample.Parameter_X22RETURN_VALUEX22_is_always_added), "Parameter \"RETURN VALUE\" is always added");
			yield return new(nameof(FormattedDisplayNameExample.Parameter_X22RETURN_VALUE_X22_is_always_added), "Parameter \"RETURN VALUE \" is always added");
			yield return new(nameof(Given_a_version_number.when_it_equals_1.then_it_should_be_less_than_2), "then it should be less than 2");
		}
	}

	private static IEnumerable<TheoryDataRow<string, string>> ReplaceUnderscoreOnlyDisplayData(TestMethodDisplay methodDisplay)
	{
		if (methodDisplay == TestMethodDisplay.ClassAndMethod)
		{
			yield return new(NameOf(FormattedDisplayNameExample.unit_tests_are_awesomeX21_U263A), "FormattedDisplayNameExample.unit tests are awesomeX21 U263A");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_1_eq_1X2E0), "FormattedDisplayNameExample.api version 1 eq 1X2E0");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_should_be_greater_than_1), "FormattedDisplayNameExample.api version should be greater than 1");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_2_should_be_gt_than_1), "FormattedDisplayNameExample.api version 2 should be gt than 1");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_2_should_be_ge_than_1), "FormattedDisplayNameExample.api version 2 should be ge than 1");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_1_should_be_lt_than_2), "FormattedDisplayNameExample.api version 1 should be lt than 2");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_1_should_be_le_than_2), "FormattedDisplayNameExample.api version 1 should be le than 2");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_1X2E0_should_ne_2U002E0), "FormattedDisplayNameExample.api version 1X2E0 should ne 2U002E0");
			yield return new(NameOf(FormattedDisplayNameExample.lt_0_should_be_an_error), "FormattedDisplayNameExample.lt 0 should be an error");
			yield return new(NameOf(FormattedDisplayNameExample.equals_operator_overload_should_be_same_as_eq_eq), "FormattedDisplayNameExample.equals operator overload should be same as eq eq");
			yield return new(NameOf(FormattedDisplayNameExample.X3DX3D_operator_overload_should_be_same_as_equals_method), "FormattedDisplayNameExample.X3DX3D operator overload should be same as equals method");
			yield return new(NameOf(FormattedDisplayNameExample.masculine_super_heroes_should_be_buffed), "FormattedDisplayNameExample.masculine super heroes should be buffed");
			yield return new(NameOf(FormattedDisplayNameExample.termination_date_should_be_updated_when_employee_is_axed), "FormattedDisplayNameExample.termination date should be updated when employee is axed");
			yield return new(NameOf(FormattedDisplayNameExample.total_amount_should_be_updated_when_order_is_taxed), "FormattedDisplayNameExample.total amount should be updated when order is taxed");
			yield return new(NameOf(FormattedDisplayNameExample.X27stuffedX27_should_not_be_ambiguous_with_X27stUFFEDX27), "FormattedDisplayNameExample.X27stuffedX27 should not be ambiguous with X27stUFFEDX27");
			yield return new(NameOf(FormattedDisplayNameExample.X27maxed_outX27_should_not_be_ambiguous_with_X27maXED_outX27), "FormattedDisplayNameExample.X27maxed outX27 should not be ambiguous with X27maXED outX27");
			yield return new(NameOf(FormattedDisplayNameExample.TestNameShouldRemainUnchanged), "FormattedDisplayNameExample.TestNameShouldRemainUnchanged");
			yield return new(NameOf(FormattedDisplayNameExample.TestX20NameX20WithU0020Spaces), "FormattedDisplayNameExample.TestX20NameX20WithU0020Spaces");
			yield return new(NameOf(FormattedDisplayNameExample.Parameter_X22RETURN_VALUEX22_is_always_added), "FormattedDisplayNameExample.Parameter X22RETURN VALUEX22 is always added");
			yield return new(NameOf(FormattedDisplayNameExample.Parameter_X22RETURN_VALUE_X22_is_always_added), "FormattedDisplayNameExample.Parameter X22RETURN VALUE X22 is always added");
			yield return new(NameOf(Given_a_version_number.when_it_equals_1.then_it_should_be_less_than_2), "Given a version number.when it equals 1.then it should be less than 2");
		}
		else
		{
			yield return new(nameof(FormattedDisplayNameExample.unit_tests_are_awesomeX21_U263A), "unit tests are awesomeX21 U263A");
			yield return new(nameof(FormattedDisplayNameExample.api_version_1_eq_1X2E0), "api version 1 eq 1X2E0");
			yield return new(nameof(FormattedDisplayNameExample.api_version_should_be_greater_than_1), "api version should be greater than 1");
			yield return new(nameof(FormattedDisplayNameExample.api_version_2_should_be_gt_than_1), "api version 2 should be gt than 1");
			yield return new(nameof(FormattedDisplayNameExample.api_version_2_should_be_ge_than_1), "api version 2 should be ge than 1");
			yield return new(nameof(FormattedDisplayNameExample.api_version_1_should_be_lt_than_2), "api version 1 should be lt than 2");
			yield return new(nameof(FormattedDisplayNameExample.api_version_1_should_be_le_than_2), "api version 1 should be le than 2");
			yield return new(nameof(FormattedDisplayNameExample.api_version_1X2E0_should_ne_2U002E0), "api version 1X2E0 should ne 2U002E0");
			yield return new(nameof(FormattedDisplayNameExample.lt_0_should_be_an_error), "lt 0 should be an error");
			yield return new(nameof(FormattedDisplayNameExample.equals_operator_overload_should_be_same_as_eq_eq), "equals operator overload should be same as eq eq");
			yield return new(nameof(FormattedDisplayNameExample.X3DX3D_operator_overload_should_be_same_as_equals_method), "X3DX3D operator overload should be same as equals method");
			yield return new(nameof(FormattedDisplayNameExample.masculine_super_heroes_should_be_buffed), "masculine super heroes should be buffed");
			yield return new(nameof(FormattedDisplayNameExample.termination_date_should_be_updated_when_employee_is_axed), "termination date should be updated when employee is axed");
			yield return new(nameof(FormattedDisplayNameExample.total_amount_should_be_updated_when_order_is_taxed), "total amount should be updated when order is taxed");
			yield return new(nameof(FormattedDisplayNameExample.X27stuffedX27_should_not_be_ambiguous_with_X27stUFFEDX27), "X27stuffedX27 should not be ambiguous with X27stUFFEDX27");
			yield return new(nameof(FormattedDisplayNameExample.X27maxed_outX27_should_not_be_ambiguous_with_X27maXED_outX27), "X27maxed outX27 should not be ambiguous with X27maXED outX27");
			yield return new(nameof(FormattedDisplayNameExample.TestNameShouldRemainUnchanged), "TestNameShouldRemainUnchanged");
			yield return new(nameof(FormattedDisplayNameExample.TestX20NameX20WithU0020Spaces), "TestX20NameX20WithU0020Spaces");
			yield return new(nameof(FormattedDisplayNameExample.Parameter_X22RETURN_VALUEX22_is_always_added), "Parameter X22RETURN VALUEX22 is always added");
			yield return new(nameof(FormattedDisplayNameExample.Parameter_X22RETURN_VALUE_X22_is_always_added), "Parameter X22RETURN VALUE X22 is always added");
			yield return new(nameof(Given_a_version_number.when_it_equals_1.then_it_should_be_less_than_2), "then it should be less than 2");
		}
	}

	private static IEnumerable<TheoryDataRow<string, string>> ReplaceUnderscoreAndOperatorDisplayData(TestMethodDisplay methodDisplay)
	{
		if (methodDisplay == TestMethodDisplay.ClassAndMethod)
		{
			yield return new(NameOf(FormattedDisplayNameExample.unit_tests_are_awesomeX21_U263A), "FormattedDisplayNameExample.unit tests are awesomeX21 U263A");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_1_eq_1X2E0), "FormattedDisplayNameExample.api version 1 = 1X2E0");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_should_be_greater_than_1), "FormattedDisplayNameExample.api version should be greater than 1");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_2_should_be_gt_than_1), "FormattedDisplayNameExample.api version 2 should be > than 1");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_2_should_be_ge_than_1), "FormattedDisplayNameExample.api version 2 should be >= than 1");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_1_should_be_lt_than_2), "FormattedDisplayNameExample.api version 1 should be < than 2");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_1_should_be_le_than_2), "FormattedDisplayNameExample.api version 1 should be <= than 2");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_1X2E0_should_ne_2U002E0), "FormattedDisplayNameExample.api version 1X2E0 should != 2U002E0");
			yield return new(NameOf(FormattedDisplayNameExample.lt_0_should_be_an_error), "FormattedDisplayNameExample.< 0 should be an error");
			yield return new(NameOf(FormattedDisplayNameExample.equals_operator_overload_should_be_same_as_eq_eq), "FormattedDisplayNameExample.equals operator overload should be same as = =");
			yield return new(NameOf(FormattedDisplayNameExample.X3DX3D_operator_overload_should_be_same_as_equals_method), "FormattedDisplayNameExample.X3DX3D operator overload should be same as equals method");
			yield return new(NameOf(FormattedDisplayNameExample.masculine_super_heroes_should_be_buffed), "FormattedDisplayNameExample.masculine super heroes should be buffed");
			yield return new(NameOf(FormattedDisplayNameExample.termination_date_should_be_updated_when_employee_is_axed), "FormattedDisplayNameExample.termination date should be updated when employee is axed");
			yield return new(NameOf(FormattedDisplayNameExample.total_amount_should_be_updated_when_order_is_taxed), "FormattedDisplayNameExample.total amount should be updated when order is taxed");
			yield return new(NameOf(FormattedDisplayNameExample.X27stuffedX27_should_not_be_ambiguous_with_X27stUFFEDX27), "FormattedDisplayNameExample.X27stuffedX27 should not be ambiguous with X27stUFFEDX27");
			yield return new(NameOf(FormattedDisplayNameExample.X27maxed_outX27_should_not_be_ambiguous_with_X27maXED_outX27), "FormattedDisplayNameExample.X27maxed outX27 should not be ambiguous with X27maXED outX27");
			yield return new(NameOf(FormattedDisplayNameExample.TestNameShouldRemainUnchanged), "FormattedDisplayNameExample.TestNameShouldRemainUnchanged");
			yield return new(NameOf(FormattedDisplayNameExample.TestX20NameX20WithU0020Spaces), "FormattedDisplayNameExample.TestX20NameX20WithU0020Spaces");
			yield return new(NameOf(FormattedDisplayNameExample.Parameter_X22RETURN_VALUEX22_is_always_added), "FormattedDisplayNameExample.Parameter X22RETURN VALUEX22 is always added");
			yield return new(NameOf(FormattedDisplayNameExample.Parameter_X22RETURN_VALUE_X22_is_always_added), "FormattedDisplayNameExample.Parameter X22RETURN VALUE X22 is always added");
			yield return new(NameOf(Given_a_version_number.when_it_equals_1.then_it_should_be_less_than_2), "Given a version number.when it equals 1.then it should be less than 2");
		}
		else
		{
			yield return new(nameof(FormattedDisplayNameExample.unit_tests_are_awesomeX21_U263A), "unit tests are awesomeX21 U263A");
			yield return new(nameof(FormattedDisplayNameExample.api_version_1_eq_1X2E0), "api version 1 = 1X2E0");
			yield return new(nameof(FormattedDisplayNameExample.api_version_should_be_greater_than_1), "api version should be greater than 1");
			yield return new(nameof(FormattedDisplayNameExample.api_version_2_should_be_gt_than_1), "api version 2 should be > than 1");
			yield return new(nameof(FormattedDisplayNameExample.api_version_2_should_be_ge_than_1), "api version 2 should be >= than 1");
			yield return new(nameof(FormattedDisplayNameExample.api_version_1_should_be_lt_than_2), "api version 1 should be < than 2");
			yield return new(nameof(FormattedDisplayNameExample.api_version_1_should_be_le_than_2), "api version 1 should be <= than 2");
			yield return new(nameof(FormattedDisplayNameExample.api_version_1X2E0_should_ne_2U002E0), "api version 1X2E0 should != 2U002E0");
			yield return new(nameof(FormattedDisplayNameExample.lt_0_should_be_an_error), "< 0 should be an error");
			yield return new(nameof(FormattedDisplayNameExample.equals_operator_overload_should_be_same_as_eq_eq), "equals operator overload should be same as = =");
			yield return new(nameof(FormattedDisplayNameExample.X3DX3D_operator_overload_should_be_same_as_equals_method), "X3DX3D operator overload should be same as equals method");
			yield return new(nameof(FormattedDisplayNameExample.masculine_super_heroes_should_be_buffed), "masculine super heroes should be buffed");
			yield return new(nameof(FormattedDisplayNameExample.termination_date_should_be_updated_when_employee_is_axed), "termination date should be updated when employee is axed");
			yield return new(nameof(FormattedDisplayNameExample.total_amount_should_be_updated_when_order_is_taxed), "total amount should be updated when order is taxed");
			yield return new(nameof(FormattedDisplayNameExample.X27stuffedX27_should_not_be_ambiguous_with_X27stUFFEDX27), "X27stuffedX27 should not be ambiguous with X27stUFFEDX27");
			yield return new(nameof(FormattedDisplayNameExample.X27maxed_outX27_should_not_be_ambiguous_with_X27maXED_outX27), "X27maxed outX27 should not be ambiguous with X27maXED outX27");
			yield return new(nameof(FormattedDisplayNameExample.TestNameShouldRemainUnchanged), "TestNameShouldRemainUnchanged");
			yield return new(nameof(FormattedDisplayNameExample.TestX20NameX20WithU0020Spaces), "TestX20NameX20WithU0020Spaces");
			yield return new(nameof(FormattedDisplayNameExample.Parameter_X22RETURN_VALUEX22_is_always_added), "Parameter X22RETURN VALUEX22 is always added");
			yield return new(nameof(FormattedDisplayNameExample.Parameter_X22RETURN_VALUE_X22_is_always_added), "Parameter X22RETURN VALUE X22 is always added");
			yield return new(nameof(Given_a_version_number.when_it_equals_1.then_it_should_be_less_than_2), "then it should be less than 2");
		}
	}

	private static IEnumerable<TheoryDataRow<string, string>> ReplaceUnderscoreAndEscapeSequenceDisplayData(TestMethodDisplay methodDisplay)
	{
		if (methodDisplay == TestMethodDisplay.ClassAndMethod)
		{
			yield return new(NameOf(FormattedDisplayNameExample.unit_tests_are_awesomeX21_U263A), "FormattedDisplayNameExample.unit tests are awesome! \u263a");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_1_eq_1X2E0), "FormattedDisplayNameExample.api version 1 eq 1.0");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_should_be_greater_than_1), "FormattedDisplayNameExample.api version should be greater than 1");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_2_should_be_gt_than_1), "FormattedDisplayNameExample.api version 2 should be gt than 1");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_2_should_be_ge_than_1), "FormattedDisplayNameExample.api version 2 should be ge than 1");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_1_should_be_lt_than_2), "FormattedDisplayNameExample.api version 1 should be lt than 2");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_1_should_be_le_than_2), "FormattedDisplayNameExample.api version 1 should be le than 2");
			yield return new(NameOf(FormattedDisplayNameExample.api_version_1X2E0_should_ne_2U002E0), "FormattedDisplayNameExample.api version 1.0 should ne 2.0");
			yield return new(NameOf(FormattedDisplayNameExample.lt_0_should_be_an_error), "FormattedDisplayNameExample.lt 0 should be an error");
			yield return new(NameOf(FormattedDisplayNameExample.equals_operator_overload_should_be_same_as_eq_eq), "FormattedDisplayNameExample.equals operator overload should be same as eq eq");
			yield return new(NameOf(FormattedDisplayNameExample.X3DX3D_operator_overload_should_be_same_as_equals_method), "FormattedDisplayNameExample.== operator overload should be same as equals method");
			yield return new(NameOf(FormattedDisplayNameExample.masculine_super_heroes_should_be_buffed), "FormattedDisplayNameExample.masculine super heroes should be buffed");
			yield return new(NameOf(FormattedDisplayNameExample.termination_date_should_be_updated_when_employee_is_axed), "FormattedDisplayNameExample.termination date should be updated when employee is axed");
			yield return new(NameOf(FormattedDisplayNameExample.total_amount_should_be_updated_when_order_is_taxed), "FormattedDisplayNameExample.total amount should be updated when order is taxed");
			yield return new(NameOf(FormattedDisplayNameExample.X27stuffedX27_should_not_be_ambiguous_with_X27stUFFEDX27), "FormattedDisplayNameExample.'stuffed' should not be ambiguous with 'st￭'");
			yield return new(NameOf(FormattedDisplayNameExample.X27maxed_outX27_should_not_be_ambiguous_with_X27maXED_outX27), "FormattedDisplayNameExample.'maxed out' should not be ambiguous with 'maí out'");
			yield return new(NameOf(FormattedDisplayNameExample.TestNameShouldRemainUnchanged), "FormattedDisplayNameExample.TestNameShouldRemainUnchanged");
			yield return new(NameOf(FormattedDisplayNameExample.TestX20NameX20WithU0020Spaces), "FormattedDisplayNameExample.Test Name With Spaces");
			yield return new(NameOf(FormattedDisplayNameExample.Parameter_X22RETURN_VALUEX22_is_always_added), "FormattedDisplayNameExample.Parameter \"RETURN VALUE\" is always added");
			yield return new(NameOf(FormattedDisplayNameExample.Parameter_X22RETURN_VALUE_X22_is_always_added), "FormattedDisplayNameExample.Parameter \"RETURN VALUE \" is always added");
			yield return new(NameOf(Given_a_version_number.when_it_equals_1.then_it_should_be_less_than_2), "Given a version number.when it equals 1.then it should be less than 2");
		}
		else
		{
			yield return new(nameof(FormattedDisplayNameExample.unit_tests_are_awesomeX21_U263A), "unit tests are awesome! \u263a");
			yield return new(nameof(FormattedDisplayNameExample.api_version_1_eq_1X2E0), "api version 1 eq 1.0");
			yield return new(nameof(FormattedDisplayNameExample.api_version_should_be_greater_than_1), "api version should be greater than 1");
			yield return new(nameof(FormattedDisplayNameExample.api_version_2_should_be_gt_than_1), "api version 2 should be gt than 1");
			yield return new(nameof(FormattedDisplayNameExample.api_version_2_should_be_ge_than_1), "api version 2 should be ge than 1");
			yield return new(nameof(FormattedDisplayNameExample.api_version_1_should_be_lt_than_2), "api version 1 should be lt than 2");
			yield return new(nameof(FormattedDisplayNameExample.api_version_1_should_be_le_than_2), "api version 1 should be le than 2");
			yield return new(nameof(FormattedDisplayNameExample.api_version_1X2E0_should_ne_2U002E0), "api version 1.0 should ne 2.0");
			yield return new(nameof(FormattedDisplayNameExample.lt_0_should_be_an_error), "lt 0 should be an error");
			yield return new(nameof(FormattedDisplayNameExample.equals_operator_overload_should_be_same_as_eq_eq), "equals operator overload should be same as eq eq");
			yield return new(nameof(FormattedDisplayNameExample.X3DX3D_operator_overload_should_be_same_as_equals_method), "== operator overload should be same as equals method");
			yield return new(nameof(FormattedDisplayNameExample.masculine_super_heroes_should_be_buffed), "masculine super heroes should be buffed");
			yield return new(nameof(FormattedDisplayNameExample.termination_date_should_be_updated_when_employee_is_axed), "termination date should be updated when employee is axed");
			yield return new(nameof(FormattedDisplayNameExample.total_amount_should_be_updated_when_order_is_taxed), "total amount should be updated when order is taxed");
			yield return new(nameof(FormattedDisplayNameExample.X27stuffedX27_should_not_be_ambiguous_with_X27stUFFEDX27), "'stuffed' should not be ambiguous with 'st￭'");
			yield return new(nameof(FormattedDisplayNameExample.X27maxed_outX27_should_not_be_ambiguous_with_X27maXED_outX27), "'maxed out' should not be ambiguous with 'maí out'");
			yield return new(nameof(FormattedDisplayNameExample.TestNameShouldRemainUnchanged), "TestNameShouldRemainUnchanged");
			yield return new(nameof(FormattedDisplayNameExample.TestX20NameX20WithU0020Spaces), "Test Name With Spaces");
			yield return new(nameof(FormattedDisplayNameExample.Parameter_X22RETURN_VALUEX22_is_always_added), "Parameter \"RETURN VALUE\" is always added");
			yield return new(nameof(FormattedDisplayNameExample.Parameter_X22RETURN_VALUE_X22_is_always_added), "Parameter \"RETURN VALUE \" is always added");
			yield return new(nameof(Given_a_version_number.when_it_equals_1.then_it_should_be_less_than_2), "then it should be less than 2");
		}
	}
}

public static class FormattedDisplayNameExample
{
	public static void unit_tests_are_awesomeX21_U263A() { }
	public static void api_version_1_eq_1X2E0() { }
	public static void api_version_should_be_greater_than_1() { }
	public static void api_version_2_should_be_gt_than_1() { }
	public static void api_version_2_should_be_ge_than_1() { }
	public static void api_version_1_should_be_lt_than_2() { }
	public static void api_version_1_should_be_le_than_2() { }
	public static void api_version_1X2E0_should_ne_2U002E0() { }
	public static void lt_0_should_be_an_error() { }
	public static void equals_operator_overload_should_be_same_as_eq_eq() { }
	public static void X3DX3D_operator_overload_should_be_same_as_equals_method() { }
	public static void masculine_super_heroes_should_be_buffed() { }
	public static void termination_date_should_be_updated_when_employee_is_axed() { }
	public static void total_amount_should_be_updated_when_order_is_taxed() { }
	public static void X27stuffedX27_should_not_be_ambiguous_with_X27stUFFEDX27() { }
	public static void X27maxed_outX27_should_not_be_ambiguous_with_X27maXED_outX27() { }
	public static void TestNameShouldRemainUnchanged() { }
	public static void TestX20NameX20WithU0020Spaces() { }
	public static void Parameter_X22RETURN_VALUEX22_is_always_added() { }
	public static void Parameter_X22RETURN_VALUE_X22_is_always_added() { }
}

namespace Given_a_version_number
{
	public static class when_it_equals_1
	{
		public static void then_it_should_be_less_than_2() { }
	}
}
