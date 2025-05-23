using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

// Token: 0x0200025B RID: 603
public class LckCameraEvents : MonoBehaviour
{
	// Token: 0x06000DC8 RID: 3528 RVA: 0x00047289 File Offset: 0x00045489
	private void OnEnable()
	{
		RenderPipelineManager.beginCameraRendering += this.RenderPipelineManagerOnbeginCameraRendering;
		RenderPipelineManager.endCameraRendering += this.RenderPipelineManagerOnendCameraRendering;
	}

	// Token: 0x06000DC9 RID: 3529 RVA: 0x000472AD File Offset: 0x000454AD
	private void OnDisable()
	{
		RenderPipelineManager.beginCameraRendering -= this.RenderPipelineManagerOnbeginCameraRendering;
		RenderPipelineManager.endCameraRendering -= this.RenderPipelineManagerOnendCameraRendering;
	}

	// Token: 0x06000DCA RID: 3530 RVA: 0x000472D1 File Offset: 0x000454D1
	private void RenderPipelineManagerOnbeginCameraRendering(ScriptableRenderContext scriptableRenderContext, Camera camera)
	{
		if (this._camera != camera)
		{
			return;
		}
		UnityEvent unityEvent = this.onPreRender;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06000DCB RID: 3531 RVA: 0x000472F2 File Offset: 0x000454F2
	private void RenderPipelineManagerOnendCameraRendering(ScriptableRenderContext scriptableRenderContext, Camera camera)
	{
		if (this._camera != camera)
		{
			return;
		}
		UnityEvent unityEvent = this.onPostRender;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x04001148 RID: 4424
	[SerializeField]
	private Camera _camera;

	// Token: 0x04001149 RID: 4425
	public UnityEvent onPreRender;

	// Token: 0x0400114A RID: 4426
	public UnityEvent onPostRender;
}
