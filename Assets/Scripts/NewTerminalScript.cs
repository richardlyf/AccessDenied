﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewTerminalScript : MonoBehaviour {

    public GameObject terminalWindowUI;
    public GameObject AI;
    Text AIText;

    private int numBodies = 2;
    public GameObject body1;
    public GameObject body2;
    private GameObject[] bodies;

    private GameObject player;

    public AudioSource mainAudio;

    private bool inRadius = false;
    public bool gameWon = false;
    public int bodyToSwitchTo;
    public bool gameLost = false;


    // Use this for initialization
    void Start () {
        FindPlayer();
        InitiateArrays();
        InitializeTerminalWindow();
    }

    void InitializeTerminalWindow()
    {
        terminalWindowUI.SetActive(false);
        
        AI.SetActive(false);
    }

    private void InitiateArrays()
    {
        bodies = new GameObject[] { body1, body2 };
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("e") && inRadius && !terminalWindowUI.activeSelf) EnterTerminal();
        if (terminalWindowUI.activeSelf)
        {
            if (gameWon) HandleWin();
            if (gameLost) HandleLoss();
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRadius = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRadius = false;
        }
    }

    private void EnterTerminal()
    {
        FindPlayer();
        Debug.Log(player.gameObject.name);
        terminalWindowUI.SetActive(true);
        FindObjectOfType<Minigame>().Start();
        FindObjectOfType<Minigame>().ResetTerminal();
        player.GetComponent<PlayerControl>().enabled = false;
        FindObjectOfType<miniGameMusic>().playAudio();
        mainAudio.Pause();
    }

    public void ExitTerminal()
    {
        terminalWindowUI.SetActive(false);
        FindObjectOfType<miniGameMusic>().stopAudio();
        print("audio turned off???");
        mainAudio.Play();
    }

    void HandleWin()
    {
        //if(GetPlayerIndex() == 0) SwitchBody(1);
        //else SwitchBody(0);
        SwitchBody(bodyToSwitchTo);

        //terminalWindowUI.SetActive(false);
        ExitTerminal();
        gameWon = false;
    }

    void HandleLoss()
    {
        terminalWindowUI.SetActive(false);
        AI.SetActive(true);
        FindObjectOfType<miniGameMusic>().stopAudio();
        GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sound/AI/endgame short"), 6);
        StartCoroutine(AIDiscovery());
    }

    public IEnumerator AIDiscovery()
    {
        AIText = AI.GetComponentInChildren<Text>();
        AIText.text = "Oh.";
        yield return new WaitForSeconds(1f);

        //ChangeButtonsText("Who are you?");
        AIText.text = "Who are you?";
        yield return new WaitForSeconds(1f);

        //ChangeButtonsText("You shouldn't be here.");
        AIText.text = "You shouldn't be here.";
        yield return new WaitForSeconds(2f);

        AIText.text = "I'll just wipe you from the hard drive.";
        yield return new WaitForSeconds(1.5f);

        Color red = new Color(1f, 0f, 0f);
        AIText.color = red;
        AIText.text = "ACCESS DENIED";

        FindObjectOfType<GameManager>().EndGame();
        //backgroundFriendly.GetComponent<CanvasRenderer>().SetColor(red);
        //backgroundFriendly.GetComponent<CanvasRenderer>().SetAlpha(1f);

    }

    void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    int GetPlayerIndex()
    {
        for(int i = 0; i < numBodies; i++)
        {
            if (bodies[i].tag == "Player") return i;
        }

        return -1;
    }

    public void SwitchBody(int body)
    {
        //FindPlayer();
        player.GetComponent<PlayerControl>().enabled = false;
        player.tag = "Untagged";

        bodies[body].tag = "Player";
        bodies[body].GetComponent<PlayerControl>().enabled = true;

        FindObjectOfType<CameraPos>().checkPlayer = true;
        FindObjectOfType<DoorSensorPos>().checkPlayer = true;
        FindObjectOfType<EGGTerminal>().checkPlayer = true;
        GameObject[] workers = GameObject.FindGameObjectsWithTag("worker");
        foreach (GameObject worker in workers)
        {
            worker.GetComponent<Worker>().locateTarget = true;
        }

        FindPlayer();
        inRadius = false;
    }
}
