using System;
using UnityEngine;

// Token: 0x0200042D RID: 1069
public class TransferrableObjectGripPosition : MonoBehaviour
{
	// Token: 0x06001A61 RID: 6753 RVA: 0x00081B67 File Offset: 0x0007FD67
	private void Awake()
	{
		if (this.parentObject == null)
		{
			this.parentObject = base.transform.parent.GetComponent<TransferrableItemSlotTransformOverride>();
		}
		this.parentObject.AddGripPosition(this.attachmentType, this);
	}

	// Token: 0x06001A62 RID: 6754 RVA: 0x00081B9F File Offset: 0x0007FD9F
	public SubGrabPoint CreateSubGrabPoint(SlotTransformOverride overrideContainer)
	{
		return new SubGrabPoint();
	}

	// Token: 0x04001D7E RID: 7550
	[SerializeField]
	private TransferrableItemSlotTransformOverride parentObject;

	// Token: 0x04001D7F RID: 7551
	[SerializeField]
	private TransferrableObject.PositionState attachmentType;
}
