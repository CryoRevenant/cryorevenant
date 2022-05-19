using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class Ascenceur : MonoBehaviour
{
    List<GameObject> enemyList = new List<GameObject>();

    [SerializeField] GameObject otherElevator;
    [SerializeField] float speed;

    GameObject player;
    Animator animator;
    float t = 0;

    public bool isUnlocked;
    public bool isIn;
    bool isClosed;
    bool playerIn;

    private PlayerInput controls = null;

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
            if (controls.currentActionMap.FindAction("Interact").triggered && isIn && isUnlocked)
            {
                if (isClosed)
                {
                    CheckOpen();
                    otherElevator.GetComponent<Animator>().SetBool("ForceClose", true);

                }
                else
                {
                    player.GetComponent<PlayerControllerV2>().enabled = false;
                    CloseDoor();
                }
            }
        }

        if (playerIn == true)
        {
            t += Time.deltaTime * speed;
            isIn = false;
            player.GetComponentInChildren<SpriteRenderer>().enabled = false;
            player.transform.position = new Vector3(player.transform.position.x, Mathf.Lerp(player.transform.position.y, otherElevator.transform.position.y, t), player.transform.position.z);

            if (player.transform.position.y.ToString("0.0") == otherElevator.transform.position.y.ToString("0.0"))
            {
                Debug.Log("bleu");
                player.GetComponentInChildren<SpriteRenderer>().enabled = true;
                t = 0;
                GetOut();
                playerIn = false;
            }
        }
    }

    void CheckOpen()
    {
        if (enemyList.Count == 0)
        {
            isUnlocked = true;
            isClosed = false;
            animator.SetBool("openDoor", isUnlocked);
        }
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
        otherElevator.GetComponent<Animator>().SetBool("ForceClose", false);
    }

    void CloseDoor()
    {
        isUnlocked = false;
        player.GetComponentInChildren<SpriteRenderer>().sortingOrder = 0;
        animator.SetBool("openDoor", isUnlocked);
    }

    void GetOut()
    {
        isUnlocked = true;
        player.GetComponent<PlayerControllerV2>().enabled = true;
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        player.GetComponent<CapsuleCollider2D>().isTrigger = false;
        otherElevator.GetComponent<Animator>().SetBool("openDoor", isUnlocked);
    }

    void OrderLayer()
    {
        player.GetComponentInChildren<SpriteRenderer>().sortingOrder = 2;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && isUnlocked == true)
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
