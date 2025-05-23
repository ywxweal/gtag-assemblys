using System;
using UnityEngine;

// Token: 0x020001E1 RID: 481
[DefaultExecutionOrder(-1000)]
public class HierarchyFlattenerRemoveXform : MonoBehaviour
{
	// Token: 0x06000B43 RID: 2883 RVA: 0x0003C62E File Offset: 0x0003A82E
	protected void Awake()
	{
		this._DoIt();
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x0003C638 File Offset: 0x0003A838
	private void _DoIt()
	{
		if (this._didIt)
		{
			return;
		}
		if (base.GetComponentInChildren<HierarchyFlattenerRemoveXform>(true) != null)
		{
			return;
		}
		HierarchyFlattenerRemoveXform componentInParent = base.GetComponentInParent<HierarchyFlattenerRemoveXform>(true);
		this._didIt = true;
		Transform transform = base.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).SetParent(transform.parent, true);
		}
		Object.Destroy(base.gameObject);
		if (componentInParent != null)
		{
			componentInParent._DoIt();
		}
	}

	// Token: 0x04000DB9 RID: 3513
	private bool _didIt;
}
