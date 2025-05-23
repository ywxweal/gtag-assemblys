using System;
using UnityEngine;

// Token: 0x020009FA RID: 2554
[CreateAssetMenu(fileName = "New TeleportNode Definition", menuName = "Teleportation/TeleportNode Definition", order = 1)]
public class TeleportNodeDefinition : ScriptableObject
{
	// Token: 0x170005FD RID: 1533
	// (get) Token: 0x06003D13 RID: 15635 RVA: 0x0012222C File Offset: 0x0012042C
	public TeleportNode Forward
	{
		get
		{
			return this.forward;
		}
	}

	// Token: 0x170005FE RID: 1534
	// (get) Token: 0x06003D14 RID: 15636 RVA: 0x00122234 File Offset: 0x00120434
	public TeleportNode Backward
	{
		get
		{
			return this.backward;
		}
	}

	// Token: 0x06003D15 RID: 15637 RVA: 0x0012223C File Offset: 0x0012043C
	public void SetForward(TeleportNode node)
	{
		Debug.Log("registered fwd node " + node.name);
		this.forward = node;
	}

	// Token: 0x06003D16 RID: 15638 RVA: 0x0012225A File Offset: 0x0012045A
	public void SetBackward(TeleportNode node)
	{
		Debug.Log("registered bkwd node " + node.name);
		this.backward = node;
	}

	// Token: 0x040040D0 RID: 16592
	[SerializeField]
	private TeleportNode forward;

	// Token: 0x040040D1 RID: 16593
	[SerializeField]
	private TeleportNode backward;
}
