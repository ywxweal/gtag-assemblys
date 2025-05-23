using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Valve.VR;

namespace UnityEngine.XR.Interaction.Toolkit
{
	// Token: 0x02000BA0 RID: 2976
	public class GorillaSnapTurn : LocomotionProvider
	{
		// Token: 0x17000714 RID: 1812
		// (get) Token: 0x060049C1 RID: 18881 RVA: 0x001603AB File Offset: 0x0015E5AB
		// (set) Token: 0x060049C2 RID: 18882 RVA: 0x001603B3 File Offset: 0x0015E5B3
		public GorillaSnapTurn.InputAxes turnUsage
		{
			get
			{
				return this.m_TurnUsage;
			}
			set
			{
				this.m_TurnUsage = value;
			}
		}

		// Token: 0x17000715 RID: 1813
		// (get) Token: 0x060049C3 RID: 18883 RVA: 0x001603BC File Offset: 0x0015E5BC
		// (set) Token: 0x060049C4 RID: 18884 RVA: 0x001603C4 File Offset: 0x0015E5C4
		public List<XRController> controllers
		{
			get
			{
				return this.m_Controllers;
			}
			set
			{
				this.m_Controllers = value;
			}
		}

		// Token: 0x17000716 RID: 1814
		// (get) Token: 0x060049C5 RID: 18885 RVA: 0x001603CD File Offset: 0x0015E5CD
		// (set) Token: 0x060049C6 RID: 18886 RVA: 0x001603D5 File Offset: 0x0015E5D5
		public float turnAmount
		{
			get
			{
				return this.m_TurnAmount;
			}
			set
			{
				this.m_TurnAmount = value;
			}
		}

		// Token: 0x17000717 RID: 1815
		// (get) Token: 0x060049C7 RID: 18887 RVA: 0x001603DE File Offset: 0x0015E5DE
		// (set) Token: 0x060049C8 RID: 18888 RVA: 0x001603E6 File Offset: 0x0015E5E6
		public float debounceTime
		{
			get
			{
				return this.m_DebounceTime;
			}
			set
			{
				this.m_DebounceTime = value;
			}
		}

		// Token: 0x17000718 RID: 1816
		// (get) Token: 0x060049C9 RID: 18889 RVA: 0x001603EF File Offset: 0x0015E5EF
		// (set) Token: 0x060049CA RID: 18890 RVA: 0x001603F7 File Offset: 0x0015E5F7
		public float deadZone
		{
			get
			{
				return this.m_DeadZone;
			}
			set
			{
				this.m_DeadZone = value;
			}
		}

		// Token: 0x17000719 RID: 1817
		// (get) Token: 0x060049CB RID: 18891 RVA: 0x00160400 File Offset: 0x0015E600
		// (set) Token: 0x060049CC RID: 18892 RVA: 0x00160408 File Offset: 0x0015E608
		public string turnType
		{
			get
			{
				return this.m_TurnType;
			}
			private set
			{
				this.m_TurnType = value;
			}
		}

		// Token: 0x1700071A RID: 1818
		// (get) Token: 0x060049CD RID: 18893 RVA: 0x00160411 File Offset: 0x0015E611
		// (set) Token: 0x060049CE RID: 18894 RVA: 0x00160419 File Offset: 0x0015E619
		public int turnFactor
		{
			get
			{
				return this.m_TurnFactor;
			}
			private set
			{
				this.m_TurnFactor = value;
			}
		}

		// Token: 0x1700071B RID: 1819
		// (get) Token: 0x060049CF RID: 18895 RVA: 0x00160422 File Offset: 0x0015E622
		public static GorillaSnapTurn CachedSnapTurnRef
		{
			get
			{
				if (GorillaSnapTurn._cachedReference == null)
				{
					Debug.LogError("[SNAP_TURN] Tried accessing static cached reference, but was still null. Trying to find component in scene");
					GorillaSnapTurn._cachedReference = Object.FindObjectOfType<GorillaSnapTurn>();
				}
				return GorillaSnapTurn._cachedReference;
			}
		}

		// Token: 0x060049D0 RID: 18896 RVA: 0x0016044A File Offset: 0x0015E64A
		protected override void Awake()
		{
			base.Awake();
			if (GorillaSnapTurn._cachedReference != null)
			{
				Debug.LogError("[SNAP_TURN] A [GorillaSnapTurn] component already exists in the scene");
				return;
			}
			GorillaSnapTurn._cachedReference = this;
		}

		// Token: 0x060049D1 RID: 18897 RVA: 0x00160470 File Offset: 0x0015E670
		private void Update()
		{
			this.ValidateTurningOverriders();
			if (this.m_Controllers.Count > 0)
			{
				this.EnsureControllerDataListSize();
				InputFeatureUsage<Vector2>[] vec2UsageList = GorillaSnapTurn.m_Vec2UsageList;
				GorillaSnapTurn.InputAxes turnUsage = this.m_TurnUsage;
				for (int i = 0; i < this.m_Controllers.Count; i++)
				{
					XRController xrcontroller = this.m_Controllers[i];
					if (xrcontroller != null && xrcontroller.enableInputActions)
					{
						InputDevice inputDevice = xrcontroller.inputDevice;
						Vector2 vector = ((xrcontroller.controllerNode == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightJoystick2DAxis.GetAxis(SteamVR_Input_Sources.RightHand));
						if (vector.x > this.deadZone)
						{
							this.StartTurn(this.m_TurnAmount);
						}
						else if (vector.x < -this.deadZone)
						{
							this.StartTurn(-this.m_TurnAmount);
						}
						else
						{
							this.m_AxisReset = true;
						}
					}
				}
			}
			if (Math.Abs(this.m_CurrentTurnAmount) > 0f && base.BeginLocomotion())
			{
				if (base.system.xrRig != null)
				{
					GTPlayer.Instance.Turn(this.m_CurrentTurnAmount);
				}
				this.m_CurrentTurnAmount = 0f;
				base.EndLocomotion();
			}
		}

		// Token: 0x060049D2 RID: 18898 RVA: 0x001605A0 File Offset: 0x0015E7A0
		private void EnsureControllerDataListSize()
		{
			if (this.m_Controllers.Count != this.m_ControllersWereActive.Count)
			{
				while (this.m_ControllersWereActive.Count < this.m_Controllers.Count)
				{
					this.m_ControllersWereActive.Add(false);
				}
				while (this.m_ControllersWereActive.Count < this.m_Controllers.Count)
				{
					this.m_ControllersWereActive.RemoveAt(this.m_ControllersWereActive.Count - 1);
				}
			}
		}

		// Token: 0x060049D3 RID: 18899 RVA: 0x0016061D File Offset: 0x0015E81D
		internal void FakeStartTurn(bool isLeft)
		{
			this.StartTurn(isLeft ? (-this.m_TurnAmount) : this.m_TurnAmount);
		}

		// Token: 0x060049D4 RID: 18900 RVA: 0x00160638 File Offset: 0x0015E838
		private void StartTurn(float amount)
		{
			if (this.m_TimeStarted + this.m_DebounceTime > Time.time && !this.m_AxisReset)
			{
				return;
			}
			if (!base.CanBeginLocomotion())
			{
				return;
			}
			if (this.turningOverriders.Count > 0)
			{
				return;
			}
			this.m_TimeStarted = Time.time;
			this.m_CurrentTurnAmount = amount;
			this.m_AxisReset = false;
		}

		// Token: 0x060049D5 RID: 18901 RVA: 0x00160694 File Offset: 0x0015E894
		public void ChangeTurnMode(string turnMode, int turnSpeedFactor)
		{
			this.turnType = turnMode;
			this.turnFactor = turnSpeedFactor;
			if (turnMode == "SNAP")
			{
				this.m_DebounceTime = 0.5f;
				this.m_TurnAmount = 60f * this.ConvertedTurnFactor((float)turnSpeedFactor);
				return;
			}
			if (!(turnMode == "SMOOTH"))
			{
				this.m_DebounceTime = 0f;
				this.m_TurnAmount = 0f;
				return;
			}
			this.m_DebounceTime = 0f;
			this.m_TurnAmount = 360f * Time.fixedDeltaTime * this.ConvertedTurnFactor((float)turnSpeedFactor);
		}

		// Token: 0x060049D6 RID: 18902 RVA: 0x00160727 File Offset: 0x0015E927
		public float ConvertedTurnFactor(float newTurnSpeed)
		{
			return Mathf.Max(0.75f, 0.5f + newTurnSpeed / 10f * 1.5f);
		}

		// Token: 0x060049D7 RID: 18903 RVA: 0x00160746 File Offset: 0x0015E946
		public void SetTurningOverride(ISnapTurnOverride caller)
		{
			if (!this.turningOverriders.Contains(caller))
			{
				this.turningOverriders.Add(caller);
			}
		}

		// Token: 0x060049D8 RID: 18904 RVA: 0x00160763 File Offset: 0x0015E963
		public void UnsetTurningOverride(ISnapTurnOverride caller)
		{
			if (this.turningOverriders.Contains(caller))
			{
				this.turningOverriders.Remove(caller);
			}
		}

		// Token: 0x060049D9 RID: 18905 RVA: 0x00160780 File Offset: 0x0015E980
		public void ValidateTurningOverriders()
		{
			foreach (ISnapTurnOverride snapTurnOverride in this.turningOverriders)
			{
				if (snapTurnOverride == null || !snapTurnOverride.TurnOverrideActive())
				{
					this.turningOverriders.Remove(snapTurnOverride);
				}
			}
		}

		// Token: 0x060049DA RID: 18906 RVA: 0x001607E4 File Offset: 0x0015E9E4
		public static void DisableSnapTurn()
		{
			Debug.Log("[SNAP_TURN] Disabling Snap Turn");
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				return;
			}
			GorillaSnapTurn._cachedTurnFactor = PlayerPrefs.GetInt("turnFactor");
			GorillaSnapTurn._cachedTurnType = PlayerPrefs.GetString("stickTurning");
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode("NONE", 0);
		}

		// Token: 0x060049DB RID: 18907 RVA: 0x00160837 File Offset: 0x0015EA37
		public static void UpdateAndSaveTurnType(string mode)
		{
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				Debug.LogError("[SNAP_TURN] Failed to Update, [CachedSnapTurnRef] is NULL");
				return;
			}
			PlayerPrefs.SetString("stickTurning", mode);
			PlayerPrefs.Save();
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode(mode, GorillaSnapTurn.CachedSnapTurnRef.turnFactor);
		}

		// Token: 0x060049DC RID: 18908 RVA: 0x00160876 File Offset: 0x0015EA76
		public static void UpdateAndSaveTurnFactor(int factor)
		{
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				Debug.LogError("[SNAP_TURN] Failed to Update, [CachedSnapTurnRef] is NULL");
				return;
			}
			PlayerPrefs.SetInt("turnFactor", factor);
			PlayerPrefs.Save();
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode(GorillaSnapTurn.CachedSnapTurnRef.turnType, factor);
		}

		// Token: 0x060049DD RID: 18909 RVA: 0x001608B8 File Offset: 0x0015EAB8
		public static void LoadSettingsFromPlayerPrefs()
		{
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				return;
			}
			string text = ((Application.platform == RuntimePlatform.Android) ? "NONE" : "SNAP");
			string @string = PlayerPrefs.GetString("stickTurning", text);
			int @int = PlayerPrefs.GetInt("turnFactor", 4);
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode(@string, @int);
		}

		// Token: 0x060049DE RID: 18910 RVA: 0x00160910 File Offset: 0x0015EB10
		public static void LoadSettingsFromCache()
		{
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(GorillaSnapTurn._cachedTurnType))
			{
				GorillaSnapTurn._cachedTurnType = ((Application.platform == RuntimePlatform.Android) ? "NONE" : "SNAP");
			}
			string cachedTurnType = GorillaSnapTurn._cachedTurnType;
			int cachedTurnFactor = GorillaSnapTurn._cachedTurnFactor;
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode(cachedTurnType, cachedTurnFactor);
		}

		// Token: 0x04004C95 RID: 19605
		private static readonly InputFeatureUsage<Vector2>[] m_Vec2UsageList = new InputFeatureUsage<Vector2>[]
		{
			CommonUsages.primary2DAxis,
			CommonUsages.secondary2DAxis
		};

		// Token: 0x04004C96 RID: 19606
		[SerializeField]
		[Tooltip("The 2D Input Axis on the primary devices that will be used to trigger a snap turn.")]
		private GorillaSnapTurn.InputAxes m_TurnUsage;

		// Token: 0x04004C97 RID: 19607
		[SerializeField]
		[Tooltip("A list of controllers that allow Snap Turn.  If an XRController is not enabled, or does not have input actions enabled.  Snap Turn will not work.")]
		private List<XRController> m_Controllers = new List<XRController>();

		// Token: 0x04004C98 RID: 19608
		[SerializeField]
		[Tooltip("The number of degrees clockwise to rotate when snap turning clockwise.")]
		private float m_TurnAmount = 45f;

		// Token: 0x04004C99 RID: 19609
		[SerializeField]
		[Tooltip("The amount of time that the system will wait before starting another snap turn.")]
		private float m_DebounceTime = 0.5f;

		// Token: 0x04004C9A RID: 19610
		[SerializeField]
		[Tooltip("The deadzone that the controller movement will have to be above to trigger a snap turn.")]
		private float m_DeadZone = 0.75f;

		// Token: 0x04004C9B RID: 19611
		private float m_CurrentTurnAmount;

		// Token: 0x04004C9C RID: 19612
		private float m_TimeStarted;

		// Token: 0x04004C9D RID: 19613
		private bool m_AxisReset;

		// Token: 0x04004C9E RID: 19614
		public float turnSpeed = 1f;

		// Token: 0x04004C9F RID: 19615
		private HashSet<ISnapTurnOverride> turningOverriders = new HashSet<ISnapTurnOverride>();

		// Token: 0x04004CA0 RID: 19616
		private List<bool> m_ControllersWereActive = new List<bool>();

		// Token: 0x04004CA1 RID: 19617
		private static int _cachedTurnFactor;

		// Token: 0x04004CA2 RID: 19618
		private static string _cachedTurnType;

		// Token: 0x04004CA3 RID: 19619
		private string m_TurnType = "";

		// Token: 0x04004CA4 RID: 19620
		private int m_TurnFactor = 1;

		// Token: 0x04004CA5 RID: 19621
		[OnEnterPlay_SetNull]
		private static GorillaSnapTurn _cachedReference;

		// Token: 0x02000BA1 RID: 2977
		public enum InputAxes
		{
			// Token: 0x04004CA7 RID: 19623
			Primary2DAxis,
			// Token: 0x04004CA8 RID: 19624
			Secondary2DAxis
		}
	}
}
