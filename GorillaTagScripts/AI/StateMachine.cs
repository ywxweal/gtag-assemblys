using System;
using System.Collections.Generic;

namespace GorillaTagScripts.AI
{
	// Token: 0x02000B9A RID: 2970
	public class StateMachine
	{
		// Token: 0x060049A0 RID: 18848 RVA: 0x0015FD30 File Offset: 0x0015DF30
		public void Tick()
		{
			StateMachine.Transition transition = this.GetTransition();
			if (transition != null)
			{
				this.SetState(transition.To);
			}
			IState currentState = this._currentState;
			if (currentState == null)
			{
				return;
			}
			currentState.Tick();
		}

		// Token: 0x060049A1 RID: 18849 RVA: 0x0015FD64 File Offset: 0x0015DF64
		public void SetState(IState state)
		{
			if (state == this._currentState)
			{
				return;
			}
			IState currentState = this._currentState;
			if (currentState != null)
			{
				currentState.OnExit();
			}
			this._currentState = state;
			this._transitions.TryGetValue(this._currentState.GetType(), out this._currentTransitions);
			if (this._currentTransitions == null)
			{
				this._currentTransitions = StateMachine.EmptyTransitions;
			}
			this._currentState.OnEnter();
		}

		// Token: 0x060049A2 RID: 18850 RVA: 0x0015FDCE File Offset: 0x0015DFCE
		public IState GetState()
		{
			return this._currentState;
		}

		// Token: 0x060049A3 RID: 18851 RVA: 0x0015FDD8 File Offset: 0x0015DFD8
		public void AddTransition(IState from, IState to, Func<bool> predicate)
		{
			List<StateMachine.Transition> list;
			if (!this._transitions.TryGetValue(from.GetType(), out list))
			{
				list = new List<StateMachine.Transition>();
				this._transitions[from.GetType()] = list;
			}
			list.Add(new StateMachine.Transition(to, predicate));
		}

		// Token: 0x060049A4 RID: 18852 RVA: 0x0015FE1F File Offset: 0x0015E01F
		public void AddAnyTransition(IState state, Func<bool> predicate)
		{
			this._anyTransitions.Add(new StateMachine.Transition(state, predicate));
		}

		// Token: 0x060049A5 RID: 18853 RVA: 0x0015FE34 File Offset: 0x0015E034
		private StateMachine.Transition GetTransition()
		{
			foreach (StateMachine.Transition transition in this._anyTransitions)
			{
				if (transition.Condition())
				{
					return transition;
				}
			}
			foreach (StateMachine.Transition transition2 in this._currentTransitions)
			{
				if (transition2.Condition())
				{
					return transition2;
				}
			}
			return null;
		}

		// Token: 0x04004C7E RID: 19582
		private IState _currentState;

		// Token: 0x04004C7F RID: 19583
		private Dictionary<Type, List<StateMachine.Transition>> _transitions = new Dictionary<Type, List<StateMachine.Transition>>();

		// Token: 0x04004C80 RID: 19584
		private List<StateMachine.Transition> _currentTransitions = new List<StateMachine.Transition>();

		// Token: 0x04004C81 RID: 19585
		private List<StateMachine.Transition> _anyTransitions = new List<StateMachine.Transition>();

		// Token: 0x04004C82 RID: 19586
		private static List<StateMachine.Transition> EmptyTransitions = new List<StateMachine.Transition>(0);

		// Token: 0x02000B9B RID: 2971
		private class Transition
		{
			// Token: 0x17000711 RID: 1809
			// (get) Token: 0x060049A8 RID: 18856 RVA: 0x0015FF16 File Offset: 0x0015E116
			public Func<bool> Condition { get; }

			// Token: 0x17000712 RID: 1810
			// (get) Token: 0x060049A9 RID: 18857 RVA: 0x0015FF1E File Offset: 0x0015E11E
			public IState To { get; }

			// Token: 0x060049AA RID: 18858 RVA: 0x0015FF26 File Offset: 0x0015E126
			public Transition(IState to, Func<bool> condition)
			{
				this.To = to;
				this.Condition = condition;
			}
		}
	}
}
