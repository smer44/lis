using UnityEngine;
using System.Collections.Generic;

public class TimeReversableObject2D : MonoBehaviour
{

    private Stack<TimeRecord2D> memory;
    private int maxMemory = 500;
    public bool isRewinding;
    public float startingWait;
    public Vector3 velocity;
    Rigidbody2D body;
    SpriteRenderer renderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        memory =new Stack<TimeRecord2D> ();
        isRewinding = false;
        body = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        CheckStartingWait();
        isRewinding = Input.GetKey(KeyCode.LeftShift);
        if (isRewinding){
            body.bodyType  = RigidbodyType2D.Static;
            renderer.color = Color.blue;
        }


        else{
            //body.bodyType  = RigidbodyType2D.Dynamic;
            renderer.color = Color.white;

        } 
        
        velocity = body.linearVelocity;
    }


    void CheckStartingWait(){
        startingWait -= Time.deltaTime;
        body.bodyType  = startingWait <=0 ? RigidbodyType2D.Dynamic :  RigidbodyType2D.Static;

    }

    void FixedUpdate(){
        if (isRewinding)
            Rewind();
        else 
            Record();

    }

    void Record(){
        //memory.Insert(0,transform.position);        

        memory.Push(new TimeRecord2D(this));
    }

    void Rewind(){
        if (memory.Count > 0)
            memory.Pop().Restore(this);
        else {
            //StopRewind();    
            Record();
        }
            
        //memory.RemoveAt(0);
    }



    void StartRewind(){
        isRewinding = true;
        body.isKinematic = true;
        Debug.Log("StartRewind");
    }

    void StopRewind(){
        isRewinding = false;
        body.isKinematic = false;
        //Debug.Log("StopRewind");
    }    


}
