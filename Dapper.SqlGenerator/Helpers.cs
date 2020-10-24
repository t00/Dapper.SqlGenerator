using System;
using System.Linq.Expressions;

namespace Dapper.SqlGenerator
{
    internal static class Helpers
    {
        internal static string GetMemberName<T>(Expression<T> expression)
        {
            switch (expression.Body)
            {
                case MemberExpression m:
                    return m.Member.Name;
                case UnaryExpression u when u.Operand is MemberExpression m:
                    return m.Member.Name;
                default:
                    throw new NotImplementedException(expression.GetType().ToString());
            }
        }
    }
}