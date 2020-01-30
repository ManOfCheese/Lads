using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	[Header("References")]
	public Rigidbody rigidBody;
	public BoxCollider boxCollider;

	[Header( "Settings" )]
	public float speed;
	public float lifeSpan; //in seconds

	[Header( "Data" )]
	private Vector2 direction; //x,z
	private int ladCount;

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		lifeSpan -= Time.deltaTime;

		direction = new Vector2( 0, 0 );
		if ( Input.GetKey( KeyCode.W ) ) {
			direction = new Vector2( direction.x, 1 );
		}
		if ( Input.GetKey( KeyCode.A ) ) {
			direction = new Vector2( -1, direction.y );
		}
		if ( Input.GetKey( KeyCode.S ) ) {
			direction = new Vector2( direction.x, -1 );
		}
		if ( Input.GetKey( KeyCode.D ) ) {
			direction = new Vector2( 1, direction.y );
		}
		rigidBody.velocity = new Vector3( direction.x * speed, 0, direction.y * speed );
	}
}
