using System;
using UnityEngine;

// Token: 0x02000330 RID: 816
public class Flashlight : MonoBehaviour
{
	// Token: 0x06001353 RID: 4947 RVA: 0x0005C284 File Offset: 0x0005A484
	private void LateUpdate()
	{
		for (int i = 0; i < this.lightVolume.transform.childCount; i++)
		{
			this.lightVolume.transform.GetChild(i).rotation = Quaternion.LookRotation((this.lightVolume.transform.GetChild(i).position - Camera.main.transform.position).normalized);
		}
	}

	// Token: 0x06001354 RID: 4948 RVA: 0x0005C2FC File Offset: 0x0005A4FC
	public void ToggleFlashlight()
	{
		this.lightVolume.SetActive(!this.lightVolume.activeSelf);
		this.spotlight.enabled = !this.spotlight.enabled;
		this.bulbGlow.SetActive(this.lightVolume.activeSelf);
	}

	// Token: 0x06001355 RID: 4949 RVA: 0x0005C351 File Offset: 0x0005A551
	public void EnableFlashlight(bool doEnable)
	{
		this.lightVolume.SetActive(doEnable);
		this.spotlight.enabled = doEnable;
		this.bulbGlow.SetActive(doEnable);
	}

	// Token: 0x04001570 RID: 5488
	public GameObject lightVolume;

	// Token: 0x04001571 RID: 5489
	public Light spotlight;

	// Token: 0x04001572 RID: 5490
	public GameObject bulbGlow;
}
