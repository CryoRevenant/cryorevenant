using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDust : MonoBehaviour
{
    [SerializeField] GameObject fallDust;
    [SerializeField] float dist;
    [SerializeField] bool isGrounded = false;
    private GameObject instance;

    private void Update()
    {
        //Debug.Log(gameObject.GetComponent<Rigidbody2D>().velocity.y);

        Debug.DrawRay(transform.position, Vector3.down * dist, Color.blue, 0.2f);
        RaycastHit2D hit2D;
        if (hit2D = Physics2D.Raycast(transform.position, Vector2.down, dist))
        {
            if (hit2D.collider.CompareTag("Ground") && gameObject.GetComponent<Rigidbody2D>().velocity.y < 0)
            {
                if (!isGrounded)
                {
                    isGrounded = true;
                    //Debug.Log("Grounded");
                    FindObjectOfType<AudioManager>().Play("groundHit");
                    instance = Instantiate(fallDust,new Vector2(transform.GetComponent<Rigidbody2D>().position.x, transform.GetComponent<Rigidbody2D>().position.y-0.25f),Quaternion.identity);
                    Destroy(instance,0.5f);
                }
            }
        }
        else
        {
            isGrounded = false;
        }

        if(instance!= null)
        instance.transform.position = new Vector2(instance.transform.position.x, transform.GetComponent<Rigidbody2D>().position.y - 0.25f);

        if(gameObject.GetComponent<Rigidbody2D>().velocity.y > 0)
        {
            if (instance != null)
                Destroy(instance);
        }
    }
}
