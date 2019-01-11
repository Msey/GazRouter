using System.Linq.Expressions;
using System.Reflection;

namespace Utils.Extensions
{
	public static class LinqExtensions
	{
		public static MemberInfo GetMemberInfo(this Expression expression)
		{
			var lambdaExpression = (LambdaExpression)expression;
			return (!(lambdaExpression.Body is UnaryExpression) ? (MemberExpression)lambdaExpression.Body : (MemberExpression)((UnaryExpression)lambdaExpression.Body).Operand).Member;
		} 
	}
}