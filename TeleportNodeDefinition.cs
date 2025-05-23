using System;
using UnityEngine;

// Token: 0x020009FA RID: 2554
[CreateAssetMenu(fileName = "New TeleportNode Definition", menuName = "Teleportation/TeleportNode Definition", order = 1)]
public class TeleportNodeDefinition : ScriptableObject
{
	// Token: 0x170005FD RID: 1533
	// (get) Token: 0x06003D14 RID: 15636 RVA: 0x00122304 File Offset: 0x00120504
	public TeleportNode Forward
	{
		get
		{
			return this.forward;
		}
	}

	// Token: 0x170005FE RID: 1534
	// (get) Token: 0x06003D15 RID: 15637 RVA: 0x0012230C File Offset: 0x0012050C
	public TeleportNode Backward
	{
		get
		{
			return this.backward;
		}
	}

	// Token: 0x06003D16 RID: 15638 RVA: 0x00122314 File Offset: 0x00120514
	public void SetForward(TeleportNode node)
	{
		Debug.Log("registered fwd node " + node.name);
		this.forward = node;
	}

	// Token: 0x06003D17 RID: 15639 RVA: 0x00122332 File Offset: 0x00120532
	public void SetBackward(TeleportNode node)
	{
		Debug.Log("registered bkwd node " + node.name);
		this.backward = node;
	}

	// Token: 0x040040D1 RID: 16593
	[SerializeField]
	private TeleportNode forward;

	// Token: 0x040040D2 RID: 16594
	[SerializeField]
	private TeleportNode backward;
}
