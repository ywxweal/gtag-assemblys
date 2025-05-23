using System;
using System.Reflection;
using UnityEngine;

// Token: 0x020009B6 RID: 2486
public class OnExitPlay_Run : OnExitPlay_Attribute
{
	// Token: 0x06003B6D RID: 15213 RVA: 0x0011B67C File Offset: 0x0011987C
	public override void OnEnterPlay(MethodInfo method)
	{
		if (!method.IsStatic)
		{
			Debug.LogError(string.Format("Can't Run non-static method {0}.{1}", method.DeclaringType, method.Name));
			return;
		}
		method.Invoke(null, new object[0]);
	}
}
