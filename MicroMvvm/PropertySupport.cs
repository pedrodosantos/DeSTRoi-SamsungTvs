// MicroMvvm.PropertySupport
using System;
using System.Linq.Expressions;
using System.Reflection;

public static class PropertySupport
{
	public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpresssion)
	{
		if (propertyExpresssion == null)
		{
			throw new ArgumentNullException("propertyExpresssion");
		}
		MemberExpression memberExpression = propertyExpresssion.Body as MemberExpression;
		if (memberExpression == null)
		{
			throw new ArgumentException("The expression is not a member access expression.", "propertyExpresssion");
		}
		PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
		if (propertyInfo == null)
		{
			throw new ArgumentException("The member access expression does not access a property.", "propertyExpresssion");
		}
		MethodInfo getMethod = propertyInfo.GetGetMethod(nonPublic: true);
		if (getMethod.IsStatic)
		{
			throw new ArgumentException("The referenced property is a static property.", "propertyExpresssion");
		}
		return memberExpression.Member.Name;
	}
}
