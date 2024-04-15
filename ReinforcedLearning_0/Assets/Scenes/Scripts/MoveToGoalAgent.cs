using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class MoveToGoalAgent : Agent
{
    //values/references passed through
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private Material baseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;

    public override void OnEpisodeBegin()
    {
        //transform.localPosition = Vector3.zero;
        //floorMeshRenderer.material = baseMaterial;//reset floor material

        //randomly spawn both the goal and character for more randomised testing
        transform.localPosition = new Vector3(Random.Range(-3f, +3f), 0 , Random.Range(-3f, +3f));
        targetTransform.localPosition = new Vector3(Random.Range(-3f, +3f), 0 , Random.Range(-3f, +3f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //base.CollectObservations(sensor);
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //Debug.Log(actions.DiscreteActions[0]);

        //set speed and movement to the x and z axis
        float speed = 2f;
        float X_movement = actions.ContinuousActions[0];
        float Z_movement = actions.ContinuousActions[1];

        //move character using above variables
        transform.localPosition += new Vector3(X_movement, 0, Z_movement) * Time.deltaTime * speed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)//for testing only, manually moves characters
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<GoalTrigger>(out GoalTrigger goal))//if character collides with the goal trigger -
        {
            SetReward(+1f);//set reward to positive 1
            floorMeshRenderer.material = winMaterial; //change floor for more visual
            EndEpisode();// end episode
        }
        
        if(other.TryGetComponent<WallTrigger>(out WallTrigger wall))//if character collides with the wall trigger -
        {
            SetReward(-1f);//set reward to negitive 1
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();// end episode
        }
       
    }
 
}
