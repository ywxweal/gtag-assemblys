using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200054C RID: 1356
[NetworkBehaviourWeaved(337)]
public class FlockingManager : NetworkComponent
{
	// Token: 0x060020DC RID: 8412 RVA: 0x000A5244 File Offset: 0x000A3444
	protected override void Awake()
	{
		base.Awake();
		foreach (GameObject gameObject in this.fishAreaContainer)
		{
			Flocking[] componentsInChildren = gameObject.GetComponentsInChildren<Flocking>(false);
			FlockingManager.FishArea fishArea = new FlockingManager.FishArea();
			fishArea.id = gameObject.name;
			fishArea.colliders = gameObject.GetComponentsInChildren<BoxCollider>();
			fishArea.colliderCenter = fishArea.colliders[0].bounds.center;
			fishArea.fishList.AddRange(componentsInChildren);
			fishArea.zoneBasedObject = gameObject.GetComponent<ZoneBasedObject>();
			this.areaToWaypointDict[fishArea.id] = Vector3.zero;
			Flocking[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].FishArea = fishArea;
			}
			this.fishAreaList.Add(fishArea);
			this.allFish.AddRange(fishArea.fishList);
			SlingshotProjectileHitNotifier component = gameObject.GetComponent<SlingshotProjectileHitNotifier>();
			if (component != null)
			{
				component.OnProjectileTriggerEnter += this.ProjectileHitReceiver;
				component.OnProjectileTriggerExit += this.ProjectileHitExit;
			}
			else
			{
				Debug.LogError("Needs SlingshotProjectileHitNotifier added to each fish area");
			}
		}
	}

	// Token: 0x060020DD RID: 8413 RVA: 0x0001A3C3 File Offset: 0x000185C3
	private new void Start()
	{
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
	}

	// Token: 0x060020DE RID: 8414 RVA: 0x000A539C File Offset: 0x000A359C
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		this.fishAreaList.Clear();
		this.areaToWaypointDict.Clear();
		this.allFish.Clear();
		foreach (GameObject gameObject in this.fishAreaContainer)
		{
			SlingshotProjectileHitNotifier component = gameObject.GetComponent<SlingshotProjectileHitNotifier>();
			if (component != null)
			{
				component.OnProjectileTriggerExit -= this.ProjectileHitExit;
				component.OnProjectileTriggerEnter -= this.ProjectileHitReceiver;
			}
		}
	}

	// Token: 0x060020DF RID: 8415 RVA: 0x000A5444 File Offset: 0x000A3644
	private void Update()
	{
		if (Random.Range(0, 10000) < 50)
		{
			foreach (FlockingManager.FishArea fishArea in this.fishAreaList)
			{
				if (fishArea.zoneBasedObject != null)
				{
					fishArea.zoneBasedObject.gameObject.SetActive(fishArea.zoneBasedObject.IsLocalPlayerInZone());
				}
				fishArea.nextWaypoint = this.GetRandomPointInsideCollider(fishArea);
				this.areaToWaypointDict[fishArea.id] = fishArea.nextWaypoint;
				Debug.DrawLine(fishArea.nextWaypoint, Vector3.forward * 5f, Color.magenta);
			}
		}
	}

	// Token: 0x060020E0 RID: 8416 RVA: 0x000A5510 File Offset: 0x000A3710
	public Vector3 GetRandomPointInsideCollider(FlockingManager.FishArea fishArea)
	{
		int num = Random.Range(0, fishArea.colliders.Length);
		BoxCollider boxCollider = fishArea.colliders[num];
		Vector3 vector = boxCollider.size / 2f;
		Vector3 vector2 = new Vector3(Random.Range(-vector.x, vector.x), Random.Range(-vector.y, vector.y), Random.Range(-vector.z, vector.z));
		return boxCollider.transform.TransformPoint(vector2);
	}

	// Token: 0x060020E1 RID: 8417 RVA: 0x000A5590 File Offset: 0x000A3790
	public bool IsInside(Vector3 point, FlockingManager.FishArea fish)
	{
		foreach (BoxCollider boxCollider in fish.colliders)
		{
			Vector3 center = boxCollider.center;
			Vector3 vector = boxCollider.transform.InverseTransformPoint(point);
			vector -= center;
			Vector3 size = boxCollider.size;
			if (Mathf.Abs(vector.x) < size.x / 2f && Mathf.Abs(vector.y) < size.y / 2f && Mathf.Abs(vector.z) < size.z / 2f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060020E2 RID: 8418 RVA: 0x000A562C File Offset: 0x000A382C
	public Vector3 RestrictPointToArea(Vector3 point, FlockingManager.FishArea fish)
	{
		Vector3 vector = default(Vector3);
		float num = float.MaxValue;
		foreach (BoxCollider boxCollider in fish.colliders)
		{
			Vector3 center = boxCollider.center;
			Vector3 vector2 = boxCollider.transform.InverseTransformPoint(point);
			Vector3 vector3 = vector2 - center;
			Vector3 size = boxCollider.size;
			float num2 = size.x / 2f;
			float num3 = size.y / 2f;
			float num4 = size.z / 2f;
			if (Mathf.Abs(vector3.x) < num2 && Mathf.Abs(vector3.y) < num3 && Mathf.Abs(vector3.z) < num4)
			{
				return point;
			}
			Vector3 vector4 = new Vector3(center.x - num2, center.y - num3, center.z - num4);
			Vector3 vector5 = new Vector3(center.x + num2, center.y + num3, center.z + num4);
			Vector3 vector6 = new Vector3(Mathf.Clamp(vector2.x, vector4.x, vector5.x), Mathf.Clamp(vector2.y, vector4.y, vector5.y), Mathf.Clamp(vector2.z, vector4.z, vector5.z));
			float num5 = Vector3.Distance(vector2, vector6);
			if (num5 < num)
			{
				num = num5;
				if (num5 > 1f)
				{
					Vector3 vector7 = Vector3.Normalize(vector2 - vector6);
					vector = boxCollider.transform.TransformPoint(vector6 + vector7 * 1f);
				}
				else
				{
					vector = point;
				}
			}
		}
		return vector;
	}

	// Token: 0x060020E3 RID: 8419 RVA: 0x000A57DC File Offset: 0x000A39DC
	private void ProjectileHitReceiver(SlingshotProjectile projectile, Collider collider1)
	{
		bool flag = projectile.CompareTag(this.foodProjectileTag);
		FlockingManager.FishFood fishFood = new FlockingManager.FishFood
		{
			collider = (collider1 as BoxCollider),
			isRealFood = flag,
			slingshotProjectile = projectile
		};
		UnityAction<FlockingManager.FishFood> unityAction = this.onFoodDetected;
		if (unityAction == null)
		{
			return;
		}
		unityAction(fishFood);
	}

	// Token: 0x060020E4 RID: 8420 RVA: 0x000A5827 File Offset: 0x000A3A27
	private void ProjectileHitExit(SlingshotProjectile projectile, Collider collider2)
	{
		UnityAction<BoxCollider> unityAction = this.onFoodDestroyed;
		if (unityAction == null)
		{
			return;
		}
		unityAction(collider2 as BoxCollider);
	}

	// Token: 0x1700034D RID: 845
	// (get) Token: 0x060020E5 RID: 8421 RVA: 0x000A583F File Offset: 0x000A3A3F
	// (set) Token: 0x060020E6 RID: 8422 RVA: 0x000A5869 File Offset: 0x000A3A69
	[Networked]
	[NetworkedWeaved(0, 337)]
	public unsafe FlockingData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing FlockingManager.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(FlockingData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing FlockingManager.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(FlockingData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x060020E7 RID: 8423 RVA: 0x000A5894 File Offset: 0x000A3A94
	public override void WriteDataFusion()
	{
		this.Data = new FlockingData(this.allFish);
	}

	// Token: 0x060020E8 RID: 8424 RVA: 0x000A58A8 File Offset: 0x000A3AA8
	public override void ReadDataFusion()
	{
		for (int i = 0; i < this.Data.count; i++)
		{
			Vector3 vector = this.Data.Positions[i];
			Quaternion quaternion = this.Data.Rotations[i];
			this.allFish[i].SetSyncPosRot(vector, quaternion);
		}
	}

	// Token: 0x060020E9 RID: 8425 RVA: 0x000023F4 File Offset: 0x000005F4
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060020EA RID: 8426 RVA: 0x000023F4 File Offset: 0x000005F4
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060020EB RID: 8427 RVA: 0x000A5913 File Offset: 0x000A3B13
	public static void RegisterAvoidPoint(GameObject obj)
	{
		FlockingManager.avoidPoints.Add(obj);
	}

	// Token: 0x060020EC RID: 8428 RVA: 0x000A5920 File Offset: 0x000A3B20
	public static void UnregisterAvoidPoint(GameObject obj)
	{
		FlockingManager.avoidPoints.Remove(obj);
	}

	// Token: 0x060020EF RID: 8431 RVA: 0x000A596E File Offset: 0x000A3B6E
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x060020F0 RID: 8432 RVA: 0x000A5986 File Offset: 0x000A3B86
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x0400250A RID: 9482
	public List<GameObject> fishAreaContainer;

	// Token: 0x0400250B RID: 9483
	public string foodProjectileTag = "WaterBalloonProjectile";

	// Token: 0x0400250C RID: 9484
	private Dictionary<string, Vector3> areaToWaypointDict = new Dictionary<string, Vector3>();

	// Token: 0x0400250D RID: 9485
	private List<FlockingManager.FishArea> fishAreaList = new List<FlockingManager.FishArea>();

	// Token: 0x0400250E RID: 9486
	private List<Flocking> allFish = new List<Flocking>();

	// Token: 0x0400250F RID: 9487
	public UnityAction<FlockingManager.FishFood> onFoodDetected;

	// Token: 0x04002510 RID: 9488
	public UnityAction<BoxCollider> onFoodDestroyed;

	// Token: 0x04002511 RID: 9489
	private bool hasBeenSerialized;

	// Token: 0x04002512 RID: 9490
	public static readonly List<GameObject> avoidPoints = new List<GameObject>();

	// Token: 0x04002513 RID: 9491
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 337)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private FlockingData _Data;

	// Token: 0x0200054D RID: 1357
	public class FishArea
	{
		// Token: 0x04002514 RID: 9492
		public string id;

		// Token: 0x04002515 RID: 9493
		public List<Flocking> fishList = new List<Flocking>();

		// Token: 0x04002516 RID: 9494
		public Vector3 colliderCenter;

		// Token: 0x04002517 RID: 9495
		public BoxCollider[] colliders;

		// Token: 0x04002518 RID: 9496
		public Vector3 nextWaypoint = Vector3.zero;

		// Token: 0x04002519 RID: 9497
		public ZoneBasedObject zoneBasedObject;
	}

	// Token: 0x0200054E RID: 1358
	public class FishFood
	{
		// Token: 0x0400251A RID: 9498
		public BoxCollider collider;

		// Token: 0x0400251B RID: 9499
		public bool isRealFood;

		// Token: 0x0400251C RID: 9500
		public SlingshotProjectile slingshotProjectile;
	}
}
