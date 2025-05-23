using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004DB RID: 1243
public class BuilderZoneRenderers : MonoBehaviour
{
	// Token: 0x06001E07 RID: 7687 RVA: 0x00091EB0 File Offset: 0x000900B0
	private void Start()
	{
		this.allRenderers.Clear();
		this.allRenderers.AddRange(this.renderers);
		foreach (GameObject gameObject in this.rootObjects)
		{
			this.allRenderers.AddRange(gameObject.GetComponentsInChildren<Renderer>(true));
		}
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		this.inBuilderZone = true;
		this.OnZoneChanged();
	}

	// Token: 0x06001E08 RID: 7688 RVA: 0x00091F60 File Offset: 0x00090160
	private void OnDestroy()
	{
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
	}

	// Token: 0x06001E09 RID: 7689 RVA: 0x00091F98 File Offset: 0x00090198
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks);
		if (flag && !this.inBuilderZone)
		{
			this.inBuilderZone = flag;
			foreach (Renderer renderer in this.allRenderers)
			{
				renderer.enabled = true;
			}
			using (List<Canvas>.Enumerator enumerator2 = this.canvases.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Canvas canvas = enumerator2.Current;
					canvas.enabled = true;
				}
				return;
			}
		}
		if (!flag && this.inBuilderZone)
		{
			this.inBuilderZone = flag;
			foreach (Renderer renderer2 in this.allRenderers)
			{
				renderer2.enabled = false;
			}
			foreach (Canvas canvas2 in this.canvases)
			{
				canvas2.enabled = false;
			}
		}
	}

	// Token: 0x04002142 RID: 8514
	public List<Renderer> renderers;

	// Token: 0x04002143 RID: 8515
	public List<Canvas> canvases;

	// Token: 0x04002144 RID: 8516
	public List<GameObject> rootObjects;

	// Token: 0x04002145 RID: 8517
	private bool inBuilderZone;

	// Token: 0x04002146 RID: 8518
	private List<Renderer> allRenderers = new List<Renderer>(200);
}
