using System;
using System.Collections.Generic;
using Drawing;
using GorillaTag;
using UnityEngine;

// Token: 0x02000783 RID: 1923
public class VolumeCast : MonoBehaviourGizmos
{
	// Token: 0x0600303C RID: 12348 RVA: 0x000EE4C8 File Offset: 0x000EC6C8
	public bool CheckOverlaps()
	{
		Transform transform = base.transform;
		Vector3 lossyScale = transform.lossyScale;
		Quaternion rotation = transform.rotation;
		int num = (int)this.physicsMask;
		QueryTriggerInteraction queryTriggerInteraction = (this.includeTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore);
		Vector3 vector;
		Vector3 vector2;
		float num2;
		VolumeCast.GetEndsAndRadius(transform, this.center, this.height, this.radius, out vector, out vector2, out num2);
		VolumeCast.VolumeShape volumeShape = this.shape;
		Vector3 vector3;
		Vector3 vector4;
		if (volumeShape != VolumeCast.VolumeShape.Box)
		{
			if (volumeShape != VolumeCast.VolumeShape.Cylinder)
			{
				return false;
			}
			vector3 = (vector + vector2) * 0.5f;
			vector4 = new Vector3(num2, Vector3.Distance(vector, vector2) * 0.5f, num2);
		}
		else
		{
			vector3 = transform.TransformPoint(this.center);
			vector4 = Vector3.Scale(lossyScale, this.size * 0.5f).Abs();
		}
		Array.Clear(this._boxOverlaps, 0, 8);
		this._boxHits = Physics.OverlapBoxNonAlloc(vector3, vector4, this._boxOverlaps, rotation, num, queryTriggerInteraction);
		if (this.shape != VolumeCast.VolumeShape.Cylinder)
		{
			return this._colliding = this._boxHits > 0;
		}
		this._hits = 0;
		Array.Clear(this._capOverlaps, 0, 8);
		Array.Clear(this._overlaps, 0, 8);
		this._capHits = Physics.OverlapCapsuleNonAlloc(vector, vector2, num2, this._capOverlaps, num, queryTriggerInteraction);
		this._set.Clear();
		int num3 = Math.Max(this._capHits, this._boxHits);
		Collider[] array = ((this._capHits < this._boxHits) ? this._capOverlaps : this._boxOverlaps);
		Collider[] array2 = ((this._capHits < this._boxHits) ? this._boxOverlaps : this._capOverlaps);
		for (int i = 0; i < num3; i++)
		{
			Collider collider = array[i];
			if (collider && !this._set.Add(collider))
			{
				Collider[] overlaps = this._overlaps;
				int num4 = this._hits;
				this._hits = num4 + 1;
				overlaps[num4] = collider;
			}
			Collider collider2 = array2[i];
			if (collider2 && !this._set.Add(collider2))
			{
				Collider[] overlaps2 = this._overlaps;
				int num4 = this._hits;
				this._hits = num4 + 1;
				overlaps2[num4] = collider2;
			}
		}
		return this._colliding = this._hits > 0;
	}

	// Token: 0x0600303D RID: 12349 RVA: 0x000EE70C File Offset: 0x000EC90C
	private static void GetEndsAndRadius(Transform t, Vector3 center, float height, float radius, out Vector3 a, out Vector3 b, out float r)
	{
		float num = height * 0.5f;
		Vector3 lossyScale = t.lossyScale;
		a = t.TransformPoint(center + Vector3.down * num);
		b = t.TransformPoint(center + Vector3.up * num);
		r = Math.Max(Math.Abs(lossyScale.x), Math.Abs(lossyScale.z)) * radius;
	}

	// Token: 0x04003655 RID: 13909
	public VolumeCast.VolumeShape shape;

	// Token: 0x04003656 RID: 13910
	[Space]
	public Vector3 center;

	// Token: 0x04003657 RID: 13911
	public Vector3 size = Vector3.one;

	// Token: 0x04003658 RID: 13912
	public float height = 1f;

	// Token: 0x04003659 RID: 13913
	public float radius = 1f;

	// Token: 0x0400365A RID: 13914
	private const int MAX_HITS = 8;

	// Token: 0x0400365B RID: 13915
	[Space]
	public UnityLayerMask physicsMask = UnityLayerMask.Everything;

	// Token: 0x0400365C RID: 13916
	public bool includeTriggers;

	// Token: 0x0400365D RID: 13917
	[Space]
	[SerializeField]
	private bool _simulateInEditMode;

	// Token: 0x0400365E RID: 13918
	[DebugReadout]
	[NonSerialized]
	private int _capHits;

	// Token: 0x0400365F RID: 13919
	[DebugReadout]
	[NonSerialized]
	private Collider[] _capOverlaps = new Collider[8];

	// Token: 0x04003660 RID: 13920
	[DebugReadout]
	[NonSerialized]
	private int _boxHits;

	// Token: 0x04003661 RID: 13921
	[DebugReadout]
	[NonSerialized]
	private Collider[] _boxOverlaps = new Collider[8];

	// Token: 0x04003662 RID: 13922
	[DebugReadout]
	[NonSerialized]
	private int _hits;

	// Token: 0x04003663 RID: 13923
	[DebugReadout]
	[NonSerialized]
	private Collider[] _overlaps = new Collider[8];

	// Token: 0x04003664 RID: 13924
	[DebugReadout]
	[NonSerialized]
	private bool _colliding;

	// Token: 0x04003665 RID: 13925
	[NonSerialized]
	private HashSet<Collider> _set = new HashSet<Collider>(8);

	// Token: 0x02000784 RID: 1924
	public enum VolumeShape
	{
		// Token: 0x04003667 RID: 13927
		Box,
		// Token: 0x04003668 RID: 13928
		Cylinder
	}
}
