using System;
using System.Collections.Generic;
using Degg.Util;
using Sandbox;

namespace Degg.Utils
{

	public interface ITickable
	{
		public TickableCollection ParentCollection { get; set; }
		public void Tick( float delta, float currentTick );
	}

	public partial class Timer: ITickable
	{

		public const float Second = 1000f;
		public const float Minute = 1000f * 60f;
		public const float Hour = 1000f * 60f * 60f;
		public const float Day = 1000f * 60f * 60f * 24f;
		public float Interval { get; protected set; }
		public float NextTick { get; protected set; }
		public float LastTick { get; protected set; }
		public float CurrentTick { get; protected set; }
		public float Delta { get; protected set; }
		public bool IsStarted { get; protected set; }
		public Action<Timer> Callback { get; protected set; }
		public TickableCollection ParentCollection { get; set; }

		public Timer( Action<Timer> callback, float interval )
		{
			TickableCollection.Global.Add( this );
			Interval = interval;
			Callback = callback;
		}
		public Timer()
		{
		}

		public void Delete()
		{
			TickableCollection.Global.Remove( this );
		}

		public void Stop()
		{
			IsStarted = false;
		}

		public void Start()
		{
			IsStarted = true;
		}


		public float GetPercentage(float currentTick = -1)
		{
			if (currentTick < 0)
			{
				currentTick = Time.Tick;
			}

			currentTick = currentTick - LastTick;
			var max = NextTick - LastTick;

			if (max <= 0)
			{
				return 0;
			}

			if (currentTick <= 0)
			{
				return 0;
			}

			return currentTick / max;
		}

		public void Tick(float delta, float currentTick )
		{
			if ( Callback != null && IsStarted )
			{
				if ( currentTick >= NextTick && currentTick != LastTick )
				{
					CurrentTick = Time.Tick;
					LastTick = NextTick;
					NextTick = currentTick + (Interval / 1000);
					Delta = currentTick - LastTick;
					Callback(this);
				}
			}
		}
	}

	public partial class DelayedCallback : Timer
	{

		public DelayedCallback( Action<Timer> callback, float interval )
		{
			TickableCollection.Global.Add( this );
			Interval = interval;
			Callback = (Timer t) =>
			{
				Delete();
				if ( callback != null )
				{
					callback( t );
				}
			};
		}
	}

	public partial class TickableCollection
	{
		public static TickableCollection Global = new TickableCollection();
		public List<ITickable> Items { get; set; } = new List<ITickable>();

		[Event.Tick]
		public static void GlobalTick()
		{
			Global.Tick();
		}
		public void Add(ITickable item)
		{						
			item.ParentCollection = this;
			Items.Add( item );
		}

		public void Remove(ITickable item )
		{
			if ( item != null )
			{
				Items.Remove( item );
				item.ParentCollection = null;
			}
		}

		public void Tick()
		{
			if ( Items != null )
			{
				var itemsToTick = new List<ITickable>( Items );
				foreach (var item in itemsToTick )
				{
					if ( item != null )
					{
						item.Tick( Time.Delta, Time.Now );
					}
				}
			}
		}

	}
}
