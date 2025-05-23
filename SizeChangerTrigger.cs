using System;
using UnityEngine;

// Token: 0x02000688 RID: 1672
public class SizeChangerTrigger : MonoBehaviour, IBuilderPieceComponent
{
	// Token: 0x1400005A RID: 90
	// (add) Token: 0x060029CE RID: 10702 RVA: 0x000CF174 File Offset: 0x000CD374
	// (remove) Token: 0x060029CF RID: 10703 RVA: 0x000CF1AC File Offset: 0x000CD3AC
	public event SizeChangerTrigger.SizeChangerTriggerEvent OnEnter;

	// Token: 0x1400005B RID: 91
	// (add) Token: 0x060029D0 RID: 10704 RVA: 0x000CF1E4 File Offset: 0x000CD3E4
	// (remove) Token: 0x060029D1 RID: 10705 RVA: 0x000CF21C File Offset: 0x000CD41C
	public event SizeChangerTrigger.SizeChangerTriggerEvent OnExit;

	// Token: 0x060029D2 RID: 10706 RVA: 0x000CF251 File Offset: 0x000CD451
	private void Awake()
	{
		this.myCollider = base.GetComponent<Collider>();
	}

	// Token: 0x060029D3 RID: 10707 RVA: 0x000CF25F File Offset: 0x000CD45F
	public void OnTriggerEnter(Collider other)
	{
		if (this.OnEnter != null)
		{
			this.OnEnter(other);
		}
	}

	// Token: 0x060029D4 RID: 10708 RVA: 0x000CF275 File Offset: 0x000CD475
	public void OnTriggerExit(Collider other)
	{
		if (this.OnExit != null)
		{
			this.OnExit(other);
		}
	}

	// Token: 0x060029D5 RID: 10709 RVA: 0x000CF28B File Offset: 0x000CD48B
	public Vector3 ClosestPoint(Vector3 position)
	{
		return this.myCollider.ClosestPoint(position);
	}

	// Token: 0x060029D6 RID: 10710 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnPieceCreate(int pieceType, int pieceId)
	{
	}

	// Token: 0x060029D7 RID: 10711 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnPieceDestroy()
	{
	}

	// Token: 0x060029D8 RID: 10712 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnPiecePlacementDeserialized()
	{
	}

	// Token: 0x060029D9 RID: 10713 RVA: 0x000CF299 File Offset: 0x000CD499
	public void OnPieceActivate()
	{
		Debug.LogError("Size Trigger Pieces no longer work, need reimplementation");
	}

	// Token: 0x060029DA RID: 10714 RVA: 0x000CF299 File Offset: 0x000CD499
	public void OnPieceDeactivate()
	{
		Debug.LogError("Size Trigger Pieces no longer work, need reimplementation");
	}

	// Token: 0x04002EF2 RID: 12018
	private Collider myCollider;

	// Token: 0x04002EF5 RID: 12021
	public bool builderEnterTrigger;

	// Token: 0x04002EF6 RID: 12022
	public bool builderExitOnEnterTrigger;

	// Token: 0x02000689 RID: 1673
	// (Invoke) Token: 0x060029DD RID: 10717
	public delegate void SizeChangerTriggerEvent(Collider other);
}
