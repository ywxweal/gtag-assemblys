using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x020009E3 RID: 2531
public class SnapTurnOverrideOnEnable : MonoBehaviour, ISnapTurnOverride
{
	// Token: 0x06003C8D RID: 15501 RVA: 0x001210F0 File Offset: 0x0011F2F0
	private void OnEnable()
	{
		if (this.snapTurn == null && GorillaTagger.Instance != null)
		{
			this.snapTurn = GorillaTagger.Instance.GetComponent<GorillaSnapTurn>();
		}
		if (this.snapTurn != null)
		{
			this.snapTurnOverride = true;
			this.snapTurn.SetTurningOverride(this);
		}
	}

	// Token: 0x06003C8E RID: 15502 RVA: 0x00121149 File Offset: 0x0011F349
	private void OnDisable()
	{
		if (this.snapTurnOverride)
		{
			this.snapTurnOverride = false;
			this.snapTurn.UnsetTurningOverride(this);
		}
	}

	// Token: 0x06003C8F RID: 15503 RVA: 0x00121166 File Offset: 0x0011F366
	bool ISnapTurnOverride.TurnOverrideActive()
	{
		return this.snapTurnOverride;
	}

	// Token: 0x04004079 RID: 16505
	private GorillaSnapTurn snapTurn;

	// Token: 0x0400407A RID: 16506
	private bool snapTurnOverride;
}
