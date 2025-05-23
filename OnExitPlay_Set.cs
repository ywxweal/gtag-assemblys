using System;
using System.Reflection;
using UnityEngine;

// Token: 0x020009B3 RID: 2483
public class OnExitPlay_Set : OnExitPlay_Attribute
{
	// Token: 0x06003B67 RID: 15207 RVA: 0x0011B701 File Offset: 0x00119901
	public OnExitPlay_Set(object value)
	{
		this.value = value;
	}

	// Token: 0x06003B68 RID: 15208 RVA: 0x0011B710 File Offset: 0x00119910
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Set non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		field.SetValue(null, this.value);
	}

	// Token: 0x04003FDC RID: 16348
	private object value;
}
