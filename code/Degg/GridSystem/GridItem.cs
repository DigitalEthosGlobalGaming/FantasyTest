using Degg.Util;
using Sandbox;

namespace Degg.GridSystem
{

	// A grid item is an individual item on a grid.
	public abstract class GridItem : ModelEntity
	{
		// An offset is where this item would sit on the grid.
		// When porting to SBox this will allow me to have an item on a "grid" but then it could be slightly left or right of the centre.
		public Vector3 Offset { get; set; }
		public GridSpace Space { get; set; }

		public bool IsSetup {get;set;}

        public void Remove()
        {
            Space = null;
            OnRemove();
        }

		public override void Spawn()
		{
			base.Spawn();
			Setup();
		}

		public void Setup()
		{
			if (!IsSetup )
			{
				if ( Space != null )
				{
					if ( Space.Map != null )
					{
						Scale = Space.Map.Scale;
					}
					UpdatePosition();
					OnSetup();
				}
			}
		}

        public GridMap GetMap()
        {
            return Space?.Map;
        }

        public bool Move(Vector2 newPosition)
        {
            var map = GetMap();
            if (map == null)
            {
                return false;
            }

            var didMove = map.MoveItem(this, newPosition);
			UpdatePosition();
			return didMove;

		}
        public bool MoveBy(Vector2 changes)
        {
			var currentPosition = GetGridPosition();
            var newPosition = new Vector2( currentPosition.x, currentPosition.y) + changes ;
            return this.Move(newPosition);
        }

		public void UpdatePosition()
		{
			Position = Space.GetWorldPosition() + Offset;
		}

		public Vector2 GetGridPosition()
        {
            return Space.GridPosition;
        }

        public virtual void OnRemove()
        {

        }

		public virtual void OnSetup()
		{

		}

		public virtual void OnAdded()
        {
			Setup();
			UpdatePosition();
		}
        public virtual void OnMove(Vector2 newPosition, Vector2 oldPosition)
        {

        }

    }
}
