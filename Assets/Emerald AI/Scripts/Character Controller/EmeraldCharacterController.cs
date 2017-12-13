//This script has been heavily modified compared to the original version of the script found here: http://wiki.unity3d.com/index.php?title=RigidbodyFPSWalker
//A run system has been added to allow for running. This running system is based on the player's stamina allowing the player to only sprint while they have stamina 
//Proper collision detection has also been added so user cannot continue to jump on and over objects.
//The script has also been made more efficient by calling GetComponent on start, storing it in a variable, then calling that variable throught the script.
//This script is based on the license CC BY-SA 3.0

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]

public class EmeraldCharacterController : MonoBehaviour {
	
	public float walkSpeed = 6.0f;
	public float runSpeed = 12.0f;
	public float gravity = 10.0f;
	public float maxVelocityChange = 10.0f;
	public bool canJump = true;
	public float jumpHeight = 2.0f;
	public bool onlyJumpOnUntagged = true;
	public Slider staminaBar;

	public AudioClip footStepSound;
	public float runFootStepSeconds;
	public float walkFootStepSeconds;

	private float footStepTimer;
	private float stamina = 1;
	private AudioSource audioSource;
	private bool grounded = false;	
	private float rayDistance;
	private RaycastHit hit;
	private Rigidbody rb;

	private Vector3 velocity;
	private Vector3 velocityChange;

	void Awake () 
	{
		audioSource = GetComponent<AudioSource>();
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;
		rb.useGravity = false;

		if (staminaBar == null)
		{
			GameObject SB = GameObject.Find("StaminaBar");
			if (SB != null){
				staminaBar = SB.GetComponent<Slider>();
			}
		}
	}

	void Update ()
	{
		if (staminaBar != null)
		{
			staminaBar.value = stamina;
		}

		if (stamina >= 1)
		{
			stamina = 1;
		}

		if (stamina <= 0)
		{
			stamina = 0;
		}
	}
	
	void FixedUpdate () 
	{
		if (grounded) 
		{
			// Calculate how fast we should be moving while running
			if (Input.GetKey(KeyCode.LeftShift) && stamina > 0.015f)
			{
				Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
				targetVelocity = transform.TransformDirection(targetVelocity);
				targetVelocity *= runSpeed;
				
				// Apply a force that attempts to reach our target velocity
				velocity = rb.velocity;
				velocityChange = (targetVelocity - velocity);
				velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
				velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
				velocityChange.y = 0;
				rb.AddForce(velocityChange, ForceMode.VelocityChange);

				if (Input.GetKey(KeyCode.W))
				{
					footStepTimer += Time.deltaTime;
					stamina -= Time.deltaTime * 0.1f;
				}
				
				if (footStepTimer >= runFootStepSeconds && audioSource != null)
				{
					audioSource.pitch = Random.Range(0.9f, 1.1f);
					audioSource.PlayOneShot(footStepSound);
					footStepTimer = 0;
				}
			}

			// Calculate how fast we should be moving while walking
			if (!Input.GetKey(KeyCode.LeftShift))
			{
				Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
				targetVelocity = transform.TransformDirection(targetVelocity);
				targetVelocity *= walkSpeed;
				
				// Apply a force that attempts to reach our target velocity
				velocity = rb.velocity;
				velocityChange = (targetVelocity - velocity);
				velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
				velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
				velocityChange.y = 0;
				rb.AddForce(velocityChange, ForceMode.VelocityChange);

				stamina += Time.deltaTime * 0.045f;

				if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
				{
					footStepTimer += Time.deltaTime;
					
					if (footStepTimer >= walkFootStepSeconds && audioSource != null)
					{
						audioSource.pitch = Random.Range(0.9f, 1.1f);
						audioSource.PlayOneShot(footStepSound);
						footStepTimer = 0;
					}
				}
			}

			// Jump
			if (canJump && Input.GetButton("Jump")) 
			{
				rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalwalkSpeed(), velocity.z);
			}
		}
		
		// We apply gravity manually for more tuning control
		rb.AddForce(new Vector3 (0, -gravity * rb.mass, 0));
		
		grounded = false;
	}

	/*
	void OnCollisionStay () 
	{
		grounded = true;    
	}
	*/

	void OnCollisionStay (Collision col) 
	{
		if (col.gameObject.tag == "Untagged" || !onlyJumpOnUntagged)
		{
			grounded = true; 
		}
	}
	
	float CalculateJumpVerticalwalkSpeed () 
	{
		// From the jump height and gravity we deduce the upwards walkSpeed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}
}