using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent Agent;

    [SerializeField]
    private Transform PlayerTr;

    private void Update()
    {
        Agent.SetDestination(PlayerTr.position);
    }
}
