using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000305 RID: 773
public abstract class TeleportSupport : MonoBehaviour
{
	// Token: 0x17000212 RID: 530
	// (get) Token: 0x0600127C RID: 4732 RVA: 0x00057632 File Offset: 0x00055832
	// (set) Token: 0x0600127D RID: 4733 RVA: 0x0005763A File Offset: 0x0005583A
	private protected LocomotionTeleport LocomotionTeleport { protected get; private set; }

	// Token: 0x0600127E RID: 4734 RVA: 0x00057643 File Offset: 0x00055843
	protected virtual void OnEnable()
	{
		this.LocomotionTeleport = base.GetComponent<LocomotionTeleport>();
		this.AddEventHandlers();
	}

	// Token: 0x0600127F RID: 4735 RVA: 0x00057657 File Offset: 0x00055857
	protected virtual void OnDisable()
	{
		this.RemoveEventHandlers();
		this.LocomotionTeleport = null;
	}

	// Token: 0x06001280 RID: 4736 RVA: 0x00057666 File Offset: 0x00055866
	[Conditional("DEBUG_TELEPORT_EVENT_HANDLERS")]
	private void LogEventHandler(string msg)
	{
		Debug.Log("EventHandler: " + base.GetType().Name + ": " + msg);
	}

	// Token: 0x06001281 RID: 4737 RVA: 0x00057688 File Offset: 0x00055888
	protected virtual void AddEventHandlers()
	{
		this._eventsActive = true;
	}

	// Token: 0x06001282 RID: 4738 RVA: 0x00057691 File Offset: 0x00055891
	protected virtual void RemoveEventHandlers()
	{
		this._eventsActive = false;
	}

	// Token: 0x04001498 RID: 5272
	private bool _eventsActive;
}
