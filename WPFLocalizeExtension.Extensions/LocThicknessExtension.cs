// WPFLocalizeExtension.Extensions.LocThicknessExtension
using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using WPFLocalizeExtension;
using WPFLocalizeExtension.BaseExtensions;
using WPFLocalizeExtension.Engine;

namespace WPFLocalizeExtension.Extensions
{
	[MarkupExtensionReturnType(typeof(Thickness))]
	public class LocThicknessExtension : BaseLocalizeExtension<Thickness>
	{
		public LocThicknessExtension()
		{
		}

		public LocThicknessExtension(string key)
			: base(key)
		{
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			object obj = base.ProvideValue(serviceProvider);
			if (obj == null)
			{
				return null;
			}
			if (IsTypeOf(obj.GetType(), typeof(BaseLocalizeExtension<>)))
			{
				return obj;
			}
			if (obj.GetType().Equals(typeof(string)))
			{
				return FormatOutput(obj);
			}
			throw new NotSupportedException($"ResourceKey '{base.Key}' returns '{obj.GetType().FullName}' which is not type of double");
		}

		protected override void HandleNewValue()
		{
			object localizedObject = LocalizeDictionary.Instance.GetLocalizedObject<object>(base.Assembly, base.Dict, base.Key, GetForcedCultureOrDefault());
			SetNewValue(FormatOutput(localizedObject));
		}

		protected override object FormatOutput(object input)
		{
			MethodInfo method = typeof(ThicknessConverter).GetMethod("FromString", BindingFlags.Static | BindingFlags.NonPublic);
			if (LocalizeDictionary.Instance.GetIsInDesignMode() && base.DesignValue != null)
			{
				try
				{
					return (Thickness)method.Invoke(null, new object[2]
					{
					base.DesignValue,
					new CultureInfo("en-US")
					});
				}
				catch
				{
					return null;
				}
			}
			return (Thickness)method.Invoke(null, new object[2]
			{
			input,
			new CultureInfo("en-US")
			});
		}
	}
}