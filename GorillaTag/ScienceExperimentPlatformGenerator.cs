using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag.GuidedRefs;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x02000D34 RID: 3380
	public class ScienceExperimentPlatformGenerator : MonoBehaviourPun, ITickSystemPost, IGuidedRefReceiverMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x060054A6 RID: 21670 RVA: 0x0019C307 File Offset: 0x0019A507
		private void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
			this.scienceExperimentManager = base.GetComponent<ScienceExperimentManager>();
		}

		// Token: 0x060054A7 RID: 21671 RVA: 0x0019C31B File Offset: 0x0019A51B
		private void OnEnable()
		{
			if (((IGuidedRefReceiverMono)this).GuidedRefsWaitingToResolveCount > 0)
			{
				return;
			}
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x060054A8 RID: 21672 RVA: 0x000D1D87 File Offset: 0x000CFF87
		protected void OnDisable()
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x17000878 RID: 2168
		// (get) Token: 0x060054A9 RID: 21673 RVA: 0x0019C32D File Offset: 0x0019A52D
		// (set) Token: 0x060054AA RID: 21674 RVA: 0x0019C335 File Offset: 0x0019A535
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x060054AB RID: 21675 RVA: 0x0019C340 File Offset: 0x0019A540
		void ITickSystemPost.PostTick()
		{
			double num = (PhotonNetwork.InRoom ? PhotonNetwork.Time : Time.unscaledTimeAsDouble);
			this.UpdateTrails(num);
			this.RemoveExpiredBubbles(num);
			this.SpawnNewBubbles(num);
			this.UpdateActiveBubbles(num);
		}

		// Token: 0x060054AC RID: 21676 RVA: 0x0019C380 File Offset: 0x0019A580
		private void RemoveExpiredBubbles(double currentTime)
		{
			for (int i = this.activeBubbles.Count - 1; i >= 0; i--)
			{
				if (Mathf.Clamp01((float)(currentTime - this.activeBubbles[i].spawnTime) / this.activeBubbles[i].lifetime) >= 1f)
				{
					this.activeBubbles[i].bubble.Pop();
					this.activeBubbles.RemoveAt(i);
				}
			}
		}

		// Token: 0x060054AD RID: 21677 RVA: 0x0019C3FC File Offset: 0x0019A5FC
		private void SpawnNewBubbles(double currentTime)
		{
			if (base.photonView.IsMine && this.scienceExperimentManager.GameState == ScienceExperimentManager.RisingLiquidState.Rising)
			{
				int num = Mathf.Min((int)(this.rockCountVsLavaProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.bubbleCountMultiplier), this.maxBubbleCount) - this.activeBubbles.Count;
				if (this.activeBubbles.Count < this.maxBubbleCount)
				{
					for (int i = 0; i < num; i++)
					{
						this.SpawnRockAuthority(currentTime, this.scienceExperimentManager.RiseProgressLinear);
					}
				}
			}
		}

		// Token: 0x060054AE RID: 21678 RVA: 0x0019C48C File Offset: 0x0019A68C
		private void UpdateActiveBubbles(double currentTime)
		{
			if (this.liquidSurfacePlane == null)
			{
				return;
			}
			float y = this.liquidSurfacePlane.transform.position.y;
			float num = this.bubblePopWobbleAmplitude * Mathf.Sin(this.bubblePopWobbleFrequency * 0.5f * 3.1415927f * Time.time);
			for (int i = 0; i < this.activeBubbles.Count; i++)
			{
				ScienceExperimentPlatformGenerator.BubbleData bubbleData = this.activeBubbles[i];
				float num2 = Mathf.Clamp01((float)(currentTime - bubbleData.spawnTime) / bubbleData.lifetime);
				float num3 = bubbleData.spawnSize * this.rockSizeVsLifetime.Evaluate(num2) * this.scaleFactor;
				bubbleData.position.y = y;
				bubbleData.bubble.body.gameObject.transform.localScale = Vector3.one * num3;
				bubbleData.bubble.body.MovePosition(bubbleData.position);
				float num4 = (float)((double)bubbleData.lifetime + bubbleData.spawnTime - currentTime);
				if (num4 < this.bubblePopAnticipationTime)
				{
					float num5 = Mathf.Clamp01(1f - num4 / this.bubblePopAnticipationTime);
					bubbleData.bubble.bubbleMesh.transform.localScale = Vector3.one * (1f + num5 * num);
				}
				this.activeBubbles[i] = bubbleData;
			}
		}

		// Token: 0x060054AF RID: 21679 RVA: 0x0019C5F4 File Offset: 0x0019A7F4
		private void UpdateTrails(double currentTime)
		{
			if (base.photonView.IsMine)
			{
				int num = (int)(this.trailCountVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.trailCountMultiplier) - this.trailHeads.Count;
				if (num > 0 && this.scienceExperimentManager.GameState == ScienceExperimentManager.RisingLiquidState.Rising)
				{
					for (int i = 0; i < num; i++)
					{
						this.SpawnTrailAuthority(currentTime, this.scienceExperimentManager.RiseProgressLinear);
					}
				}
				else if (num < 0)
				{
					for (int j = 0; j > num; j--)
					{
						this.trailHeads.RemoveAt(0);
					}
				}
				float num2 = this.trailSpawnRateVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.trailSpawnRateMultiplier;
				float num3 = this.trailBubbleBoundaryRadiusVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.surfaceRadiusSpawnRange.y;
				for (int k = this.trailHeads.Count - 1; k >= 0; k--)
				{
					if ((float)(currentTime - this.trailHeads[k].spawnTime) > num2)
					{
						float num4 = -this.trailMaxTurnAngle;
						float num5 = this.trailMaxTurnAngle;
						float num6 = Vector3.SignedAngle(this.trailHeads[k].direction, this.trailHeads[k].position - this.liquidSurfacePlane.transform.position, Vector3.up);
						float num7 = num3 - Vector3.Distance(this.trailHeads[k].position, this.liquidSurfacePlane.transform.position);
						if (num7 < this.trailEdgeAvoidanceSpawnsMinMax.x * this.trailDistanceBetweenSpawns * this.scaleFactor)
						{
							float num8 = Mathf.InverseLerp(this.trailEdgeAvoidanceSpawnsMinMax.x * this.trailDistanceBetweenSpawns * this.scaleFactor, this.trailEdgeAvoidanceSpawnsMinMax.y * this.trailDistanceBetweenSpawns * this.scaleFactor, num7);
							if (num6 > 0f)
							{
								float num9 = num6 - 90f * num8;
								num5 = Mathf.Min(num5, num9);
								num4 = Mathf.Min(num4, num5 - this.trailMaxTurnAngle);
							}
							else
							{
								float num10 = num6 + 90f * num8;
								num4 = Mathf.Max(num4, num10);
								num5 = Mathf.Max(num5, num4 + this.trailMaxTurnAngle);
							}
						}
						Vector3 vector = Quaternion.AngleAxis(Random.Range(num4, num5), Vector3.up) * this.trailHeads[k].direction;
						Vector3 vector2 = this.trailHeads[k].position + vector * this.trailDistanceBetweenSpawns * this.scaleFactor - this.liquidSurfacePlane.transform.position;
						if (vector2.sqrMagnitude > this.surfaceRadiusSpawnRange.y * this.surfaceRadiusSpawnRange.y)
						{
							vector2 = vector2.normalized * this.surfaceRadiusSpawnRange.y;
						}
						Vector2 vector3 = new Vector2(vector2.x, vector2.z);
						float num11 = this.trailBubbleSize;
						float num12 = this.trailBubbleLifetimeVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.trailBubbleLifetimeMultiplier;
						this.trailHeads.RemoveAt(k);
						base.photonView.RPC("SpawnSodaBubbleRPC", RpcTarget.Others, new object[] { vector3, num11, num12, currentTime });
						this.SpawnSodaBubbleLocal(vector3, num11, num12, currentTime, true, vector);
					}
				}
			}
		}

		// Token: 0x060054B0 RID: 21680 RVA: 0x0019C98C File Offset: 0x0019AB8C
		private void SpawnRockAuthority(double currentTime, float lavaProgress)
		{
			if (base.photonView.IsMine)
			{
				float num = this.rockLifetimeMultiplierVsLavaProgress.Evaluate(lavaProgress);
				float num2 = this.rockMaxSizeMultiplierVsLavaProgress.Evaluate(lavaProgress);
				float num3 = Random.Range(this.lifetimeRange.x, this.lifetimeRange.y) * num;
				float num4 = Random.Range(this.sizeRange.x, this.sizeRange.y * num2);
				float num5 = this.spawnRadiusMultiplierVsLavaProgress.Evaluate(lavaProgress);
				Vector2 vector = Random.insideUnitCircle.normalized * Random.Range(this.surfaceRadiusSpawnRange.x, this.surfaceRadiusSpawnRange.y) * num5;
				vector = this.GetSpawnPositionWithClearance(vector, num4 * this.scaleFactor, this.surfaceRadiusSpawnRange.y, this.liquidSurfacePlane.transform.position);
				base.photonView.RPC("SpawnSodaBubbleRPC", RpcTarget.Others, new object[] { vector, num4, num3, currentTime });
				this.SpawnSodaBubbleLocal(vector, num4, num3, currentTime, false, default(Vector3));
			}
		}

		// Token: 0x060054B1 RID: 21681 RVA: 0x0019CAC4 File Offset: 0x0019ACC4
		private void SpawnTrailAuthority(double currentTime, float lavaProgress)
		{
			if (base.photonView.IsMine)
			{
				float num = this.trailBubbleLifetimeVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.trailBubbleLifetimeMultiplier;
				float num2 = this.trailBubbleSize;
				Vector2 vector = Random.insideUnitCircle.normalized * Random.Range(this.surfaceRadiusSpawnRange.x, this.surfaceRadiusSpawnRange.y);
				vector = this.GetSpawnPositionWithClearance(vector, num2 * this.scaleFactor, this.surfaceRadiusSpawnRange.y, this.liquidSurfacePlane.transform.position);
				Vector3 vector2 = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up) * Vector3.forward;
				base.photonView.RPC("SpawnSodaBubbleRPC", RpcTarget.Others, new object[] { vector, num2, num, currentTime });
				this.SpawnSodaBubbleLocal(vector, num2, num, currentTime, true, vector2);
			}
		}

		// Token: 0x060054B2 RID: 21682 RVA: 0x0019CBCC File Offset: 0x0019ADCC
		private void SpawnSodaBubbleLocal(Vector2 surfacePosLocal, float spawnSize, float lifetime, double spawnTime, bool addAsTrail = false, Vector3 direction = default(Vector3))
		{
			if (this.activeBubbles.Count < this.maxBubbleCount)
			{
				Vector3 vector = this.liquidSurfacePlane.transform.position + new Vector3(surfacePosLocal.x, 0f, surfacePosLocal.y);
				ScienceExperimentPlatformGenerator.BubbleData bubbleData = new ScienceExperimentPlatformGenerator.BubbleData
				{
					position = vector,
					spawnSize = spawnSize,
					lifetime = lifetime,
					spawnTime = spawnTime,
					isTrail = false
				};
				bubbleData.bubble = ObjectPools.instance.Instantiate(this.spawnedPrefab, bubbleData.position, Quaternion.identity, 0f, true).GetComponent<SodaBubble>();
				if (base.photonView.IsMine && addAsTrail)
				{
					bubbleData.direction = direction;
					bubbleData.isTrail = true;
					this.trailHeads.Add(bubbleData);
				}
				this.activeBubbles.Add(bubbleData);
			}
		}

		// Token: 0x060054B3 RID: 21683 RVA: 0x0019CCB4 File Offset: 0x0019AEB4
		[PunRPC]
		public void SpawnSodaBubbleRPC(Vector2 surfacePosLocal, float spawnSize, float lifetime, double spawnTime, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SpawnSodaBubbleRPC");
			if (info.Sender == PhotonNetwork.MasterClient)
			{
				if (!float.IsFinite(spawnSize) || !float.IsFinite(lifetime) || !double.IsFinite(spawnTime))
				{
					return;
				}
				float num = Mathf.Clamp01(this.scienceExperimentManager.RiseProgressLinear);
				(ref surfacePosLocal).ClampThisMagnitudeSafe(this.surfaceRadiusSpawnRange.y);
				spawnSize = Mathf.Clamp(spawnSize, this.sizeRange.x, this.sizeRange.y * this.rockMaxSizeMultiplierVsLavaProgress.Evaluate(num));
				lifetime = Mathf.Clamp(lifetime, this.lifetimeRange.x, this.lifetimeRange.y * this.rockLifetimeMultiplierVsLavaProgress.Evaluate(num));
				double num2 = (PhotonNetwork.InRoom ? PhotonNetwork.Time : Time.unscaledTimeAsDouble);
				spawnTime = ((Mathf.Abs((float)(spawnTime - num2)) < 10f) ? spawnTime : num2);
				this.SpawnSodaBubbleLocal(surfacePosLocal, spawnSize, lifetime, spawnTime, false, default(Vector3));
			}
		}

		// Token: 0x060054B4 RID: 21684 RVA: 0x0019CDB4 File Offset: 0x0019AFB4
		private Vector2 GetSpawnPositionWithClearance(Vector2 inputPosition, float inputSize, float maxDistance, Vector3 lavaSurfaceOrigin)
		{
			Vector2 vector = inputPosition;
			for (int i = 0; i < this.activeBubbles.Count; i++)
			{
				Vector3 vector2 = this.activeBubbles[i].position - lavaSurfaceOrigin;
				Vector2 vector3 = new Vector2(vector2.x, vector2.z);
				Vector2 vector4 = vector - vector3;
				float num = (inputSize + this.activeBubbles[i].spawnSize * this.scaleFactor) * 0.5f;
				if (vector4.sqrMagnitude < num * num)
				{
					float magnitude = vector4.magnitude;
					if (magnitude > 0.001f)
					{
						Vector2 vector5 = vector4 / magnitude;
						vector += vector5 * (num - magnitude);
						if (vector.sqrMagnitude > maxDistance * maxDistance)
						{
							vector = vector.normalized * maxDistance;
						}
					}
				}
			}
			if (vector.sqrMagnitude > this.surfaceRadiusSpawnRange.y * this.surfaceRadiusSpawnRange.y)
			{
				vector = vector.normalized * this.surfaceRadiusSpawnRange.y;
			}
			return vector;
		}

		// Token: 0x060054B5 RID: 21685 RVA: 0x0019CEC7 File Offset: 0x0019B0C7
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterReceiverField<ScienceExperimentPlatformGenerator>(this, "liquidSurfacePlane", ref this.liquidSurfacePlane_gRef);
			GuidedRefHub.ReceiverFullyRegistered<ScienceExperimentPlatformGenerator>(this);
		}

		// Token: 0x17000879 RID: 2169
		// (get) Token: 0x060054B6 RID: 21686 RVA: 0x0019CEE0 File Offset: 0x0019B0E0
		// (set) Token: 0x060054B7 RID: 21687 RVA: 0x0019CEE8 File Offset: 0x0019B0E8
		int IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount { get; set; }

		// Token: 0x060054B8 RID: 21688 RVA: 0x0019CEF1 File Offset: 0x0019B0F1
		bool IGuidedRefReceiverMono.GuidedRefTryResolveReference(GuidedRefTryResolveInfo target)
		{
			return GuidedRefHub.TryResolveField<ScienceExperimentPlatformGenerator, Transform>(this, ref this.liquidSurfacePlane, this.liquidSurfacePlane_gRef, target);
		}

		// Token: 0x060054B9 RID: 21689 RVA: 0x0019CF06 File Offset: 0x0019B106
		void IGuidedRefReceiverMono.OnAllGuidedRefsResolved()
		{
			if (!base.enabled)
			{
				return;
			}
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x060054BA RID: 21690 RVA: 0x000D1D87 File Offset: 0x000CFF87
		void IGuidedRefReceiverMono.OnGuidedRefTargetDestroyed(int fieldId)
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x060054BC RID: 21692 RVA: 0x00045F89 File Offset: 0x00044189
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x060054BD RID: 21693 RVA: 0x00017401 File Offset: 0x00015601
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040057D6 RID: 22486
		[SerializeField]
		private GameObject spawnedPrefab;

		// Token: 0x040057D7 RID: 22487
		[SerializeField]
		private float scaleFactor = 0.03f;

		// Token: 0x040057D8 RID: 22488
		[Header("Random Bubbles")]
		[SerializeField]
		private Vector2 surfaceRadiusSpawnRange = new Vector2(0.1f, 0.7f);

		// Token: 0x040057D9 RID: 22489
		[SerializeField]
		private Vector2 lifetimeRange = new Vector2(5f, 10f);

		// Token: 0x040057DA RID: 22490
		[SerializeField]
		private Vector2 sizeRange = new Vector2(0.5f, 2f);

		// Token: 0x040057DB RID: 22491
		[SerializeField]
		private AnimationCurve rockCountVsLavaProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040057DC RID: 22492
		[SerializeField]
		[FormerlySerializedAs("rockCountMultiplier")]
		private float bubbleCountMultiplier = 80f;

		// Token: 0x040057DD RID: 22493
		[SerializeField]
		private int maxBubbleCount = 100;

		// Token: 0x040057DE RID: 22494
		[SerializeField]
		private AnimationCurve rockLifetimeMultiplierVsLavaProgress = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x040057DF RID: 22495
		[SerializeField]
		private AnimationCurve rockMaxSizeMultiplierVsLavaProgress = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x040057E0 RID: 22496
		[SerializeField]
		private AnimationCurve spawnRadiusMultiplierVsLavaProgress = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x040057E1 RID: 22497
		[SerializeField]
		private AnimationCurve rockSizeVsLifetime = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040057E2 RID: 22498
		[Header("Bubble Trails")]
		[SerializeField]
		private AnimationCurve trailSpawnRateVsProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040057E3 RID: 22499
		[SerializeField]
		private float trailSpawnRateMultiplier = 1f;

		// Token: 0x040057E4 RID: 22500
		[SerializeField]
		private AnimationCurve trailBubbleLifetimeVsProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040057E5 RID: 22501
		[SerializeField]
		private AnimationCurve trailBubbleBoundaryRadiusVsProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040057E6 RID: 22502
		[SerializeField]
		private float trailBubbleLifetimeMultiplier = 6f;

		// Token: 0x040057E7 RID: 22503
		[SerializeField]
		private float trailDistanceBetweenSpawns = 3f;

		// Token: 0x040057E8 RID: 22504
		[SerializeField]
		private float trailMaxTurnAngle = 55f;

		// Token: 0x040057E9 RID: 22505
		[SerializeField]
		private float trailBubbleSize = 1.5f;

		// Token: 0x040057EA RID: 22506
		[SerializeField]
		private AnimationCurve trailCountVsProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040057EB RID: 22507
		[SerializeField]
		private float trailCountMultiplier = 12f;

		// Token: 0x040057EC RID: 22508
		[SerializeField]
		private Vector2 trailEdgeAvoidanceSpawnsMinMax = new Vector2(3f, 1f);

		// Token: 0x040057ED RID: 22509
		[Header("Feedback Effects")]
		[SerializeField]
		private float bubblePopAnticipationTime = 2f;

		// Token: 0x040057EE RID: 22510
		[SerializeField]
		private float bubblePopWobbleFrequency = 25f;

		// Token: 0x040057EF RID: 22511
		[SerializeField]
		private float bubblePopWobbleAmplitude = 0.01f;

		// Token: 0x040057F0 RID: 22512
		[SerializeField]
		private Transform liquidSurfacePlane;

		// Token: 0x040057F1 RID: 22513
		[SerializeField]
		private GuidedRefReceiverFieldInfo liquidSurfacePlane_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x040057F2 RID: 22514
		private List<ScienceExperimentPlatformGenerator.BubbleData> activeBubbles = new List<ScienceExperimentPlatformGenerator.BubbleData>();

		// Token: 0x040057F3 RID: 22515
		private List<ScienceExperimentPlatformGenerator.BubbleData> trailHeads = new List<ScienceExperimentPlatformGenerator.BubbleData>();

		// Token: 0x040057F4 RID: 22516
		private List<ScienceExperimentPlatformGenerator.BubbleSpawnDebug> bubbleSpawnDebug = new List<ScienceExperimentPlatformGenerator.BubbleSpawnDebug>();

		// Token: 0x040057F5 RID: 22517
		private ScienceExperimentManager scienceExperimentManager;

		// Token: 0x02000D35 RID: 3381
		private struct BubbleData
		{
			// Token: 0x040057F8 RID: 22520
			public Vector3 position;

			// Token: 0x040057F9 RID: 22521
			public Vector3 direction;

			// Token: 0x040057FA RID: 22522
			public float spawnSize;

			// Token: 0x040057FB RID: 22523
			public float lifetime;

			// Token: 0x040057FC RID: 22524
			public double spawnTime;

			// Token: 0x040057FD RID: 22525
			public bool isTrail;

			// Token: 0x040057FE RID: 22526
			public SodaBubble bubble;
		}

		// Token: 0x02000D36 RID: 3382
		private struct BubbleSpawnDebug
		{
			// Token: 0x040057FF RID: 22527
			public Vector3 initialPosition;

			// Token: 0x04005800 RID: 22528
			public Vector3 initialDirection;

			// Token: 0x04005801 RID: 22529
			public Vector3 spawnPosition;

			// Token: 0x04005802 RID: 22530
			public float minAngle;

			// Token: 0x04005803 RID: 22531
			public float maxAngle;

			// Token: 0x04005804 RID: 22532
			public float edgeCorrectionAngle;

			// Token: 0x04005805 RID: 22533
			public double spawnTime;
		}
	}
}
