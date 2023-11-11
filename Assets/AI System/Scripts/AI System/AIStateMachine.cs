using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

namespace jcsilva.AISystem {

    [RequireComponent(typeof(NavMeshAgent))]
    public class AIStateMachine : MonoBehaviour {

        // Events Idle Related
        public Action EventAIEnableIdle;
        public Action EventAIDisableIdle;

        // Events Patrol Related
        public Action EventAIEnablePatrol;
        public Action EventAIDisablePatrol;

        // Events Chase Related
        public Action EventAIEnableChase;
        public Action EventAIDisableChase;

        // Events Attack Related
        public Action EventAIEnableAttack;
        public Action EventAIDisableAttack;

        // Events Last Known Location Related
        public Action EventAIEnableLastKnownLocation;
        public Action EventAIDisableLastKnownLocation;

        [Header("AI Settings")]
        [SerializeField] bool isEnable;
        [SerializeField] AIStates initialState;
        [SerializeField] Transform target;
        [SerializeField] NavMeshAgent agent;
        [SerializeField] Animator animator;

        [Header("AI Behaviours Settings")]
        [SerializeField] float idleDuration;
        [SerializeField] float maxDistanceToView;
        [SerializeField] float minDistanceToAttack;
        [SerializeField] float maxFieldOfView;

        // AI Patrol Settings
        //[HideInInspector]
        [SerializeField] public List<Transform> waypoints = new List<Transform>();

        // Developer Settings
        [HideInInspector]
        [SerializeField] bool enableDebug;
        [HideInInspector]
        [SerializeField] AIStates currentState;
        [HideInInspector]
        AIBehaviour currentBehaviour;
        AIBehaviour[] l_behaviours;


        

        /// <summary>
        /// Dictionary that holds the next state depending on the event occuring,
        /// NOTE: The order is important, so keep the changes you make bellow the comment line
        /// </summary>
        Dictionary<AIEvents, AIStates> nextEvent = new Dictionary<AIEvents, AIStates> {
            [AIEvents.NoLongerIdle] = AIStates.Patrol,
            [AIEvents.SeePlayer] = AIStates.Chase,
            [AIEvents.ReachedDestination] = AIStates.Idle,
            [AIEvents.InRange] = AIStates.Attack,
            [AIEvents.OutOfRange] = AIStates.Chase,
            [AIEvents.LostPlayer] = AIStates.LastKnownLocation,

            // Add new Events bellow this line
            [AIEvents.ReachedLastKnownPosition] = AIStates.Idle,
        };


        // Start is called before the first frame update
        void Start() {

            if (waypoints.Count > 0) {
                ClearEmptyWaypoints();
            }

            if (agent == null) {
                agent = GetComponent<NavMeshAgent>();
            }

            if(animator == null)
            {
                animator = GetComponent<Animator>();
            }

            currentState = initialState;

            InitializeBehaviours();
        }

        private void InitializeBehaviours() {
            l_behaviours = new AIBehaviour[] {
                new IdleBehaviour(this, this),
                new PatrolBehaviour(this, this),
                new ChaseBehaviour(this, this),
                new AttackBehaviour(this, this),
                
                // Add new behaviours bellow this line seperated with a comma ","
                new LastKnownLocation(this, this),
            };

            SelectNextBehaviour(initialState);
        }

        /// <summary>
        /// Method that will enables the next Behaviour
        /// </summary>
        /// <param name="nextBehaviour">Next State to which the AI State needs to go</param>
        private void SelectNextBehaviour(AIStates nextBehaviour) {
            currentState = nextBehaviour;
            currentBehaviour = l_behaviours[(int)currentState];
            currentBehaviour.OnBehaviourStart();
        }

        /// <summary>
        /// Method that handles the AI State Change
        /// </summary>
        /// <param name="AIEvent">AI Event</param>
        public void HandleState(AIEvents AIEvent) {

            // Disable current Behaviour
            currentBehaviour.OnBehaviourEnd();

            // Set the next AI State
            AIStates nextState = nextEvent[AIEvent];

            // Enable the new AI Behaviour
            SelectNextBehaviour(nextState);
        }

        // Update is called once per frame
        void Update() {
            if (isEnable && currentBehaviour != null) {
                currentBehaviour.OnUpdate();
            }
        }

        #region Getters and Setters

        public Transform GetSelfPosition() {
            return this.transform;
        }

        public Transform GetTargetPosition() {
            return target;
        }

        public float GetMaxDistanceToView() {
            return maxDistanceToView;
        }

        public float GetMaxFieldOfView() {
            return maxFieldOfView;
        }

        public float GetIdleDuration() {
            return idleDuration;
        }

        public List<Transform> GetWaypoints() {
            return waypoints;
        }

        public NavMeshAgent GetNavMeshAgent() {
            return agent;
        }

        public float GetMinDistanceToAttack() {
            return minDistanceToAttack;
        }

        public void ClearEmptyWaypoints() {
            waypoints = waypoints.Where(item => item != null).ToList();
        }

        //custom stuff
        public Animator GetAnimator()
        {
            return animator;
        }

        public bool GetEnabled()
        {
            return isEnable;
        }

        public void SetEnabled(bool enabled)
        {
            isEnable = enabled;
        }

        #endregion


    }
}