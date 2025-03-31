using UnityEngine;
using System;

public class MaxBehaviourScript : MonoBehaviour
{
    bool running;
    bool casting;
    bool castStart;
    public bool grounded;
    public float groundedLookDepth;
    bool jumpingTillFixedUpdate;
    Vector2 xzMovement; 
    Animator animator;
    public float speed;
    Transform cameraPivotTransform;
    Transform visualsTransform;
    Vector3 moveRelCamera;
    Rigidbody rigidbody;
    Rigidbody? floorBody;
    public BoxCollider floorCollider;
    public float mouseSensitivity = 360f;
    public float jumpForce = 10f;
    float rx;
    float ry;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        running = false;
        casting = false;
        grounded = false;
        xzMovement = new Vector2(0f, 0f);
        moveRelCamera = Vector3.zero;

        visualsTransform = transform. Find("MaxVisuals");
        cameraPivotTransform = transform. Find("CameraPivot");

        animator = visualsTransform.GetComponent<Animator>();        
        rigidbody = GetComponent<Rigidbody>();

        rx = 0;
        ry = 0;
    }

    // Update is called once per frame
    void Update()
    {   
        //UpdateMovement();
        UpdateCameraRotation();
        UpdateMovementRelativeToCamera();
        UpdateCasting();
        UpdateJumpInput();
        
    }

    void FixedUpdate(){
        //CorrectFloor();
        //grounded = false;
        FixedUpdateDetectGround();
        FixedUpdateRewindStartSpeedErasure();
        FixedUpdateMovement();
        FixedUpdateJump();

    }

    void FixedUpdateDetectGround(){
        Vector3 halfExtents = new Vector3(0.4f,0.01f,0.4f);
        //Vector3 smallExtents = new Vector3(0.1f,0.1f,0.1f);
        Vector3 castBegin =  transform.position;// - Vector3.up* 0.01f;
        //float maxDistance = 0.01f;
        grounded = Physics.BoxCast(castBegin,halfExtents, -Vector3.up,out RaycastHit hitInfo,Quaternion.identity, groundedLookDepth);
        //Debug.DrawLine(transform.position,transform.position+smallExtents);
        if (grounded){
            //Debug.Log("DetectGround with " + hitInfo.transform.gameObject); 
        }
        

    }

    void UpdateMovementInput(){
        float dx = 0f;
        float dz = 0f;
        if(Input.GetKey(KeyCode.W)){
            dx+=1f;
        }
        if(Input.GetKey(KeyCode.S)){
            dx-=1f;
        }

        if(Input.GetKey(KeyCode.A)){
            dz-=1f;
        }        

        if(Input.GetKey(KeyCode.D)){
            dz+=1f;
        }  
        xzMovement = new Vector2(dx,dz).normalized;
        bool newRunning = xzMovement.magnitude > 0.001f;
        if (newRunning != running){
            running = newRunning;
            animator.SetBool("running",running);

        }
    }


    void OnCollisionEnter(Collision col){
        //grounded = true;
        floorBody = col.gameObject.GetComponent<Rigidbody>();
        //Debug.Log("OnCollisionEnter with " + floorBody); 
    }
    //OnCollisionStay
    void OnCollisionStay(Collision col){
        //grounded = true;
        floorBody = col.gameObject.GetComponent<Rigidbody>();
        //Debug.Log("OnCollisionStay with " + floorBody); 
    }


    void OnCollisionExit(Collision col){
        //grounded = false;
        floorBody = null;//other.GetComponent<Rigidbody>();
    }

    void FixedUpdateJump(){
        if (jumpingTillFixedUpdate){
            jumpingTillFixedUpdate = false;
            rigidbody.linearVelocity += new Vector3(0, jumpForce, 0);
            //Debug.Log("FixedUpdateJump "); 
        }
    }

    void UpdateJumpInput(){

        if (grounded && Input.GetKeyDown("space")){          
            jumpingTillFixedUpdate= true;  
            //rigidbody.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse); 
            //rigidbody.linearVelocity += new Vector3(0, jumpForce, 0);
            //grounded = false;    
                  
        }

    }


    void UpdateCasting(){
        if(Input.GetMouseButtonDown(1)){
            casting = true;
            castStart = true;
            animator.SetLayerWeight(1,1f);
            animator.SetBool("casting",true);
        }

        if(Input.GetMouseButtonUp(1)){
            casting = false;
            animator.SetLayerWeight(1,0f);
            animator.SetBool("casting",false);
        }        

    }

    void UpdateMovementRelativeToCamera(){
        //get players WASD oc contoller:

        float dz = Input.GetAxis("Vertical");
        float dx = Input.GetAxis("Horizontal");



        //get camera- normalized Directional Vectors:
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;

        Vector3 right = Camera.main.transform.right;
        right.y = 0;

        Vector3 dzRelCamera = dz * forward.normalized;
        Vector3 dxRelCamera = dx * right.normalized;

        moveRelCamera = dzRelCamera + dxRelCamera;
        

        bool newRunning = moveRelCamera.magnitude > 0.001f;
        if (newRunning != running){
            running = newRunning;
            animator.SetBool("running",running);
        }        

        //update character orientation:
        if (grounded && running){
            Vector3 newForward =  moveRelCamera;
            newForward.y = 0;
            visualsTransform.rotation  = Quaternion.LookRotation(newForward);
        }

    }

    void FixedUpdateRewindStartSpeedErasure(){
        if (castStart &&  ! grounded){
            rigidbody.linearVelocity = Vector3.zero;
            castStart = false;
        }

    }


    void FixedUpdateMovement(){

        // set linearVelocity to be as for plattform, if it exist:
        if (grounded){
            if (floorBody != null){
                rigidbody.linearVelocity = floorBody.linearVelocity;
            }
            else{
                rigidbody.linearVelocity = new Vector3(0f,rigidbody.linearVelocity.y, 0f); 
            }
            
            //Debug.Log("floorBody.linearVelocity set ");
        }
        else{
           //change nothing if character is flying.
        }
        

        if ( running){
                Vector3 velocity = moveRelCamera * speed;// * Time.deltaTime;
                velocity.y = 0f; //rigidbody.linearVelocity.y;// not nessesary ?? 

            if (grounded){
                rigidbody.linearVelocity += velocity;  
                Debug.Log("Adding velocity on ground");

            }   
            else{
                rigidbody.linearVelocity += velocity * Time.deltaTime;;  

            }     
           
            
        }




    }//cameraPivotTransform

    float MinMax(float value, float a, float b){

        return Math.Max(a, Math.Min(b,value));

    }

    void UpdateCameraRotation(){
        rx += Input.GetAxis("Mouse X")* mouseSensitivity *  Time.deltaTime;     
        ry += Input.GetAxis("Mouse Y")* mouseSensitivity *  Time.deltaTime;     
        ry = MinMax(ry, -80f,80f);
        cameraPivotTransform.localRotation = Quaternion.Euler(-ry,rx,0);       

    }

    void OnTriggerStay (Collider other){
        floorBody = other.GetComponent<Rigidbody>();
        //grounded = true;
      
    }

    void OnTriggerEnter (Collider other){
        floorBody = other.GetComponent<Rigidbody>();
        //grounded = true;    
    }    

    void OnTriggerExit (Collider other){
        floorBody = null;//other.GetComponent<Rigidbody>();
        //grounded = false;    
    }  


    void CorrectFloor(){
        if (floorBody != null){        
            Vector3 myVel = rigidbody.linearVelocity;
            float otherVy = floorBody.linearVelocity.y;
            float myVy = rigidbody.linearVelocity.y;
            if (otherVy >= myVy){
                myVel.y = otherVy;
                rigidbody.linearVelocity = myVel;
            }
        }
     }


}
