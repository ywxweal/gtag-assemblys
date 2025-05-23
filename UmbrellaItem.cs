using System;
using GorillaTag;
using UnityEngine;

// Token: 0x0200042F RID: 1071
public class UmbrellaItem : TransferrableObject
{
	// Token: 0x06001A6D RID: 6765 RVA: 0x00081CCB File Offset: 0x0007FECB
	protected override void Start()
	{
		base.Start();
		this.itemState = TransferrableObject.ItemStates.State1;
	}

	// Token: 0x06001A6E RID: 6766 RVA: 0x00081CDC File Offset: 0x0007FEDC
	public override void OnActivate()
	{
		base.OnActivate();
		float num = GorillaTagger.Instance.tapHapticStrength / 4f;
		float fixedDeltaTime = Time.fixedDeltaTime;
		float num2 = 0.08f;
		int num3;
		if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			num3 = this.SoundIdOpen;
			this.itemState = TransferrableObject.ItemStates.State0;
			BetterDayNightManager.instance.collidersToAddToWeatherSystems.Add(this.umbrellaRainDestroyTrigger);
		}
		else
		{
			num3 = this.SoundIdClose;
			this.itemState = TransferrableObject.ItemStates.State1;
			BetterDayNightManager.instance.collidersToAddToWeatherSystems.Remove(this.umbrellaRainDestroyTrigger);
		}
		base.ActivateItemFX(num, fixedDeltaTime, num3, num2);
		this.OnUmbrellaStateChanged();
	}

	// Token: 0x06001A6F RID: 6767 RVA: 0x00081D74 File Offset: 0x0007FF74
	internal override void OnEnable()
	{
		base.OnEnable();
		this.OnUmbrellaStateChanged();
	}

	// Token: 0x06001A70 RID: 6768 RVA: 0x00081D82 File Offset: 0x0007FF82
	internal override void OnDisable()
	{
		base.OnDisable();
		BetterDayNightManager.instance.collidersToAddToWeatherSystems.Remove(this.umbrellaRainDestroyTrigger);
	}

	// Token: 0x06001A71 RID: 6769 RVA: 0x00081DA2 File Offset: 0x0007FFA2
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		BetterDayNightManager.instance.collidersToAddToWeatherSystems.Remove(this.umbrellaRainDestroyTrigger);
		this.itemState = TransferrableObject.ItemStates.State1;
		this.OnUmbrellaStateChanged();
	}

	// Token: 0x06001A72 RID: 6770 RVA: 0x00081DCF File Offset: 0x0007FFCF
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (base.InHand())
		{
			return false;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			this.OnActivate();
		}
		return true;
	}

	// Token: 0x06001A73 RID: 6771 RVA: 0x00081DF8 File Offset: 0x0007FFF8
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		UmbrellaItem.UmbrellaStates itemState = (UmbrellaItem.UmbrellaStates)this.itemState;
		if (itemState != this.previousUmbrellaState)
		{
			this.OnUmbrellaStateChanged();
		}
		this.UpdateAngles((itemState == UmbrellaItem.UmbrellaStates.UmbrellaOpen) ? this.startingAngles : this.endingAngles, this.lerpValue);
		this.previousUmbrellaState = itemState;
	}

	// Token: 0x06001A74 RID: 6772 RVA: 0x00081E48 File Offset: 0x00080048
	protected virtual void OnUmbrellaStateChanged()
	{
		bool flag = this.itemState == TransferrableObject.ItemStates.State0;
		GameObject[] array = this.gameObjectsActivatedOnOpen;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(flag);
		}
		ParticleSystem[] array2;
		if (flag)
		{
			array2 = this.particlesEmitOnOpen;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Play();
			}
			return;
		}
		array2 = this.particlesEmitOnOpen;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Stop();
		}
	}

	// Token: 0x06001A75 RID: 6773 RVA: 0x00081EBC File Offset: 0x000800BC
	protected virtual void UpdateAngles(Quaternion[] toAngles, float t)
	{
		for (int i = 0; i < this.umbrellaBones.Length; i++)
		{
			this.umbrellaBones[i].localRotation = Quaternion.Lerp(this.umbrellaBones[i].localRotation, toAngles[i], t);
		}
	}

	// Token: 0x06001A76 RID: 6774 RVA: 0x00081F04 File Offset: 0x00080104
	protected void GenerateAngles()
	{
		this.startingAngles = new Quaternion[this.umbrellaBones.Length];
		for (int i = 0; i < this.endingAngles.Length; i++)
		{
			this.startingAngles[i] = this.umbrellaToCopy.startingAngles[i];
		}
		this.endingAngles = new Quaternion[this.umbrellaBones.Length];
		for (int j = 0; j < this.endingAngles.Length; j++)
		{
			this.endingAngles[j] = this.umbrellaToCopy.endingAngles[j];
		}
	}

	// Token: 0x06001A77 RID: 6775 RVA: 0x00047642 File Offset: 0x00045842
	public override bool CanActivate()
	{
		return true;
	}

	// Token: 0x06001A78 RID: 6776 RVA: 0x00047642 File Offset: 0x00045842
	public override bool CanDeactivate()
	{
		return true;
	}

	// Token: 0x04001D83 RID: 7555
	[AssignInCorePrefab]
	public Transform[] umbrellaBones;

	// Token: 0x04001D84 RID: 7556
	[AssignInCorePrefab]
	public Quaternion[] startingAngles;

	// Token: 0x04001D85 RID: 7557
	[AssignInCorePrefab]
	public Quaternion[] endingAngles;

	// Token: 0x04001D86 RID: 7558
	[AssignInCorePrefab]
	[Tooltip("Assign to use the 'Generate Angles' button")]
	private UmbrellaItem umbrellaToCopy;

	// Token: 0x04001D87 RID: 7559
	[AssignInCorePrefab]
	public float lerpValue = 0.25f;

	// Token: 0x04001D88 RID: 7560
	[AssignInCorePrefab]
	public Collider umbrellaRainDestroyTrigger;

	// Token: 0x04001D89 RID: 7561
	[AssignInCorePrefab]
	public GameObject[] gameObjectsActivatedOnOpen;

	// Token: 0x04001D8A RID: 7562
	[AssignInCorePrefab]
	public ParticleSystem[] particlesEmitOnOpen;

	// Token: 0x04001D8B RID: 7563
	[GorillaSoundLookup]
	public int SoundIdOpen = 64;

	// Token: 0x04001D8C RID: 7564
	[GorillaSoundLookup]
	public int SoundIdClose = 65;

	// Token: 0x04001D8D RID: 7565
	private UmbrellaItem.UmbrellaStates previousUmbrellaState = UmbrellaItem.UmbrellaStates.UmbrellaOpen;

	// Token: 0x02000430 RID: 1072
	private enum UmbrellaStates
	{
		// Token: 0x04001D8F RID: 7567
		UmbrellaOpen = 1,
		// Token: 0x04001D90 RID: 7568
		UmbrellaClosed
	}
}
