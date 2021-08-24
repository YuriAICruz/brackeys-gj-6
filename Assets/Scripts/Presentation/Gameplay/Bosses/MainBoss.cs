using System.Collections.Generic;
using System.Gameplay;
using Graphene.BehaviourTree;
using Graphene.BehaviourTree.Actions;
using Graphene.BehaviourTree.Composites;
using Graphene.BehaviourTree.Conditions;
using Graphene.Time;
using UnityEngine;
using Zenject;
using Behaviour = Graphene.BehaviourTree.Behaviour;

namespace Presentation.Gameplay.Bosses
{
    public class MainBoss : Boss
    {
        private enum BlackboardIds
        {
            TutorialState = 0,
            ChallengeState = 1,
            FinalState = 2,

            PlayerNear = 10,
            Chase = 11,
            Spit = 12,
            Stag = 13
        }

        private Behaviour _tree;
        private Blackboard _blackboard;

        [Inject] private GameManager _gameManager;

        protected override void Awake()
        {
            base.Awake();
            
            _blackboard = new Blackboard();
            _tree = new Behaviour();

            SetBlackboard();

            _tree.root = new Priority(new List<Node>
                {
                    new Sequence(new List<Node>() // Tutorial State
                    {
                        new CallSystemActionMemory((int) BlackboardIds.TutorialState),
                        new MemoryPriority(new List<Node>
                        {
                            new MemorySequence(new List<Node>()
                            {
                                new CallSystemActionMemory((int) BlackboardIds.PlayerNear),
                                new CallSystemActionMemory((int) BlackboardIds.Spit),
                                new CallSystemActionMemory((int) BlackboardIds.Stag),
                            }),
                            new MemorySequence(new List<Node>()
                            {
                                new CallSystemActionMemory((int) BlackboardIds.Chase),
                            }),
                        })
                    }),
                    new Sequence(new List<Node>() // Challenge State
                    {
                        new CallSystemActionMemory((int) BlackboardIds.ChallengeState),
                    }),
                    new Sequence(new List<Node>() // Final State
                    {
                        new CallSystemActionMemory((int) BlackboardIds.FinalState),
                    })
                }
            );
        }

        protected override void Update()
        {
            _tree.Tick(this.gameObject, _blackboard);
        }

        private void SetBlackboard()
        {
            _blackboard.Set((int) BlackboardIds.TutorialState, new Behaviour.NodeResponseAction(InTutorialState),
                _tree.id);
            _blackboard.Set((int) BlackboardIds.ChallengeState, new Behaviour.NodeResponseAction(InChallengeState),
                _tree.id);
            _blackboard.Set((int) BlackboardIds.FinalState, new Behaviour.NodeResponseAction(InFinalState), _tree.id);

            _blackboard.Set((int) BlackboardIds.PlayerNear, new Behaviour.NodeResponseAction(IsPlayerNear), _tree.id);
            _blackboard.Set((int) BlackboardIds.Spit, new Behaviour.NodeResponseAction(DoSpit), _tree.id);
            _blackboard.Set((int) BlackboardIds.Chase, new Behaviour.NodeResponseAction(DoChase), _tree.id);
            _blackboard.Set((int) BlackboardIds.Stag, new Behaviour.NodeResponseAction(Stag), _tree.id);
        }
        
        private NodeStates InTutorialState()
        {
            if (Hp < stats.maxHp * 0.8f)
                return NodeStates.Failure;

            bossStates.stage = 0;
            return NodeStates.Success;
        }

        private NodeStates InChallengeState()
        {
            if (Hp < stats.maxHp * 0.4f)
                return NodeStates.Failure;
            
            bossStates.stage = 1;
            return NodeStates.Success;
        }

        private NodeStates InFinalState()
        {
            bossStates.stage = 2;
            return NodeStates.Success;
        }

        
        private NodeStates Stag()
        {
            if (bossStates.stagged)
            {
                bossStates.stagElapsed += Timer.deltaTime;

                if (bossStates.stagElapsed >= bossStats.stagBaseDuration)
                {
                    bossStates.stagged = false;
                    return NodeStates.Success;
                }

                return NodeStates.Running;
            }
            
            Debug.Log("Stag");
            bossStates.stagged = true;
            bossStates.stagElapsed = 0f;
            
            return NodeStates.Running;
        }


        private NodeStates DoChase()
        {
            var pos = transform.position;
            var dir = PlayerDistance(pos);

            Debug.DrawRay(pos + dir, Vector3.up * 2, Color.red, 5);


            return NodeStates.Success;
        }

        private NodeStates DoSpit()
        {
            if (bossStates.spiting)
            {
                bossStates.spitingElapsed += Timer.deltaTime;

                if (bossStates.spitingElapsed >= bossStats.spitDuration)
                {
                    bossStates.spiting = false;
                    return NodeStates.Success;
                }

                return NodeStates.Running;
            }
            
            Debug.Log("Spit");
            bossStates.spiting = true;
            bossStates.spitingElapsed = 0f;
            
            return NodeStates.Running;
        }

        private NodeStates IsPlayerNear()
        {
            var dir = PlayerDistance(transform.position);

            Debug.Log(dir.magnitude);
            if (dir.magnitude < bossStats.chaseDistance)
                return NodeStates.Success;
            
            return NodeStates.Failure;
        }


        private Vector3 PlayerDistance(Vector3 pos)
        {
            return _gameManager.Player.Position - pos;
        }
    }
}