using System;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

namespace UnityStandardAssets._2D
{
    public class PlatformerCharacter2D : NetworkBehaviour
    {
        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
        public GameObject playerArm;

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        public bool m_Grounded;            // Whether or not the player is grounded.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        public Animator m_Anim;            // Reference to the player's animator component.
        public Rigidbody2D m_Rigidbody2D;
       // [SyncVar (hook = "CmdChangeDirection")]
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.
   //     [SyncVar]
        public Transform playerGraphics;
        
        private float playerHealth = 100;
        public TextMeshProUGUI healthText;
        [SyncVar (hook = "CmdChangeDirection")]
        public bool Direction;
        public GameObject PlayerCharacter;
        [SyncVar (hook = "CmdTest")]
        public Vector3 theScale;

        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            playerGraphics = transform.GetChild(3);
            Direction = m_FacingRight;
          //  healthText.text = playerHealth.ToString();
            if (playerGraphics == null)
            {
                Debug.LogError("PlayerGraphics not found!");
            }
            theScale = playerGraphics.localScale;
        }

        

        private void FixedUpdate()
        {
            if(!isLocalPlayer)
            {
                return;
            }
            m_Grounded = false;

            //if (Direction)
            //{
            //    playerGraphics.localScale = new Vector3(1.3f, 1.3f, 1);
            //}
            //else
            //{
            //    playerGraphics.localScale = new Vector3(-1.3f, 1.3f, 1);
            //}
            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                    m_Grounded = true;
            }
            m_Anim.SetBool("Ground", m_Grounded);

            // Set the vertical animation
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
            playerArm.GetComponent<ArmRotation>().Rotate();
            PlayerCharacter.GetComponent<Player>().PlayerUpdate();
        }


        public void Move(float move, bool crouch, bool jump)
        {
            if (!isLocalPlayer)
            {
                return;
            }

           
            // If crouching, check to see if the character can stand up
            if (!crouch && m_Anim.GetBool("Crouch"))
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                {
                    crouch = true;
                }
            }

            // Set whether or not the character is crouching in the animator
            m_Anim.SetBool("Crouch", crouch);

            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                // Reduce the speed if crouching by the crouchSpeed multiplier
                move = (crouch ? move*m_CrouchSpeed : move);

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(move));

                // Move the character
                m_Rigidbody2D.velocity = new Vector2(move*m_MaxSpeed, m_Rigidbody2D.velocity.y);

                Debug.Log("Move: " + move + " facing right: " + m_FacingRight + " direction: " + Direction);
                // If the input is moving the player right and the player is facing left...

                if (move > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    CmdFlip();
                }
                    // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    CmdFlip();
                }
            }
            // If the player should jump...
            if (m_Grounded && jump && m_Anim.GetBool("Ground"))
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                m_Anim.SetBool("Ground", false);
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            }
        }

        //   public override void OnStartLocalPlayer()
        // {
        // base.OnStartLocalPlayer();
        //  }

        // [ClientRpc]

        //[Command]
        public void CmdChangeDirection(bool Directi)
        {
            m_FacingRight = Directi;
            Direction = m_FacingRight;
            //if (Direction)
            //{
            //    playerGraphics.localScale = new Vector3(1.3f, 1.3f, 1);
            //}
            //else
            //{
            //    playerGraphics.localScale = new Vector3(-1.3f, 1.3f, 1);
            //}
            Debug.Log("Direcion changed to " + Direction);
        }

      // [Command]
      // [ClientRpc]
      //[Command]
        public void CmdTest(Vector3 theScale)
        {
           
            playerGraphics.localScale = theScale;
            
            Debug.Log("Scale changed to " + playerGraphics.localScale);
        }

      

        [Command]
        public void CmdFlip()
        {
            if (!isServer)
            {
                return;
            }

            m_FacingRight = !m_FacingRight;
            Direction = m_FacingRight;
            // Switch the way the player is labelled as facing.

            Vector3 Scale = playerGraphics.localScale;
            Scale.x *= -1;
            //playerGraphics.localScale = Scale;
            theScale = Scale;
            //CmdTest(theScale);
     
            
           // playerGraphics.localScale = theScale;

        }
    }

}
