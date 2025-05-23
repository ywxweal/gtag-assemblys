using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020000AC RID: 172
public class ShadeJumpscare : MonoBehaviour
{
	// Token: 0x0600043B RID: 1083 RVA: 0x00018A83 File Offset: 0x00016C83
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x0600043C RID: 1084 RVA: 0x00018A91 File Offset: 0x00016C91
	private void OnEnable()
	{
		this.startTime = Time.time;
		this.startAngle = Random.value * 360f;
		this.audioSource.clip = this.audioClips.GetRandomItem<AudioClip>();
		this.audioSource.Play();
	}

	// Token: 0x0600043D RID: 1085 RVA: 0x00018AD0 File Offset: 0x00016CD0
	private void Update()
	{
		float num = Time.time - this.startTime;
		float num2 = num / this.animationTime;
		this.shadeTransform.SetPositionAndRotation(base.transform.position + new Vector3(0f, this.shadeHeightFunction.Evaluate(num2), 0f), Quaternion.Euler(0f, this.startAngle + num * this.shadeRotationSpeed, 0f));
		float num3 = this.shadeScaleFunction.Evaluate(num2);
		this.shadeTransform.localScale = new Vector3(num3, num3 * this.shadeYScaleMultFunction.Evaluate(num2), num3);
		this.audioSource.volume = this.soundVolumeFunction.Evaluate(num2);
	}

	// Token: 0x040004C1 RID: 1217
	[SerializeField]
	private Transform shadeTransform;

	// Token: 0x040004C2 RID: 1218
	[SerializeField]
	private float animationTime;

	// Token: 0x040004C3 RID: 1219
	[SerializeField]
	private float shadeRotationSpeed = 1f;

	// Token: 0x040004C4 RID: 1220
	[SerializeField]
	private AnimationCurve shadeHeightFunction;

	// Token: 0x040004C5 RID: 1221
	[SerializeField]
	private AnimationCurve shadeScaleFunction;

	// Token: 0x040004C6 RID: 1222
	[SerializeField]
	private AnimationCurve shadeYScaleMultFunction;

	// Token: 0x040004C7 RID: 1223
	[SerializeField]
	private AnimationCurve soundVolumeFunction;

	// Token: 0x040004C8 RID: 1224
	[SerializeField]
	private AudioClip[] audioClips;

	// Token: 0x040004C9 RID: 1225
	private AudioSource audioSource;

	// Token: 0x040004CA RID: 1226
	private float startTime;

	// Token: 0x040004CB RID: 1227
	private float startAngle;
}
