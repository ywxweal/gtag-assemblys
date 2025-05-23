using System;
using System.Collections.Generic;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000B32 RID: 2866
	public class CMSObjectActivationTrigger : CMSTrigger
	{
		// Token: 0x0600468A RID: 18058 RVA: 0x0014F464 File Offset: 0x0014D664
		public override void CopyTriggerSettings(TriggerSettings settings)
		{
			if (settings.GetType() == typeof(ObjectActivationTriggerSettings))
			{
				ObjectActivationTriggerSettings objectActivationTriggerSettings = (ObjectActivationTriggerSettings)settings;
				this.objectsToActivate = objectActivationTriggerSettings.objectsToActivate;
				this.objectsToDeactivate = objectActivationTriggerSettings.objectsToDeactivate;
				this.triggersToReset = objectActivationTriggerSettings.triggersToReset;
				this.onlyResetTriggerCount = objectActivationTriggerSettings.onlyResetTriggerCount;
			}
			for (int i = this.objectsToActivate.Count - 1; i >= 0; i--)
			{
				if (this.objectsToActivate[i] == null)
				{
					this.objectsToActivate.RemoveAt(i);
				}
			}
			for (int j = this.objectsToDeactivate.Count - 1; j >= 0; j--)
			{
				if (this.objectsToDeactivate[j] == null)
				{
					this.objectsToDeactivate.RemoveAt(j);
				}
			}
			for (int k = this.triggersToReset.Count - 1; k >= 0; k--)
			{
				if (this.triggersToReset[k] == null)
				{
					this.triggersToReset.RemoveAt(k);
				}
			}
			base.CopyTriggerSettings(settings);
		}

		// Token: 0x0600468B RID: 18059 RVA: 0x0014F570 File Offset: 0x0014D770
		public override void Trigger(double triggerTime = -1.0, bool originatedLocally = false, bool ignoreTriggerCount = false)
		{
			base.Trigger(triggerTime, originatedLocally, ignoreTriggerCount);
			foreach (GameObject gameObject in this.objectsToDeactivate)
			{
				if (gameObject.IsNotNull())
				{
					gameObject.SetActive(false);
				}
			}
			foreach (GameObject gameObject2 in this.objectsToActivate)
			{
				if (gameObject2.IsNotNull())
				{
					gameObject2.SetActive(true);
				}
			}
			foreach (GameObject gameObject3 in this.triggersToReset)
			{
				if (!gameObject3.IsNull())
				{
					CMSTrigger component = gameObject3.GetComponent<CMSTrigger>();
					if (component.IsNotNull())
					{
						component.ResetTrigger(this.onlyResetTriggerCount);
					}
				}
			}
		}

		// Token: 0x04004919 RID: 18713
		public List<GameObject> objectsToActivate = new List<GameObject>();

		// Token: 0x0400491A RID: 18714
		public List<GameObject> objectsToDeactivate = new List<GameObject>();

		// Token: 0x0400491B RID: 18715
		public List<GameObject> triggersToReset = new List<GameObject>();

		// Token: 0x0400491C RID: 18716
		public bool onlyResetTriggerCount;
	}
}
