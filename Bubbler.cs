using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200046B RID: 1131
public class Bubbler : TransferrableObject
{
	// Token: 0x06001BCC RID: 7116 RVA: 0x000886B4 File Offset: 0x000868B4
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.hasParticleSystem = this.bubbleParticleSystem != null;
		if (this.hasParticleSystem)
		{
			this.bubbleParticleArray = new ParticleSystem.Particle[this.bubbleParticleSystem.main.maxParticles];
			this.bubbleParticleSystem.trigger.SetCollider(0, GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<SphereCollider>());
			this.bubbleParticleSystem.trigger.SetCollider(1, GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<SphereCollider>());
		}
		this.initialTriggerDuration = 0.05f;
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06001BCD RID: 7117 RVA: 0x00088758 File Offset: 0x00086958
	internal override void OnEnable()
	{
		base.OnEnable();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.hasBubblerAudio = this.bubblerAudio != null && this.bubblerAudio.clip != null;
		this.hasPopBubbleAudio = this.popBubbleAudio != null && this.popBubbleAudio.clip != null;
		this.hasFan = this.fan != null;
		this.hasActiveOnlyComponent = this.gameObjectActiveOnlyWhileTriggerDown != null;
	}

	// Token: 0x06001BCE RID: 7118 RVA: 0x000887E8 File Offset: 0x000869E8
	private void InitToDefault()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.hasParticleSystem && this.bubbleParticleSystem.isPlaying)
		{
			this.bubbleParticleSystem.Stop();
		}
		if (this.hasBubblerAudio && this.bubblerAudio.isPlaying)
		{
			this.bubblerAudio.GTStop();
		}
	}

	// Token: 0x06001BCF RID: 7119 RVA: 0x0008883C File Offset: 0x00086A3C
	internal override void OnDisable()
	{
		base.OnDisable();
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.hasParticleSystem && this.bubbleParticleSystem.isPlaying)
		{
			this.bubbleParticleSystem.Stop();
		}
		if (this.hasBubblerAudio && this.bubblerAudio.isPlaying)
		{
			this.bubblerAudio.GTStop();
		}
		this.currentParticles.Clear();
		this.particleInfoDict.Clear();
	}

	// Token: 0x06001BD0 RID: 7120 RVA: 0x000888AC File Offset: 0x00086AAC
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x06001BD1 RID: 7121 RVA: 0x000888BA File Offset: 0x00086ABA
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!this._worksInWater && GTPlayer.Instance.InWater)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
		}
	}

	// Token: 0x06001BD2 RID: 7122 RVA: 0x000888E0 File Offset: 0x00086AE0
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (!this.IsMyItem() && base.myOnlineRig != null && base.myOnlineRig.muted)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
		}
		bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
		bool flag2 = this.itemState != TransferrableObject.ItemStates.State0;
		Behaviour[] array = this.behavioursToEnableWhenTriggerPressed;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = flag2;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			if (this.hasParticleSystem && this.bubbleParticleSystem.isPlaying)
			{
				this.bubbleParticleSystem.Stop();
			}
			if (this.hasBubblerAudio && this.bubblerAudio.isPlaying)
			{
				this.bubblerAudio.GTStop();
			}
			if (this.hasActiveOnlyComponent)
			{
				this.gameObjectActiveOnlyWhileTriggerDown.SetActive(false);
			}
		}
		else
		{
			if (this.hasParticleSystem && !this.bubbleParticleSystem.isEmitting)
			{
				this.bubbleParticleSystem.Play();
			}
			if (this.hasBubblerAudio && !this.bubblerAudio.isPlaying)
			{
				this.bubblerAudio.GTPlay();
			}
			if (this.hasActiveOnlyComponent && !this.gameObjectActiveOnlyWhileTriggerDown.activeSelf)
			{
				this.gameObjectActiveOnlyWhileTriggerDown.SetActive(true);
			}
			if (this.IsMyItem())
			{
				this.initialTriggerPull = Time.time;
				GorillaTagger.Instance.StartVibration(flag, this.triggerStrength, this.initialTriggerDuration);
				if (Time.time > this.initialTriggerPull + this.initialTriggerDuration)
				{
					GorillaTagger.Instance.StartVibration(flag, this.ongoingStrength, Time.deltaTime);
				}
			}
			if (this.hasFan)
			{
				if (!this.fanYaxisinstead)
				{
					float num = this.fan.transform.localEulerAngles.z + this.rotationSpeed * Time.fixedDeltaTime;
					this.fan.transform.localEulerAngles = new Vector3(0f, 0f, num);
				}
				else
				{
					float num2 = this.fan.transform.localEulerAngles.y + this.rotationSpeed * Time.fixedDeltaTime;
					this.fan.transform.localEulerAngles = new Vector3(0f, num2, 0f);
				}
			}
		}
		if (this.hasParticleSystem && (!this.allBubblesPopped || this.itemState == TransferrableObject.ItemStates.State1))
		{
			int particles = this.bubbleParticleSystem.GetParticles(this.bubbleParticleArray);
			this.allBubblesPopped = particles <= 0;
			if (!this.allBubblesPopped)
			{
				for (int j = 0; j < particles; j++)
				{
					if (this.currentParticles.Contains(this.bubbleParticleArray[j].randomSeed))
					{
						this.currentParticles.Remove(this.bubbleParticleArray[j].randomSeed);
					}
				}
				foreach (uint num3 in this.currentParticles)
				{
					if (this.particleInfoDict.TryGetValue(num3, out this.outPosition))
					{
						if (this.hasPopBubbleAudio)
						{
							GTAudioSourceExtensions.GTPlayClipAtPoint(this.popBubbleAudio.clip, this.outPosition);
						}
						this.particleInfoDict.Remove(num3);
					}
				}
				this.currentParticles.Clear();
				for (int k = 0; k < particles; k++)
				{
					if (this.particleInfoDict.TryGetValue(this.bubbleParticleArray[k].randomSeed, out this.outPosition))
					{
						this.particleInfoDict[this.bubbleParticleArray[k].randomSeed] = this.bubbleParticleArray[k].position;
					}
					else
					{
						this.particleInfoDict.Add(this.bubbleParticleArray[k].randomSeed, this.bubbleParticleArray[k].position);
					}
					this.currentParticles.Add(this.bubbleParticleArray[k].randomSeed);
				}
			}
		}
	}

	// Token: 0x06001BD3 RID: 7123 RVA: 0x00088CEC File Offset: 0x00086EEC
	public override void OnActivate()
	{
		base.OnActivate();
		this.itemState = TransferrableObject.ItemStates.State1;
	}

	// Token: 0x06001BD4 RID: 7124 RVA: 0x00023C14 File Offset: 0x00021E14
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06001BD5 RID: 7125 RVA: 0x00088CFB File Offset: 0x00086EFB
	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	// Token: 0x06001BD6 RID: 7126 RVA: 0x00088D06 File Offset: 0x00086F06
	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	// Token: 0x04001ED6 RID: 7894
	[SerializeField]
	private bool _worksInWater = true;

	// Token: 0x04001ED7 RID: 7895
	public ParticleSystem bubbleParticleSystem;

	// Token: 0x04001ED8 RID: 7896
	private ParticleSystem.Particle[] bubbleParticleArray;

	// Token: 0x04001ED9 RID: 7897
	public AudioSource bubblerAudio;

	// Token: 0x04001EDA RID: 7898
	public AudioSource popBubbleAudio;

	// Token: 0x04001EDB RID: 7899
	private List<uint> currentParticles = new List<uint>();

	// Token: 0x04001EDC RID: 7900
	private Dictionary<uint, Vector3> particleInfoDict = new Dictionary<uint, Vector3>();

	// Token: 0x04001EDD RID: 7901
	private Vector3 outPosition;

	// Token: 0x04001EDE RID: 7902
	private bool allBubblesPopped;

	// Token: 0x04001EDF RID: 7903
	public bool disableActivation;

	// Token: 0x04001EE0 RID: 7904
	public bool disableDeactivation;

	// Token: 0x04001EE1 RID: 7905
	public float rotationSpeed = 5f;

	// Token: 0x04001EE2 RID: 7906
	public GameObject fan;

	// Token: 0x04001EE3 RID: 7907
	public bool fanYaxisinstead;

	// Token: 0x04001EE4 RID: 7908
	public float ongoingStrength = 0.005f;

	// Token: 0x04001EE5 RID: 7909
	public float triggerStrength = 0.2f;

	// Token: 0x04001EE6 RID: 7910
	private float initialTriggerPull;

	// Token: 0x04001EE7 RID: 7911
	private float initialTriggerDuration;

	// Token: 0x04001EE8 RID: 7912
	private bool hasBubblerAudio;

	// Token: 0x04001EE9 RID: 7913
	private bool hasPopBubbleAudio;

	// Token: 0x04001EEA RID: 7914
	public GameObject gameObjectActiveOnlyWhileTriggerDown;

	// Token: 0x04001EEB RID: 7915
	public Behaviour[] behavioursToEnableWhenTriggerPressed;

	// Token: 0x04001EEC RID: 7916
	private bool hasParticleSystem;

	// Token: 0x04001EED RID: 7917
	private bool hasFan;

	// Token: 0x04001EEE RID: 7918
	private bool hasActiveOnlyComponent;

	// Token: 0x0200046C RID: 1132
	private enum BubblerState
	{
		// Token: 0x04001EF0 RID: 7920
		None = 1,
		// Token: 0x04001EF1 RID: 7921
		Bubbling
	}
}
