using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020000C6 RID: 198
public class TempMask : MonoBehaviour
{
	// Token: 0x060004E4 RID: 1252 RVA: 0x0001C704 File Offset: 0x0001A904
	private void Awake()
	{
		this.dayOn = new DateTime(this.year, this.month, this.day);
		this.myRig = base.GetComponentInParent<VRRig>();
		if (this.myRig != null && this.myRig.netView.IsMine && !this.myRig.isOfflineVRRig)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060004E5 RID: 1253 RVA: 0x0001C772 File Offset: 0x0001A972
	private void OnEnable()
	{
		base.StartCoroutine(this.MaskOnDuringDate());
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x00004F01 File Offset: 0x00003101
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x0001C781 File Offset: 0x0001A981
	private IEnumerator MaskOnDuringDate()
	{
		for (;;)
		{
			if (GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
			{
				this.myDate = new DateTime(GorillaComputer.instance.startupMillis * 10000L + (long)(Time.realtimeSinceStartup * 1000f * 10000f)).Subtract(TimeSpan.FromHours(7.0));
				if (this.myDate.DayOfYear == this.dayOn.DayOfYear)
				{
					if (!this.myRenderer.enabled)
					{
						this.myRenderer.enabled = true;
					}
				}
				else if (this.myRenderer.enabled)
				{
					this.myRenderer.enabled = false;
				}
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x040005C7 RID: 1479
	public int year;

	// Token: 0x040005C8 RID: 1480
	public int month;

	// Token: 0x040005C9 RID: 1481
	public int day;

	// Token: 0x040005CA RID: 1482
	public DateTime dayOn;

	// Token: 0x040005CB RID: 1483
	public MeshRenderer myRenderer;

	// Token: 0x040005CC RID: 1484
	private DateTime myDate;

	// Token: 0x040005CD RID: 1485
	private VRRig myRig;
}
