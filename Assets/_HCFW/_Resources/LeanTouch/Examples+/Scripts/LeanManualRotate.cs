using UnityEngine;

namespace Lean.Touch
{
	// This component allows you to rotate the current GameObject using events
	public class LeanManualRotate : MonoBehaviour
	{
		public enum AngleType
		{
			Degrees,
			Radians
		}

		public enum ScaleType
		{
			None,
			DeltaTime
		}

		[Tooltip("Anle type")]
		public AngleType Angle;

		[Tooltip("Angle multiplier")]
		public ScaleType Scale;

		[Tooltip("The rotation space")]
		public Space Space = Space.World;

		[Tooltip("The first axis of rotation")]
		public Vector3 Axis = Vector3.forward;

		[Tooltip("Fixed multiplier of each rotation")]
		public float Multiplier = 1.0f;

		public virtual void Rotate(float delta)
		{
			if (Angle == AngleType.Radians)
			{
				delta *= Mathf.Rad2Deg;
			}

			if (Scale == ScaleType.DeltaTime)
			{
				delta *= Time.deltaTime;
			}

			transform.Rotate(Axis, delta * Multiplier, Space);
		}
	}
}