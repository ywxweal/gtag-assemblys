using System;
using UnityEngine;

// Token: 0x020001E2 RID: 482
[DefaultExecutionOrder(-1000)]
public class HierarchyFlattenerReparentXform : MonoBehaviour
{
	// Token: 0x06000B46 RID: 2886 RVA: 0x0003C6B2 File Offset: 0x0003A8B2
	protected void Awake()
	{
		if (base.enabled)
		{
			this._DoIt();
		}
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x0003C6C2 File Offset: 0x0003A8C2
	protected void OnEnable()
	{
		this._DoIt();
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x0003C6CA File Offset: 0x0003A8CA
	private void _DoIt()
	{
		if (this._didIt)
		{
			return;
		}
		if (this.newParent != null)
		{
			base.transform.SetParent(this.newParent, true);
		}
		Object.Destroy(this);
	}

	// Token: 0x04000DBA RID: 3514
	public Transform newParent;

	// Token: 0x04000DBB RID: 3515
	private bool _didIt;
}
