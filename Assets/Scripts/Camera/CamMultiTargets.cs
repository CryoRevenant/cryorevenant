using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamMultiTargets : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private Transform camOffset;
    private List<Transform> bigSoldats = new List<Transform>();

    [SerializeField] private Vector3 offset;
    private List<Transform> targets = new List<Transform>();
    bool canAdd;
    private Transform closestSac;

    private void Awake()
    {
        for (int i = 0; i < GameObject.FindObjectsOfType<SacMove>().Length; i++)
        {
            bigSoldats.Add(GameObject.FindObjectsOfType<SacMove>()[i].transform);
            //Debug.Log(bigSoldats[i].name);
        }

        targets.Add(player);
        canAdd = true;
    }

    void LateUpdate()
    {
        if (bigSoldats.Count != 0)
        {
            for (int i = 0; i < bigSoldats.Count; i++)
            {
                float distX = bigSoldats[i].transform.position.x - player.GetComponent<Rigidbody2D>().position.x;
                float distY = player.GetComponent<Rigidbody2D>().position.y - bigSoldats[i].transform.position.y;
                //Debug.Log(dist);
                if (distX <= 18 && distX >= -18 && bigSoldats[i].gameObject.activeSelf && distY <= 5 && distY >= -1)
                {
                    //Debug.Log("magnetic");
                    closestSac = bigSoldats[i];
                }
            }
        }

        if(closestSac != null)
        {
            if (canAdd)
            {
                targets.Add(closestSac.transform);

                canAdd = false;
            }

            Vector3 centerPoint = GetCenterPoint();
            //Debug.Log(centerPoint);

            Vector3 newPosition = centerPoint + offset;

            transform.position = newPosition;

            float distX = closestSac.transform.position.x - player.GetComponent<Rigidbody2D>().position.x;
            float distY = player.GetComponent<Rigidbody2D>().position.y - closestSac.transform.position.y;
            //Debug.Log("distY "+ distY);
            //Debug.Log("distX "+ distX);
            if (distX <= 18 && distX >= -18 && closestSac.gameObject.activeSelf && distY <= 5 && distY >= -1)
            {
                //Debug.Log("attach");
                vcam.GetCinemachineComponent<CinemachineTransposer>().m_XDamping = 1;
                vcam.GetCinemachineComponent<CinemachineTransposer>().m_YDamping = 1;
                vcam.GetCinemachineComponent<CinemachineTransposer>().m_YawDamping = 15;

                vcam.Follow = transform;
                vcam.m_Lens.OrthographicSize = Mathf.Lerp(vcam.m_Lens.OrthographicSize, 7, 0.05f);
            }
            else
            {
                //Debug.Log("dettach");

                vcam.Follow = camOffset;
                //Debug.Log("remove");
                targets.Remove(closestSac.transform);
                closestSac = null;
            }
        }
        else
        {
            canAdd = true;
            vcam.GetCinemachineComponent<CinemachineTransposer>().m_YawDamping -= Time.deltaTime/2;
            vcam.GetCinemachineComponent<CinemachineTransposer>().m_YawDamping = Mathf.Clamp(vcam.GetCinemachineComponent<CinemachineTransposer>().m_YawDamping, 0, 15);
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(vcam.m_Lens.OrthographicSize, 5.5f, 0.05f);
        }

        //Debug.Log(targets.Count);
    }

    Vector3 GetCenterPoint()
    {
        var bounds = new Bounds(closestSac.position, Vector3.zero);

        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }
}
