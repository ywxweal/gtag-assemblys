using System;
using System.Collections;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D3B RID: 3387
	public class CustomMapTestingScript : GorillaPressableButton
	{
		// Token: 0x060054DE RID: 21726 RVA: 0x0019DD80 File Offset: 0x0019BF80
		public override void ButtonActivation()
		{
			base.ButtonActivation();
			base.StartCoroutine(this.ButtonPressed_Local());
		}

		// Token: 0x060054DF RID: 21727 RVA: 0x0019DD95 File Offset: 0x0019BF95
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
