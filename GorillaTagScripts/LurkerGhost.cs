using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000B1B RID: 2843
	[NetworkBehaviourWeaved(6)]
	public class LurkerGhost : NetworkComponent
	{
		// Token: 0x060045EF RID: 17903 RVA: 0x0014C066 File Offset: 0x0014A266
		protected override void Awake()
		{
			base.Awake();
			this.possibleTargets = new List<NetPlayer>();
			this.targetPlayer = null;
			this.targetTransform = null;
			this.targetVRRig = null;
		}

		// Token: 0x060045F0 RID: 17904 RVA: 0x0014C08E File Offset: 0x0014A28E
		protected override void Start()
		{
			base.Start();
			this.waypointRegions = this.waypointsContainer.GetComponentsInChildren<ZoneBasedObject>();
			this.PickNextWaypoint();
			this.ChangeState(LurkerGhost.ghostState.patrol);
		}

		// Token: 0x060045F1 RID: 17905 RVA: 0x0014C0B4 File Offset: 0x0014A2B4
		private void LateUpdate()
		{
			this.UpdateState();
			this.UpdateGhostVisibility();
		}

		// Token: 0x060045F2 RID: 17906 RVA: 0x0014C0C4 File Offset: 0x0014A2C4
		private void PickNextWaypoint()
		{
			if (this.waypoints.Count == 0 || this.lastWaypointRegion == null || !this.lastWaypointRegion.IsLocalPlayerInZone())
			{
				ZoneBasedObject zoneBasedObject = ZoneBasedObject.SelectRandomEligible(this.waypointRegions, "");
				if (zoneBasedObject == null)
				{
					zoneBasedObject = this.lastWaypointRegion;
				}
				if (zoneBasedObject == null)
				{
					return;
				}
				this.lastWaypointRegion = zoneBasedObject;
				this.waypoints.Clear();
				foreach (object obj in zoneBasedObject.transform)
				{
					Transform transform = (Transform)obj;
					this.waypoints.Add(transform);
				}
			}
			int num = Random.Range(0, this.waypoints.Count);
			this.currentWaypoint = this.waypoints[num];
			this.targetRotation = Quaternion.LookRotation(this.currentWaypoint.position - base.transform.position);
			this.waypoints.RemoveAt(num);
		}

		// Token: 0x060045F3 RID: 17907 RVA: 0x0014C1E4 File Offset: 0x0014A3E4
		private void Patrol()
		{
			Transform transform = this.currentWaypoint;
			if (transform != null)
			{
				base.transform.position = Vector3.MoveTowards(base.transform.position, transform.position, this.patrolSpeed * Time.deltaTime);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, 360f * Time.deltaTime);
			}
		}

		// Token: 0x060045F4 RID: 17908 RVA: 0x0014C25C File Offset: 0x0014A45C
		private void PlaySound(AudioClip clip, bool loop)
		{
			if (this.audioSource && this.audioSource.isPlaying)
			{
				this.audioSource.GTStop();
			}
			if (this.audioSource && clip != null)
			{
				this.audioSource.clip = clip;
				this.audioSource.loop = loop;
				this.audioSource.GTPlay();
			}
		}

		// Token: 0x060045F5 RID: 17909 RVA: 0x0014C2C8 File Offset: 0x0014A4C8
		private bool PickPlayer(float maxDistance)
		{
			if (base.IsMine)
			{
				this.possibleTargets.Clear();
				for (int i = 0; i < GorillaParent.instance.vrrigs.Count; i++)
				{
					if ((GorillaParent.instance.vrrigs[i].transform.position - base.transform.position).magnitude < maxDistance && GorillaParent.instance.vrrigs[i].creator != this.targetPlayer)
					{
						this.possibleTargets.Add(GorillaParent.instance.vrrigs[i].creator);
					}
				}
				this.targetPlayer = null;
				this.targetTransform = null;
				this.targetVRRig = null;
				if (this.possibleTargets.Count > 0)
				{
					int num = Random.Range(0, this.possibleTargets.Count);
					this.PickPlayer(this.possibleTargets[num]);
				}
			}
			else
			{
				this.targetPlayer = null;
				this.targetTransform = null;
				this.targetVRRig = null;
			}
			return this.targetPlayer != null && this.targetTransform != null;
		}

		// Token: 0x060045F6 RID: 17910 RVA: 0x0014C3F8 File Offset: 0x0014A5F8
		private void PickPlayer(NetPlayer player)
		{
			int num = GorillaParent.instance.vrrigs.FindIndex((VRRig x) => x.creator != null && x.creator == player);
			if (num > -1 && num < GorillaParent.instance.vrrigs.Count)
			{
				this.targetPlayer = GorillaParent.instance.vrrigs[num].creator;
				this.targetTransform = GorillaParent.instance.vrrigs[num].head.rigTarget;
				this.targetVRRig = GorillaParent.instance.vrrigs[num];
			}
		}

		// Token: 0x060045F7 RID: 17911 RVA: 0x0014C4A0 File Offset: 0x0014A6A0
		private void SeekPlayer()
		{
			if (this.targetTransform == null)
			{
				this.ChangeState(LurkerGhost.ghostState.patrol);
				return;
			}
			this.targetPosition = this.targetTransform.position + this.targetTransform.forward.x0z() * this.seekAheadDistance;
			this.targetRotation = Quaternion.LookRotation(this.targetTransform.position - base.transform.position);
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPosition, this.seekSpeed * Time.deltaTime);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, 720f * Time.deltaTime);
		}

		// Token: 0x060045F8 RID: 17912 RVA: 0x0014C574 File Offset: 0x0014A774
		private void ChargeAtPlayer()
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPosition, this.chargeSpeed * Time.deltaTime);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, 720f * Time.deltaTime);
		}

		// Token: 0x060045F9 RID: 17913 RVA: 0x0014C5DC File Offset: 0x0014A7DC
		private void UpdateGhostVisibility()
		{
			switch (this.currentState)
			{
			case LurkerGhost.ghostState.patrol:
				this.meshRenderer.sharedMaterial = this.scryableMaterial;
				this.bonesMeshRenderer.sharedMaterial = this.scryableMaterialBones;
				return;
			case LurkerGhost.ghostState.seek:
			case LurkerGhost.ghostState.charge:
				if (this.targetPlayer == NetworkSystem.Instance.LocalPlayer || this.passingPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.meshRenderer.sharedMaterial = this.visibleMaterial;
					this.bonesMeshRenderer.sharedMaterial = this.visibleMaterialBones;
					return;
				}
				this.meshRenderer.sharedMaterial = this.scryableMaterial;
				this.bonesMeshRenderer.sharedMaterial = this.scryableMaterialBones;
				return;
			case LurkerGhost.ghostState.possess:
				if (this.targetPlayer == NetworkSystem.Instance.LocalPlayer || this.passingPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.meshRenderer.sharedMaterial = this.visibleMaterial;
					this.bonesMeshRenderer.sharedMaterial = this.visibleMaterialBones;
					return;
				}
				this.meshRenderer.sharedMaterial = this.scryableMaterial;
				this.bonesMeshRenderer.sharedMaterial = this.scryableMaterialBones;
				return;
			default:
				return;
			}
		}

		// Token: 0x060045FA RID: 17914 RVA: 0x0014C700 File Offset: 0x0014A900
		private void HauntObjects()
		{
			Collider[] array = new Collider[20];
			int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.sphereColliderRadius, array);
			for (int i = 0; i < num; i++)
			{
				if (array[i].CompareTag("HauntedObject"))
				{
					UnityAction<GameObject> triggerHauntedObjects = this.TriggerHauntedObjects;
					if (triggerHauntedObjects != null)
					{
						triggerHauntedObjects(array[i].gameObject);
					}
				}
			}
		}

		// Token: 0x060045FB RID: 17915 RVA: 0x0014C764 File Offset: 0x0014A964
		private void ChangeState(LurkerGhost.ghostState newState)
		{
			this.currentState = newState;
			VRRig vrrig = null;
			switch (this.currentState)
			{
			case LurkerGhost.ghostState.patrol:
				this.PlaySound(this.patrolAudio, true);
				this.passingPlayer = null;
				this.cooldownTimeRemaining = Random.Range(this.cooldownDuration, this.maxCooldownDuration);
				this.currentRepeatHuntTimes = 0;
				break;
			case LurkerGhost.ghostState.charge:
				this.PlaySound(this.huntAudio, false);
				this.targetPosition = this.targetTransform.position;
				this.targetRotation = Quaternion.LookRotation(this.targetTransform.position - base.transform.position);
				break;
			case LurkerGhost.ghostState.possess:
				if (this.targetPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.PlaySound(this.possessedAudio, true);
					GorillaTagger.Instance.StartVibration(true, this.hapticStrength, this.hapticDuration);
					GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
				}
				vrrig = GorillaGameManager.StaticFindRigForPlayer(this.targetPlayer);
				break;
			}
			Shader.SetGlobalFloat(this._BlackAndWhite, (float)((newState == LurkerGhost.ghostState.possess && this.targetPlayer == NetworkSystem.Instance.LocalPlayer) ? 1 : 0));
			if (vrrig != this.lastHauntedVRRig && this.lastHauntedVRRig != null)
			{
				this.lastHauntedVRRig.IsHaunted = false;
			}
			if (vrrig != null)
			{
				vrrig.IsHaunted = true;
			}
			this.lastHauntedVRRig = vrrig;
			this.UpdateGhostVisibility();
		}

		// Token: 0x060045FC RID: 17916 RVA: 0x0014C8E2 File Offset: 0x0014AAE2
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			Shader.SetGlobalFloat(this._BlackAndWhite, 0f);
		}

		// Token: 0x060045FD RID: 17917 RVA: 0x0014C900 File Offset: 0x0014AB00
		private void UpdateState()
		{
			switch (this.currentState)
			{
			case LurkerGhost.ghostState.patrol:
				this.Patrol();
				if (base.IsMine)
				{
					if (this.currentWaypoint == null || Vector3.Distance(base.transform.position, this.currentWaypoint.position) < 0.2f)
					{
						this.PickNextWaypoint();
					}
					this.cooldownTimeRemaining -= Time.deltaTime;
					if (this.cooldownTimeRemaining <= 0f)
					{
						this.cooldownTimeRemaining = 0f;
						if (this.PickPlayer(this.maxHuntDistance))
						{
							this.ChangeState(LurkerGhost.ghostState.seek);
							return;
						}
					}
				}
				break;
			case LurkerGhost.ghostState.seek:
				this.SeekPlayer();
				if (base.IsMine && (this.targetPosition - base.transform.position).sqrMagnitude < this.seekCloseEnoughDistance * this.seekCloseEnoughDistance)
				{
					this.ChangeState(LurkerGhost.ghostState.charge);
					return;
				}
				break;
			case LurkerGhost.ghostState.charge:
				this.ChargeAtPlayer();
				if (base.IsMine && (this.targetPosition - base.transform.position).sqrMagnitude < 0.25f)
				{
					if ((this.targetTransform.position - this.targetPosition).magnitude < this.minCatchDistance)
					{
						this.ChangeState(LurkerGhost.ghostState.possess);
						return;
					}
					this.huntedPassedTime = 0f;
					this.ChangeState(LurkerGhost.ghostState.patrol);
					return;
				}
				break;
			case LurkerGhost.ghostState.possess:
				if (this.targetTransform != null)
				{
					float num = this.SpookyMagicNumbers.x + MathF.Abs(MathF.Sin(Time.time * this.SpookyMagicNumbers.y));
					float num2 = this.HauntedMagicNumbers.x * MathF.Sin(Time.time * this.HauntedMagicNumbers.y) + this.HauntedMagicNumbers.z * MathF.Sin(Time.time * this.HauntedMagicNumbers.w);
					float num3 = 0.5f + 0.5f * MathF.Sin(Time.time * this.SpookyMagicNumbers.z);
					Vector3 vector = this.targetTransform.position + new Vector3(num * (float)Math.Sin((double)num2), num3, num * (float)Math.Cos((double)num2));
					base.transform.position = Vector3.MoveTowards(base.transform.position, vector, this.chargeSpeed);
					base.transform.rotation = Quaternion.LookRotation(base.transform.position - this.targetTransform.position);
				}
				if (base.IsMine)
				{
					this.huntedPassedTime += Time.deltaTime;
					if (this.huntedPassedTime >= this.PossessionDuration)
					{
						this.huntedPassedTime = 0f;
						if (this.hauntNeighbors && this.currentRepeatHuntTimes < this.maxRepeatHuntTimes && this.PickPlayer(this.maxRepeatHuntDistance))
						{
							this.currentRepeatHuntTimes++;
							this.ChangeState(LurkerGhost.ghostState.seek);
							return;
						}
						this.ChangeState(LurkerGhost.ghostState.patrol);
					}
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x170006D0 RID: 1744
		// (get) Token: 0x060045FE RID: 17918 RVA: 0x0014CC0D File Offset: 0x0014AE0D
		// (set) Token: 0x060045FF RID: 17919 RVA: 0x0014CC37 File Offset: 0x0014AE37
		[Networked]
		[NetworkedWeaved(0, 6)]
		private unsafe LurkerGhost.LurkerGhostData Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing LurkerGhost.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(LurkerGhost.LurkerGhostData*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing LurkerGhost.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(LurkerGhost.LurkerGhostData*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x06004600 RID: 17920 RVA: 0x0014CC62 File Offset: 0x0014AE62
		public override void WriteDataFusion()
		{
			this.Data = new LurkerGhost.LurkerGhostData(this.currentState, this.currentIndex, this.targetPlayer.ActorNumber, this.targetPosition);
		}

		// Token: 0x06004601 RID: 17921 RVA: 0x0014CC8C File Offset: 0x0014AE8C
		public override void ReadDataFusion()
		{
			this.ReadDataShared(this.Data.CurrentState, this.Data.CurrentIndex, this.Data.TargetActor, this.Data.TargetPos);
		}

		// Token: 0x06004602 RID: 17922 RVA: 0x0014CCD8 File Offset: 0x0014AED8
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			stream.SendNext(this.currentState);
			stream.SendNext(this.currentIndex);
			if (this.targetPlayer != null)
			{
				stream.SendNext(this.targetPlayer.ActorNumber);
			}
			else
			{
				stream.SendNext(-1);
			}
			stream.SendNext(this.targetPosition);
		}

		// Token: 0x06004603 RID: 17923 RVA: 0x0014CD54 File Offset: 0x0014AF54
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			LurkerGhost.ghostState ghostState = (LurkerGhost.ghostState)stream.ReceiveNext();
			int num = (int)stream.ReceiveNext();
			int num2 = (int)stream.ReceiveNext();
			Vector3 vector = (Vector3)stream.ReceiveNext();
			this.ReadDataShared(ghostState, num, num2, vector);
		}

		// Token: 0x06004604 RID: 17924 RVA: 0x0014CDAC File Offset: 0x0014AFAC
		private void ReadDataShared(LurkerGhost.ghostState state, int index, int targetActorNumber, Vector3 targetPos)
		{
			LurkerGhost.ghostState ghostState = this.currentState;
			this.currentState = state;
			this.currentIndex = index;
			NetPlayer netPlayer = this.targetPlayer;
			this.targetPlayer = NetworkSystem.Instance.GetPlayer(targetActorNumber);
			this.targetPosition = targetPos;
			float num = 10000f;
			if (!(in this.targetPosition).IsValid(in num))
			{
				RigContainer rigContainer;
				if (VRRigCache.Instance.TryGetVrrig(this.targetPlayer, out rigContainer))
				{
					this.targetPosition = (this.targetPlayer.IsLocal ? rigContainer.Rig.transform.position : rigContainer.Rig.syncPos);
				}
				else
				{
					this.targetPosition = base.transform.position;
				}
			}
			if (this.targetPlayer != netPlayer)
			{
				this.PickPlayer(this.targetPlayer);
			}
			if (ghostState != this.currentState || this.targetPlayer != netPlayer)
			{
				this.ChangeState(this.currentState);
			}
		}

		// Token: 0x06004605 RID: 17925 RVA: 0x0014CE8D File Offset: 0x0014B08D
		public override void OnOwnerChange(Player newOwner, Player previousOwner)
		{
			base.OnOwnerChange(newOwner, previousOwner);
			if (newOwner == PhotonNetwork.LocalPlayer)
			{
				this.ChangeState(this.currentState);
			}
		}

		// Token: 0x06004607 RID: 17927 RVA: 0x0014CFB0 File Offset: 0x0014B1B0
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x06004608 RID: 17928 RVA: 0x0014CFC8 File Offset: 0x0014B1C8
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x04004876 RID: 18550
		public float patrolSpeed = 3f;

		// Token: 0x04004877 RID: 18551
		public float seekSpeed = 6f;

		// Token: 0x04004878 RID: 18552
		public float chargeSpeed = 6f;

		// Token: 0x04004879 RID: 18553
		[Tooltip("Cooldown until the next time the ghost needs to hunt a new player")]
		public float cooldownDuration = 10f;

		// Token: 0x0400487A RID: 18554
		[Tooltip("Max Cooldown (randomized)")]
		public float maxCooldownDuration = 10f;

		// Token: 0x0400487B RID: 18555
		[Tooltip("How long the possession effects should last")]
		public float PossessionDuration = 15f;

		// Token: 0x0400487C RID: 18556
		[Tooltip("Hunted objects within this radius will get triggered ")]
		public float sphereColliderRadius = 2f;

		// Token: 0x0400487D RID: 18557
		[Tooltip("Maximum distance to the possible player to get hunted")]
		public float maxHuntDistance = 20f;

		// Token: 0x0400487E RID: 18558
		[Tooltip("Minimum distance from the player to start the possession effects")]
		public float minCatchDistance = 2f;

		// Token: 0x0400487F RID: 18559
		[Tooltip("Maximum distance to the possible player to get repeat hunted")]
		public float maxRepeatHuntDistance = 5f;

		// Token: 0x04004880 RID: 18560
		[Tooltip("Maximum times the lurker can haunt a nearby player before going back on cooldown")]
		public int maxRepeatHuntTimes = 3;

		// Token: 0x04004881 RID: 18561
		[Tooltip("Time in seconds before a haunted player can pass the lurker to another player by tagging")]
		public float tagCoolDown = 2f;

		// Token: 0x04004882 RID: 18562
		[Tooltip("UP & DOWN, IN & OUT")]
		public Vector3 SpookyMagicNumbers = new Vector3(1f, 1f, 1f);

		// Token: 0x04004883 RID: 18563
		[Tooltip("SPIN, SPIN, SPIN, SPIN")]
		public Vector4 HauntedMagicNumbers = new Vector4(1f, 2f, 3f, 1f);

		// Token: 0x04004884 RID: 18564
		[Tooltip("Haptic vibration when haunted by the ghost")]
		public float hapticStrength = 1f;

		// Token: 0x04004885 RID: 18565
		public float hapticDuration = 1.5f;

		// Token: 0x04004886 RID: 18566
		public GameObject waypointsContainer;

		// Token: 0x04004887 RID: 18567
		private ZoneBasedObject[] waypointRegions;

		// Token: 0x04004888 RID: 18568
		private ZoneBasedObject lastWaypointRegion;

		// Token: 0x04004889 RID: 18569
		private List<Transform> waypoints = new List<Transform>();

		// Token: 0x0400488A RID: 18570
		private Transform currentWaypoint;

		// Token: 0x0400488B RID: 18571
		public Material visibleMaterial;

		// Token: 0x0400488C RID: 18572
		public Material scryableMaterial;

		// Token: 0x0400488D RID: 18573
		public Material visibleMaterialBones;

		// Token: 0x0400488E RID: 18574
		public Material scryableMaterialBones;

		// Token: 0x0400488F RID: 18575
		public MeshRenderer meshRenderer;

		// Token: 0x04004890 RID: 18576
		public MeshRenderer bonesMeshRenderer;

		// Token: 0x04004891 RID: 18577
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04004892 RID: 18578
		public AudioClip patrolAudio;

		// Token: 0x04004893 RID: 18579
		public AudioClip huntAudio;

		// Token: 0x04004894 RID: 18580
		public AudioClip possessedAudio;

		// Token: 0x04004895 RID: 18581
		public ThrowableSetDressing scryingGlass;

		// Token: 0x04004896 RID: 18582
		public float scryingAngerAngle;

		// Token: 0x04004897 RID: 18583
		public float scryingAngerDelay;

		// Token: 0x04004898 RID: 18584
		public float seekAheadDistance;

		// Token: 0x04004899 RID: 18585
		public float seekCloseEnoughDistance;

		// Token: 0x0400489A RID: 18586
		private float scryingAngerAfterTimestamp;

		// Token: 0x0400489B RID: 18587
		private int currentRepeatHuntTimes;

		// Token: 0x0400489C RID: 18588
		public UnityAction<GameObject> TriggerHauntedObjects;

		// Token: 0x0400489D RID: 18589
		private int currentIndex;

		// Token: 0x0400489E RID: 18590
		private LurkerGhost.ghostState currentState;

		// Token: 0x0400489F RID: 18591
		private float cooldownTimeRemaining;

		// Token: 0x040048A0 RID: 18592
		private List<NetPlayer> possibleTargets;

		// Token: 0x040048A1 RID: 18593
		private NetPlayer targetPlayer;

		// Token: 0x040048A2 RID: 18594
		private Transform targetTransform;

		// Token: 0x040048A3 RID: 18595
		private float huntedPassedTime;

		// Token: 0x040048A4 RID: 18596
		private Vector3 targetPosition;

		// Token: 0x040048A5 RID: 18597
		private Quaternion targetRotation;

		// Token: 0x040048A6 RID: 18598
		private VRRig targetVRRig;

		// Token: 0x040048A7 RID: 18599
		private ShaderHashId _BlackAndWhite = "_BlackAndWhite";

		// Token: 0x040048A8 RID: 18600
		private VRRig lastHauntedVRRig;

		// Token: 0x040048A9 RID: 18601
		private float nextTagTime;

		// Token: 0x040048AA RID: 18602
		private NetPlayer passingPlayer;

		// Token: 0x040048AB RID: 18603
		[SerializeField]
		private bool hauntNeighbors = true;

		// Token: 0x040048AC RID: 18604
		[WeaverGenerated]
		[DefaultForProperty("Data", 0, 6)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private LurkerGhost.LurkerGhostData _Data;

		// Token: 0x02000B1C RID: 2844
		private enum ghostState
		{
			// Token: 0x040048AE RID: 18606
			patrol,
			// Token: 0x040048AF RID: 18607
			seek,
			// Token: 0x040048B0 RID: 18608
			charge,
			// Token: 0x040048B1 RID: 18609
			possess
		}

		// Token: 0x02000B1D RID: 2845
		[NetworkStructWeaved(6)]
		[StructLayout(LayoutKind.Explicit, Size = 24)]
		private struct LurkerGhostData : INetworkStruct
		{
			// Token: 0x170006D1 RID: 1745
			// (get) Token: 0x06004609 RID: 17929 RVA: 0x0014CFDC File Offset: 0x0014B1DC
			// (set) Token: 0x0600460A RID: 17930 RVA: 0x0014CFE4 File Offset: 0x0014B1E4
			public LurkerGhost.ghostState CurrentState { readonly get; set; }

			// Token: 0x170006D2 RID: 1746
			// (get) Token: 0x0600460B RID: 17931 RVA: 0x0014CFED File Offset: 0x0014B1ED
			// (set) Token: 0x0600460C RID: 17932 RVA: 0x0014CFF5 File Offset: 0x0014B1F5
			public int CurrentIndex { readonly get; set; }

			// Token: 0x170006D3 RID: 1747
			// (get) Token: 0x0600460D RID: 17933 RVA: 0x0014CFFE File Offset: 0x0014B1FE
			// (set) Token: 0x0600460E RID: 17934 RVA: 0x0014D006 File Offset: 0x0014B206
			public int TargetActor { readonly get; set; }

			// Token: 0x170006D4 RID: 1748
			// (get) Token: 0x0600460F RID: 17935 RVA: 0x0014D00F File Offset: 0x0014B20F
			// (set) Token: 0x06004610 RID: 17936 RVA: 0x0014D021 File Offset: 0x0014B221
			[Networked]
			public unsafe Vector3 TargetPos
			{
				readonly get
				{
					return *(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._TargetPos);
				}
				set
				{
					*(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._TargetPos) = value;
				}
			}

			// Token: 0x06004611 RID: 17937 RVA: 0x0014D034 File Offset: 0x0014B234
			public LurkerGhostData(LurkerGhost.ghostState state, int index, int actor, Vector3 pos)
			{
				this.CurrentState = state;
				this.CurrentIndex = index;
				this.TargetActor = actor;
				this.TargetPos = pos;
			}

			// Token: 0x040048B5 RID: 18613
			[FixedBufferProperty(typeof(Vector3), typeof(UnityValueSurrogate@ReaderWriter@UnityEngine_Vector3), 0, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(12)]
			private FixedStorage@3 _TargetPos;
		}
	}
}
