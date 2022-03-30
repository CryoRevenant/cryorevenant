using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public bool mustGo;
    [SerializeField] float speed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, posToGo.x, Time.deltaTime * speed), Mathf.Lerp(transform.position.y, posToGo.y, Time.deltaTime * speed), 0);
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
