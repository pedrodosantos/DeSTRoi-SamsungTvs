// AttachedCommandBehavior.EventHandlerGenerator
using AttachedCommandBehavior;
using System;
using System.Reflection;
using System.Reflection.Emit;
namespace AttachedCommandBehavior
{
	public static class EventHandlerGenerator
	{
		public static Delegate CreateDelegate(Type eventHandlerType, MethodInfo methodToInvoke, object methodInvoker)
		{
			MethodInfo method;
			method = eventHandlerType.GetMethod("Invoke");
			Type parameterType;
			parameterType = method.ReturnParameter.ParameterType;
			if (parameterType != typeof(void))
			{
				throw new ApplicationException("Delegate has a return type. This only supprts event handlers that are void");
			}
			ParameterInfo[] parameters;
			parameters = method.GetParameters();
			Type[] array;
			array = new Type[parameters.Length + 1];
			array[0] = methodInvoker.GetType();
			for (int i = 0; i < parameters.Length; i++)
			{
				array[i + 1] = parameters[i].ParameterType;
			}
			DynamicMethod dynamicMethod;
			dynamicMethod = new DynamicMethod("", null, array, typeof(EventHandlerGenerator));
			ILGenerator iLGenerator;
			iLGenerator = dynamicMethod.GetILGenerator();
			LocalBuilder local;
			local = iLGenerator.DeclareLocal(typeof(object[]));
			iLGenerator.Emit(OpCodes.Ldc_I4, parameters.Length + 1);
			iLGenerator.Emit(OpCodes.Newarr, typeof(object));
			iLGenerator.Emit(OpCodes.Stloc, local);
			for (int j = 1; j < parameters.Length + 1; j++)
			{
				iLGenerator.Emit(OpCodes.Ldloc, local);
				iLGenerator.Emit(OpCodes.Ldc_I4, j);
				iLGenerator.Emit(OpCodes.Ldarg, j);
				iLGenerator.Emit(OpCodes.Stelem_Ref);
			}
			iLGenerator.Emit(OpCodes.Ldloc, local);
			iLGenerator.Emit(OpCodes.Ldarg_0);
			iLGenerator.EmitCall(OpCodes.Call, methodToInvoke, null);
			iLGenerator.Emit(OpCodes.Pop);
			iLGenerator.Emit(OpCodes.Ret);
			return dynamicMethod.CreateDelegate(eventHandlerType, methodInvoker);
		}
	}
}