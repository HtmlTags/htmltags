using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HtmlTags.Reflection
{
    internal static class ReflectionHelper
    {
        public static bool MeetsSpecialGenericConstraints(Type genericArgType, Type proposedSpecificType)
        {
            GenericParameterAttributes gpa = genericArgType.GetTypeInfo().GenericParameterAttributes;
            GenericParameterAttributes constraints = gpa & GenericParameterAttributes.SpecialConstraintMask;

            // No constraints, away we go!
            if (constraints == GenericParameterAttributes.None)
                return true;

            // "class" constraint and this is a value type
            if ((constraints & GenericParameterAttributes.ReferenceTypeConstraint) != 0
                && proposedSpecificType.GetTypeInfo().IsValueType)
            {
                return false;
            }

            // "struct" constraint and this is not a value type
            if ((constraints & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0
                && ! proposedSpecificType.GetTypeInfo().IsValueType)
            {
                return false;
            }

            // "new()" constraint and this type has no default constructor
            if ((constraints & GenericParameterAttributes.DefaultConstructorConstraint) != 0
                && proposedSpecificType.GetConstructor(Type.EmptyTypes) == null)
            {
                return false;
            }

            return true;
        }

        public static PropertyInfo GetProperty<TModel>(Expression<Func<TModel, object>> expression)
        {
            MemberExpression memberExpression = GetMemberExpression(expression);
            return (PropertyInfo) memberExpression.Member;
        }

        public static PropertyInfo GetProperty<TModel, T>(Expression<Func<TModel, T>> expression)
        {
            MemberExpression memberExpression = GetMemberExpression(expression);
            return (PropertyInfo) memberExpression.Member;
        }

        public static PropertyInfo GetProperty(LambdaExpression expression)
        {
            MemberExpression memberExpression = GetMemberExpression(expression, true);
            return (PropertyInfo)memberExpression.Member;
        }

        private static MemberExpression GetMemberExpression<TModel, T>(Expression<Func<TModel, T>> expression)
        {
            MemberExpression memberExpression = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression) expression.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression.Body as MemberExpression;
            }


            if (memberExpression == null) throw new ArgumentException("Not a member access", "member");
            return memberExpression;
        }

        public static Accessor GetAccessor(LambdaExpression expression)
        {
            MemberExpression memberExpression = GetMemberExpression(expression, true);

            return GetAccessor(memberExpression);
        }

        public static MemberExpression GetMemberExpression(this LambdaExpression expression, bool enforceMemberExpression)
        {
            MemberExpression memberExpression = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression)expression.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression.Body as MemberExpression;
            }


            if (enforceMemberExpression && memberExpression == null) throw new ArgumentException("Not a member access", "member");
            return memberExpression;
        }

        public static bool IsMemberExpression<T>(Expression<Func<T, object>> expression) => IsMemberExpression<T, object>(expression);

        public static bool IsMemberExpression<T, U>(Expression<Func<T, U>> expression) => GetMemberExpression(expression, false) != null;

        public static Accessor GetAccessor<TModel>(Expression<Func<TModel, object>> expression)
        {
            if (expression.Body is MethodCallExpression || expression.Body.NodeType == ExpressionType.ArrayIndex)
            {
                return GetAccessor(expression.Body);
            }

            MemberExpression memberExpression = GetMemberExpression(expression);

            return GetAccessor(memberExpression);
        }

        public static Accessor GetAccessor(Expression memberExpression)
        {
            var list = new List<IValueGetter>();

            BuildValueGetters(memberExpression, list);

            if (list.Count == 1 && list[0] is PropertyValueGetter)
            {
                return new SingleProperty(((PropertyValueGetter) list[0]).PropertyInfo);
            }

            if (list.Count == 1 && list[0] is MethodValueGetter)
            {
                return new SingleMethod((MethodValueGetter) list[0]);
            }

            if (list.Count == 1 && list[0] is IndexerValueGetter)
            {
                return new ArrayIndexer((IndexerValueGetter) list[0]);
            }

            list.Reverse();
            return new PropertyChain(list.ToArray());
        }

        private static void BuildValueGetters(Expression expression, IList<IValueGetter> list)
        {
            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
            {
                var propertyInfo = (PropertyInfo) memberExpression.Member;
                list.Add(new PropertyValueGetter(propertyInfo));
                if (memberExpression.Expression != null)
                {
                    BuildValueGetters(memberExpression.Expression, list);
                }
            }

            //deals with collection indexers, an indexer [0] will look like a get(0) method call expression
            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression != null)
            {
                var methodInfo = methodCallExpression.Method;
                Expression argument = methodCallExpression.Arguments.FirstOrDefault();

                if (argument == null)
                {
                    var methodValueGetter = new MethodValueGetter(methodInfo, new object[0]);
                    list.Add(methodValueGetter);
                }
                else
                {
                    object value;
                    if (TryEvaluateExpression(argument, out value))
                    {
                        var methodValueGetter = new MethodValueGetter(methodInfo, new object[] { value });
                        list.Add(methodValueGetter);
                    }  
                }
               

                
                if (methodCallExpression.Object != null)
                {
                    BuildValueGetters(methodCallExpression.Object, list);
                }
            }

            if (expression.NodeType == ExpressionType.ArrayIndex)
            {
                var binaryExpression = (BinaryExpression) expression;

                var indexExpression = binaryExpression.Right;

                object index;
                if (TryEvaluateExpression(indexExpression, out index))
                {
                    var indexValueGetter = new IndexerValueGetter(binaryExpression.Left.Type, (int) index);
                    
                    list.Add(indexValueGetter);
                }

                BuildValueGetters(binaryExpression.Left, list);
            }
        }

        private static bool TryEvaluateExpression(Expression operation, out object value)
        {
            if (operation == null)
            {   // used for static fields, etc
                value = null;
                return true;
            }
            switch (operation.NodeType)
            {
                case ExpressionType.Constant:
                    value = ((ConstantExpression)operation).Value;
                    return true;
                case ExpressionType.MemberAccess:
                    MemberExpression me = (MemberExpression)operation;
                    object target;
                    if (TryEvaluateExpression(me.Expression, out target))
                    { // instance target
                        if (me.Member is FieldInfo)
                        {
                            value = ((FieldInfo)me.Member).GetValue(target);
                            return true;
                        }
                        if (me.Member is PropertyInfo)
                        {
                            value = ((PropertyInfo)me.Member).GetValue(target, null);
                            return true;
                        }
                    }
                    break;
            }
            value = null;
            return false;
        }

        public static Accessor GetAccessor<TModel, T>(Expression<Func<TModel, T>> expression)
        {
            MemberExpression memberExpression = GetMemberExpression(expression);

            return GetAccessor(memberExpression);
        }

        public static MethodInfo GetMethod<T>(Expression<Func<T, object>> expression) => new FindMethodVisitor(expression).Method;

        public static MethodInfo GetMethod(Expression<Func<object>> expression) => GetMethod<Func<object>>(expression);

        public static MethodInfo GetMethod(Expression expression) => new FindMethodVisitor(expression).Method;

        public static MethodInfo GetMethod<TDelegate>(Expression<TDelegate> expression) => new FindMethodVisitor(expression).Method;

        public static MethodInfo GetMethod<T, U>(Expression<Func<T, U>> expression) => new FindMethodVisitor(expression).Method;

        public static MethodInfo GetMethod<T, U, V>(Expression<Func<T, U, V>> expression) => new FindMethodVisitor(expression).Method;

        public static MethodInfo GetMethod<T>(Expression<Action<T>> expression) => new FindMethodVisitor(expression).Method;
    }

    public class FindMethodVisitor : ExpressionVisitor
    {
        public FindMethodVisitor(Expression expression)
        {
            Visit(expression);
        }

        public MethodInfo Method { get; private set; }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            Method = m.Method;
            return m;
        }
    }
}