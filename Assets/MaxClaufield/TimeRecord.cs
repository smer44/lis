using UnityEngine;

public class TimeRecord
{

    public Vector3 pos;
    public Quaternion rot;
    public Vector3 velocity;
    public Vector3 angularVelocity;
    public float startingWait;


    public TimeRecord(TimeReversableObject obj){

       pos = obj.body.position;
       rot = obj.body.rotation;
       velocity = obj.body.linearVelocity;
       angularVelocity = obj.body.angularVelocity;
       startingWait = obj. startingWait;
    }

    public void Restore(TimeReversableObject obj){
        obj.body.position = pos;
        obj.body.rotation = rot;
        obj.body.linearVelocity = - velocity;//Vector3.zero;
        obj.body.angularVelocity = - angularVelocity; //Vector3.zero;

        obj.startingWait = startingWait;
    }      

    public void RestoreActive(TimeReversableObject obj){
        obj.body.position = pos;
        obj.body.rotation = rot;
        obj.body.linearVelocity = velocity;//Vector3.zero;
        obj.body.angularVelocity = angularVelocity; //Vector3.zero;

        obj.startingWait = startingWait;

    }

    public void toZeroSpeed(){
        velocity = Vector3.zero;
        angularVelocity = Vector3.zero;
        //startingWait = 0;
    }

    
}

public class TimeRecord2D
{

    public Vector2 pos;
    public Quaternion rot;
    public float startingWait;


    public TimeRecord2D(TimeReversableObject2D obj){

       pos = obj.transform.position;
       rot = obj.transform.rotation;
       startingWait = obj. startingWait;
    }

    public void Restore(TimeReversableObject2D obj){
        obj.transform.position = pos;
        obj.transform.rotation = rot;
        obj.startingWait = startingWait;

    }
        

    
}