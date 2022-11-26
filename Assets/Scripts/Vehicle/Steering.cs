using System.Collections;
using UnityEngine;

namespace Vehicle
{
	public class Steering : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("For all steer drive, or other special cases, place at turn center. If not set, it will be generated automatically.")]
		private GameObject turnCenterObject = null;

		[SerializeField]
		[Tooltip("If the vehicle has a visible steering wheel, dropping it's transform in here will make it rotate.")]
		private Transform steeringWheel;

		[SerializeField]
		[Tooltip("How many degrees the steering wheel can rotate.")]
		private int steeringWheelDegrees = 972;

		[SerializeField]
		[Tooltip("Turn radius in meters.")]
		private float turnRadius = 5f;

		[SerializeField]
		[Tooltip("Minumum time, in seconds, to turn the steering wheel from center to one side.")]
		private float steerTime = 1f;


		private Wheel[] wheels;
		private float steer = 0f;
		private float lastSteer = 0f;

		private IEnumerator steerCoroutine;

		private void Awake ()
		{
			wheels = GetComponentsInChildren<Wheel>();
			locateAckermannBase();
		}

		private void locateAckermannBase ()
		{
			float front = 0f;
			float rear = 0f;
			float right = 0f;
			float left = 0f;
			int staticCount = 0;
			float turnCenter = 0f;

			foreach (var wheel in wheels) {
				Vector3 lp = wheel.transform.localPosition;

				if (lp.z > front) {
					front = lp.z;
				}
				if (lp.z < rear) {
					rear = lp.z;
				}

				if (lp.x < right) {
					right = lp.x;
				}
				if (lp.x > left) {
					left = lp.x;
				}

				if (!wheel.steer) {
					staticCount++;
				}
			}

			if (turnCenterObject == null) {
				turnCenterObject = new GameObject("Dynamically Generated Turn Center");
				turnCenterObject.transform.parent = this.gameObject.transform;
				if (staticCount > 0) {
					foreach (var wheel in wheels) {
						if (!wheel.steer) {
							Vector3 lp = wheel.transform.localPosition;
							turnCenter += Mathf.Abs(front - lp.z);
						}
					}
					turnCenter /= staticCount;
					turnCenterObject.transform.localPosition = new Vector3(0f, 0f, front - turnCenter);
				}
				else {
					turnCenterObject.transform.localPosition = new Vector3(0f, 0f, (front / 2) + (rear / 2));
				}
			}
		}

		public void Steer (float value)
		{
			if (lastSteer != value) {
				if (steerCoroutine != null) {
					StopCoroutine(steerCoroutine);
				}
				steerCoroutine = SteerLerp(steer, value);
				StartCoroutine(steerCoroutine);
				lastSteer = value;
			}
		}

		private void FixedUpdate ()
		{
			foreach (var wheel in wheels) {
				if (wheel.steer) {
					float centerDist = turnCenterObject.transform.localPosition.x - wheel.transform.localPosition.x;
					float lengthDist = wheel.transform.localPosition.z - turnCenterObject.transform.localPosition.z;

					if (steer > 0) {
						wheel.SteerAngle(Mathf.Rad2Deg * Mathf.Atan(lengthDist / (turnRadius + centerDist)) * steer);
					}
					else if (steer < 0) {
						wheel.SteerAngle(Mathf.Rad2Deg * Mathf.Atan(lengthDist / (turnRadius - centerDist)) * steer);
					}
					else {
						wheel.SteerAngle(0f);
					}
				}
			}

			if (steeringWheel != null) {
				steeringWheel.localEulerAngles = new Vector3(steeringWheel.localEulerAngles.x, steeringWheel.localEulerAngles.y, -(steer * (steeringWheelDegrees / 2)));
			}
		}

		private IEnumerator SteerLerp (float startValue, float endValue)
		{
			float timeElapsed = 0;
			float diff = Mathf.Abs(startValue - endValue);
			float internalSteerTime = steerTime * diff;
			while (timeElapsed < internalSteerTime) {
				steer = Mathf.Lerp(startValue, endValue, timeElapsed / internalSteerTime);
				timeElapsed += Time.deltaTime;
				yield return null;
			}
			steer = endValue;
		}
	}
}
