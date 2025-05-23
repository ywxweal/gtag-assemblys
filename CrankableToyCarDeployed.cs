using System;
using UnityEngine;

// Token: 0x02000168 RID: 360
public class CrankableToyCarDeployed : MonoBehaviour
{
	// Token: 0x06000906 RID: 2310 RVA: 0x00030C90 File Offset: 0x0002EE90
	public void Deploy(CrankableToyCarHoldable holdable, Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, float lifetime, bool isRemote = false)
	{
		this.holdable = holdable;
		holdable.OnCarDeployed();
		base.transform.position = launchPos;
		base.transform.rotation = launchRot;
		base.transform.localScale = holdable.transform.lossyScale;
		this.rb.velocity = releaseVel;
		this.startedAtTimestamp = Time.time;
		this.expiresAtTimestamp = Time.time + lifetime;
		this.isRemote = isRemote;
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x00030D08 File Offset: 0x0002EF08
	private void Update()
	{
		if (!this.isRemote && Time.time > this.expiresAtTimestamp)
		{
			if (this.holdable != null)
			{
				this.holdable.OnCarReturned();
			}
			return;
		}
		if (!this.wheelDriver.hasCollision)
		{
			this.expiresAtTimestamp -= Time.deltaTime;
			if (!this.offGroundDrivingAudio.isPlaying)
			{
				this.offGroundDrivingAudio.Play();
				this.drivingAudio.Stop();
			}
		}
		else if (!this.drivingAudio.isPlaying)
		{
			this.drivingAudio.Play();
			this.offGroundDrivingAudio.Stop();
		}
		float num = Mathf.InverseLerp(this.startedAtTimestamp, this.expiresAtTimestamp, Time.time);
		float num2 = this.thrustCurve.Evaluate(num);
		this.wheelDriver.SetThrust(this.maxThrust * num2);
	}

	// Token: 0x04000ABC RID: 2748
	[SerializeField]
	private Rigidbody rb;

	// Token: 0x04000ABD RID: 2749
	[SerializeField]
	private FakeWheelDriver wheelDriver;

	// Token: 0x04000ABE RID: 2750
	[SerializeField]
	private Vector3 maxThrust;

	// Token: 0x04000ABF RID: 2751
	[SerializeField]
	private AnimationCurve thrustCurve;

	// Token: 0x04000AC0 RID: 2752
	private float startedAtTimestamp;

	// Token: 0x04000AC1 RID: 2753
	private float expiresAtTimestamp;

	// Token: 0x04000AC2 RID: 2754
	private CrankableToyCarHoldable holdable;

	// Token: 0x04000AC3 RID: 2755
	[SerializeField]
	private AudioSource drivingAudio;

	// Token: 0x04000AC4 RID: 2756
	[SerializeField]
	private AudioSource offGroundDrivingAudio;

	// Token: 0x04000AC5 RID: 2757
	private bool isRemote;
}
