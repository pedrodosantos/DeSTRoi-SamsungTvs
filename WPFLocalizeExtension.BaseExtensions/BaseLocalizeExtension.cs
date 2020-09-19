using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using WPFLocalizeExtension.Engine;

namespace WPFLocalizeExtension.BaseExtensions
{
	[ContentProperty("ResourceIdentifierKey")]
	[MarkupExtensionReturnType(typeof(object))]
	public abstract class BaseLocalizeExtension<TValue> : MarkupExtension, IWeakEventListener, INotifyPropertyChanged
	{
		private readonly Dictionary<WeakReference, object> targetObjects;

		private string assembly;

		private TValue currentValue;

		private string dict;

		private string key;

		public string Assembly
		{
			get
			{
				return assembly ?? LocalizeDictionary.Instance.GetAssemblyName(System.Reflection.Assembly.GetExecutingAssembly());
			}
			set
			{
				assembly = ((!string.IsNullOrEmpty(value)) ? value : null);
			}
		}

		public TValue CurrentValue
		{
			get
			{
				return currentValue;
			}
			private set
			{
				currentValue = value;
				RaiseNotifyPropertyChanged("CurrentValue");
			}
		}

		[DesignOnly(true)]
		public object DesignValue
		{
			get;
			set;
		}

		public string Dict
		{
			get
			{
				return dict ?? "Resources";
			}
			set
			{
				dict = ((!string.IsNullOrEmpty(value)) ? value : null);
			}
		}

		public string ForceCulture
		{
			get;
			set;
		}

		public string Key
		{
			get
			{
				return key;
			}
			set
			{
				key = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[ConstructorArgument("key")]
		public string InitializeValue
		{
			get;
			set;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public string ResourceIdentifierKey
		{
			get
			{
				return string.Format("{0}:{1}:{2}", Assembly, Dict, Key ?? "(null)");
			}
			set
			{
				LocalizeDictionary.ParseKey(value, out assembly, out dict, out key);
			}
		}

		public Dictionary<WeakReference, object> TargetObjects => targetObjects;

		public event PropertyChangedEventHandler PropertyChanged;

		protected BaseLocalizeExtension()
		{
			targetObjects = new Dictionary<WeakReference, object>();
		}

		protected BaseLocalizeExtension(string key)
			: this()
		{
			LocalizeDictionary.ParseKey(key, out assembly, out dict, out this.key);
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
			{
				return this;
			}
			IProvideValueTarget provideValueTarget;
			provideValueTarget = (serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget);
			if (provideValueTarget == null)
			{
				return this;
			}
			if (provideValueTarget.TargetObject is Binding)
			{
				throw new InvalidOperationException("Use as binding is not supported!");
			}
			object obj;
			obj = null;
			if (provideValueTarget.TargetProperty is DependencyProperty || provideValueTarget.TargetProperty is PropertyInfo)
			{
				obj = provideValueTarget.TargetProperty;
			}
			if (obj == null)
			{
				return this;
			}
			if (!(provideValueTarget.TargetObject is DependencyObject) && !(provideValueTarget.TargetProperty is PropertyInfo))
			{
				return this;
			}
			bool flag;
			flag = false;
			foreach (KeyValuePair<WeakReference, object> targetObject in targetObjects)
			{
				if (targetObject.Key.Target == provideValueTarget.TargetObject && targetObject.Value == provideValueTarget.TargetProperty)
				{
					flag = true;
					break;
				}
			}
			if (provideValueTarget.TargetObject is DependencyObject && !flag)
			{
				if (targetObjects.Count == 0)
				{
					LocalizeDictionary.Instance.AddEventListener(this);
				}
				targetObjects.Add(new WeakReference(provideValueTarget.TargetObject), provideValueTarget.TargetProperty);
				ObjectDependencyManager.AddObjectDependency(new WeakReference(provideValueTarget.TargetObject), this);
			}
			return LocalizeDictionary.Instance.GetLocalizedObject<object>(Assembly, Dict, Key, GetForcedCultureOrDefault());
		}

		public bool ResolveLocalizedValue(out TValue resolvedValue)
		{
			return ResolveLocalizedValue(out resolvedValue, GetForcedCultureOrDefault());
		}

		public bool ResolveLocalizedValue(out TValue resolvedValue, CultureInfo targetCulture)
		{
			resolvedValue = default(TValue);
			object localizedObject;
			localizedObject = LocalizeDictionary.Instance.GetLocalizedObject<object>(Assembly, Dict, Key, targetCulture);
			if (localizedObject is TValue)
			{
				object obj;
				obj = FormatOutput(localizedObject);
				if (obj != null)
				{
					resolvedValue = (TValue)obj;
				}
				return true;
			}
			return false;
		}

		public bool SetBinding(DependencyObject targetObject, object targetProperty)
		{
			if (!(targetProperty is DependencyProperty) && !(targetProperty is PropertyInfo))
			{
				throw new ArgumentException("The targetProperty should be a DependencyProperty or PropertyInfo!", "targetProperty");
			}
			bool flag;
			flag = false;
			foreach (KeyValuePair<WeakReference, object> targetObject2 in targetObjects)
			{
				if (targetObject2.Key.Target == targetObject && targetObject2.Value == targetProperty)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (targetObjects.Count == 0)
				{
					LocalizeDictionary.Instance.AddEventListener(this);
				}
				targetObjects.Add(new WeakReference(targetObject), targetProperty);
				ObjectDependencyManager.AddObjectDependency(new WeakReference(targetObject), this);
				object value;
				value = FormatOutput(LocalizeDictionary.Instance.GetLocalizedObject<object>(Assembly, Dict, Key, GetForcedCultureOrDefault()));
				SetTargetValue(targetObject, targetProperty, value);
				return true;
			}
			return false;
		}

		public sealed override string ToString()
		{
			return base.ToString() + " -> " + ResourceIdentifierKey;
		}

		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			if (managerType == typeof(LocalizeDictionary.WeakCultureChangedEventManager))
			{
				HandleNewValue();
				return true;
			}
			return false;
		}

		public bool IsTypeOf(Type checkType, Type targetType)
		{
			if (checkType == null)
			{
				return false;
			}
			if (targetType == null)
			{
				return false;
			}
			if (targetType.IsGenericType)
			{
				if (checkType.IsGenericType && checkType.GetGenericTypeDefinition() == targetType)
				{
					return true;
				}
				return IsTypeOf(checkType.BaseType, targetType);
			}
			if (checkType.Equals(targetType))
			{
				return true;
			}
			return IsTypeOf(checkType.BaseType, targetType);
		}

		protected abstract object FormatOutput(object input);

		protected CultureInfo GetForcedCultureOrDefault()
		{
			if (!string.IsNullOrEmpty(ForceCulture))
			{
				try
				{
					return CultureInfo.CreateSpecificCulture(ForceCulture);
				}
				catch (ArgumentException innerException)
				{
					if (!LocalizeDictionary.Instance.GetIsInDesignMode())
					{
						throw new ArgumentException("Cannot create a CultureInfo with '" + ForceCulture + "'", innerException);
					}
					return LocalizeDictionary.Instance.SpecificCulture;
				}
			}
			return LocalizeDictionary.Instance.SpecificCulture;
		}

		protected virtual void HandleNewValue()
		{
			SetNewValue(LocalizeDictionary.Instance.GetLocalizedObject<object>(Assembly, Dict, Key, GetForcedCultureOrDefault()));
		}

		protected void SetNewValue(object newValue)
		{
			if (newValue is TValue)
			{
				CurrentValue = (TValue)newValue;
			}
			if (targetObjects.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<WeakReference, object> targetObject in targetObjects)
			{
				if (targetObject.Key.IsAlive)
				{
					SetTargetValue((DependencyObject)targetObject.Key.Target, targetObject.Value, newValue);
				}
			}
		}

		private void RaiseNotifyPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private void SetTargetValue(DependencyObject targetObject, object targetProperty, object value)
		{
			if (targetProperty is DependencyProperty)
			{
				SetTargetValue(targetObject, (DependencyProperty)targetProperty, value);
			}
			if (targetProperty is PropertyInfo)
			{
				SetTargetValue(targetObject, (PropertyInfo)targetProperty, value);
			}
		}

		private void SetTargetValue(DependencyObject targetObject, DependencyProperty targetProperty, object value)
		{
			targetObject.SetValue(targetProperty, value);
		}

		private void SetTargetValue(DependencyObject targetObject, PropertyInfo targetProperty, object value)
		{
			targetProperty.SetValue(targetObject, value, null);
		}
	}
}
