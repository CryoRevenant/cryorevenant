using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Experimental.Rendering.Universal;


public class Ascenceur : MonoBehaviour
{
    List<GameObject> enemyList = new List<GameObject>();

    [SerializeField] GameObject otherElevator;
    [SerializeField] float speed;
    [SerializeField] Light2D light2D;

    GameObject player;
    Animator animator;
    float t = 0;

    public bool isUnlocked;
    public bool isIn;
    bool isClosed;
    bool playerIn;
    bool isMoving;

    private PlayerInput controls = null;

    void Start()
    {
        isMoving = false;

        if (isUnlocked)
        {
            light2D.color = Color.green;
        }
        player = GameObject.Find("Player");
        controls = player.GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controls != null)
        {
            if (controls.currentActionMap.FindAction("Interact").triggered && isIn && isUnlocked)
            {
                if (isClosed)
                {
                    CheckOpen();
                    otherElevator.GetComponent<Animator>().SetBool("ForceClose", true);
                }
                else
                {
                    player.layer = 6;
                    player.tag = "Untagged";
                    player.GetComponent<PlayerControllerV2>().enabled = false;
                    player.transform.GetChild(0).GetComponent<Animator>().SetFloat("Movement", 0);
                    player.GetComponent<PlayerControllerV2>().StopMovement();
                    player.GetComponent<PlayerAttack>().enabled = false;
                    CloseDoor();
                    //Debug.Log("ascenceur in use");
                }
            }
        }

        if (playerIn == true)
        {
            t += Time.deltaTime * speed;
            isIn = false;
            player.GetComponentInChildren<SpriteRenderer>().enabled = false;
            player.transform.position = new Vector3(player.transform.position.x, Mathf.Lerp(player.transform.position.y, otherElevator.transform.position.y, t), player.transform.position.z);

            if (isMoving)
            {
                //Debug.Log("ascenseur monte/descend");

                float random = Random.value;
                if (random <= 0.5f)
                {
                    FindObjectOfType<AudioManager>().Play("elevatorMove");
                    //Debug.Log("elevatorMove");
                }
                else if (random > 0.5f)
                {
                    FindObjectOfType<AudioManager>().Play("elevatorMove2");
                    //Debug.Log("elevatorMove2");
                }

                isMoving = false;

            }

            if (t >= 0.04f)
            {
                GetOut();
            }

            if (player.transform.position.y.ToString("0.0") == otherElevator.transform.position.y.ToString("0.0"))
            {
                player.GetComponentInChildren<SpriteRenderer>().enabled = true;
                t = 0;
                GetOut();
                playerIn = false;
            }
        }
    }

    public void CheckOpen()
    {
        int i = 0;
        foreach (GameObject enemy in enemyList)
        {
            if (!enemy.activeSelf)
            {
                i++;
            }
        }

        if (i >= enemyList.Count - 1)
        {
            Unlock();
        }
    }

    public void Unlock()
    {
        isUnlocked = true;
        isClosed = false;
        light2D.color = Color.green;
        FindObjectOfType<AudioManager>().Play("elevatorDing");
        animator.SetBool("openDoor", true);
        FindObjectOfType<AudioManager>().Play("elevatorOpenDoor");
    }

    public void Lock()
    {
        isUnlocked = false;
        isClosed = true;
        light2D.color = Color.red;
        animator.SetTrigger("closeDoor");
        otherElevator.GetComponent<Animator>().SetBool("ForceClose", true); 
    }

    void GetIn()
    {
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        player.GetComponent<CapsuleCollider2D>().isTrigger = true;
        playerIn = true;
    }

    void IsClosed()
    {
        isClosed = true;
        otherElevator.GetComponent<Animator>().SetBool("openDoor", false);
        otherElevator.GetComponent<Animator>().SetBool("ForceClose", false);
    }

    void CloseDoor()
    {
        Debug.Log("closeDoor");
        isUnlocked = false;
        player.GetComponentInChildren<SpriteRenderer>().sortingOrder = 0;
        animator.SetBool("openDoor", isUnlocked);
        //Debug.Log("ascenseur se ferme");
        isMoving = true;
        FindObjectOfType<AudioManager>().Play("elevatorCloseDoor");
    }

    void GetOut()
    {
        player.transform.position = otherElevator.transform.position;
        isUnlocked = true;
        player.GetComponent<PlayerControllerV2>().enabled = true;
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        player.GetComponent<CapsuleCollider2D>().isTrigger = false;
        otherElevator.GetComponent<Animator>().SetBool("openDoor", true);
        //Debug.Log("ascenseur ouvre");
        isMoving = true;
        FindObjectOfType<AudioManager>().Play("elevatorOpenDoor");
    }

    void OrderLayer()
    {
        player.tag = "Player";
        player.layer = 0;
        player.GetComponentInChildren<SpriteRenderer>().sortingOrder = 3;
        player.GetComponent<PlayerAttack>().enabled = true;
    }

    #region Add/Remove enemyList

    public void AddEnemy(GameObject enemy)
    {
        enemyList.Add(enemy);
    }
    #endregion

    #region Triggers

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && isUnlocked == true)
        {
            isIn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isIn = false;
        }
    }
    #endregion
}
