using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000743 RID: 1859
[NetworkBehaviourWeaved(1)]
public class WanderingGhost : NetworkComponent
{
	// Token: 0x06002E73 RID: 11891 RVA: 0x000E7E78 File Offset: 0x000E6078
	protected override void Start()
	{
		base.Start();
		this.waypointRegions = this.waypointsContainer.GetComponentsInChildren<ZoneBasedObject>();
		this.idlePassedTime = 0f;
		ThrowableSetDressing[] array = this.allFlowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].anchor.position = this.flowerDisabledPosition;
		}
		base.Invoke("DelayedStart", 0.5f);
	}

	// Token: 0x06002E74 RID: 11892 RVA: 0x000E7EDF File Offset: 0x000E60DF
	private void DelayedStart()
	{
		this.PickNextWaypoint();
		base.transform.position = this.currentWaypoint._transform.position;
		this.PickNextWaypoint();
		this.ChangeState(WanderingGhost.ghostState.patrol);
	}

	// Token: 0x06002E75 RID: 11893 RVA: 0x000E7F10 File Offset: 0x000E6110
	private void LateUpdate()
	{
		this.UpdateState();
		this.hoverVelocity -= this.mrenderer.transform.localPosition * this.hoverRectifyForce * Time.deltaTime;
		this.hoverVelocity += Random.insideUnitSphere * this.hoverRandomForce * Time.deltaTime;
		this.hoverVelocity = Vector3.MoveTowards(this.hoverVelocity, Vector3.zero, this.hoverDrag * Time.deltaTime);
		this.mrenderer.transform.localPosition += this.hoverVelocity * Time.deltaTime;
	}

	// Token: 0x06002E76 RID: 11894 RVA: 0x000E7FD4 File Offset: 0x000E61D4
	private void PickNextWaypoint()
	{
		if (this.waypoints.Count == 0 || this.lastWaypointRegion == null || !this.lastWaypointRegion.IsLocalPlayerInZone())
		{
			ZoneBasedObject zoneBasedObject = ZoneBasedObject.SelectRandomEligible(this.waypointRegions, this.debugForceWaypointRegion);
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
				this.waypoints.Add(new WanderingGhost.Waypoint(transform.name.Contains("_v_"), transform));
			}
		}
		int num = Random.Range(0, this.waypoints.Count);
		this.currentWaypoint = this.waypoints[num];
		this.waypoints.RemoveAt(num);
	}

	// Token: 0x06002E77 RID: 11895 RVA: 0x000E80E4 File Offset: 0x000E62E4
	private void Patrol()
	{
		this.idlePassedTime = 0f;
		this.mrenderer.sharedMaterial = this.scryableMaterial;
		Transform transform = this.currentWaypoint._transform;
		base.transform.position = Vector3.MoveTowards(base.transform.position, transform.position, this.patrolSpeed * Time.deltaTime);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.LookRotation(transform.position - base.transform.position), 360f * Time.deltaTime);
	}

	// Token: 0x06002E78 RID: 11896 RVA: 0x000E8188 File Offset: 0x000E6388
	private bool MaybeHideGhost()
	{
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.sphereColliderRadius, this.hitColliders);
		for (int i = 0; i < num; i++)
		{
			if (this.hitColliders[i].gameObject.IsOnLayer(UnityLayer.GorillaHand) || this.hitColliders[i].gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider))
			{
				this.ChangeState(WanderingGhost.ghostState.patrol);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002E79 RID: 11897 RVA: 0x000E81F4 File Offset: 0x000E63F4
	private void ChangeState(WanderingGhost.ghostState newState)
	{
		this.currentState = newState;
		this.mrenderer.sharedMaterial = ((newState == WanderingGhost.ghostState.idle) ? this.visibleMaterial : this.scryableMaterial);
		if (newState == WanderingGhost.ghostState.patrol)
		{
			this.audioSource.GTStop();
			this.audioSource.volume = this.patrolVolume;
			this.audioSource.clip = this.patrolAudio;
			this.audioSource.GTPlay();
			return;
		}
		if (newState != WanderingGhost.ghostState.idle)
		{
			return;
		}
		this.audioSource.GTStop();
		this.audioSource.volume = this.idleVolume;
		this.audioSource.GTPlayOneShot(this.appearAudio.GetRandomItem<AudioClip>(), 1f);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.SpawnFlowerNearby();
		}
	}

	// Token: 0x06002E7A RID: 11898 RVA: 0x000E82B0 File Offset: 0x000E64B0
	private void UpdateState()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		WanderingGhost.ghostState ghostState = this.currentState;
		if (ghostState != WanderingGhost.ghostState.patrol)
		{
			if (ghostState != WanderingGhost.ghostState.idle)
			{
				return;
			}
			this.idlePassedTime += Time.deltaTime;
			if (this.idlePassedTime >= this.idleStayDuration || this.MaybeHideGhost())
			{
				this.PickNextWaypoint();
				this.ChangeState(WanderingGhost.ghostState.patrol);
			}
		}
		else
		{
			if (this.currentWaypoint._transform == null)
			{
				this.PickNextWaypoint();
				return;
			}
			this.Patrol();
			if (Vector3.Distance(base.transform.position, this.currentWaypoint._transform.position) < 0.2f)
			{
				if (this.currentWaypoint._visible)
				{
					this.ChangeState(WanderingGhost.ghostState.idle);
					return;
				}
				this.PickNextWaypoint();
				return;
			}
		}
	}

	// Token: 0x06002E7B RID: 11899 RVA: 0x000E8374 File Offset: 0x000E6574
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

	// Token: 0x1700048A RID: 1162
	// (get) Token: 0x06002E7C RID: 11900 RVA: 0x000E83D5 File Offset: 0x000E65D5
	// (set) Token: 0x06002E7D RID: 11901 RVA: 0x000E83FF File Offset: 0x000E65FF
	[Networked]
	[NetworkedWeaved(0, 1)]
	private unsafe WanderingGhost.ghostState Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing WanderingGhost.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return (WanderingGhost.ghostState)this.Ptr[0];
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing WanderingGhost.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			this.Ptr[0] = (int)value;
		}
	}

	// Token: 0x06002E7E RID: 11902 RVA: 0x000E842A File Offset: 0x000E662A
	public override void WriteDataFusion()
	{
		this.Data = this.currentState;
	}

	// Token: 0x06002E7F RID: 11903 RVA: 0x000E8438 File Offset: 0x000E6638
	public override void ReadDataFusion()
	{
		this.ReadDataShared(this.Data);
	}

	// Token: 0x06002E80 RID: 11904 RVA: 0x000E8446 File Offset: 0x000E6646
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		stream.SendNext(this.currentState);
	}

	// Token: 0x06002E81 RID: 11905 RVA: 0x000E8468 File Offset: 0x000E6668
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		WanderingGhost.ghostState ghostState = (WanderingGhost.ghostState)stream.ReceiveNext();
		this.ReadDataShared(ghostState);
	}

	// Token: 0x06002E82 RID: 11906 RVA: 0x000E8496 File Offset: 0x000E6696
	private void ReadDataShared(WanderingGhost.ghostState state)
	{
		WanderingGhost.ghostState ghostState = this.currentState;
		this.currentState = state;
		if (ghostState != this.currentState)
		{
			this.ChangeState(this.currentState);
		}
	}

	// Token: 0x06002E83 RID: 11907 RVA: 0x000E84B9 File Offset: 0x000E66B9
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		base.OnOwnerChange(newOwner, previousOwner);
		if (newOwner == PhotonNetwork.LocalPlayer)
		{
			this.ChangeState(this.currentState);
		}
	}

	// Token: 0x06002E84 RID: 11908 RVA: 0x000E84D8 File Offset: 0x000E66D8
	private void SpawnFlowerNearby()
	{
		Vector3 vector = base.transform.position + Vector3.down * 0.25f;
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(base.transform.position + Random.insideUnitCircle.x0y() * this.flowerSpawnRadius, Vector3.down), out raycastHit, 3f, this.flowerGroundMask))
		{
			vector = raycastHit.point;
		}
		ThrowableSetDressing throwableSetDressing = null;
		int num = 0;
		foreach (ThrowableSetDressing throwableSetDressing2 in this.allFlowers)
		{
			if (!throwableSetDressing2.InHand())
			{
				num++;
				if (Random.Range(0, num) == 0)
				{
					throwableSetDressing = throwableSetDressing2;
				}
			}
		}
		if (throwableSetDressing != null)
		{
			if (!throwableSetDressing.IsLocalOwnedWorldShareable)
			{
				throwableSetDressing.WorldShareableRequestOwnership();
			}
			throwableSetDressing.SetWillTeleport();
			throwableSetDressing.transform.position = vector;
			throwableSetDressing.StartRespawnTimer(this.flowerSpawnDuration);
		}
	}

	// Token: 0x06002E86 RID: 11910 RVA: 0x000E8618 File Offset: 0x000E6818
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06002E87 RID: 11911 RVA: 0x000E8630 File Offset: 0x000E6830
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x040034ED RID: 13549
	public float patrolSpeed = 3f;

	// Token: 0x040034EE RID: 13550
	public float idleStayDuration = 5f;

	// Token: 0x040034EF RID: 13551
	public float sphereColliderRadius = 2f;

	// Token: 0x040034F0 RID: 13552
	public ThrowableSetDressing[] allFlowers;

	// Token: 0x040034F1 RID: 13553
	public Vector3 flowerDisabledPosition;

	// Token: 0x040034F2 RID: 13554
	public float flowerSpawnRadius;

	// Token: 0x040034F3 RID: 13555
	public float flowerSpawnDuration;

	// Token: 0x040034F4 RID: 13556
	public LayerMask flowerGroundMask;

	// Token: 0x040034F5 RID: 13557
	public MeshRenderer mrenderer;

	// Token: 0x040034F6 RID: 13558
	public Material visibleMaterial;

	// Token: 0x040034F7 RID: 13559
	public Material scryableMaterial;

	// Token: 0x040034F8 RID: 13560
	public GameObject waypointsContainer;

	// Token: 0x040034F9 RID: 13561
	private ZoneBasedObject[] waypointRegions;

	// Token: 0x040034FA RID: 13562
	private ZoneBasedObject lastWaypointRegion;

	// Token: 0x040034FB RID: 13563
	private List<WanderingGhost.Waypoint> waypoints = new List<WanderingGhost.Waypoint>();

	// Token: 0x040034FC RID: 13564
	private WanderingGhost.Waypoint currentWaypoint;

	// Token: 0x040034FD RID: 13565
	public string debugForceWaypointRegion;

	// Token: 0x040034FE RID: 13566
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040034FF RID: 13567
	public AudioClip[] appearAudio;

	// Token: 0x04003500 RID: 13568
	public float idleVolume;

	// Token: 0x04003501 RID: 13569
	public AudioClip patrolAudio;

	// Token: 0x04003502 RID: 13570
	public float patrolVolume;

	// Token: 0x04003503 RID: 13571
	private WanderingGhost.ghostState currentState;

	// Token: 0x04003504 RID: 13572
	private float idlePassedTime;

	// Token: 0x04003505 RID: 13573
	public UnityAction<GameObject> TriggerHauntedObjects;

	// Token: 0x04003506 RID: 13574
	private Vector3 hoverVelocity;

	// Token: 0x04003507 RID: 13575
	public float hoverRectifyForce;

	// Token: 0x04003508 RID: 13576
	public float hoverRandomForce;

	// Token: 0x04003509 RID: 13577
	public float hoverDrag;

	// Token: 0x0400350A RID: 13578
	private const int maxColliders = 10;

	// Token: 0x0400350B RID: 13579
	private Collider[] hitColliders = new Collider[10];

	// Token: 0x0400350C RID: 13580
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private WanderingGhost.ghostState _Data;

	// Token: 0x02000744 RID: 1860
	[Serializable]
	public struct Waypoint
	{
		// Token: 0x06002E88 RID: 11912 RVA: 0x000E8644 File Offset: 0x000E6844
		public Waypoint(bool visible, Transform tr)
		{
			this._visible = visible;
			this._transform = tr;
		}

		// Token: 0x0400350D RID: 13581
		[Tooltip("The ghost will be visible when its reached to this waypoint")]
		public bool _visible;

		// Token: 0x0400350E RID: 13582
		public Transform _transform;
	}

	// Token: 0x02000745 RID: 1861
	private enum ghostState
	{
		// Token: 0x04003510 RID: 13584
		patrol,
		// Token: 0x04003511 RID: 13585
		idle
	}
}
