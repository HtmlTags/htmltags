using FubuTestingSupport;
using NUnit.Framework;

namespace HtmlTags.Testing
{
    public class CssClassNameValidatorTester
    {
        [Test]
        public void do_not_allow_special_characters_in_class_names()
        {
            CssClassNameValidator.IsValidClassName("$test@@").ShouldBeFalse();
        }

        [Test]
        public void do_not_allow_start_with_number_in_class_names()
        {
            CssClassNameValidator.IsValidClassName("4test").ShouldBeFalse();
        }

        [Test]
        public void do_not_allow_first_double_dashes_in_class_names()
        {
            CssClassNameValidator.IsValidClassName("--test").ShouldBeFalse();
        }

        [Test]
        public void class_name_must_have_at_least_two_chars_if_starts_with_dash()
        {
            CssClassNameValidator.IsValidClassName("-").ShouldBeFalse();
        }

        [Test]
        public void valid_class_names()
        {
            CssClassNameValidator.IsValidClassName("-test").ShouldBeTrue();
            CssClassNameValidator.IsValidClassName("-test").ShouldBeTrue();
            CssClassNameValidator.IsValidClassName("_test").ShouldBeTrue();
            CssClassNameValidator.IsValidClassName("TEST_2-test").ShouldBeTrue();
            CssClassNameValidator.IsValidClassName("-just-4-test").ShouldBeTrue();
        }

        [Test]
        public void do_allow_a_class_that_is_a_json_blob_with_spaces()
        {
            CssClassNameValidator.IsValidClassName("{a:1, a:2}").ShouldBeTrue();
            CssClassNameValidator.IsValidClassName("[1, 2, 3]").ShouldBeTrue();
        }

        [Test]
        public void class_name_with_wrong_json()
        {
            CssClassNameValidator.IsValidClassName("[1,2,3}").ShouldBeFalse();
            CssClassNameValidator.IsValidClassName("{a:1, a:2]").ShouldBeFalse();
        } 

        [Test]
        public void json_objects_must_start_and_end_with_curly_brackets()
        {
            CssClassNameValidator.IsJsonClassName("{a:1, a:2}").ShouldBeTrue();
            CssClassNameValidator.IsJsonClassName("{a:1, a:2]").ShouldBeFalse();
        }

        [Test]
        public void json_arrays_must_start_and_end_with_square_brackets()
        {
            CssClassNameValidator.IsJsonClassName("[a, b, c]").ShouldBeTrue();
            CssClassNameValidator.IsJsonClassName("[a, b, c}").ShouldBeFalse();
        }

        [Test]
        public void do_nothing_if_already_valid_class_name()
        {
            CssClassNameValidator.SanitizeClassName("asdf").ShouldEqual("asdf");
            CssClassNameValidator.SanitizeClassName("-test").ShouldEqual("-test");
            CssClassNameValidator.SanitizeClassName("-_test").ShouldEqual("-_test");
            CssClassNameValidator.SanitizeClassName("TEST_2-test").ShouldEqual("TEST_2-test");
            CssClassNameValidator.SanitizeClassName("-just-4-test").ShouldEqual("-just-4-test");
        }

        [Test]
        public void remove_leading_numbers()
        {
            CssClassNameValidator.SanitizeClassName("9asdf").ShouldEqual("asdf");
        }

        [Test]
        public void should_handle_different_combos_of_leading_invalid_characters()
        {
            CssClassNameValidator.SanitizeClassName("9-9asdf").ShouldEqual("asdf");
            CssClassNameValidator.SanitizeClassName("-99asdf").ShouldEqual("asdf");
            CssClassNameValidator.SanitizeClassName("@-99asdf").ShouldEqual("asdf");
            CssClassNameValidator.SanitizeClassName("@-99@asdf").ShouldEqual("asdf");
        }

        [Test]
        public void remove_leading_hyphen_and_numbers_if_not_followed_by_an_underscore_or_alpha_char()
        {
            CssClassNameValidator.SanitizeClassName("-9asdf").ShouldEqual("asdf");
        }

        [Test]
        public void remove_bogus_characters()
        {
            CssClassNameValidator.SanitizeClassName("a!@#$%^&*()`=':;?><|{}[]~sdf").ShouldEqual("asdf");
        }

        [Test]
        public void return_default_if_null_input()
        {
            CssClassNameValidator.SanitizeClassName(null).ShouldEqual(CssClassNameValidator.DefaultClass);
        }

        [Test]
        public void return_default_if_empty_input()
        {
            CssClassNameValidator.SanitizeClassName("").ShouldEqual(CssClassNameValidator.DefaultClass);
        }

        [Test]
        public void return_default_if_completely_invalid_input()
        {
            CssClassNameValidator.SanitizeClassName("-99").ShouldEqual(CssClassNameValidator.DefaultClass);
        }

    }
}
