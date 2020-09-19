// AttachedCommandBehavior.BehaviorBindingCollection
using AttachedCommandBehavior;
using System.Windows;
namespace AttachedCommandBehavior
{
	public class BehaviorBindingCollection : FreezableCollection<BehaviorBinding>
	{
		public DependencyObject Owner
		{
			get;
			set;
		}
	}
}