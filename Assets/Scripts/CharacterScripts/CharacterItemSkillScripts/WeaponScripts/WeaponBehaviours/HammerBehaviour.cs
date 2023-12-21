using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class HammerBehaviour : ItemBehaviour
{
    public float rotationSpeed = 360f; // Speed of rotation in degrees per second
    //public float moveSpeed = 5f; // Speed of movement in units per second
    public float circleRadius = 3f; // Radius of the circular path

    private float angle = 0f;

    private Vector2 _initialPosition;

    private void Start()
    {
        _initialPosition = transform.position;
    }


    void Update()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        // Calculate the position based on the circular path
        float x = circleRadius * Mathf.Cos(angle);
        float y = circleRadius * Mathf.Sin(angle);

        // Set the position of the weapon
        transform.position = new Vector3(x, y, 0) + (Vector3)_initialPosition;

        // Increment the angle for the next frame
        circleRadius += _speed/6 * Time.deltaTime;
        angle += _speed / circleRadius * Time.deltaTime;
    }

}
