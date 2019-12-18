using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	public GameObject[] target;
	public int index = 0;
  public GameObject cover;
	public GameObject player;
  public GameObject shotSound;
  public GameObject muzzlePrefab;
  public GameObject end, start;
  public float time;
  public bool isDead;
  public float health = 100;
  public float timec;
  public int l;
  //public GameObject bulletHole;

    // Start is called before the first frame update
    void Start()
    {
    	 time = 0.2f;
       timec = 0.0f;
        
    }

    // Update is called once per frame
    void Update()
    {
    	 l = target.Length;
    	

       float dist = Vector3.Distance(target[index].transform.position, transform.position);

       if(dist < 2 && index < l-3){
       	index = index + 1;
       }

       if(dist < 2 && index ==l-3){
       	index = 0;
       }

       Vector3 tempPos = new Vector3(target[index].transform.position.x,transform.position.y,target[index].transform.position.z);
        //transform.LookAt(tempPos);
       Quaternion diseredRotation = Quaternion.LookRotation(tempPos - transform.position);
       transform.rotation = Quaternion.Lerp(transform.rotation,diseredRotation, Time.deltaTime);

       Vector3 distoPlayer =  player.transform.position - transform.position;

       float realDist = Vector3.Distance(transform.position, player.transform.position);
       //print(realDist);
       distoPlayer = Vector3.Normalize(distoPlayer);
       float disbtw = Vector3.Dot(distoPlayer,transform.forward);
        float distocov = Vector3.Distance(transform.position,cover.transform.position);
       //print(time);
       print(distocov);
       if(!isDead){
       if(disbtw > 0.8 && realDist < 15 && distocov > 3 && distocov <30){
        GetComponent<Animator>().SetBool("run",true);
       	index = l-2;
        target[index] = cover;
       	
       }else if(disbtw > 0.8 && realDist < 15 && distocov >= 30){
        GetComponent<Animator>().SetBool("run",true);
       	index = l-1;
        target[index] = player;
       }

       if(timec > 4){
         timec = 0.0f;
       } 
     
       if(distocov <= 3 && realDist < 15){
        GetComponent<CharacterController>().enabled = false;
        GetComponent<Animator>().SetBool("sit",true);
        
        index = l-1;
        target[index] = player;
        timec = timec + Time.deltaTime;
      

      if(timec > 2){
         GetComponent<CharacterController>().enabled = true;
         GetComponent<Animator>().SetBool("sit",false);
        if(time < 0.2f){
          GetComponent<Animator>().SetBool("fire",false);
          time = time + Time.deltaTime;
        }else{
        GetComponent<Animator>().SetBool("fire",true);
        addEffects();
        shotDetection();
        time = 0;
        }

     }


    }else if(realDist < 10 && distocov > 30){
      GetComponent<Animator>().SetBool("sit",false);
        index = l-1;
        target[index] = player;
        if(time < 0.2f){
          GetComponent<Animator>().SetBool("fire",false);
          time = time + Time.deltaTime;
        }else{
        GetComponent<Animator>().SetBool("fire",true);
        addEffects();
        shotDetection();
        time = 0;
        }

     

    }




      if(GetComponent<Animator>().GetBool("fire") == true && realDist > 10){
        GetComponent<Animator>().SetBool("fire",false);
        GetComponent<Animator>().SetBool("run",true);
      }
    }


      if(health <= 0){
        
        GetComponent<Animator>().SetBool("dead",true);
        GetComponent<CharacterController>().enabled = false;
        isDead = true;
       // Vector3 temp = transform.position;
        //temp.y = -0.8f;
        //transform.position = temp;
        

      }



    }

        public void Being_shot(float damage) // getting hit from enemy
    {
        if(health <= 0){
            isDead = true;
            GetComponent<Animator>().SetBool("dead",true);
            GetComponent<CharacterController>().enabled = false;
        } else{
          //print(damage);
            health = health - damage;
            index = l-1;
            target[index] = player;

            //print(health);
        }
    }

    void shotDetection() // Detecting the object which player shot 
    {
        RaycastHit rayHit;
        float tempp = Random.Range(-0.1f,0.1f);
        Vector3 newendu = end.transform.up * tempp;
        float temppp = Random.Range(-0.1f,0.1f);
        Vector3 newendr = end.transform.right* temppp;

        if(Physics.Raycast(end.transform.position + newendu + newendr, (end.transform.position + newendu + newendr - start.transform.position), out rayHit, 100.0f)){

            if(rayHit.transform.root.tag == "Player"){
                //if(rayHit.transform.root.GetComponent<Damage>() != null){
                  rayHit.transform.root.GetComponent<GunVR>().Being_shot(rayHit.transform.GetComponent<Damage>().DamageValue);
                  Debug.Log(rayHit.transform.GetComponent<Damage>().DamageValue);
                //rayHit.transform.GetComponent<GunVR>().Being_shot(20);
                //}

            }

        }

    }

        void addEffects() // Adding muzzle flash, shoot sound and bullet hole on the wall
    {


        Destroy(Instantiate(shotSound, transform.position, transform.rotation),2.0f);

        GameObject tempMuzzle = Instantiate(muzzlePrefab, end.transform.position, end.transform.rotation);
        tempMuzzle.GetComponent<ParticleSystem>().Play();

        Destroy(tempMuzzle, 2.0f);

    }
}
