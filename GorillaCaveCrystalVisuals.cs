using System;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x020005F7 RID: 1527
public class GorillaCaveCrystalVisuals : MonoBehaviour
{
	// Token: 0x1700039A RID: 922
	// (get) Token: 0x060025AC RID: 9644 RVA: 0x000BB72A File Offset: 0x000B992A
	// (set) Token: 0x060025AD RID: 9645 RVA: 0x000BB732 File Offset: 0x000B9932
	public float lerp
	{
		get
		{
			return this._lerp;
		}
		set
		{
			this._lerp = value;
		}
	}

	// Token: 0x060025AE RID: 9646 RVA: 0x000BB73C File Offset: 0x000B993C
	public void Setup()
	{
		base.TryGetComponent<MeshRenderer>(out this._renderer);
		if (this._renderer == null)
		{
			return;
		}
		this._setup = GorillaCaveCrystalSetup.Instance;
		this._sharedMaterial = this._renderer.sharedMaterial;
		this._initialized = this.crysalPreset != null && this._renderer != null && this._sharedMaterial != null;
		this.Update();
	}

	// Token: 0x060025AF RID: 9647 RVA: 0x000BB7B8 File Offset: 0x000B99B8
	private void Start()
	{
		this.UpdateAlbedo();
		this.ForceUpdate();
	}

	// Token: 0x060025B0 RID: 9648 RVA: 0x000BB7C8 File Offset: 0x000B99C8
	public void UpdateAlbedo()
	{
		if (!this._initialized)
		{
			return;
		}
		if (this.instanceAlbedo == null)
		{
			return;
		}
		if (this._block == null)
		{
			this._block = new MaterialPropertyBlock();
		}
		this._renderer.GetPropertyBlock(this._block);
		this._block.SetTexture(GorillaCaveCrystalVisuals._MainTex, this.instanceAlbedo);
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x060025B1 RID: 9649 RVA: 0x000BB83D File Offset: 0x000B9A3D
	private void Awake()
	{
		this.UpdateAlbedo();
		this.Update();
	}

	// Token: 0x060025B2 RID: 9650 RVA: 0x000BB84C File Offset: 0x000B9A4C
	private void Update()
	{
		if (!this._initialized)
		{
			return;
		}
		if (Application.isPlaying)
		{
			int hashCode = new ValueTuple<CrystalVisualsPreset, float>(this.crysalPreset, this._lerp).GetHashCode();
			if (this._lastState == hashCode)
			{
				return;
			}
			this._lastState = hashCode;
		}
		if (this._block == null)
		{
			this._block = new MaterialPropertyBlock();
		}
		CrystalVisualsPreset.VisualState stateA = this.crysalPreset.stateA;
		CrystalVisualsPreset.VisualState stateB = this.crysalPreset.stateB;
		Color color = Color.Lerp(stateA.albedo, stateB.albedo, this._lerp);
		Color color2 = Color.Lerp(stateA.emission, stateB.emission, this._lerp);
		this._renderer.GetPropertyBlock(this._block);
		this._block.SetColor(GorillaCaveCrystalVisuals._Color, color);
		this._block.SetColor(GorillaCaveCrystalVisuals._EmissionColor, color2);
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x060025B3 RID: 9651 RVA: 0x000BB942 File Offset: 0x000B9B42
	public void ForceUpdate()
	{
		this._lastState = 0;
		this.Update();
	}

	// Token: 0x060025B4 RID: 9652 RVA: 0x000BB954 File Offset: 0x000B9B54
	private static void InitializeCrystals()
	{
		foreach (GorillaCaveCrystalVisuals gorillaCaveCrystalVisuals in Object.FindObjectsByType<GorillaCaveCrystalVisuals>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID))
		{
			gorillaCaveCrystalVisuals.UpdateAlbedo();
			gorillaCaveCrystalVisuals.ForceUpdate();
			gorillaCaveCrystalVisuals._lastState = -1;
		}
	}

	// Token: 0x04002A3F RID: 10815
	public CrystalVisualsPreset crysalPreset;

	// Token: 0x04002A40 RID: 10816
	[SerializeField]
	[Range(0f, 1f)]
	private float _lerp;

	// Token: 0x04002A41 RID: 10817
	[Space]
	public MeshRenderer _renderer;

	// Token: 0x04002A42 RID: 10818
	public Material _sharedMaterial;

	// Token: 0x04002A43 RID: 10819
	[SerializeField]
	public Texture2D instanceAlbedo;

	// Token: 0x04002A44 RID: 10820
	[SerializeField]
	private bool _initialized;

	// Token: 0x04002A45 RID: 10821
	[SerializeField]
	private int _lastState;

	// Token: 0x04002A46 RID: 10822
	[SerializeField]
	public GorillaCaveCrystalSetup _setup;

	// Token: 0x04002A47 RID: 10823
	private MaterialPropertyBlock _block;

	// Token: 0x04002A48 RID: 10824
	[NonSerialized]
	private bool _ranSetupOnce;

	// Token: 0x04002A49 RID: 10825
	private static readonly ShaderHashId _Color = "_Color";

	// Token: 0x04002A4A RID: 10826
	private static readonly ShaderHashId _EmissionColor = "_EmissionColor";

	// Token: 0x04002A4B RID: 10827
	private static readonly ShaderHashId _MainTex = "_MainTex";
}
