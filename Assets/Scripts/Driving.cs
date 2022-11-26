using Vehicle;
using UnityEngine;

public class Driving : MonoBehaviour
{
	private Steering vehicleSteer;
	private InputActions inputAction;

	void Awake ()
	{
		vehicleSteer = GetComponent<Steering>();
		inputAction = new InputActions();
		inputAction.Driving.Enable();
	}

	void FixedUpdate ()
	{
		Vector2 steerVector = inputAction.Driving.Steer.ReadValue<Vector2>();

		vehicleSteer.Steer(steerVector.x);
	}
}
