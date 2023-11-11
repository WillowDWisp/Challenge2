using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System;

//student's note: this is not entirely the original code; I have added some functions (mostly at the end) which will have an attached comment detailing what I have done

namespace Worq
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AWSEntityIdentifier))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animation))]
    public class AWSPatrol : MonoBehaviour
    {
        //group settings
        [Header("Group")] 
        [Space(10)]
        public WaypointRoute group;
        [HideInInspector] public int groupID = 0;

        //the route that panicing biters take; denotes the "meeting spot"
        public WaypointRoute panicGroup;
        public WaypointRoute chargeGroup;

        //patrol settings
        [Header("Patrol")] [Space(10)] [Tooltip("Minimum amount of time to wait before moving to next patrol point")]
        public float minPatrolWaitTime = 1f;

        [Tooltip("Maximum amount of time to wait before moving to next patrol point")]
        public float maxPatrolWaitTime = 3f;

        [Tooltip("If or not all entities patrol waypoints at random or in sequence")]
        public bool randomPatroler = false;

//		[Tooltip("When you drag in any active gameObject into this slot the patrol entity will abandon current patrol" +
//		         "and go to this position. Can also be set from script by calling the static method " +
//		         "AWSPatrol.GoTo(position); The patrol entity will stop upon arriving this position. " +
//		         "Please tick the resetPatrol checkbox in the inspector, or call ")] 
		public Transform goTo;

        //NavMesh Agent settings
        [Space(10)] [Header("Agent")] [Space(10)] [Tooltip("Speed by which the NavMesh agent moves")]
        public float moveSpeed = 3f;

        //custom variable; this is the move speed in "panic" mode
        [Tooltip("How fast the agent moves while in Panic mode")]
        public float panicMoveSpeed = 5f;

        [Tooltip("The distance from destination the Navmesh agent stops")]
        public float stoppingDistance = 1f;

        [Tooltip("Turning speed of the NavMesh  agent")]
        public float angularSpeed = 500f;

        [Tooltip("Defines how high up the entity is. This is useful for creating flying entities")]
        public float distanceFromGround = 0.5f;

        //Animations
        [Space(10)]
        [Header("Animations")]
        [Space(10)]
        [Tooltip(
            "Animations to play when the agent is in idle position (Selects one at random if multiple are inserted)")]
        public AnimationClip[] idleAnimations;

        [Tooltip(
            "Animations to play when the agent is walking/moving position (Selects one at random if multiple are inserted)")]
        public AnimationClip[] walkAnimations;

        //Debug
        [Space(10)] [Header("Debug")] [Space(10)]
        public bool resetPatrol;

        public bool interruptPatrol;
        public static bool reset;

        //private variables
        private AWSManager mAWSManager;
        private NavMeshAgent agent;
        private Animation anim;
        private AudioSource src;
        private Transform[] patrolPoints;
        private bool isWaiting;
        private bool hasPlayedDetectSound;
        private bool hasReachedGoTo;
        private int waypointCount;
        private int destPoint;

        //private variable that allows the state machine to do it's job
        private bool chase = false;

        //public bool solely for the empty that hides the panic route
        public bool isSpecialEmpty;

        //stores if the biter is panicked
        private bool isPanicking;

        //stores weither the biter has random movement, bc apparently that being enabled breaks the FSM
        private bool isRandomPatrol;

        //reference to the FSM
        private jcsilva.AISystem.AIStateMachine stateMachine;

        void Awake()
        {
            mAWSManager = GameObject.FindObjectOfType<AWSManager>();

            agent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animation>();
            src = GetComponent<AudioSource>();

            //grab the state machine
            try
            {
                stateMachine = GetComponent<jcsilva.AISystem.AIStateMachine>();
            }
            catch (Exception)
            {

                
            }

            try
            {
                /* waypointCount = 0;
                 Transform groupTransform = group.transform;
                 int childrenCount = groupTransform.childCount;

                 for (int i = 0; i < childrenCount; i++)
                 {
                     if (groupTransform.GetChild(i).GetComponent<WaypointIdentifier>())
                     {
                         waypointCount += 1;
                     }
                 }

                 patrolPoints = new Transform[waypointCount];
                 int curIndex = 0;
                 for (int i = 0; i < childrenCount; i++)
                 {
                     if (groupTransform.GetChild(i).GetComponent<WaypointIdentifier>())
                     {
                         patrolPoints[curIndex] = groupTransform.GetChild(i);
                         if (patrolPoints[curIndex].gameObject.GetComponent<MeshRenderer>())
                             patrolPoints[curIndex].gameObject.GetComponent<MeshRenderer>().enabled = false;
                         if (patrolPoints[curIndex].gameObject.GetComponent<Collider>())
                             patrolPoints[curIndex].gameObject.GetComponent<Collider>().enabled = false;
                         curIndex++;
                     }
                 }*/

                WaypointInitialize(group);


            }
            catch (Exception e)
            {
                Debug.LogWarning("Group not assigned for " + gameObject.name);
            }

            if (isSpecialEmpty)
            {
                gameObject.SetActive(false);
            }
        }

        //I moved the code from Awake to this function to make the Panic() function work
        void WaypointInitialize(WaypointRoute group)
        {
            waypointCount = 0;
            Transform groupTransform = group.transform;
            int childrenCount = groupTransform.childCount;

            for (int i = 0; i < childrenCount; i++)
            {
                if (groupTransform.GetChild(i).GetComponent<WaypointIdentifier>())
                {
                    waypointCount += 1;
                }
            }

            patrolPoints = new Transform[waypointCount];
            if(patrolPoints.Length != 0)
            {

            }
            int curIndex = 0;
            for (int i = 0; i < childrenCount; i++)
            {
                if (groupTransform.GetChild(i).GetComponent<WaypointIdentifier>())
                {
                    patrolPoints[curIndex] = groupTransform.GetChild(i);
                    if (patrolPoints[curIndex].gameObject.GetComponent<MeshRenderer>())
                        patrolPoints[curIndex].gameObject.GetComponent<MeshRenderer>().enabled = false;
                    if (patrolPoints[curIndex].gameObject.GetComponent<Collider>())
                        patrolPoints[curIndex].gameObject.GetComponent<Collider>().enabled = false;
                    curIndex++;
                }
            }
        }

        

        void Start()
        {
            anim = GetComponent<Animation>();
            if (anim == null)
                anim = gameObject.AddComponent<Animation>();

            agent.autoBraking = false;
            agent.stoppingDistance = stoppingDistance;
            agent.speed = moveSpeed;
            agent.angularSpeed = angularSpeed;
            agent.baseOffset = distanceFromGround;

            goTo = null;

            //make sure the state machine is disabled
            disableFSM();

            string newName;
            if (idleAnimations != null && idleAnimations.Length > 0)
            {
                for (int i = 0; i < idleAnimations.Length; i += 1)
                {
                    newName = idleAnimations[i].name;
                    idleAnimations[i].legacy = true;
                    anim.AddClip(idleAnimations[i], newName);
                }
            }

            if (walkAnimations != null && walkAnimations.Length > 0)
            {
                for (var i = 0; i < walkAnimations.Length; i += 1)
                {
                    newName = walkAnimations[i].name;
                    walkAnimations[i].legacy = true;
                    anim.AddClip(walkAnimations[i], newName);
                }
            }

            isRandomPatrol = randomPatroler;

            try
            {
                GotoNextPoint();
            }
            catch (NullReferenceException e)
            {
            }
        }

        void Update()
        {
            if (resetPatrol || reset)
            {
                agent.isStopped = false;
                randomPatroler = isRandomPatrol;
                goToNextPointDirect();
                interruptPatrol = false;
                resetPatrol = false;
                reset = false;

            }

            if (interruptPatrol)
            {
                randomPatroler = false;

                if (!chase){
                agent.isStopped = true;
                }

                if (null != idleAnimations)
                    playAnimation(idleAnimations);
            }

            if (!interruptPatrol && !isWaiting && agent.remainingDistance <= stoppingDistance && null != group)
            {
                GotoNextPoint();
            }

            //press to cause panic
            if (Input.GetKeyDown(KeyCode.B))
            {
                Panic();
            }

            //press to stop panicking
            if (Input.GetKeyDown(KeyCode.C))
            {
                Calm();
            }

            //press to make the biters go to the target (for when they aren't cooperating)
            if (Input.GetKeyDown(KeyCode.F))
            {
                agent.SetDestination(stateMachine.GetTargetPosition().position);
            }

//For future a release
    //screw that, i'm gonna use it!

    //later note: I shouldn't have used it
/*			if (null != goTo && !hasReachedGoTo)
			{
				interruptPatrol = true;
				agent.SetDestination(goTo.position);
				if (walkAnimations != null)
					playAnimation (walkAnimations);

			if (agent.remainingDistance <= stoppingDistance)
				{
					playAnimation (idleAnimations);
					hasReachedGoTo = true;
				}
			}*/

            //updating variables

            agent.stoppingDistance = stoppingDistance;

            if (isPanicking)
            {
                agent.speed = this.panicMoveSpeed;
            }
            else
            {
                agent.speed = this.moveSpeed;
            }


            agent.angularSpeed = angularSpeed;
            agent.baseOffset = distanceFromGround;
        }

        private void GotoNextPoint()
        {
            if (patrolPoints.Length == 0)
                return;
//			Debug.Log ("Going to next point...");
            //if(null != goTo)
            //{
            StartCoroutine(pauseAndContinuePatrol());
           //}


        }

        IEnumerator pauseAndContinuePatrol()
        {
            isWaiting = true;
            if (idleAnimations != null)
                playAnimation(idleAnimations);

            float waitTime = UnityEngine.Random.Range(minPatrolWaitTime, maxPatrolWaitTime);
            if (waitTime < 0f)
                waitTime = 1f;

            yield return new WaitForSeconds(waitTime);


            if (randomPatroler)
            {
                agent.destination = patrolPoints[destPoint].position;
                int nextPos;
                do
                {
                    nextPos = UnityEngine.Random.Range(0, patrolPoints.Length);
                } while (nextPos == destPoint);

                destPoint = nextPos;
            }
            else
            {
                agent.destination = patrolPoints[destPoint].position;
                destPoint = (destPoint + 1) % patrolPoints.Length;
            }
            



            if (walkAnimations != null)
                playAnimation(walkAnimations);
            isWaiting = false;
        }

        void goToNextPointDirect()
        {
            if (randomPatroler)
            {
                try
                {
                agent.destination = patrolPoints[destPoint].position;
                }
                catch (Exception)
                {

                    agent.destination = patrolPoints[0].position;
                }

                int nextPos;
                do
                {

                        nextPos = UnityEngine.Random.Range(0, patrolPoints.Length);

                } while (nextPos == destPoint);

                destPoint = nextPos;
            }
            else
            {
                agent.destination = patrolPoints[destPoint].position;
                destPoint = (destPoint + 1) % patrolPoints.Length;
            }

            if (walkAnimations != null)
                playAnimation(walkAnimations);
        }

        void RestartPatrol()
        {
            hasPlayedDetectSound = false;
            resetPatrol = false;
            agent.speed = moveSpeed;

            agent.stoppingDistance = 1f;
            if (walkAnimations != null)
                playAnimation(walkAnimations);
            goToNextPointDirect();
        }

        void playAnimation(AnimationClip clip)
        {
            anim.Play(clip.name);
        }

        void playAnimation(AnimationClip[] clips)
        {
            if (clips.Length > 0 && null != clips)
            {
                int rand = UnityEngine.Random.Range(0, clips.Length);
                if (clips[rand] != null)
                    anim.Play(clips[rand].name);
//				Debug.Log ("Now Playing: " + clips [rand].name);
//			} else {
//				Debug.LogWarning("Some enemy animations are missing ");
            }
        }

        public void ResetPatrol()
        {
            resetPatrol = true;
        }

        public void InterruptPatrol()
        {
            interruptPatrol = true;
        }

        public void SetDestination(Transform t)
        {
            agent.destination = t.position;
            if (walkAnimations != null)
                playAnimation(walkAnimations);
            isWaiting = false;
        }

        //Custom things, added by yours truly

        //handles the Biters going from patrolling their base to the "meeting spot" when panicking
        void Panic()
        {
            try
            {
                TextHandler handler = mAWSManager.gameObject.GetComponent<TextHandler>();
                handler.SetText(true);
                InterruptPatrol();
                WaypointInitialize(panicGroup);
                isPanicking = true;
                agent.speed = panicMoveSpeed;
                ResetPatrol();
                StartCoroutine("Charge");
            }
            catch (Exception)
            {

                Debug.LogWarning("You forgot to give the following a panic group: " + gameObject.name);
            }
        }
        //sends the biters back to their base
        void Calm()
        {
            try
            {
                TextHandler handler = mAWSManager.gameObject.GetComponent<TextHandler>();
                handler.SetText(false);

                goTo = null;
                chase = false;
                isPanicking = false;
                InterruptPatrol();
                randomPatroler = isRandomPatrol;
                WaypointInitialize(group);
                agent.speed = moveSpeed;
                ResetPatrol();
                
            }
            catch (Exception)
            {

                Debug.LogWarning("How is it missing an initial group?! " + gameObject.name);
            }
        }

        //enables the state machine
        void enableFSM()
        {
            stateMachine.SetEnabled(true);
        }

        void disableFSM()
        {
            stateMachine.SetEnabled(false);
        }
        //handles going from the panic point to the target
        IEnumerator Charge()
        {
            yield return new WaitForSeconds(10);
            //chase = true;
            randomPatroler = false;
            //goTo = stateMachine.GetTargetPosition();
            //agent.SetDestination(stateMachine.GetTargetPosition().position);
            //enableFSM();
            destPoint = 0;
            WaypointInitialize(chargeGroup);

            //agent.isStopped = false;
            Debug.Log("CHAAAARGE!");
            StopCoroutine("Charge");
        }

    }

}