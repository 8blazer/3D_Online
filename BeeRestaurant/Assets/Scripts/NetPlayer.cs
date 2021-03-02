﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetPlayer : NetworkBehaviour
{
    [SerializeField] private Renderer displayerColorRenderer = null;

    [SyncVar(hook = nameof(HandleDisplayColorUpdated))]
    [SerializeField]
    private Color playerColor = new Color(1, 1, 1);
    public Color PlayerPrefColor;
    //public bool playerPrefSet;

    #region ServerLogic

    [Command]
    public void CmdSetColor(Color color)
    {
        playerColor = color;
    }
    #endregion

    #region ClientLogic

    private void HandleDisplayColorUpdated(Color oldColor, Color newColor)
    {
        displayerColorRenderer.material.SetColor("_BaseColor", newColor);
    }
    [TargetRpc]
    public void TargetGetPlayerPrefColor()
    {
        PlayerPrefColor = new Color(
    PlayerPrefs.GetFloat("PCredValue"),
    PlayerPrefs.GetFloat("PCgreenValue"),
    PlayerPrefs.GetFloat("PCblueValue")
    );
        CmdSetColor(PlayerPrefColor);
    }
    #endregion
}
