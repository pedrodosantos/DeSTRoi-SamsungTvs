// AttachedCommandBehavior.CommandBehaviorBinding
using AttachedCommandBehavior;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
namespace AttachedCommandBehavior
{
	public class CommandBehaviorBinding : IDisposable
	{
		private IExecutionStrategy strategy;

		private ICommand command;

		private Action<object> action;

		private bool disposed;

		public DependencyObject Owner
		{
			get;
			private set;
		}

		public string EventName
		{
			get;
			private set;
		}

		public EventInfo Event
		{
			get;
			private set;
		}

		public Delegate EventHandler
		{
			get;
			private set;
		}

		public object CommandParameter
		{
			get;
			set;
		}

		public ICommand Command
		{
			get
			{
				return command;
			}
			set
			{
				command = value;
				strategy = new CommandExecutionStrategy
				{
					Behavior = this
				};
			}
		}

		public Action<object> Action
		{
			get
			{
				return action;
			}
			set
			{
				action = value;
				strategy = new ActionExecutionStrategy
				{
					Behavior = this
				};
			}
		}

		public void BindEvent(DependencyObject owner, string eventName)
		{
			EventName = eventName;
			Owner = owner;
			Event = Owner.GetType().GetEvent(EventName, BindingFlags.Instance | BindingFlags.Public);
			if (Event == null)
			{
				throw new InvalidOperationException($"Could not resolve event name {EventName}");
			}
			EventHandler = EventHandlerGenerator.CreateDelegate(Event.EventHandlerType, typeof(CommandBehaviorBinding).GetMethod("Execute", BindingFlags.Instance | BindingFlags.Public), this);
			Event.AddEventHandler(Owner, EventHandler);
		}

		public void Execute()
		{
			strategy.Execute(CommandParameter);
		}

		public void Dispose()
		{
			if (!disposed)
			{
				Event.RemoveEventHandler(Owner, EventHandler);
				disposed = true;
			}
		}
	}
}