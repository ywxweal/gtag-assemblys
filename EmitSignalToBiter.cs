using System;
using UnityEngine;

// Token: 0x0200053F RID: 1343
public class EmitSignalToBiter : GTSignalEmitter
{
	// Token: 0x06002091 RID: 8337 RVA: 0x000A3540 File Offset: 0x000A1740
	public override void Emit()
	{
		if (this.onEdibleState == EmitSignalToBiter.EdibleState.None)
		{
			return;
		}
		if (!this.targetEdible)
		{
			return;
		}
		if (this.targetEdible.lastBiterActorID == -1)
		{
			return;
		}
		TransferrableObject.ItemStates itemState = this.targetEdible.itemState;
		if (itemState - TransferrableObject.ItemStates.State0 <= 1 || itemState == TransferrableObject.ItemStates.State2 || itemState == TransferrableObject.ItemStates.State3)
		{
			int num = (int)itemState;
			if ((this.onEdibleState & (EmitSignalToBiter.EdibleState)num) == (EmitSignalToBiter.EdibleState)num)
			{
				GTSignal.Emit(this.targetEdible.lastBiterActorID, this.signal, Array.Empty<object>());
			}
		}
	}

	// Token: 0x06002092 RID: 8338 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void Emit(int targetActor)
	{
	}

	// Token: 0x06002093 RID: 8339 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void Emit(params object[] data)
	{
	}

	// Token: 0x04002488 RID: 9352
	[Space]
	public EdibleHoldable targetEdible;

	// Token: 0x04002489 RID: 9353
	[Space]
	[SerializeField]
	private EmitSignalToBiter.EdibleState onEdibleState;

	// Token: 0x02000540 RID: 1344
	[Flags]
	private enum EdibleState
	{
		// Token: 0x0400248B RID: 9355
		None = 0,
		// Token: 0x0400248C RID: 9356
		State0 = 1,
		// Token: 0x0400248D RID: 9357
		State1 = 2,
		// Token: 0x0400248E RID: 9358
		State2 = 4,
		// Token: 0x0400248F RID: 9359
		State3 = 8
	}
}
