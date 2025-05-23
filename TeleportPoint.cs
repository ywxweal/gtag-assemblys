using System;
using UnityEngine;

// Token: 0x02000304 RID: 772
public class TeleportPoint : MonoBehaviour
{
	// Token: 0x06001277 RID: 4727 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Start()
	{
	}

	// Token: 0x06001278 RID: 4728 RVA: 0x000575AB File Offset: 0x000557AB
	public Transform GetDestTransform()
	{
		return this.destTransform;
	}

	// Token: 0x06001279 RID: 4729 RVA: 0x000575B4 File Offset: 0x000557B4
	private void Update()
	{
		float num = Mathf.SmoothStep(this.fullIntensity, this.lowIntensity, (Time.time - this.lastLookAtTime) * this.dimmingSpeed);
		base.GetComponent<MeshRenderer>().material.SetFloat("_Intensity", num);
	}

	// Token: 0x0600127A RID: 4730 RVA: 0x000575FC File Offset: 0x000557FC
	public void OnLookAt()
	{
		this.lastLookAtTime = Time.time;
	}

	// Token: 0x04001492 RID: 5266
	public float dimmingSpeed = 1f;

	// Token: 0x04001493 RID: 5267
	public float fullIntensity = 1f;

	// Token: 0x04001494 RID: 5268
	public float lowIntensity = 0.5f;

	// Token: 0x04001495 RID: 5269
	public Transform destTransform;

	// Token: 0x04001496 RID: 5270
	private float lastLookAtTime;
}
