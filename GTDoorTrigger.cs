using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

// Token: 0x020001D9 RID: 473
public class GTDoorTrigger : MonoBehaviour
{
	// Token: 0x1700011B RID: 283
	// (get) Token: 0x06000B15 RID: 2837 RVA: 0x0003B7F8 File Offset: 0x000399F8
	public int overlapCount
	{
		get
		{
			return this.overlappingColliders.Count;
		}
	}

	// Token: 0x1700011C RID: 284
	// (get) Token: 0x06000B16 RID: 2838 RVA: 0x0003B805 File Offset: 0x00039A05
	public bool TriggeredThisFrame
	{
		get
		{
			return this.lastTriggeredFrame == Time.frameCount;
		}
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x0003B814 File Offset: 0x00039A14
	public void ValidateOverlappingColliders()
	{
		for (int i = this.overlappingColliders.Count - 1; i >= 0; i--)
		{
			if (this.overlappingColliders[i] == null || !this.overlappingColliders[i].gameObject.activeInHierarchy || !this.overlappingColliders[i].enabled)
			{
				this.overlappingColliders.RemoveAt(i);
			}
		}
	}

	// Token: 0x06000B18 RID: 2840 RVA: 0x0003B884 File Offset: 0x00039A84
	private void OnTriggerEnter(Collider other)
	{
		if (!this.overlappingColliders.Contains(other))
		{
			this.overlappingColliders.Add(other);
		}
		this.lastTriggeredFrame = Time.frameCount;
		this.TriggeredEvent.Invoke();
		if (this.timeline != null && (this.timeline.time == 0.0 || this.timeline.time >= this.timeline.duration))
		{
			this.timeline.Play();
		}
	}

	// Token: 0x06000B19 RID: 2841 RVA: 0x0003B908 File Offset: 0x00039B08
	private void OnTriggerExit(Collider other)
	{
		this.overlappingColliders.Remove(other);
	}

	// Token: 0x04000D8D RID: 3469
	[Tooltip("Optional timeline to play to animate the thing getting activated, play sound, particles, etc...")]
	public PlayableDirector timeline;

	// Token: 0x04000D8E RID: 3470
	private int lastTriggeredFrame = -1;

	// Token: 0x04000D8F RID: 3471
	private List<Collider> overlappingColliders = new List<Collider>(20);

	// Token: 0x04000D90 RID: 3472
	internal UnityEvent TriggeredEvent = new UnityEvent();
}
