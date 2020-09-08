using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GasGasGas : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerCtroller playerCtrScript;
    private Image GasBar;
    public GameObject GasUI;
    public GameObject Player;
    void Start()
    {
        GasBar = GetComponent<Image>();
        playerCtrScript = Player.GetComponent<PlayerCtroller>();
    }
    private void LateUpdate()
    {
        GasBar.fillAmount = playerCtrScript.currentGas/playerCtrScript.Gas_MaxValue;
    }

    // Update is called once per frame
}
