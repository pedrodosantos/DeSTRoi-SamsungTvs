// WPFLocalizeExtension.Engine.OddsFormatManager
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using WPFLocalizeExtension.Engine;
namespace WPFLocalizeExtension.Engine
{
	public sealed class OddsFormatManager : DependencyObject
	{
		public sealed class WeakOddsFormatChangedEventManager : WeakEventManager
		{
			private bool isListening;

			private ListenerList listeners;

			private static WeakOddsFormatChangedEventManager CurrentManager
			{
				get
				{
					Type typeFromHandle = typeof(WeakOddsFormatChangedEventManager);
					WeakOddsFormatChangedEventManager weakOddsFormatChangedEventManager = (WeakOddsFormatChangedEventManager)WeakEventManager.GetCurrentManager(typeFromHandle);
					if (weakOddsFormatChangedEventManager == null)
					{
						weakOddsFormatChangedEventManager = new WeakOddsFormatChangedEventManager();
						WeakEventManager.SetCurrentManager(typeFromHandle, weakOddsFormatChangedEventManager);
					}
					return weakOddsFormatChangedEventManager;
				}
			}

			private WeakOddsFormatChangedEventManager()
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
					Instance.OnOddsFormatChanged += Instance_OnOddsFormatChanged;
					LocalizeDictionary.Instance.OnCultureChanged += Instance_OnCultureChanged;
					isListening = true;
				}
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			protected override void StopListening(object source)
			{
				if (isListening)
				{
					Instance.OnOddsFormatChanged -= Instance_OnOddsFormatChanged;
					LocalizeDictionary.Instance.OnCultureChanged -= Instance_OnCultureChanged;
					isListening = false;
				}
			}

			private void Instance_OnCultureChanged()
			{
				DeliverEventToList(Instance, EventArgs.Empty, listeners);
			}

			private void Instance_OnOddsFormatChanged()
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

		[DesignOnly(true)]
		public static readonly DependencyProperty DesignOddsFormatProperty = DependencyProperty.RegisterAttached("DesignOddsFormat", typeof(OddsFormatType), typeof(OddsFormatManager), new PropertyMetadata(DefaultOddsFormatType, SetOddsFormatFromDependencyProperty));

		private static readonly object SyncRoot = new object();

		private static OddsFormatManager instance;

		private OddsFormatType oddsFormatType = DefaultOddsFormatType;

		public static OddsFormatType DefaultOddsFormatType => OddsFormatType.EU;

		public static OddsFormatManager Instance
		{
			get
			{
				if (instance == null)
				{
					lock (SyncRoot)
					{
						if (instance == null)
						{
							instance = new OddsFormatManager();
						}
					}
				}
				return instance;
			}
		}

		public OddsFormatType OddsFormatType
		{
			get
			{
				return oddsFormatType;
			}
			set
			{
				if (!Enum.IsDefined(typeof(OddsFormatType), value))
				{
					throw new ArgumentNullException("value");
				}
				oddsFormatType = value;
				if (this.OnOddsFormatChanged != null)
				{
					this.OnOddsFormatChanged();
				}
			}
		}

		internal event Action OnOddsFormatChanged;

		private OddsFormatManager()
		{
		}

		[DesignOnly(true)]
		public static OddsFormatType GetDesignOddsFormat(DependencyObject obj)
		{
			if (Instance.GetIsInDesignMode())
			{
				return (OddsFormatType)obj.GetValue(DesignOddsFormatProperty);
			}
			return Instance.OddsFormatType;
		}

		[DesignOnly(true)]
		public static void SetDesignOddsFormat(DependencyObject obj, OddsFormatType value)
		{
			if (Instance.GetIsInDesignMode())
			{
				obj.SetValue(DesignOddsFormatProperty, value);
			}
		}

		public void AddEventListener(IWeakEventListener listener)
		{
			WeakOddsFormatChangedEventManager.AddListener(listener);
		}

		public bool GetIsInDesignMode()
		{
			return DesignerProperties.GetIsInDesignMode(this);
		}

		public void RemoveEventListener(IWeakEventListener listener)
		{
			WeakOddsFormatChangedEventManager.RemoveListener(listener);
		}

		[DesignOnly(true)]
		private static void SetOddsFormatFromDependencyProperty(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			if (!Instance.GetIsInDesignMode())
			{
				return;
			}
			if (!Enum.IsDefined(typeof(OddsFormatType), args.NewValue))
			{
				if (!Instance.GetIsInDesignMode())
				{
					throw new InvalidCastException($"\"{args.NewValue}\" not defined in Enum OddsFormatType");
				}
				Instance.OddsFormatType = DefaultOddsFormatType;
			}
			else
			{
				Instance.OddsFormatType = (OddsFormatType)Enum.Parse(typeof(OddsFormatType), args.NewValue.ToString(), ignoreCase: true);
			}
		}
	}
}