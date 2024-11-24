using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Gate : Interactable
{
    public BoxCollider boxCollider;
    private void Start()
    {
        boxCollider.GetComponentInChildren<BoxCollider>();
    }
    public void GameWon()
    {
        GameOverScreen.instance.StartCoroutine(GameOverScreen.instance.GameWon());
        PlayerManager.instance.dead = true;
        PlayerManager.instance.TurnOnOrOffInteractable(false);
        PlayerManager.instance.middlePoint.enabled = false;
        GameObject player = PlayerManager.instance.GetPlayer();
        PlayerMovement playerMovementScript = player.GetComponent<PlayerMovement>();
        playerMovementScript.controller.Move(Vector3.zero);
        playerMovementScript.enabled = false;
        MouseLook mouseLookScript = player.GetComponentInChildren<MouseLook>();
        if (mouseLookScript != null) mouseLookScript.enabled = false;
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.rb.isKinematic = true;
    }
}