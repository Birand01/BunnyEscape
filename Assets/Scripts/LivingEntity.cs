using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    public event System.Action OnReachedEndOfLevel;
    [Header("Camera")]
    [SerializeField] float camZOffset;
    [SerializeField] float camLerpSpeed;
    [Space]
    [Header("References")]
    RaycastHit hit;
    Camera mainCam;
    
    [Space]
    [Header("Movement Variables")]
    Vector3 targetObjectNextPosition;
    float targetObjectHeight;
    [SerializeField] float moveSpeedMouse;
    bool disable;
   

    protected virtual void Start()
    {
        mainCam = Camera.main;
    }

    protected virtual void Update()
    {
        if(!disable)
        {
            Movement();
        }
      
    }
  
    // Bunny Movement
    protected virtual void Movement()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 worldMousePos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
            Vector3 direction = worldMousePos - mainCam.transform.position;
            if (Physics.Raycast(mainCam.transform.position, direction, out hit, 100f))
            {
                if (hit.collider.gameObject.name == "Ground")
                {
                    Debug.DrawLine(mainCam.transform.position, hit.point, Color.green, .5f);
                    targetObjectNextPosition = hit.point + new Vector3(0, targetObjectHeight / 2f, 0f);
                }
            }
            else
            {
                Debug.DrawLine(mainCam.transform.position, hit.point, Color.red, .5f);
            }


            this.transform.position = Vector3.MoveTowards(this.transform.position, targetObjectNextPosition, moveSpeedMouse * Time.deltaTime);
            Vector3 heightCorrectedPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            this.transform.LookAt(heightCorrectedPoint);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Finish")
        {
            if(OnReachedEndOfLevel!=null)
            {
                DisableBunny();
                OnReachedEndOfLevel();
            }
        }
    }

    private void DisableBunny()
    {
        disable = true;
    }

    // Camera Movement
    protected virtual void LateUpdate()
    {
        mainCam.transform.position = new Vector3(mainCam.transform.position.x, mainCam.transform.position.y,
           Mathf.Lerp(mainCam.transform.position.z, this.transform.position.z - camZOffset, camLerpSpeed * Time.deltaTime));
    }
}
