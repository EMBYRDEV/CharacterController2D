using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
sealed public class CharacterController2D : MonoBehaviour {
	// Properties
	public float skinWidth = 0.01f;
	public float minMoveDistance = 0.001f;

	public float slopeLimit = 30f;
	public Vector2 groundNormal {get; private set;}
	private Vector2 maxSlopeNormal;

	public LayerMask blockerMask;
	public string oneWayPlatformTag = "One Way Platform";
	
	public bool ignoreOneWayPlatforms = false;
	public bool ignoreOneWayPlatformsOnce = false;

	// Flags
	public bool isGrounded;
	public bool isOnCeiling;
	public bool isOnOneWayPlatform; 

	// References
	private Rigidbody2D rb;

	// Misc
	[HideInInspector][System.NonSerialized]
	public ContactFilter2D contactFilter;

	void Awake() {
		rb = GetComponent<Rigidbody2D>();
		
		contactFilter.useTriggers = false;
		contactFilter.SetLayerMask(blockerMask);
		contactFilter.useLayerMask = true;
		
		maxSlopeNormal = Quaternion.AngleAxis(slopeLimit, Vector3.forward) * Vector3.up;
	}
	
	void Reset(){
		rb = GetComponent<Rigidbody2D>();

		rb.bodyType = RigidbodyType2D.Kinematic;
		rb.useFullKinematicContacts = true;
		rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		rb.interpolation = RigidbodyInterpolation2D.Interpolate;
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;

		blockerMask = Physics2D.GetLayerCollisionMask(gameObject.layer);
	}

	// Should be called in FixedUpdate.
	public void Move(Vector2 moveVel, bool slideAlongGroundNormal = false) {
		isGrounded = false;
		isOnCeiling = false;
		isOnOneWayPlatform = false;
		
		Vector2 xMoveVel = Vector2.Scale(moveVel, Vector2.right);
		Vector2 yMoveVel = Vector2.Scale(moveVel, Vector2.up);
		
		if(slideAlongGroundNormal){
			xMoveVel = new Vector2(groundNormal.y, -groundNormal.x) * moveVel.x;
		}
		
		DoMovement(xMoveVel, true);	
		DoMovement(yMoveVel, false);
	}
	
	
	private RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
	
	private void DoMovement(Vector2 moveVel, bool xMovement = true) {
		float distance = moveVel.magnitude;
		if (distance > minMoveDistance) {
			int count = rb.Cast(moveVel.normalized, contactFilter, hitBuffer, distance + skinWidth);
	
			for (int i = 0; i < count; i++) {
				bool oneWayPlatform = hitBuffer[i].transform.tag == oneWayPlatformTag;
				
				Vector2 currentNormal = hitBuffer[i].normal;
				
				if(!oneWayPlatform){
					if (currentNormal.y >= maxSlopeNormal.y) {
						isGrounded = true;
						groundNormal = currentNormal;
					} else if (currentNormal.y <= -maxSlopeNormal.y) {
						isOnCeiling = true;
					}

					float modifiedDistance = hitBuffer[i].distance - skinWidth;
					distance = modifiedDistance < distance ? modifiedDistance : distance;
				}else{
					if(ignoreOneWayPlatforms || ignoreOneWayPlatformsOnce){
						ignoreOneWayPlatformsOnce = false;
						continue;
					}
					
					if (currentNormal.y >= maxSlopeNormal.y && moveVel.y <= 0 && hitBuffer[0].distance > 0) {
						isGrounded = true;
						groundNormal = currentNormal;
					}
					
					if(isGrounded && moveVel.x == 0){
						isOnOneWayPlatform = true;
						float modifiedDistance = hitBuffer[i].distance - skinWidth;
						distance = modifiedDistance < distance ? modifiedDistance : distance;
					}
				}
			}
		}

		rb.position = rb.position + moveVel.normalized * distance;
	}
}