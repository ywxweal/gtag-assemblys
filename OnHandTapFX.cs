using System;
using UnityEngine;

// Token: 0x020005EF RID: 1519
internal struct OnHandTapFX : IFXEffectContext<HandEffectContext>
{
	// Token: 0x1700038F RID: 911
	// (get) Token: 0x06002557 RID: 9559 RVA: 0x000BA510 File Offset: 0x000B8710
	public HandEffectContext effectContext
	{
		get
		{
			if (!this.isLeftHand)
			{
				return this.rig.GetRightHandEffect(this.surfaceIndex, this.volume, this.tapDir);
			}
			return this.rig.GetLeftHandEffect(this.surfaceIndex, this.volume, this.tapDir);
		}
	}

	// Token: 0x17000390 RID: 912
	// (get) Token: 0x06002558 RID: 9560 RVA: 0x000BA560 File Offset: 0x000B8760
	public FXSystemSettings settings
	{
		get
		{
			return this.rig.fxSettings;
		}
	}

	// Token: 0x04002A04 RID: 10756
	public VRRig rig;

	// Token: 0x04002A05 RID: 10757
	public Vector3 tapDir;

	// Token: 0x04002A06 RID: 10758
	public bool isLeftHand;

	// Token: 0x04002A07 RID: 10759
	public int surfaceIndex;

	// Token: 0x04002A08 RID: 10760
	public float volume;
}
