using System;
using UnityEngine;

// Token: 0x020007E7 RID: 2023
public class KIDTelemetry
{
	// Token: 0x17000508 RID: 1288
	// (get) Token: 0x060031B0 RID: 12720 RVA: 0x000F5AA2 File Offset: 0x000F3CA2
	public static string GameVersionCustomTag
	{
		get
		{
			return "game_version_" + Application.version;
		}
	}

	// Token: 0x17000509 RID: 1289
	// (get) Token: 0x060031B1 RID: 12721 RVA: 0x000F5AB3 File Offset: 0x000F3CB3
	public static string Open_MetricActionCustomTag
	{
		get
		{
			return "metric_action_Open";
		}
	}

	// Token: 0x1700050A RID: 1290
	// (get) Token: 0x060031B2 RID: 12722 RVA: 0x000F5ABA File Offset: 0x000F3CBA
	public static string Updated_MetricActionCustomTag
	{
		get
		{
			return "metric_action_Updated";
		}
	}

	// Token: 0x1700050B RID: 1291
	// (get) Token: 0x060031B3 RID: 12723 RVA: 0x000F5AC1 File Offset: 0x000F3CC1
	public static string Closed_MetricActionCustomTag
	{
		get
		{
			return "metric_action_Closed";
		}
	}

	// Token: 0x1700050C RID: 1292
	// (get) Token: 0x060031B4 RID: 12724 RVA: 0x000F5AC8 File Offset: 0x000F3CC8
	public static string GameEnvironment
	{
		get
		{
			return "game_environment_live";
		}
	}

	// Token: 0x060031B5 RID: 12725 RVA: 0x000F5ACF File Offset: 0x000F3CCF
	public static string GetPermissionManagedByBodyData(string permission)
	{
		return "permission_managedby_" + permission.Replace('-', '_');
	}

	// Token: 0x060031B6 RID: 12726 RVA: 0x000F5AE5 File Offset: 0x000F3CE5
	public static string GetPermissionEnabledBodyData(string permission)
	{
		return "permission_eneabled_" + permission.Replace('-', '_');
	}

	// Token: 0x0400384E RID: 14414
	public const string SCREEN_SHOWN_EVENT_NAME = "kid_screen_shown";

	// Token: 0x0400384F RID: 14415
	public const string PHASE_TWO_IN_COHORT_EVENT_NAME = "kid_phase2_incohort";

	// Token: 0x04003850 RID: 14416
	public const string PHASE_THREE_OPTIONAL_EVENT_NAME = "kid_phase3_optional";

	// Token: 0x04003851 RID: 14417
	public const string AGE_GATE_EVENT_NAME = "kid_age_gate";

	// Token: 0x04003852 RID: 14418
	public const string AGE_GATE_CONFIRM_EVENT_NAME = "kid_age_gate_confirm";

	// Token: 0x04003853 RID: 14419
	public const string AGE_DISCREPENCY_EVENT_NAME = "kid_age_gate_discrepency";

	// Token: 0x04003854 RID: 14420
	public const string GAME_SETTINGS_EVENT_NAME = "kid_game_settings";

	// Token: 0x04003855 RID: 14421
	public const string EMAIL_CONFIRM_EVENT_NAME = "kid_email_confirm";

	// Token: 0x04003856 RID: 14422
	public const string AGE_APPEAL_EVENT_NAME = "kid_age_appeal";

	// Token: 0x04003857 RID: 14423
	public const string APPEAL_AGE_GATE_EVENT_NAME = "kid_age_appeal_age_gate";

	// Token: 0x04003858 RID: 14424
	public const string APPEAL_ENTER_EMAIL_EVENT_NAME = "kid_age_appeal_enter_email";

	// Token: 0x04003859 RID: 14425
	public const string APPEAL_CONFIRM_EMAIL_EVENT_NAME = "kid_age_appeal_confirm_email";

	// Token: 0x0400385A RID: 14426
	private const string GAME_VERSION_CUSTOM_TAG_PREFIX = "game_version_";

	// Token: 0x0400385B RID: 14427
	private const string METRIC_ACTION_CUSTOM_TAG_PREFIX = "metric_action_";

	// Token: 0x0400385C RID: 14428
	public const string WARNING_SCREEN_CUSTOM_TAG = "kid_warning_screen";

	// Token: 0x0400385D RID: 14429
	public const string PHASE_TWO = "kid_phase_2";

	// Token: 0x0400385E RID: 14430
	public const string PHASE_THREE = "kid_phase_3";

	// Token: 0x0400385F RID: 14431
	public const string PHASE_FOUR = "kid_phase_4";

	// Token: 0x04003860 RID: 14432
	public const string AGE_GATE_CUSTOM_TAG = "kid_age_gate";

	// Token: 0x04003861 RID: 14433
	public const string SETTINGS_CUSTOM_TAG = "kid_settings";

	// Token: 0x04003862 RID: 14434
	public const string SETUP_CUSTOM_TAG = "kid_setup";

	// Token: 0x04003863 RID: 14435
	public const string APPEAL_CUSTOM_TAG = "kid_age_appeal";

	// Token: 0x04003864 RID: 14436
	public const string SCREEN_TYPE_BODY_DATA = "screen";

	// Token: 0x04003865 RID: 14437
	public const string OPT_IN_CHOICE_BODY_DATA = "opt_in_choice";

	// Token: 0x04003866 RID: 14438
	public const string BUTTON_PRESSED_BODY_DATA = "button_pressed";

	// Token: 0x04003867 RID: 14439
	public const string MISMATCH_EXPECTED_BODY_DATA = "mismatch_expected";

	// Token: 0x04003868 RID: 14440
	public const string MISMATCH_ACTUAL_BODY_DATA = "mismatch_actual";

	// Token: 0x04003869 RID: 14441
	public const string AGE_DECLARED_BODY_DATA = "age_declared";

	// Token: 0x0400386A RID: 14442
	public const string LEARN_MORE_URL_PRESSED_BODY_DATA = "learn_more_url_pressed";

	// Token: 0x0400386B RID: 14443
	public const string SCREEN_SHOWN_REASON_BODY_DATA = "screen_shown_reason";

	// Token: 0x0400386C RID: 14444
	public const string SUBMITTED_AGE_BODY_DATA = "submitted_age";

	// Token: 0x0400386D RID: 14445
	public const string CORRECT_AGE_BODY_DATA = "correct_age";

	// Token: 0x0400386E RID: 14446
	public const string APPEAL_EMAIL_TYPE_BODY_DATA = "email_type";

	// Token: 0x0400386F RID: 14447
	public const string SHOWN_SETTINGS_SCREEN = "saw_game_settings";

	// Token: 0x04003870 RID: 14448
	public const string KID_STATUS_BODY_DATA = "kid_status";

	// Token: 0x04003871 RID: 14449
	private const string PERMISSION_MANAGED_BY_BODY_DATA = "permission_managedby_";

	// Token: 0x04003872 RID: 14450
	private const string PERMISSION_ENABLED_BODY_DATA = "permission_eneabled_";
}
