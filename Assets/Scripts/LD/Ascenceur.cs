using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Ascenceur : MonoBehaviour
{
    List<GameObject> enemyList = new List<GameObject>();
    [SerializeField] GameObject otherElevator;
    Animator animator;

    public bool isOpen;
    public bool isIn;

    private PlayerInput controls = null;
    GameObject player;

    void Start()
    {
        player = GameObject.Find("Player");
        controls = player.GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controls != null)
        {
            if (controls.currentActionMap.FindAction("Interact").triggered && isIn == true)
            {
                TPPlayer();
                isIn = false;
            }
        }
    }

    void CheckOpen()
    {
        Debug.Log(enemyList.Count);
        if (enemyList.Count == 0)
        {
            isOpen = true;
            animator.SetBool("openDoor", isOpen);
        }
    }

    void TPPlayer()
    {
        player.transform.position = otherElevator.transform.position;
    }

    #region Add/Remove enemyList
    public void RemoveEnemy(GameObject enemy)
    {
        enemyList.Remove(enemy);
        CheckOpen();
    }

    public void AddEnemy(GameObject enemy)
    {
        enemyList.Add(enemy);
    }
    #endregion

    #region Triggers

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && isOpen == true)
        {
            isIn = true;
            player.GetComponent<PlayerAttack>().enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player.GetComponent<PlayerAttack>().enabled = true;
            isIn = false;
        }
    }
    #endregion
}
