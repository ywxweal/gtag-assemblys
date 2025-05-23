using System;
using UnityEngine;

// Token: 0x02000688 RID: 1672
public class SizeChangerTrigger : MonoBehaviour, IBuilderPieceComponent
{
	// Token: 0x1400005A RID: 90
	// (add) Token: 0x060029CF RID: 10703 RVA: 0x000CF218 File Offset: 0x000CD418
	// (remove) Token: 0x060029D0 RID: 10704 RVA: 0x000CF250 File Offset: 0x000CD450
	public event SizeChangerTrigger.SizeChangerTriggerEvent OnEnter;

	// Token: 0x1400005B RID: 91
	// (add) Token: 0x060029D1 RID: 10705 RVA: 0x000CF288 File Offset: 0x000CD488
	// (remove) Token: 0x060029D2 RID: 10706 RVA: 0x000CF2C0 File Offset: 0x000CD4C0
	public event SizeChangerTrigger.SizeChangerTriggerEvent OnExit;

	// Token: 0x060029D3 RID: 10707 RVA: 0x000CF2F5 File Offset: 0x000CD4F5
	private void Awake()
	{
		this.myCollider = base.GetComponent<Collider>();
	}

	// Token: 0x060029D4 RID: 10708 RVA: 0x000CF303 File Offset: 0x000CD503
	public void OnTriggerEnter(Collider other)
	{
		if (this.OnEnter != null)
		{
			this.OnEnter(other);
		}
	}

	// Token: 0x060029D5 RID: 10709 RVA: 0x000CF319 File Offset: 0x000CD519
	public void OnTriggerExit(Collider other)
	{
		if (this.OnExit != null)
		{
			this.OnExit(other);
		}
	}

	// Token: 0x060029D6 RID: 10710 RVA: 0x000CF32F File Offset: 0x000CD52F
	public Vector3 ClosestPoint(Vector3 position)
	{
		return this.myCollider.ClosestPoint(position);
	}

	// Token: 0x060029D7 RID: 10711 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnPieceCreate(int pieceType, int pieceId)
	{
	}

	// Token: 0x060029D8 RID: 10712 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnPieceDestroy()
	{
	}

	// Token: 0x060029D9 RID: 10713 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnPiecePlacementDeserialized()
	{
	}

	// Token: 0x060029DA RID: 10714 RVA: 0x000CF33D File Offset: 0x000CD53D
	public void OnPieceActivate()
	{
		Debug.LogError("Size Trigger Pieces no longer work, need reimplementation");
	}

	// Token: 0x060029DB RID: 10715 RVA: 0x000CF33D File Offset: 0x000CD53D
	public void OnPieceDeactivate()
	{
		Debug.LogError("Size Trigger Pieces no longer work, need reimplementation");
	}

	// Token: 0x04002EF4 RID: 12020
	private Collider myCollider;

	// Token: 0x04002EF7 RID: 12023
	public bool builderEnterTrigger;

	// Token: 0x04002EF8 RID: 12024
	public bool builderExitOnEnterTrigger;

	// Token: 0x02000689 RID: 1673
	// (Invoke) Token: 0x060029DE RID: 10718
	public delegate void SizeChangerTriggerEvent(Collider other);
}
