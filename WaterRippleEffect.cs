using System;
using GorillaLocomotion.Swimming;
using UnityEngine;

// Token: 0x02000230 RID: 560
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class WaterRippleEffect : MonoBehaviour
{
	// Token: 0x06000CF2 RID: 3314 RVA: 0x000443BD File Offset: 0x000425BD
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.renderer = base.GetComponent<SpriteRenderer>();
		this.ripplePlaybackSpeedHash = Animator.StringToHash(this.ripplePlaybackSpeedName);
	}

	// Token: 0x06000CF3 RID: 3315 RVA: 0x000443E8 File Offset: 0x000425E8
	public void Destroy()
	{
		this.waterVolume = null;
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x06000CF4 RID: 3316 RVA: 0x00044404 File Offset: 0x00042604
	public void PlayEffect(WaterVolume volume = null)
	{
		this.waterVolume = volume;
		this.rippleStartTime = Time.time;
		this.animator.SetFloat(this.ripplePlaybackSpeedHash, this.ripplePlaybackSpeed);
		if (this.waterVolume != null && this.waterVolume.Parameters != null)
		{
			this.renderer.color = this.waterVolume.Parameters.rippleSpriteColor;
		}
		Color color = this.renderer.color;
		color.a = 1f;
		this.renderer.color = color;
	}

	// Token: 0x06000CF5 RID: 3317 RVA: 0x0004449C File Offset: 0x0004269C
	private void Update()
	{
		if (this.waterVolume != null && !this.waterVolume.isStationary && this.waterVolume.surfacePlane != null)
		{
			Vector3 vector = Vector3.Dot(base.transform.position - this.waterVolume.surfacePlane.position, this.waterVolume.surfacePlane.up) * this.waterVolume.surfacePlane.up;
			base.transform.position = base.transform.position - vector;
		}
		float num = Mathf.Clamp01((Time.time - this.rippleStartTime - this.fadeOutDelay) / this.fadeOutTime);
		Color color = this.renderer.color;
		color.a = 1f - num;
		this.renderer.color = color;
		if (num >= 1f - Mathf.Epsilon)
		{
			this.Destroy();
			return;
		}
	}

	// Token: 0x04001055 RID: 4181
	[SerializeField]
	private float ripplePlaybackSpeed = 1f;

	// Token: 0x04001056 RID: 4182
	[SerializeField]
	private float fadeOutDelay = 0.5f;

	// Token: 0x04001057 RID: 4183
	[SerializeField]
	private float fadeOutTime = 1f;

	// Token: 0x04001058 RID: 4184
	private string ripplePlaybackSpeedName = "RipplePlaybackSpeed";

	// Token: 0x04001059 RID: 4185
	private int ripplePlaybackSpeedHash;

	// Token: 0x0400105A RID: 4186
	private float rippleStartTime = -1f;

	// Token: 0x0400105B RID: 4187
	private Animator animator;

	// Token: 0x0400105C RID: 4188
	private SpriteRenderer renderer;

	// Token: 0x0400105D RID: 4189
	private WaterVolume waterVolume;
}
