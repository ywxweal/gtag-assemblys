using System;
using UnityEngine;

// Token: 0x02000373 RID: 883
[ExecuteInEditMode]
public class SimpleResizable : MonoBehaviour
{
	// Token: 0x1700024B RID: 587
	// (get) Token: 0x06001472 RID: 5234 RVA: 0x00063AAD File Offset: 0x00061CAD
	public Vector3 PivotPosition
	{
		get
		{
			return this._pivotTransform.position;
		}
	}

	// Token: 0x1700024C RID: 588
	// (get) Token: 0x06001473 RID: 5235 RVA: 0x00063ABA File Offset: 0x00061CBA
	// (set) Token: 0x06001474 RID: 5236 RVA: 0x00063AC2 File Offset: 0x00061CC2
	public Vector3 DefaultSize { get; private set; }

	// Token: 0x1700024D RID: 589
	// (get) Token: 0x06001475 RID: 5237 RVA: 0x00063ACB File Offset: 0x00061CCB
	// (set) Token: 0x06001476 RID: 5238 RVA: 0x00063AD3 File Offset: 0x00061CD3
	public Mesh OriginalMesh { get; private set; }

	// Token: 0x06001477 RID: 5239 RVA: 0x00063ADC File Offset: 0x00061CDC
	public void SetNewSize(Vector3 newSize)
	{
		this._newSize = newSize;
	}

	// Token: 0x06001478 RID: 5240 RVA: 0x00063AE8 File Offset: 0x00061CE8
	private void Awake()
	{
		this._meshFilter = base.GetComponent<MeshFilter>();
		this.OriginalMesh = base.GetComponent<MeshFilter>().sharedMesh;
		this.DefaultSize = this.OriginalMesh.bounds.size;
		this._newSize = this.DefaultSize;
		this._oldSize = this._newSize;
		if (!this._pivotTransform)
		{
			this._pivotTransform = base.transform.Find("Pivot");
		}
	}

	// Token: 0x06001479 RID: 5241 RVA: 0x00063B68 File Offset: 0x00061D68
	private void OnEnable()
	{
		this.DefaultSize = this.OriginalMesh.bounds.size;
		if (this._newSize == Vector3.zero)
		{
			this._newSize = this.DefaultSize;
		}
	}

	// Token: 0x0600147A RID: 5242 RVA: 0x00063BAC File Offset: 0x00061DAC
	private void Update()
	{
		if (Application.isPlaying && !this._updateInPlayMode)
		{
			return;
		}
		if (this._newSize != this._oldSize)
		{
			this._oldSize = this._newSize;
			Mesh mesh = SimpleResizer.ProcessVertices(this, this._newSize, true);
			this._meshFilter.sharedMesh = mesh;
			this._meshFilter.sharedMesh.RecalculateBounds();
		}
	}

	// Token: 0x0600147B RID: 5243 RVA: 0x00063C14 File Offset: 0x00061E14
	private void OnDrawGizmos()
	{
		if (!this._pivotTransform)
		{
			return;
		}
		Gizmos.color = Color.red;
		float num = 0.1f;
		Vector3 vector = this._pivotTransform.position + Vector3.left * num * 0.5f;
		Vector3 vector2 = this._pivotTransform.position + Vector3.down * num * 0.5f;
		Vector3 vector3 = this._pivotTransform.position + Vector3.back * num * 0.5f;
		Gizmos.DrawRay(vector, Vector3.right * num);
		Gizmos.DrawRay(vector2, Vector3.up * num);
		Gizmos.DrawRay(vector3, Vector3.forward * num);
	}

	// Token: 0x0600147C RID: 5244 RVA: 0x00063CE4 File Offset: 0x00061EE4
	private void OnDrawGizmosSelected()
	{
		if (this._meshFilter.sharedMesh == null)
		{
			return;
		}
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Vector3 center = this._meshFilter.sharedMesh.bounds.center;
		Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		switch (this.ScalingX)
		{
		case SimpleResizable.Method.Adapt:
			Gizmos.DrawWireCube(center, new Vector3(this._newSize.x * this.PaddingX * 2f, this._newSize.y, this._newSize.z));
			break;
		case SimpleResizable.Method.AdaptWithAsymmetricalPadding:
			Gizmos.DrawWireCube(center + new Vector3(this._newSize.x * this.PaddingX, 0f, 0f), new Vector3(0f, this._newSize.y, this._newSize.z));
			Gizmos.DrawWireCube(center + new Vector3(this._newSize.x * this.PaddingXMax, 0f, 0f), new Vector3(0f, this._newSize.y, this._newSize.z));
			break;
		case SimpleResizable.Method.None:
			Gizmos.DrawWireCube(center, this._newSize);
			break;
		}
		Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
		switch (this.ScalingY)
		{
		case SimpleResizable.Method.Adapt:
			Gizmos.DrawWireCube(center, new Vector3(this._newSize.x, this._newSize.y * this.PaddingY * 2f, this._newSize.z));
			break;
		case SimpleResizable.Method.AdaptWithAsymmetricalPadding:
			Gizmos.DrawWireCube(center + new Vector3(0f, this._newSize.y * this.PaddingY, 0f), new Vector3(this._newSize.x, 0f, this._newSize.z));
			Gizmos.DrawWireCube(center + new Vector3(0f, this._newSize.y * this.PaddingYMax, 0f), new Vector3(this._newSize.x, 0f, this._newSize.z));
			break;
		case SimpleResizable.Method.None:
			Gizmos.DrawWireCube(center, this._newSize);
			break;
		}
		Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
		switch (this.ScalingZ)
		{
		case SimpleResizable.Method.Adapt:
			Gizmos.DrawWireCube(center, new Vector3(this._newSize.x, this._newSize.y, this._newSize.z * this.PaddingZ * 2f));
			break;
		case SimpleResizable.Method.AdaptWithAsymmetricalPadding:
			Gizmos.DrawWireCube(center + new Vector3(0f, 0f, this._newSize.z * this.PaddingZ), new Vector3(this._newSize.x, this._newSize.y, 0f));
			Gizmos.DrawWireCube(center + new Vector3(0f, 0f, this._newSize.z * this.PaddingZMax), new Vector3(this._newSize.x, this._newSize.y, 0f));
			break;
		case SimpleResizable.Method.None:
			Gizmos.DrawWireCube(center, this._newSize);
			break;
		}
		Gizmos.color = new Color(0f, 1f, 1f, 1f);
		Gizmos.DrawWireCube(center, this._newSize);
	}

	// Token: 0x040016C8 RID: 5832
	[Space(15f)]
	public SimpleResizable.Method ScalingX;

	// Token: 0x040016C9 RID: 5833
	[Range(0f, 0.5f)]
	public float PaddingX;

	// Token: 0x040016CA RID: 5834
	[Range(-0.5f, 0f)]
	public float PaddingXMax;

	// Token: 0x040016CB RID: 5835
	[Space(15f)]
	public SimpleResizable.Method ScalingY;

	// Token: 0x040016CC RID: 5836
	[Range(0f, 0.5f)]
	public float PaddingY;

	// Token: 0x040016CD RID: 5837
	[Range(-0.5f, 0f)]
	public float PaddingYMax;

	// Token: 0x040016CE RID: 5838
	[Space(15f)]
	public SimpleResizable.Method ScalingZ;

	// Token: 0x040016CF RID: 5839
	[Range(0f, 0.5f)]
	public float PaddingZ;

	// Token: 0x040016D0 RID: 5840
	[Range(-0.5f, 0f)]
	public float PaddingZMax;

	// Token: 0x040016D3 RID: 5843
	private Vector3 _oldSize;

	// Token: 0x040016D4 RID: 5844
	private MeshFilter _meshFilter;

	// Token: 0x040016D5 RID: 5845
	[SerializeField]
	private Vector3 _newSize;

	// Token: 0x040016D6 RID: 5846
	[SerializeField]
	private bool _updateInPlayMode;

	// Token: 0x040016D7 RID: 5847
	[SerializeField]
	private Transform _pivotTransform;

	// Token: 0x02000374 RID: 884
	public enum Method
	{
		// Token: 0x040016D9 RID: 5849
		Adapt,
		// Token: 0x040016DA RID: 5850
		AdaptWithAsymmetricalPadding,
		// Token: 0x040016DB RID: 5851
		Scale,
		// Token: 0x040016DC RID: 5852
		None
	}
}
