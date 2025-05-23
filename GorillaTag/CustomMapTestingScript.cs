using System;
using System.Collections;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D3B RID: 3387
	public class CustomMapTestingScript : GorillaPressableButton
	{
		// Token: 0x060054DD RID: 21725 RVA: 0x0019DCA8 File Offset: 0x0019BEA8
		public override void ButtonActivation()
		{
			base.ButtonActivation();
			base.StartCoroutine(this.ButtonPressed_Local());
		}

		// Token: 0x060054DE RID: 21726 RVA: 0x0019DCBD File Offset: 0x0019BEBD
		private IEnumerator ButtonPressed_Local()
		{
			this.isOn = true;
			this.UpdateColor();
			yield return new WaitForSeconds(this.debounceTime);
			this.isOn = false;
			this.UpdateColor();
			yield break;
		}
	}
}
