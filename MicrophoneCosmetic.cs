using System;
using UnityEngine;

// Token: 0x0200010C RID: 268
public class MicrophoneCosmetic : MonoBehaviour
{
	// Token: 0x060006DB RID: 1755 RVA: 0x00026B58 File Offset: 0x00024D58
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		if (!Application.isEditor && Application.platform == RuntimePlatform.Android && Microphone.devices.Length != 0)
		{
			this.audioSource.clip = Microphone.Start(Microphone.devices[0], true, 10, 16000);
		}
		else
		{
			int sampleRate = AudioSettings.GetConfiguration().sampleRate;
			this.audioSource.clip = Microphone.Start(null, true, 10, sampleRate);
		}
		this.audioSource.loop = true;
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x00026BD8 File Offset: 0x00024DD8
	private void OnEnable()
	{
		int num = ((Application.platform == RuntimePlatform.Android && Microphone.devices.Length != 0) ? Microphone.GetPosition(Microphone.devices[0]) : Microphone.GetPosition(null));
		num -= 10;
		if ((float)num < 0f)
		{
			num = this.audioSource.clip.samples + num - 1;
		}
		this.audioSource.GTPlay();
		this.audioSource.timeSamples = num;
	}

	// Token: 0x060006DD RID: 1757 RVA: 0x00026C45 File Offset: 0x00024E45
	private void OnDisable()
	{
		this.audioSource.GTStop();
	}

	// Token: 0x060006DE RID: 1758 RVA: 0x00026C54 File Offset: 0x00024E54
	private void Update()
	{
		Vector3 vector = this.mouthTransform.position - base.transform.position;
		float sqrMagnitude = vector.sqrMagnitude;
		float num = 0f;
		if (sqrMagnitude < this.mouthProximityRampRange.x * this.mouthProximityRampRange.x)
		{
			float magnitude = vector.magnitude;
			num = Mathf.InverseLerp(this.mouthProximityRampRange.x, this.mouthProximityRampRange.y, magnitude);
		}
		if (num != this.audioSource.volume)
		{
			this.audioSource.volume = num;
		}
		int num2 = (this.audioSource.timeSamples -= 10);
		if ((float)num2 < 0f)
		{
			num2 = this.audioSource.clip.samples + num2 - 1;
		}
		this.audioSource.clip.SetData(this.zero, num2);
	}

	// Token: 0x060006DF RID: 1759 RVA: 0x000023F4 File Offset: 0x000005F4
	private void OnAudioFilterRead(float[] data, int channels)
	{
	}

	// Token: 0x04000825 RID: 2085
	[SerializeField]
	private Transform mouthTransform;

	// Token: 0x04000826 RID: 2086
	[SerializeField]
	private Vector2 mouthProximityRampRange = new Vector2(0.6f, 0.3f);

	// Token: 0x04000827 RID: 2087
	private AudioSource audioSource;

	// Token: 0x04000828 RID: 2088
	private float[] zero = new float[1];
}
