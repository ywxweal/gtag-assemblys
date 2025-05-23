using System;

// Token: 0x020004F0 RID: 1264
public interface IBuilderPieceFunctional
{
	// Token: 0x06001E98 RID: 7832
	void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp);

	// Token: 0x06001E99 RID: 7833
	void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp);

	// Token: 0x06001E9A RID: 7834
	bool IsStateValid(byte state);

	// Token: 0x06001E9B RID: 7835
	void FunctionalPieceUpdate();

	// Token: 0x06001E9C RID: 7836 RVA: 0x00002628 File Offset: 0x00000828
	void FunctionalPieceFixedUpdate()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06001E9D RID: 7837 RVA: 0x00095643 File Offset: 0x00093843
	float GetInteractionDistace()
	{
		return 2.5f;
	}
}
