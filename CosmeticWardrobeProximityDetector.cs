using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000450 RID: 1104
[RequireComponent(typeof(SphereCollider))]
public class CosmeticWardrobeProximityDetector : MonoBehaviour
{
	// Token: 0x06001B3A RID: 6970 RVA: 0x00086039 File Offset: 0x00084239
	private void OnEnable()
	{
		if (this.wardrobeNearbyCollider != null)
		{
			CosmeticWardrobeProximityDetector.wardrobeNearbyDetection.Add(this.wardrobeNearbyCollider);
		}
	}

	// Token: 0x06001B3B RID: 6971 RVA: 0x00086059 File Offset: 0x00084259
	private void OnDisable()
	{
		if (this.wardrobeNearbyCollider != null)
		{
			CosmeticWardrobeProximityDetector.wardrobeNearbyDetection.Remove(this.wardrobeNearbyCollider);
		}
	}

	// Token: 0x06001B3C RID: 6972 RVA: 0x0008607C File Offset: 0x0008427C
	public static bool IsUserNearWardrobe(string userID)
	{
		int num = LayerMask.GetMask(new string[] { "Gorilla Tag Collider" }) | LayerMask.GetMask(new string[] { "Gorilla Body Collider" });
		foreach (SphereCollider sphereCollider in CosmeticWardrobeProximityDetector.wardrobeNearbyDetection)
		{
			int num2 = Physics.OverlapSphereNonAlloc(sphereCollider.transform.position, sphereCollider.radius, CosmeticWardrobeProximityDetector.overlapColliders, num);
			num2 = Mathf.Min(num2, CosmeticWardrobeProximityDetector.overlapColliders.Length);
			if (num2 > 0)
			{
				for (int i = 0; i < num2; i++)
				{
					Collider collider = CosmeticWardrobeProximityDetector.overlapColliders[i];
					if (!(collider == null))
					{
						GameObject gameObject = collider.attachedRigidbody.gameObject;
						VRRig component = gameObject.GetComponent<VRRig>();
						if (component == null || component.creator == null || component.creator.IsNull || string.IsNullOrEmpty(component.creator.UserId))
						{
							if (gameObject.GetComponent<GTPlayer>() == null || NetworkSystem.Instance.LocalPlayer == null)
							{
								goto IL_0135;
							}
							if (userID == NetworkSystem.Instance.LocalPlayer.UserId)
							{
								return true;
							}
						}
						else if (userID == component.creator.UserId)
						{
							return true;
						}
						CosmeticWardrobeProximityDetector.overlapColliders[i] = null;
					}
					IL_0135:;
				}
			}
		}
		return false;
	}

	// Token: 0x04001E33 RID: 7731
	[SerializeField]
	private SphereCollider wardrobeNearbyCollider;

	// Token: 0x04001E34 RID: 7732
	private static List<SphereCollider> wardrobeNearbyDetection = new List<SphereCollider>();

	// Token: 0x04001E35 RID: 7733
	private static readonly Collider[] overlapColliders = new Collider[20];
}
