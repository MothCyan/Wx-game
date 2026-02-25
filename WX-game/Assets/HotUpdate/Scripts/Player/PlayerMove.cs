using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public int speed = 5;
    public Joystick joystick;

    void Update()
    {
        Vector2 moveDirection = joystick.GetInputDirection();
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }
}
