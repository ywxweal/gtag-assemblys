using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200076B RID: 1899
public class LifeCycleEventTrigger : MonoBehaviour
{
	// Token: 0x06002F55 RID: 12117 RVA: 0x000EBFE7 File Offset: 0x000EA1E7
	private void Awake()
	{
		UnityEvent onAwake = this._onAwake;
		if (onAwake == null)
		{
			return;
		}
		onAwake.Invoke();
	}

	// Token: 0x06002F56 RID: 12118 RVA: 0x000EBFF9 File Offset: 0x000EA1F9
	private void Start()
	{
		UnityEvent onStart = this._onStart;
		if (onStart == null)
		{
			return;
		}
		onStart.Invoke();
	}

	// Token: 0x06002F57 RID: 12119 RVA: 0x000EC00B File Offset: 0x000EA20B
	private void OnEnable()
	{
		UnityEvent onEnable = this._onEnable;
		if (onEnable == null)
		{
			return;
		}
		onEnable.Invoke();
	}

	// Token: 0x06002F58 RID: 12120 RVA: 0x000EC01D File Offset: 0x000EA21D
	private void OnDisable()
	{
		UnityEvent onDisable = this._onDisable;
		if (onDisable == null)
		{
			return;
		}
		onDisable.Invoke();
	}

	// Token: 0x06002F59 RID: 12121 RVA: 0x000EC02F File Offset: 0x000EA22F
	private void OnDestroy()
	{
		UnityEvent onDestroy = this._onDestroy;
		if (onDestroy == null)
		{
			return;
		}
		onDestroy.Invoke();
	}

	// Token: 0x040035B8 RID: 13752
	[SerializeField]
	private UnityEvent _onAwake;

	// Token: 0x040035B9 RID: 13753
	[SerializeField]
	private UnityEvent _onStart;

	// Token: 0x040035BA RID: 13754
	[SerializeField]
	private UnityEvent _onEnable;

	// Token: 0x040035BB RID: 13755
	[SerializeField]
	private UnityEvent _onDisable;

	// Token: 0x040035BC RID: 13756
	[SerializeField]
	private UnityEvent _onDestroy;
}
