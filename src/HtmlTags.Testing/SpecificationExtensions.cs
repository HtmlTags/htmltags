using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace HtmlTags.Testing
{
    public delegate void MethodThatThrows();

    public static class SpecificationExtensions
    {
        //public static void ShouldBeValid(this object target)
        //{
        //    Notification notification = Validator.ValidateObject(target);

        //    bool isValid = notification.IsValid();

        //    if (!isValid)
        //    {
        //        foreach (NotificationMessage message in notification.Flatten().AllMessages)
        //        {
        //            Console.WriteLine("**FAILURE** Notification: {0}", message);
        //        }
        //    }

        //    isValid.ShouldBeTrue();
        //}

        public static void ShouldHave<T>(this IEnumerable<T> values, Func<T, bool> func)
        {
            values.FirstOrDefault(func).ShouldNotBeNull();
        }

        //public static void ShouldNotBeValid(this object target)
        //{
        //    Validator.ValidateObject(target).IsValid().ShouldBeFalse();
        //}

        //public static void FieldShouldBeValid(this object target, string fieldName)
        //{
        //    Validator.ValidateField(target, fieldName).Length.ShouldEqual(0);
        //}

        //public static void FieldShouldNotBeValid(this object target, string fieldName)
        //{
        //    Validator.ValidateField(target, fieldName).Length.ShouldBeGreaterThan(0);
        //}

        public static void ShouldBeFalse(this bool condition)
        {
            Assert.IsFalse(condition);
        }

        public static void ShouldBeTrue(this bool condition)
        {
            Assert.IsTrue(condition);
        }

        public static object ShouldEqual(this object actual, object expected)
        {
            Assert.AreEqual(expected, actual);
            return expected;
        }


        public static object ShouldEqual(this string actual, object expected)
        {
            Assert.AreEqual(expected.ToString(), actual);
            return expected;
        }

        public static XmlElement AttributeShouldEqual(this XmlElement element, string attributeName, object expected)
        {
            Assert.IsNotNull(element, "The Element is null");

            string actual = element.GetAttribute(attributeName);
            Assert.AreEqual(expected, actual);
            return element;
        }

        public static XmlElement AttributeShouldEqual(this XmlNode node, string attributeName, object expected)
        {
            var element = node as XmlElement;

            Assert.IsNotNull(element, "The Element is null");

            string actual = element.GetAttribute(attributeName);
            Assert.AreEqual(expected, actual);
            return element;
        }

        public static XmlElement Should(this XmlNode node, Action<XmlElement> action)
        {
            var element = (XmlElement) node;
            action(element);

            return element;
        }

        public static T Should<T>(this T target, Action<T> action)
        {
            action(target);
            return target;
        }

        public static XmlElement CountOfChildNodesShouldBe(this XmlElement element, int count)
        {
            element.ChildNodes.Count.ShouldEqual(count);
            return element;
        }

        public static XmlElement ShouldHaveAttributes(this XmlNode node, string text)
        {
            var element = (XmlElement) node;

            string[] atts = text.Split(',');
            foreach (string attText in atts)
            {
                string[] parts = attText.Split(':');
                element.AttributeShouldEqual(parts[0].Trim(), parts[1].Trim());
            }

            return element;
        }

        public static XmlElement ShouldHaveName(this XmlNode node, string name)
        {
            var element = (XmlElement) node;

            element.Name.ShouldEqual(name);

            return element;
        }

        public static XmlElement ShouldHaveChild(this XmlElement element, string xpath)
        {
            var child = element.SelectSingleNode(xpath) as XmlElement;
            Assert.IsNotNull(child, "Should have a child element matching " + xpath);

            return child;
        }

        public static XmlElement DoesNotHaveAttribute(this XmlElement element, string attributeName)
        {
            Assert.IsNotNull(element, "The Element is null");
            Assert.IsFalse(element.HasAttribute(attributeName),
                           "Element should not have an attribute named " + attributeName);

            return element;
        }

        public static object ShouldNotEqual(this object actual, object expected)
        {
            Assert.AreNotEqual(expected, actual);
            return expected;
        }

        public static void ShouldBeNull(this object anObject)
        {
            Assert.IsNull(anObject);
        }

        public static void ShouldNotBeNull(this object anObject)
        {
            Assert.IsNotNull(anObject);
        }

        public static object ShouldBeTheSameAs(this object actual, object expected)
        {
            Assert.AreSame(expected, actual);
            return expected;
        }

        public static object ShouldNotBeTheSameAs(this object actual, object expected)
        {
            Assert.AreNotSame(expected, actual);
            return expected;
        }

        public static T ShouldBeOfType<T>(this object actual)
        {
            actual.ShouldNotBeNull();
            actual.ShouldBeOfType(typeof (T));
            return (T) actual;
        }

        public static T AssertThat<T>(this T actual, Action<T> asserts)
        {
            asserts(actual);
            return actual;
        }

        public static T As<T>(this object actual)
        {
            actual.ShouldNotBeNull();
            actual.ShouldBeOfType(typeof (T));
            return (T) actual;
        }

        public static object ShouldBeOfType(this object actual, Type expected)
        {
            Assert.IsInstanceOfType(expected, actual);
            return actual;
        }

        public static void ShouldNotBeOfType(this object actual, Type expected)
        {
            Assert.IsNotInstanceOfType(expected, actual);
        }

        public static void ShouldContain(this IList actual, object expected)
        {
            Assert.Contains(expected, actual);
        }

        public static void ShouldContain<T>(this IEnumerable<T> actual, T expected)
        {
            if (actual.Count(t => t.Equals(expected)) == 0)
            {
                Assert.Fail("The item was not found in the sequence.");
            }
        }

        public static void ShouldNotBeEmpty<T>(this IEnumerable<T> actual)
        {
            Assert.Greater(actual.Count(), 0, "The list should have at least one element");
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> actual, T expected)
        {
            if (actual.Count(t => t.Equals(expected)) > 0)
            {
                Assert.Fail("The item was found in the sequence it should not be in.");
            }
        }

        public static void ShouldHaveTheSameElementsAs(this IList actual, IList expected)
        {
            actual.ShouldNotBeNull();
            expected.ShouldNotBeNull();

            actual.Count.ShouldEqual(expected.Count);

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i].ShouldEqual(expected[i]);
            }
        }

        public static void ShouldHaveTheSameElementsAs<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            IList actualList = (actual is IList) ? (IList) actual : actual.ToList();
            IList expectedList = (expected is IList) ? (IList) expected : expected.ToList();

            ShouldHaveTheSameElementsAs(actualList, expectedList);
        }

        public static void ShouldHaveTheSameElementKeysAs<ELEMENT, KEY>(this IEnumerable<ELEMENT> actual,
                                                                        IEnumerable expected,
                                                                        Func<ELEMENT, KEY> keySelector)
        {
            actual.ShouldNotBeNull();
            expected.ShouldNotBeNull();

            ELEMENT[] actualArray = actual.ToArray();
            object[] expectedArray = expected.Cast<object>().ToArray();

            actualArray.Length.ShouldEqual(expectedArray.Length);

            for (int i = 0; i < actual.Count(); i++)
            {
                keySelector(actualArray[i]).ShouldEqual(expectedArray[i]);
            }
        }

        public static IComparable ShouldBeGreaterThan(this IComparable arg1, IComparable arg2)
        {
            Assert.Greater(arg1, arg2);
            return arg2;
        }

        public static IComparable ShouldBeLessThan(this IComparable arg1, IComparable arg2)
        {
            Assert.Less(arg1, arg2);
            return arg2;
        }

        public static void ShouldBeEmpty(this ICollection collection)
        {
            Assert.IsEmpty(collection);
        }

        public static void ShouldBeEmpty(this string aString)
        {
            Assert.IsEmpty(aString);
        }

        public static void ShouldNotBeEmpty(this ICollection collection)
        {
            Assert.IsNotEmpty(collection);
        }

        public static void ShouldNotBeEmpty(this string aString)
        {
            Assert.IsNotEmpty(aString);
        }

        public static void ShouldContain(this string actual, string expected)
        {
            StringAssert.Contains(expected, actual);
        }

        public static void ShouldNotContain(this string actual, string expected)
        {
            Assert.That(actual, new NotConstraint(new SubstringConstraint(expected)));
        }

        public static string ShouldBeEqualIgnoringCase(this string actual, string expected)
        {
            StringAssert.AreEqualIgnoringCase(expected, actual);
            return expected;
        }

        public static void ShouldEndWith(this string actual, string expected)
        {
            StringAssert.EndsWith(expected, actual);
        }

        public static void ShouldStartWith(this string actual, string expected)
        {
            StringAssert.StartsWith(expected, actual);
        }

        public static void ShouldContainErrorMessage(this Exception exception, string expected)
        {
            StringAssert.Contains(expected, exception.Message);
        }

        public static Exception ShouldBeThrownBy(this Type exceptionType, MethodThatThrows method)
        {
            Exception exception = null;

            try
            {
                method();
            }
            catch (Exception e)
            {
                Assert.AreEqual(exceptionType, e.GetType());
                exception = e;
            }

            if (exception == null)
            {
                Assert.Fail(String.Format("Expected {0} to be thrown.", exceptionType.FullName));
            }

            return exception;
        }

        public static void ShouldEqualSqlDate(this DateTime actual, DateTime expected)
        {
            TimeSpan timeSpan = actual - expected;
            Assert.Less(Math.Abs(timeSpan.TotalMilliseconds), 3);
        }

        public static IEnumerable<T> ShouldHaveCount<T>(this IEnumerable<T> actual, int expected)
        {
            actual.Count().ShouldEqual(expected);
            return actual;
        }


        //    public static CapturingConstraint CaptureArgumentsFor<MOCK>(this MOCK mock,
        //                                                                Expression<Action<MOCK>> methodExpression)
        //    {
        //        MethodInfo method = Core.Util.ReflectionHelper.GetMethod(methodExpression);

        //        var constraint = new CapturingConstraint();
        //        var constraints = new List<AbstractConstraint>();

        //        foreach (ParameterInfo arg in method.GetParameters())
        //        {
        //            constraints.Add(constraint);
        //        }

        //        mock.Expect(methodExpression.Compile()).Constraints(constraints.ToArray()).Repeat.Any();

        //        return constraint;
        //    }

        //    #region Nested type: CapturingConstraint

        //    public class CapturingConstraint : AbstractConstraint
        //    {
        //        private readonly ArrayList argList = new ArrayList();

        //        public override string Message
        //        {
        //            get { return ""; }
        //        }

        //        public override bool Eval(object obj)
        //        {
        //            argList.Add(obj);
        //            return true;
        //        }

        //        public T First<T>()
        //        {
        //            return ArgumentAt<T>(0);
        //        }

        //        public T ArgumentAt<T>(int pos)
        //        {
        //            return (T)argList[pos];
        //        }

        //        public T Second<T>()
        //        {
        //            return ArgumentAt<T>(1);
        //        }
        //    }

        //    #endregion
        //}
    }
}