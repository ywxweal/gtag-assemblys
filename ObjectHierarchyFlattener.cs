using System;
using UnityEngine;

// Token: 0x020001EC RID: 492
public class ObjectHierarchyFlattener : MonoBehaviour
{
	// Token: 0x06000B63 RID: 2915 RVA: 0x0003D0A0 File Offset: 0x0003B2A0
	private void ResetTransform()
	{
		base.transform.SetParent(this.originalParentTransform);
		base.transform.localPosition = this.originalLocalPosition;
		base.transform.localRotation = this.originalLocalRotation;
		base.transform.localScale = this.originalScale;
		if (this.crumb != null)
		{
			Object.Destroy(this.crumb);
		}
	}

	// Token: 0x06000B64 RID: 2916 RVA: 0x0003D10C File Offset: 0x0003B30C
	public void InvokeLateUpdate()
	{
		if (!this.originalParentGO.activeInHierarchy)
		{
			ObjectHierarchyFlattenerManager.UnregisterOHF(this);
			base.Invoke("ResetTransform", 0f);
			return;
		}
		if (!this.trackTransformOfParent)
		{
			return;
		}
		if (this.maintainRelativeScale)
		{
			base.transform.localScale = Vector3.Scale(this.originalParentTransform.lossyScale, this.originalScale);
		}
		base.transform.rotation = this.originalParentTransform.rotation * this.originalLocalRotation;
		base.transform.position = this.originalParentTransform.position + base.transform.rotation * this.calcOffset * (this.originalParentTransform.lossyScale.x / this.originalParentScale) * this.originalParentScale;
	}

	// Token: 0x06000B65 RID: 2917 RVA: 0x0003D1E8 File Offset: 0x0003B3E8
	private void OnEnable()
	{
		ObjectHierarchyFlattenerManager.RegisterOHF(this);
		this.originalParentTransform = base.transform.parent;
		this.originalParentGO = this.originalParentTransform.gameObject;
		this.originalLocalPosition = base.transform.localPosition;
		this.originalLocalRotation = base.transform.localRotation;
		this.originalParentScale = base.transform.parent.lossyScale.x;
		this.originalScale = base.transform.localScale;
		this.calcOffset = Vector3.Scale(this.originalLocalPosition, this.originalScale);
		if (this.originalParentGO.GetComponent<FlattenerCrumb>() == null)
		{
			this.crumb = this.originalParentGO.AddComponent<FlattenerCrumb>();
		}
		base.transform.SetParent(null);
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x0003D2B2 File Offset: 0x0003B4B2
	private void OnDisable()
	{
		ObjectHierarchyFlattenerManager.UnregisterOHF(this);
		base.Invoke("ResetTransform", 0f);
	}

	// Token: 0x04000DFE RID: 3582
	private GameObject originalParentGO;

	// Token: 0x04000DFF RID: 3583
	private Transform originalParentTransform;

	// Token: 0x04000E00 RID: 3584
	private Vector3 originalLocalPosition;

	// Token: 0x04000E01 RID: 3585
	private Vector3 calcOffset;

	// Token: 0x04000E02 RID: 3586
	private Quaternion originalLocalRotation;

	// Token: 0x04000E03 RID: 3587
	private Vector3 originalScale;

	// Token: 0x04000E04 RID: 3588
	private float originalParentScale;

	// Token: 0x04000E05 RID: 3589
	public bool trackTransformOfParent;

	// Token: 0x04000E06 RID: 3590
	public bool maintainRelativeScale;

	// Token: 0x04000E07 RID: 3591
	private FlattenerCrumb crumb;
}
