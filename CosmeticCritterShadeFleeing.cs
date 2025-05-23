using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020000A9 RID: 169
public class CosmeticCritterShadeFleeing : CosmeticCritter
{
	// Token: 0x06000431 RID: 1073 RVA: 0x000186C3 File Offset: 0x000168C3
	public override void OnSpawn()
	{
		this.spawnFX.Play();
		this.spawnAudioSource.clip = this.spawnAudioClips.GetRandomItem<AudioClip>();
		this.spawnAudioSource.Play();
		this.pullVector = Vector3.zero;
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x000186FC File Offset: 0x000168FC
	public void SetFleePosition(Vector3 position, Vector3 fleeFrom)
	{
		this.origin = position;
		Vector3 vector = position - fleeFrom;
		this.fleeForward = vector.normalized;
		this.fleeRight = Vector3.Cross(this.fleeForward, Vector3.up);
		this.fleeUp = Vector3.Cross(this.fleeForward, this.fleeRight);
		this.trailingPosition = position + vector.normalized * 3f;
	}

	// Token: 0x06000433 RID: 1075 RVA: 0x00018770 File Offset: 0x00016970
	public override void SetRandomVariables()
	{
		float num = 0f;
		for (int i = 0; i < this.modelSwaps.Length; i++)
		{
			num += this.modelSwaps[i].relativeProbability;
			this.modelSwaps[i].gameObject.SetActive(false);
		}
		float num2 = Random.value * num;
		for (int j = 0; j < this.modelSwaps.Length; j++)
		{
			if (num2 < this.modelSwaps[j].relativeProbability)
			{
				this.modelSwaps[j].gameObject.SetActive(true);
				break;
			}
			num2 -= this.modelSwaps[j].relativeProbability;
		}
		this.fleeBobFrequencyXY = new Vector2(Random.Range(-1f, 1f) * this.fleeBobFrequencyXYMax.x, Random.Range(-1f, 1f) * this.fleeBobFrequencyXYMax.y);
		this.fleeBobMagnitudeXY = new Vector2(Random.Range(-1f, 1f) * this.fleeBobMagnitudeXYMax.x, Random.Range(-1f, 1f) * this.fleeBobMagnitudeXYMax.y);
	}

	// Token: 0x06000434 RID: 1076 RVA: 0x00018890 File Offset: 0x00016A90
	public override void Tick()
	{
		float num = (float)base.GetAliveTime();
		Vector3 vector = this.origin + num * this.fleeForward + this.pullVector + Mathf.Sin(this.fleeBobFrequencyXY.x * num) * this.fleeBobMagnitudeXY.x * this.fleeRight + Mathf.Sin(this.fleeBobFrequencyXY.y * num) * this.fleeBobMagnitudeXY.y * this.fleeUp;
		Quaternion quaternion = Quaternion.LookRotation((vector - this.trailingPosition).normalized, Vector3.up);
		this.trailingPosition = Vector3.Lerp(this.trailingPosition, vector, 0.05f);
		base.transform.SetPositionAndRotation(vector, quaternion);
		this.animator.SetFloat(this.animatorProperty, Mathf.Sin(num * 3f) * 0.5f + 0.5f);
	}

	// Token: 0x040004A5 RID: 1189
	[Tooltip("Randomly selects one of these models when spawned, accounting for relative probabilities. For example, if one model has a probability of 1 and another a probability of 2, the second is twice as likely to be picked (and thus will be picked 67% of the time).")]
	[SerializeField]
	private CosmeticCritterShadeFleeing.ModelSwap[] modelSwaps;

	// Token: 0x040004A6 RID: 1190
	[Space]
	[Tooltip("Despawn the Shade after it has fled (fleed?) this many meters.")]
	[SerializeField]
	private float fleeDistanceToDespawn = 10f;

	// Token: 0x040004A7 RID: 1191
	[Tooltip("Flee away from the spotter at this many meters per second.")]
	[SerializeField]
	private float fleeSpeed;

	// Token: 0x040004A8 RID: 1192
	[Tooltip("The maximum strength the shade can move bob around in the horizontal and vertical axes, with final value chosen randomly.")]
	[SerializeField]
	private Vector2 fleeBobMagnitudeXYMax;

	// Token: 0x040004A9 RID: 1193
	[Tooltip("The maximum frequency the shade can move bob around in the horizontal and vertical axes, with final value chosen randomly.")]
	[SerializeField]
	private Vector2 fleeBobFrequencyXYMax;

	// Token: 0x040004AA RID: 1194
	[SerializeField]
	private Animator animator;

	// Token: 0x040004AB RID: 1195
	[SerializeField]
	private ParticleSystem spawnFX;

	// Token: 0x040004AC RID: 1196
	[SerializeField]
	private AudioSource spawnAudioSource;

	// Token: 0x040004AD RID: 1197
	[SerializeField]
	private AudioClip[] spawnAudioClips;

	// Token: 0x040004AE RID: 1198
	[HideInInspector]
	public Vector3 pullVector;

	// Token: 0x040004AF RID: 1199
	private Vector3 origin;

	// Token: 0x040004B0 RID: 1200
	private Vector3 fleeForward;

	// Token: 0x040004B1 RID: 1201
	private Vector3 fleeRight;

	// Token: 0x040004B2 RID: 1202
	private Vector3 fleeUp = Vector3.up;

	// Token: 0x040004B3 RID: 1203
	private Vector2 fleeBobFrequencyXY;

	// Token: 0x040004B4 RID: 1204
	private Vector2 fleeBobMagnitudeXY;

	// Token: 0x040004B5 RID: 1205
	private Vector3 trailingPosition;

	// Token: 0x040004B6 RID: 1206
	private float closestCatcherDistance;

	// Token: 0x040004B7 RID: 1207
	private int animatorProperty = Animator.StringToHash("Distance");

	// Token: 0x020000AA RID: 170
	[Serializable]
	private class ModelSwap
	{
		// Token: 0x040004B8 RID: 1208
		public float relativeProbability;

		// Token: 0x040004B9 RID: 1209
		public GameObject gameObject;
	}
}
