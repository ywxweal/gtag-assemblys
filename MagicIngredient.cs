using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200067E RID: 1662
[Obsolete("replaced with ThrowableSetDressing.cs")]
public class MagicIngredient : TransferrableObject
{
	// Token: 0x0600297E RID: 10622 RVA: 0x000CE0B0 File Offset: 0x000CC2B0
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.item = this.worldShareableInstance;
		this.grabPtInitParent = this.anchor.transform.parent;
	}

	// Token: 0x0600297F RID: 10623 RVA: 0x000CE0DC File Offset: 0x000CC2DC
	private void ReParent()
	{
		Transform transform = this.anchor.transform;
		base.gameObject.transform.parent = transform;
		transform.parent = this.grabPtInitParent;
	}

	// Token: 0x06002980 RID: 10624 RVA: 0x000CE112 File Offset: 0x000CC312
	public void Disable()
	{
		this.DropItem();
		base.OnDisable();
		if (this.item)
		{
			this.item.OnDisable();
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x04002E8E RID: 11918
	[FormerlySerializedAs("IngredientType")]
	public MagicIngredientType IngredientTypeSO;

	// Token: 0x04002E8F RID: 11919
	public Transform rootParent;

	// Token: 0x04002E90 RID: 11920
	private WorldShareableItem item;

	// Token: 0x04002E91 RID: 11921
	private Transform grabPtInitParent;
}
