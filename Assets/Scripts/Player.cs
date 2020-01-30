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
	private float currentLifeSpan;
	private Vector2 direction; //x,z
	private List<GameObject> ladList = new List<GameObject>();

	// Start is called before the first frame update
	void Start() {
		currentLifeSpan = lifeSpan;
	}

	// Update is called once per frame
	void Update() {
		//Lifespan
		currentLifeSpan -= Time.deltaTime;
		if ( currentLifeSpan <= 0 && ladList.Count > 0 ) {
			GameObject ladToKill = ladList[ ladList.Count - 1 ];
			ladList.Remove( ladToKill );
			Destroy( ladToKill );
			boxCollider.size -= new Vector3( 0, 1, 0 );
			boxCollider.center = new Vector3( 0, 0.5f * ladList.Count, 0 );

			rigidBody.position += new Vector3( 0, 1, 0 );
			currentLifeSpan = lifeSpan;
		}
		else if ( currentLifeSpan <= 0 && ladList.Count <= 0 ) {
			Time.timeScale = 0;
		}

		//Controls
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
		rigidBody.velocity = new Vector3( direction.x * speed, rigidBody.velocity.y, direction.y * speed );
	}

	private void OnTriggerEnter( Collider other ) {
		if ( other.gameObject.tag == "Lad" ) {
			ladList.Add( other.gameObject );
			Rigidbody ladRigidBody = other.gameObject.GetComponent<Rigidbody>();
			ladRigidBody.isKinematic = true;
			ladRigidBody.useGravity = false;
			other.gameObject.GetComponent<BoxCollider>().enabled = false;
			boxCollider.size += new Vector3( 0, 1, 0 );
			boxCollider.center = new Vector3( 0, 0.5f * ladList.Count, 0 );
			other.gameObject.transform.parent = this.transform;
			other.gameObject.transform.localPosition = new Vector3( 0, ladList.Count, 0 );
		}
	}
}
