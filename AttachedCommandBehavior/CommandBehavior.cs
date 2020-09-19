// AttachedCommandBehavior.CommandBehavior
using AttachedCommandBehavior;
using System;
using System.Windows;
using System.Windows.Input;
namespace AttachedCommandBehavior
{
	public class CommandBehavior
	{
		private static readonly DependencyProperty BehaviorProperty = DependencyProperty.RegisterAttached("Behavior", typeof(CommandBehaviorBinding), typeof(CommandBehavior), new FrameworkPropertyMetadata((object)null));

		public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(CommandBehavior), new FrameworkPropertyMetadata(null, OnCommandChanged));

		public static readonly DependencyProperty ActionProperty = DependencyProperty.RegisterAttached("Action", typeof(Action<object>), typeof(CommandBehavior), new FrameworkPropertyMetadata(null, OnActionChanged));

		public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(CommandBehavior), new FrameworkPropertyMetadata(null, OnCommandParameterChanged));

		public static readonly DependencyProperty EventProperty = DependencyProperty.RegisterAttached("Event", typeof(string), typeof(CommandBehavior), new FrameworkPropertyMetadata(string.Empty, OnEventChanged));

		private static CommandBehaviorBinding GetBehavior(DependencyObject d)
		{
			return (CommandBehaviorBinding)d.GetValue(BehaviorProperty);
		}

		private static void SetBehavior(DependencyObject d, CommandBehaviorBinding value)
		{
			d.SetValue(BehaviorProperty, value);
		}

		public static ICommand GetCommand(DependencyObject d)
		{
			return (ICommand)d.GetValue(CommandProperty);
		}

		public static void SetCommand(DependencyObject d, ICommand value)
		{
			d.SetValue(CommandProperty, value);
		}

		private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CommandBehaviorBinding commandBehaviorBinding;
			commandBehaviorBinding = FetchOrCreateBinding(d);
			commandBehaviorBinding.Command = (ICommand)e.NewValue;
		}

		public static Action<object> GetAction(DependencyObject d)
		{
			return (Action<object>)d.GetValue(ActionProperty);
		}

		public static void SetAction(DependencyObject d, Action<object> value)
		{
			d.SetValue(ActionProperty, value);
		}

		private static void OnActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CommandBehaviorBinding commandBehaviorBinding;
			commandBehaviorBinding = FetchOrCreateBinding(d);
			commandBehaviorBinding.Action = (Action<object>)e.NewValue;
		}

		public static object GetCommandParameter(DependencyObject d)
		{
			return d.GetValue(CommandParameterProperty);
		}

		public static void SetCommandParameter(DependencyObject d, object value)
		{
			d.SetValue(CommandParameterProperty, value);
		}

		private static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CommandBehaviorBinding commandBehaviorBinding;
			commandBehaviorBinding = FetchOrCreateBinding(d);
			commandBehaviorBinding.CommandParameter = e.NewValue;
		}

		public static string GetEvent(DependencyObject d)
		{
			return (string)d.GetValue(EventProperty);
		}

		public static void SetEvent(DependencyObject d, string value)
		{
			d.SetValue(EventProperty, value);
		}

		private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CommandBehaviorBinding commandBehaviorBinding;
			commandBehaviorBinding = FetchOrCreateBinding(d);
			if (commandBehaviorBinding.Event != null && commandBehaviorBinding.Owner != null)
			{
				commandBehaviorBinding.Dispose();
			}
			commandBehaviorBinding.BindEvent(d, e.NewValue.ToString());
		}

		private static CommandBehaviorBinding FetchOrCreateBinding(DependencyObject d)
		{
			CommandBehaviorBinding commandBehaviorBinding;
			commandBehaviorBinding = GetBehavior(d);
			if (commandBehaviorBinding == null)
			{
				commandBehaviorBinding = new CommandBehaviorBinding();
				SetBehavior(d, commandBehaviorBinding);
			}
			return commandBehaviorBinding;
		}
	}
}