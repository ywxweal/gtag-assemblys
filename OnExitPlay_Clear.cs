using System;
using System.Reflection;
using UnityEngine;

// Token: 0x020009B5 RID: 2485
public class OnExitPlay_Clear : OnExitPlay_Attribute
{
	// Token: 0x06003B6B RID: 15211 RVA: 0x0011B79C File Offset: 0x0011999C
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
