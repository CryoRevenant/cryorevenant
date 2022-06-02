using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDust : MonoBehaviour
{
    [SerializeField] ParticleSystem fallDust;
    [SerializeField] float dist;
    [SerializeField] bool isGrounded = false;

    private void Update()
    {
        Debug.DrawRay(transform.position, Vector3.down * dist, Color.blue, 0.2f);
        RaycastHit2D hit2D;
        if (hit2D = Physics2D.Raycast(transform.position, Vector2.down, dist))
        {
            if (hit2D.collider.CompareTag("Ground"))
            {
                if (!isGrounded)
                {
                    isGrounded = true;
                    //Debug.Log("Grounded");
                    FindObjectOfType<AudioManager>().Play("groundHit");
                    fallDust.Emit(1);
                }
            }
        }
        else
        {
            isGrounded = false;
        }
    }
}
