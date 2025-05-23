using System;
using System.Collections;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTagScripts.ModIO
{
	// Token: 0x02000B37 RID: 2871
	public class CustomMapEjectButton : GorillaPressableButton
	{
		// Token: 0x060046B6 RID: 18102 RVA: 0x0015080A File Offset: 0x0014EA0A
		public override void ButtonActivation()
		{
			base.ButtonActivation();
			base.StartCoroutine(this.ButtonPressed_Local());
			if (!this.processing)
			{
				this.HandleTeleport();
			}
		}

		// Token: 0x060046B7 RID: 18103 RVA: 0x0015082D File Offset: 0x0014EA2D
		private IEnumerator ButtonPressed_Local()
		{
			this.isOn = true;
			this.UpdateColor();
			yield return new WaitForSeconds(this.debounceTime);
			this.isOn = false;
			this.UpdateColor();
			yield break;
		}

		// Token: 0x060046B8 RID: 18104 RVA: 0x0015083C File Offset: 0x0014EA3C
		private void HandleTeleport()
		{
			if (this.processing)
			{
				return;
			}
			this.processing = true;
			CustomMapEjectButton.EjectType ejectType = this.ejectType;
			if (ejectType != CustomMapEjectButton.EjectType.EjectFromVirtualStump)
			{
				if (ejectType == CustomMapEjectButton.EjectType.ReturnToVirtualStump)
				{
					CustomMapManager.ReturnToVirtualStump();
					this.processing = false;
					return;
				}
			}
			else
			{
				CustomMapManager.ExitVirtualStump(new Action<bool>(this.FinishTeleport));
			}
		}

		// Token: 0x060046B9 RID: 18105 RVA: 0x00150885 File Offset: 0x0014EA85
		private void FinishTeleport(bool success = true)
		{
			if (!this.processing)
			{
				return;
			}
			this.processing = false;
		}

		// Token: 0x060046BA RID: 18106 RVA: 0x00150897 File Offset: 0x0014EA97
		public void CopySettings(CustomMapEjectButtonSettings customMapEjectButtonSettings)
		{
			this.ejectType = (CustomMapEjectButton.EjectType)customMapEjectButtonSettings.ejectType;
		}

		// Token: 0x0400493B RID: 18747
		[SerializeField]
		private CustomMapEjectButton.EjectType ejectType;

		// Token: 0x0400493C RID: 18748
		private bool processing;

		// Token: 0x02000B38 RID: 2872
		public enum EjectType
		{
			// Token: 0x0400493E RID: 18750
			EjectFromVirtualStump,
			// Token: 0x0400493F RID: 18751
			ReturnToVirtualStump
		}
	}
}
