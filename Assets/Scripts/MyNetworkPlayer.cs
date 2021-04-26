using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class MyNetworkPlayer : NetworkBehaviour
{
    // Tunables
    [SerializeField] TextMeshProUGUI displayTextField = null;
    [SerializeField] Renderer playerRenderer = null;

    // Sync'd Variables
    [SyncVar(hook = nameof(HandleDisplayNameUpdated))] [SerializeField] string displayName = "Missing Name";
    [SyncVar(hook = nameof(HandleDisplayColorUpdated))] [SerializeField] Color color = Color.white;

    #region Server

    [Server]
    public void SetDisplayName(string displayName)
    {
        HandleDisplayNameUpdated("", displayName);
    }

    [Server]
    public void SetColor(Color color)
    {
        this.color = color;
    }

    [Command]
    private void CmdSetDisplayName(string newDisplayName)
    {
        if (newDisplayName.Contains(" ") || newDisplayName.Length > 10 || newDisplayName.Length < 2) { return; }

        RpcLogNewName(newDisplayName);
        SetDisplayName(newDisplayName);
    }


    #endregion

    #region Client

    private void Awake()
    {
        displayTextField.text = displayName;
        playerRenderer.material.color = color;
    }

    private void Start()
    {
        displayTextField.gameObject.transform.forward = Camera.main.transform.forward;
    }

    private void HandleDisplayColorUpdated(Color oldColor, Color newColor)
    {
        color = newColor;
        playerRenderer.material.SetColor("_BaseColor", newColor);
    }

    private void HandleDisplayNameUpdated(string oldName, string newName)
    {
        displayName = newName;
        displayTextField.text = newName;
    }

    [ContextMenu("SetMyName")]
    private void SetMyName()
    {
        CmdSetDisplayName("My New Name");
    }

    [ClientRpc]
    private void RpcLogNewName(string newDisplayName)
    {
        UnityEngine.Debug.Log($"New Name is {newDisplayName}");
    }

    #endregion
}
