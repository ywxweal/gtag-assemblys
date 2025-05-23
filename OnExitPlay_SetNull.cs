using System;
using System.Reflection;
using UnityEngine;

// Token: 0x020009B2 RID: 2482
public class OnExitPlay_SetNull : OnExitPlay_Attribute
{
	// Token: 0x06003B65 RID: 15205 RVA: 0x0011B558 File Offset: 0x00119758
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't SetNull non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		field.SetValue(null, null);
	}
}
