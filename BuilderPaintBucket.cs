using System;
using UnityEngine;

// Token: 0x020004D0 RID: 1232
public class BuilderPaintBucket : MonoBehaviour
{
	// Token: 0x06001DE7 RID: 7655 RVA: 0x0009196C File Offset: 0x0008FB6C
	private void Awake()
	{
		if (string.IsNullOrEmpty(this.materialId))
		{
			return;
		}
		this.materialType = this.materialId.GetHashCode();
		if (this.bucketMaterialOptions != null && this.paintBucketRenderer != null)
		{
			Material material;
			int num;
			this.bucketMaterialOptions.GetMaterialFromType(this.materialType, out material, out num);
			if (material != null)
			{
				this.paintBucketRenderer.material = material;
			}
		}
	}

	// Token: 0x06001DE8 RID: 7656 RVA: 0x000919E0 File Offset: 0x0008FBE0
	private void OnTriggerEnter(Collider other)
	{
		if (this.materialType == -1)
		{
			return;
		}
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			BuilderPaintBrush component = attachedRigidbody.GetComponent<BuilderPaintBrush>();
			if (component != null)
			{
				component.SetBrushMaterial(this.materialType);
			}
		}
	}

	// Token: 0x04002117 RID: 8471
	[SerializeField]
	private BuilderMaterialOptions bucketMaterialOptions;

	// Token: 0x04002118 RID: 8472
	[SerializeField]
	private MeshRenderer paintBucketRenderer;

	// Token: 0x04002119 RID: 8473
	[SerializeField]
	private string materialId;

	// Token: 0x0400211A RID: 8474
	private int materialType = -1;
}
