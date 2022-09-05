
using Sandbox;

namespace Degg.Util.RoundSystem
{
	public partial class Round: Entity
	{
		[Net]
		public RoundState State { get; set; }

		[Net]
		public float StartTime { get; set; }

		[Net]
		public float EndTime { get; set; }

		[Net]
		public float Duration { get; set; }

		[Net]
		public float RunningTime { get; set; }

		public virtual bool CanRoundEnd()
		{
			if (Duration == 0)
			{
				return false;
			}
			return RunningTime > Duration;
		}
		public enum RoundState
		{
			Warmup,			
			InProgress,
			Paused,
			Ended			
		}

		public override void Spawn()
		{
			base.Spawn();
			State = RoundState.Warmup;
		}

		public virtual void StartRound()
		{
			StartTime = Time.Now;
			EndTime = Time.Now;
			RunningTime = 0;
			State = RoundState.InProgress;
			OnRoundStart();
		}

		public virtual void PauseRound()
		{
			State = RoundState.Paused;
			OnRoundPaused();
		}
		public virtual void UnPauseRound()
		{
			State = RoundState.InProgress;
			OnRoundUnPaused();
		}
		public virtual void EndRound()
		{
			State = RoundState.Ended;			
			EndTime = Time.Now;
			OnRoundEnd();
		}

		public virtual void OnStateChange(RoundState before, RoundState after)
		{

		}
		public virtual void OnRoundEnd()
		{

		}
		public virtual void OnRoundPaused()
		{

		}
		public virtual void OnRoundUnPaused()
		{

		}
		public virtual void OnRoundStart()
		{

		}
		public virtual void Tick()
		{
			if ( IsServer )
			{
				if ( State == RoundState.InProgress )
				{
					if ( Duration > 0 )
					{
						RunningTime = RunningTime + Time.Delta;
					}
					if ( CanRoundEnd() )
					{
						EndRound();
					}
					else
					{
						InProgressTick();
					}
				} else if (State == RoundState.Ended)
				{
					OnEndTick();
				}
			}
		}
		public virtual void OnEndTick()
		{
		}
		public virtual void InProgressTick()
		{
		}
	}
}
