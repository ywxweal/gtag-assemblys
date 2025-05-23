using System;
using UnityEngine;

// Token: 0x020003D0 RID: 976
public class TransformReset : MonoBehaviour
{
	// Token: 0x060016BB RID: 5819 RVA: 0x0006D4E0 File Offset: 0x0006B6E0
	private void Awake()
	{
		Transform[] componentsInChildren = base.GetComponentsInChildren<Transform>();
		this.transformList = new TransformReset.OriginalGameObjectTransform[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			this.transformList[i] = new TransformReset.OriginalGameObjectTransform(componentsInChildren[i]);
		}
		this.ResetTransforms();
	}

	// Token: 0x060016BC RID: 5820 RVA: 0x0006D52C File Offset: 0x0006B72C
	public void ReturnTransforms()
	{
		foreach (TransformReset.OriginalGameObjectTransform originalGameObjectTransform in this.tempTransformList)
		{
			originalGameObjectTransform.thisTransform.position = originalGameObjectTransform.thisPosition;
			originalGameObjectTransform.thisTransform.rotation = originalGameObjectTransform.thisRotation;
		}
	}

	// Token: 0x060016BD RID: 5821 RVA: 0x0006D57C File Offset: 0x0006B77C
	public void SetScale(float ratio)
	{
		foreach (TransformReset.OriginalGameObjectTransform originalGameObjectTransform in this.transformList)
		{
			originalGameObjectTransform.thisTransform.localScale *= ratio;
		}
	}

	// Token: 0x060016BE RID: 5822 RVA: 0x0006D5C0 File Offset: 0x0006B7C0
	public void ResetTransforms()
	{
		this.tempTransformList = new TransformReset.OriginalGameObjectTransform[this.transformList.Length];
		for (int i = 0; i < this.transformList.Length; i++)
		{
			this.tempTransformList[i] = new TransformReset.OriginalGameObjectTransform(this.transformList[i].thisTransform);
		}
		foreach (TransformReset.OriginalGameObjectTransform originalGameObjectTransform in this.transformList)
		{
			originalGameObjectTransform.thisTransform.position = originalGameObjectTransform.thisPosition;
			originalGameObjectTransform.thisTransform.rotation = originalGameObjectTransform.thisRotation;
		}
	}

	// Token: 0x04001929 RID: 6441
	private TransformReset.OriginalGameObjectTransform[] transformList;

	// Token: 0x0400192A RID: 6442
	private TransformReset.OriginalGameObjectTransform[] tempTransformList;

	// Token: 0x020003D1 RID: 977
	private struct OriginalGameObjectTransform
	{
		// Token: 0x060016C0 RID: 5824 RVA: 0x0006D658 File Offset: 0x0006B858
		public OriginalGameObjectTransform(Transform constructionTransform)
		{
			this._thisTransform = constructionTransform;
			this._thisPosition = constructionTransform.position;
			this._thisRotation = constructionTransform.rotation;
		}

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x060016C1 RID: 5825 RVA: 0x0006D679 File Offset: 0x0006B879
		// (set) Token: 0x060016C2 RID: 5826 RVA: 0x0006D681 File Offset: 0x0006B881
		public Transform thisTransform
		{
			get
			{
				return this._thisTransform;
			}
			set
			{
				this._thisTransform = value;
			}
		}

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x060016C3 RID: 5827 RVA: 0x0006D68A File Offset: 0x0006B88A
		// (set) Token: 0x060016C4 RID: 5828 RVA: 0x0006D692 File Offset: 0x0006B892
		public Vector3 thisPosition
		{
			get
			{
				return this._thisPosition;
			}
			set
			{
				this._thisPosition = value;
			}
		}

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x060016C5 RID: 5829 RVA: 0x0006D69B File Offset: 0x0006B89B
		// (set) Token: 0x060016C6 RID: 5830 RVA: 0x0006D6A3 File Offset: 0x0006B8A3
		public Quaternion thisRotation
		{
			get
			{
				return this._thisRotation;
			}
			set
			{
				this._thisRotation = value;
			}
		}

		// Token: 0x0400192B RID: 6443
		private Transform _thisTransform;

		// Token: 0x0400192C RID: 6444
		private Vector3 _thisPosition;

		// Token: 0x0400192D RID: 6445
		private Quaternion _thisRotation;
	}
}
