using System;
using UnityEngine;

// Token: 0x02000196 RID: 406
public class PartyInABox : MonoBehaviour
{
	// Token: 0x06000A05 RID: 2565 RVA: 0x00034DBE File Offset: 0x00032FBE
	private void Awake()
	{
		this.Reset();
	}

	// Token: 0x06000A06 RID: 2566 RVA: 0x00034DBE File Offset: 0x00032FBE
	private void OnEnable()
	{
		this.Reset();
	}

	// Token: 0x06000A07 RID: 2567 RVA: 0x00034DC6 File Offset: 0x00032FC6
	public void Cranked_ReleaseParty()
	{
		if (!this.parentHoldable.IsLocalObject())
		{
			return;
		}
		this.ReleaseParty();
	}

	// Token: 0x06000A08 RID: 2568 RVA: 0x00034DDC File Offset: 0x00032FDC
	public void ReleaseParty()
	{
		if (this.isReleased)
		{
			return;
		}
		if (this.parentHoldable.IsLocalObject())
		{
			this.parentHoldable.itemState |= TransferrableObject.ItemStates.State0;
			GorillaTagger.Instance.StartVibration(true, this.partyHapticStrength, this.partyHapticDuration);
			GorillaTagger.Instance.StartVibration(false, this.partyHapticStrength, this.partyHapticDuration);
		}
		this.isReleased = true;
		this.spring.enabled = true;
		this.anim.Play();
		this.particles.Play();
		this.partyAudio.Play();
	}

	// Token: 0x06000A09 RID: 2569 RVA: 0x00034E78 File Offset: 0x00033078
	private void Update()
	{
		if (this.parentHoldable.IsLocalObject())
		{
			return;
		}
		if (this.parentHoldable.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			if (!this.isReleased)
			{
				this.ReleaseParty();
				return;
			}
		}
		else if (this.isReleased)
		{
			this.Reset();
		}
	}

	// Token: 0x06000A0A RID: 2570 RVA: 0x00034ED0 File Offset: 0x000330D0
	public void Reset()
	{
		this.isReleased = false;
		this.parentHoldable.itemState &= (TransferrableObject.ItemStates)(-2);
		this.spring.enabled = false;
		this.anim.Stop();
		foreach (PartyInABox.ForceTransform forceTransform in this.forceTransforms)
		{
			forceTransform.Apply();
		}
	}

	// Token: 0x04000C22 RID: 3106
	[SerializeField]
	private TransferrableObject parentHoldable;

	// Token: 0x04000C23 RID: 3107
	[SerializeField]
	private ParticleSystem particles;

	// Token: 0x04000C24 RID: 3108
	[SerializeField]
	private Animation anim;

	// Token: 0x04000C25 RID: 3109
	[SerializeField]
	private SpringyWobbler spring;

	// Token: 0x04000C26 RID: 3110
	[SerializeField]
	private AudioSource partyAudio;

	// Token: 0x04000C27 RID: 3111
	[SerializeField]
	private float partyHapticStrength;

	// Token: 0x04000C28 RID: 3112
	[SerializeField]
	private float partyHapticDuration;

	// Token: 0x04000C29 RID: 3113
	private bool isReleased;

	// Token: 0x04000C2A RID: 3114
	[SerializeField]
	private PartyInABox.ForceTransform[] forceTransforms;

	// Token: 0x02000197 RID: 407
	[Serializable]
	private struct ForceTransform
	{
		// Token: 0x06000A0C RID: 2572 RVA: 0x00034F33 File Offset: 0x00033133
		public void Apply()
		{
			this.transform.localPosition = this.localPosition;
			this.transform.localRotation = this.localRotation;
		}

		// Token: 0x04000C2B RID: 3115
		public Transform transform;

		// Token: 0x04000C2C RID: 3116
		public Vector3 localPosition;

		// Token: 0x04000C2D RID: 3117
		public Quaternion localRotation;
	}
}
