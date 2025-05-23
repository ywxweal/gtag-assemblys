using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000B4D RID: 2893
	public class WinnerScoreboard : MonoBehaviour
	{
		// Token: 0x0600475C RID: 18268 RVA: 0x00153330 File Offset: 0x00151530
		public void UpdateBoard(string winner, ObstacleCourse.RaceState _currentState)
		{
			if (this.output == null)
			{
				return;
			}
			switch (_currentState)
			{
			case ObstacleCourse.RaceState.Started:
				Debug.Log(this.raceStarted);
				this.output.text = this.raceStarted;
				return;
			case ObstacleCourse.RaceState.Waiting:
				Debug.Log(this.raceLoading);
				this.output.text = this.raceLoading;
				return;
			case ObstacleCourse.RaceState.Finished:
				Debug.Log(winner + " WON!!");
				this.output.text = winner + " WON!!";
				return;
			default:
				return;
			}
		}

		// Token: 0x040049AD RID: 18861
		public string raceStarted = "RACE STARTED!";

		// Token: 0x040049AE RID: 18862
		public string raceLoading = "RACE LOADING...";

		// Token: 0x040049AF RID: 18863
		[SerializeField]
		private TextMeshPro output;
	}
}
