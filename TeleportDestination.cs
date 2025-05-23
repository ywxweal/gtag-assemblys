using System;
using UnityEngine;

// Token: 0x020002F6 RID: 758
public class TeleportDestination : MonoBehaviour
{
	// Token: 0x1700020A RID: 522
	// (get) Token: 0x06001236 RID: 4662 RVA: 0x00056891 File Offset: 0x00054A91
	// (set) Token: 0x06001237 RID: 4663 RVA: 0x00056899 File Offset: 0x00054A99
	public bool IsValidDestination { get; private set; }

	// Token: 0x06001238 RID: 4664 RVA: 0x000568A2 File Offset: 0x00054AA2
	private TeleportDestination()
	{
		this._updateTeleportDestinationAction = new Action<bool, Vector3?, Quaternion?, Quaternion?>(this.UpdateTeleportDestination);
	}

	// Token: 0x06001239 RID: 4665 RVA: 0x000568C0 File Offset: 0x00054AC0
	public void OnEnable()
	{
		this.PositionIndicator.gameObject.SetActive(false);
		if (this.OrientationIndicator != null)
		{
			this.OrientationIndicator.gameObject.SetActive(false);
		}
		this.LocomotionTeleport.UpdateTeleportDestination += this._updateTeleportDestinationAction;
		this._eventsActive = true;
	}

	// Token: 0x0600123A RID: 4666 RVA: 0x00056915 File Offset: 0x00054B15
	private void TryDisableEventHandlers()
	{
		if (!this._eventsActive)
		{
			return;
		}
		this.LocomotionTeleport.UpdateTeleportDestination -= this._updateTeleportDestinationAction;
		this._eventsActive = false;
	}

	// Token: 0x0600123B RID: 4667 RVA: 0x00056938 File Offset: 0x00054B38
	public void OnDisable()
	{
		this.TryDisableEventHandlers();
	}

	// Token: 0x1400003E RID: 62
	// (add) Token: 0x0600123C RID: 4668 RVA: 0x00056940 File Offset: 0x00054B40
	// (remove) Token: 0x0600123D RID: 4669 RVA: 0x00056978 File Offset: 0x00054B78
	public event Action<TeleportDestination> Deactivated;

	// Token: 0x0600123E RID: 4670 RVA: 0x000569AD File Offset: 0x00054BAD
	public void OnDeactivated()
	{
		if (this.Deactivated != null)
		{
			this.Deactivated(this);
			return;
		}
		this.Recycle();
	}

	// Token: 0x0600123F RID: 4671 RVA: 0x000569CA File Offset: 0x00054BCA
	public void Recycle()
	{
		this.LocomotionTeleport.RecycleTeleportDestination(this);
	}

	// Token: 0x06001240 RID: 4672 RVA: 0x000569D8 File Offset: 0x00054BD8
	public virtual void UpdateTeleportDestination(bool isValidDestination, Vector3? position, Quaternion? rotation, Quaternion? landingRotation)
	{
		this.IsValidDestination = isValidDestination;
		this.LandingRotation = landingRotation.GetValueOrDefault();
		GameObject gameObject = this.PositionIndicator.gameObject;
		bool activeInHierarchy = gameObject.activeInHierarchy;
		if (position == null)
		{
			if (activeInHierarchy)
			{
				gameObject.SetActive(false);
			}
			return;
		}
		if (!activeInHierarchy)
		{
			gameObject.SetActive(true);
		}
		base.transform.position = position.GetValueOrDefault();
		if (this.OrientationIndicator == null)
		{
			if (rotation != null)
			{
				base.transform.rotation = rotation.GetValueOrDefault();
			}
			return;
		}
		GameObject gameObject2 = this.OrientationIndicator.gameObject;
		bool activeInHierarchy2 = gameObject2.activeInHierarchy;
		if (rotation == null)
		{
			if (activeInHierarchy2)
			{
				gameObject2.SetActive(false);
			}
			return;
		}
		this.OrientationIndicator.rotation = rotation.GetValueOrDefault();
		if (!activeInHierarchy2)
		{
			gameObject2.SetActive(true);
		}
	}

	// Token: 0x04001453 RID: 5203
	[Tooltip("If the target handler provides a target position, this transform will be moved to that position and it's game object enabled. A target position being provided does not mean the position is valid, only that the aim handler found something to test as a destination.")]
	public Transform PositionIndicator;

	// Token: 0x04001454 RID: 5204
	[Tooltip("This transform will be rotated to match the rotation of the aiming target. Simple teleport destinations should assign this to the object containing this component. More complex teleport destinations might assign this to a sub-object that is used to indicate the landing orientation independently from the rest of the destination indicator, such as when world space effects are required. This will typically be a child of the PositionIndicator.")]
	public Transform OrientationIndicator;

	// Token: 0x04001455 RID: 5205
	[Tooltip("After the player teleports, the character controller will have it's rotation set to this value. It is different from the OrientationIndicator transform.rotation in order to support both head-relative and forward-facing teleport modes (See TeleportOrientationHandlerThumbstick.cs).")]
	public Quaternion LandingRotation;

	// Token: 0x04001456 RID: 5206
	[NonSerialized]
	public LocomotionTeleport LocomotionTeleport;

	// Token: 0x04001457 RID: 5207
	[NonSerialized]
	public LocomotionTeleport.States TeleportState;

	// Token: 0x04001458 RID: 5208
	private readonly Action<bool, Vector3?, Quaternion?, Quaternion?> _updateTeleportDestinationAction;

	// Token: 0x04001459 RID: 5209
	private bool _eventsActive;
}
