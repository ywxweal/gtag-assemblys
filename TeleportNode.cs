using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020009F9 RID: 2553
public class TeleportNode : GorillaTriggerBox
{
	// Token: 0x06003D12 RID: 15634 RVA: 0x00122284 File Offset: 0x00120484
	public override void OnBoxTriggered()
	{
		if (Time.time - this.teleportTime < 0.1f)
		{
			return;
		}
		base.OnBoxTriggered();
		Transform transform;
		Transform transform2;
		if (this.teleportFromRef.TryResolve<Transform>(out transform) && this.teleportToRef.TryResolve<Transform>(out transform2))
		{
			GTPlayer instance = GTPlayer.Instance;
			Vector3 vector = transform2.TransformPoint(transform.InverseTransformPoint(instance.transform.position));
			instance.TeleportTo(vector, instance.transform.rotation);
			this.teleportTime = Time.time;
		}
	}

	// Token: 0x040040CE RID: 16590
	[SerializeField]
	private XSceneRef teleportFromRef;

	// Token: 0x040040CF RID: 16591
	[SerializeField]
	private XSceneRef teleportToRef;

	// Token: 0x040040D0 RID: 16592
	private float teleportTime;
}
