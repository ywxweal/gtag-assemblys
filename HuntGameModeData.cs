using System;
using Fusion;

// Token: 0x0200048A RID: 1162
[NetworkBehaviourWeaved(23)]
public class HuntGameModeData : FusionGameModeData
{
	// Token: 0x17000316 RID: 790
	// (get) Token: 0x06001C69 RID: 7273 RVA: 0x0008B07B File Offset: 0x0008927B
	// (set) Token: 0x06001C6A RID: 7274 RVA: 0x0008B088 File Offset: 0x00089288
	public override object Data
	{
		get
		{
			return this.huntdata;
		}
		set
		{
			this.huntdata = (HuntData)value;
		}
	}

	// Token: 0x17000317 RID: 791
	// (get) Token: 0x06001C6B RID: 7275 RVA: 0x0008B096 File Offset: 0x00089296
	// (set) Token: 0x06001C6C RID: 7276 RVA: 0x0008B0C0 File Offset: 0x000892C0
	[Networked]
	[NetworkedWeaved(0, 23)]
	private unsafe HuntData huntdata
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HuntGameModeData.huntdata. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(HuntData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HuntGameModeData.huntdata. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(HuntData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06001C6E RID: 7278 RVA: 0x0008B0EB File Offset: 0x000892EB
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.huntdata = this._huntdata;
	}

	// Token: 0x06001C6F RID: 7279 RVA: 0x0008B103 File Offset: 0x00089303
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._huntdata = this.huntdata;
	}

	// Token: 0x04001F87 RID: 8071
	[WeaverGenerated]
	[DefaultForProperty("huntdata", 0, 23)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private HuntData _huntdata;
}
