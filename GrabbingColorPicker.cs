using System;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x02000451 RID: 1105
public class GrabbingColorPicker : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06001B3F RID: 6975 RVA: 0x00086220 File Offset: 0x00084420
	private void Start()
	{
		float @float = PlayerPrefs.GetFloat("redValue", 0f);
		float float2 = PlayerPrefs.GetFloat("greenValue", 0f);
		float float3 = PlayerPrefs.GetFloat("blueValue", 0f);
		this.Segment1 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, @float));
		this.Segment2 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, float2));
		this.Segment3 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, float3));
		this.R_PushSlider.SetProgress(@float);
		this.G_PushSlider.SetProgress(float2);
		this.B_PushSlider.SetProgress(float3);
		this.UpdateDisplay();
		if (GorillaTagger.Instance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.OnColorChanged += this.HandleLocalColorChanged;
		}
	}

	// Token: 0x06001B40 RID: 6976 RVA: 0x00086310 File Offset: 0x00084510
	private void OnDestroy()
	{
		if (GorillaTagger.Instance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.OnColorChanged -= this.HandleLocalColorChanged;
		}
	}

	// Token: 0x06001B41 RID: 6977 RVA: 0x00017251 File Offset: 0x00015451
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001B42 RID: 6978 RVA: 0x0001725A File Offset: 0x0001545A
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001B43 RID: 6979 RVA: 0x0008634C File Offset: 0x0008454C
	public void SliceUpdate()
	{
		float num = Vector3.Distance(base.transform.position, GTPlayer.Instance.transform.position);
		this.hasUpdated = false;
		if (num < 5f)
		{
			int segment = this.Segment1;
			int segment2 = this.Segment2;
			int segment3 = this.Segment3;
			this.Segment1 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, this.R_PushSlider.GetProgress()));
			this.Segment2 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, this.G_PushSlider.GetProgress()));
			this.Segment3 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, this.B_PushSlider.GetProgress()));
			if (segment != this.Segment1 || segment2 != this.Segment2 || segment3 != this.Segment3)
			{
				this.hasUpdated = true;
				PlayerPrefs.SetFloat("redValue", (float)this.Segment1 / 9f);
				PlayerPrefs.SetFloat("greenValue", (float)this.Segment2 / 9f);
				PlayerPrefs.SetFloat("blueValue", (float)this.Segment3 / 9f);
				GorillaTagger.Instance.UpdateColor((float)this.Segment1 / 9f, (float)this.Segment2 / 9f, (float)this.Segment3 / 9f);
				GorillaComputer.instance.UpdateColor((float)this.Segment1 / 9f, (float)this.Segment2 / 9f, (float)this.Segment3 / 9f);
				PlayerPrefs.Save();
				if (NetworkSystem.Instance.InRoom)
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
					{
						(float)this.Segment1 / 9f,
						(float)this.Segment2 / 9f,
						(float)this.Segment3 / 9f
					});
				}
				this.UpdateDisplay();
				if (segment != this.Segment1)
				{
					this.R_SliderAudio.transform.position = this.R_PushSlider.transform.position;
					this.R_SliderAudio.GTPlay();
				}
				if (segment2 != this.Segment2)
				{
					this.G_SliderAudio.transform.position = this.G_PushSlider.transform.position;
					this.G_SliderAudio.GTPlay();
				}
				if (segment3 != this.Segment3)
				{
					this.B_SliderAudio.transform.position = this.B_PushSlider.transform.position;
					this.B_SliderAudio.GTPlay();
				}
			}
		}
	}

	// Token: 0x06001B44 RID: 6980 RVA: 0x000865E8 File Offset: 0x000847E8
	private void SetSliderColors(float r, float g, float b)
	{
		if (!this.hasUpdated)
		{
			this.Segment1 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, r));
			this.Segment2 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, g));
			this.Segment3 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, b));
			this.R_PushSlider.SetProgress(r);
			this.G_PushSlider.SetProgress(g);
			this.B_PushSlider.SetProgress(b);
			this.UpdateDisplay();
		}
	}

	// Token: 0x06001B45 RID: 6981 RVA: 0x00086678 File Offset: 0x00084878
	private void HandleLocalColorChanged(Color newColor)
	{
		this.SetSliderColors(newColor.r, newColor.g, newColor.b);
	}

	// Token: 0x06001B46 RID: 6982 RVA: 0x00086694 File Offset: 0x00084894
	private void UpdateDisplay()
	{
		this.textR.text = this.Segment1.ToString();
		this.textG.text = this.Segment2.ToString();
		this.textB.text = this.Segment3.ToString();
		Color color = new Color((float)this.Segment1 / 9f, (float)this.Segment2 / 9f, (float)this.Segment3 / 9f);
		Renderer[] componentsInChildren = this.ColorSwatch.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Material[] materials = componentsInChildren[i].materials;
			for (int j = 0; j < materials.Length; j++)
			{
				materials[j].color = color;
			}
		}
	}

	// Token: 0x06001B48 RID: 6984 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x04001E36 RID: 7734
	[SerializeField]
	private PushableSlider R_PushSlider;

	// Token: 0x04001E37 RID: 7735
	[SerializeField]
	private PushableSlider G_PushSlider;

	// Token: 0x04001E38 RID: 7736
	[SerializeField]
	private PushableSlider B_PushSlider;

	// Token: 0x04001E39 RID: 7737
	[SerializeField]
	private AudioSource R_SliderAudio;

	// Token: 0x04001E3A RID: 7738
	[SerializeField]
	private AudioSource G_SliderAudio;

	// Token: 0x04001E3B RID: 7739
	[SerializeField]
	private AudioSource B_SliderAudio;

	// Token: 0x04001E3C RID: 7740
	[SerializeField]
	private TextMeshPro textR;

	// Token: 0x04001E3D RID: 7741
	[SerializeField]
	private TextMeshPro textG;

	// Token: 0x04001E3E RID: 7742
	[SerializeField]
	private TextMeshPro textB;

	// Token: 0x04001E3F RID: 7743
	[SerializeField]
	private GameObject ColorSwatch;

	// Token: 0x04001E40 RID: 7744
	private int Segment1;

	// Token: 0x04001E41 RID: 7745
	private int Segment2;

	// Token: 0x04001E42 RID: 7746
	private int Segment3;

	// Token: 0x04001E43 RID: 7747
	private bool hasUpdated;
}
