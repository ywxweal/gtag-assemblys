using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000171 RID: 369
public class DJScratchtable : MonoBehaviour
{
	// Token: 0x06000942 RID: 2370 RVA: 0x00031EEB File Offset: 0x000300EB
	public void SetPlaying(bool playing)
	{
		this.isPlaying = playing;
	}

	// Token: 0x06000943 RID: 2371 RVA: 0x00031EF4 File Offset: 0x000300F4
	private void OnTriggerStay(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator componentInParent = collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent == null)
		{
			return;
		}
		Vector3 vector = (base.transform.parent.InverseTransformPoint(collider.transform.position) - base.transform.localPosition).WithY(0f);
		float num = Mathf.Atan2(vector.z, vector.x) * 57.29578f;
		if (this.isTouching)
		{
			base.transform.localRotation = Quaternion.LookRotation(vector) * this.firstTouchRotation;
			if (this.isPlaying)
			{
				float num2 = Mathf.DeltaAngle(this.lastScratchSoundAngle, num);
				if (num2 > this.scratchMinAngle)
				{
					if (Time.time > this.cantForwardScratchUntilTimestamp)
					{
						this.scratchPlayer.Play(ScratchSoundType.Forward, this.isLeft);
						this.cantForwardScratchUntilTimestamp = Time.time + this.scratchCooldown;
						this.lastScratchSoundAngle = num;
						GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, this.hapticStrength, this.hapticDuration);
					}
				}
				else if (num2 < -this.scratchMinAngle && Time.time > this.cantBackScratchUntilTimestamp)
				{
					this.scratchPlayer.Play(ScratchSoundType.Back, this.isLeft);
					this.cantBackScratchUntilTimestamp = Time.time + this.scratchCooldown;
					this.lastScratchSoundAngle = num;
					GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, this.hapticStrength, this.hapticDuration);
				}
			}
		}
		else
		{
			this.firstTouchRotation = Quaternion.Inverse(Quaternion.LookRotation(base.transform.InverseTransformPoint(collider.transform.position).WithY(0f)));
			if (this.isPlaying)
			{
				this.PauseTrack();
				this.scratchPlayer.Play(ScratchSoundType.Pause, this.isLeft);
				this.lastScratchSoundAngle = num;
				this.cantForwardScratchUntilTimestamp = Time.time + this.scratchCooldown;
				this.cantBackScratchUntilTimestamp = Time.time + this.scratchCooldown;
			}
		}
		this.isTouching = true;
	}

	// Token: 0x06000944 RID: 2372 RVA: 0x000320F8 File Offset: 0x000302F8
	private void OnTriggerExit(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		if (this.isPlaying)
		{
			this.ResumeTrack();
			this.scratchPlayer.Play(ScratchSoundType.Resume, this.isLeft);
		}
		this.isTouching = false;
	}

	// Token: 0x06000945 RID: 2373 RVA: 0x00032144 File Offset: 0x00030344
	public void SelectTrack(int track)
	{
		this.lastSelectedTrack = track;
		if (track == 0)
		{
			this.turntableVisual.Stop();
			this.isPlaying = false;
		}
		else
		{
			this.turntableVisual.Run();
			this.isPlaying = true;
		}
		int num = track - 1;
		for (int i = 0; i < this.tracks.Length; i++)
		{
			if (num == i)
			{
				float num2 = (float)(PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)) % this.trackDuration;
				this.tracks[i].Play();
				this.tracks[i].time = num2;
			}
			else
			{
				this.tracks[i].Stop();
			}
		}
	}

	// Token: 0x06000946 RID: 2374 RVA: 0x000321E4 File Offset: 0x000303E4
	public void PauseTrack()
	{
		for (int i = 0; i < this.tracks.Length; i++)
		{
			this.tracks[i].Stop();
		}
		this.pausedUntilTimestamp = Time.time + 1f;
	}

	// Token: 0x06000947 RID: 2375 RVA: 0x00032222 File Offset: 0x00030422
	public void ResumeTrack()
	{
		this.SelectTrack(this.lastSelectedTrack);
		this.pausedUntilTimestamp = 0f;
	}

	// Token: 0x04000B14 RID: 2836
	[SerializeField]
	private bool isLeft;

	// Token: 0x04000B15 RID: 2837
	[SerializeField]
	private DJScratchSoundPlayer scratchPlayer;

	// Token: 0x04000B16 RID: 2838
	[SerializeField]
	private float scratchCooldown;

	// Token: 0x04000B17 RID: 2839
	[SerializeField]
	private float scratchMinAngle;

	// Token: 0x04000B18 RID: 2840
	[SerializeField]
	private AudioSource[] tracks;

	// Token: 0x04000B19 RID: 2841
	[SerializeField]
	private CosmeticFan turntableVisual;

	// Token: 0x04000B1A RID: 2842
	[SerializeField]
	private float trackDuration;

	// Token: 0x04000B1B RID: 2843
	[SerializeField]
	private float hapticStrength;

	// Token: 0x04000B1C RID: 2844
	[SerializeField]
	private float hapticDuration;

	// Token: 0x04000B1D RID: 2845
	private int lastSelectedTrack;

	// Token: 0x04000B1E RID: 2846
	private bool isPlaying;

	// Token: 0x04000B1F RID: 2847
	private bool isTouching;

	// Token: 0x04000B20 RID: 2848
	private Quaternion firstTouchRotation;

	// Token: 0x04000B21 RID: 2849
	private float lastScratchSoundAngle;

	// Token: 0x04000B22 RID: 2850
	private float cantForwardScratchUntilTimestamp;

	// Token: 0x04000B23 RID: 2851
	private float cantBackScratchUntilTimestamp;

	// Token: 0x04000B24 RID: 2852
	private float pausedUntilTimestamp;
}
