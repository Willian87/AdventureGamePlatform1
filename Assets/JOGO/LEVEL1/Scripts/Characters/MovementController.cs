using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    //InputController inputController;
    //private Rigidbody2D rb;


    ////Movement System
    //[SerializeField] private float speed;
    //[SerializeField] private Transform groundCheck;
    //[SerializeField] private bool isGrounded;
    //[SerializeField] private float groundCheckRadius;
    //[SerializeField] private LayerMask groundCheckLayerMask;
    //[SerializeField] private float jumpForce;
    //bool isJumping = false;

    ////Flip sprite
    //[SerializeField] private bool isRight;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    rb = GetComponent<Rigidbody2D>();
    //}

    //// Update is called once per frame
    //void FixedUpdate()
    //{

    //}

    //public void Move(Vector2 direction)
    //{
    //    Vector2 moviment = direction.normalized * speed;
    //    rb.velocity = new Vector2(moviment.x, 0);

    //    if (!isRight)
    //    {
    //        FlipX();
    //    }
    //    if (isRight)
    //    {
    //        FlipX();
    //    }
    //}

    //public void Jump()
    //{
    //    isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundCheckLayerMask);

    //    if (isGrounded)
    //    {
    //        //isJumping = true;
    //        rb.velocity = Vector2.up * jumpForce;
    //    }


    //}

    //public void FlipX()
    //{
    //    isRight = !isRight;
    //    transform.Rotate(0f, 180f, 0f);
    //}

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    //}
}
