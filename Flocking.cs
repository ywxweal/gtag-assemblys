using System;
using GorillaExtensions;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000549 RID: 1353
public class Flocking : MonoBehaviour
{
	// Token: 0x1700034C RID: 844
	// (get) Token: 0x060020C3 RID: 8387 RVA: 0x000A491D File Offset: 0x000A2B1D
	// (set) Token: 0x060020C4 RID: 8388 RVA: 0x000A4925 File Offset: 0x000A2B25
	public FlockingManager.FishArea FishArea { get; set; }

	// Token: 0x060020C5 RID: 8389 RVA: 0x000A492E File Offset: 0x000A2B2E
	private void Awake()
	{
		this.manager = base.GetComponentInParent<FlockingManager>();
	}

	// Token: 0x060020C6 RID: 8390 RVA: 0x000A493C File Offset: 0x000A2B3C
	private void Start()
	{
		this.speed = Random.Range(this.minSpeed, this.maxSpeed);
		this.fishState = Flocking.FishState.patrol;
	}

	// Token: 0x060020C7 RID: 8391 RVA: 0x000A495C File Offset: 0x000A2B5C
	private void OnDisable()
	{
		FlockingManager flockingManager = this.manager;
		flockingManager.onFoodDetected = (UnityAction<FlockingManager.FishFood>)Delegate.Remove(flockingManager.onFoodDetected, new UnityAction<FlockingManager.FishFood>(this.HandleOnFoodDetected));
		FlockingManager flockingManager2 = this.manager;
		flockingManager2.onFoodDestroyed = (UnityAction<BoxCollider>)Delegate.Remove(flockingManager2.onFoodDestroyed, new UnityAction<BoxCollider>(this.HandleOnFoodDestroyed));
		FlockingUpdateManager.UnregisterFlocking(this);
	}

	// Token: 0x060020C8 RID: 8392 RVA: 0x000A49C0 File Offset: 0x000A2BC0
	public void InvokeUpdate()
	{
		if (this.manager == null)
		{
			this.manager = base.GetComponentInParent<FlockingManager>();
		}
		this.AvoidPlayerHands();
		this.MaybeTurn();
		switch (this.fishState)
		{
		case Flocking.FishState.flock:
			this.Flock(this.FishArea.nextWaypoint);
			this.SwitchState(Flocking.FishState.patrol);
			break;
		case Flocking.FishState.patrol:
			if (Random.Range(0, 10) < 2)
			{
				this.SwitchState(Flocking.FishState.flock);
			}
			break;
		case Flocking.FishState.followFood:
			if (this.isTurning)
			{
				return;
			}
			if (this.isRealFood)
			{
				if ((double)Vector3.Distance(base.transform.position, this.projectileGameObject.transform.position) > this.FollowFoodStopDistance)
				{
					this.FollowFood();
				}
				else
				{
					this.followingFood = false;
					this.Flock(this.projectileGameObject.transform.position);
					this.feedingTimeStarted += Time.deltaTime;
					if (this.feedingTimeStarted > this.eatFoodDuration)
					{
						this.SwitchState(Flocking.FishState.patrol);
					}
				}
			}
			else if (Vector3.Distance(base.transform.position, this.projectileGameObject.transform.position) > this.FollowFakeFoodStopDistance)
			{
				this.FollowFood();
			}
			else
			{
				this.followingFood = false;
				this.SwitchState(Flocking.FishState.patrol);
			}
			break;
		}
		if (!this.followingFood)
		{
			base.transform.Translate(0f, 0f, this.speed * Time.deltaTime);
		}
		this.pos = base.transform.position;
		this.rot = base.transform.rotation;
	}

	// Token: 0x060020C9 RID: 8393 RVA: 0x000A4B5C File Offset: 0x000A2D5C
	private void MaybeTurn()
	{
		if (!this.manager.IsInside(base.transform.position, this.FishArea))
		{
			this.Turn(this.FishArea.colliderCenter);
			if (Vector3.Angle(this.FishArea.colliderCenter - base.transform.position, Vector3.forward) > 5f)
			{
				this.isTurning = true;
				return;
			}
		}
		else
		{
			this.isTurning = false;
		}
	}

	// Token: 0x060020CA RID: 8394 RVA: 0x000A4BD4 File Offset: 0x000A2DD4
	private void Turn(Vector3 towardPoint)
	{
		this.isTurning = true;
		Quaternion quaternion = Quaternion.LookRotation(towardPoint - base.transform.position);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, quaternion, this.rotationSpeed * Time.deltaTime);
	}

	// Token: 0x060020CB RID: 8395 RVA: 0x000A4C27 File Offset: 0x000A2E27
	private void SwitchState(Flocking.FishState state)
	{
		this.fishState = state;
	}

	// Token: 0x060020CC RID: 8396 RVA: 0x000A4C30 File Offset: 0x000A2E30
	private void Flock(Vector3 nextGoal)
	{
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		float num = 1f;
		int num2 = 0;
		foreach (Flocking flocking in this.FishArea.fishList)
		{
			if (flocking.gameObject != base.gameObject)
			{
				float num3 = Vector3.Distance(flocking.transform.position, base.transform.position);
				if (num3 <= this.maxNeighbourDistance)
				{
					vector += flocking.transform.position;
					num2++;
					if (num3 < this.flockingAvoidanceDistance)
					{
						vector2 += base.transform.position - flocking.transform.position;
					}
					num += flocking.speed;
				}
			}
		}
		if (num2 > 0)
		{
			this.fishState = Flocking.FishState.flock;
			vector = vector / (float)num2 + (nextGoal - base.transform.position);
			this.speed = num / (float)num2;
			this.speed = Mathf.Clamp(this.speed, this.minSpeed, this.maxSpeed);
			Vector3 vector3 = vector + vector2 - base.transform.position;
			if (vector3 != Vector3.zero)
			{
				Quaternion quaternion = Quaternion.LookRotation(vector3);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, quaternion, this.rotationSpeed * Time.deltaTime);
			}
		}
	}

	// Token: 0x060020CD RID: 8397 RVA: 0x000A4DD4 File Offset: 0x000A2FD4
	private void HandleOnFoodDetected(FlockingManager.FishFood fishFood)
	{
		bool flag = false;
		foreach (BoxCollider boxCollider in this.FishArea.colliders)
		{
			if (fishFood.collider == boxCollider)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		this.SwitchState(Flocking.FishState.followFood);
		this.feedingTimeStarted = 0f;
		this.projectileGameObject = fishFood.slingshotProjectile.gameObject;
		this.isRealFood = fishFood.isRealFood;
	}

	// Token: 0x060020CE RID: 8398 RVA: 0x000A4E44 File Offset: 0x000A3044
	private void HandleOnFoodDestroyed(BoxCollider collider)
	{
		bool flag = false;
		foreach (BoxCollider boxCollider in this.FishArea.colliders)
		{
			if (collider == boxCollider)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		this.SwitchState(Flocking.FishState.patrol);
		this.projectileGameObject = null;
		this.followingFood = false;
	}

	// Token: 0x060020CF RID: 8399 RVA: 0x000A4E98 File Offset: 0x000A3098
	private void FollowFood()
	{
		this.followingFood = true;
		Quaternion quaternion = Quaternion.LookRotation(this.projectileGameObject.transform.position - base.transform.position);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, quaternion, this.rotationSpeed * Time.deltaTime);
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.projectileGameObject.transform.position, this.speed * this.followFoodSpeedMult * Time.deltaTime);
	}

	// Token: 0x060020D0 RID: 8400 RVA: 0x000A4F38 File Offset: 0x000A3138
	private void AvoidPlayerHands()
	{
		foreach (GameObject gameObject in FlockingManager.avoidPoints)
		{
			Vector3 position = gameObject.transform.position;
			if ((base.transform.position - position).IsShorterThan(this.avointPointRadius))
			{
				Vector3 randomPointInsideCollider = this.manager.GetRandomPointInsideCollider(this.FishArea);
				this.Turn(randomPointInsideCollider);
				this.speed = this.avoidHandSpeed;
			}
		}
	}

	// Token: 0x060020D1 RID: 8401 RVA: 0x000A4FD0 File Offset: 0x000A31D0
	internal void SetSyncPosRot(Vector3 syncPos, Quaternion syncRot)
	{
		if (this.manager == null)
		{
			this.manager = base.GetComponentInParent<FlockingManager>();
		}
		if (this.FishArea == null)
		{
			Debug.LogError("FISH AREA NULL");
		}
		if ((in syncRot).IsValid())
		{
			this.rot = syncRot;
		}
		float num = 10000f;
		if ((in syncPos).IsValid(in num))
		{
			this.pos = this.manager.RestrictPointToArea(syncPos, this.FishArea);
		}
	}

	// Token: 0x060020D2 RID: 8402 RVA: 0x000A5044 File Offset: 0x000A3244
	private void OnEnable()
	{
		if (this.manager == null)
		{
			this.manager = base.GetComponentInParent<FlockingManager>();
		}
		FlockingManager flockingManager = this.manager;
		flockingManager.onFoodDetected = (UnityAction<FlockingManager.FishFood>)Delegate.Combine(flockingManager.onFoodDetected, new UnityAction<FlockingManager.FishFood>(this.HandleOnFoodDetected));
		FlockingManager flockingManager2 = this.manager;
		flockingManager2.onFoodDestroyed = (UnityAction<BoxCollider>)Delegate.Combine(flockingManager2.onFoodDestroyed, new UnityAction<BoxCollider>(this.HandleOnFoodDestroyed));
		FlockingUpdateManager.RegisterFlocking(this);
	}

	// Token: 0x040024E7 RID: 9447
	[Tooltip("Speed is randomly generated from min and max speed")]
	public float minSpeed = 2f;

	// Token: 0x040024E8 RID: 9448
	public float maxSpeed = 4f;

	// Token: 0x040024E9 RID: 9449
	public float rotationSpeed = 360f;

	// Token: 0x040024EA RID: 9450
	[Tooltip("Maximum distance to the neighbours to form a flocking group")]
	public float maxNeighbourDistance = 4f;

	// Token: 0x040024EB RID: 9451
	public float eatFoodDuration = 10f;

	// Token: 0x040024EC RID: 9452
	[Tooltip("How fast should it follow the food? This value multiplies by the current speed")]
	public float followFoodSpeedMult = 3f;

	// Token: 0x040024ED RID: 9453
	[Tooltip("How fast should it run away from players hand?")]
	public float avoidHandSpeed = 1.2f;

	// Token: 0x040024EE RID: 9454
	[FormerlySerializedAs("avoidanceDistance")]
	[Tooltip("When flocking they will avoid each other if the distance between them is less than this value")]
	public float flockingAvoidanceDistance = 2f;

	// Token: 0x040024EF RID: 9455
	[Tooltip("Follow the fish food until they are this far from it")]
	[FormerlySerializedAs("distanceToFollowFood")]
	public double FollowFoodStopDistance = 0.20000000298023224;

	// Token: 0x040024F0 RID: 9456
	[Tooltip("Follow any fake fish food until they are this far from it")]
	[FormerlySerializedAs("distanceToFollowFakeFood")]
	public float FollowFakeFoodStopDistance = 2f;

	// Token: 0x040024F1 RID: 9457
	private float speed;

	// Token: 0x040024F2 RID: 9458
	private Vector3 averageHeading;

	// Token: 0x040024F3 RID: 9459
	private Vector3 averagePosition;

	// Token: 0x040024F4 RID: 9460
	private float feedingTimeStarted;

	// Token: 0x040024F5 RID: 9461
	private GameObject projectileGameObject;

	// Token: 0x040024F6 RID: 9462
	private bool followingFood;

	// Token: 0x040024F7 RID: 9463
	private FlockingManager manager;

	// Token: 0x040024F8 RID: 9464
	private GameObjectManagerWithId _fishSceneGameObjectsManager;

	// Token: 0x040024F9 RID: 9465
	private UnityEvent<string, Transform> sendIdEvent;

	// Token: 0x040024FA RID: 9466
	private Flocking.FishState fishState;

	// Token: 0x040024FB RID: 9467
	[HideInInspector]
	public Vector3 pos;

	// Token: 0x040024FC RID: 9468
	[HideInInspector]
	public Quaternion rot;

	// Token: 0x040024FD RID: 9469
	private float velocity;

	// Token: 0x040024FE RID: 9470
	private bool isTurning;

	// Token: 0x040024FF RID: 9471
	private bool isRealFood;

	// Token: 0x04002500 RID: 9472
	public float avointPointRadius = 0.5f;

	// Token: 0x04002501 RID: 9473
	private float cacheSpeed;

	// Token: 0x0200054A RID: 1354
	public enum FishState
	{
		// Token: 0x04002504 RID: 9476
		flock,
		// Token: 0x04002505 RID: 9477
		patrol,
		// Token: 0x04002506 RID: 9478
		followFood
	}
}
