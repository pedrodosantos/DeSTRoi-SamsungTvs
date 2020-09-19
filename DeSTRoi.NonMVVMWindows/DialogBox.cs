// DeSTRoi.NonMVVMWindows.DialogBox
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
public class DialogBox : Window, IComponentConnector
{
	public class DialogButton
	{
		public enum SpecialButtonEnum
		{
			None,
			Cancel,
			Ok
		}

		private string _content = "";

		private Key _key;

		private ModifierKeys _modifier;

		private SpecialButtonEnum _specialButton;

		private bool _closeDialog;

		public string Content
		{
			get
			{
				return _content;
			}
			set
			{
				_content = value;
			}
		}

		public Key ShortcutKey
		{
			get
			{
				return _key;
			}
			set
			{
				_key = value;
			}
		}

		public ModifierKeys ShortcutModifier
		{
			get
			{
				return _modifier;
			}
			set
			{
				_modifier = value;
			}
		}

		public SpecialButtonEnum SpecialButton
		{
			get
			{
				return _specialButton;
			}
			set
			{
				_specialButton = value;
			}
		}

		public bool CloseDialog
		{
			get
			{
				return _closeDialog;
			}
			set
			{
				_closeDialog = value;
			}
		}

		public DialogButton()
		{
		}

		public DialogButton(string content)
		{
			_content = content;
		}

		public DialogButton(string content, bool closeDialog)
			: this(content)
		{
			_closeDialog = closeDialog;
		}

		public DialogButton(string content, SpecialButtonEnum specialButton)
			: this(content)
		{
			_specialButton = specialButton;
		}

		public DialogButton(string content, SpecialButtonEnum specialButton, bool closeDialog)
			: this(content, closeDialog)
		{
			_specialButton = specialButton;
		}

		public DialogButton(string content, Key key, ModifierKeys modifier)
			: this(content)
		{
			_key = key;
			_modifier = modifier;
		}

		public DialogButton(string content, Key key, ModifierKeys modifier, bool closeDialog)
			: this(content, key, modifier)
		{
			_closeDialog = closeDialog;
		}

		public override string ToString()
		{
			return _content;
		}
	}

	private static DialogBox staticThis = null;

	private string _selectedButton;

	public static readonly DependencyProperty MainIconProperty = DependencyProperty.Register("MainIcon", typeof(BitmapImage), typeof(DialogBox), new FrameworkPropertyMetadata(new BitmapImage()));

	public static readonly DependencyProperty IconVisibleProperty = DependencyProperty.Register("IconVisible", typeof(bool), typeof(DialogBox), new FrameworkPropertyMetadata(true));

	public static readonly DependencyProperty MainInstructionProperty = DependencyProperty.Register("MainInstruction", typeof(string), typeof(DialogBox), new FrameworkPropertyMetadata(""));

	public static readonly DependencyProperty MainContentProperty = DependencyProperty.Register("MainContent", typeof(string), typeof(DialogBox), new FrameworkPropertyMetadata(""));

	public static readonly DependencyProperty ExpanderHeaderProperty = DependencyProperty.Register("ExpanderHeader", typeof(string), typeof(DialogBox), new FrameworkPropertyMetadata(""));

	public static readonly DependencyProperty ExpanderContentProperty = DependencyProperty.Register("ExpanderContent", typeof(string), typeof(DialogBox), new FrameworkPropertyMetadata(""));

	public static readonly DependencyProperty ExpanderIsExpandedProperty = DependencyProperty.Register("ExpanderIsExpanded", typeof(bool), typeof(DialogBox), new FrameworkPropertyMetadata(false));

	public static readonly DependencyProperty ExpanderVisibleProperty = DependencyProperty.Register("ExpanderVisible", typeof(bool), typeof(DialogBox), new FrameworkPropertyMetadata(false));

	public static readonly DependencyProperty IconAnimationEnabledProperty = DependencyProperty.Register("IconAnimationEnabled", typeof(bool), typeof(DialogBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, IconAnimationEnablePropertyChanged));

	public static readonly DependencyProperty ProgressBarVisibleProperty = DependencyProperty.Register("ProgressBarVisible", typeof(bool), typeof(DialogBox), new FrameworkPropertyMetadata(false));

	public static readonly DependencyProperty ProgressBarMinProperty = DependencyProperty.Register("ProgressBarMin", typeof(int), typeof(DialogBox), new FrameworkPropertyMetadata(0));

	public static readonly DependencyProperty ProgressBarMaxProperty = DependencyProperty.Register("ProgressBarMax", typeof(int), typeof(DialogBox), new FrameworkPropertyMetadata(100));

	public static readonly DependencyProperty ProgressBarValueProperty = DependencyProperty.Register("ProgressBarValue", typeof(int), typeof(DialogBox), new FrameworkPropertyMetadata(0));

	public static readonly DependencyProperty ProgressBarIsMarqueeProperty = DependencyProperty.Register("ProgressBarIsMarquee", typeof(bool), typeof(DialogBox), new FrameworkPropertyMetadata(false));

	public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register("Buttons", typeof(DialogButton[]), typeof(DialogBox), new FrameworkPropertyMetadata(new DialogButton[1]
	{
		new DialogButton("_Ok", DialogButton.SpecialButtonEnum.Ok)
	}, FrameworkPropertyMetadataOptions.None, ButtonsPropertyChanged));

	internal ColumnDefinition gcol1;

	internal ColumnDefinition gcol2;

	internal Image dbMainIcon;

	internal TextBlock dbMainInstruction;

	internal TextBlock dbContent;

	internal Expander dbExpander;

	internal TextBlock dbExpandedInfo;

	internal ProgressBar dbProgress;

	internal StackPanel dbspButtons;

	private bool _contentLoaded;

	public BitmapImage MainIcon
	{
		get
		{
			return (BitmapImage)GetValue(MainIconProperty);
		}
		set
		{
			SetValue(MainIconProperty, value);
		}
	}

	public bool IconVisible
	{
		get
		{
			return (bool)GetValue(IconVisibleProperty);
		}
		set
		{
			SetValue(IconVisibleProperty, value);
		}
	}

	public string MainInstruction
	{
		get
		{
			return (string)GetValue(MainInstructionProperty);
		}
		set
		{
			SetValue(MainInstructionProperty, value);
		}
	}

	public string MainContent
	{
		get
		{
			return (string)GetValue(MainContentProperty);
		}
		set
		{
			SetValue(MainContentProperty, value);
		}
	}

	public string ExpanderHeader
	{
		get
		{
			return (string)GetValue(ExpanderHeaderProperty);
		}
		set
		{
			SetValue(ExpanderHeaderProperty, value);
		}
	}

	public string ExpanderContent
	{
		get
		{
			return (string)GetValue(ExpanderContentProperty);
		}
		set
		{
			SetValue(ExpanderContentProperty, value);
		}
	}

	public bool ExpanderIsExpanded
	{
		get
		{
			return (bool)GetValue(ExpanderIsExpandedProperty);
		}
		set
		{
			SetValue(ExpanderIsExpandedProperty, value);
		}
	}

	public bool ExpanderVisible
	{
		get
		{
			return (bool)GetValue(ExpanderVisibleProperty);
		}
		set
		{
			SetValue(ExpanderVisibleProperty, value);
		}
	}

	public bool IconAnimationEnable
	{
		get
		{
			return (bool)GetValue(IconAnimationEnabledProperty);
		}
		set
		{
			SetValue(IconAnimationEnabledProperty, value);
		}
	}

	public bool ProgressBarVisible
	{
		get
		{
			return (bool)GetValue(ProgressBarVisibleProperty);
		}
		set
		{
			SetValue(ProgressBarVisibleProperty, value);
		}
	}

	public int ProgressBarMin
	{
		get
		{
			return (int)GetValue(ProgressBarMinProperty);
		}
		set
		{
			SetValue(ProgressBarMinProperty, value);
		}
	}

	public int ProgressBarMax
	{
		get
		{
			return (int)GetValue(ProgressBarMaxProperty);
		}
		set
		{
			SetValue(ProgressBarMaxProperty, value);
		}
	}

	public int ProgressBarValue
	{
		get
		{
			return (int)GetValue(ProgressBarValueProperty);
		}
		set
		{
			SetValue(ProgressBarValueProperty, value);
		}
	}

	public bool ProgressBarIsMarquee
	{
		get
		{
			return (bool)GetValue(ProgressBarIsMarqueeProperty);
		}
		set
		{
			SetValue(ProgressBarIsMarqueeProperty, value);
		}
	}

	public DialogButton[] Buttons
	{
		get
		{
			return (DialogButton[])GetValue(ButtonsProperty);
		}
		set
		{
			SetValue(ButtonsProperty, value);
		}
	}

	public string SelectedButton => _selectedButton;

	public event RoutedEventHandler ButtonClicked;

	public DialogBox()
	{
		InitializeComponent();
	}

	private static void IconAnimationEnablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (staticThis != null && e.NewValue != e.OldValue)
		{
			if ((bool)e.NewValue)
			{
				staticThis.BeginIconAnimation();
			}
			else
			{
				staticThis.StopIconAnimation();
			}
		}
	}

	private static void ButtonsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (staticThis == null)
		{
			return;
		}
		staticThis.dbspButtons.Children.Clear();
		if (e.NewValue == null)
		{
			return;
		}
		DialogButton[] array = (DialogButton[])e.NewValue;
		foreach (DialogButton dialogButton in array)
		{
			Button button = new Button();
			button.Tag = dialogButton;
			button.Height = 23.0;
			button.Content = dialogButton.Content;
			switch (dialogButton.SpecialButton)
			{
			case DialogButton.SpecialButtonEnum.Cancel:
				button.IsCancel = true;
				button.IsDefault = false;
				break;
			case DialogButton.SpecialButtonEnum.Ok:
				button.IsDefault = true;
				button.IsCancel = false;
				break;
			default:
			{
				bool isCancel = button.IsDefault = false;
				button.IsCancel = isCancel;
				break;
			}
			}
			button.Padding = new Thickness(6.0, 0.0, 6.0, 0.0);
			button.Margin = new Thickness(6.0, 0.0, 6.0, 0.0);
			button.Click += staticThis.btn_Click;
			staticThis.dbspButtons.Children.Add(button);
		}
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		staticThis = this;
		if (IconAnimationEnable)
		{
			BeginIconAnimation();
		}
		if (Buttons == null)
		{
			return;
		}
		DialogButton[] buttons = Buttons;
		foreach (DialogButton dialogButton in buttons)
		{
			Button button = new Button();
			button.Tag = dialogButton;
			button.Height = 23.0;
			button.Content = dialogButton.Content;
			switch (dialogButton.SpecialButton)
			{
			case DialogButton.SpecialButtonEnum.Cancel:
				button.IsCancel = true;
				button.IsDefault = false;
				break;
			case DialogButton.SpecialButtonEnum.Ok:
				button.IsDefault = true;
				button.IsCancel = false;
				break;
			default:
			{
				bool isCancel = button.IsDefault = false;
				button.IsCancel = isCancel;
				break;
			}
			}
			button.Padding = new Thickness(6.0, 0.0, 6.0, 0.0);
			button.Margin = new Thickness(6.0, 0.0, 6.0, 0.0);
			button.Click += staticThis.btn_Click;
			staticThis.dbspButtons.Children.Add(button);
		}
	}

	private void BeginIconAnimation()
	{
		DoubleAnimation doubleAnimation = new DoubleAnimation();
		doubleAnimation.From = 28.0;
		doubleAnimation.To = 36.0;
		doubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(4.0));
		SineEase sineEase = new SineEase();
		sineEase.EasingMode = EasingMode.EaseInOut;
		doubleAnimation.AutoReverse = true;
		doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
		dbMainIcon.BeginAnimation(FrameworkElement.WidthProperty, doubleAnimation);
		dbMainIcon.BeginAnimation(FrameworkElement.HeightProperty, doubleAnimation);
	}

	private void StopIconAnimation()
	{
		dbMainIcon.BeginAnimation(FrameworkElement.WidthProperty, null);
		dbMainIcon.BeginAnimation(FrameworkElement.HeightProperty, null);
	}

	private void btn_Click(object sender, RoutedEventArgs e)
	{
		Button button = (Button)sender;
		_selectedButton = button.Content.ToString();
		if (this.ButtonClicked != null)
		{
			this.ButtonClicked(button.Tag, new RoutedEventArgs());
		}
		if (((DialogButton)button.Tag).CloseDialog)
		{
			Close();
		}
	}

	private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
	{
		DialogButton[] buttons = Buttons;
		DialogButton btn;
		for (int i = 0; i < buttons.Length; i++)
		{
			btn = buttons[i];
			if (btn.SpecialButton != 0 || btn.ShortcutKey == Key.None || !e.KeyboardDevice.IsKeyDown(btn.ShortcutKey))
			{
				continue;
			}
			if (btn.ShortcutModifier == ModifierKeys.None)
			{
				btn_Click((from Button bt in staticThis.dbspButtons.Children
					where bt.Tag == btn
					select bt).First(), new RoutedEventArgs());
				continue;
			}
			switch (btn.ShortcutModifier)
			{
			case ModifierKeys.Alt:
				if (e.KeyboardDevice.IsKeyDown(Key.LeftAlt))
				{
					btn_Click((from Button bt in staticThis.dbspButtons.Children
						where bt.Tag == btn
						select bt).First(), new RoutedEventArgs());
				}
				break;
			case ModifierKeys.Control:
				if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
				{
					btn_Click((from Button bt in staticThis.dbspButtons.Children
						where bt.Tag == btn
						select bt).First(), new RoutedEventArgs());
				}
				break;
			case ModifierKeys.Shift:
				if (e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift))
				{
					btn_Click((from Button bt in staticThis.dbspButtons.Children
						where bt.Tag == btn
						select bt).First(), new RoutedEventArgs());
				}
				break;
			case ModifierKeys.Windows:
				if (e.KeyboardDevice.IsKeyDown(Key.LWin) || e.KeyboardDevice.IsKeyDown(Key.RWin))
				{
					btn_Click((from Button bt in staticThis.dbspButtons.Children
						where bt.Tag == btn
						select bt).First(), new RoutedEventArgs());
				}
				break;
			}
		}
	}

	public static bool? ShowDialog(string title, string content, bool showErrorIcon)
	{
		return ShowDialog(null, title, content, showErrorIcon, null, null);
	}

	public static bool? ShowDialog(Window owner, string title, string content, bool showErrorIcon)
	{
		return ShowDialog(owner, title, content, showErrorIcon, null, null);
	}

	public static bool? ShowDialog(string title, string content, bool showErrorIcon, string expander, string expcontent)
	{
		return ShowDialog(null, title, content, showErrorIcon, expander, expcontent);
	}

	public static bool? ShowDialog(Window owner, string title, string content, bool showErrorIcon, string expander, string expcontent)
	{
		DialogBox dialogBox = new DialogBox();
		dialogBox.MainInstruction = title;
		dialogBox.MainContent = content;
		if (showErrorIcon)
		{
			dialogBox.MainIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Delete_32.png"));
		}
		if (expander != null)
		{
			dialogBox.ExpanderVisible = true;
			dialogBox.ExpanderHeader = expander;
			dialogBox.ExpanderContent = expcontent;
		}
		dialogBox.Buttons = new DialogButton[1]
		{
			new DialogButton("_Ok", DialogButton.SpecialButtonEnum.Ok, closeDialog: true)
		};
		return dialogBox.ShowDialog();
	}

	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/DeSTRoi;component/nonmvvmwindows/dialogbox/dialogbox.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			((DialogBox)target).Loaded += Window_Loaded;
			((DialogBox)target).PreviewKeyDown += Window_PreviewKeyDown;
			break;
		case 2:
			gcol1 = (ColumnDefinition)target;
			break;
		case 3:
			gcol2 = (ColumnDefinition)target;
			break;
		case 4:
			dbMainIcon = (Image)target;
			break;
		case 5:
			dbMainInstruction = (TextBlock)target;
			break;
		case 6:
			dbContent = (TextBlock)target;
			break;
		case 7:
			dbExpander = (Expander)target;
			break;
		case 8:
			dbExpandedInfo = (TextBlock)target;
			break;
		case 9:
			dbProgress = (ProgressBar)target;
			break;
		case 10:
			dbspButtons = (StackPanel)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
