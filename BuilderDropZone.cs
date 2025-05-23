using System;
using System.Collections;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

// Token: 0x020004E5 RID: 1253
public class BuilderDropZone : MonoBehaviour
{
	// Token: 0x06001E5C RID: 7772 RVA: 0x00093E86 File Offset: 0x00092086
	private void Awake()
	{
		this.repelDirectionWorld = base.transform.TransformDirection(this.repelDirectionLocal.normalized);
	}

	// Token: 0x06001E5D RID: 7773 RVA: 0x00093EA4 File Offset: 0x000920A4
	private void OnTriggerEnter(Collider other)
	{
		if (!this.onEnter)
		{
			return;
		}
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		BuilderPieceCollider component = other.GetComponent<BuilderPieceCollider>();
		if (component != null)
		{
			BuilderPiece piece = component.piece;
			if (this.table != null && this.table.builderNetworking != null)
			{
				if (piece == null)
				{
					return;
				}
				if (this.dropType == BuilderDropZone.DropType.Recycle)
				{
					bool flag = piece.state != BuilderPiece.State.Displayed && piece.state != BuilderPiece.State.OnShelf && piece.state > BuilderPiece.State.AttachedAndPlaced;
					if (!piece.isBuiltIntoTable && flag)
					{
						this.table.builderNetworking.RequestRecyclePiece(piece.pieceId, piece.transform.position, piece.transform.rotation, true, -1);
						return;
					}
				}
				else
				{
					this.table.builderNetworking.PieceEnteredDropZone(piece, this.dropType, this.dropZoneID);
				}
			}
		}
	}

	// Token: 0x06001E5E RID: 7774 RVA: 0x00093F8E File Offset: 0x0009218E
	public Vector3 GetRepelDirectionWorld()
	{
		return this.repelDirectionWorld;
	}

	// Token: 0x06001E5F RID: 7775 RVA: 0x00093F98 File Offset: 0x00092198
	public void PlayEffect()
	{
		if (this.vfxRoot != null && !this.playingEffect)
		{
			this.vfxRoot.SetActive(true);
			this.playingEffect = true;
			if (this.sfxPrefab != null)
			{
				ObjectPools.instance.Instantiate(this.sfxPrefab, base.transform.position, base.transform.rotation, true);
			}
			base.StartCoroutine(this.DelayedStopEffect());
		}
	}

	// Token: 0x06001E60 RID: 7776 RVA: 0x00094011 File Offset: 0x00092211
	private IEnumerator DelayedStopEffect()
	{
		yield return new WaitForSeconds(this.effectDuration);
		this.vfxRoot.SetActive(false);
		this.playingEffect = false;
		yield break;
	}

	// Token: 0x06001E61 RID: 7777 RVA: 0x00094020 File Offset: 0x00092220
	private void OnTriggerExit(Collider other)
	{
		if (this.onEnter)
		{
			return;
		}
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		BuilderPieceCollider component = other.GetComponent<BuilderPieceCollider>();
		if (component != null)
		{
			BuilderPiece piece = component.piece;
			if (this.table != null && this.table.builderNetworking != null)
			{
				if (piece == null)
				{
					return;
				}
				if (this.dropType == BuilderDropZone.DropType.Recycle)
				{
					bool flag = piece.state != BuilderPiece.State.Displayed && piece.state != BuilderPiece.State.OnShelf && piece.state > BuilderPiece.State.AttachedAndPlaced;
					if (!piece.isBuiltIntoTable && flag)
					{
						this.table.builderNetworking.RequestRecyclePiece(piece.pieceId, piece.transform.position, piece.transform.rotation, true, -1);
						return;
					}
				}
				else
				{
					this.table.builderNetworking.PieceEnteredDropZone(piece, this.dropType, this.dropZoneID);
				}
			}
		}
	}

	// Token: 0x040021AA RID: 8618
	[SerializeField]
	private BuilderDropZone.DropType dropType;

	// Token: 0x040021AB RID: 8619
	[SerializeField]
	private bool onEnter = true;

	// Token: 0x040021AC RID: 8620
	[SerializeField]
	private GameObject vfxRoot;

	// Token: 0x040021AD RID: 8621
	[SerializeField]
	private GameObject sfxPrefab;

	// Token: 0x040021AE RID: 8622
	public float effectDuration = 1f;

	// Token: 0x040021AF RID: 8623
	private bool playingEffect;

	// Token: 0x040021B0 RID: 8624
	public bool overrideDirection;

	// Token: 0x040021B1 RID: 8625
	[SerializeField]
	private Vector3 repelDirectionLocal = Vector3.up;

	// Token: 0x040021B2 RID: 8626
	private Vector3 repelDirectionWorld = Vector3.up;

	// Token: 0x040021B3 RID: 8627
	[HideInInspector]
	public int dropZoneID = -1;

	// Token: 0x040021B4 RID: 8628
	internal BuilderTable table;

	// Token: 0x020004E6 RID: 1254
	public enum DropType
	{
		// Token: 0x040021B6 RID: 8630
		Invalid = -1,
		// Token: 0x040021B7 RID: 8631
		Repel,
		// Token: 0x040021B8 RID: 8632
		ReturnToShelf,
		// Token: 0x040021B9 RID: 8633
		BreakApart,
		// Token: 0x040021BA RID: 8634
		Recycle
	}
}
