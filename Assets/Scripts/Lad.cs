using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lad : MonoBehaviour {
	public Animator animator;
    public AudioSource PickupSound;

	public void OnPickUp() {
		animator.SetBool("IsCarrying", true);
        PickupSound.Play();
	}
}
