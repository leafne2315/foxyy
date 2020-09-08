using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShow : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform Player;
    public Vector3 LastDir;
    void Start()
    {
        LastDir = Vector3.up;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Player.position;
        //DirCache_mouse();
        DirCache_PS4();
    }
    void DirCache_mouse()
    {
        Vector3 mouse = Input.mousePosition;
        mouse.z = -Camera.main.transform.position.z;
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(mouse);
        transform.rotation = Quaternion.LookRotation(targetPos-transform.position,Vector3.back);
    }
    void DirCache_PS4()
    {
        Vector3 L_Joy = new Vector3(Input.GetAxis("PS4-L-Horizontal"),Input.GetAxis("PS4-L-Vertical"),0.0f);
        L_Joy = L_Joy.normalized;

        if(L_Joy!=Vector3.zero)
        {
            LastDir = L_Joy;
        }
        transform.rotation = Quaternion.LookRotation(LastDir,Vector3.back);
    }
}
