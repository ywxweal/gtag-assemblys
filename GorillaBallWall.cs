using System;
using UnityEngine;

// Token: 0x020005F1 RID: 1521
public class GorillaBallWall : MonoBehaviour
{
	// Token: 0x06002592 RID: 9618 RVA: 0x000BB326 File Offset: 0x000B9526
	private void Awake()
	{
		if (GorillaBallWall.instance == null)
		{
			GorillaBallWall.instance = this;
			return;
		}
		if (GorillaBallWall.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06002593 RID: 9619 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Update()
	{
	}

	// Token: 0x04002A14 RID: 10772
	[OnEnterPlay_SetNull]
	public static volatile GorillaBallWall instance;
}
