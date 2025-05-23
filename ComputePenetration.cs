using System;
using Drawing;
using UnityEngine;

// Token: 0x0200074E RID: 1870
public class ComputePenetration : MonoBehaviour
{
	// Token: 0x06002ECC RID: 11980 RVA: 0x000EA92F File Offset: 0x000E8B2F
	public void Compute()
	{
		if (this.colliderA == null)
		{
			return;
		}
		this.colliderB == null;
	}

	// Token: 0x06002ECD RID: 11981 RVA: 0x000EA950 File Offset: 0x000E8B50
	public void OnDrawGizmos()
	{
		if (this.colliderA.AsNull<Collider>() == null)
		{
			return;
		}
		if (this.colliderB.AsNull<Collider>() == null)
		{
			return;
		}
		Transform transform = this.colliderA.transform;
		Transform transform2 = this.colliderB.transform;
		if (this.lastUpdate.HasElapsed(0.5f, true))
		{
			this.overlapped = Physics.ComputePenetration(this.colliderA, transform.position, transform.rotation, this.colliderB, transform2.position, transform2.rotation, out this.direction, out this.distance);
		}
		Color color = (this.overlapped ? Color.red : Color.green);
		this.DrawCollider(this.colliderA, color);
		this.DrawCollider(this.colliderB, color);
		if (this.overlapped)
		{
			Vector3 position = this.colliderB.transform.position;
			Vector3 vector = position + this.direction * this.distance;
			Gizmos.DrawLine(position, vector);
		}
	}

	// Token: 0x06002ECE RID: 11982 RVA: 0x000EAA50 File Offset: 0x000E8C50
	private unsafe void DrawCollider(Collider c, Color color)
	{
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithMatrix(c.transform.localToWorldMatrix))
		{
			commandBuilder.PushColor(color);
			BoxCollider boxCollider = c as BoxCollider;
			if (boxCollider == null)
			{
				SphereCollider sphereCollider = c as SphereCollider;
				if (sphereCollider == null)
				{
					CapsuleCollider capsuleCollider = c as CapsuleCollider;
					if (capsuleCollider != null)
					{
						commandBuilder.WireCapsule(capsuleCollider.center, Vector3.up, capsuleCollider.height, capsuleCollider.radius);
					}
				}
				else
				{
					commandBuilder.WireSphere(sphereCollider.center, sphereCollider.radius);
				}
			}
			else
			{
				commandBuilder.WireBox(boxCollider.center, boxCollider.size);
			}
			commandBuilder.PopColor();
		}
	}

	// Token: 0x0400354E RID: 13646
	public Collider colliderA;

	// Token: 0x0400354F RID: 13647
	public Collider colliderB;

	// Token: 0x04003550 RID: 13648
	public bool overlapped;

	// Token: 0x04003551 RID: 13649
	public Vector3 direction;

	// Token: 0x04003552 RID: 13650
	public float distance;

	// Token: 0x04003553 RID: 13651
	private TimeSince lastUpdate = TimeSince.Now();
}
