using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewDetect : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform OriginPos;
    private Vector2 RayDir;
    private Vector2 RayDir_02;
    public float RayLength = 100;
    public LayerMask TargetLayer;
    public float DetectAngle;
    [Range(1, 50)]public float accuracy;
    [SerializeField]private float Ray_RotatePerSecond;
    private float currentAngle;
    private PlayerCtroller PlayerCtroller; 
    public LayerMask ignoreYourself;
    void Start()
    {
        PlayerCtroller = GameObject.Find("Player").GetComponent<PlayerCtroller>();
    }

    // Update is called once per frame
    void Update()
    
    {
        float subAngle = DetectAngle / accuracy;
        for(int i=0;i<accuracy;i++)
        {
            RayDir = PlayerCtroller.FlyDir.normalized;
            RayDir = Quaternion.AngleAxis(-DetectAngle/2 + Mathf.Repeat(Ray_RotatePerSecond*Time.time + i*subAngle,DetectAngle),Vector3.back)*RayDir;
            RaycastHit2D hit = Physics2D.Raycast(OriginPos.position,RayDir,RayLength, ~ignoreYourself);
            
            
            if(hit.collider)
            {
                Debug.DrawLine(OriginPos.position,hit.point,Color.red);
                //Debug.Log(hit.collider.name);
            }
            else
            {
                Debug.DrawLine(OriginPos.position,OriginPos.position+new Vector3(RayDir.x,RayDir.y,0)*RayLength,Color.red);
            }
        }
        //currentAngle += Ray_RotatePerSecond*Time.deltaTime;
        
        

        
    }
    
}
