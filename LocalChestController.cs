using System;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x020001EA RID: 490
public class LocalChestController : MonoBehaviour
{
	// Token: 0x06000B5A RID: 2906 RVA: 0x0003CBF0 File Offset: 0x0003ADF0
	private void OnTriggerEnter(Collider other)
	{
		if (this.isOpen)
		{
			return;
		}
		TransformFollow component = other.GetComponent<TransformFollow>();
		if (component == null)
		{
			return;
		}
		Transform transformToFollow = component.transformToFollow;
		if (transformToFollow == null)
		{
			return;
		}
		VRRig componentInParent = transformToFollow.GetComponentInParent<VRRig>();
		if (componentInParent == null)
		{
			return;
		}
		if (this.playerCollectionVolume != null && !this.playerCollectionVolume.containedRigs.Contains(componentInParent))
		{
			return;
		}
		this.isOpen = true;
		this.director.Play();
	}

	// Token: 0x04000DF9 RID: 3577
	public PlayableDirector director;

	// Token: 0x04000DFA RID: 3578
	public MazePlayerCollection playerCollectionVolume;

	// Token: 0x04000DFB RID: 3579
	private bool isOpen;
}
