using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000A39 RID: 2617
[DefaultExecutionOrder(5555)]
public class ZoneGraph : MonoBehaviour
{
	// Token: 0x06003E2E RID: 15918 RVA: 0x001277DC File Offset: 0x001259DC
	public static ZoneGraph Instance()
	{
		return ZoneGraph.gGraph;
	}

	// Token: 0x06003E2F RID: 15919 RVA: 0x001277E3 File Offset: 0x001259E3
	public static ZoneDef ColliderToZoneDef(BoxCollider collider)
	{
		if (!(collider == null))
		{
			return ZoneGraph.gGraph._colliderToZoneDef[collider];
		}
		return null;
	}

	// Token: 0x06003E30 RID: 15920 RVA: 0x00127800 File Offset: 0x00125A00
	public static ZoneNode ColliderToNode(BoxCollider collider)
	{
		if (!(collider == null))
		{
			return ZoneGraph.gGraph._colliderToNode[collider];
		}
		return ZoneNode.Null;
	}

	// Token: 0x06003E31 RID: 15921 RVA: 0x00127821 File Offset: 0x00125A21
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

	// Token: 0x06003E32 RID: 15922 RVA: 0x00127858 File Offset: 0x00125A58
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

	// Token: 0x06003E33 RID: 15923 RVA: 0x001278DC File Offset: 0x00125ADC
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

	// Token: 0x06003E34 RID: 15924 RVA: 0x00127941 File Offset: 0x00125B41
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

	// Token: 0x06003E35 RID: 15925 RVA: 0x0012797C File Offset: 0x00125B7C
	public static void Unregister(ZoneEntity entity)
	{
		ZoneGraph.gGraph._entityList.Remove(entity);
	}

	// Token: 0x040042D0 RID: 17104
	[SerializeField]
	private ZoneDef[] _zoneDefs = new ZoneDef[0];

	// Token: 0x040042D1 RID: 17105
	[SerializeField]
	private BoxCollider[] _colliders = new BoxCollider[0];

	// Token: 0x040042D2 RID: 17106
	[SerializeField]
	private ZoneNode[] _nodes = new ZoneNode[0];

	// Token: 0x040042D3 RID: 17107
	[Space]
	[NonSerialized]
	private Dictionary<BoxCollider, ZoneDef> _colliderToZoneDef = new Dictionary<BoxCollider, ZoneDef>(64);

	// Token: 0x040042D4 RID: 17108
	[Space]
	[NonSerialized]
	private Dictionary<BoxCollider, ZoneNode> _colliderToNode = new Dictionary<BoxCollider, ZoneNode>(64);

	// Token: 0x040042D5 RID: 17109
	[Space]
	[NonSerialized]
	private List<ZoneEntity> _entityList = new List<ZoneEntity>(16);

	// Token: 0x040042D6 RID: 17110
	private static ZoneGraph gGraph;
}
