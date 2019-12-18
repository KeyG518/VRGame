using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GunVR : MonoBehaviour {

    public GameObject end, start; // The gun start and end point
    public GameObject gun;
    public Animator animator;
    
    public GameObject spine;
    public GameObject handMag;
    public GameObject gunMag;

    public GameObject shotSound;
    public GameObject muzzlePrefab;

    public GameObject bulletHole;
    public GameObject amCrate1;
    public GameObject amCrate2;

    float gunShotTime = 0.1f;
    float gunReloadTime = 1.0f;
    Quaternion previousRotation;
    public float health = 100;
    public bool isDead;
 
    public Text healtht;
    public Text magBullets;
    public Text remainingBullets;

    int magBulletsVal = 30;
    int remainingBulletsVal = 90;
    int magSize = 30;
    public GameObject headMesh;
    public static bool leftHanded { get; private set; }
    public float time;

    // Use this for initialization
    void Start() {
        headMesh.GetComponent<SkinnedMeshRenderer>().enabled = false; // Hiding player character head to avoid bugs :)
        time = 0;
    }

    // Update is called once per frame
    void Update() {
        

        if(health <= 0){
            isDead = true;
            GetComponent<Animator>().SetBool("dead",true);
            GetComponent<CharacterMovement>().isDead = true;
            GetComponent<CharacterController>().enabled = false;
            headMesh.GetComponent<SkinnedMeshRenderer>().enabled = true;
            Invoke("restart",10.0f);
            
        } 
        // Cool down times
        if (gunShotTime >= 0.0f)
        {
            gunShotTime -= Time.deltaTime;
        }
        if (gunReloadTime >= 0.0f)
        {
            gunReloadTime -= Time.deltaTime;
        }


        float disC1 = Vector3.Distance(transform.position, amCrate1.transform.position);
        float disC2 = Vector3.Distance(transform.position, amCrate2.transform.position);
        //print(disC1);
        //print(disC2);
        
        if(disC1<3 || disC2 <3){
            remainingBulletsVal = 90;
            updateText();
        }

        
        if ((OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger)|| Input.GetMouseButtonDown(0)) && gunShotTime <= 0 && gunReloadTime <= 0.0f && magBulletsVal > 0 && !isDead)
        { 
            shotDetection(); // Should be completed

            addEffects(); // Should be completed

            animator.SetBool("fire", true);
            gunShotTime = 0.5f;
            
            // Instantiating the muzzle prefab and shot sound
            
            magBulletsVal = magBulletsVal - 1;
            if (magBulletsVal <= 0 && remainingBulletsVal > 0)
            {
                animator.SetBool("reloadAfterFire", true);
                gunReloadTime = 2.5f;
                Invoke("reloaded", 2.5f);
            }
        }
        else
        {
            animator.SetBool("fire", false);
        }

        if ((OVRInput.GetDown(OVRInput.Button.Back) || OVRInput.Get(OVRInput.Button.Back) || OVRInput.GetDown(OVRInput.RawButton.Back) || OVRInput.Get(OVRInput.RawButton.Back) || Input.GetKey(KeyCode.R)) && gunReloadTime <= 0.0f && gunShotTime <= 0.1f && remainingBulletsVal > 0 && magBulletsVal < magSize && !isDead )
        {
            animator.SetBool("reload", true);
            gunReloadTime = 2.5f;
            Invoke("reloaded", 2.0f);
        }
        else
        {
            animator.SetBool("reload", false);
        }
        updateText();
       
    }

  

    public void Being_shot(float damage) // getting hit from enemy
    {
        if(health <= 0){
            isDead = true;
            GetComponent<Animator>().SetBool("dead",true);
            GetComponent<CharacterMovement>().isDead = true;
            GetComponent<CharacterController>().enabled = false;
            headMesh.GetComponent<SkinnedMeshRenderer>().enabled = true;
            Invoke("restart",10.0f);
            
        } else{
            health = health - damage;
            healtht.text = health.ToString();
        }
    }

    public void ReloadEvent(int eventNumber) // appearing and disappearing the handMag and gunMag
    {
        if(eventNumber == 1){
            handMag.GetComponent<SkinnedMeshRenderer>().enabled = true;
            gunMag.GetComponent<SkinnedMeshRenderer>().enabled = false;

        }
        if(eventNumber == 2){
            handMag.GetComponent<SkinnedMeshRenderer>().enabled = false;
            gunMag.GetComponent<SkinnedMeshRenderer>().enabled = true;

        }
    }

    void reloaded()
    {
        int newMagBulletsVal = Mathf.Min(remainingBulletsVal + magBulletsVal, magSize);
        int addedBullets = newMagBulletsVal - magBulletsVal;
        magBulletsVal = newMagBulletsVal;
        remainingBulletsVal = Mathf.Max(0, remainingBulletsVal - addedBullets);
        animator.SetBool("reloadAfterFire", false);
    }

    void updateText()
    {
        magBullets.text = magBulletsVal.ToString() ;
        remainingBullets.text = remainingBulletsVal.ToString();
    }

    void shotDetection() // Detecting the object which player shot 
    {
        RaycastHit rayHit;

        if(Physics.Raycast(end.transform.position, (end.transform.position - start.transform.position), out rayHit, 100.0f)){

            if(rayHit.transform.tag == "enemy"){
                rayHit.transform.root.GetComponent<Enemy>().Being_shot(rayHit.transform.GetComponent<Damage>().DamageValue);

            }
            else {

                Instantiate(bulletHole, rayHit.point + rayHit.transform.up*0.01f, rayHit.transform.rotation);
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

    void OnTriggerEnter(Collider other)
    {
        
        Invoke("restart", 10.0f);
        
    }


    void restart(){
        SceneManager.LoadScene(0);
    }


}
