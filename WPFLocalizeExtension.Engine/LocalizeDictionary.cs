// WPFLocalizeExtension.Engine.LocalizeDictionary
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
namespace WPFLocalizeExtension.Engine
{
	public sealed class LocalizeDictionary : DependencyObject
	{
		public sealed class WeakCultureChangedEventManager : WeakEventManager
		{
			private bool isListening;

			private ListenerList listeners;

			private static WeakCultureChangedEventManager CurrentManager
			{
				get
				{
					Type typeFromHandle = typeof(WeakCultureChangedEventManager);
					WeakCultureChangedEventManager weakCultureChangedEventManager = (WeakCultureChangedEventManager)WeakEventManager.GetCurrentManager(typeFromHandle);
					if (weakCultureChangedEventManager == null)
					{
						weakCultureChangedEventManager = new WeakCultureChangedEventManager();
						WeakEventManager.SetCurrentManager(typeFromHandle, weakCultureChangedEventManager);
					}
					return weakCultureChangedEventManager;
				}
			}

			private WeakCultureChangedEventManager()
			{
				listeners = new ListenerList();
			}

			internal static void AddListener(IWeakEventListener listener)
			{
				CurrentManager.listeners.Add(listener);
				CurrentManager.StartStopListening();
			}

			internal static void RemoveListener(IWeakEventListener listener)
			{
				CurrentManager.listeners.Remove(listener);
				CurrentManager.StartStopListening();
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			protected override void StartListening(object source)
			{
				if (!isListening)
				{
					Instance.OnCultureChanged += Instance_OnCultureChanged;
					isListening = true;
				}
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			protected override void StopListening(object source)
			{
				if (isListening)
				{
					Instance.OnCultureChanged -= Instance_OnCultureChanged;
					isListening = false;
				}
			}

			private void Instance_OnCultureChanged()
			{
				DeliverEventToList(Instance, EventArgs.Empty, listeners);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			private void StartStopListening()
			{
				if (listeners.Count != 0)
				{
					if (!isListening)
					{
						StartListening(null);
					}
				}
				else if (isListening)
				{
					StopListening(null);
				}
			}
		}

		public const string ResourcesName = "Resources";

		private const string ResourceManagerName = "ResourceManager";

		private const string ResourceFileExtension = ".resources";

		private const BindingFlags ResourceBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		[DesignOnly(true)]
		public static readonly DependencyProperty DesignCultureProperty = DependencyProperty.RegisterAttached("DesignCulture", typeof(string), typeof(LocalizeDictionary), new PropertyMetadata(SetCultureFromDependencyProperty));

		private static readonly object SyncRoot = new object();

		private static LocalizeDictionary instance;

		private CultureInfo culture;

		public static CultureInfo DefaultCultureInfo => CultureInfo.InvariantCulture;

		public static LocalizeDictionary Instance
		{
			get
			{
				if (instance == null)
				{
					lock (SyncRoot)
					{
						if (instance == null)
						{
							instance = new LocalizeDictionary();
						}
					}
				}
				return instance;
			}
		}

		public CultureInfo Culture
		{
			get
			{
				if (culture == null)
				{
					culture = DefaultCultureInfo;
				}
				return culture;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				culture = value;
				if (this.OnCultureChanged != null)
				{
					this.OnCultureChanged();
				}
			}
		}

		public Dictionary<string, ResourceManager> ResourceManagerList
		{
			get;
			private set;
		}

		public CultureInfo SpecificCulture => CultureInfo.CreateSpecificCulture(Culture.ToString());

		internal event Action OnCultureChanged;

		private LocalizeDictionary()
		{
			ResourceManagerList = new Dictionary<string, ResourceManager>();
		}

		[DesignOnly(true)]
		public static string GetDesignCulture(DependencyObject obj)
		{
			if (Instance.GetIsInDesignMode())
			{
				return (string)obj.GetValue(DesignCultureProperty);
			}
			return Instance.Culture.ToString();
		}

		public static void ParseKey(string inKey, out string outAssembly, out string outDict, out string outKey)
		{
			outAssembly = null;
			outDict = null;
			outKey = null;
			if (!string.IsNullOrEmpty(inKey))
			{
				string[] array = inKey.Trim().Split(":".ToCharArray(), 3);
				if (array.Length == 3)
				{
					outAssembly = ((!string.IsNullOrEmpty(array[0])) ? array[0] : null);
					outDict = ((!string.IsNullOrEmpty(array[1])) ? array[1] : null);
					outKey = array[2];
				}
				if (array.Length == 2)
				{
					outDict = ((!string.IsNullOrEmpty(array[0])) ? array[0] : null);
					outKey = array[1];
				}
				if (array.Length == 1)
				{
					outKey = array[0];
				}
			}
			else if (!Instance.GetIsInDesignMode())
			{
				throw new ArgumentNullException("inKey");
			}
		}

		[DesignOnly(true)]
		public static void SetDesignCulture(DependencyObject obj, string value)
		{
			if (Instance.GetIsInDesignMode())
			{
				obj.SetValue(DesignCultureProperty, value);
			}
		}

		public void AddEventListener(IWeakEventListener listener)
		{
			WeakCultureChangedEventManager.AddListener(listener);
		}

		public string GetAssemblyName(Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			if (assembly.FullName == null)
			{
				throw new NullReferenceException("assembly.FullName is null");
			}
			return assembly.FullName.Split(',')[0];
		}

		public bool GetIsInDesignMode()
		{
			bool retVal = false;
			if (Thread.CurrentThread == Application.Current.Dispatcher.Thread)
			{
				return DesignerProperties.GetIsInDesignMode(this);
			}
			Application.Current.Dispatcher.Invoke((Action)delegate
			{
				retVal = DesignerProperties.GetIsInDesignMode(this);
			}, null);
			return retVal;
		}

		public TType GetLocalizedObject<TType>(string resourceAssembly, string resourceDictionary, string resourceKey, CultureInfo cultureToUse) where TType : class
		{
			if (resourceAssembly == null)
			{
				throw new ArgumentNullException("resourceAssembly");
			}
			if (resourceAssembly == string.Empty)
			{
				throw new ArgumentException("resourceAssembly is empty", "resourceAssembly");
			}
			if (resourceDictionary == null)
			{
				throw new ArgumentNullException("resourceDictionary");
			}
			if (resourceDictionary == string.Empty)
			{
				throw new ArgumentException("resourceDictionary is empty", "resourceDictionary");
			}
			if (string.IsNullOrEmpty(resourceKey))
			{
				if (GetIsInDesignMode())
				{
					return null;
				}
				if (resourceKey == null)
				{
					throw new ArgumentNullException("resourceKey");
				}
				if (resourceKey == string.Empty)
				{
					throw new ArgumentException("resourceKey is empty", "resourceKey");
				}
			}
			ResourceManager resourceManager;
			try
			{
				resourceManager = GetResourceManager(resourceAssembly, resourceDictionary, resourceKey);
			}
			catch
			{
				if (GetIsInDesignMode())
				{
					return null;
				}
				throw;
			}
			object obj2 = resourceManager.GetObject(resourceKey, cultureToUse) as TType;
			if (obj2 == null && !GetIsInDesignMode())
			{
				throw new ArgumentException(string.Format("No resource key with name '{0}' in dictionary '{1}' in assembly '{2}' founded! ({2}.{1}.{0})", resourceKey, resourceDictionary, resourceAssembly));
			}
			return obj2 as TType;
		}

		public void RemoveEventListener(IWeakEventListener listener)
		{
			WeakCultureChangedEventManager.RemoveListener(listener);
		}

		public bool ResourceKeyExists(string resourceAssembly, string resourceDictionary, string resourceKey)
		{
			return ResourceKeyExists(resourceAssembly, resourceDictionary, resourceKey, CultureInfo.InvariantCulture);
		}

		public bool ResourceKeyExists(string resourceAssembly, string resourceDictionary, string resourceKey, CultureInfo cultureToUse)
		{
			try
			{
				return GetResourceManager(resourceAssembly, resourceDictionary, resourceKey).GetObject(resourceKey, cultureToUse) != null;
			}
			catch
			{
				if (GetIsInDesignMode())
				{
					return false;
				}
				throw;
			}
		}

		[DesignOnly(true)]
		private static void SetCultureFromDependencyProperty(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			if (!Instance.GetIsInDesignMode())
			{
				return;
			}
			CultureInfo cultureInfo;
			try
			{
				cultureInfo = CultureInfo.GetCultureInfo((string)args.NewValue);
			}
			catch
			{
				if (!Instance.GetIsInDesignMode())
				{
					throw;
				}
				cultureInfo = DefaultCultureInfo;
			}
			if (cultureInfo != null)
			{
				Instance.Culture = cultureInfo;
			}
		}

		private ResourceManager GetResourceManager(string resourceAssembly, string resourceDictionary, string resourceKey)
		{
			if (resourceAssembly == null)
			{
				resourceAssembly = GetAssemblyName(Assembly.GetExecutingAssembly());
			}
			if (resourceDictionary == null)
			{
				resourceDictionary = "Resources";
			}
			if (string.IsNullOrEmpty(resourceKey))
			{
				throw new ArgumentNullException("resourceKey");
			}
			Assembly assembly = null;
			string text = null;
			string text2 = "." + resourceDictionary + ".resources";
			ResourceManager resourceManager;
			if (ResourceManagerList.ContainsKey(resourceAssembly + text2))
			{
				resourceManager = ResourceManagerList[resourceAssembly + text2];
			}
			else
			{
				try
				{
					Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
					foreach (Assembly assembly2 in assemblies)
					{
						if (assembly2.FullName != null)
						{
							AssemblyName assemblyName = new AssemblyName(assembly2.FullName);
							if (assemblyName.Name == resourceAssembly)
							{
								assembly = assembly2;
								break;
							}
						}
					}
					if (assembly == null)
					{
						assembly = Assembly.Load(new AssemblyName(resourceAssembly));
					}
				}
				catch (Exception innerException)
				{
					throw new Exception($"The Assembly '{resourceAssembly}' cannot be loaded.", innerException);
				}
				string[] manifestResourceNames = assembly.GetManifestResourceNames();
				for (int j = 0; j < manifestResourceNames.Length; j++)
				{
					if (manifestResourceNames[j].StartsWith(resourceAssembly + ".") && manifestResourceNames[j].EndsWith(text2))
					{
						text = manifestResourceNames[j];
						break;
					}
				}
				if (text == null)
				{
					throw new ArgumentException(string.Format("No resource key with name '{0}' in dictionary '{1}' in assembly '{2}' founded! ({2}.{1}.{0})", resourceKey, resourceDictionary, resourceAssembly));
				}
				text = text.Substring(0, text.Length - ".resources".Length);
				try
				{
					PropertyInfo property = assembly.GetType(text).GetProperty("ResourceManager", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					MethodInfo getMethod = property.GetGetMethod(nonPublic: true);
					object obj = getMethod.Invoke(null, null);
					resourceManager = (ResourceManager)obj;
				}
				catch (Exception innerException2)
				{
					throw new InvalidOperationException("Cannot resolve the ResourceManager!", innerException2);
				}
				ResourceManagerList.Add(resourceAssembly + text2, resourceManager);
			}
			return resourceManager;
		}
	}
}