using System;
using Fusion;

// Token: 0x0200048C RID: 1164
[NetworkBehaviourWeaved(12)]
public class TagGameModeData : FusionGameModeData
{
	// Token: 0x1700031A RID: 794
	// (get) Token: 0x06001C73 RID: 7283 RVA: 0x0008B150 File Offset: 0x00089350
	// (set) Token: 0x06001C74 RID: 7284 RVA: 0x0008B15D File Offset: 0x0008935D
	public override object Data
	{
		get
		{
			return this.tagData;
		}
		set
		{
			this.tagData = (TagData)value;
		}
	}

	// Token: 0x1700031B RID: 795
	// (get) Token: 0x06001C75 RID: 7285 RVA: 0x0008B16B File Offset: 0x0008936B
	// (set) Token: 0x06001C76 RID: 7286 RVA: 0x0008B195 File Offset: 0x00089395
	[Networked]
	[NetworkedWeaved(0, 12)]
	private unsafe TagData tagData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing TagGameModeData.tagData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(TagData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing TagGameModeData.tagData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(TagData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06001C78 RID: 7288 RVA: 0x0008B1C0 File Offset: 0x000893C0
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.tagData = this._tagData;
	}

	// Token: 0x06001C79 RID: 7289 RVA: 0x0008B1D8 File Offset: 0x000893D8
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._tagData = this.tagData;
	}

	// Token: 0x04001F8B RID: 8075
	[WeaverGenerated]
	[DefaultForProperty("tagData", 0, 12)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private TagData _tagData;
}
