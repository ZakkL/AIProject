using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;

public class MoveToZoneAgent : Agent
{
    [SerializeField] private Transform targetZone1;
    [SerializeField] private Transform targetZone2;

    public override void OnEpisodeBegin(){
        transform.localPosition = new Vector3(-13f, 1, 13f);
    }
    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetZone1.localPosition);
        sensor.AddObservation(targetZone2.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions) {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 10f;
        Vector3 pos = transform.localPosition + new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
        transform.LookAt(pos);
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }


    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }
    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<Zone1>(out Zone1 zone1)) {
            float prob = Random.Range(0.0f, 1.0f);
            float reward;
            if(prob < 0.4f) {
                reward = 10.0f;
            } else {
                reward = -10f;
            }

            SetReward(reward);
            EndEpisode();
        }
        if (other.TryGetComponent<Zone2>(out Zone2 zone2)) {
            SetReward(2.0f);
            EndEpisode();
        }
        if (other.TryGetComponent<Breadcrumbs>(out Breadcrumbs crumb)) {
            AddReward(crumb.crumb);
            other.gameObject.SetActive(false);
            // Debug.Log("I hit a crumb!!!");
            // EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall)) {
            SetReward(-20f);
            EndEpisode();
        }


    }
}


