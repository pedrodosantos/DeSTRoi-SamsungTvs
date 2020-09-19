// WPFLocalizeExtension.Engine.ObjectDependencyManager
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace WPFLocalizeExtension.Engine
{
	public static class ObjectDependencyManager
	{
		private static Dictionary<object, List<WeakReference>> internalList;

		static ObjectDependencyManager()
		{
			internalList = new Dictionary<object, List<WeakReference>>();
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static bool AddObjectDependency(WeakReference weakRefDp, object objToHold)
		{
			CleanUp();
			if (objToHold == null)
			{
				throw new ArgumentNullException("objToHold", "The objToHold cannot be null");
			}
			if (objToHold.GetType() == typeof(WeakReference))
			{
				throw new ArgumentException("objToHold cannot be type of WeakReference", "objToHold");
			}
			if (weakRefDp.Target == objToHold)
			{
				throw new InvalidOperationException("The WeakReference.Target cannot be the same as objToHold");
			}
			bool result = false;
			if (!internalList.ContainsKey(objToHold))
			{
				List<WeakReference> list = new List<WeakReference>();
				list.Add(weakRefDp);
				List<WeakReference> value = list;
				internalList.Add(objToHold, value);
				result = true;
			}
			else
			{
				List<WeakReference> list2 = internalList[objToHold];
				if (!list2.Contains(weakRefDp))
				{
					list2.Add(weakRefDp);
					result = true;
				}
			}
			return result;
		}

		public static void CleanUp()
		{
			CleanUp(null);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void CleanUp(object objToRemove)
		{
			if (objToRemove != null)
			{
				if (!internalList.Remove(objToRemove))
				{
					throw new Exception("Key was not found!");
				}
				return;
			}
			List<object> list = new List<object>();
			foreach (KeyValuePair<object, List<WeakReference>> @internal in internalList)
			{
				for (int num = @internal.Value.Count - 1; num >= 0; num--)
				{
					if (!@internal.Value[num].IsAlive)
					{
						@internal.Value.RemoveAt(num);
					}
				}
				if (@internal.Value.Count == 0)
				{
					list.Add(@internal.Key);
				}
			}
			for (int num2 = list.Count - 1; num2 >= 0; num2--)
			{
				internalList.Remove(list[num2]);
			}
			list.Clear();
		}
	}
}