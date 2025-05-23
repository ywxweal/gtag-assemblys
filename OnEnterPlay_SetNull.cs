using System;
using System.Reflection;
using UnityEngine;

// Token: 0x020009AA RID: 2474
public class OnEnterPlay_SetNull : OnEnterPlay_Attribute
{
	// Token: 0x06003B54 RID: 15188 RVA: 0x0011B480 File Offset: 0x00119680
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
