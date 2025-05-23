using System;
using System.Collections;
using UnityEngine;

// Token: 0x020004AC RID: 1196
public class AtticHider : MonoBehaviour
{
	// Token: 0x06001CDC RID: 7388 RVA: 0x0008C0F2 File Offset: 0x0008A2F2
	private void Start()
	{
		this.OnZoneChanged();
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06001CDD RID: 7389 RVA: 0x0008C120 File Offset: 0x0008A320
	private void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06001CDE RID: 7390 RVA: 0x0008C148 File Offset: 0x0008A348
	private void OnZoneChanged()
	{
		if (this.AtticRenderer == null)
		{
			return;
		}
		if (ZoneManagement.instance.IsZoneActive(GTZone.attic))
		{
			if (this._coroutine != null)
			{
				base.StopCoroutine(this._coroutine);
				this._coroutine = null;
			}
			this._coroutine = base.StartCoroutine(this.WaitForAtticLoad());
			return;
		}
		if (this._coroutine != null)
		{
			base.StopCoroutine(this._coroutine);
			this._coroutine = null;
		}
		this.AtticRenderer.enabled = true;
	}

	// Token: 0x06001CDF RID: 7391 RVA: 0x0008C1C7 File Offset: 0x0008A3C7
	private IEnumerator WaitForAtticLoad()
	{
		while (!ZoneManagement.instance.IsSceneLoaded(GTZone.attic))
		{
			yield return new WaitForSeconds(0.2f);
		}
		yield return null;
		this.AtticRenderer.enabled = false;
		this._coroutine = null;
		yield break;
	}

	// Token: 0x04002017 RID: 8215
	[SerializeField]
	private MeshRenderer AtticRenderer;

	// Token: 0x04002018 RID: 8216
	private Coroutine _coroutine;
}
