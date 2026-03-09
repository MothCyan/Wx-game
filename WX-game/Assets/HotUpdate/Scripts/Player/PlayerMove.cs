using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public int speed = 5;
    public Joystick joystick;
    public static Vector2 PlayerPosition;

    void Update()
    {
        Vector2 moveDirection = joystick.GetInputDirection();
        transform.Translate(moveDirection * speed * Time.deltaTime);
        PlayerPosition = transform.position;

        // 向右移动时 X 轴翻转 180 度，向左时恢复
        if (moveDirection.x < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (moveDirection.x > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}
