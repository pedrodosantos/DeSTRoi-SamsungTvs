// AttachedCommandBehavior.ActionExecutionStrategy

using System;

namespace AttachedCommandBehavior
{
  public class ActionExecutionStrategy : IExecutionStrategy
  {
    public CommandBehaviorBinding Behavior
    {
      get;
      set;
    }

    public bool RetriesOnFailure => throw new NotImplementedException();

    public void Execute(object parameter)
    {
      Behavior.Action(parameter);
    }

  }
}
