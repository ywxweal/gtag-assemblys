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
		// (get) Token: 0x0600472A RID: 18218 RVA: 0x00152CB2 File Offset: 0x00150EB2
		// (set) Token: 0x0600472B RID: 18219 RVA: 0x00152CB9 File Offset: 0x00150EB9
		public static ObstacleCourseManager Instance { get; private set; }

		// Token: 0x170006E6 RID: 1766
		// (get) Token: 0x0600472C RID: 18220 RVA: 0x00152CC1 File Offset: 0x00150EC1
		// (set) Token: 0x0600472D RID: 18221 RVA: 0x00152CC9 File Offset: 0x00150EC9
		public bool TickRunning { get; set; }

		// Token: 0x0600472E RID: 18222 RVA: 0x00152CD2 File Offset: 0x00150ED2
		protected override void Awake()
		{
			base.Awake();
			ObstacleCourseManager.Instance = this;
		}

		// Token: 0x0600472F RID: 18223 RVA: 0x00152CE0 File Offset: 0x00150EE0
		internal override void OnEnable()
		{
			NetworkBehaviourUtils.InternalOnEnable(this);
			base.OnEnable();
			TickSystem<object>.AddCallbackTarget(this);
		}

		// Token: 0x06004730 RID: 18224 RVA: 0x00152CF4 File Offset: 0x00150EF4
		internal override void OnDisable()
		{
			NetworkBehaviourUtils.InternalOnDisable(this);
			base.OnEnable();
			TickSystem<object>.RemoveCallbackTarget(this);
		}

		// Token: 0x06004731 RID: 18225 RVA: 0x00152D08 File Offset: 0x00150F08
		public void Tick()
		{
			foreach (ObstacleCourse obstacleCourse in this.allObstaclesCourses)
			{
				obstacleCourse.InvokeUpdate();
			}
		}

		// Token: 0x06004732 RID: 18226 RVA: 0x00152D58 File Offset: 0x00150F58
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			this.allObstaclesCourses.Clear();
		}

		// Token: 0x170006E7 RID: 1767
		// (get) Token: 0x06004733 RID: 18227 RVA: 0x00152D6B File Offset: 0x00150F6B
		// (set) Token: 0x06004734 RID: 18228 RVA: 0x00152D95 File Offset: 0x00150F95
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

		// Token: 0x06004735 RID: 18229 RVA: 0x00152DC0 File Offset: 0x00150FC0
		public override void WriteDataFusion()
		{
			this.Data = new ObstacleCourseData(this.allObstaclesCourses);
		}

		// Token: 0x06004736 RID: 18230 RVA: 0x00152DD4 File Offset: 0x00150FD4
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

		// Token: 0x06004737 RID: 18231 RVA: 0x00152E54 File Offset: 0x00151054
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

		// Token: 0x06004738 RID: 18232 RVA: 0x00152ED4 File Offset: 0x001510D4
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

		// Token: 0x0600473A RID: 18234 RVA: 0x00152F59 File Offset: 0x00151159
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x0600473B RID: 18235 RVA: 0x00152F71 File Offset: 0x00151171
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x040049A0 RID: 18848
		public List<ObstacleCourse> allObstaclesCourses = new List<ObstacleCourse>();

		// Token: 0x040049A2 RID: 18850
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 9)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private ObstacleCourseData _Data;
	}
}
