using System;
using UnityEngine;

// Token: 0x0200016E RID: 366
public class DJDeckEqualizer : MonoBehaviour
{
	// Token: 0x06000933 RID: 2355 RVA: 0x00031B55 File Offset: 0x0002FD55
	private void Start()
	{
		this.inputColorHash = this.inputColorProperty;
		this.material = this.display.material;
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x00031B7C File Offset: 0x0002FD7C
	private void Update()
	{
		Color color = default(Color);
		color.r = 0.25f;
		color.g = 0.25f;
		color.b = 0.5f;
		for (int i = 0; i < this.redTracks.Length; i++)
		{
			AudioSource audioSource = this.redTracks[i];
			if (audioSource.isPlaying)
			{
				color.r = Mathf.Lerp(0.25f, 1f, this.redTrackCurves[i].Evaluate(audioSource.time));
				break;
			}
		}
		for (int j = 0; j < this.greenTracks.Length; j++)
		{
			AudioSource audioSource2 = this.greenTracks[j];
			if (audioSource2.isPlaying)
			{
				color.g = Mathf.Lerp(0.25f, 1f, this.greenTrackCurves[j].Evaluate(audioSource2.time));
				break;
			}
		}
		this.material.SetColor(this.inputColorHash, color);
	}

	// Token: 0x04000AFD RID: 2813
	[SerializeField]
	private MeshRenderer display;

	// Token: 0x04000AFE RID: 2814
	[SerializeField]
	private AnimationCurve[] redTrackCurves;

	// Token: 0x04000AFF RID: 2815
	[SerializeField]
	private AnimationCurve[] greenTrackCurves;

	// Token: 0x04000B00 RID: 2816
	[SerializeField]
	private AudioSource[] redTracks;

	// Token: 0x04000B01 RID: 2817
	[SerializeField]
	private AudioSource[] greenTracks;

	// Token: 0x04000B02 RID: 2818
	private Material material;

	// Token: 0x04000B03 RID: 2819
	[SerializeField]
	private string inputColorProperty;

	// Token: 0x04000B04 RID: 2820
	private ShaderHashId inputColorHash;
}
