using System;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000B0E RID: 2830
	public class GorillaAmbushManager : GorillaTagManager
	{
		// Token: 0x06004582 RID: 17794 RVA: 0x0014A863 File Offset: 0x00148A63
		public override GameModeType GameType()
		{
			if (!this.isGhostTag)
			{
				return GameModeType.Ambush;
			}
			return GameModeType.Ghost;
		}

		// Token: 0x170006C7 RID: 1735
		// (get) Token: 0x06004583 RID: 17795 RVA: 0x0014A870 File Offset: 0x00148A70
		public static int HandEffectHash
		{
			get
			{
				return GorillaAmbushManager.handTapHash;
			}
		}

		// Token: 0x170006C8 RID: 1736
		// (get) Token: 0x06004584 RID: 17796 RVA: 0x0014A877 File Offset: 0x00148A77
		// (set) Token: 0x06004585 RID: 17797 RVA: 0x0014A87E File Offset: 0x00148A7E
		public static float HandFXScaleModifier { get; private set; }

		// Token: 0x170006C9 RID: 1737
		// (get) Token: 0x06004586 RID: 17798 RVA: 0x0014A886 File Offset: 0x00148A86
		// (set) Token: 0x06004587 RID: 17799 RVA: 0x0014A88E File Offset: 0x00148A8E
		public bool isGhostTag { get; private set; }

		// Token: 0x06004588 RID: 17800 RVA: 0x0014A897 File Offset: 0x00148A97
		public override void Awake()
		{
			base.Awake();
			if (this.handTapFX != null)
			{
				GorillaAmbushManager.handTapHash = PoolUtils.GameObjHashCode(this.handTapFX);
			}
			GorillaAmbushManager.HandFXScaleModifier = this.handTapScaleFactor;
		}

		// Token: 0x06004589 RID: 17801 RVA: 0x0014A8C8 File Offset: 0x00148AC8
		private void Start()
		{
			this.hasScryingPlane = this.scryingPlaneRef.TryResolve<MeshRenderer>(out this.scryingPlane);
			this.hasScryingPlane3p = this.scryingPlane3pRef.TryResolve<MeshRenderer>(out this.scryingPlane3p);
		}

		// Token: 0x0600458A RID: 17802 RVA: 0x0014A8F8 File Offset: 0x00148AF8
		public override string GameModeName()
		{
			if (!this.isGhostTag)
			{
				return "AMBUSH";
			}
			return "GHOST";
		}

		// Token: 0x0600458B RID: 17803 RVA: 0x0014A910 File Offset: 0x00148B10
		public override void UpdatePlayerAppearance(VRRig rig)
		{
			int num = this.MyMatIndex(rig.creator);
			rig.ChangeMaterialLocal(num);
			bool flag = base.IsInfected(rig.Creator);
			bool flag2 = base.IsInfected(NetworkSystem.Instance.LocalPlayer);
			rig.bodyRenderer.SetGameModeBodyType(flag ? GorillaBodyType.Skeleton : GorillaBodyType.Default);
			rig.SetInvisibleToLocalPlayer(flag && !flag2);
			if (this.isGhostTag && rig.isOfflineVRRig)
			{
				CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(flag);
				if (this.hasScryingPlane)
				{
					this.scryingPlane.enabled = flag2;
				}
				if (this.hasScryingPlane3p)
				{
					this.scryingPlane3p.enabled = flag2;
				}
			}
		}

		// Token: 0x0600458C RID: 17804 RVA: 0x0014A9B6 File Offset: 0x00148BB6
		public override int MyMatIndex(NetPlayer forPlayer)
		{
			if (!base.IsInfected(forPlayer))
			{
				return 0;
			}
			return 13;
		}

		// Token: 0x0600458D RID: 17805 RVA: 0x0014A9C8 File Offset: 0x00148BC8
		public override void StopPlaying()
		{
			base.StopPlaying();
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				GorillaSkin.ApplyToRig(vrrig, null, GorillaSkin.SkinType.gameMode);
				vrrig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Default);
				vrrig.SetInvisibleToLocalPlayer(false);
			}
			CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
			if (this.hasScryingPlane)
			{
				this.scryingPlane.enabled = false;
			}
			if (this.hasScryingPlane3p)
			{
				this.scryingPlane3p.enabled = false;
			}
		}

		// Token: 0x04004818 RID: 18456
		public GameObject handTapFX;

		// Token: 0x04004819 RID: 18457
		public GorillaSkin ambushSkin;

		// Token: 0x0400481A RID: 18458
		[SerializeField]
		private AudioClip[] firstPersonTaggedSounds;

		// Token: 0x0400481B RID: 18459
		[SerializeField]
		private float firstPersonTaggedSoundVolume;

		// Token: 0x0400481C RID: 18460
		private static int handTapHash = -1;

		// Token: 0x0400481D RID: 18461
		public float handTapScaleFactor = 0.5f;

		// Token: 0x0400481F RID: 18463
		public float crawlingSpeedForMaxVolume;

		// Token: 0x04004821 RID: 18465
		[SerializeField]
		private XSceneRef scryingPlaneRef;

		// Token: 0x04004822 RID: 18466
		[SerializeField]
		private XSceneRef scryingPlane3pRef;

		// Token: 0x04004823 RID: 18467
		private const int STEALTH_MATERIAL_INDEX = 13;

		// Token: 0x04004824 RID: 18468
		private MeshRenderer scryingPlane;

		// Token: 0x04004825 RID: 18469
		private bool hasScryingPlane;

		// Token: 0x04004826 RID: 18470
		private MeshRenderer scryingPlane3p;

		// Token: 0x04004827 RID: 18471
		private bool hasScryingPlane3p;
	}
}
