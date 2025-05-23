using System;
using System.Collections;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000747 RID: 1863
public class HalloweenWatcherEyes : MonoBehaviour
{
	// Token: 0x06002E8C RID: 11916 RVA: 0x000E872C File Offset: 0x000E692C
	private void Start()
	{
		this.playersViewCenterCosAngle = Mathf.Cos(this.playersViewCenterAngle * 0.017453292f);
		this.watchMinCosAngle = Mathf.Cos(this.watchMaxAngle * 0.017453292f);
		base.StartCoroutine(this.CheckIfNearPlayer(Random.Range(0f, this.timeBetweenUpdates)));
		base.enabled = false;
	}

	// Token: 0x06002E8D RID: 11917 RVA: 0x000E878B File Offset: 0x000E698B
	private IEnumerator CheckIfNearPlayer(float initialSleep)
	{
		yield return new WaitForSeconds(initialSleep);
		for (;;)
		{
			base.enabled = (base.transform.position - GTPlayer.Instance.transform.position).sqrMagnitude < this.watchRange * this.watchRange;
			if (!base.enabled)
			{
				this.LookNormal();
			}
			yield return new WaitForSeconds(this.timeBetweenUpdates);
		}
		yield break;
	}

	// Token: 0x06002E8E RID: 11918 RVA: 0x000E87A4 File Offset: 0x000E69A4
	private void Update()
	{
		Vector3 normalized = (GTPlayer.Instance.headCollider.transform.position - base.transform.position).normalized;
		if (Vector3.Dot(GTPlayer.Instance.headCollider.transform.forward, -normalized) > this.playersViewCenterCosAngle)
		{
			this.LookNormal();
			this.pretendingToBeNormalUntilTimestamp = Time.time + this.durationToBeNormalWhenPlayerLooks;
		}
		if (this.pretendingToBeNormalUntilTimestamp > Time.time)
		{
			return;
		}
		if (Vector3.Dot(base.transform.forward, normalized) < this.watchMinCosAngle)
		{
			this.LookNormal();
			return;
		}
		Quaternion quaternion = Quaternion.LookRotation(normalized, base.transform.up);
		Quaternion quaternion2 = Quaternion.Lerp(base.transform.rotation, quaternion, this.lerpValue);
		this.leftEye.transform.rotation = quaternion2;
		this.rightEye.transform.rotation = quaternion2;
		if (this.lerpDuration > 0f)
		{
			this.lerpValue = Mathf.MoveTowards(this.lerpValue, 1f, Time.deltaTime / this.lerpDuration);
			return;
		}
		this.lerpValue = 1f;
	}

	// Token: 0x06002E8F RID: 11919 RVA: 0x000E88D2 File Offset: 0x000E6AD2
	private void LookNormal()
	{
		this.leftEye.transform.localRotation = Quaternion.identity;
		this.rightEye.transform.localRotation = Quaternion.identity;
		this.lerpValue = 0f;
	}

	// Token: 0x04003513 RID: 13587
	public float timeBetweenUpdates = 5f;

	// Token: 0x04003514 RID: 13588
	public float watchRange;

	// Token: 0x04003515 RID: 13589
	public float watchMaxAngle;

	// Token: 0x04003516 RID: 13590
	public float lerpDuration = 1f;

	// Token: 0x04003517 RID: 13591
	public float playersViewCenterAngle = 30f;

	// Token: 0x04003518 RID: 13592
	public float durationToBeNormalWhenPlayerLooks = 3f;

	// Token: 0x04003519 RID: 13593
	public GameObject leftEye;

	// Token: 0x0400351A RID: 13594
	public GameObject rightEye;

	// Token: 0x0400351B RID: 13595
	private float playersViewCenterCosAngle;

	// Token: 0x0400351C RID: 13596
	private float watchMinCosAngle;

	// Token: 0x0400351D RID: 13597
	private float pretendingToBeNormalUntilTimestamp;

	// Token: 0x0400351E RID: 13598
	private float lerpValue;
}
