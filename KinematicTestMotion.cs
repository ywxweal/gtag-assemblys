using System;
using UnityEngine;

// Token: 0x02000961 RID: 2401
public class KinematicTestMotion : MonoBehaviour
{
	// Token: 0x06003A0B RID: 14859 RVA: 0x00116BF2 File Offset: 0x00114DF2
	private void FixedUpdate()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.FixedUpdate)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x06003A0C RID: 14860 RVA: 0x00116C09 File Offset: 0x00114E09
	private void Update()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.Update)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x06003A0D RID: 14861 RVA: 0x00116C1F File Offset: 0x00114E1F
	private void LateUpdate()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.LateUpdate)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x06003A0E RID: 14862 RVA: 0x00116C38 File Offset: 0x00114E38
	private void UpdatePosition(float time)
	{
		float num = Mathf.Sin(time * 2f * 3.1415927f * this.period) * 0.5f + 0.5f;
		Vector3 vector = Vector3.Lerp(this.start.position, this.end.position, num);
		if (this.moveType == KinematicTestMotion.MoveType.TransformPosition)
		{
			base.transform.position = vector;
			return;
		}
		if (this.moveType == KinematicTestMotion.MoveType.RigidbodyMovePosition)
		{
			this.rigidbody.MovePosition(vector);
		}
	}

	// Token: 0x04003F21 RID: 16161
	public Transform start;

	// Token: 0x04003F22 RID: 16162
	public Transform end;

	// Token: 0x04003F23 RID: 16163
	public Rigidbody rigidbody;

	// Token: 0x04003F24 RID: 16164
	public KinematicTestMotion.UpdateType updateType;

	// Token: 0x04003F25 RID: 16165
	public KinematicTestMotion.MoveType moveType = KinematicTestMotion.MoveType.RigidbodyMovePosition;

	// Token: 0x04003F26 RID: 16166
	public float period = 4f;

	// Token: 0x02000962 RID: 2402
	public enum UpdateType
	{
		// Token: 0x04003F28 RID: 16168
		Update,
		// Token: 0x04003F29 RID: 16169
		LateUpdate,
		// Token: 0x04003F2A RID: 16170
		FixedUpdate
	}

	// Token: 0x02000963 RID: 2403
	public enum MoveType
	{
		// Token: 0x04003F2C RID: 16172
		TransformPosition,
		// Token: 0x04003F2D RID: 16173
		RigidbodyMovePosition
	}
}
