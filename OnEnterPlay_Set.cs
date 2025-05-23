using System;
using System.Reflection;
using UnityEngine;

// Token: 0x020009AB RID: 2475
public class OnEnterPlay_Set : OnEnterPlay_Attribute
{
	// Token: 0x06003B57 RID: 15191 RVA: 0x0011B58E File Offset: 0x0011978E
	public OnEnterPlay_Set(object value)
	{
		this.value = value;
	}

	// Token: 0x06003B58 RID: 15192 RVA: 0x0011B59D File Offset: 0x0011979D
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Set non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		field.SetValue(null, this.value);
	}

	// Token: 0x04003FD9 RID: 16345
	private object value;
}
