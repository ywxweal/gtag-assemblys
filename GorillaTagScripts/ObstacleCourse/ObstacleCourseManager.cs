using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000B45 RID: 2885
	[NetworkBehaviourWeaved(9)]
	public class ObstacleCourseManager : NetworkComponent, ITickSystemTick
	{
		// Token: 0x170006E5 RID: 1765
		// (get) Token: 0x06004729 RID: 18217 RVA: 0x00152BDA File Offset: 0x00150DDA
		// (set) Token: 0x0600472A RID: 18218 RVA: 0x00152BE1 File Offset: 0x00150DE1
		public static ObstacleCourseManager Instance { get; private set; }

		// Token: 0x170006E6 RID: 1766
		// (get) Token: 0x0600472B RID: 18219 RVA: 0x00152BE9 File Offset: 0x00150DE9
		// (set) Token: 0x0600472C RID: 18220 RVA: 0x00152BF1 File Offset: 0x00150DF1
		public bool TickRunning { get; set; }

		// Token: 0x0600472D RID: 18221 RVA: 0x00152BFA File Offset: 0x00150DFA
		protected override void Awake()
		{
			base.Awake();
			ObstacleCourseManager.Instance = this;
		}

		// Token: 0x0600472E RID: 18222 RVA: 0x00152C08 File Offset: 0x00150E08
		internal override void OnEnable()
		{
			NetworkBehaviourUtils.InternalOnEnable(this);
			base.OnEnable();
			TickSystem<object>.AddCallbackTarget(this);
		}

		// Token: 0x0600472F RID: 18223 RVA: 0x00152C1C File Offset: 0x00150E1C
		internal override void OnDisable()
		{
			NetworkBehaviourUtils.InternalOnDisable(this);
			base.OnEnable();
			TickSystem<object>.RemoveCallbackTarget(this);
		}

		// Token: 0x06004730 RID: 18224 RVA: 0x00152C30 File Offset: 0x00150E30
		public void Tick()
		{
			foreach (ObstacleCourse obstacleCourse in this.allObstaclesCourses)
			{
				obstacleCourse.InvokeUpdate();
			}
		}

		// Token: 0x06004731 RID: 18225 RVA: 0x00152C80 File Offset: 0x00150E80
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			this.allObstaclesCourses.Clear();
		}

		// Token: 0x170006E7 RID: 1767
		// (get) Token: 0x06004732 RID: 18226 RVA: 0x00152C93 File Offset: 0x00150E93
		// (set) Token: 0x06004733 RID: 18227 RVA: 0x00152CBD File Offset: 0x00150EBD
		[Networked]
		[NetworkedWeaved(0, 9)]
		public unsafe ObstacleCourseData Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ObstacleCourseManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(ObstacleCourseData*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ObstacleCourseManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(ObstacleCourseData*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x06004734 RID: 18228 RVA: 0x00152CE8 File Offset: 0x00150EE8
		public override void WriteDataFusion()
		{
			this.Data = new ObstacleCourseData(this.allObstaclesCourses);
		}

		// Token: 0x06004735 RID: 18229 RVA: 0x00152CFC File Offset: 0x00150EFC
		public override void ReadDataFusion()
		{
			for (int i = 0; i < this.Data.ObstacleCourseCount; i++)
			{
				int num = this.Data.WinnerActorNumber[i];
				ObstacleCourse.RaceState raceState = (ObstacleCourse.RaceState)this.Data.CurrentRaceState[i];
				if (this.allObstaclesCourses[i].currentState != raceState)
				{
					this.allObstaclesCourses[i].Deserialize(num, raceState);
				}
			}
		}

		// Token: 0x06004736 RID: 18230 RVA: 0x00152D7C File Offset: 0x00150F7C
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			stream.SendNext(this.allObstaclesCourses.Count);
			for (int i = 0; i < this.allObstaclesCourses.Count; i++)
			{
				stream.SendNext(this.allObstaclesCourses[i].winnerActorNumber);
				stream.SendNext(this.allObstaclesCourses[i].currentState);
			}
		}

		// Token: 0x06004737 RID: 18231 RVA: 0x00152DFC File Offset: 0x00150FFC
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			int num = (int)stream.ReceiveNext();
			for (int i = 0; i < num; i++)
			{
				int num2 = (int)stream.ReceiveNext();
				ObstacleCourse.RaceState raceState = (ObstacleCourse.RaceState)stream.ReceiveNext();
				if (this.allObstaclesCourses[i].currentState != raceState)
				{
					this.allObstaclesCourses[i].Deserialize(num2, raceState);
				}
			}
		}

		// Token: 0x06004739 RID: 18233 RVA: 0x00152E81 File Offset: 0x00151081
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x0600473A RID: 18234 RVA: 0x00152E99 File Offset: 0x00151099
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x0400499F RID: 18847
		public List<ObstacleCourse> allObstaclesCourses = new List<ObstacleCourse>();

		// Token: 0x040049A1 RID: 18849
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 9)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private ObstacleCourseData _Data;
	}
}
