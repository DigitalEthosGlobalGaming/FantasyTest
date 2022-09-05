using Degg.Core;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Degg.Cameras
{
	public partial class CinematicScene: Entity
	{
		public LinkedListNode<CinematicTransition> CurrentTransition { get; set; }
		public LinkedList<CinematicTransition> Transitions { get; set; }
		public List<PlayerCinematicTransition> PlayerTransitions { get; set; }
		public List<DeggPlayer> Players { get; set; }
		public float StartTime { get; set; }
		public bool Running { get; set; }

		public override void Spawn()
		{
			base.Spawn();
			Running = false;
			PlayerTransitions = new List<PlayerCinematicTransition>();
			Transitions = new LinkedList<CinematicTransition>();
			Players = new List<DeggPlayer>();
		}

		public T AddTransition<T>() where T : CinematicTransition, new()
		{
			return (T) AddTransition( new T() );
		}

		public virtual void AddPlayer(DeggPlayer p)
		{
			Players.Add( p );
		}
		public virtual void RemovePlayer( DeggPlayer p )
		{
			Players.Remove( p );
		}

		public CinematicTransition AddTransition( CinematicTransition t)
		{

			t.Parent = this;
			Transitions.AddLast( t );
			return t;
		}
		protected override void OnDestroy()
		{
			base.OnDestroy();
		}
		public void Tick()
		{
			var current = CurrentTransition?.ValueRef;
			if ( current != null ) {
				if ( Running )
				{
					CurrentTransition?.ValueRef.Tick();
				}

				foreach ( var player in Players )
				{
					if ( player.CameraMode is CinematicCamera camera )
					{
						camera.TargetPosition = current.Position;
						camera.FocusPosition = current.Target?.Position ?? Vector3.Zero;
					}
				}
			}

		}

		public void StartNextTransition()
		{
			if ( CurrentTransition == null )
			{
				CurrentTransition = Transitions.First;
			}

			var previous = CurrentTransition.Previous;
			previous?.Value?.End();

			var next = CurrentTransition.Next;
			if (next?.Value == null)
			{
				Stop();
			} else
			{
				next.Value.Start();
			}

		}

		public void Start()
		{
			if (Running)
			{
				return;
			}

			CurrentTransition = null;			
			StartTime = Time.Now;
			Running = true;
			StartNextTransition();
		}
		public void Stop()
		{
			CurrentTransition = null;
			Running = false;
		}
		public void Restart()
		{
			if ( Running )
			{
				Stop();
			}
			Start();
		}
		public void Pause()
		{
			if ( Running )
			{
				Running = false;
			}
		}
	}

	public partial class PlayerCinematicTransition
	{
		public PlayerCinematicTransition Transition { get; set; }
		public DeggPlayer Player { get; set; }

		public PlayerCinematicTransition(DeggPlayer p, PlayerCinematicTransition t)
		{
			Transition = t;
			Player = p;
		}

		public virtual void Attach()
		{

		}
		public virtual void Detach()
		{

		}
	}

	public partial class CinematicTransition
	{
		public CinematicScene Parent { get; set; }
		public Vector3 Position { get; set; }
		public float Duration {get;set;}
		public float StartTime { get; set; }

		public float EndTime { get; set; }
		public Entity Target { get; set; }

		public virtual void Tick()
		{
			if ( IsFinished())
			{
				End();
			}
			DebugOverlay.Sphere( Position, 10f, Color.Red );
		}


		public float GetDurationPercentage()
		{
			var a = Time.Now - StartTime;
			var b = EndTime - StartTime;
			var p = a / b;
			if (p > 1)
			{
				p = 1;
			}
			return p;
		}
		public virtual void Start()
		{
			StartTime = Time.Now;
			EndTime = Time.Now + Duration;
		}

		public virtual void End()
		{
			Parent.StartNextTransition();
		}

		public virtual bool IsFinished()
		{
			return Time.Now > EndTime;
		}
	}

	public partial class MovementTransition: CinematicTransition
	{
		public Vector3 StartPosition { get; set; }
		public Vector3 EndPosition { get; set; }


		public override void Start()
		{
			base.Start();

		}

		public override void Tick()
		{
			base.Tick();
			var percentage = GetDurationPercentage();
			Position = StartPosition.LerpTo( EndPosition, percentage );
			DebugOverlay.Sphere( Position, 10f, Color.Red );
		}
	}

}
