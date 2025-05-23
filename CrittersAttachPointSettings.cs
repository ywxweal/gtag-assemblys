using System;

// Token: 0x02000048 RID: 72
public class CrittersAttachPointSettings : CrittersActorSettings
{
	// Token: 0x0600015E RID: 350 RVA: 0x0000924D File Offset: 0x0000744D
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersAttachPoint crittersAttachPoint = (CrittersAttachPoint)this.parentActor;
		crittersAttachPoint.anchorLocation = this.anchoredLocation;
		crittersAttachPoint.rb.isKinematic = true;
		crittersAttachPoint.isLeft = this.isLeft;
	}

	// Token: 0x0400018C RID: 396
	public bool isLeft;

	// Token: 0x0400018D RID: 397
	public CrittersAttachPoint.AnchoredLocationTypes anchoredLocation;
}
