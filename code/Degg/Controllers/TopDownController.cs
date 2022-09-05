using Sandbox;

namespace Degg.Controllers
{
	public class TopDownController : BasePlayerController
	{
		public float MoveSpeed { get; set; } = 250f;

		public override void Simulate()
		{
			MoveSpeed = 250f;
			var moveDirection = Vector3.Zero;
			if (Input.Down( InputButton.Back ))
			{
				moveDirection = moveDirection + Vector3.Backward;
			}
			if ( Input.Down( InputButton.Forward ) )
			{
				moveDirection = moveDirection + Vector3.Forward;
			}
			if ( Input.Down( InputButton.Left ) )
			{
				moveDirection = moveDirection + Vector3.Left;
			}
			if ( Input.Down( InputButton.Right ) )
			{
				moveDirection = moveDirection + Vector3.Right;
			}

			moveDirection = moveDirection * Time.Delta * MoveSpeed;
			Velocity = moveDirection;
		}
	}
}
