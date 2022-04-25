using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Dialogue : MonoBehaviour
{
    [SerializeField] GameObject player;

    public int index = 0;
    [SerializeField] float speed;

    float length;

    [SerializeField] List<string> names = new List<string>();
    [SerializeField] List<string> repliques = new List<string>();

    GameObject dialBox;
    TextMeshProUGUI dialText;
    TextMeshProUGUI dialName;

    public bool isTalking;

    private PlayerInput controls = null;

    public static Dialogue Instance = null;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        controls = player.GetComponent<PlayerInput>();

    }
    private void Start()
    {
        StartDial();
        if (dialBox == null)
        {
            dialBox = GameObject.Find("DialBox");
        }
        if (dialName == null)
        {
            dialName = GameObject.Find("Name").GetComponent<TextMeshProUGUI>();
        }
        if (dialText == null)
        {
            dialText = GameObject.Find("Dialog").GetComponent<TextMeshProUGUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isTalking == true)
        {
            length += Time.deltaTime * speed;
            length = Mathf.Clamp(length, 0, repliques[index].Length);
            dialText.text = repliques[index].Substring(0, (int)length);
            dialName.text = names[index];

            if (controls.currentActionMap.FindAction("Interact").triggered)
            {
                NextReplique();
            }
        }
    }

    void NextReplique()
    {
        if (length < repliques[index].Length)
        {
            length = repliques[index].Length;
        }
        else
        {
            index++;
            NewReplique();
        }
    }
    public void StartDial()
    {
        if (isTalking == false)
        {
            player.GetComponent<PlayerControllerV2>().enabled = false;
            player.GetComponent<PlayerAttack>().enabled = false;
            index = 0;
            isTalking = true;
        }
    }
    void NewReplique()
    {
        if (index < repliques.Count)
        {
            length = 0;
            dialName.text = names[index];
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        index = 0;
        length = 0;
        isTalking = false;
        dialBox.SetActive(false);
        player.GetComponent<PlayerControllerV2>().enabled = true;
        player.GetComponent<PlayerAttack>().enabled = true;
    }
}
