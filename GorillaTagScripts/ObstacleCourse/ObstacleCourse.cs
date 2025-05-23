using System;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000B43 RID: 2883
	public class ObstacleCourse : MonoBehaviour
	{
		// Token: 0x170006E4 RID: 1764
		// (get) Token: 0x0600471A RID: 18202 RVA: 0x00152890 File Offset: 0x00150A90
		// (set) Token: 0x0600471B RID: 18203 RVA: 0x00152898 File Offset: 0x00150A98
		public int winnerActorNumber { get; private set; }

		// Token: 0x0600471C RID: 18204 RVA: 0x001528A4 File Offset: 0x00150AA4
		private void Awake()
		{
			this.numPlayersOnCourse = 0;
			for (int i = 0; i < this.zoneTriggers.Length; i++)
			{
				ObstacleCourseZoneTrigger obstacleCourseZoneTrigger = this.zoneTriggers[i];
				if (!(obstacleCourseZoneTrigger == null))
				{
					obstacleCourseZoneTrigger.OnPlayerTriggerEnter += this.OnPlayerEnterZone;
					obstacleCourseZoneTrigger.OnPlayerTriggerExit += this.OnPlayerExitZone;
				}
			}
			this.TappableBell.OnTapped += this.OnEndLineTrigger;
		}

		// Token: 0x0600471D RID: 18205 RVA: 0x00152918 File Offset: 0x00150B18
		private void OnDestroy()
		{
			for (int i = 0; i < this.zoneTriggers.Length; i++)
			{
				ObstacleCourseZoneTrigger obstacleCourseZoneTrigger = this.zoneTriggers[i];
				if (!(obstacleCourseZoneTrigger == null))
				{
					obstacleCourseZoneTrigger.OnPlayerTriggerEnter -= this.OnPlayerEnterZone;
					obstacleCourseZoneTrigger.OnPlayerTriggerExit -= this.OnPlayerExitZone;
				}
			}
			this.TappableBell.OnTapped -= this.OnEndLineTrigger;
		}

		// Token: 0x0600471E RID: 18206 RVA: 0x00152985 File Offset: 0x00150B85
		private void Start()
		{
			this.RestartTimer(false);
		}

		// Token: 0x0600471F RID: 18207 RVA: 0x00152990 File Offset: 0x00150B90
		public void InvokeUpdate()
		{
			foreach (ZoneBasedObject zoneBasedObject in this.zoneBasedVisuals)
			{
				if (zoneBasedObject != null)
				{
					zoneBasedObject.gameObject.SetActive(zoneBasedObject.IsLocalPlayerInZone());
				}
			}
			if (NetworkSystem.Instance.InRoom && ObstacleCourseManager.Instance.IsMine && this.currentState == ObstacleCourse.RaceState.Finished && Time.time - this.startTime >= this.cooldownTime)
			{
				this.RestartTimer(true);
			}
		}

		// Token: 0x06004720 RID: 18208 RVA: 0x00152A0C File Offset: 0x00150C0C
		public void OnPlayerEnterZone(Collider other)
		{
			if (ObstacleCourseManager.Instance.IsMine)
			{
				this.numPlayersOnCourse++;
			}
		}

		// Token: 0x06004721 RID: 18209 RVA: 0x00152A28 File Offset: 0x00150C28
		public void OnPlayerExitZone(Collider other)
		{
			if (ObstacleCourseManager.Instance.IsMine)
			{
				this.numPlayersOnCourse--;
			}
		}

		// Token: 0x06004722 RID: 18210 RVA: 0x00152A44 File Offset: 0x00150C44
		private void RestartTimer(bool playFx = true)
		{
			this.UpdateState(ObstacleCourse.RaceState.Started, playFx);
		}

		// Token: 0x06004723 RID: 18211 RVA: 0x00152A4E File Offset: 0x00150C4E
		private void EndRace()
		{
			this.UpdateState(ObstacleCourse.RaceState.Finished, true);
			this.startTime = Time.time;
		}

		// Token: 0x06004724 RID: 18212 RVA: 0x00152A64 File Offset: 0x00150C64
		public void PlayWinningEffects()
		{
			if (this.confettiParticle)
			{
				this.confettiParticle.Play();
			}
			if (this.bannerRenderer)
			{
				UberShaderProperty baseColor = UberShader.BaseColor;
				Material material = this.bannerRenderer.material;
				RigContainer rigContainer = this.winnerRig;
				baseColor.SetValue<Color?>(material, (rigContainer != null) ? new Color?(rigContainer.Rig.playerColor) : null);
			}
			this.audioSource.GTPlay();
		}

		// Token: 0x06004725 RID: 18213 RVA: 0x00152ADA File Offset: 0x00150CDA
		public void OnEndLineTrigger(VRRig rig)
		{
			if (ObstacleCourseManager.Instance.IsMine && this.currentState == ObstacleCourse.RaceState.Started)
			{
				this.winnerActorNumber = rig.creator.ActorNumber;
				this.winnerRig = rig.rigContainer;
				this.EndRace();
			}
		}

		// Token: 0x06004726 RID: 18214 RVA: 0x00152B13 File Offset: 0x00150D13
		public void Deserialize(int _winnerActorNumber, ObstacleCourse.RaceState _currentState)
		{
			if (!ObstacleCourseManager.Instance.IsMine)
			{
				this.winnerActorNumber = _winnerActorNumber;
				VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(this.winnerActorNumber), out this.winnerRig);
				this.UpdateState(_currentState, true);
			}
		}

		// Token: 0x06004727 RID: 18215 RVA: 0x00152B54 File Offset: 0x00150D54
		private void UpdateState(ObstacleCourse.RaceState state, bool playFX = true)
		{
			this.currentState = state;
			WinnerScoreboard winnerScoreboard = this.scoreboard;
			RigContainer rigContainer = this.winnerRig;
			winnerScoreboard.UpdateBoard((rigContainer != null) ? rigContainer.Rig.playerNameVisible : null, this.currentState);
			if (this.currentState == ObstacleCourse.RaceState.Finished)
			{
				this.PlayWinningEffects();
			}
			else if (this.currentState == ObstacleCourse.RaceState.Started && this.bannerRenderer)
			{
				UberShader.BaseColor.SetValue<Color>(this.bannerRenderer.material, Color.white);
			}
			this.UpdateStartingGate();
		}

		// Token: 0x06004728 RID: 18216 RVA: 0x00152BD8 File Offset: 0x00150DD8
		private void UpdateStartingGate()
		{
			if (this.currentState == ObstacleCourse.RaceState.Finished)
			{
				this.leftGate.transform.RotateAround(this.leftGate.transform.position, Vector3.up, 90f);
				this.rightGate.transform.RotateAround(this.rightGate.transform.position, Vector3.up, -90f);
				return;
			}
			if (this.currentState == ObstacleCourse.RaceState.Started)
			{
				this.leftGate.transform.RotateAround(this.leftGate.transform.position, Vector3.up, -90f);
				this.rightGate.transform.RotateAround(this.rightGate.transform.position, Vector3.up, 90f);
			}
		}

		// Token: 0x0400498C RID: 18828
		public WinnerScoreboard scoreboard;

		// Token: 0x0400498E RID: 18830
		private RigContainer winnerRig;

		// Token: 0x0400498F RID: 18831
		public ObstacleCourseZoneTrigger[] zoneTriggers;

		// Token: 0x04004990 RID: 18832
		[HideInInspector]
		public ObstacleCourse.RaceState currentState;

		// Token: 0x04004991 RID: 18833
		[SerializeField]
		private ParticleSystem confettiParticle;

		// Token: 0x04004992 RID: 18834
		[SerializeField]
		private Renderer bannerRenderer;

		// Token: 0x04004993 RID: 18835
		[SerializeField]
		private TappableBell TappableBell;

		// Token: 0x04004994 RID: 18836
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04004995 RID: 18837
		[SerializeField]
		private float cooldownTime = 20f;

		// Token: 0x04004996 RID: 18838
		[SerializeField]
		private ZoneBasedObject[] zoneBasedVisuals;

		// Token: 0x04004997 RID: 18839
		public GameObject leftGate;

		// Token: 0x04004998 RID: 18840
		public GameObject rightGate;

		// Token: 0x04004999 RID: 18841
		private int numPlayersOnCourse;

		// Token: 0x0400499A RID: 18842
		private float startTime;

		// Token: 0x02000B44 RID: 2884
		public enum RaceState
		{
			// Token: 0x0400499C RID: 18844
			Started,
			// Token: 0x0400499D RID: 18845
			Waiting,
			// Token: 0x0400499E RID: 18846
			Finished
		}
	}
}
