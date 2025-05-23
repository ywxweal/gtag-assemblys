using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200077F RID: 1919
public class TaggedColliderTrigger : MonoBehaviour
{
	// Token: 0x06003006 RID: 12294 RVA: 0x000EDF5A File Offset: 0x000EC15A
	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag(this.tag))
		{
			return;
		}
		if (this._sinceLastEnter.HasElapsed(this.enterHysteresis, true))
		{
			UnityEvent<Collider> unityEvent = this.onEnter;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(other);
		}
	}

	// Token: 0x06003007 RID: 12295 RVA: 0x000EDF90 File Offset: 0x000EC190
	private void OnTriggerExit(Collider other)
	{
		if (!other.CompareTag(this.tag))
		{
			return;
		}
		if (this._sinceLastExit.HasElapsed(this.exitHysteresis, true))
		{
			UnityEvent<Collider> unityEvent = this.onExit;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(other);
		}
	}

	// Token: 0x04003643 RID: 13891
	public new UnityTag tag;

	// Token: 0x04003644 RID: 13892
	public UnityEvent<Collider> onEnter = new UnityEvent<Collider>();

	// Token: 0x04003645 RID: 13893
	public UnityEvent<Collider> onExit = new UnityEvent<Collider>();

	// Token: 0x04003646 RID: 13894
	public float enterHysteresis = 0.125f;

	// Token: 0x04003647 RID: 13895
	public float exitHysteresis = 0.125f;

	// Token: 0x04003648 RID: 13896
	private TimeSince _sinceLastEnter;

	// Token: 0x04003649 RID: 13897
	private TimeSince _sinceLastExit;
}
