using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000AC3 RID: 2755
	public class Mole : Tappable
	{
		// Token: 0x14000079 RID: 121
		// (add) Token: 0x0600427D RID: 17021 RVA: 0x00133240 File Offset: 0x00131440
		// (remove) Token: 0x0600427E RID: 17022 RVA: 0x00133278 File Offset: 0x00131478
		public event Mole.MoleTapEvent OnTapped;

		// Token: 0x170006A0 RID: 1696
		// (get) Token: 0x0600427F RID: 17023 RVA: 0x001332AD File Offset: 0x001314AD
		// (set) Token: 0x06004280 RID: 17024 RVA: 0x001332B5 File Offset: 0x001314B5
		public bool IsLeftSideMole { get; set; }

		// Token: 0x06004281 RID: 17025 RVA: 0x001332C0 File Offset: 0x001314C0
		private void Awake()
		{
			this.currentState = Mole.MoleState.Hidden;
			Vector3 position = base.transform.position;
			this.origin = (this.target = position);
			this.visiblePosition = new Vector3(position.x, position.y + this.positionOffset, position.z);
			this.hiddenPosition = new Vector3(position.x, position.y - this.positionOffset, position.z);
			this.travelTime = this.normalTravelTime;
			this.animCurve = (this.normalAnimCurve = AnimationCurves.EaseInOutQuad);
			this.hitAnimCurve = AnimationCurves.EaseOutBack;
			for (int i = 0; i < this.moleTypes.Length; i++)
			{
				if (this.moleTypes[i].isHazard)
				{
					this.hazardMoles.Add(i);
				}
				else
				{
					this.safeMoles.Add(i);
				}
			}
			this.randomMolePickedIndex = -1;
		}

		// Token: 0x06004282 RID: 17026 RVA: 0x001333A8 File Offset: 0x001315A8
		public void InvokeUpdate()
		{
			if (this.currentState == Mole.MoleState.Ready)
			{
				return;
			}
			switch (this.currentState)
			{
			case Mole.MoleState.Reset:
			case Mole.MoleState.Hidden:
				this.currentState = Mole.MoleState.Ready;
				break;
			case Mole.MoleState.TransitionToVisible:
			case Mole.MoleState.TransitionToHidden:
			{
				float num = this.animCurve.Evaluate(Mathf.Clamp01((Time.time - this.animStartTime) / this.travelTime));
				base.transform.position = Vector3.Lerp(this.origin, this.target, num);
				if (num >= 1f)
				{
					this.currentState++;
				}
				break;
			}
			}
			if (Time.time - this.currentTime >= this.showMoleDuration && this.currentState > Mole.MoleState.Ready && this.currentState < Mole.MoleState.TransitionToHidden)
			{
				this.HideMole(false);
			}
		}

		// Token: 0x06004283 RID: 17027 RVA: 0x00133473 File Offset: 0x00131673
		public bool CanPickMole()
		{
			return this.currentState == Mole.MoleState.Ready;
		}

		// Token: 0x06004284 RID: 17028 RVA: 0x00133480 File Offset: 0x00131680
		public void ShowMole(float _showMoleDuration, int randomMoleTypeIndex)
		{
			if (randomMoleTypeIndex >= this.moleTypes.Length || randomMoleTypeIndex < 0)
			{
				return;
			}
			this.randomMolePickedIndex = randomMoleTypeIndex;
			for (int i = 0; i < this.moleTypes.Length; i++)
			{
				this.moleTypes[i].gameObject.SetActive(i == randomMoleTypeIndex);
				if (this.moleTypes[i].monkeMoleDefaultMaterial != null)
				{
					this.moleTypes[i].MeshRenderer.material = this.moleTypes[i].monkeMoleDefaultMaterial;
				}
			}
			this.showMoleDuration = _showMoleDuration;
			this.origin = base.transform.position;
			this.target = this.visiblePosition;
			this.animCurve = this.normalAnimCurve;
			this.currentState = Mole.MoleState.TransitionToVisible;
			this.animStartTime = (this.currentTime = Time.time);
			this.travelTime = this.normalTravelTime;
		}

		// Token: 0x06004285 RID: 17029 RVA: 0x00133558 File Offset: 0x00131758
		public void HideMole(bool isHit = false)
		{
			if (this.currentState < Mole.MoleState.TransitionToVisible || this.currentState > Mole.MoleState.Visible)
			{
				return;
			}
			this.origin = base.transform.position;
			this.target = this.hiddenPosition;
			this.animCurve = (isHit ? this.hitAnimCurve : this.normalAnimCurve);
			this.animStartTime = Time.time;
			this.travelTime = (isHit ? this.hitTravelTime : this.normalTravelTime);
			this.currentState = Mole.MoleState.TransitionToHidden;
		}

		// Token: 0x06004286 RID: 17030 RVA: 0x001335D8 File Offset: 0x001317D8
		public bool CanTap()
		{
			Mole.MoleState moleState = this.currentState;
			return moleState == Mole.MoleState.TransitionToVisible || moleState == Mole.MoleState.Visible;
		}

		// Token: 0x06004287 RID: 17031 RVA: 0x001335FD File Offset: 0x001317FD
		public override bool CanTap(bool isLeftHand)
		{
			return this.CanTap();
		}

		// Token: 0x06004288 RID: 17032 RVA: 0x00133608 File Offset: 0x00131808
		public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
		{
			if (!this.CanTap())
			{
				return;
			}
			bool flag = info.Sender.ActorNumber == NetworkSystem.Instance.LocalPlayerID;
			bool flag2 = flag && GorillaTagger.Instance.lastLeftTap >= GorillaTagger.Instance.lastRightTap;
			MoleTypes moleTypes = null;
			if (this.randomMolePickedIndex >= 0 && this.randomMolePickedIndex < this.moleTypes.Length)
			{
				moleTypes = this.moleTypes[this.randomMolePickedIndex];
			}
			if (moleTypes != null)
			{
				Mole.MoleTapEvent onTapped = this.OnTapped;
				if (onTapped == null)
				{
					return;
				}
				onTapped(moleTypes, base.transform.position, flag, flag2);
			}
		}

		// Token: 0x06004289 RID: 17033 RVA: 0x001336A7 File Offset: 0x001318A7
		public void ResetPosition()
		{
			base.transform.position = this.hiddenPosition;
			this.currentState = Mole.MoleState.Reset;
		}

		// Token: 0x0600428A RID: 17034 RVA: 0x001336C1 File Offset: 0x001318C1
		public int GetMoleTypeIndex(bool useHazardMole)
		{
			if (!useHazardMole)
			{
				return this.safeMoles[Random.Range(0, this.safeMoles.Count)];
			}
			return this.hazardMoles[Random.Range(0, this.hazardMoles.Count)];
		}

		// Token: 0x040044DA RID: 17626
		public float positionOffset = 0.2f;

		// Token: 0x040044DB RID: 17627
		public MoleTypes[] moleTypes;

		// Token: 0x040044DC RID: 17628
		private float showMoleDuration;

		// Token: 0x040044DD RID: 17629
		private Vector3 visiblePosition;

		// Token: 0x040044DE RID: 17630
		private Vector3 hiddenPosition;

		// Token: 0x040044DF RID: 17631
		private float currentTime;

		// Token: 0x040044E0 RID: 17632
		private float animStartTime;

		// Token: 0x040044E1 RID: 17633
		private float travelTime;

		// Token: 0x040044E2 RID: 17634
		private float normalTravelTime = 0.3f;

		// Token: 0x040044E3 RID: 17635
		private float hitTravelTime = 0.2f;

		// Token: 0x040044E4 RID: 17636
		private AnimationCurve animCurve;

		// Token: 0x040044E5 RID: 17637
		private AnimationCurve normalAnimCurve;

		// Token: 0x040044E6 RID: 17638
		private AnimationCurve hitAnimCurve;

		// Token: 0x040044E7 RID: 17639
		private Mole.MoleState currentState;

		// Token: 0x040044E8 RID: 17640
		private Vector3 origin;

		// Token: 0x040044E9 RID: 17641
		private Vector3 target;

		// Token: 0x040044EA RID: 17642
		private int randomMolePickedIndex;

		// Token: 0x040044EC RID: 17644
		public CallLimiter rpcCooldown;

		// Token: 0x040044ED RID: 17645
		private int moleScore;

		// Token: 0x040044EE RID: 17646
		private List<int> safeMoles = new List<int>();

		// Token: 0x040044EF RID: 17647
		private List<int> hazardMoles = new List<int>();

		// Token: 0x02000AC4 RID: 2756
		// (Invoke) Token: 0x0600428D RID: 17037
		public delegate void MoleTapEvent(MoleTypes moleType, Vector3 position, bool isLocalTap, bool isLeft);

		// Token: 0x02000AC5 RID: 2757
		public enum MoleState
		{
			// Token: 0x040044F2 RID: 17650
			Reset,
			// Token: 0x040044F3 RID: 17651
			Ready,
			// Token: 0x040044F4 RID: 17652
			TransitionToVisible,
			// Token: 0x040044F5 RID: 17653
			Visible,
			// Token: 0x040044F6 RID: 17654
			TransitionToHidden,
			// Token: 0x040044F7 RID: 17655
			Hidden
		}
	}
}
