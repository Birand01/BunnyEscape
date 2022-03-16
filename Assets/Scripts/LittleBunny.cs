using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class LittleBunny : LivingEntity
{
    public ParticleSystem carrotParticle;
    public ParticleSystem evolutionParticle;
    private Animator anim;
    public Transform bunnies;
    bool disableBunny;
    public float carrotScore;

    #region Singleton Class:LittleBunny
    public static LittleBunny Instance;
    private void Awake()
    {
        if(Instance==null)
        {
            Instance = this;
        }
    }
    #endregion
    protected override void Start()
    {
        base.Start();
        Gardener.OnGardenerSpottedBunny += DisableBunny;
        bunnies = GameObject.FindGameObjectWithTag("Bunnies").transform;
        anim = GetComponent<Animator>();
      
     
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetMouseButtonUp(0))
        {
            anim.SetFloat("BunnySpeed", 0.0f);
        }
        ChangeBunny();

    }

    private void ChangeBunny()
    {
        if (carrotScore == 7)
        {
            Destroy(Instantiate(evolutionParticle, transform.position, transform.rotation), 3.0f);
            bunnies.GetChild(1).transform.position = bunnies.GetChild(0).transform.position;
            bunnies.GetChild(0).transform.position = new Vector3(0, -10, 0);
            bunnies.GetChild(0).gameObject.SetActive(false);
            bunnies.GetChild(1).gameObject.SetActive(true);
          
        }
    }

    protected override void Movement()
    {
        if(!disableBunny)
        {
            base.Movement();
            if (Input.GetMouseButton(0))
            {
                anim.SetFloat("BunnySpeed", 1.0f);
            }
        }
    }
  



    protected override void OnTriggerEnter(Collider other)
    {
      
        switch (other.tag)
        {
            case "Watermelon":
                carrotScore++;
                Destroy(Instantiate(carrotParticle,transform.position,Quaternion.identity),0.4f);
                other.transform.DOScale(Vector3.zero, 0.5f);
                other.GetComponent<BoxCollider>().enabled = false;
                GameUI.Instance.UpdateScore();
                Destroy(other.gameObject,0.4f);
                break;
            case "Finish":
                base.OnTriggerEnter(other);
                break;
        }
    }
    private void DisableBunny()
    {
        anim.SetInteger("BunnyDeath", 1);
        disableBunny = true;
    }
    private void OnDestroy()
    {
        Gardener.OnGardenerSpottedBunny -= DisableBunny;
    }
}
