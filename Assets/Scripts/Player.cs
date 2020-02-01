using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	[Header( "References" )]
	public GameObject deadLad;
	public Rigidbody rigidBody;
	public BoxCollider boxCollider;
	public Text highScoreText;

	[Header( "Settings" )]
	public float speed;
	public float throwSpeed;
	public float lifeSpan; //in seconds

	[Header( "Data" )]
	private float currentLifeSpan;
	private Vector3 direction;
	private List<GameObject> ladList = new List<GameObject>();
	private int highScore = 0;

	// Start is called before the first frame update
	void Start() {
		currentLifeSpan = lifeSpan;
		highScoreText.text = "Highest Lad Count: " + highScore;
	}

	// Update is called once per frame
	void Update() {
		//Lifespan
		currentLifeSpan -= Time.deltaTime;
		if ( currentLifeSpan <= 0 && ladList.Count > 0 ) {
			KillBottomLad();

			currentLifeSpan = lifeSpan;
		}
		else if ( currentLifeSpan <= 0 && ladList.Count <= 0 ) {
			Time.timeScale = 0;
			highScoreText.text = "You Died | Final Score: " + highScore;
		}

		//Movmement
		direction = new Vector2( 0, 0 );
		if ( Input.GetKey( KeyCode.W ) ) {
			direction = new Vector3( direction.x, 0, 1 );
		}
		if ( Input.GetKey( KeyCode.A ) ) {
			direction = new Vector3( -1, direction.y );
		}
		if ( Input.GetKey( KeyCode.S ) ) {
			direction = new Vector3( direction.x, 0, -1 );
		}
		if ( Input.GetKey( KeyCode.D ) ) {
			direction = new Vector3( 1, 0, direction.y );
		}
		rigidBody.velocity = new Vector3( direction.x * speed, rigidBody.velocity.y, direction.z * speed );
       
        if ( Input.GetKeyDown( KeyCode.K ) ) {
			if ( ladList.Count > 0 ) {
				KillBottomLad();
			}
		}
	}

	public void KillBottomLad() {
		GameObject ladToKill = ladList[ ladList.Count - 1 ];
		ladList.Remove( ladToKill );
		Destroy( ladToKill );
		boxCollider.size -= new Vector3( 0, 1, 0 );
		boxCollider.center = new Vector3( 0, 0.5f * ladList.Count, 0 );
		rigidBody.position += new Vector3( 0, 1, 0 );

		GameObject ladToThrow = Instantiate( deadLad, transform.position + direction + new Vector3( 0, 0.5f, 0 ), Quaternion.identity );
		ladToThrow.GetComponent<Rigidbody>().velocity = new Vector3( direction.x * throwSpeed, rigidBody.velocity.y, direction.z * throwSpeed );
		ladToThrow.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
	}

	public void Win() {
		highScoreText.text = "You Win | Final Score: " + highScore;
	}

	private void OnTriggerEnter( Collider other ) {
		if ( other.gameObject.tag == "Lad" ) {
			ladList.Add( other.gameObject );
			Rigidbody ladRigidBody = other.gameObject.GetComponent<Rigidbody>();
			ladRigidBody.isKinematic = true;
			ladRigidBody.useGravity = false;
			other.gameObject.GetComponent<BoxCollider>().enabled = false;
			other.gameObject.GetComponent<Lad>().OnPickUp();
			boxCollider.size += new Vector3( 0, 1, 0 );
			boxCollider.center = new Vector3( 0, 0.5f * ladList.Count, 0 );
			other.gameObject.transform.parent = this.transform;
			other.gameObject.transform.localPosition = new Vector3( 0, ladList.Count, 0 );

			if ( ladList.Count > highScore ) {
				highScore = ladList.Count;
				highScoreText.text = "Highest Lad Count: " + highScore;
			}
		}
	}
}
