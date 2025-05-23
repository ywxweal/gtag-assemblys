using System;
using System.Collections;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTagScripts.ModIO
{
	// Token: 0x02000B37 RID: 2871
	public class CustomMapEjectButton : GorillaPressableButton
	{
		// Token: 0x060046B5 RID: 18101 RVA: 0x00150732 File Offset: 0x0014E932
		public override void ButtonActivation()
		{
			base.ButtonActivation();
			base.StartCoroutine(this.ButtonPressed_Local());
			if (!this.processing)
			{
				this.HandleTeleport();
			}
		}

		// Token: 0x060046B6 RID: 18102 RVA: 0x00150755 File Offset: 0x0014E955
		private IEnumerator ButtonPressed_Local()
		{
			this.isOn = true;
			this.UpdateColor();
			yield return new WaitForSeconds(this.debounceTime);
			this.isOn = false;
			this.UpdateColor();
			yield break;
		}

		// Token: 0x060046B7 RID: 18103 RVA: 0x00150764 File Offset: 0x0014E964
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

		// Token: 0x060046B8 RID: 18104 RVA: 0x001507AD File Offset: 0x0014E9AD
		private void FinishTeleport(bool success = true)
		{
			if (!this.processing)
			{
				return;
			}
			this.processing = false;
		}

		// Token: 0x060046B9 RID: 18105 RVA: 0x001507BF File Offset: 0x0014E9BF
		public void CopySettings(CustomMapEjectButtonSettings customMapEjectButtonSettings)
		{
			this.ejectType = (CustomMapEjectButton.EjectType)customMapEjectButtonSettings.ejectType;
		}

		// Token: 0x0400493A RID: 18746
		[SerializeField]
		private CustomMapEjectButton.EjectType ejectType;

		// Token: 0x0400493B RID: 18747
		private bool processing;

		// Token: 0x02000B38 RID: 2872
		public enum EjectType
		{
			// Token: 0x0400493D RID: 18749
			EjectFromVirtualStump,
			// Token: 0x0400493E RID: 18750
			ReturnToVirtualStump
		}
	}
}
