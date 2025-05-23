using System;
using GorillaTagScripts.ModIO;
using ModIO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.UI.ModIO
{
	// Token: 0x02000B2D RID: 2861
	public class CustomMapsRoomMapDisplay : MonoBehaviour
	{
		// Token: 0x06004670 RID: 18032 RVA: 0x0014EC84 File Offset: 0x0014CE84
		public void Start()
		{
			this.loginToModioText.gameObject.SetActive(true);
			this.roomMapNameText.text = this.noRoomMapString;
			this.roomMapStatusText.text = this.notLoadedStatusString;
			this.roomMapLabelText.gameObject.SetActive(false);
			this.roomMapNameText.gameObject.SetActive(false);
			this.roomMapStatusLabelText.gameObject.SetActive(false);
			this.roomMapStatusText.gameObject.SetActive(false);
			NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnDisconnectedFromRoom;
			GameEvents.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
			GameEvents.OnModIOLoggedOut.AddListener(new UnityAction(this.OnModIOLoggedOut));
			CustomMapManager.OnRoomMapChanged.AddListener(new UnityAction<ModId>(this.OnRoomMapChanged));
			CustomMapManager.OnMapLoadStatusChanged.AddListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
			CustomMapManager.OnMapLoadComplete.AddListener(new UnityAction<bool>(this.OnMapLoadComplete));
		}

		// Token: 0x06004671 RID: 18033 RVA: 0x0014EDA4 File Offset: 0x0014CFA4
		public void OnDestroy()
		{
			NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnDisconnectedFromRoom;
			CustomMapManager.OnRoomMapChanged.RemoveListener(new UnityAction<ModId>(this.OnRoomMapChanged));
		}

		// Token: 0x06004672 RID: 18034 RVA: 0x0014EDF4 File Offset: 0x0014CFF4
		private void OnModIOLoggedOut()
		{
			this.roomMapLabelText.gameObject.SetActive(false);
			this.roomMapNameText.gameObject.SetActive(false);
			this.roomMapStatusText.gameObject.SetActive(false);
			this.roomMapStatusLabelText.gameObject.SetActive(false);
			this.loginToModioText.gameObject.SetActive(true);
		}

		// Token: 0x06004673 RID: 18035 RVA: 0x0014EE56 File Offset: 0x0014D056
		private void OnModIOLoggedIn()
		{
			this.loginToModioText.gameObject.SetActive(false);
			this.roomMapLabelText.gameObject.SetActive(true);
			this.roomMapNameText.gameObject.SetActive(true);
			this.UpdateRoomMap();
		}

		// Token: 0x06004674 RID: 18036 RVA: 0x0014EE91 File Offset: 0x0014D091
		private void OnJoinedRoom()
		{
			this.UpdateRoomMap();
		}

		// Token: 0x06004675 RID: 18037 RVA: 0x0014EE91 File Offset: 0x0014D091
		private void OnDisconnectedFromRoom()
		{
			this.UpdateRoomMap();
		}

		// Token: 0x06004676 RID: 18038 RVA: 0x0014EE91 File Offset: 0x0014D091
		private void OnRoomMapChanged(ModId roomMapModId)
		{
			this.UpdateRoomMap();
		}

		// Token: 0x06004677 RID: 18039 RVA: 0x0014EE9C File Offset: 0x0014D09C
		private void UpdateRoomMap()
		{
			if (!ModIOManager.IsLoggedIn())
			{
				return;
			}
			ModId currentRoomMap = CustomMapManager.GetRoomMapId();
			if (currentRoomMap == ModId.Null)
			{
				this.roomMapNameText.text = this.noRoomMapString;
				this.roomMapStatusLabelText.gameObject.SetActive(false);
				this.roomMapStatusText.gameObject.SetActive(false);
				return;
			}
			ModIOManager.GetModProfile(currentRoomMap, delegate(ModIORequestResultAnd<ModProfile> result)
			{
				if (!ModIOManager.IsLoggedIn())
				{
					return;
				}
				if (!result.result.success)
				{
					this.roomMapNameText.text = "Failed to retrieve mod info.";
					return;
				}
				this.roomMapNameText.text = result.data.name;
				this.roomMapStatusLabelText.gameObject.SetActive(true);
				if (CustomMapLoader.IsModLoaded(currentRoomMap.id))
				{
					this.roomMapStatusText.text = this.readyToPlayStatusString;
					this.roomMapStatusText.color = this.readyToPlayStatusStringColor;
				}
				else if (CustomMapManager.IsLoading(currentRoomMap.id))
				{
					this.roomMapStatusText.text = this.loadingStatusString;
					this.roomMapStatusText.color = this.loadingStatusStringColor;
				}
				else
				{
					this.roomMapStatusText.text = this.notLoadedStatusString;
					this.roomMapStatusText.color = this.notLoadedStatusStringColor;
				}
				this.roomMapStatusText.gameObject.SetActive(true);
			});
		}

		// Token: 0x06004678 RID: 18040 RVA: 0x0014EF28 File Offset: 0x0014D128
		private void OnMapLoadComplete(bool success)
		{
			if (!ModIOManager.IsLoggedIn())
			{
				return;
			}
			if (success)
			{
				this.roomMapStatusText.text = this.readyToPlayStatusString;
				this.roomMapStatusText.color = this.readyToPlayStatusStringColor;
				return;
			}
			this.roomMapStatusText.text = this.loadFailedStatusString;
			this.roomMapStatusText.color = this.loadFailedStatusStringColor;
		}

		// Token: 0x06004679 RID: 18041 RVA: 0x0014EF85 File Offset: 0x0014D185
		private void OnMapLoadProgress(MapLoadStatus status, int progress, string message)
		{
			if (!ModIOManager.IsLoggedIn())
			{
				return;
			}
			if (status - MapLoadStatus.Downloading <= 1)
			{
				this.roomMapStatusText.text = this.loadingStatusString;
				this.roomMapStatusText.color = this.loadingStatusStringColor;
			}
		}

		// Token: 0x040048FD RID: 18685
		[SerializeField]
		private TMP_Text roomMapLabelText;

		// Token: 0x040048FE RID: 18686
		[SerializeField]
		private TMP_Text roomMapNameText;

		// Token: 0x040048FF RID: 18687
		[SerializeField]
		private TMP_Text roomMapStatusLabelText;

		// Token: 0x04004900 RID: 18688
		[SerializeField]
		private TMP_Text roomMapStatusText;

		// Token: 0x04004901 RID: 18689
		[SerializeField]
		private TMP_Text loginToModioText;

		// Token: 0x04004902 RID: 18690
		[SerializeField]
		private string noRoomMapString = "NONE";

		// Token: 0x04004903 RID: 18691
		[SerializeField]
		private string notLoadedStatusString = "NOT LOADED";

		// Token: 0x04004904 RID: 18692
		[SerializeField]
		private string loadingStatusString = "LOADING...";

		// Token: 0x04004905 RID: 18693
		[SerializeField]
		private string readyToPlayStatusString = "READY!";

		// Token: 0x04004906 RID: 18694
		[SerializeField]
		private string loadFailedStatusString = "LOAD FAILED";

		// Token: 0x04004907 RID: 18695
		[SerializeField]
		private Color notLoadedStatusStringColor = Color.red;

		// Token: 0x04004908 RID: 18696
		[SerializeField]
		private Color loadingStatusStringColor = Color.yellow;

		// Token: 0x04004909 RID: 18697
		[SerializeField]
		private Color readyToPlayStatusStringColor = Color.green;

		// Token: 0x0400490A RID: 18698
		[SerializeField]
		private Color loadFailedStatusStringColor = Color.red;
	}
}
