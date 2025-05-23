using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000108 RID: 264
public class ButterflySwarmManager : MonoBehaviour
{
	// Token: 0x1700008B RID: 139
	// (get) Token: 0x060006A9 RID: 1705 RVA: 0x000264EF File Offset: 0x000246EF
	// (set) Token: 0x060006AA RID: 1706 RVA: 0x000264F7 File Offset: 0x000246F7
	public float PerchedFlapSpeed { get; private set; }

	// Token: 0x1700008C RID: 140
	// (get) Token: 0x060006AB RID: 1707 RVA: 0x00026500 File Offset: 0x00024700
	// (set) Token: 0x060006AC RID: 1708 RVA: 0x00026508 File Offset: 0x00024708
	public float PerchedFlapPhase { get; private set; }

	// Token: 0x1700008D RID: 141
	// (get) Token: 0x060006AD RID: 1709 RVA: 0x00026511 File Offset: 0x00024711
	// (set) Token: 0x060006AE RID: 1710 RVA: 0x00026519 File Offset: 0x00024719
	public float BeeSpeed { get; private set; }

	// Token: 0x1700008E RID: 142
	// (get) Token: 0x060006AF RID: 1711 RVA: 0x00026522 File Offset: 0x00024722
	// (set) Token: 0x060006B0 RID: 1712 RVA: 0x0002652A File Offset: 0x0002472A
	public float BeeMaxTravelTime { get; private set; }

	// Token: 0x1700008F RID: 143
	// (get) Token: 0x060006B1 RID: 1713 RVA: 0x00026533 File Offset: 0x00024733
	// (set) Token: 0x060006B2 RID: 1714 RVA: 0x0002653B File Offset: 0x0002473B
	public float BeeAcceleration { get; private set; }

	// Token: 0x17000090 RID: 144
	// (get) Token: 0x060006B3 RID: 1715 RVA: 0x00026544 File Offset: 0x00024744
	// (set) Token: 0x060006B4 RID: 1716 RVA: 0x0002654C File Offset: 0x0002474C
	public float BeeJitterStrength { get; private set; }

	// Token: 0x17000091 RID: 145
	// (get) Token: 0x060006B5 RID: 1717 RVA: 0x00026555 File Offset: 0x00024755
	// (set) Token: 0x060006B6 RID: 1718 RVA: 0x0002655D File Offset: 0x0002475D
	public float BeeJitterDamping { get; private set; }

	// Token: 0x17000092 RID: 146
	// (get) Token: 0x060006B7 RID: 1719 RVA: 0x00026566 File Offset: 0x00024766
	// (set) Token: 0x060006B8 RID: 1720 RVA: 0x0002656E File Offset: 0x0002476E
	public float BeeMaxJitterRadius { get; private set; }

	// Token: 0x17000093 RID: 147
	// (get) Token: 0x060006B9 RID: 1721 RVA: 0x00026577 File Offset: 0x00024777
	// (set) Token: 0x060006BA RID: 1722 RVA: 0x0002657F File Offset: 0x0002477F
	public float BeeNearDestinationRadius { get; private set; }

	// Token: 0x17000094 RID: 148
	// (get) Token: 0x060006BB RID: 1723 RVA: 0x00026588 File Offset: 0x00024788
	// (set) Token: 0x060006BC RID: 1724 RVA: 0x00026590 File Offset: 0x00024790
	public float DestRotationAlignmentSpeed { get; private set; }

	// Token: 0x17000095 RID: 149
	// (get) Token: 0x060006BD RID: 1725 RVA: 0x00026599 File Offset: 0x00024799
	// (set) Token: 0x060006BE RID: 1726 RVA: 0x000265A1 File Offset: 0x000247A1
	public Vector3 TravellingLocalRotationEuler { get; private set; }

	// Token: 0x17000096 RID: 150
	// (get) Token: 0x060006BF RID: 1727 RVA: 0x000265AA File Offset: 0x000247AA
	// (set) Token: 0x060006C0 RID: 1728 RVA: 0x000265B2 File Offset: 0x000247B2
	public Quaternion TravellingLocalRotation { get; private set; }

	// Token: 0x17000097 RID: 151
	// (get) Token: 0x060006C1 RID: 1729 RVA: 0x000265BB File Offset: 0x000247BB
	// (set) Token: 0x060006C2 RID: 1730 RVA: 0x000265C3 File Offset: 0x000247C3
	public float AvoidPointRadius { get; private set; }

	// Token: 0x17000098 RID: 152
	// (get) Token: 0x060006C3 RID: 1731 RVA: 0x000265CC File Offset: 0x000247CC
	// (set) Token: 0x060006C4 RID: 1732 RVA: 0x000265D4 File Offset: 0x000247D4
	public float BeeMinFlowerDuration { get; private set; }

	// Token: 0x17000099 RID: 153
	// (get) Token: 0x060006C5 RID: 1733 RVA: 0x000265DD File Offset: 0x000247DD
	// (set) Token: 0x060006C6 RID: 1734 RVA: 0x000265E5 File Offset: 0x000247E5
	public float BeeMaxFlowerDuration { get; private set; }

	// Token: 0x1700009A RID: 154
	// (get) Token: 0x060006C7 RID: 1735 RVA: 0x000265EE File Offset: 0x000247EE
	// (set) Token: 0x060006C8 RID: 1736 RVA: 0x000265F6 File Offset: 0x000247F6
	public Color[] BeeColors { get; private set; }

	// Token: 0x060006C9 RID: 1737 RVA: 0x00026600 File Offset: 0x00024800
	private void Awake()
	{
		this.TravellingLocalRotation = Quaternion.Euler(this.TravellingLocalRotationEuler);
		this.butterflies = new List<AnimatedButterfly>(this.numBees);
		for (int i = 0; i < this.numBees; i++)
		{
			AnimatedButterfly animatedButterfly = default(AnimatedButterfly);
			animatedButterfly.InitVisual(this.beePrefab, this);
			if (this.BeeColors.Length != 0)
			{
				animatedButterfly.SetColor(this.BeeColors[i % this.BeeColors.Length]);
			}
			this.butterflies.Add(animatedButterfly);
		}
	}

	// Token: 0x060006CA RID: 1738 RVA: 0x00026688 File Offset: 0x00024888
	private void Start()
	{
		foreach (XSceneRef xsceneRef in this.perchSections)
		{
			GameObject gameObject;
			if (xsceneRef.TryResolve(out gameObject))
			{
				List<GameObject> list = new List<GameObject>();
				this.allPerchZones.Add(list);
				foreach (object obj in gameObject.transform)
				{
					Transform transform = (Transform)obj;
					list.Add(transform.gameObject);
				}
			}
		}
		this.OnSeedChange();
		RandomTimedSeedManager.instance.AddCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	// Token: 0x060006CB RID: 1739 RVA: 0x00026748 File Offset: 0x00024948
	private void OnDestroy()
	{
		RandomTimedSeedManager.instance.RemoveCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	// Token: 0x060006CC RID: 1740 RVA: 0x00026760 File Offset: 0x00024960
	private void Update()
	{
		for (int i = 0; i < this.butterflies.Count; i++)
		{
			AnimatedButterfly animatedButterfly = this.butterflies[i];
			animatedButterfly.UpdateVisual(RandomTimedSeedManager.instance.currentSyncTime, this);
			this.butterflies[i] = animatedButterfly;
		}
	}

	// Token: 0x060006CD RID: 1741 RVA: 0x000267B0 File Offset: 0x000249B0
	private void OnSeedChange()
	{
		SRand srand = new SRand(RandomTimedSeedManager.instance.seed);
		List<List<GameObject>> list = new List<List<GameObject>>(this.allPerchZones.Count);
		for (int i = 0; i < this.allPerchZones.Count; i++)
		{
			List<GameObject> list2 = new List<GameObject>();
			list2.AddRange(this.allPerchZones[i]);
			list.Add(list2);
		}
		List<GameObject> list3 = new List<GameObject>(this.loopSizePerBee);
		List<float> list4 = new List<float>(this.loopSizePerBee);
		for (int j = 0; j < this.butterflies.Count; j++)
		{
			AnimatedButterfly animatedButterfly = this.butterflies[j];
			animatedButterfly.SetFlapSpeed(srand.NextFloat(this.minFlapSpeed, this.maxFlapSpeed));
			list3.Clear();
			list4.Clear();
			this.PickPoints(this.loopSizePerBee, list, ref srand, list3);
			for (int k = 0; k < list3.Count; k++)
			{
				list4.Add(srand.NextFloat(this.BeeMinFlowerDuration, this.BeeMaxFlowerDuration));
			}
			if (list3.Count == 0)
			{
				this.butterflies.Clear();
				return;
			}
			animatedButterfly.InitRoute(list3, list4, this);
			this.butterflies[j] = animatedButterfly;
		}
	}

	// Token: 0x060006CE RID: 1742 RVA: 0x000268F4 File Offset: 0x00024AF4
	private void PickPoints(int n, List<List<GameObject>> pickBuffer, ref SRand rand, List<GameObject> resultBuffer)
	{
		int num = rand.NextInt(0, pickBuffer.Count);
		int num2 = -1;
		int num3 = n - 2;
		while (resultBuffer.Count < n)
		{
			int num4;
			if (resultBuffer.Count < num3)
			{
				num4 = rand.NextIntWithExclusion(0, pickBuffer.Count, num2);
			}
			else
			{
				num4 = rand.NextIntWithExclusion2(0, pickBuffer.Count, num2, num);
			}
			int num5 = 10;
			while (num4 == num2 || pickBuffer[num4].Count == 0)
			{
				num4 = (num4 + 1) % pickBuffer.Count;
				num5--;
				if (num5 <= 0)
				{
					return;
				}
			}
			num2 = num4;
			List<GameObject> list = pickBuffer[num2];
			while (list.Count == 0)
			{
				num2 = (num2 + 1) % pickBuffer.Count;
				list = pickBuffer[num2];
			}
			resultBuffer.Add(list[list.Count - 1]);
			list.RemoveAt(list.Count - 1);
		}
	}

	// Token: 0x04000803 RID: 2051
	[SerializeField]
	private XSceneRef[] perchSections;

	// Token: 0x04000804 RID: 2052
	[SerializeField]
	private int loopSizePerBee;

	// Token: 0x04000805 RID: 2053
	[SerializeField]
	private int numBees;

	// Token: 0x04000806 RID: 2054
	[SerializeField]
	private MeshRenderer beePrefab;

	// Token: 0x04000807 RID: 2055
	[SerializeField]
	private float maxFlapSpeed;

	// Token: 0x04000808 RID: 2056
	[SerializeField]
	private float minFlapSpeed;

	// Token: 0x04000819 RID: 2073
	private List<AnimatedButterfly> butterflies;

	// Token: 0x0400081A RID: 2074
	private List<List<GameObject>> allPerchZones = new List<List<GameObject>>();
}
