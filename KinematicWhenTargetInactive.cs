using System;
using UnityEngine;

// Token: 0x0200046D RID: 1133
public class KinematicWhenTargetInactive : MonoBehaviour
{
	// Token: 0x06001BD8 RID: 7128 RVA: 0x00088D68 File Offset: 0x00086F68
	private void LateUpdate()
	{
		if (!this.target.activeSelf)
		{
			foreach (Rigidbody rigidbody in this.rigidBodies)
			{
				if (!rigidbody.isKinematic)
				{
					rigidbody.isKinematic = true;
				}
			}
			return;
		}
		foreach (Rigidbody rigidbody2 in this.rigidBodies)
		{
			if (rigidbody2.isKinematic)
			{
				rigidbody2.isKinematic = false;
			}
		}
	}

	// Token: 0x04001EF2 RID: 7922
	public Rigidbody[] rigidBodies;

	// Token: 0x04001EF3 RID: 7923
	public GameObject target;
}
