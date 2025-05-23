using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000A39 RID: 2617
[DefaultExecutionOrder(5555)]
public class ZoneGraph : MonoBehaviour
{
	// Token: 0x06003E2D RID: 15917 RVA: 0x00127704 File Offset: 0x00125904
	public static ZoneGraph Instance()
	{
		return ZoneGraph.gGraph;
	}

	// Token: 0x06003E2E RID: 15918 RVA: 0x0012770B File Offset: 0x0012590B
	public static ZoneDef ColliderToZoneDef(BoxCollider collider)
	{
		if (!(collider == null))
		{
			return ZoneGraph.gGraph._colliderToZoneDef[collider];
		}
		return null;
	}

	// Token: 0x06003E2F RID: 15919 RVA: 0x00127728 File Offset: 0x00125928
	public static ZoneNode ColliderToNode(BoxCollider collider)
	{
		if (!(collider == null))
		{
			return ZoneGraph.gGraph._colliderToNode[collider];
		}
		return ZoneNode.Null;
	}

	// Token: 0x06003E30 RID: 15920 RVA: 0x00127749 File Offset: 0x00125949
	private void Awake()
	{
		if (ZoneGraph.gGraph != null && ZoneGraph.gGraph != this)
		{
			Object.Destroy(this);
		}
		else
		{
			ZoneGraph.gGraph = this;
		}
		this.CompileColliderMaps(this._zoneDefs);
	}

	// Token: 0x06003E31 RID: 15921 RVA: 0x00127780 File Offset: 0x00125980
	private void CompileColliderMaps(ZoneDef[] zones)
	{
		foreach (ZoneDef zoneDef in zones)
		{
			for (int j = 0; j < zoneDef.colliders.Length; j++)
			{
				BoxCollider boxCollider = zoneDef.colliders[j];
				this._colliderToZoneDef[boxCollider] = zoneDef;
			}
		}
		for (int k = 0; k < this._colliders.Length; k++)
		{
			BoxCollider boxCollider2 = this._colliders[k];
			this._colliderToNode[boxCollider2] = this._nodes[k];
		}
	}

	// Token: 0x06003E32 RID: 15922 RVA: 0x00127804 File Offset: 0x00125A04
	public static int Compare(ZoneDef x, ZoneDef y)
	{
		if (x == null && y == null)
		{
			return 0;
		}
		if (x == null)
		{
			return 1;
		}
		if (y == null)
		{
			return -1;
		}
		int num = (int)x.zoneId;
		int num2 = num.CompareTo((int)y.zoneId);
		if (num2 == 0)
		{
			num = (int)x.subZoneId;
			num2 = num.CompareTo((int)y.subZoneId);
		}
		return num2;
	}

	// Token: 0x06003E33 RID: 15923 RVA: 0x00127869 File Offset: 0x00125A69
	public static void Register(ZoneEntity entity)
	{
		if (ZoneGraph.gGraph == null)
		{
			ZoneGraph.gGraph = Object.FindFirstObjectByType<ZoneGraph>();
		}
		if (!ZoneGraph.gGraph._entityList.Contains(entity))
		{
			ZoneGraph.gGraph._entityList.Add(entity);
		}
	}

	// Token: 0x06003E34 RID: 15924 RVA: 0x001278A4 File Offset: 0x00125AA4
	public static void Unregister(ZoneEntity entity)
	{
		ZoneGraph.gGraph._entityList.Remove(entity);
	}

	// Token: 0x040042CF RID: 17103
	[SerializeField]
	private ZoneDef[] _zoneDefs = new ZoneDef[0];

	// Token: 0x040042D0 RID: 17104
	[SerializeField]
	private BoxCollider[] _colliders = new BoxCollider[0];

	// Token: 0x040042D1 RID: 17105
	[SerializeField]
	private ZoneNode[] _nodes = new ZoneNode[0];

	// Token: 0x040042D2 RID: 17106
	[Space]
	[NonSerialized]
	private Dictionary<BoxCollider, ZoneDef> _colliderToZoneDef = new Dictionary<BoxCollider, ZoneDef>(64);

	// Token: 0x040042D3 RID: 17107
	[Space]
	[NonSerialized]
	private Dictionary<BoxCollider, ZoneNode> _colliderToNode = new Dictionary<BoxCollider, ZoneNode>(64);

	// Token: 0x040042D4 RID: 17108
	[Space]
	[NonSerialized]
	private List<ZoneEntity> _entityList = new List<ZoneEntity>(16);

	// Token: 0x040042D5 RID: 17109
	private static ZoneGraph gGraph;
}
