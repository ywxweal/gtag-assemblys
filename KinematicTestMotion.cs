using System;
using UnityEngine;

// Token: 0x02000961 RID: 2401
public class KinematicTestMotion : MonoBehaviour
{
	// Token: 0x06003A0A RID: 14858 RVA: 0x00116B1A File Offset: 0x00114D1A
	private void FixedUpdate()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.FixedUpdate)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x06003A0B RID: 14859 RVA: 0x00116B31 File Offset: 0x00114D31
	private void Update()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.Update)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x06003A0C RID: 14860 RVA: 0x00116B47 File Offset: 0x00114D47
	private void LateUpdate()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.LateUpdate)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x06003A0D RID: 14861 RVA: 0x00116B60 File Offset: 0x00114D60
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

	// Token: 0x04003F20 RID: 16160
	public Transform start;

	// Token: 0x04003F21 RID: 16161
	public Transform end;

	// Token: 0x04003F22 RID: 16162
	public Rigidbody rigidbody;

	// Token: 0x04003F23 RID: 16163
	public KinematicTestMotion.UpdateType updateType;

	// Token: 0x04003F24 RID: 16164
	public KinematicTestMotion.MoveType moveType = KinematicTestMotion.MoveType.RigidbodyMovePosition;

	// Token: 0x04003F25 RID: 16165
	public float period = 4f;

	// Token: 0x02000962 RID: 2402
	public enum UpdateType
	{
		// Token: 0x04003F27 RID: 16167
		Update,
		// Token: 0x04003F28 RID: 16168
		LateUpdate,
		// Token: 0x04003F29 RID: 16169
		FixedUpdate
	}

	// Token: 0x02000963 RID: 2403
	public enum MoveType
	{
		// Token: 0x04003F2B RID: 16171
		TransformPosition,
		// Token: 0x04003F2C RID: 16172
		RigidbodyMovePosition
	}
}
