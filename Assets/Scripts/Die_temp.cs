using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die_temp : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 StartPos;
    private Rigidbody2D rb;
    void Start()
    {
        StartPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Sea"))
        {
            transform.position = StartPos;
            rb.velocity = Vector2.zero;
            GetComponent<PlayerCtroller>().currentGas = 100;
            GetComponent<PlayerCtroller>().Out_Of_Gas = false;
        }
    }
    
}
