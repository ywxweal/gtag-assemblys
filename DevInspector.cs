using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001C0 RID: 448
public class DevInspector : MonoBehaviour
{
	// Token: 0x06000AA7 RID: 2727 RVA: 0x0003A34F File Offset: 0x0003854F
	private void OnEnable()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04000D26 RID: 3366
	public GameObject pivot;

	// Token: 0x04000D27 RID: 3367
	public Text outputInfo;

	// Token: 0x04000D28 RID: 3368
	public Component[] componentToInspect;

	// Token: 0x04000D29 RID: 3369
	public bool isEnabled;

	// Token: 0x04000D2A RID: 3370
	public bool autoFind = true;

	// Token: 0x04000D2B RID: 3371
	public GameObject canvas;

	// Token: 0x04000D2C RID: 3372
	public int sidewaysOffset;
}
