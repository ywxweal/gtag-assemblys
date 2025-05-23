using System;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x020004DF RID: 1247
public class BuilderArmShelf : MonoBehaviour
{
	// Token: 0x06001E16 RID: 7702 RVA: 0x00092410 File Offset: 0x00090610
	private void Start()
	{
		this.ownerRig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x06001E17 RID: 7703 RVA: 0x0009241E File Offset: 0x0009061E
	public bool IsOwnedLocally()
	{
		return this.ownerRig != null && this.ownerRig.isLocal;
	}

	// Token: 0x06001E18 RID: 7704 RVA: 0x0009243B File Offset: 0x0009063B
	public bool CanAttachToArmPiece()
	{
		return this.ownerRig != null && this.ownerRig.scaleFactor >= 1f;
	}

	// Token: 0x06001E19 RID: 7705 RVA: 0x00092464 File Offset: 0x00090664
	public void DropAttachedPieces()
	{
		if (this.ownerRig != null && this.piece != null)
		{
			Vector3 vector = Vector3.zero;
			if (this.piece.firstChildPiece == null)
			{
				return;
			}
			BuilderTable table = this.piece.GetTable();
			Vector3 vector2 = table.roomCenter.position - this.piece.transform.position;
			vector2.Normalize();
			Vector3 vector3 = Quaternion.Euler(0f, 180f, 0f) * vector2;
			vector = BuilderTable.DROP_ZONE_REPEL * vector3;
			BuilderPiece builderPiece = this.piece.firstChildPiece;
			while (builderPiece != null)
			{
				table.RequestDropPiece(builderPiece, builderPiece.transform.position + vector3 * 0.1f, builderPiece.transform.rotation, vector, Vector3.zero);
				builderPiece = builderPiece.nextSiblingPiece;
			}
		}
	}

	// Token: 0x04002161 RID: 8545
	[HideInInspector]
	public BuilderPiece piece;

	// Token: 0x04002162 RID: 8546
	public Transform pieceAnchor;

	// Token: 0x04002163 RID: 8547
	private VRRig ownerRig;
}
