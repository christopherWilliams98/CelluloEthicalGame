using System.Linq;
using UnityEngine;

public class GhostSheepBehavior : AgentBehaviour
{    

    float detectingDistance = 15f;
    public AudioSource m_MyAudioSourceGhost;
    public AudioSource m_MyAudioSourceSheep;

    public enum Role{
        sheep =0, 
        ghost = 1
    }
    
    Role role = Role.sheep;

    public void Start(){
        Invoke("SwitchRoles", 0.5f);
    }

    void SwitchRoles(){

        float randomTime = Random.Range(5, 10);
    
        if(role == Role.sheep) {
            role = Role.ghost;
            this.agent.SetVisualEffect(VisualEffect.VisualEffectConstAll, Color.red, 255);
 
            m_MyAudioSourceGhost.Play();
      
            
        } else {
            role = Role.sheep;
            this.agent.SetVisualEffect(VisualEffect.VisualEffectConstAll, Color.green, 255);
            m_MyAudioSourceSheep.Play();
               
            
        }
        
        Invoke("SwitchRoles", randomTime);
    
    }

    public override Steering GetSteering()
    {

        Steering steering = new Steering();

        GameObject[] wolfs;
        wolfs = GameObject.FindGameObjectsWithTag("Wolf");

        Vector3 dir = new Vector3();
        GameObject closestWolf = null;
        float distance = Mathf.Infinity;

        foreach (GameObject wolf in wolfs)
        {

            
            Vector3 diff = wolf.transform.position - this.transform.position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closestWolf = wolf;
                distance = curDistance;
            }
            
            if(role == Role.sheep){
                if (curDistance < detectingDistance)
                {
                    dir -= diff;
                }
            } 
                //should make it so it goes after the closest dog! now its a bit confused
                
            
                
        }
        if(role == Role.ghost){
            dir =  closestWolf.transform.position - this.transform.position;
        }

        steering.linear = dir * agent.maxAccel;
        steering.linear = this.transform.parent.TransformDirection(Vector3.ClampMagnitude(steering.
        linear , agent.maxAccel)) ;
    

        return steering;
    }



}
