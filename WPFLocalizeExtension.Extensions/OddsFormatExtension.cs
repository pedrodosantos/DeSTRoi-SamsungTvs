// WPFLocalizeExtension.Extensions.OddsFormatExtension
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using WPFLocalizeExtension.Engine;

namespace WPFLocalizeExtension.Extensions
{
	[ContentProperty("ResourceIdentifierKey")]
	[MarkupExtensionReturnType(typeof(string))]
	public sealed class OddsFormatExtension : MarkupExtension, IWeakEventListener
	{
		private static Dictionary<decimal, string> oddsFormatLookupTableUk;

		private decimal displayValue;

		private Collection<WeakReference> targetObjects;

		private bool cutTrailingDecimals;

		public static Dictionary<decimal, string> OddsFormatLookupTableUk
		{
			get
			{
				if (oddsFormatLookupTableUk == null)
				{
					oddsFormatLookupTableUk = GetUKOddsFormatLookupTable();
				}
				return oddsFormatLookupTableUk;
			}
		}

		public decimal DisplayValue
		{
			get
			{
				return displayValue;
			}
			set
			{
				displayValue = value;
				HandleNewValue();
			}
		}

		public bool CutTrailingDecimals
		{
			get
			{
				return cutTrailingDecimals;
			}
			set
			{
				cutTrailingDecimals = value;
				HandleNewValue();
			}
		}

		[ConstructorArgument("displayValue")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public decimal InitializeValue
		{
			get;
			set;
		}

		public OddsFormatType? ForceOddsFormatType
		{
			get;
			set;
		}

		public ReadOnlyCollection<WeakReference> TargetObjects => new ReadOnlyCollection<WeakReference>(targetObjects);

		public DependencyProperty TargetProperty
		{
			get;
			private set;
		}

		public OddsFormatExtension(decimal displayValue)
			: this()
		{
			this.displayValue = displayValue;
		}

		private OddsFormatExtension()
		{
			targetObjects = new Collection<WeakReference>();
		}

		public static string GetLocalizedOddsString(decimal sourceOdds)
		{
			return GetLocalizedOddsString(sourceOdds, cutTrailingDecimals: false);
		}

		public static string GetLocalizedOddsString(decimal sourceOdds, bool cutTrailingDecimals)
		{
			return GetLocalizedOddsString(sourceOdds, OddsFormatManager.Instance.OddsFormatType, LocalizeDictionary.Instance.SpecificCulture, cutTrailingDecimals);
		}

		public static string GetLocalizedOddsString(decimal sourceOdds, OddsFormatType oddsType)
		{
			return GetLocalizedOddsString(sourceOdds, oddsType, cutTrailingDecimals: false);
		}

		public static string GetLocalizedOddsString(decimal sourceOdds, OddsFormatType oddsType, bool cutTrailingDecimals)
		{
			return GetLocalizedOddsString(sourceOdds, oddsType, LocalizeDictionary.Instance.SpecificCulture, cutTrailingDecimals);
		}

		public static string GetLocalizedOddsString(decimal sourceOdds, OddsFormatType oddsType, CultureInfo specificCulture, bool cutTrailingDecimals)
		{
			switch (oddsType)
			{
				case OddsFormatType.UK:
					{
						if (sourceOdds <= 0m)
						{
							return "0";
						}
						if (TryGetUKOddsLookupValue(sourceOdds, out string retVal))
						{
							return retVal;
						}
						decimal d = sourceOdds % 1.0m * 100m;
						if (d == 0m)
						{
							return string.Format(specificCulture, "{0:N0}/1", new object[1]
							{
					sourceOdds - 1m
							});
						}
						int num = 0;
						int num2 = 0;
						decimal d2 = 1m;
						while (d > 1m && num < 2 && num2 < 2 && (d % 2m == 0m || d % 5m == 0m))
						{
							if (d % 2m == 0m)
							{
								num++;
								d /= 2m;
								d2 *= 2m;
							}
							if (d % 5m == 0m)
							{
								num2++;
								d /= 5m;
								d2 *= 5m;
							}
						}
						decimal num3 = 100m / d2;
						decimal num4 = (sourceOdds - 1m) * num3;
						return string.Format(specificCulture, "{0:N0}/{1:N0}", new object[2]
						{
				num4,
				num3
						});
					}
				case OddsFormatType.US:
					if (sourceOdds <= 1m)
					{
						return "0";
					}
					if (sourceOdds < 2m)
					{
						return (-100m / (sourceOdds - 1m)).ToString("N0", specificCulture);
					}
					return string.Format(specificCulture, "+{0:N0}", new object[1]
					{
				(sourceOdds - 1m) * 100m
					});
				case OddsFormatType.EU:
					if (cutTrailingDecimals)
					{
						return sourceOdds.ToString("N0", specificCulture);
					}
					return sourceOdds.ToString("N2", specificCulture);
				default:
					throw new NotSupportedException();
			}
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			IProvideValueTarget provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (provideValueTarget == null)
			{
				throw new InvalidOperationException("IProvideValueTarget service is unavailable");
			}
			if (provideValueTarget.TargetObject is Binding)
			{
				throw new InvalidOperationException("Use as binding is not supported!");
			}
			TargetProperty = (provideValueTarget.TargetProperty as DependencyProperty);
			if (TargetProperty == null)
			{
				return this;
			}
			bool flag = false;
			foreach (WeakReference targetObject in targetObjects)
			{
				if (targetObject.Target == provideValueTarget.TargetObject)
				{
					flag = true;
					break;
				}
			}
			if (provideValueTarget.TargetObject is DependencyObject && !flag)
			{
				if (targetObjects.Count == 0)
				{
					OddsFormatManager.Instance.AddEventListener(this);
				}
				targetObjects.Add(new WeakReference(provideValueTarget.TargetObject));
				ObjectDependencyManager.AddObjectDependency(new WeakReference(provideValueTarget.TargetObject), this);
			}
			if (!(provideValueTarget.TargetObject is DependencyObject))
			{
				return this;
			}
			return GetLocalizedOddsString(displayValue, GetForcedOddsFormatOrDefault(), CutTrailingDecimals);
		}

		public bool SetBinding(DependencyObject targetObject, DependencyProperty targetProperty)
		{
			bool flag = false;
			foreach (WeakReference targetObject2 in targetObjects)
			{
				if (targetObject2.Target == targetObject)
				{
					flag = true;
					break;
				}
			}
			TargetProperty = targetProperty;
			if (!flag)
			{
				if (targetObjects.Count == 0)
				{
					OddsFormatManager.Instance.AddEventListener(this);
				}
				targetObjects.Add(new WeakReference(targetObject));
				ObjectDependencyManager.AddObjectDependency(new WeakReference(targetObject), this);
				targetObject.SetValue(TargetProperty, GetLocalizedOddsString(displayValue, GetForcedOddsFormatOrDefault(), CutTrailingDecimals));
				return true;
			}
			return false;
		}

		public override string ToString()
		{
			return $"{DisplayValue} -> {GetForcedOddsFormatOrDefault()}";
		}

		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			if (managerType == typeof(OddsFormatManager.WeakOddsFormatChangedEventManager))
			{
				HandleNewValue();
				return true;
			}
			return false;
		}

		private static Dictionary<decimal, string> GetUKOddsFormatLookupTable()
		{
			Dictionary<decimal, string> dictionary = new Dictionary<decimal, string>();
			dictionary.Add(11.00m, "10/1");
			dictionary.Add(10.00m, "9/1");
			dictionary.Add(9.50m, "17/2");
			dictionary.Add(9.00m, "8/1");
			dictionary.Add(8.50m, "15/2");
			dictionary.Add(8.00m, "7/1");
			dictionary.Add(7.50m, "13/2");
			dictionary.Add(7.00m, "6/1");
			dictionary.Add(6.50m, "11/2");
			dictionary.Add(6.00m, "5/1");
			dictionary.Add(5.50m, "9/2");
			dictionary.Add(5.00m, "4/1");
			dictionary.Add(4.60m, "18/5");
			dictionary.Add(4.50m, "7/2");
			dictionary.Add(4.333m, "10/3");
			dictionary.Add(4.20m, "16/5");
			dictionary.Add(4.00m, "3/1");
			dictionary.Add(3.80m, "15/5");
			dictionary.Add(3.75m, "11/4");
			dictionary.Add(3.60m, "13/5");
			dictionary.Add(3.50m, "5/2");
			dictionary.Add(3.40m, "12/5");
			dictionary.Add(3.375m, "19/8");
			dictionary.Add(3.30m, "23/10");
			dictionary.Add(3.25m, "9/4");
			dictionary.Add(3.20m, "11/5");
			dictionary.Add(3.125m, "17/8");
			dictionary.Add(3.10m, "21/10");
			dictionary.Add(3.00m, "2/1");
			dictionary.Add(2.90m, "19/10");
			dictionary.Add(2.875m, "15/8");
			dictionary.Add(2.80m, "9/5");
			dictionary.Add(2.75m, "7/4");
			dictionary.Add(2.70m, "17/10");
			dictionary.Add(2.625m, "13/8");
			dictionary.Add(2.60m, "8/5");
			dictionary.Add(2.50m, "6/4");
			dictionary.Add(2.40m, "7/5");
			dictionary.Add(2.375m, "11/8");
			dictionary.Add(2.30m, "13/10");
			dictionary.Add(2.25m, "5/4");
			dictionary.Add(2.20m, "6/5");
			dictionary.Add(2.10m, "11/10");
			dictionary.Add(2.05m, "21/20");
			dictionary.Add(2.00m, "1/1");
			dictionary.Add(1.952m, "20/21");
			dictionary.Add(1.909m, "10/11");
			dictionary.Add(1.90m, "9/10");
			dictionary.Add(1.833m, "5/6");
			dictionary.Add(1.80m, "4/5");
			dictionary.Add(1.727m, "8/11");
			dictionary.Add(1.70m, "7/10");
			dictionary.Add(1.667m, "4/6");
			dictionary.Add(1.625m, "5/8");
			dictionary.Add(1.615m, "8/13");
			dictionary.Add(1.60m, "3/5");
			dictionary.Add(1.571m, "4/7");
			dictionary.Add(1.533m, "8/15");
			dictionary.Add(1.50m, "1/2");
			dictionary.Add(1.471m, "8/17");
			dictionary.Add(1.45m, "9/20");
			dictionary.Add(1.444m, "4/9");
			dictionary.Add(1.40m, "2/5");
			dictionary.Add(1.364m, "4/11");
			dictionary.Add(1.35m, "7/20");
			dictionary.Add(1.333m, "1/3");
			dictionary.Add(1.30m, "3/10");
			dictionary.Add(1.286m, "2/7");
			dictionary.Add(1.25m, "1/4");
			dictionary.Add(1.222m, "2/9");
			dictionary.Add(1.2m, "1/5");
			return dictionary;
		}

		private static bool TryGetUKOddsLookupValue(decimal valToCheck, out string retVal)
		{
			if (OddsFormatLookupTableUk.ContainsKey(valToCheck))
			{
				retVal = OddsFormatLookupTableUk[valToCheck];
				return true;
			}
			retVal = null;
			return false;
		}

		private OddsFormatType GetForcedOddsFormatOrDefault()
		{
			return ForceOddsFormatType ?? OddsFormatManager.Instance.OddsFormatType;
		}

		private void HandleNewValue()
		{
			SetNewValue(GetLocalizedOddsString(displayValue, GetForcedOddsFormatOrDefault(), CutTrailingDecimals));
		}

		private void SetNewValue(object newValue)
		{
			if (targetObjects.Count == 0 || TargetProperty == null)
			{
				return;
			}
			foreach (WeakReference targetObject in targetObjects)
			{
				if (targetObject.IsAlive)
				{
					((DependencyObject)targetObject.Target).SetValue(TargetProperty, newValue);
				}
			}
		}
	}
}