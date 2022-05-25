using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceParticle : MonoBehaviour
{
    Transform destination;
    Vector3 origin;
    [SerializeField] float speed;
    float travel = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.SetParent(destination);
        destination = GameObject.Find("ParticlePoint").transform;
        origin = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vectorDest = new Vector3(destination.position.x, destination.position.y, 0);
        travel += Time.deltaTime * speed;
        transform.position = Vector3.Slerp(origin, vectorDest, travel);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Particle"))
        {
            Destroy(gameObject);
        }
    }
}
