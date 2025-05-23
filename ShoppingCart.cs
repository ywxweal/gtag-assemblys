using System;
using UnityEngine;

// Token: 0x0200043D RID: 1085
public class ShoppingCart : MonoBehaviour
{
	// Token: 0x06001AC0 RID: 6848 RVA: 0x00082E11 File Offset: 0x00081011
	public void Awake()
	{
		if (ShoppingCart.instance == null)
		{
			ShoppingCart.instance = this;
			return;
		}
		if (ShoppingCart.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06001AC1 RID: 6849 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Start()
	{
	}

	// Token: 0x06001AC2 RID: 6850 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Update()
	{
	}

	// Token: 0x04001DCB RID: 7627
	[OnEnterPlay_SetNull]
	public static volatile ShoppingCart instance;
}
