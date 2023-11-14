using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class HammerBehaviour : ItemBehaviour
{
    public float rotationSpeed = 180f; // Speed of rotation in degrees per second
    //public float moveSpeed = 5f; // Speed of movement in units per second
    public float circleRadius = 3f; // Radius of the circular path

    private float angle = 0f;

    void Update()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        // Calculate the position based on the circular path
        float x = circleRadius * Mathf.Cos(angle);
        float y = circleRadius * Mathf.Sin(angle);

        // Set the position of the weapon
        transform.position = new Vector3(x, y, 0) + (Vector3)transform.parent.position;

        // Increment the angle for the next frame
        circleRadius += _speed/6 * Time.deltaTime;
        angle += _speed / circleRadius * Time.deltaTime;
    }

    protected override void Level2Effect()
    {
        transform.localScale += new Vector3(0.2f, 0.2f, 0);
        _damage *= 1.2f;
    }

    protected override void Level3Effect()
    {
        transform.localScale += new Vector3(0.2f, 0.2f, 0);
        _damage *= 1.2f;
    }

    protected override void Level4Effect()
    {
        transform.localScale += new Vector3(0.2f, 0.2f, 0);
        _damage *= 1.2f;
    }
}
