// AttachedCommandBehavior.CommandBehaviorCollection
using AttachedCommandBehavior;
using System;
using System.Collections.Specialized;
using System.Windows;
namespace AttachedCommandBehavior
{
	public class CommandBehaviorCollection
	{
		private static readonly DependencyPropertyKey BehaviorsPropertyKey = DependencyProperty.RegisterAttachedReadOnly("BehaviorsInternal", typeof(BehaviorBindingCollection), typeof(CommandBehaviorCollection), new FrameworkPropertyMetadata((object)null));

		public static readonly DependencyProperty BehaviorsProperty = BehaviorsPropertyKey.DependencyProperty;

		public static BehaviorBindingCollection GetBehaviors(DependencyObject d)
		{
			if (d == null)
			{
				throw new InvalidOperationException("The dependency object trying to attach to is set to null");
			}
			BehaviorBindingCollection behaviorBindingCollection;
			behaviorBindingCollection = (d.GetValue(BehaviorsProperty) as BehaviorBindingCollection);
			if (behaviorBindingCollection == null)
			{
				behaviorBindingCollection = new BehaviorBindingCollection();
				behaviorBindingCollection.Owner = d;
				SetBehaviors(d, behaviorBindingCollection);
			}
			return behaviorBindingCollection;
		}

		private static void SetBehaviors(DependencyObject d, BehaviorBindingCollection value)
		{
			d.SetValue(BehaviorsPropertyKey, value);
			((INotifyCollectionChanged)value).CollectionChanged += CollectionChanged;
		}

		private static void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			BehaviorBindingCollection behaviorBindingCollection;
			behaviorBindingCollection = (BehaviorBindingCollection)sender;
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Move:
					break;
				case NotifyCollectionChangedAction.Add:
					if (e.NewItems == null)
					{
						break;
					}
					foreach (BehaviorBinding newItem in e.NewItems)
					{
						newItem.Owner = behaviorBindingCollection.Owner;
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					if (e.OldItems == null)
					{
						break;
					}
					foreach (BehaviorBinding oldItem in e.OldItems)
					{
						oldItem.Behavior.Dispose();
					}
					break;
				case NotifyCollectionChangedAction.Replace:
					if (e.NewItems != null)
					{
						foreach (BehaviorBinding newItem2 in e.NewItems)
						{
							newItem2.Owner = behaviorBindingCollection.Owner;
						}
					}
					if (e.OldItems == null)
					{
						break;
					}
					foreach (BehaviorBinding oldItem2 in e.OldItems)
					{
						oldItem2.Behavior.Dispose();
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					if (e.OldItems == null)
					{
						break;
					}
					foreach (BehaviorBinding oldItem3 in e.OldItems)
					{
						oldItem3.Behavior.Dispose();
					}
					break;
			}
		}
	}
}