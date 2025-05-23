using System;
using UnityEngine;

// Token: 0x0200026F RID: 623
public class ArcadeMachineButton : GorillaPressableButton
{
	// Token: 0x14000028 RID: 40
	// (add) Token: 0x06000E64 RID: 3684 RVA: 0x00048ECC File Offset: 0x000470CC
	// (remove) Token: 0x06000E65 RID: 3685 RVA: 0x00048F04 File Offset: 0x00047104
	public event ArcadeMachineButton.ArcadeMachineButtonEvent OnStateChange;

	// Token: 0x06000E66 RID: 3686 RVA: 0x00048F39 File Offset: 0x00047139
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		if (!this.state)
		{
			this.state = true;
			if (this.OnStateChange != null)
			{
				this.OnStateChange(this.ButtonID, this.state);
			}
		}
	}

	// Token: 0x06000E67 RID: 3687 RVA: 0x00048F70 File Offset: 0x00047170
	private void OnTriggerExit(Collider collider)
	{
		if (!base.enabled || !this.state)
		{
			return;
		}
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		this.state = false;
		if (this.OnStateChange != null)
		{
			this.OnStateChange(this.ButtonID, this.state);
		}
	}

	// Token: 0x040011A8 RID: 4520
	private bool state;

	// Token: 0x040011A9 RID: 4521
	[SerializeField]
	private int ButtonID;

	// Token: 0x02000270 RID: 624
	// (Invoke) Token: 0x06000E6A RID: 3690
	public delegate void ArcadeMachineButtonEvent(int id, bool state);
}
