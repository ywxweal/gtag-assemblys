using System;
using UnityEngine;

// Token: 0x02000493 RID: 1171
public class GorillaCameraTriggerIndex : MonoBehaviour
{
	// Token: 0x06001C92 RID: 7314 RVA: 0x0008B6BA File Offset: 0x000898BA
	private void Start()
	{
		this.parentTrigger = base.GetComponentInParent<GorillaCameraSceneTrigger>();
	}

	// Token: 0x06001C93 RID: 7315 RVA: 0x0008B6C8 File Offset: 0x000898C8
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("SceneChanger"))
		{
			this.parentTrigger.mostRecentSceneTrigger = this;
			this.parentTrigger.ChangeScene(this);
		}
	}

	// Token: 0x06001C94 RID: 7316 RVA: 0x0008B6F4 File Offset: 0x000898F4
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("SceneChanger"))
		{
			this.parentTrigger.ChangeScene(this);
		}
	}

	// Token: 0x04001FA7 RID: 8103
	public int sceneTriggerIndex;

	// Token: 0x04001FA8 RID: 8104
	public GorillaCameraSceneTrigger parentTrigger;
}
