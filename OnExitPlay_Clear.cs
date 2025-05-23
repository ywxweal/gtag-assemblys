using System;
using System.Reflection;
using UnityEngine;

// Token: 0x020009B5 RID: 2485
public class OnExitPlay_Clear : OnExitPlay_Attribute
{
	// Token: 0x06003B6A RID: 15210 RVA: 0x0011B6C4 File Offset: 0x001198C4
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Clear non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		field.FieldType.GetMethod("Clear").Invoke(field.GetValue(null), new object[0]);
	}
}
