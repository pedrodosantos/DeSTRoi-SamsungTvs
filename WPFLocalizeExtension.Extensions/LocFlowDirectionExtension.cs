// WPFLocalizeExtension.Extensions.LocFlowDirectionExtension
using System;
using System.Windows;
using System.Windows.Markup;
using WPFLocalizeExtension;
using WPFLocalizeExtension.BaseExtensions;
using WPFLocalizeExtension.Engine;

namespace WPFLocalizeExtension.Extensions
{
	[MarkupExtensionReturnType(typeof(FlowDirection))]
	public class LocFlowDirectionExtension : BaseLocalizeExtension<FlowDirection>
	{
		public LocFlowDirectionExtension()
		{
		}

		public LocFlowDirectionExtension(string key)
			: base(key)
		{
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			object obj = base.ProvideValue(serviceProvider) ?? "LeftToRight";
			if (IsTypeOf(obj.GetType(), typeof(BaseLocalizeExtension<>)))
			{
				return obj;
			}
			if (obj.GetType().Equals(typeof(string)))
			{
				return FormatOutput(obj);
			}
			throw new NotSupportedException($"ResourceKey '{base.Key}' returns '{obj.GetType().FullName}' which is not type of FlowDirection");
		}

		protected override void HandleNewValue()
		{
			object localizedObject = LocalizeDictionary.Instance.GetLocalizedObject<object>(base.Assembly, base.Dict, base.Key, GetForcedCultureOrDefault());
			SetNewValue(FormatOutput(localizedObject));
		}

		protected override object FormatOutput(object input)
		{
			if (LocalizeDictionary.Instance.GetIsInDesignMode() && base.DesignValue != null)
			{
				try
				{
					return Enum.Parse(typeof(FlowDirection), (string)base.DesignValue, ignoreCase: true);
				}
				catch
				{
					return null;
				}
			}
			return Enum.Parse(typeof(FlowDirection), (string)input, ignoreCase: true);
		}
	}
}