using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rb;
    public float flyPw = 5.0f;
    public float acel = 0.05f;
    public float MaxFSpeed = 5.0f;

    [SerializeField]private float flySpeed;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetKey(KeyCode.F))
        {
            rb.AddForce(transform.up*flyPw);
        }*/
        //LimitGSpeed();
        AccelControll();
        if(Input.GetKey(KeyCode.F))
        {
            // if(rb.velocity.y<0)
            // {
            //     rb.velocity = new Vector2(rb.velocity.x,0);
            // }

            rb.velocity += Vector2.up*acel;
            
            if(rb.velocity.y>MaxFSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x,MaxFSpeed);
            }
        }
    }
    private void LimitGSpeed()
    {
        if(rb.velocity.y<-15.0f)
        {
            rb.velocity =new Vector2(rb.velocity.x,-15.0f);
        }
    }
    void AccelControll()
    {
        if(rb.velocity.y<0)
        {
            acel = 5.0f;
        }
        else
        {
            acel = 2.0f;
        }
    }
}
