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
    }
}
