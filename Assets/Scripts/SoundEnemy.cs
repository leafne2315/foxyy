using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEnemy : MonoBehaviour
{
    public Transform redDetection;
    public Transform yellowDetection;
    public LayerMask whatIsPlayer;
    public float yellowDetectionRadius;
    public float redDetectionRadius;
    private Collider2D yellowWarning;
    private Collider2D redWarning;
    public float yellowDetectTime;
    private PlayerCtroller PlayerCtroller; 
    void Start()
    {
        PlayerCtroller = GameObject.Find("Player").GetComponent<PlayerCtroller>();
    }

    // Update is called once per frame
    void Update()
    {
        yellowWarning = Physics2D.OverlapCircle(yellowDetection.position,yellowDetectionRadius,whatIsPlayer);
        if(yellowWarning)
            //要怎麼判斷角色有無飛行
            if(PlayerCtroller.currentState == PlayerCtroller.PlayerState.AirDash||PlayerCtroller.currentState == PlayerCtroller.PlayerState.BugFly)
                {
                    print("a");
                    yellowDetectTime += Time.deltaTime;
                    if(yellowDetectTime >=3)
                    {
                        //print("detect");
                    }
                }
                else
                {
                    if(yellowDetectTime >0)
                    {
                        yellowDetectTime -= Time.deltaTime;
                    }
                }
        
        
        redWarning = Physics2D.OverlapCircle(redDetection.position,redDetectionRadius,whatIsPlayer);
        if(redWarning)
        {
            print("detect");
        }
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(yellowDetection.position, yellowDetectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(redDetection.position, redDetectionRadius);
    }
}
