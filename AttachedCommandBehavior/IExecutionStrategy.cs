// AttachedCommandBehavior.IExecutionStrategy
using AttachedCommandBehavior;
namespace AttachedCommandBehavior
{
	public interface IExecutionStrategy
	{
		CommandBehaviorBinding Behavior
		{
			get;
			set;
		}

		void Execute(object parameter);
	}
}