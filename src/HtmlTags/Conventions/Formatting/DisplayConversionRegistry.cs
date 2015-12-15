namespace HtmlTags.Conventions.Formatting
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class DisplayConversionRegistry
    {
        private readonly IList<StringifierStrategy> _strategies = new List<StringifierStrategy>();


        public Stringifier BuildStringifier()
        {
            var stringifier = new Stringifier();
            Configure(stringifier);
            return stringifier;
        }

        public void Configure(Stringifier stringifier) => _strategies.Each(stringifier.AddStrategy);


        private MakeDisplayExpression MakeDisplay(Func<GetStringRequest, bool> filter)
        {
            return new MakeDisplayExpression(func =>
            {
                _strategies.Add(new StringifierStrategy
                {
                    Matches = filter,
                    StringFunction = func
                });
            });
        }

        private MakeDisplayExpression<T> MakeDisplay<T>(Func<GetStringRequest, bool> filter)
        {
            return new MakeDisplayExpression<T>(func =>
            {
                _strategies.Add(new StringifierStrategy
                {
                    Matches = filter,
                    StringFunction = func
                });
            });
        }

        public MakeDisplayExpression IfTypeMatches(Func<Type, bool> filter) => MakeDisplay(request => filter(request.PropertyType));

        public MakeDisplayExpression<T> IfIsType<T>() => MakeDisplay<T>(request => request.PropertyType == typeof (T));

        public MakeDisplayExpression<T> IfCanBeCastToType<T>() => MakeDisplay<T>(t => t.PropertyType.CanBeCastTo<T>());

        public MakeDisplayExpression IfPropertyMatches(Func<PropertyInfo, bool> matches) => MakeDisplay(request => request.Property != null && matches(request.Property));

        public MakeDisplayExpression<T> IfPropertyMatches<T>(Func<PropertyInfo, bool> matches)
        {
            return
                MakeDisplay<T>(
                    request =>
                        request.Property != null && request.PropertyType == typeof (T) && matches(request.Property));
        }

        #region Nested type: MakeDisplayExpression

        public class MakeDisplayExpression : MakeDisplayExpressionBase
        {
            public MakeDisplayExpression(Action<Func<GetStringRequest, string>> callback)
                : base(callback)
            {
            }

            public void ConvertBy(Func<GetStringRequest, string> display)
            {
                _callback(display);
            }

            public void ConvertWith<TService>(Func<TService, GetStringRequest, string> display)
            {
                apply(o => display(o.Get<TService>(), o));
            }
        }

        public class MakeDisplayExpression<T> : MakeDisplayExpressionBase
        {
            public MakeDisplayExpression(Action<Func<GetStringRequest, string>> callback)
                : base(callback)
            {
            }

            public void ConvertBy(Func<T, string> display)
            {
                apply(o => display((T) o.RawValue));
            }

            public void ConvertBy(Func<GetStringRequest, T, string> display)
            {
                apply(o => display(o, (T) o.RawValue));
            }

            public void ConvertWith<TService>(Func<TService, T, string> display)
            {
                apply(o => display(o.Get<TService>(), (T) o.RawValue));
            }
        }

        #endregion

        #region Nested type: MakeDisplayExpressionBase

        public abstract class MakeDisplayExpressionBase
        {
            protected Action<Func<GetStringRequest, string>> _callback;

            public MakeDisplayExpressionBase(Action<Func<GetStringRequest, string>> callback)
            {
                _callback = callback;
            }

            protected void apply(Func<GetStringRequest, string> func)
            {
                _callback(func);
            }
        }

        #endregion
    }
}