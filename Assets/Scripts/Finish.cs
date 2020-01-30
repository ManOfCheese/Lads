using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)

    {

  if (other.gameObject.tag == "Player"){
            other.gameObject.GetComponent<Player>().Win();




        }





    }
    // Start is called before the first frame update. I use this to make Sam Suffer
    void Start()

    {





    }


    // Update is called once per frame

    void Update()

    {

        //Nee.  



    }


                      }
