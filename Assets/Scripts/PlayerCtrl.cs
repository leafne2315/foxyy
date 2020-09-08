using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public float hp;
	public float speed;
	public float JumpForce;
	public float DoubleJumpForce;
	private float moveInput;
	private float JumpInput;
	private Rigidbody2D rb;
	private bool facingRight = true;
	private bool isGrounded;
	public float checkRadius;
	public Transform GroundCheck;
	public LayerMask WhatIsGround;
	public int extraJumps;
	public int extraJumpValue;
	private float JumpTimer;
	public float JumpTime;
	private bool isJumping;

	public float dashTimer;
	public float dashTime;
	public float dashCD;
	[SerializeField]private bool canDash = true;
	public float dashSpeed;
	public float KnockTimer;
	public bool isHit;
	public bool isGhost = false;
	public enum PlayerState{Normal,Defend,GetHit,Dash};
	public PlayerState currentState;
	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}
	void Start()
	{		
		KnockTimer = 0;
		extraJumps = extraJumpValue;
		JumpTimer = JumpTime;
		currentState = PlayerState.Normal;
	}
	void FixedUpdate()
	{
		switch(currentState)
		{
			case PlayerState.Normal:

				isGrounded = Physics2D.OverlapCircle(GroundCheck.position,checkRadius,WhatIsGround);
				rb.velocity = new Vector2(moveInput * speed , rb.velocity.y);
				checkGravity();
				
				if(Input.GetKeyDown(KeyCode.Space) && extraJumps>0 && !isGrounded)
				{
					rb.velocity = new Vector2 (moveInput*speed,DoubleJumpForce);
					extraJumps--;
				}
				
				if(Input.GetKeyDown(KeyCode.Space) && isGrounded == true)
				{
					rb.velocity =  new Vector2 (moveInput*speed,JumpForce);
				}	
								
				if(Input.GetKeyDown(KeyCode.X))
				{	
					if(canDash)
					{
						//進入Dash階段

						dashTimer = 0; //重置dash 時間
						currentState = PlayerState.Dash;
						canDash = false;

						
						rb.gravityScale = 0;
						StartCoroutine(dashCD_Count());
					} 	
				}
			break;

			case PlayerState.Dash:

				if(dashTimer<dashTime)
				{
					dashTimer+=Time.deltaTime;
					
					if(facingRight)
					{
						rb.velocity = Vector2.right*dashSpeed;
						print("dashing");
					}
					else
					{
						rb.velocity = Vector2.left*dashSpeed;
					}
				}
				else
				{
					currentState = PlayerState.Normal;
					
					rb.gravityScale = 5;
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
		
		moveInput = Input.GetAxisRaw("Horizontal");
		//Debug.Log(rb.velocity.x);
		//Debug.Log(rb.velocity.y);
		if(isGrounded == true)
		{
			extraJumps = extraJumpValue;
			JumpTimer = JumpTime;
		}

		if(facingRight==false&&moveInput>0)
		{
			Flip();
		}
		else if(facingRight == true&&moveInput<0)
		{
			Flip();
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
	IEnumerator dashCD_Count()
	{
		for(float i =0 ; i<=dashCD ; i+=Time.deltaTime)
		{

			yield return 0;
		}
		canDash = true;
	}
	private void checkGravity()
	{
		if(rb.gravityScale!=5)
		{
			rb.gravityScale = 5;
		}
	}
}
