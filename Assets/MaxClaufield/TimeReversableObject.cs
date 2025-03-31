using UnityEngine;
using System.Collections.Generic;

public class TimeReversableObject : MonoBehaviour
{

    private Stack<TimeRecord> memory;
    private int maxMemory = 500;
    public bool isRewinding;
    public float startingWait;
    Vector3 velocity;
    public Vector3 startingVelocity;
    public Rigidbody body;
    public TimeRecord noSpeed;
    //SpriteRenderer renderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        memory =new Stack<TimeRecord> ();
        isRewinding = false;
        body = GetComponent<Rigidbody>();
        body.linearVelocity = startingVelocity;
        noSpeed = new TimeRecord(this);
        noSpeed.toZeroSpeed();
        //renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        CheckStartingWait();
        if (Input.GetMouseButtonDown(1)){
            StartRewind();

        }

        else{
            //body.bodyType  = RigidbodyType2D.Dynamic;
            //renderer.color = Color.white;
            if (Input.GetMouseButtonUp(1)){
                StopRewind();
            }           
        }      

    }


    void StartRewind(){
        isRewinding = true;
    }

    void StopRewind(){
        isRewinding = false;
        memory.Peek ().RestoreActive(this);
    }


    void CheckStartingWait(){
        startingWait -= Time.deltaTime;
        body.isKinematic  = startingWait >= 0;

    }

    void FixedUpdate(){
        velocity = body.linearVelocity;
        if (isRewinding)
            Rewind();
        else 
            Record();

    }

    void Record(){
        //memory.Insert(0,transform.position);        

        memory.Push(new TimeRecord(this));
    }

    void Rewind(){
        if (memory.Count > 1){
            memory.Pop().Restore(this);
        }
            
        else {
            // always be in the last 
            noSpeed.Restore(this);
        }
            
        //memory.RemoveAt(0);
    }



}
