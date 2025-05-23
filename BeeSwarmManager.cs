using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000107 RID: 263
public class BeeSwarmManager : MonoBehaviour
{
	// Token: 0x1700007F RID: 127
	// (get) Token: 0x06000687 RID: 1671 RVA: 0x00026046 File Offset: 0x00024246
	// (set) Token: 0x06000688 RID: 1672 RVA: 0x0002604E File Offset: 0x0002424E
	public BeePerchPoint BeeHive { get; private set; }

	// Token: 0x17000080 RID: 128
	// (get) Token: 0x06000689 RID: 1673 RVA: 0x00026057 File Offset: 0x00024257
	// (set) Token: 0x0600068A RID: 1674 RVA: 0x0002605F File Offset: 0x0002425F
	public float BeeSpeed { get; private set; }

	// Token: 0x17000081 RID: 129
	// (get) Token: 0x0600068B RID: 1675 RVA: 0x00026068 File Offset: 0x00024268
	// (set) Token: 0x0600068C RID: 1676 RVA: 0x00026070 File Offset: 0x00024270
	public float BeeMaxTravelTime { get; private set; }

	// Token: 0x17000082 RID: 130
	// (get) Token: 0x0600068D RID: 1677 RVA: 0x00026079 File Offset: 0x00024279
	// (set) Token: 0x0600068E RID: 1678 RVA: 0x00026081 File Offset: 0x00024281
	public float BeeAcceleration { get; private set; }

	// Token: 0x17000083 RID: 131
	// (get) Token: 0x0600068F RID: 1679 RVA: 0x0002608A File Offset: 0x0002428A
	// (set) Token: 0x06000690 RID: 1680 RVA: 0x00026092 File Offset: 0x00024292
	public float BeeJitterStrength { get; private set; }

	// Token: 0x17000084 RID: 132
	// (get) Token: 0x06000691 RID: 1681 RVA: 0x0002609B File Offset: 0x0002429B
	// (set) Token: 0x06000692 RID: 1682 RVA: 0x000260A3 File Offset: 0x000242A3
	public float BeeJitterDamping { get; private set; }

	// Token: 0x17000085 RID: 133
	// (get) Token: 0x06000693 RID: 1683 RVA: 0x000260AC File Offset: 0x000242AC
	// (set) Token: 0x06000694 RID: 1684 RVA: 0x000260B4 File Offset: 0x000242B4
	public float BeeMaxJitterRadius { get; private set; }

	// Token: 0x17000086 RID: 134
	// (get) Token: 0x06000695 RID: 1685 RVA: 0x000260BD File Offset: 0x000242BD
	// (set) Token: 0x06000696 RID: 1686 RVA: 0x000260C5 File Offset: 0x000242C5
	public float BeeNearDestinationRadius { get; private set; }

	// Token: 0x17000087 RID: 135
	// (get) Token: 0x06000697 RID: 1687 RVA: 0x000260CE File Offset: 0x000242CE
	// (set) Token: 0x06000698 RID: 1688 RVA: 0x000260D6 File Offset: 0x000242D6
	public float AvoidPointRadius { get; private set; }

	// Token: 0x17000088 RID: 136
	// (get) Token: 0x06000699 RID: 1689 RVA: 0x000260DF File Offset: 0x000242DF
	// (set) Token: 0x0600069A RID: 1690 RVA: 0x000260E7 File Offset: 0x000242E7
	public float BeeMinFlowerDuration { get; private set; }

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x0600069B RID: 1691 RVA: 0x000260F0 File Offset: 0x000242F0
	// (set) Token: 0x0600069C RID: 1692 RVA: 0x000260F8 File Offset: 0x000242F8
	public float BeeMaxFlowerDuration { get; private set; }

	// Token: 0x1700008A RID: 138
	// (get) Token: 0x0600069D RID: 1693 RVA: 0x00026101 File Offset: 0x00024301
	// (set) Token: 0x0600069E RID: 1694 RVA: 0x00026109 File Offset: 0x00024309
	public float GeneralBuzzRange { get; private set; }

	// Token: 0x0600069F RID: 1695 RVA: 0x00026114 File Offset: 0x00024314
	private void Awake()
	{
		this.bees = new List<AnimatedBee>(this.numBees);
		for (int i = 0; i < this.numBees; i++)
		{
			AnimatedBee animatedBee = default(AnimatedBee);
			animatedBee.InitVisual(this.beePrefab, this);
			this.bees.Add(animatedBee);
		}
		this.playerCamera = Camera.main.transform;
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x00026178 File Offset: 0x00024378
	private void Start()
	{
		foreach (XSceneRef xsceneRef in this.flowerSections)
		{
			GameObject gameObject;
			if (xsceneRef.TryResolve(out gameObject))
			{
				foreach (BeePerchPoint beePerchPoint in gameObject.GetComponentsInChildren<BeePerchPoint>())
				{
					this.allPerchPoints.Add(beePerchPoint);
				}
			}
		}
		this.OnSeedChange();
		RandomTimedSeedManager.instance.AddCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x000261F8 File Offset: 0x000243F8
	private void OnDestroy()
	{
		RandomTimedSeedManager.instance.RemoveCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x00026210 File Offset: 0x00024410
	private void Update()
	{
		Vector3 position = this.playerCamera.transform.position;
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		float num = 1f / (float)this.bees.Count;
		float num2 = float.PositiveInfinity;
		float num3 = this.GeneralBuzzRange * this.GeneralBuzzRange;
		int num4 = 0;
		for (int i = 0; i < this.bees.Count; i++)
		{
			AnimatedBee animatedBee = this.bees[i];
			animatedBee.UpdateVisual(RandomTimedSeedManager.instance.currentSyncTime, this);
			Vector3 position2 = animatedBee.visual.transform.position;
			float sqrMagnitude = (position2 - position).sqrMagnitude;
			if (sqrMagnitude < num2)
			{
				vector = position2;
				num2 = sqrMagnitude;
			}
			if (sqrMagnitude < num3)
			{
				vector2 += position2;
				num4++;
			}
			this.bees[i] = animatedBee;
		}
		this.nearbyBeeBuzz.transform.position = vector;
		if (num4 > 0)
		{
			this.generalBeeBuzz.transform.position = vector2 / (float)num4;
			this.generalBeeBuzz.enabled = true;
			return;
		}
		this.generalBeeBuzz.enabled = false;
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x00026340 File Offset: 0x00024540
	private void OnSeedChange()
	{
		SRand srand = new SRand(RandomTimedSeedManager.instance.seed);
		List<BeePerchPoint> list = new List<BeePerchPoint>(this.allPerchPoints.Count);
		List<BeePerchPoint> list2 = new List<BeePerchPoint>(this.loopSizePerBee);
		List<float> list3 = new List<float>(this.loopSizePerBee);
		for (int i = 0; i < this.bees.Count; i++)
		{
			AnimatedBee animatedBee = this.bees[i];
			list2 = new List<BeePerchPoint>(this.loopSizePerBee);
			list3 = new List<float>(this.loopSizePerBee);
			this.PickPoints(this.loopSizePerBee, list, this.allPerchPoints, ref srand, list2);
			for (int j = 0; j < list2.Count; j++)
			{
				list3.Add(srand.NextFloat(this.BeeMinFlowerDuration, this.BeeMaxFlowerDuration));
			}
			animatedBee.InitRoute(list2, list3, this);
			animatedBee.InitRouteTimestamps();
			this.bees[i] = animatedBee;
		}
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x00026434 File Offset: 0x00024634
	private void PickPoints(int n, List<BeePerchPoint> pickBuffer, List<BeePerchPoint> allPerchPoints, ref SRand rand, List<BeePerchPoint> resultBuffer)
	{
		resultBuffer.Add(this.BeeHive);
		n--;
		int num = 100;
		while (pickBuffer.Count < n && num-- > 0)
		{
			n -= pickBuffer.Count;
			resultBuffer.AddRange(pickBuffer);
			pickBuffer.Clear();
			pickBuffer.AddRange(allPerchPoints);
			rand.Shuffle<BeePerchPoint>(pickBuffer);
		}
		resultBuffer.AddRange(pickBuffer.GetRange(pickBuffer.Count - n, n));
		pickBuffer.RemoveRange(pickBuffer.Count - n, n);
	}

	// Token: 0x060006A5 RID: 1701 RVA: 0x000264B5 File Offset: 0x000246B5
	public static void RegisterAvoidPoint(GameObject obj)
	{
		BeeSwarmManager.avoidPoints.Add(obj);
	}

	// Token: 0x060006A6 RID: 1702 RVA: 0x000264C2 File Offset: 0x000246C2
	public static void UnregisterAvoidPoint(GameObject obj)
	{
		BeeSwarmManager.avoidPoints.Remove(obj);
	}

	// Token: 0x040007EC RID: 2028
	[SerializeField]
	private XSceneRef[] flowerSections;

	// Token: 0x040007ED RID: 2029
	[SerializeField]
	private int loopSizePerBee;

	// Token: 0x040007EE RID: 2030
	[SerializeField]
	private int numBees;

	// Token: 0x040007EF RID: 2031
	[SerializeField]
	private MeshRenderer beePrefab;

	// Token: 0x040007F0 RID: 2032
	[SerializeField]
	private AudioSource nearbyBeeBuzz;

	// Token: 0x040007F1 RID: 2033
	[SerializeField]
	private AudioSource generalBeeBuzz;

	// Token: 0x040007F2 RID: 2034
	private GameObject[] flowerSectionsResolved;

	// Token: 0x040007FF RID: 2047
	private List<AnimatedBee> bees;

	// Token: 0x04000800 RID: 2048
	private Transform playerCamera;

	// Token: 0x04000801 RID: 2049
	private List<BeePerchPoint> allPerchPoints = new List<BeePerchPoint>();

	// Token: 0x04000802 RID: 2050
	public static readonly List<GameObject> avoidPoints = new List<GameObject>();
}
