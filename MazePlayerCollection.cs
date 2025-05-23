using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000B8 RID: 184
public class MazePlayerCollection : MonoBehaviour
{
	// Token: 0x0600047D RID: 1149 RVA: 0x00019DD1 File Offset: 0x00017FD1
	private void Start()
	{
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x00019DE9 File Offset: 0x00017FE9
	private void OnDestroy()
	{
		NetworkSystem.Instance.OnPlayerLeft -= this.OnPlayerLeftRoom;
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x00019E04 File Offset: 0x00018004
	public void OnTriggerEnter(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (!this.containedRigs.Contains(component))
		{
			this.containedRigs.Add(component);
		}
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x00019E54 File Offset: 0x00018054
	public void OnTriggerExit(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (this.containedRigs.Contains(component))
		{
			this.containedRigs.Remove(component);
		}
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x00019EA8 File Offset: 0x000180A8
	public void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		this.containedRigs.RemoveAll((VRRig r) => ((r != null) ? r.creator : null) == null || r.creator == otherPlayer);
	}

	// Token: 0x0400052E RID: 1326
	public List<VRRig> containedRigs = new List<VRRig>();

	// Token: 0x0400052F RID: 1327
	public List<MonkeyeAI> monkeyeAis = new List<MonkeyeAI>();
}
