using System;
using UnityEngine;

// Token: 0x02000A2B RID: 2603
public class VoiceLoudnessReactor : MonoBehaviour
{
	// Token: 0x06003DF0 RID: 15856 RVA: 0x00126158 File Offset: 0x00124358
	private void Start()
	{
		for (int i = 0; i < this.transformPositionTargets.Length; i++)
		{
			this.transformPositionTargets[i].Initial = this.transformPositionTargets[i].transform.localPosition;
		}
		for (int j = 0; j < this.transformScaleTargets.Length; j++)
		{
			this.transformScaleTargets[j].Initial = this.transformScaleTargets[j].transform.localScale;
		}
		for (int k = 0; k < this.transformRotationTargets.Length; k++)
		{
			this.transformRotationTargets[k].Initial = this.transformRotationTargets[k].transform.localRotation;
		}
		for (int l = 0; l < this.particleTargets.Length; l++)
		{
			this.particleTargets[l].Main = this.particleTargets[l].particleSystem.main;
			this.particleTargets[l].InitialSpeed = this.particleTargets[l].Main.startSpeedMultiplier;
			this.particleTargets[l].InitialSize = this.particleTargets[l].Main.startSizeMultiplier;
			this.particleTargets[l].Emission = this.particleTargets[l].particleSystem.emission;
			this.particleTargets[l].InitialRate = this.particleTargets[l].Emission.rateOverTimeMultiplier;
			this.particleTargets[l].Main.startSpeedMultiplier = 0f;
			this.particleTargets[l].Main.startSizeMultiplier = 0f;
			this.particleTargets[l].Emission.rateOverTimeMultiplier = 0f;
		}
		for (int m = 0; m < this.gameObjectEnableTargets.Length; m++)
		{
			this.gameObjectEnableTargets[m].GameObject.SetActive(!this.gameObjectEnableTargets[m].TurnOnAtThreshhold);
		}
		for (int n = 0; n < this.rendererColorTargets.Length; n++)
		{
			this.rendererColorTargets[n].Inititialize();
		}
	}

	// Token: 0x06003DF1 RID: 15857 RVA: 0x00126354 File Offset: 0x00124554
	private void OnEnable()
	{
		if (this.loudness != null)
		{
			return;
		}
		this.loudness = base.GetComponentInParent<GorillaSpeakerLoudness>(true);
		if (this.loudness == null)
		{
			GorillaTagger componentInParent = base.GetComponentInParent<GorillaTagger>();
			if (componentInParent != null)
			{
				this.loudness = componentInParent.offlineVRRig.GetComponent<GorillaSpeakerLoudness>();
			}
		}
		if (this.loudness != null)
		{
			this.frameLoudness = this.loudness.Loudness;
			this.frameSmoothedLoudness = this.loudness.SmoothedLoudness;
		}
	}

	// Token: 0x06003DF2 RID: 15858 RVA: 0x001263DC File Offset: 0x001245DC
	private void Update()
	{
		if (this.loudness == null)
		{
			return;
		}
		float num = this.loudness.Loudness;
		float num2 = this.frameLoudness;
		float num3 = this.loudness.Loudness;
		float num4 = this.frameLoudness;
		if (this.attack > 0f && this.loudness.Loudness > this.frameLoudness)
		{
			this.frameLoudness = Mathf.MoveTowards(this.frameLoudness, this.loudness.Loudness, Time.deltaTime / this.attack);
		}
		else if (this.decay > 0f && this.loudness.Loudness < this.frameLoudness)
		{
			this.frameLoudness = Mathf.MoveTowards(this.frameLoudness, this.loudness.Loudness, Time.deltaTime / this.decay);
		}
		else
		{
			this.frameLoudness = this.loudness.Loudness;
		}
		if (this.attack > 0f && this.loudness.SmoothedLoudness > this.frameSmoothedLoudness)
		{
			this.frameSmoothedLoudness = Mathf.MoveTowards(this.frameLoudness, this.loudness.SmoothedLoudness, Time.deltaTime * this.attack);
		}
		else if (this.decay > 0f && this.loudness.SmoothedLoudness < this.frameSmoothedLoudness)
		{
			this.frameSmoothedLoudness = Mathf.MoveTowards(this.frameLoudness, this.loudness.SmoothedLoudness, Time.deltaTime * this.decay);
		}
		else
		{
			this.frameSmoothedLoudness = this.loudness.SmoothedLoudness;
		}
		for (int i = 0; i < this.blendShapeTargets.Length; i++)
		{
			float num5 = (this.blendShapeTargets[i].UseSmoothedLoudness ? this.frameSmoothedLoudness : this.frameLoudness);
			this.blendShapeTargets[i].SkinnedMeshRenderer.SetBlendShapeWeight(this.blendShapeTargets[i].BlendShapeIndex, Mathf.Lerp(this.blendShapeTargets[i].minValue, this.blendShapeTargets[i].maxValue, num5));
		}
		for (int j = 0; j < this.transformPositionTargets.Length; j++)
		{
			float num6 = (this.transformPositionTargets[j].UseSmoothedLoudness ? this.frameSmoothedLoudness : this.frameLoudness) * this.transformPositionTargets[j].Scale;
			this.transformPositionTargets[j].transform.localPosition = Vector3.Lerp(this.transformPositionTargets[j].Initial, this.transformPositionTargets[j].Max, num6);
		}
		for (int k = 0; k < this.transformScaleTargets.Length; k++)
		{
			float num7 = (this.transformScaleTargets[k].UseSmoothedLoudness ? this.frameSmoothedLoudness : this.frameLoudness) * this.transformScaleTargets[k].Scale;
			this.transformScaleTargets[k].transform.localScale = Vector3.Lerp(this.transformScaleTargets[k].Initial, this.transformScaleTargets[k].Max, num7);
		}
		for (int l = 0; l < this.transformRotationTargets.Length; l++)
		{
			float num8 = (this.transformRotationTargets[l].UseSmoothedLoudness ? this.frameSmoothedLoudness : this.frameLoudness) * this.transformRotationTargets[l].Scale;
			this.transformRotationTargets[l].transform.localRotation = Quaternion.Slerp(this.transformRotationTargets[l].Initial, this.transformRotationTargets[l].Max, num8);
		}
		for (int m = 0; m < this.particleTargets.Length; m++)
		{
			float num9 = (this.particleTargets[m].UseSmoothedLoudness ? this.frameSmoothedLoudness : this.frameLoudness) * this.particleTargets[m].Scale;
			this.particleTargets[m].Main.startSpeedMultiplier = this.particleTargets[m].InitialSpeed * this.particleTargets[m].speed.Evaluate(num9);
			this.particleTargets[m].Main.startSizeMultiplier = this.particleTargets[m].InitialSize * this.particleTargets[m].size.Evaluate(num9);
			this.particleTargets[m].Emission.rateOverTimeMultiplier = this.particleTargets[m].InitialRate * this.particleTargets[m].rate.Evaluate(num9);
		}
		for (int n = 0; n < this.gameObjectEnableTargets.Length; n++)
		{
			bool flag = (this.gameObjectEnableTargets[n].UseSmoothedLoudness ? this.frameSmoothedLoudness : (this.frameLoudness * this.gameObjectEnableTargets[n].Scale)) >= this.gameObjectEnableTargets[n].Threshold;
			if (!this.gameObjectEnableTargets[n].TurnOnAtThreshhold)
			{
				flag = !flag;
			}
			if (this.gameObjectEnableTargets[n].GameObject.activeInHierarchy != flag)
			{
				this.gameObjectEnableTargets[n].GameObject.SetActive(flag);
			}
		}
		for (int num10 = 0; num10 < this.rendererColorTargets.Length; num10++)
		{
			VoiceLoudnessReactorRendererColorTarget voiceLoudnessReactorRendererColorTarget = this.rendererColorTargets[num10];
			float num11 = (voiceLoudnessReactorRendererColorTarget.useSmoothedLoudness ? this.frameSmoothedLoudness : (this.frameLoudness * voiceLoudnessReactorRendererColorTarget.scale));
			voiceLoudnessReactorRendererColorTarget.UpdateMaterialColor(num11);
		}
		for (int num12 = 0; num12 < this.animatorTargets.Length; num12++)
		{
			VoiceLoudnessReactorAnimatorTarget voiceLoudnessReactorAnimatorTarget = this.animatorTargets[num12];
			float num13 = (voiceLoudnessReactorAnimatorTarget.useSmoothedLoudness ? this.frameSmoothedLoudness : this.frameLoudness);
			if (voiceLoudnessReactorAnimatorTarget.animatorSpeedToLoudness < 0f)
			{
				voiceLoudnessReactorAnimatorTarget.animator.speed = Mathf.Max(0f, (1f - num13) * -voiceLoudnessReactorAnimatorTarget.animatorSpeedToLoudness);
			}
			else
			{
				voiceLoudnessReactorAnimatorTarget.animator.speed = Mathf.Max(0f, num13 * voiceLoudnessReactorAnimatorTarget.animatorSpeedToLoudness);
			}
		}
	}

	// Token: 0x04004260 RID: 16992
	private GorillaSpeakerLoudness loudness;

	// Token: 0x04004261 RID: 16993
	[SerializeField]
	private VoiceLoudnessReactorBlendShapeTarget[] blendShapeTargets = new VoiceLoudnessReactorBlendShapeTarget[0];

	// Token: 0x04004262 RID: 16994
	[SerializeField]
	private VoiceLoudnessReactorTransformTarget[] transformPositionTargets = new VoiceLoudnessReactorTransformTarget[0];

	// Token: 0x04004263 RID: 16995
	[SerializeField]
	private VoiceLoudnessReactorTransformRotationTarget[] transformRotationTargets = new VoiceLoudnessReactorTransformRotationTarget[0];

	// Token: 0x04004264 RID: 16996
	[SerializeField]
	private VoiceLoudnessReactorTransformTarget[] transformScaleTargets = new VoiceLoudnessReactorTransformTarget[0];

	// Token: 0x04004265 RID: 16997
	[SerializeField]
	private VoiceLoudnessReactorParticleSystemTarget[] particleTargets = new VoiceLoudnessReactorParticleSystemTarget[0];

	// Token: 0x04004266 RID: 16998
	[SerializeField]
	private VoiceLoudnessReactorGameObjectEnableTarget[] gameObjectEnableTargets = new VoiceLoudnessReactorGameObjectEnableTarget[0];

	// Token: 0x04004267 RID: 16999
	[SerializeField]
	private VoiceLoudnessReactorRendererColorTarget[] rendererColorTargets = new VoiceLoudnessReactorRendererColorTarget[0];

	// Token: 0x04004268 RID: 17000
	[SerializeField]
	private VoiceLoudnessReactorAnimatorTarget[] animatorTargets = new VoiceLoudnessReactorAnimatorTarget[0];

	// Token: 0x04004269 RID: 17001
	private float frameLoudness;

	// Token: 0x0400426A RID: 17002
	private float frameSmoothedLoudness;

	// Token: 0x0400426B RID: 17003
	[SerializeField]
	private float attack;

	// Token: 0x0400426C RID: 17004
	[SerializeField]
	private float decay;
}
