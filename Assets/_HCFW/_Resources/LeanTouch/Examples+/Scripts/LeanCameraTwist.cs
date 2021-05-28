using UnityEngine;

namespace Lean.Touch
{
	// This component allows you to rotate the current GameObject using a twist gesture
	public class LeanCameraTwist : MonoBehaviour
	{
		[Tooltip("The camera the movement will be done relative to (None = MainCamera)")]
		public Camera Camera;

		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreStartedOverGui = true;

		[Tooltip("Ignore fingers with IsOverGui?")]
		public bool IgnoreIsOverGui;

		[Tooltip("Ignore fingers if the finger count doesn't match? (0 = any)")]
		public int RequiredFingerCount;

		protected virtual void LateUpdate()
		{
			// Get the fingers we want to use
			var fingers = LeanTouch.GetFingers(IgnoreStartedOverGui, IgnoreIsOverGui, RequiredFingerCount);

			// Get the degrees these fingers twisted
			var degrees = LeanGesture.GetTwistDegrees(fingers);

			// Apply twist
			transform.Rotate(Vector3.forward, -degrees, Space.Self);
		}
	}
}