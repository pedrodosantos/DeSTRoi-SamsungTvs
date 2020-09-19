// WPFLocalizeExtension.Extensions.LocTextUpperExtension
using System;
using System.Windows.Markup;
using WPFLocalizeExtension;
using WPFLocalizeExtension.BaseExtensions;
using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Extensions;
namespace WPFLocalizeExtension.Extensions
{
	[MarkupExtensionReturnType(typeof(string))]
	public class LocTextUpperExtension : LocTextExtension
	{
		public LocTextUpperExtension()
		{
		}

		public LocTextUpperExtension(string key)
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
				return obj;
			}
			throw new NotSupportedException($"ResourceKey '{base.Key}' returns '{obj.GetType().FullName}' which is not type of System.String");
		}

		protected override string FormatText(string target)
		{
			if (target != null)
			{
				return target.ToUpper(GetForcedCultureOrDefault()).Replace("ß", "SS");
			}
			return string.Empty;
		}

		protected override void HandleNewValue()
		{
			object localizedObject = LocalizeDictionary.Instance.GetLocalizedObject<object>(base.Assembly, base.Dict, base.Key, GetForcedCultureOrDefault());
			SetNewValue(FormatOutput(localizedObject));
		}
	}
}