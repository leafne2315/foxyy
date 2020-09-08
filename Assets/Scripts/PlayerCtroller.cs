using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PlayerCtroller : MonoBehaviour {
	public GameObject Arrow;
	public Vector2 FlyDir;
	public float hp;
	public float speed;
	private float moveInput_X;
	private float moveInput_Y;
	private float JumpInput;
	private Rigidbody2D rb;
	private bool facingRight = true;
	//偵測
	private bool isGrounded;
	private bool isAttachWall;
	public float checkRadius;
	public Transform GroundCheck;
	public Transform FrontCheck;
	public Transform UpCheck;
	public LayerMask WhatIsGround;
	public LayerMask WhatIsWall;
	//
	//跳躍
	public float JumpForce;
	public float DoubleJumpForce;
	public int extraJumps;
	public int extraJumpValue;
	private float JumpTimer;
	public float JumpTime;
	private bool isJumping;
	//
	//飛行
	public float flyPw;
    public float acel;
    public float MaxFSpeed;
    [SerializeField]private float flySpeed;
	//
	//空中衝刺
	public float AirDashTime;
	[SerializeField]private bool isAirDash;
	public float AirDashSpeed;
	public float AirDashCD;
	[SerializeField]private bool canAirDash = true;
	//
	//衝刺
	[SerializeField]private Vector2 DashDir;
	public float Charge_MaxTime;
	public float Dash_PreTime;
	public float dashTimer;
	public float dashTime;
	public float dashCD;
	public float holdingTime = 0.2f;
	[SerializeField]private bool isDash;
	private bool canDash;
	[SerializeField]private float dashSpeed;
	public float Max_dashSpeed;
	//
	//被攻擊
	public float KnockTimer;
	public bool isHit;
	public bool isGhost = false;
	//
	//狀態控制
	public enum PlayerState{Normal,Defend,GetHit,Dash,Attach,BugFly,AirDash};
	public PlayerState currentState;
	//
	private float OriginGravity;
	private float StableValue;
	//
	//燃料
	public float currentGas;
	public float Gas_MaxValue;
	public bool Out_Of_Gas;
	private bool RestoreGas_isOver = true;
	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		OriginGravity = rb.gravityScale;
	}
	void Start()
	{		
		KnockTimer = 0;
		extraJumps = extraJumpValue;
		JumpTimer = JumpTime;
		currentState = PlayerState.Normal;
		currentGas = Gas_MaxValue;
		Arrow.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
	}
	void FixedUpdate()
	{
		isGrounded = Physics2D.OverlapCircle(GroundCheck.position,checkRadius,WhatIsGround);
		isAttachWall = Physics2D.OverlapCircle(FrontCheck.position,0.05f,WhatIsWall)||Physics2D.OverlapCircle(UpCheck.position,0.05f,WhatIsWall);
		
		switch(currentState)
		{
			case PlayerState.Normal:
				
				CheckStability();
				rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x,moveInput_X * speed,StableValue) , rb.velocity.y);
				flySpeed = rb.velocity.y;
				checkGravity();
				AccelControll();
				LimitGSpeed();
				/*
				if(Input.GetKey(KeyCode.W)||Input.GetButton("PS4-R2")&&!Out_Of_Gas)
				{
					rb.velocity += Vector2.up*acel;
					if(rb.velocity.y>MaxFSpeed)
					{
						rb.velocity = new Vector2(rb.velocity.x,MaxFSpeed);
					}
					GasUse(0);
				}
				*/
				if(Input.GetKeyDown(KeyCode.Space)||Input.GetButtonDown("PS4-x") && isGrounded == true)
				{
					rb.velocity =  new Vector2 (moveInput_X*speed,JumpForce);
				}	
				
				if(isAttachWall&&!isGrounded)
				{

					currentState = PlayerState.Attach;
					rb.gravityScale = 0;
					rb.velocity = Vector2.zero;
	
				}

				if(Input.GetMouseButtonDown(0)||Input.GetButtonDown("PS4-Triangle")&&!Out_Of_Gas&&(isAttachWall||isGrounded))//->Dash
				{	
					
					dashTimer = 0; //重置dash 時間
					dashSpeed = 0;
					currentState = PlayerState.Dash;
	
					// rb.gravityScale = 0;
					rb.velocity = Vector2.down*1.0f;
					//StartCoroutine(dashCD_Count());
					Arrow.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
					Arrow.GetComponent<ArrowShow>().LastDir = Vector3.up;
				}

				if(Input.GetKeyDown(KeyCode.C)||Input.GetButtonDown("PS4-L1"))
				{
					currentState = PlayerState.BugFly;
					FlyDir = rb.velocity.normalized;
				}
			break;

			case PlayerState.Attach:

				//isAttachWall = Physics2D.OverlapCircle(FrontCheck.position,0.05f,WhatIsWall);
				RestoreGas();

				if(Input.GetKeyUp(KeyCode.Q)||Input.GetButtonUp("PS4-L2"))//->Normal
				{
					ResetGravity();
					currentState = PlayerState.Normal;
				}
				if(Input.GetMouseButtonDown(0)||Input.GetButtonDown("PS4-Triangle"))//->Dash
				{	
				
					dashSpeed = 0;
					currentState = PlayerState.Dash;
					//rb.gravityScale = 2;
					rb.velocity = Vector2.zero;
					//StartCoroutine(dashCD_Count());
					Arrow.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
					Arrow.GetComponent<ArrowShow>().LastDir = Vector3.up;	
				}
				if(Input.GetKeyDown(KeyCode.C)||Input.GetButtonDown("PS4-L1")&&!Out_Of_Gas)
				{
					currentState = PlayerState.BugFly;
					FlyDir = rb.velocity.normalized;
				}

			break;

			case PlayerState.BugFly:

				FlyMovement();
				if(Input.GetButtonDown("PS4-Triangle")&&canAirDash)
				{
					isAirDash = true;
					currentState = PlayerState.AirDash;
					StartCoroutine(AirDash_Count());
					FlyDir = new Vector2(moveInput_X,moveInput_Y);
					canAirDash = false;
				}
				if(Input.GetKeyDown(KeyCode.V)||Input.GetButtonDown("PS4-L1"))
				{
					currentState = PlayerState.Normal;
					rb.gravityScale = OriginGravity;
					//FlyDir = Vector2.zero;
				}
				if(Out_Of_Gas)
				{
					currentState = PlayerState.Normal;
				}

			break;

			case PlayerState.AirDash:

				if(isAirDash)
				{
					rb.velocity = FlyDir.normalized*AirDashSpeed;
				}
				else
				{
					currentState = PlayerState.BugFly;
				}

			break;

			case PlayerState.Dash:

				if(!isDash)
				{
					GasUse(40);
					if(dashSpeed<Max_dashSpeed)
					{
						dashSpeed+=1.0f;
					}
					else
					{
						dashSpeed = Max_dashSpeed;
					}

					dashTimer+=Time.deltaTime;

					if(Input.GetMouseButtonUp(0)||Input.GetButtonUp("PS4-Triangle")||dashTimer>Charge_MaxTime||Out_Of_Gas)
					{
						if(dashTimer<Dash_PreTime)//->Normal
						{
							currentState = PlayerState.Normal;
							ResetGravity();	
						}
						else
						{
							isDash = true;
						
							//Mouse_DirCache();
							Joysticks_DirCache();
						}

						dashTimer = 0;
						Arrow.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
					}
				}
				else
				{
					if(dashTimer<dashTime)
					{
						dashTimer+=Time.deltaTime;
						
						rb.velocity = DashDir*dashSpeed;

						if(DashDir.x>0 &&!facingRight)
						{
							Flip();
						}
						else if(DashDir.x<0 && facingRight)
						{
							Flip();
						}

						if((isAttachWall||isGrounded) && dashTimer>0.1f)//撞牆
						{
							rb.velocity = Vector2.zero;

							dashTimer = 0; //重置dash 時間
							currentState = PlayerState.Normal;
							isDash = false;
							ResetGravity();
						}
							
					}
					else if(dashTimer<dashTime+holdingTime)
					{
						dashTimer+=Time.deltaTime;
						rb.velocity =Vector2.Lerp(rb.velocity,Vector2.zero,0.1f);
						ResetGravity();
						if(isAttachWall)
						{
							rb.velocity = Vector2.zero;

							dashTimer = 0; //重置dash 時間
							currentState = PlayerState.Normal;
							isDash = false;
							ResetGravity();
						}		
					}
					else
					{
						dashTimer = 0; //重置dash 時間
						currentState = PlayerState.Normal;
						isDash = false;
						
						ResetGravity();			
					}
				}
			break;

			case PlayerState.GetHit:

				if(facingRight)
					KnockBack(20.0f,0.1f,new Vector2(-Mathf.Cos(30*Mathf.Deg2Rad),Mathf.Sin(30*Mathf.Deg2Rad)));
				else
				{
					KnockBack(20.0f,0.1f,new Vector2(Mathf.Cos(30*Mathf.Deg2Rad),Mathf.Sin(30*Mathf.Deg2Rad)));
				}

			break;
		}	
		
	}
	void Update()
	{
		CheckStability();
		moveInput_X = Input.GetAxis("PS4-L-Horizontal");
		moveInput_Y = Input.GetAxis("PS4-L-Vertical");
		//Debug.Log(rb.velocity.x);
		//Debug.Log(rb.velocity.y);
		if(isGrounded == true)
		{
			extraJumps = extraJumpValue;
			JumpTimer = JumpTime;

			if(currentState!=PlayerState.Dash)
			{	
				RestoreGas();
			}
			// if(RestoreGas_isOver)
			// {
			// 	StartCoroutine(GasRestore());
			// 	RestoreGas_isOver = false;
			// }
		}

		if(!isAttachWall)
		{
			if(facingRight==false&&moveInput_X>0)
			{
				Flip();
			}
			else if(facingRight == true&&moveInput_X<0)
			{
				Flip();
			}
		}

	}
	void Flip()
	{
		facingRight = !facingRight;
		Vector3 Scaler = transform.localScale;
		Scaler.x*=-1;
		transform.localScale = Scaler;
	}
	public void KnockBack(float KnockPwr,float KnockDur,Vector2 KnockDir)
	{
		if(KnockTimer<KnockDur)
		{
			KnockTimer+=Time.deltaTime;
			rb.velocity = KnockDir*KnockPwr;
		}
		else
		{
			KnockTimer = 0;
			currentState = PlayerState.Normal;
		}		
	}
	IEnumerator AirDash_Count()
	{
		for(float i =0 ; i<=AirDashTime ; i+=Time.deltaTime)
		{
			yield return 0;
		}
		isAirDash = false;

		for(float i =0 ; i<=(AirDashCD-AirDashTime) ; i+=Time.deltaTime)
		{
			yield return 0;
		}
		canAirDash = true;
	}
	/*
	IEnumerator dashCD_Count()
	{
		for(float i =0 ; i<=dashCD ; i+=Time.deltaTime)
		{

			yield return 0;
		}
		canDash = true;
	}
	*/
	private void checkGravity()
	{
		if(rb.gravityScale!=OriginGravity)
		{
			rb.gravityScale = OriginGravity;
		}
	}
	private void ResetGravity()
	{
		rb.gravityScale = OriginGravity;
	}
	private void LimitGSpeed()
    {
        if(rb.velocity.y<-60.0f)
        {
            rb.velocity =new Vector2(rb.velocity.x,-60.0f);
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
            acel = 1.2f;
        }
    }
	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(GroundCheck.position,checkRadius);
	}
	private void CheckStability()
	{
		
		if(isGrounded)
		{
			StableValue = 0.9f;
		}
		else
		{
			StableValue = 0.1f;
		}
	}
	void GasUse(float comsumePS)
	{
		if(currentGas>0)
			currentGas-=comsumePS*Time.deltaTime;
		else
		{
			Out_Of_Gas = true;
		}
	}
	/*IEnumerator GasRestore()
	{
		yield return new WaitForSeconds(0.1f);

		if(currentGas<Gas_MaxValue)
		{
			currentGas+=20*0.1f;

			if(Out_Of_Gas)
				Out_Of_Gas = false;	
		}
		else
		{
			currentGas = Gas_MaxValue;
		}
		RestoreGas_isOver = true;
		
	}*/
	void RestoreGas()
	{
		if(currentGas<Gas_MaxValue)
		{
			currentGas+=60*Time.deltaTime;

			if(Out_Of_Gas)
				Out_Of_Gas = false;	
		}
		else
		{
			currentGas = Gas_MaxValue;
		}
	}
	void Joysticks_DirCache()
	{
		//Vector3 L_Joy = new Vector3(Input.GetAxis("PS4-L-Horizontal"),Input.GetAxis("PS4-L-Vertical"),0.0f);
		DashDir = Arrow.GetComponent<ArrowShow>().LastDir;
	}
	void Mouse_DirCache()
	{
		Vector3 mousePos = Input.mousePosition;
		mousePos.z = -Camera.main.transform.position.z;
		Vector3 targetPos = Camera.main.ScreenToWorldPoint(mousePos);
		Vector3 myPos = new Vector2(transform.position.x,transform.position.y);
		DashDir = (targetPos-myPos).normalized;
	}
	void FlyMovement()
	{
		rb.gravityScale = 0;
		FlyDir = Vector2.Lerp(FlyDir,new Vector2(moveInput_X,moveInput_Y),0.05f);
		if(Input.GetButton("PS4-R2"))
		{
			rb.velocity = FlyDir*30;
			GasUse(15);
		}
		else
		{
			rb.velocity = FlyDir*20;
			GasUse(5);
		}
		
	}
}
