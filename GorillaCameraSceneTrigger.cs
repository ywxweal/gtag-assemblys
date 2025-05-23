using System;
using UnityEngine;

// Token: 0x02000492 RID: 1170
public class GorillaCameraSceneTrigger : MonoBehaviour
{
	// Token: 0x06001C90 RID: 7312 RVA: 0x0008B654 File Offset: 0x00089854
	public void ChangeScene(GorillaCameraTriggerIndex triggerLeft)
	{
		if (triggerLeft == this.currentSceneTrigger || this.currentSceneTrigger == null)
		{
			if (this.mostRecentSceneTrigger != this.currentSceneTrigger)
			{
				this.sceneCamera.SetSceneCamera(this.mostRecentSceneTrigger.sceneTriggerIndex);
				this.currentSceneTrigger = this.mostRecentSceneTrigger;
				return;
			}
			this.currentSceneTrigger = null;
		}
	}

	// Token: 0x04001FA4 RID: 8100
	public GorillaSceneCamera sceneCamera;

	// Token: 0x04001FA5 RID: 8101
	public GorillaCameraTriggerIndex currentSceneTrigger;

	// Token: 0x04001FA6 RID: 8102
	public GorillaCameraTriggerIndex mostRecentSceneTrigger;
}
