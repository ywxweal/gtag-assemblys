using System;
using UnityEngine;

// Token: 0x020001C5 RID: 453
public static class GTAudioClipExtensions
{
	// Token: 0x06000AB2 RID: 2738 RVA: 0x0003A4AC File Offset: 0x000386AC
	public static float GetPeakMagnitude(this AudioClip audioClip)
	{
		if (audioClip == null)
		{
			return 0f;
		}
		float num = float.NegativeInfinity;
		float[] array = new float[audioClip.samples];
		audioClip.GetData(array, 0);
		foreach (float num2 in array)
		{
			num = Mathf.Max(num, Mathf.Abs(num2));
		}
		return num;
	}

	// Token: 0x06000AB3 RID: 2739 RVA: 0x0003A508 File Offset: 0x00038708
	public static float GetRMSMagnitude(this AudioClip audioClip)
	{
		if (audioClip == null)
		{
			return 0f;
		}
		float num = 0f;
		float[] array = new float[audioClip.samples];
		audioClip.GetData(array, 0);
		foreach (float num2 in array)
		{
			num += num2 * num2;
		}
		return Mathf.Sqrt(num / (float)array.Length);
	}
}
