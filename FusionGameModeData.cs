using System;
using Fusion;

// Token: 0x02000488 RID: 1160
[NetworkBehaviourWeaved(0)]
public abstract class FusionGameModeData : NetworkBehaviour
{
	// Token: 0x17000313 RID: 787
	// (get) Token: 0x06001C62 RID: 7266
	// (set) Token: 0x06001C63 RID: 7267
	public abstract object Data { get; set; }

	// Token: 0x06001C65 RID: 7269 RVA: 0x000023F4 File Offset: 0x000005F4
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
	}

	// Token: 0x06001C66 RID: 7270 RVA: 0x000023F4 File Offset: 0x000005F4
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
	}

	// Token: 0x04001F81 RID: 8065
	protected INetworkStruct data;
}
