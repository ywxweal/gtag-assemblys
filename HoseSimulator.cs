using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x0200018B RID: 395
public class HoseSimulator : MonoBehaviour, ISpawnable
{
	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x060009BF RID: 2495 RVA: 0x00033DF1 File Offset: 0x00031FF1
	// (set) Token: 0x060009C0 RID: 2496 RVA: 0x00033DF9 File Offset: 0x00031FF9
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x060009C1 RID: 2497 RVA: 0x00033E02 File Offset: 0x00032002
	// (set) Token: 0x060009C2 RID: 2498 RVA: 0x00033E0A File Offset: 0x0003200A
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060009C3 RID: 2499 RVA: 0x000023F4 File Offset: 0x000005F4
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x00033E14 File Offset: 0x00032014
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.anchors = rig.cosmeticReferences.Get(this.startAnchorRef).GetComponent<HoseSimulatorAnchors>();
		if (this.skinnedMeshRenderer != null)
		{
			Bounds localBounds = this.skinnedMeshRenderer.localBounds;
			localBounds.extents = this.localBoundsOverride;
			this.skinnedMeshRenderer.localBounds = localBounds;
		}
		this.hoseSectionLengths = new float[this.hoseBones.Length - 1];
		this.hoseBonePositions = new Vector3[this.hoseBones.Length];
		this.hoseBoneVelocities = new Vector3[this.hoseBones.Length];
		for (int i = 0; i < this.hoseSectionLengths.Length; i++)
		{
			float num = 1f;
			this.hoseSectionLengths[i] = num;
			this.totalHoseLength += num;
		}
	}

	// Token: 0x060009C5 RID: 2501 RVA: 0x00033EDC File Offset: 0x000320DC
	private void LateUpdate()
	{
		if (this.myHoldable.InLeftHand())
		{
			this.isLeftHanded = true;
		}
		else if (this.myHoldable.InRightHand())
		{
			this.isLeftHanded = false;
		}
		for (int i = 0; i < this.miscBones.Length; i++)
		{
			Transform transform = (this.isLeftHanded ? this.anchors.miscAnchorsLeft[i] : this.anchors.miscAnchorsRight[i]);
			this.miscBones[i].transform.position = transform.position;
			this.miscBones[i].transform.rotation = transform.rotation;
		}
		this.startAnchor = (this.isLeftHanded ? this.anchors.leftAnchorPoint : this.anchors.rightAnchorPoint);
		float x = this.myHoldable.transform.lossyScale.x;
		float num = 0f;
		Vector3 position = this.startAnchor.position;
		Vector3 vector = position + this.startAnchor.forward * this.startStiffness * x;
		Vector3 position2 = this.endAnchor.position;
		Vector3 vector2 = position2 - this.endAnchor.forward * this.endStiffness * x;
		for (int j = 0; j < this.hoseBones.Length; j++)
		{
			float num2 = num / this.totalHoseLength;
			Vector3 vector3 = BezierUtils.BezierSolve(num2, position, vector, vector2, position2);
			Vector3 vector4 = BezierUtils.BezierSolve(num2 + 0.1f, position, vector, vector2, position2);
			if (this.firstUpdate)
			{
				this.hoseBones[j].transform.position = vector3;
				this.hoseBonePositions[j] = vector3;
				this.hoseBoneVelocities[j] = Vector3.zero;
			}
			else
			{
				this.hoseBoneVelocities[j] *= this.damping;
				this.hoseBonePositions[j] += this.hoseBoneVelocities[j] * Time.deltaTime;
				float num3 = this.hoseBoneMaxDisplacement[j] * x;
				if ((vector3 - this.hoseBonePositions[j]).IsLongerThan(num3))
				{
					Vector3 vector5 = vector3 + (this.hoseBonePositions[j] - vector3).normalized * num3;
					this.hoseBoneVelocities[j] += (vector5 - this.hoseBonePositions[j]) / Time.deltaTime;
					this.hoseBonePositions[j] = vector5;
				}
				this.hoseBones[j].transform.position = this.hoseBonePositions[j];
			}
			this.hoseBones[j].transform.rotation = Quaternion.LookRotation(vector4 - vector3, this.endAnchor.transform.up);
			if (j < this.hoseSectionLengths.Length)
			{
				num += this.hoseSectionLengths[j];
			}
		}
		this.firstUpdate = false;
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x0003421A File Offset: 0x0003241A
	private void OnDrawGizmosSelected()
	{
		if (this.hoseBonePositions != null)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLineStrip(this.hoseBonePositions, false);
		}
	}

	// Token: 0x04000BD6 RID: 3030
	[SerializeField]
	private SkinnedMeshRenderer skinnedMeshRenderer;

	// Token: 0x04000BD7 RID: 3031
	[SerializeField]
	private Vector3 localBoundsOverride;

	// Token: 0x04000BD8 RID: 3032
	[SerializeField]
	private Transform[] miscBones;

	// Token: 0x04000BD9 RID: 3033
	[SerializeField]
	private Transform[] hoseBones;

	// Token: 0x04000BDA RID: 3034
	[SerializeField]
	private float[] hoseBoneMaxDisplacement;

	// Token: 0x04000BDB RID: 3035
	[SerializeField]
	private CosmeticRefID startAnchorRef;

	// Token: 0x04000BDC RID: 3036
	private Transform startAnchor;

	// Token: 0x04000BDD RID: 3037
	[SerializeField]
	private float startStiffness = 0.5f;

	// Token: 0x04000BDE RID: 3038
	[SerializeField]
	private Transform endAnchor;

	// Token: 0x04000BDF RID: 3039
	[SerializeField]
	private float endStiffness = 0.5f;

	// Token: 0x04000BE0 RID: 3040
	private Vector3[] hoseBonePositions;

	// Token: 0x04000BE1 RID: 3041
	private Vector3[] hoseBoneVelocities;

	// Token: 0x04000BE2 RID: 3042
	[SerializeField]
	private float damping = 0.97f;

	// Token: 0x04000BE3 RID: 3043
	private float[] hoseSectionLengths;

	// Token: 0x04000BE4 RID: 3044
	private float totalHoseLength;

	// Token: 0x04000BE5 RID: 3045
	private bool firstUpdate = true;

	// Token: 0x04000BE6 RID: 3046
	private HoseSimulatorAnchors anchors;

	// Token: 0x04000BE7 RID: 3047
	[SerializeField]
	private TransferrableObject myHoldable;

	// Token: 0x04000BE8 RID: 3048
	private bool isLeftHanded;
}
