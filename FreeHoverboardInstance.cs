using System;
using UnityEngine;

// Token: 0x02000666 RID: 1638
public class FreeHoverboardInstance : MonoBehaviour
{
	// Token: 0x170003E4 RID: 996
	// (get) Token: 0x060028EF RID: 10479 RVA: 0x000CBF70 File Offset: 0x000CA170
	// (set) Token: 0x060028F0 RID: 10480 RVA: 0x000CBF78 File Offset: 0x000CA178
	public Rigidbody Rigidbody { get; private set; }

	// Token: 0x170003E5 RID: 997
	// (get) Token: 0x060028F1 RID: 10481 RVA: 0x000CBF81 File Offset: 0x000CA181
	// (set) Token: 0x060028F2 RID: 10482 RVA: 0x000CBF89 File Offset: 0x000CA189
	public Color boardColor { get; private set; }

	// Token: 0x060028F3 RID: 10483 RVA: 0x000CBF94 File Offset: 0x000CA194
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
		Material[] sharedMaterials = this.boardMesh.sharedMaterials;
		this.colorMaterial = new Material(sharedMaterials[1]);
		sharedMaterials[1] = this.colorMaterial;
		this.boardMesh.sharedMaterials = sharedMaterials;
	}

	// Token: 0x060028F4 RID: 10484 RVA: 0x000CBFDC File Offset: 0x000CA1DC
	public void SetColor(Color col)
	{
		this.colorMaterial.color = col;
		this.boardColor = col;
	}

	// Token: 0x060028F5 RID: 10485 RVA: 0x000CBFF4 File Offset: 0x000CA1F4
	private void Update()
	{
		RaycastHit raycastHit;
		if (Physics.SphereCast(new Ray(base.transform.TransformPoint(this.sphereCastCenter), base.transform.TransformVector(Vector3.down)), this.sphereCastRadius, out raycastHit, 1f, this.hoverRaycastMask.value))
		{
			this.hasHoverPoint = true;
			this.hoverPoint = raycastHit.point;
			this.hoverNormal = raycastHit.normal;
			return;
		}
		this.hasHoverPoint = false;
	}

	// Token: 0x060028F6 RID: 10486 RVA: 0x000CC070 File Offset: 0x000CA270
	private void FixedUpdate()
	{
		if (this.hasHoverPoint)
		{
			float num = Vector3.Dot(base.transform.TransformPoint(this.sphereCastCenter) - this.hoverPoint, this.hoverNormal);
			if (num < this.hoverHeight)
			{
				base.transform.position += this.hoverNormal * (this.hoverHeight - num);
				this.Rigidbody.velocity = Vector3.ProjectOnPlane(this.Rigidbody.velocity, this.hoverNormal);
				Vector3 vector = Quaternion.Inverse(base.transform.rotation) * this.Rigidbody.angularVelocity;
				vector.x *= this.avelocityDragWhileHovering;
				vector.z *= this.avelocityDragWhileHovering;
				this.Rigidbody.angularVelocity = base.transform.rotation * vector;
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(base.transform.forward, this.hoverNormal), this.hoverNormal), this.hoverRotationLerp);
			}
		}
	}

	// Token: 0x04002DFF RID: 11775
	public int ownerActorNumber;

	// Token: 0x04002E00 RID: 11776
	public int boardIndex;

	// Token: 0x04002E01 RID: 11777
	[SerializeField]
	private Vector3 sphereCastCenter;

	// Token: 0x04002E02 RID: 11778
	[SerializeField]
	private float sphereCastRadius;

	// Token: 0x04002E03 RID: 11779
	[SerializeField]
	private LayerMask hoverRaycastMask;

	// Token: 0x04002E04 RID: 11780
	[SerializeField]
	private float hoverHeight;

	// Token: 0x04002E05 RID: 11781
	[SerializeField]
	private float hoverRotationLerp;

	// Token: 0x04002E06 RID: 11782
	[SerializeField]
	private float avelocityDragWhileHovering;

	// Token: 0x04002E07 RID: 11783
	[SerializeField]
	private MeshRenderer boardMesh;

	// Token: 0x04002E09 RID: 11785
	private Material colorMaterial;

	// Token: 0x04002E0A RID: 11786
	private bool hasHoverPoint;

	// Token: 0x04002E0B RID: 11787
	private Vector3 hoverPoint;

	// Token: 0x04002E0C RID: 11788
	private Vector3 hoverNormal;
}
