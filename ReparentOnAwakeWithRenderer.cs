using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200020D RID: 525
public class ReparentOnAwakeWithRenderer : MonoBehaviour, IBuildValidation
{
	// Token: 0x06000C1F RID: 3103 RVA: 0x00040247 File Offset: 0x0003E447
	public bool BuildValidationCheck()
	{
		if (base.GetComponent<MeshRenderer>() != null && this.myRenderer == null)
		{
			Debug.Log("needs a reference to its renderer since it has one");
			return false;
		}
		return true;
	}

	// Token: 0x06000C20 RID: 3104 RVA: 0x00040274 File Offset: 0x0003E474
	private void OnEnable()
	{
		base.transform.SetParent(this.newParent, true);
		if (this.sortLast)
		{
			base.transform.SetAsLastSibling();
		}
		else
		{
			base.transform.SetAsFirstSibling();
		}
		if (this.myRenderer != null)
		{
			this.myRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			this.myRenderer.lightProbeUsage = LightProbeUsage.CustomProvided;
			this.myRenderer.probeAnchor = this.newParent;
		}
	}

	// Token: 0x06000C21 RID: 3105 RVA: 0x000402EA File Offset: 0x0003E4EA
	[ContextMenu("Set Renderer")]
	public void SetMyRenderer()
	{
		this.myRenderer = base.GetComponent<MeshRenderer>();
	}

	// Token: 0x04000EE1 RID: 3809
	public Transform newParent;

	// Token: 0x04000EE2 RID: 3810
	public MeshRenderer myRenderer;

	// Token: 0x04000EE3 RID: 3811
	[Tooltip("We're mostly using this for UI elements like text and images, so this will help you separate these in whatever target parent object.Keep images and texts together, otherwise you'll get extra draw calls. Put images above text or they'll overlap weird tho lol")]
	public bool sortLast;
}
