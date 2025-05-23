using System;
using System.Collections.Generic;
using GorillaNetworking;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005CF RID: 1487
public class GRUIEmployeeTerminal : MonoBehaviour
{
	// Token: 0x06002444 RID: 9284 RVA: 0x000B6624 File Offset: 0x000B4824
	public void Setup()
	{
		this.signupButton.onPressButton.AddListener(new UnityAction(this.OnSignup));
		global::PlayFab.ClientModels.GetUserDataRequest getUserDataRequest = new global::PlayFab.ClientModels.GetUserDataRequest();
		getUserDataRequest.PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		getUserDataRequest.Keys = new List<string> { "GRData" };
		this.isSigningUp = true;
		PlayFabClientAPI.GetUserData(getUserDataRequest, new Action<GetUserDataResult>(this.OnGetUserDataInitialState), new Action<PlayFabError>(this.OnGetUserDataInitialStateFail), null, null);
		this.Refresh();
	}

	// Token: 0x06002445 RID: 9285 RVA: 0x000B66A8 File Offset: 0x000B48A8
	public void OnSignup()
	{
		if (this.isSigningUp || this.isEmployee)
		{
			return;
		}
		UpdateUserDataRequest updateUserDataRequest = new UpdateUserDataRequest
		{
			Data = new Dictionary<string, string> { { "GRData", "Now we have data" } }
		};
		if (!PlayFabClientAPI.IsClientLoggedIn())
		{
			if (PlayFabAuthenticator.instance != null)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
			}
			return;
		}
		this.isSigningUp = true;
		PlayFabClientAPI.UpdateUserData(updateUserDataRequest, new Action<UpdateUserDataResult>(this.OnSaveTableSuccess), new Action<PlayFabError>(this.OnSaveTableFailure), null, null);
	}

	// Token: 0x06002446 RID: 9286 RVA: 0x000B6731 File Offset: 0x000B4931
	public Transform GetSpawnMarker()
	{
		return this.spawnMarker;
	}

	// Token: 0x06002447 RID: 9287 RVA: 0x000B673C File Offset: 0x000B493C
	public void Refresh()
	{
		if (this.isSigningUp)
		{
			this.signupButtonText.text = "APPLYING";
			return;
		}
		if (this.isEmployee)
		{
			this.signupButtonText.text = "HIRED";
			return;
		}
		this.signupButtonText.text = "APPLY";
	}

	// Token: 0x06002448 RID: 9288 RVA: 0x000B678C File Offset: 0x000B498C
	private void OnGetUserDataInitialState(GetUserDataResult result)
	{
		UserDataRecord userDataRecord;
		if (result.Data.TryGetValue("GRData", out userDataRecord))
		{
			string value = userDataRecord.Value;
			this.isEmployee = true;
		}
		else
		{
			this.isEmployee = false;
		}
		this.isSigningUp = false;
		this.Refresh();
	}

	// Token: 0x06002449 RID: 9289 RVA: 0x000B67D1 File Offset: 0x000B49D1
	private void OnGetUserDataInitialStateFail(PlayFabError error)
	{
		this.isEmployee = false;
		this.isSigningUp = false;
		this.Refresh();
	}

	// Token: 0x0600244A RID: 9290 RVA: 0x000B67E7 File Offset: 0x000B49E7
	private void OnSaveTableSuccess(UpdateUserDataResult result)
	{
		this.isEmployee = true;
		this.isSigningUp = false;
		this.Refresh();
	}

	// Token: 0x0600244B RID: 9291 RVA: 0x000B67D1 File Offset: 0x000B49D1
	private void OnSaveTableFailure(PlayFabError error)
	{
		this.isEmployee = false;
		this.isSigningUp = false;
		this.Refresh();
	}

	// Token: 0x0400295F RID: 10591
	[SerializeField]
	private GorillaPressableButton signupButton;

	// Token: 0x04002960 RID: 10592
	[SerializeField]
	private TMP_Text signupButtonText;

	// Token: 0x04002961 RID: 10593
	[SerializeField]
	private Transform spawnMarker;

	// Token: 0x04002962 RID: 10594
	[SerializeField]
	private GRUIStationEmployeeBadges badgeStation;

	// Token: 0x04002963 RID: 10595
	private int entityTypeId;

	// Token: 0x04002964 RID: 10596
	private bool isEmployee;

	// Token: 0x04002965 RID: 10597
	private bool isSigningUp;

	// Token: 0x04002966 RID: 10598
	private const string GR_DATA_KEY = "GRData";
}
