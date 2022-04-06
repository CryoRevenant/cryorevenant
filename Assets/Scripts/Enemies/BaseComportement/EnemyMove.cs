using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public bool mustGo;
    bool isGrounded;

    [SerializeField] float speed;
    [SerializeField] float speedFall;
    [SerializeField] float rayLength;
    [SerializeField] float bounds;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Physics2D.Raycast(new Vector2(transform.position.x + bounds, transform.position.y), Vector2.down, rayLength))
        {
            if (Physics2D.Raycast(new Vector2(transform.position.x - bounds, transform.position.y), Vector2.down, rayLength))
            {
                isGrounded = true;
            }
        }
        else
        {
            isGrounded = false;
        }

        if (isGrounded == false)
        {
            Vector2 pos = transform.position;
            pos.y -= Time.deltaTime * speedFall;
            transform.position = pos;
        }
    }

    IEnumerator Move()
    {
        yield return null;
    }

    IEnumerator MoveOver(GameObject lastPos)
    {
        Vector3 posToGo = lastPos.transform.position;
        Debug.Log(lastPos.transform.position + " " + gameObject.name);
        while (mustGo == true)
        {
            transform.LookAt(new Vector3(transform.position.x, transform.position.y,lastPos.transform.position.x));
            transform.position = Vector3.MoveTowards(transform.position,new Vector3(posToGo.x, transform.position.y, 0),Time.deltaTime*speed);
            if (Vector3.Distance(transform.position, posToGo) < 1f)
            {
                Reset(lastPos);
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    void Reset(GameObject player)
    {
        player = null;
        mustGo = false;
    }
}
