using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200056E RID: 1390
public class GameGrabbable : MonoBehaviour
{
	// Token: 0x060021F9 RID: 8697 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Awake()
	{
	}

	// Token: 0x060021FA RID: 8698 RVA: 0x000AA574 File Offset: 0x000A8774
	public void GetBestGrabPoint(Vector3 handPos, Quaternion handRot, int handIndex, out GameGrab grab)
	{
		float num = 0.15f;
		grab = default(GameGrab);
		grab.position = base.transform.position;
		grab.rotation = base.transform.rotation;
		bool flag = GamePlayer.IsLeftHand(handIndex);
		if (this.snapGrabPoints != null)
		{
			for (int i = 0; i < this.snapGrabPoints.Count; i++)
			{
				GameGrabbable.SnapGrabPoints snapGrabPoints = this.snapGrabPoints[i];
				if (snapGrabPoints.isLeftHand == flag && Vector3.Dot(snapGrabPoints.handTransform.rotation * GameGrabbable.GRAB_UP, handRot * GameGrabbable.GRAB_UP) >= 0f && Vector3.Dot(snapGrabPoints.handTransform.rotation * GameGrabbable.GRAB_PALM, handRot * GameGrabbable.GRAB_PALM) >= 0f && (double)(handPos - snapGrabPoints.handTransform.position).sqrMagnitude <= 0.0225)
				{
					grab.position = handPos + handRot * Quaternion.Inverse(snapGrabPoints.handTransform.localRotation) * -snapGrabPoints.handTransform.localPosition;
					grab.rotation = handRot * Quaternion.Inverse(snapGrabPoints.handTransform.localRotation);
				}
			}
		}
		Vector3 vector = grab.position - handPos;
		if (vector.sqrMagnitude > num * num)
		{
			grab.position = handPos + vector.normalized * num;
		}
	}

	// Token: 0x04002607 RID: 9735
	public GameEntity gameEntity;

	// Token: 0x04002608 RID: 9736
	public List<GameGrabbable.SnapGrabPoints> snapGrabPoints;

	// Token: 0x04002609 RID: 9737
	private static Vector3 GRAB_UP = new Vector3(0f, 0f, 1f);

	// Token: 0x0400260A RID: 9738
	private static Vector3 GRAB_PALM = new Vector3(1f, 0f, 0f);

	// Token: 0x0200056F RID: 1391
	[Serializable]
	public class SnapGrabPoints
	{
		// Token: 0x0400260B RID: 9739
		public bool isLeftHand;

		// Token: 0x0400260C RID: 9740
		public Transform handTransform;
	}
}
