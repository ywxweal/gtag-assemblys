﻿using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000075 RID: 117
public class CritterVisuals : MonoBehaviour
{
	// Token: 0x1700002F RID: 47
	// (get) Token: 0x060002E1 RID: 737 RVA: 0x000122E7 File Offset: 0x000104E7
	public CritterAppearance Appearance
	{
		get
		{
			return this._appearance;
		}
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x000122F0 File Offset: 0x000104F0
	public void SetAppearance(CritterAppearance appearance)
	{
		this._appearance = appearance;
		float num = this._appearance.size.ClampSafe(0.25f, 1.5f);
		this.bodyRoot.localScale = new Vector3(num, num, num);
		if (!string.IsNullOrEmpty(appearance.hatName))
		{
			foreach (GameObject gameObject in this.hats)
			{
				gameObject.SetActive(gameObject.name == this._appearance.hatName);
			}
			this.hatRoot.gameObject.SetActive(true);
			return;
		}
		this.hatRoot.gameObject.SetActive(false);
	}

	// Token: 0x060002E3 RID: 739 RVA: 0x00012395 File Offset: 0x00010595
	public void ApplyMesh(Mesh newMesh)
	{
		this.myMeshFilter.sharedMesh = newMesh;
	}

	// Token: 0x060002E4 RID: 740 RVA: 0x000123A3 File Offset: 0x000105A3
	public void ApplyMaterial(Material mat)
	{
		this.myRenderer.sharedMaterial = mat;
	}

	// Token: 0x04000385 RID: 901
	public int critterType;

	// Token: 0x04000386 RID: 902
	[Header("Visuals")]
	public Transform bodyRoot;

	// Token: 0x04000387 RID: 903
	public MeshRenderer myRenderer;

	// Token: 0x04000388 RID: 904
	public MeshFilter myMeshFilter;

	// Token: 0x04000389 RID: 905
	public Transform hatRoot;

	// Token: 0x0400038A RID: 906
	public GameObject[] hats;

	// Token: 0x0400038B RID: 907
	private CritterAppearance _appearance;
}
