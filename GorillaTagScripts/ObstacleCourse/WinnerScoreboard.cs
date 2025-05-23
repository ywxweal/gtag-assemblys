using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000B4D RID: 2893
	public class WinnerScoreboard : MonoBehaviour
	{
		// Token: 0x0600475B RID: 18267 RVA: 0x00153258 File Offset: 0x00151458
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

		// Token: 0x040049AC RID: 18860
		public string raceStarted = "RACE STARTED!";

		// Token: 0x040049AD RID: 18861
		public string raceLoading = "RACE LOADING...";

		// Token: 0x040049AE RID: 18862
		[SerializeField]
		private TextMeshPro output;
	}
}
