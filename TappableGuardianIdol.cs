using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;

// Token: 0x020006BC RID: 1724
[DisallowMultipleComponent]
public class TappableGuardianIdol : Tappable
{
	// Token: 0x17000432 RID: 1074
	// (get) Token: 0x06002B04 RID: 11012 RVA: 0x000D3B2F File Offset: 0x000D1D2F
	// (set) Token: 0x06002B05 RID: 11013 RVA: 0x000D3B37 File Offset: 0x000D1D37
	public bool isChangingPositions { get; private set; }

	// Token: 0x06002B06 RID: 11014 RVA: 0x000D3B40 File Offset: 0x000D1D40
	protected override void OnEnable()
	{
		base.OnEnable();
		this._colliderBaseRadius = this.tapCollision.radius;
	}

	// Token: 0x06002B07 RID: 11015 RVA: 0x000D3B59 File Offset: 0x000D1D59
	protected override void OnDisable()
	{
		base.OnDisable();
		this.isChangingPositions = false;
		this._activationState = -1;
		this.isActivationReady = true;
		this.tapCollision.radius = this._colliderBaseRadius;
	}

	// Token: 0x06002B08 RID: 11016 RVA: 0x000D3B87 File Offset: 0x000D1D87
	public void OnZoneActiveStateChanged(bool zoneActive)
	{
		GTDev.Log<string>(string.Format("OnZoneActiveStateChanged({0}->{1})", this._zoneIsActive, zoneActive), this, null);
		this._zoneIsActive = zoneActive;
		this.idolVisualRoot.SetActive(this._zoneIsActive);
	}

	// Token: 0x06002B09 RID: 11017 RVA: 0x000D3BC4 File Offset: 0x000D1DC4
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		if (info.Sender.IsLocal)
		{
			this.zoneManager.SetScaleCenterPoint(base.transform);
		}
		if (!this.isChangingPositions)
		{
			if (!this.zoneManager.IsZoneValid())
			{
				return;
			}
			RigContainer rigContainer;
			if (PhotonNetwork.LocalPlayer.IsMasterClient && VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
			{
				if (Vector3.Magnitude(rigContainer.Rig.transform.position - base.transform.position) > this.requiredTapDistance + Mathf.Epsilon)
				{
					return;
				}
				this.zoneManager.IdolWasTapped(info.Sender);
			}
			if (!this.zoneManager.IsPlayerGuardian(info.Sender))
			{
				this.tapFX.Play();
			}
		}
	}

	// Token: 0x06002B0A RID: 11018 RVA: 0x000D3C90 File Offset: 0x000D1E90
	public void SetPosition(Vector3 position)
	{
		base.transform.position = position + new Vector3(0f, this.activeHeight, 0f);
		this.UpdateStageActivatedObjects();
		this._audio.GTPlayOneShot(this._activateSound, this._audio.volume);
		base.StartCoroutine(this.<SetPosition>g__Unshrink|49_0());
	}

	// Token: 0x06002B0B RID: 11019 RVA: 0x000D3CF2 File Offset: 0x000D1EF2
	public void MovePositions(Vector3 finalPosition)
	{
		if (this.isChangingPositions)
		{
			return;
		}
		this.transitionPos = finalPosition + this.fallStartOffset;
		this.finalPos = finalPosition;
		base.StartCoroutine(this.TransitionToNextIdol());
	}

	// Token: 0x06002B0C RID: 11020 RVA: 0x000D3D24 File Offset: 0x000D1F24
	public void UpdateActivationProgress(float rawProgress, bool progressing)
	{
		this.isActivationReady = !progressing;
		if (rawProgress <= 0f && !progressing)
		{
			if (this._activationState >= 0)
			{
				if (this._activationRoutine != null)
				{
					base.StopCoroutine(this._activationRoutine);
					this._activationRoutine = null;
				}
				this.idolMeshRoot.transform.localScale = Vector3.one;
			}
			this._activationState = -1;
			this.UpdateStageActivatedObjects();
			this._audio.GTStop();
			return;
		}
		int num = (int)rawProgress;
		progressing &= this._activationStageSounds.Length > num;
		if (this._activationState == num || !progressing)
		{
			return;
		}
		if (this._activationRoutine != null)
		{
			base.StopCoroutine(this._activationRoutine);
		}
		this._activationRoutine = base.StartCoroutine(this.ShowActivationEffect());
		this._activationState = num;
		this.UpdateStageActivatedObjects();
		TappableGuardianIdol.IdolActivationSound idolActivationSound = this._activationStageSounds[num];
		this._audio.GTPlayOneShot(idolActivationSound.activation, this._audio.volume);
		this._audio.clip = idolActivationSound.loop;
		this._audio.loop = true;
		this._audio.GTPlay();
	}

	// Token: 0x06002B0D RID: 11021 RVA: 0x000D3E3B File Offset: 0x000D203B
	public void StartLookingAround()
	{
		if (this._lookRoutine != null)
		{
			base.StopCoroutine(this._lookRoutine);
		}
		this._lookRoutine = base.StartCoroutine(this.DoLookingAround());
	}

	// Token: 0x06002B0E RID: 11022 RVA: 0x000D3E63 File Offset: 0x000D2063
	public void StopLookingAround()
	{
		if (this._lookRoutine == null)
		{
			return;
		}
		base.StopCoroutine(this._lookRoutine);
		this._lookRoot.localRotation = Quaternion.identity;
		this._lookRoutine = null;
	}

	// Token: 0x06002B0F RID: 11023 RVA: 0x000D3E91 File Offset: 0x000D2091
	private IEnumerator DoLookingAround()
	{
		TappableGuardianIdol.<>c__DisplayClass54_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.nextLookTime = Time.time;
		CS$<>8__locals1._lookDirection = this._lookRoot.rotation;
		yield return null;
		for (;;)
		{
			if (Time.time >= CS$<>8__locals1.nextLookTime)
			{
				this.<DoLookingAround>g__PickLookTarget|54_0(ref CS$<>8__locals1);
			}
			this._lookRoot.rotation = Quaternion.Slerp(this._lookRoot.rotation, CS$<>8__locals1._lookDirection, Time.deltaTime * Mathf.Max(1f, (float)this._activationState * this._baseLookRate));
			yield return null;
		}
		yield break;
	}

	// Token: 0x06002B10 RID: 11024 RVA: 0x000D3EA0 File Offset: 0x000D20A0
	private void UpdateStageActivatedObjects()
	{
		foreach (TappableGuardianIdol.StageActivatedObject stageActivatedObject in this._stageActivatedObjects)
		{
			stageActivatedObject.UpdateActiveState(this._activationState);
		}
	}

	// Token: 0x06002B11 RID: 11025 RVA: 0x000D3ED7 File Offset: 0x000D20D7
	private IEnumerator ShowActivationEffect()
	{
		float bulgeDuration = 1f;
		float lerpVal = 0f;
		while (lerpVal < 1f)
		{
			lerpVal += Time.deltaTime / bulgeDuration;
			float num = Mathf.Lerp(1f, this.bulgeScale, this.bulgeCurve.Evaluate(lerpVal));
			this.idolMeshRoot.transform.localScale = Vector3.one * num;
			this.tapCollision.radius = this._colliderBaseRadius * num;
			yield return null;
		}
		this._activationRoutine = null;
		yield break;
	}

	// Token: 0x06002B12 RID: 11026 RVA: 0x000D3EE6 File Offset: 0x000D20E6
	private IEnumerator TransitionToNextIdol()
	{
		this.isChangingPositions = true;
		this._audio.GTStop();
		if (this.knockbackOnTrigger)
		{
			this.zoneManager.TriggerIdolKnockback();
		}
		if (this.explodeFX)
		{
			ObjectPools.instance.Instantiate(this.explodeFX, base.transform.position, true);
		}
		this.UpdateActivationProgress(-1f, false);
		this.idolMeshRoot.SetActive(false);
		this.tapCollision.enabled = false;
		base.transform.position = this.transitionPos;
		yield return new WaitForSeconds(this.floatDuration);
		this.idolMeshRoot.SetActive(true);
		this.tapCollision.enabled = true;
		if (this.startFallFX)
		{
			ObjectPools.instance.Instantiate(this.startFallFX, this.transitionPos, true);
		}
		this._audio.GTPlayOneShot(this._descentSound, 1f);
		this.trailFX.Play();
		float fall = 0f;
		Vector3 startPos = this.transitionPos;
		Vector3 destinationPos = this.finalPos;
		while (fall < this.fallDuration)
		{
			fall += Time.deltaTime;
			base.transform.position = Vector3.Lerp(startPos, destinationPos, fall / this.fallDuration);
			yield return null;
		}
		base.transform.position = destinationPos;
		this.trailFX.Stop();
		if (this.landedFX)
		{
			ObjectPools.instance.Instantiate(this.landedFX, destinationPos, true);
		}
		if (this.knockbackOnLand)
		{
			this.zoneManager.TriggerIdolKnockback();
		}
		yield return new WaitForSeconds(this.inactiveDuration);
		this._audio.GTPlayOneShot(this._activateSound, this._audio.volume);
		float activateLerp = 0f;
		startPos = this.finalPos;
		destinationPos = this.finalPos + new Vector3(0f, this.activeHeight, 0f);
		AnimationCurve animCurve = AnimationCurves.EaseInOutQuad;
		while (activateLerp < 1f)
		{
			activateLerp = Mathf.Clamp01(activateLerp + Time.deltaTime / this.activationDuration);
			base.transform.position = Vector3.Lerp(startPos, destinationPos, animCurve.Evaluate(activateLerp));
			yield return null;
		}
		if (this.activatedFX)
		{
			ObjectPools.instance.Instantiate(this.activatedFX, base.transform.position, true);
		}
		if (this.knockbackOnActivate)
		{
			this.zoneManager.TriggerIdolKnockback();
		}
		this.isChangingPositions = false;
		yield break;
	}

	// Token: 0x06002B13 RID: 11027 RVA: 0x000D3EF5 File Offset: 0x000D20F5
	private float EaseInOut(float input)
	{
		if (input >= 0.5f)
		{
			return 1f - Mathf.Pow(-2f * input + 2f, 3f) / 2f;
		}
		return 4f * input * input * input;
	}

	// Token: 0x06002B15 RID: 11029 RVA: 0x000D402C File Offset: 0x000D222C
	[CompilerGenerated]
	private IEnumerator <SetPosition>g__Unshrink|49_0()
	{
		float lerpVal = 0f;
		float growDuration = 0.5f;
		while (lerpVal < 1f)
		{
			lerpVal += Time.deltaTime / growDuration;
			float num = Mathf.Lerp(0f, 1f, AnimationCurves.EaseOutQuad.Evaluate(lerpVal));
			this.idolMeshRoot.transform.localScale = Vector3.one * num;
			this.tapCollision.radius = this._colliderBaseRadius * num;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06002B16 RID: 11030 RVA: 0x000D403C File Offset: 0x000D223C
	[CompilerGenerated]
	private void <DoLookingAround>g__PickLookTarget|54_0(ref TappableGuardianIdol.<>c__DisplayClass54_0 A_1)
	{
		Transform transform = this.<DoLookingAround>g__GetClosestPlayerPosition|54_2(ref A_1);
		A_1._lookDirection = (transform ? Quaternion.LookRotation(transform.position - this._lookRoot.position) : Quaternion.Euler((float)Random.Range(-15, 15), this._lookRoot.rotation.eulerAngles.y + (float)Random.Range(-45, 45), 0f));
		this.<DoLookingAround>g__SetLookTime|54_1(ref A_1);
	}

	// Token: 0x06002B17 RID: 11031 RVA: 0x000D40BA File Offset: 0x000D22BA
	[CompilerGenerated]
	private void <DoLookingAround>g__SetLookTime|54_1(ref TappableGuardianIdol.<>c__DisplayClass54_0 A_1)
	{
		A_1.nextLookTime = Time.time + this._lookInterval / (float)this._activationState * 0.5f + Random.value;
	}

	// Token: 0x06002B18 RID: 11032 RVA: 0x000D40E4 File Offset: 0x000D22E4
	[CompilerGenerated]
	private Transform <DoLookingAround>g__GetClosestPlayerPosition|54_2(ref TappableGuardianIdol.<>c__DisplayClass54_0 A_1)
	{
		if (Random.value < this._randomLookChance)
		{
			return null;
		}
		Vector3 position = base.transform.position;
		float num = float.MaxValue;
		Transform transform = null;
		foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
		{
			if (!(vrrig == null))
			{
				bool flag = vrrig.OwningNetPlayer == this.zoneManager.CurrentGuardian;
				float num2 = Vector3.SqrMagnitude(vrrig.transform.position - position) * (float)(flag ? 100 : 1);
				if (num2 < num)
				{
					num = num2;
					transform = vrrig.transform;
				}
			}
		}
		return transform;
	}

	// Token: 0x04002FFD RID: 12285
	[SerializeField]
	private GorillaGuardianZoneManager zoneManager;

	// Token: 0x04002FFE RID: 12286
	[SerializeField]
	private float floatDuration = 2f;

	// Token: 0x04002FFF RID: 12287
	[SerializeField]
	private float fallDuration = 1.5f;

	// Token: 0x04003000 RID: 12288
	[SerializeField]
	private float inactiveDuration = 2f;

	// Token: 0x04003001 RID: 12289
	[SerializeField]
	private float activationDuration = 1f;

	// Token: 0x04003002 RID: 12290
	[SerializeField]
	private float activeHeight = 1f;

	// Token: 0x04003003 RID: 12291
	[SerializeField]
	private bool knockbackOnTrigger;

	// Token: 0x04003004 RID: 12292
	[SerializeField]
	private bool knockbackOnLand = true;

	// Token: 0x04003005 RID: 12293
	[SerializeField]
	private bool knockbackOnActivate;

	// Token: 0x04003006 RID: 12294
	[SerializeField]
	private Vector3 fallStartOffset = new Vector3(3f, 20f, 3f);

	// Token: 0x04003007 RID: 12295
	[SerializeField]
	private ParticleSystem trailFX;

	// Token: 0x04003008 RID: 12296
	[SerializeField]
	private ParticleSystem tapFX;

	// Token: 0x04003009 RID: 12297
	[SerializeField]
	private GameObject explodeFX;

	// Token: 0x0400300A RID: 12298
	[SerializeField]
	private GameObject startFallFX;

	// Token: 0x0400300B RID: 12299
	[SerializeField]
	private GameObject landedFX;

	// Token: 0x0400300C RID: 12300
	[SerializeField]
	private GameObject activatedFX;

	// Token: 0x0400300D RID: 12301
	[SerializeField]
	private SphereCollider tapCollision;

	// Token: 0x0400300E RID: 12302
	[SerializeField]
	private GameObject idolVisualRoot;

	// Token: 0x0400300F RID: 12303
	[SerializeField]
	private GameObject idolMeshRoot;

	// Token: 0x04003010 RID: 12304
	[SerializeField]
	private AnimationCurve bulgeCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.5f, 1f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x04003011 RID: 12305
	[SerializeField]
	private float bulgeScale = 1.1f;

	// Token: 0x04003012 RID: 12306
	[SerializeField]
	private AudioSource _audio;

	// Token: 0x04003013 RID: 12307
	[SerializeField]
	private AudioClip[] _descentSound;

	// Token: 0x04003014 RID: 12308
	[SerializeField]
	private AudioClip[] _activateSound;

	// Token: 0x04003015 RID: 12309
	[SerializeField]
	private TappableGuardianIdol.IdolActivationSound[] _activationStageSounds;

	// Token: 0x04003016 RID: 12310
	[SerializeField]
	private TappableGuardianIdol.StageActivatedObject[] _stageActivatedObjects;

	// Token: 0x04003017 RID: 12311
	[Header("Look Around")]
	[SerializeField]
	private Transform _lookRoot;

	// Token: 0x04003018 RID: 12312
	[SerializeField]
	private float _lookInterval = 10f;

	// Token: 0x04003019 RID: 12313
	[SerializeField]
	private float _baseLookRate = 1f;

	// Token: 0x0400301A RID: 12314
	[SerializeField]
	private float _randomLookChance = 0.25f;

	// Token: 0x0400301B RID: 12315
	private Coroutine _lookRoutine;

	// Token: 0x0400301D RID: 12317
	private Vector3 transitionPos;

	// Token: 0x0400301E RID: 12318
	private Vector3 finalPos;

	// Token: 0x0400301F RID: 12319
	private int _activationState;

	// Token: 0x04003020 RID: 12320
	private Coroutine _activationRoutine;

	// Token: 0x04003021 RID: 12321
	private float _colliderBaseRadius;

	// Token: 0x04003022 RID: 12322
	private bool _zoneIsActive = true;

	// Token: 0x04003023 RID: 12323
	public bool isActivationReady;

	// Token: 0x04003024 RID: 12324
	private float requiredTapDistance = 3f;

	// Token: 0x020006BD RID: 1725
	[Serializable]
	public struct IdolActivationSound
	{
		// Token: 0x04003025 RID: 12325
		public AudioClip activation;

		// Token: 0x04003026 RID: 12326
		public AudioClip loop;
	}

	// Token: 0x020006BE RID: 1726
	[Serializable]
	public struct StageActivatedObject
	{
		// Token: 0x06002B19 RID: 11033 RVA: 0x000D41B0 File Offset: 0x000D23B0
		public void UpdateActiveState(int stage)
		{
			bool flag = stage >= this.min && stage <= this.max;
			GameObject[] array = this.objects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(flag);
			}
		}

		// Token: 0x04003027 RID: 12327
		public GameObject[] objects;

		// Token: 0x04003028 RID: 12328
		public int min;

		// Token: 0x04003029 RID: 12329
		public int max;
	}
}
