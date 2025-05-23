using System;
using Fusion;

// Token: 0x02000487 RID: 1159
[NetworkBehaviourWeaved(1)]
public class CasualGameModeData : FusionGameModeData
{
	// Token: 0x17000311 RID: 785
	// (get) Token: 0x06001C5B RID: 7259 RVA: 0x0008AF9B File Offset: 0x0008919B
	// (set) Token: 0x06001C5C RID: 7260 RVA: 0x000023F4 File Offset: 0x000005F4
	public override object Data
	{
		get
		{
			return this.casualData;
		}
		set
		{
		}
	}

	// Token: 0x17000312 RID: 786
	// (get) Token: 0x06001C5D RID: 7261 RVA: 0x0008AFA8 File Offset: 0x000891A8
	// (set) Token: 0x06001C5E RID: 7262 RVA: 0x0008AFD2 File Offset: 0x000891D2
	[Networked]
	[NetworkedWeaved(0, 1)]
	private unsafe CasualData casualData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CasualGameModeData.casualData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(CasualData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CasualGameModeData.casualData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(CasualData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06001C60 RID: 7264 RVA: 0x0008AFFD File Offset: 0x000891FD
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.casualData = this._casualData;
	}

	// Token: 0x06001C61 RID: 7265 RVA: 0x0008B015 File Offset: 0x00089215
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._casualData = this.casualData;
	}

	// Token: 0x04001F80 RID: 8064
	[WeaverGenerated]
	[DefaultForProperty("casualData", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private CasualData _casualData;
}
