using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform attackPos;
    [SerializeField] private Vector2 attackPosDistance;
    [SerializeField] private float attackRange;
    [SerializeField] private int damage;
    [SerializeField] private float damageCooldown;
    [SerializeField] private float attackCooldown;
    [Header("IceBar")]
    [SerializeField] private int iceToAdd;
    [Header("Sprites")]
    [SerializeField] private GameObject slashEffect;
    [SerializeField] private GameObject slashEffect2;
    [SerializeField] private SpriteRenderer playerSprite;

    private PlayerInput controls;
    private float timerDamage;
    private float timerAttack;

    private void Awake()
    {
        timerDamage = damageCooldown;
        timerAttack = attackCooldown;
        controls = gameObject.GetComponent<PlayerInput>();
        attackPos.localPosition = new Vector3(attackPosDistance.x, attackPosDistance.y, attackPos.position.z);
        slashEffect2.SetActive(false);
        slashEffect2.GetComponent<Animator>().SetBool("Recover", false);
        slashEffect.SetActive(false);
        slashEffect.GetComponent<Animator>().SetBool("Recover", false);
    }

    private void Update()
    {
        #region attack for sprites and ice bar : with attackCooldown
        timerAttack -= Time.deltaTime;
        if (controls.currentActionMap.FindAction("Attack").triggered && timerAttack <= 0)
        {
            switch (playerSprite.flipX)
            {
                case true:
                    slashEffect2.SetActive(true);
                    slashEffect2.GetComponent<Animator>().SetBool("Recover", true);
                    break;
                case false:
                    slashEffect.SetActive(true);
                    slashEffect.GetComponent<Animator>().SetBool("Recover", true);
                    break;
            }
            StartCoroutine(StopSlashAnim());

            GetComponent<IceBar>().AddBar(iceToAdd);
            //Debug.Log("stop dashing");
            timerAttack = attackCooldown;
        }
        #endregion

        #region attack for damages or effects : with damageCooldown
        Collider2D[] col = Physics2D.OverlapCircleAll(new Vector3(attackPos.position.x, attackPos.position.y, attackPos.position.z), attackRange);

        //Debug.Log(col.Length);

        for (int i = 0; i < col.Length; i++)
        {
            // Debug.Log(col[i].gameObject.name);
            timerDamage -= Time.deltaTime;
            if (col[i].gameObject.CompareTag("Enemy"))
            {
                int j = Random.Range(0, 150);

                if (j == 1)
                {
                    Debug.Log("dash");
                    col[i].gameObject.GetComponent<EnemyMove>().StartCoroutine("Dash", 3);
                }
                if (controls.currentActionMap.FindAction("Attack").triggered && timerDamage <= 0)
                {
                    Debug.Log("hit Enemy");
                    col[i].gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);
                    timerDamage = damageCooldown;
                }
            }

            if (col[i].gameObject.CompareTag("Door") && controls.currentActionMap.FindAction("Attack").triggered && timerDamage <= 0)
            {
                col[i].gameObject.GetComponent<Door>().DestroyDoor();
                timerDamage = damageCooldown;
            }
            if (col[i].gameObject.CompareTag("FuzeBox") && controls.currentActionMap.FindAction("Attack").triggered && timerDamage <= 0)
            {
                col[i].gameObject.GetComponent<FuzeBox>().DestoyFuze();
                timerDamage = damageCooldown;
            }
        }
        #endregion

        #region modifications en fonction de la direction du joueur
        float xAxis = controls.currentActionMap.FindAction("Move").ReadValue<float>();

        if (xAxis > 0)
        {
            attackPos.localPosition = new Vector3(attackPosDistance.x, attackPosDistance.y, attackPos.position.z);

            slashEffect2.SetActive(false);
            slashEffect2.GetComponent<Animator>().SetBool("Recover", false);
        }
        if (xAxis < 0)
        {
            attackPos.localPosition = new Vector3(-attackPosDistance.x, attackPosDistance.y, attackPos.position.z);

            slashEffect.SetActive(false);
            slashEffect.GetComponent<Animator>().SetBool("Recover", false);
        }
        #endregion
    }

    /// <summary>
    /// feedback visuel de debug pour le rayon d'attaque
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(new Vector3(attackPos.position.x, attackPos.position.y, attackPos.position.z), attackRange);
    }

    /// <summary>
    /// permet d'arr�ter l'animation de slash
    /// </summary>
    /// <returns></returns>
    IEnumerator StopSlashAnim()
    {
        yield return new WaitForSeconds(1);

        switch (playerSprite.flipX)
        {
            case true:
                slashEffect2.SetActive(false);
                slashEffect2.GetComponent<Animator>().SetBool("Recover", false);
                break;
            case false:
                slashEffect.SetActive(false);
                slashEffect.GetComponent<Animator>().SetBool("Recover", false);
                break;
        }

        yield break;
    }
}
