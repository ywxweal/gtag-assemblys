using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020004A3 RID: 1187
public class GravityOverrideVolume : MonoBehaviour
{
	// Token: 0x06001CBE RID: 7358 RVA: 0x0008BBBC File Offset: 0x00089DBC
	private void OnEnable()
	{
		if (this.triggerEvents != null)
		{
			this.triggerEvents.CompositeTriggerEnter += this.OnColliderEnteredVolume;
			this.triggerEvents.CompositeTriggerExit += this.OnColliderExitedVolume;
		}
	}

	// Token: 0x06001CBF RID: 7359 RVA: 0x0008BBFA File Offset: 0x00089DFA
	private void OnDisable()
	{
		if (this.triggerEvents != null)
		{
			this.triggerEvents.CompositeTriggerEnter -= this.OnColliderEnteredVolume;
			this.triggerEvents.CompositeTriggerExit -= this.OnColliderExitedVolume;
		}
	}

	// Token: 0x06001CC0 RID: 7360 RVA: 0x0008BC38 File Offset: 0x00089E38
	private void OnColliderEnteredVolume(Collider collider)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && collider == instance.headCollider)
		{
			instance.SetGravityOverride(this, new Action<GTPlayer>(this.GravityOverrideFunction));
		}
	}

	// Token: 0x06001CC1 RID: 7361 RVA: 0x0008BC78 File Offset: 0x00089E78
	private void OnColliderExitedVolume(Collider collider)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && collider == instance.headCollider)
		{
			instance.UnsetGravityOverride(this);
		}
	}

	// Token: 0x06001CC2 RID: 7362 RVA: 0x0008BCAC File Offset: 0x00089EAC
	public void GravityOverrideFunction(GTPlayer player)
	{
		GravityOverrideVolume.GravityType gravityType = this.gravityType;
		if (gravityType == GravityOverrideVolume.GravityType.Directional)
		{
			Vector3 forward = this.referenceTransform.forward;
			player.AddForce(forward * this.strength, ForceMode.Acceleration);
			return;
		}
		if (gravityType != GravityOverrideVolume.GravityType.Radial)
		{
			return;
		}
		Vector3 normalized = (this.referenceTransform.position - player.headCollider.transform.position).normalized;
		player.AddForce(normalized * this.strength, ForceMode.Acceleration);
	}

	// Token: 0x04001FFF RID: 8191
	[SerializeField]
	private GravityOverrideVolume.GravityType gravityType;

	// Token: 0x04002000 RID: 8192
	[SerializeField]
	private float strength = 9.8f;

	// Token: 0x04002001 RID: 8193
	[SerializeField]
	[Tooltip("In Radial: the center point of gravity, In Directional: the forward vector of this transform defines the direction")]
	private Transform referenceTransform;

	// Token: 0x04002002 RID: 8194
	[SerializeField]
	private CompositeTriggerEvents triggerEvents;

	// Token: 0x020004A4 RID: 1188
	public enum GravityType
	{
		// Token: 0x04002004 RID: 8196
		Directional,
		// Token: 0x04002005 RID: 8197
		Radial
	}
}
