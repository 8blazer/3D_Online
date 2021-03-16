using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetPlayer : NetworkBehaviour
{
    [SerializeField] private Renderer[] displayerColorRenderers = new Renderer[3];

    [SyncVar(hook = nameof(HandleDisplayColorUpdated))]
    [SerializeField]
    private Color playerColor = new Color(1, 1, 1);
    public Color PlayerPrefColor;
    GameObject netManager;

    /*
    private void Start()
    {
        netManager = GameObject.Find("NetManager");
        if (netManager == null)
        {
            netManager = GameObject.Find("NetManager 1");
        }
        netManager.GetComponent<NetSync>().RefreshHeirarachy();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            netManager.GetComponent<NetSync>().RefreshHeirarachy();
        }
    }
    */

    #region ServerLogic

    [Command]
    private void CmdSetCustomization(Color color, int hatNumber)
    {
        playerColor = color;
        GameObject hat = Instantiate(HatList.hatListReference.hatList[hatNumber], gameObject.transform.Find("HatPosition 1"));
        NetworkServer.Spawn(hat);
    }
    #endregion

    #region ClientLogic

    private void HandleDisplayColorUpdated(Color oldColor, Color newColor)
    {
        foreach(Renderer rend in displayerColorRenderers)
        {
            rend.material.SetColor("_BaseColor", newColor);
        }

    }
    [TargetRpc]
    public void TargetGetPlayerCustomization()
    {
        PlayerPrefColor = new Color(
    PlayerPrefs.GetFloat("PCredValue"),
    PlayerPrefs.GetFloat("PCgreenValue"),
    PlayerPrefs.GetFloat("PCblueValue")
    );
        CmdSetCustomization(PlayerPrefColor, PlayerPrefs.GetInt("HatNumber"));
    }
    #endregion
}
