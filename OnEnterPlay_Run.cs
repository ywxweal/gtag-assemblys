using System;
using System.Reflection;
using UnityEngine;

// Token: 0x020009AE RID: 2478
public class OnEnterPlay_Run : OnEnterPlay_Attribute
{
	// Token: 0x06003B5C RID: 15196 RVA: 0x0011B5A4 File Offset: 0x001197A4
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
