using System;
using UnityEngine;

// Token: 0x020000DE RID: 222
public class SpiderDangler : MonoBehaviour
{
	// Token: 0x06000594 RID: 1428 RVA: 0x00020428 File Offset: 0x0001E628
	protected void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		Vector3 position = base.transform.position;
		float magnitude = (this.endTransform.position - position).magnitude;
		this.ropeSegLen = magnitude / 6f;
		this.ropeSegs = new SpiderDangler.RopeSegment[6];
		for (int i = 0; i < 6; i++)
		{
			this.ropeSegs[i] = new SpiderDangler.RopeSegment(position);
			position.y -= this.ropeSegLen;
		}
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x000204AF File Offset: 0x0001E6AF
	protected void FixedUpdate()
	{
		this.Simulate();
	}

	// Token: 0x06000596 RID: 1430 RVA: 0x000204B8 File Offset: 0x0001E6B8
	protected void LateUpdate()
	{
		this.DrawRope();
		Vector3 normalized = (this.ropeSegs[this.ropeSegs.Length - 2].pos - this.ropeSegs[this.ropeSegs.Length - 1].pos).normalized;
		this.endTransform.position = this.ropeSegs[this.ropeSegs.Length - 1].pos;
		this.endTransform.up = normalized;
		Vector4 vector = this.spinSpeeds * Time.time;
		vector = new Vector4(Mathf.Sin(vector.x), Mathf.Sin(vector.y), Mathf.Sin(vector.z), Mathf.Sin(vector.w));
		vector.Scale(this.spinScales);
		this.endTransform.Rotate(Vector3.up, vector.x + vector.y + vector.z + vector.w);
	}

	// Token: 0x06000597 RID: 1431 RVA: 0x000205BC File Offset: 0x0001E7BC
	private void Simulate()
	{
		this.ropeSegLenScaled = this.ropeSegLen * base.transform.lossyScale.x;
		Vector3 vector = new Vector3(0f, -0.5f, 0f) * Time.fixedDeltaTime;
		for (int i = 1; i < 6; i++)
		{
			Vector3 vector2 = this.ropeSegs[i].pos - this.ropeSegs[i].posOld;
			this.ropeSegs[i].posOld = this.ropeSegs[i].pos;
			SpiderDangler.RopeSegment[] array = this.ropeSegs;
			int num = i;
			array[num].pos = array[num].pos + vector2 * 0.95f;
			SpiderDangler.RopeSegment[] array2 = this.ropeSegs;
			int num2 = i;
			array2[num2].pos = array2[num2].pos + vector;
		}
		for (int j = 0; j < 8; j++)
		{
			this.ApplyConstraint();
		}
	}

	// Token: 0x06000598 RID: 1432 RVA: 0x000206C4 File Offset: 0x0001E8C4
	private void ApplyConstraint()
	{
		this.ropeSegs[0].pos = base.transform.position;
		this.ApplyConstraintSegment(ref this.ropeSegs[0], ref this.ropeSegs[1], 0f, 1f);
		for (int i = 1; i < 5; i++)
		{
			this.ApplyConstraintSegment(ref this.ropeSegs[i], ref this.ropeSegs[i + 1], 0.5f, 0.5f);
		}
	}

	// Token: 0x06000599 RID: 1433 RVA: 0x0002074C File Offset: 0x0001E94C
	private void ApplyConstraintSegment(ref SpiderDangler.RopeSegment segA, ref SpiderDangler.RopeSegment segB, float dampenA, float dampenB)
	{
		float num = (segA.pos - segB.pos).magnitude - this.ropeSegLenScaled;
		Vector3 vector = (segA.pos - segB.pos).normalized * num;
		segA.pos -= vector * dampenA;
		segB.pos += vector * dampenB;
	}

	// Token: 0x0600059A RID: 1434 RVA: 0x000207D8 File Offset: 0x0001E9D8
	private void DrawRope()
	{
		Vector3[] array = new Vector3[6];
		for (int i = 0; i < 6; i++)
		{
			array[i] = this.ropeSegs[i].pos;
		}
		this.lineRenderer.positionCount = array.Length;
		this.lineRenderer.SetPositions(array);
	}

	// Token: 0x04000697 RID: 1687
	public Transform endTransform;

	// Token: 0x04000698 RID: 1688
	public Vector4 spinSpeeds = new Vector4(0.1f, 0.2f, 0.3f, 0.4f);

	// Token: 0x04000699 RID: 1689
	public Vector4 spinScales = new Vector4(180f, 90f, 120f, 180f);

	// Token: 0x0400069A RID: 1690
	private LineRenderer lineRenderer;

	// Token: 0x0400069B RID: 1691
	private SpiderDangler.RopeSegment[] ropeSegs;

	// Token: 0x0400069C RID: 1692
	private float ropeSegLen;

	// Token: 0x0400069D RID: 1693
	private float ropeSegLenScaled;

	// Token: 0x0400069E RID: 1694
	private const int kSegmentCount = 6;

	// Token: 0x0400069F RID: 1695
	private const float kVelocityDamper = 0.95f;

	// Token: 0x040006A0 RID: 1696
	private const int kConstraintCalculationIterations = 8;

	// Token: 0x020000DF RID: 223
	public struct RopeSegment
	{
		// Token: 0x0600059C RID: 1436 RVA: 0x0002087D File Offset: 0x0001EA7D
		public RopeSegment(Vector3 pos)
		{
			this.pos = pos;
			this.posOld = pos;
		}

		// Token: 0x040006A1 RID: 1697
		public Vector3 pos;

		// Token: 0x040006A2 RID: 1698
		public Vector3 posOld;
	}
}
