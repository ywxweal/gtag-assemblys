using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KID.Model;
using UnityEngine;

// Token: 0x020007AD RID: 1965
public class TMPSession
{
	// Token: 0x170004F1 RID: 1265
	// (get) Token: 0x060030B8 RID: 12472 RVA: 0x000EF62B File Offset: 0x000ED82B
	public bool IsValidSession
	{
		get
		{
			return (this.IsDefault && this.Permissions != null && this.Permissions.Count > 0) || (!this.IsDefault && this.SessionId != Guid.Empty);
		}
	}

	// Token: 0x060030B9 RID: 12473 RVA: 0x000EF668 File Offset: 0x000ED868
	public TMPSession(Session session, KIDDefaultSession defaultSession, SessionStatus status)
	{
		this.Permissions = new Dictionary<EKIDFeatures, Permission>();
		this.SessionStatus = status;
		if (session == null && defaultSession == null)
		{
			return;
		}
		if (session == null)
		{
			this.IsDefault = true;
			this.AgeStatus = defaultSession.AgeStatus;
			this.InitialiseDefaultPermissionSet(defaultSession);
			return;
		}
		this.SessionId = session.SessionId;
		this.Etag = session.Etag;
		this.AgeStatus = session.AgeStatus;
		this.KidStatus = session.Status;
		this.DateOfBirth = session.DateOfBirth;
		this.KUID = session.Kuid;
		this.Jurisdiction = session.Jurisdiction;
		this.ManagedBy = session.ManagedBy;
		this.Age = this.GetAgeFromDateOfBirth();
		for (int i = 0; i < session.Permissions.Count; i++)
		{
			EKIDFeatures? ekidfeatures = KIDFeaturesExtensions.FromString(session.Permissions[i].Name);
			if (ekidfeatures != null)
			{
				if (this.Permissions.ContainsKey(ekidfeatures.Value))
				{
					Debug.LogError("[KID::SESSION] Tried creating new session, but permission for [" + ekidfeatures.Value.ToStandardisedString() + "] already exists");
				}
				else
				{
					this.Permissions.Add(ekidfeatures.Value, session.Permissions[i]);
				}
			}
		}
	}

	// Token: 0x060030BA RID: 12474 RVA: 0x000EF7AB File Offset: 0x000ED9AB
	public bool TryGetPermission(EKIDFeatures feature, out Permission permission)
	{
		if (!this.Permissions.ContainsKey(feature))
		{
			Debug.LogError("[KID::SESSION] Tried retreiving permission for [" + feature.ToStandardisedString() + "], but does not exist");
			permission = null;
			return false;
		}
		permission = this.Permissions[feature];
		return true;
	}

	// Token: 0x060030BB RID: 12475 RVA: 0x000EF7E9 File Offset: 0x000ED9E9
	public List<Permission> GetAllPermissions()
	{
		return this.Permissions.Values.ToList<Permission>();
	}

	// Token: 0x060030BC RID: 12476 RVA: 0x000EF7FC File Offset: 0x000ED9FC
	public bool HasPermissionForFeature(EKIDFeatures feature)
	{
		Permission permission;
		if (!this.TryGetPermission(feature, out permission))
		{
			Debug.LogError("[KID::SESSION] Tried checking for permission but couldn't find [" + feature.ToStandardisedString() + "]. Assuming disabled");
			return false;
		}
		return permission.Enabled;
	}

	// Token: 0x060030BD RID: 12477 RVA: 0x000EF838 File Offset: 0x000EDA38
	public void UpdatePermission(EKIDFeatures feature, Permission newData)
	{
		if (!this.Permissions.ContainsKey(feature))
		{
			Debug.Log("[KID::SESSION] Trying to update permission, but could not find [" + feature.ToStandardisedString() + "] in dictionary. Will add new one");
			this.Permissions.Add(feature, null);
		}
		this.Permissions[feature] = newData;
	}

	// Token: 0x060030BE RID: 12478 RVA: 0x000EF888 File Offset: 0x000EDA88
	private void InitialiseDefaultPermissionSet(KIDDefaultSession defaultSession)
	{
		for (int i = 0; i < defaultSession.Permissions.Count; i++)
		{
			EKIDFeatures? ekidfeatures = KIDFeaturesExtensions.FromString(defaultSession.Permissions[i].Name);
			if (ekidfeatures == null)
			{
				Debug.LogError("[KID::SESSION] Tried creating new session, but failed to cast from [" + defaultSession.Permissions[i].Name + "] to [EKIDFeatures] enum");
			}
			else if (this.Permissions.ContainsKey(ekidfeatures.Value))
			{
				Debug.LogError("[KID::SESSION] Tried creating new session, but permission for [" + ekidfeatures.Value.ToStandardisedString() + "] already exists");
			}
			else
			{
				this.Permissions.Add(ekidfeatures.Value, defaultSession.Permissions[i]);
			}
		}
	}

	// Token: 0x060030BF RID: 12479 RVA: 0x000EF94C File Offset: 0x000EDB4C
	private int GetAgeFromDateOfBirth()
	{
		DateTime today = DateTime.Today;
		int num = today.Year - this.DateOfBirth.Year;
		int num2 = today.Month - this.DateOfBirth.Month;
		if (num2 < 0)
		{
			num--;
		}
		else if (num2 == 0 && today.Day - this.DateOfBirth.Day < 0)
		{
			num--;
		}
		return num;
	}

	// Token: 0x060030C0 RID: 12480 RVA: 0x000EF9B0 File Offset: 0x000EDBB0
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("New TMPSession]:");
		stringBuilder.AppendLine(string.Format("    - Is Default    :   {0}", this.IsDefault));
		stringBuilder.AppendLine(string.Format("    - Is Valid      :   {0}", this.IsValidSession));
		stringBuilder.AppendLine(string.Format("    - SessionID     :   {0}", this.SessionId));
		stringBuilder.AppendLine(string.Format("    - Age           :   {0}", this.Age));
		stringBuilder.AppendLine(string.Format("    - AgeStatus     :   {0}", this.AgeStatus));
		stringBuilder.AppendLine(string.Format("    - SessionStatus :   {0}", this.KidStatus));
		stringBuilder.AppendLine("    - DoB           :   " + this.DateOfBirth.ToString());
		stringBuilder.AppendLine("    - KUID          :   " + this.KUID);
		stringBuilder.AppendLine("    - Jurisdiction  :   " + this.Jurisdiction);
		stringBuilder.AppendLine("    - PERMISSIONS   :");
		if (this.Permissions != null)
		{
			foreach (Permission permission in this.Permissions.Values)
			{
				stringBuilder.AppendLine(string.Format("        - {0} - Enabled: {1} - ManagedBy: {2}", permission.Name, permission.Enabled, permission.ManagedBy));
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0400370C RID: 14092
	public readonly Guid SessionId;

	// Token: 0x0400370D RID: 14093
	public readonly string Etag;

	// Token: 0x0400370E RID: 14094
	public readonly AgeStatusType AgeStatus;

	// Token: 0x0400370F RID: 14095
	public readonly Session.StatusEnum KidStatus;

	// Token: 0x04003710 RID: 14096
	public readonly Session.ManagedByEnum ManagedBy;

	// Token: 0x04003711 RID: 14097
	public readonly DateTime DateOfBirth;

	// Token: 0x04003712 RID: 14098
	public readonly string Jurisdiction;

	// Token: 0x04003713 RID: 14099
	public readonly string KUID;

	// Token: 0x04003714 RID: 14100
	public readonly int Age;

	// Token: 0x04003715 RID: 14101
	public readonly bool IsDefault;

	// Token: 0x04003716 RID: 14102
	public readonly SessionStatus SessionStatus;

	// Token: 0x04003717 RID: 14103
	private Dictionary<EKIDFeatures, Permission> Permissions;
}
