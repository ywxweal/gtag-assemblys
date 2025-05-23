using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.UI.ModIO
{
	// Token: 0x02000B2F RID: 2863
	public class VirtualStumpTeleportingHUD : MonoBehaviour
	{
		// Token: 0x0600467D RID: 18045 RVA: 0x0014F17C File Offset: 0x0014D37C
		public void Initialize(bool isEntering)
		{
			this.isEnteringVirtualStump = isEntering;
			if (isEntering)
			{
				this.teleportingStatusText.text = this.enteringVirtualStumpString;
				this.teleportingStatusText.gameObject.SetActive(true);
				return;
			}
			this.teleportingStatusText.text = this.leavingVirtualStumpString;
			this.teleportingStatusText.gameObject.SetActive(true);
		}

		// Token: 0x0600467E RID: 18046 RVA: 0x0014F1D8 File Offset: 0x0014D3D8
		private void Update()
		{
			if (Time.time - this.lastTextUpdateTime > this.textUpdateInterval)
			{
				this.lastTextUpdateTime = Time.time;
				this.IncrementProgressDots();
				this.teleportingStatusText.text = (this.isEnteringVirtualStump ? this.enteringVirtualStumpString : this.leavingVirtualStumpString);
				for (int i = 0; i < this.numProgressDots; i++)
				{
					TMP_Text tmp_Text = this.teleportingStatusText;
					tmp_Text.text += ".";
				}
			}
		}

		// Token: 0x0600467F RID: 18047 RVA: 0x0014F257 File Offset: 0x0014D457
		private void IncrementProgressDots()
		{
			this.numProgressDots++;
			if (this.numProgressDots > this.maxNumProgressDots)
			{
				this.numProgressDots = 0;
			}
		}

		// Token: 0x0400490D RID: 18701
		[SerializeField]
		private string enteringVirtualStumpString = "Now Entering the Virtual Stump";

		// Token: 0x0400490E RID: 18702
		[SerializeField]
		private string leavingVirtualStumpString = "Now Leaving the Virtual Stump";

		// Token: 0x0400490F RID: 18703
		[SerializeField]
		private TMP_Text teleportingStatusText;

		// Token: 0x04004910 RID: 18704
		[SerializeField]
		private int maxNumProgressDots = 3;

		// Token: 0x04004911 RID: 18705
		[SerializeField]
		private float textUpdateInterval = 0.5f;

		// Token: 0x04004912 RID: 18706
		private float lastTextUpdateTime;

		// Token: 0x04004913 RID: 18707
		private int numProgressDots;

		// Token: 0x04004914 RID: 18708
		private bool isEnteringVirtualStump;
	}
}
