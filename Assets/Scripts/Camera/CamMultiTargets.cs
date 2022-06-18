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
                float dist = bigSoldats[i].transform.position.x - player.transform.position.x;
                //Debug.Log(dist);
                if (dist <= 18 && dist >= -18 && bigSoldats[i].gameObject.activeSelf)
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

            float distX = closestSac.transform.position.x - player.transform.position.x;
            float distY = player.transform.position.y - closestSac.transform.position.y;
            Debug.Log("distY "+ distY);
            if (distX <= 18 && distX >= -18 && closestSac.gameObject.activeSelf && distY <= 5 && distY >= -5)
            {
                //Debug.Log("attach");
                vcam.Follow = transform;
                vcam.m_Lens.OrthographicSize = Mathf.Lerp(vcam.m_Lens.OrthographicSize, 7, 1);
            }
            else
            {
                //Debug.Log("dettach");
                vcam.Follow = camOffset;
                vcam.m_Lens.OrthographicSize = Mathf.Lerp(vcam.m_Lens.OrthographicSize, 5.5f, 1);
                //Debug.Log("remove");
                targets.Remove(closestSac.transform);
                closestSac = null;
            }
        }

        Debug.Log(targets.Count);
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
