using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
	public float sensitivity = 2f;
	Vector3 euler;
	void Update ()
	{
		euler.x -= Input.GetAxis("Mouse Y")* sensitivity;
		euler.y += Input.GetAxis("Mouse X")* sensitivity;
		euler.x = Mathf.Clamp(euler.x, -89, 89);
		transform.localEulerAngles = euler;
		float speed = Input.GetKey(KeyCode.LeftShift) ? 40f : 10f;
		Vector2 movement=new Vector2();
		movement.y -= Input.GetKey(KeyCode.S) ? 1f : 0f;
		movement.y += Input.GetKey(KeyCode.W) ? 1f : 0f;
		movement.x -= Input.GetKey(KeyCode.A) ? 1f : 0f;
		movement.x += Input.GetKey(KeyCode.D) ? 1f : 0f;
		transform.position += transform.forward * movement.y*speed*Time.deltaTime;
		transform.position += transform.right * movement.x* speed * Time.deltaTime;
	}
}
