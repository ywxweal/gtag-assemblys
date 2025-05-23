using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000306 RID: 774
public abstract class TeleportTargetHandler : TeleportSupport
{
	// Token: 0x06001284 RID: 4740 RVA: 0x0005769A File Offset: 0x0005589A
	protected TeleportTargetHandler()
	{
		this._startAimAction = delegate
		{
			base.StartCoroutine(this.TargetAimCoroutine());
		};
	}

	// Token: 0x06001285 RID: 4741 RVA: 0x000576CA File Offset: 0x000558CA
	protected override void AddEventHandlers()
	{
		base.AddEventHandlers();
		base.LocomotionTeleport.EnterStateAim += this._startAimAction;
	}

	// Token: 0x06001286 RID: 4742 RVA: 0x000576E3 File Offset: 0x000558E3
	protected override void RemoveEventHandlers()
	{
		base.RemoveEventHandlers();
		base.LocomotionTeleport.EnterStateAim -= this._startAimAction;
	}

	// Token: 0x06001287 RID: 4743 RVA: 0x000576FC File Offset: 0x000558FC
	private IEnumerator TargetAimCoroutine()
	{
		while (base.LocomotionTeleport.CurrentState == LocomotionTeleport.States.Aim)
		{
			this.ResetAimData();
			Vector3 vector = base.LocomotionTeleport.transform.position;
			this._aimPoints.Clear();
			base.LocomotionTeleport.AimHandler.GetPoints(this._aimPoints);
			for (int i = 0; i < this._aimPoints.Count; i++)
			{
				Vector3 vector2 = this._aimPoints[i];
				this.AimData.TargetValid = this.ConsiderTeleport(vector, ref vector2);
				this.AimData.Points.Add(vector2);
				if (this.AimData.TargetValid)
				{
					this.AimData.Destination = this.ConsiderDestination(vector2);
					this.AimData.TargetValid = this.AimData.Destination != null;
					break;
				}
				vector = this._aimPoints[i];
			}
			base.LocomotionTeleport.OnUpdateAimData(this.AimData);
			yield return null;
		}
		yield break;
	}

	// Token: 0x06001288 RID: 4744 RVA: 0x0005770B File Offset: 0x0005590B
	protected virtual void ResetAimData()
	{
		this.AimData.Reset();
	}

	// Token: 0x06001289 RID: 4745
	protected abstract bool ConsiderTeleport(Vector3 start, ref Vector3 end);

	// Token: 0x0600128A RID: 4746 RVA: 0x00057718 File Offset: 0x00055918
	public virtual Vector3? ConsiderDestination(Vector3 location)
	{
		CapsuleCollider characterController = base.LocomotionTeleport.LocomotionController.CharacterController;
		float num = characterController.radius - 0.1f;
		Vector3 vector = location;
		vector.y += num + 0.1f;
		Vector3 vector2 = vector;
		vector2.y += characterController.height - 0.1f;
		if (Physics.CheckCapsule(vector, vector2, num, this.AimCollisionLayerMask, QueryTriggerInteraction.Ignore))
		{
			return null;
		}
		return new Vector3?(location);
	}

	// Token: 0x04001499 RID: 5273
	[Tooltip("This bitmask controls which game object layers will be included in the targeting collision tests.")]
	public LayerMask AimCollisionLayerMask;

	// Token: 0x0400149A RID: 5274
	protected readonly LocomotionTeleport.AimData AimData = new LocomotionTeleport.AimData();

	// Token: 0x0400149B RID: 5275
	private readonly Action _startAimAction;

	// Token: 0x0400149C RID: 5276
	private readonly List<Vector3> _aimPoints = new List<Vector3>();

	// Token: 0x0400149D RID: 5277
	private const float ERROR_MARGIN = 0.1f;
}
