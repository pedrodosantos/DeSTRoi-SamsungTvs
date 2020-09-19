// WPFLocalizeExtension.Extensions.LocTextExtension
using System;
using System.Windows.Markup;
using WPFLocalizeExtension.BaseExtensions;
using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Extensions;

namespace WPFLocalizeExtension.Extensions
{

	[MarkupExtensionReturnType(typeof(string))]
	public class LocTextExtension : BaseLocalizeExtension<string>
	{
		protected enum TextAppendType
		{
			Prefix,
			Suffix
		}

		private string prefix;

		private string suffix;

		private string[] formatSegments;

		public string Prefix
		{
			get
			{
				return prefix;
			}
			set
			{
				prefix = value;
				HandleNewValue();
			}
		}

		public string Suffix
		{
			get
			{
				return suffix;
			}
			set
			{
				suffix = value;
				HandleNewValue();
			}
		}

		public string FormatSegment1
		{
			get
			{
				return formatSegments[0];
			}
			set
			{
				formatSegments[0] = value;
				HandleNewValue();
			}
		}

		public string FormatSegment2
		{
			get
			{
				return formatSegments[1];
			}
			set
			{
				formatSegments[1] = value;
				HandleNewValue();
			}
		}

		public bool ResolveLocalizedValue2(out string resolvedValue)
		{
			return ResolveLocalizedValue(out resolvedValue);
		}

		public string FormatSegment3
		{
			get
			{
				return formatSegments[2];
			}
			set
			{
				formatSegments[2] = value;
				HandleNewValue();
			}
		}

		public string FormatSegment4
		{
			get
			{
				return formatSegments[3];
			}
			set
			{
				formatSegments[3] = value;
				HandleNewValue();
			}
		}

		public string FormatSegment5
		{
			get
			{
				return formatSegments[4];
			}
			set
			{
				formatSegments[4] = value;
				HandleNewValue();
			}
		}

		public LocTextExtension()
		{
			InitializeLocText();
		}

		public LocTextExtension(string key)
			: base(key)
		{
			InitializeLocText();
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
			throw new NotSupportedException($"ResourceKey '{base.Key}' returns '{obj.GetType().FullName}' which is not type of System.String");
		}

		protected virtual string FormatText(string target)
		{
			return target ?? string.Empty;
		}

		protected override void HandleNewValue()
		{
			SetNewValue(FormatOutput(null));
		}

		protected override object FormatOutput(object input)
		{
			input = ((!LocalizeDictionary.Instance.GetIsInDesignMode() || base.DesignValue == null) ? (input ?? LocalizeDictionary.Instance.GetLocalizedObject<object>(base.Assembly, base.Dict, base.Key, GetForcedCultureOrDefault())) : base.DesignValue);
			string format = (input as string) ?? string.Empty;
			try
			{
				format = string.Format(LocalizeDictionary.Instance.SpecificCulture, format, formatSegments[0] ?? string.Empty, formatSegments[1] ?? string.Empty, formatSegments[2] ?? string.Empty, formatSegments[3] ?? string.Empty, formatSegments[4] ?? string.Empty);
			}
			catch (FormatException)
			{
				format = "TextFormatError: Max 5 Format PlaceHolders! {0} to {4}";
			}
			string appendText = GetAppendText(TextAppendType.Prefix);
			string appendText2 = GetAppendText(TextAppendType.Suffix);
			input = FormatText(appendText + format + appendText2);
			return input;
		}

		private void InitializeLocText()
		{
			formatSegments = new string[5];
			formatSegments.Initialize();
		}

		private string GetAppendText(TextAppendType at)
		{
			string result = string.Empty;
			if (at == TextAppendType.Prefix && !string.IsNullOrEmpty(prefix))
			{
				result = (prefix ?? string.Empty);
			}
			else if (at == TextAppendType.Suffix && !string.IsNullOrEmpty(suffix))
			{
				result = (suffix ?? string.Empty);
			}
			return result;
		}
	}
}