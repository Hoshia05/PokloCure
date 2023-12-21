using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookBehaviour : ItemBehaviour
{
    float radius = 4f;

    private float angle = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Rotate(Vector3.forward, _speed * 10 * Time.deltaTime);

        // Calculate the position based on the circular path
        float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);

        // Set the position of the weapon
        transform.position = new Vector3(x, y, 0) + (Vector3)transform.parent.position;


        angle += _speed * 200 / radius * Time.deltaTime;
    }

    public void SetInitialPosition(Vector2 initialPosition, float initialAngle)
    {
        transform.position += (Vector3)initialPosition;
        angle = initialAngle;
    }
}
