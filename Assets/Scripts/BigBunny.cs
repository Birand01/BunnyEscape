using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BigBunny : LivingEntity
{
    private Animator bigBunnyAnim;
    private Rigidbody rb;
   
  
    protected override void Start()
    {
        base.Start(); 
        bigBunnyAnim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
      
      
    }

    protected override void Update()
    {
        base.Update();
 
        if (LittleBunny.Instance.carrotScore == 7)
        {
            StartCoroutine(InitializeCarrotScore());
        }
       
    }
   
    protected override void Movement()
    {
        
            base.Movement();

            if (Input.GetMouseButton(0))
            {
            bigBunnyAnim.SetFloat("Motion", 1);
            }
        if(Input.GetMouseButtonUp(0))
        {
            bigBunnyAnim.SetFloat("Motion", 0);
        }
       
        }



    public IEnumerator InitializeCarrotScore()
    {
        yield return new WaitForSeconds(7.0f);
        LittleBunny.Instance.carrotScore--;
        float val = LittleBunny.Instance.carrotScore / 7f;
        GameUI.Instance.scoreProgressImage.DOFillAmount(val, 0.4f);
        if (LittleBunny.Instance.carrotScore == 0)
        {
          
            Destroy(Instantiate(LittleBunny.Instance.evolutionParticle, transform.position, transform.rotation), 3.0f);
            LittleBunny.Instance.bunnies.GetChild(0).transform.position = LittleBunny.Instance.bunnies.GetChild(1).transform.position;
            LittleBunny.Instance.bunnies.GetChild(0).gameObject.SetActive(true);
            LittleBunny.Instance.bunnies.GetChild(1).gameObject.SetActive(false);
           
        }
    }
    


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Gardener" || collision.gameObject.tag == "Gate")
        {
            float force = 200f;
            bigBunnyAnim.SetInteger("Punch", 1);
            //Vector3 dir = collision.contacts[0].point - transform.position;  
            Vector3 dir = transform.position - collision.transform.position;
            dir = -dir.normalized;
            collision.gameObject.GetComponent<Rigidbody>().AddForce(dir * force,ForceMode.Impulse);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Gardener" || collision.gameObject.tag == "Gate")
        {
            bigBunnyAnim.SetInteger("Punch", 0);
        }
    }
}
