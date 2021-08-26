using System.Collections.Generic;
using System.Gameplay;
using System.Linq;
using Graphene.BehaviourTree;
using Graphene.BehaviourTree.Actions;
using Graphene.BehaviourTree.Composites;
using Graphene.BehaviourTree.Conditions;
using Graphene.Time;
using Models.Interfaces;
using Models.Signals;
using Presentation.Gameplay.Projectiles;
using UnityEngine;
using Zenject;
using Behaviour = Graphene.BehaviourTree.Behaviour;
using Physics = UnityEngine.Physics;

namespace Presentation.Gameplay.Bosses
{
    public class MainBoss : Boss
    {
        public override Vector3 Center => mouth.position;

        private enum BlackboardIds
        {
            TutorialState = 0,
            ChallengeState = 1,
            FinalState = 2,

            PlayerNear = 10,
            Chase = 11,
            Spit = 12,
            Stag = 13,
            Anticipate = 14,
            PlayerBehind = 15,
            TailHit = 16,
            Backup = 17
        }

        private Behaviour _tree;
        private Blackboard _blackboard;

        [Inject] private GameManager _gameManager;
        [Inject] private PhysicsSettings _physicsSettings;
        [Inject] private Spit.Factory _factory;

        [Space] public Transform mouth;

        public BossPieces[] _pieces;

        private Spit[] _bullets;
        private int _currentSpit;

        [Header("Attack")] public Transform[] tailPoints;
        public Transform[] headPoints;
        private Vector3[] _currentDamageTracker;


        protected override void Awake()
        {
            base.Awake();

            _bullets = new Spit[bossStats.sprayCount * 7];
            for (int i = 0; i < _bullets.Length; i++)
            {
                _bullets[i] = _factory.Create();
            }

            for (int i = 0; i < _pieces.Length; i++)
            {
                _pieces[i].damaged += Damage;
            }

            _blackboard = new Blackboard();
            _tree = new Behaviour();

            SetBlackboard();

            _tree.root = new MemoryPriority(new List<Node>
                {
                    new MemorySequence(new List<Node>() // Tutorial State
                    {
                        new CallSystemActionMemory((int) BlackboardIds.TutorialState),
                        new MemoryPriority(new List<Node>
                        {
                            new MemorySequence(new List<Node>()
                            {
                                new CallSystemActionMemory((int) BlackboardIds.PlayerNear),
                                new CallSystemActionMemory((int) BlackboardIds.Anticipate),
                                new MemoryPriority(new List<Node>
                                {
                                    new CallSystemActionMemory((int) BlackboardIds.PlayerBehind),
                                    new CallSystemActionMemory((int) BlackboardIds.TailHit),
                                    new CallSystemActionMemory((int) BlackboardIds.Stag),
                                }),
                                new MemorySequence(new List<Node>
                                {
                                    new CallSystemActionMemory((int) BlackboardIds.Backup),
                                    new CallSystemActionMemory((int) BlackboardIds.Spit),
                                    new CallSystemActionMemory((int) BlackboardIds.Stag),
                                }),
                            }),
                            new MemorySequence(new List<Node>
                            {
                                new Chance(0.2f),
                                new CallSystemActionMemory((int) BlackboardIds.Spit),
                                new CallSystemActionMemory((int) BlackboardIds.Stag),
                            }),
                            new MemorySequence(new List<Node>()
                            {
                                new CallSystemActionMemory((int) BlackboardIds.Chase),
                                new CallSystemActionMemory((int) BlackboardIds.Anticipate),
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
            if (!_running) return;

            base.Update();
            _tree.Tick(this.gameObject, _blackboard);
        }

        public override void Damage(int damage)
        {
            states.damaged = false;
            base.Damage(damage);
        }

        protected override void Move(float delta)
        {
            if (!bossStates.moving)
            {
                states.currentSpeed = Mathf.Max(0, states.currentSpeed - Timer.fixedDeltaTime);
                states.grounded = _physics.Grounded;
                return;
            }

            var dir = new Vector2(bossStates.playerDirection.normalized.x, bossStates.playerDirection.normalized.z);
            transform.position =
                _physics.Evaluate(
                    dir * stats.speed,
                    transform.position, delta);

            states.currentSpeed = dir.magnitude * stats.speed;
            states.grounded = _physics.Grounded;
        }

        protected override void TurnTo(Vector3 direction, float delta)
        {
            if (!bossStates.moving) return;

            //if (!bossStates.backingUp)
                base.TurnTo(direction, delta);

            states.turnAngle = Vector3.Angle(transform.forward, direction);
        }

        protected override void TurnTo(Vector2 direction)
        {
            if (!bossStates.moving) return;
            base.TurnTo(direction);
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
            _blackboard.Set((int) BlackboardIds.Backup, new Behaviour.NodeResponseAction(Backup), _tree.id);
            _blackboard.Set((int) BlackboardIds.Stag, new Behaviour.NodeResponseAction(Stag), _tree.id);
            _blackboard.Set((int) BlackboardIds.Anticipate, new Behaviour.NodeResponseAction(Anticipate), _tree.id);
            _blackboard.Set((int) BlackboardIds.PlayerBehind, new Behaviour.NodeResponseAction(IsPlayerBehind),
                _tree.id);
            _blackboard.Set((int) BlackboardIds.TailHit, new Behaviour.NodeResponseAction(DoTailHit), _tree.id);
        }

        
        private NodeStates InTutorialState()
        {
            // if (Hp < stats.maxHp * 0.8f)
            //     return NodeStates.Failure;

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


        private NodeStates Anticipate()
        {
            if (bossStates.anticipating)
            {
                bossStates.anticipatingElapsed += Timer.deltaTime;

                if (bossStates.anticipatingElapsed >= bossStats.anticipationBaseDuration[bossStates.stage])
                {
                    bossStates.anticipating = false;
                    return NodeStates.Success;
                }

                return NodeStates.Running;
            }

            bossStates.anticipating = true;
            bossStates.anticipatingElapsed = 0f;

            return NodeStates.Running;
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

            bossStates.stagged = true;
            bossStates.stagElapsed = 0f;

            return NodeStates.Running;
        }


        private NodeStates DoChase()
        {
            var pos = transform.position;

            if (bossStates.moving)
            {
                Debug.DrawRay(bossStates.destination, Vector3.up * 2, Color.red, 5);

                bossStates.movingElapsed += Timer.deltaTime;

                if (bossStates.movingElapsed > bossStats.movingStepDuration)
                {
                    bossStates.moving = false;
                    return NodeStates.Success;
                }

                return NodeStates.Running;
            }

            bossStates.moving = true;
            bossStates.movingElapsed = 0f;
            bossStates.playerDirection = PlayerDistance(pos);
            bossStates.destination = pos + bossStates.playerDirection;

            return NodeStates.Running;
        }

        private NodeStates Backup()
        {
            var pos = transform.position;

            if (bossStates.moving)
            {
                Debug.DrawRay(bossStates.destination, Vector3.up * 2, Color.blue, 5);

                bossStates.movingElapsed += Timer.deltaTime;

                if (bossStates.movingElapsed > bossStats.backupStepDuration)
                {
                    bossStates.moving = false;
                    bossStates.backingUp = false;
                    return NodeStates.Success;
                }

                return NodeStates.Running;
            }

            var dir = PlayerDistance(pos);

            if (dir.magnitude > bossStats.backupDistance)
                return NodeStates.Success;
            
            bossStates.moving = true;
            bossStates.backingUp = true;
            bossStates.movingElapsed = 0f;
            bossStates.playerDirection = -dir;
            bossStates.destination = pos + bossStates.playerDirection;

            return NodeStates.Running;
        }

        private NodeStates DoSpit()
        {
            if (bossStates.spiting)
            {
                bossStates.spitingElapsed += Timer.deltaTime;

                if (!bossStates.spited && bossStates.spitingElapsed >= bossStats.spitDelay)
                {
                    bossStates.spited = true;
                    InstantiateSplash(bossStates.stage);
                }

                if (bossStates.spitingElapsed >= bossStats.spitDuration)
                {
                    bossStates.spiting = false;
                    return NodeStates.Success;
                }

                return NodeStates.Running;
            }

            bossStates.spiting = true;
            bossStates.spited = false;
            bossStates.spitingElapsed = 0f;

            return NodeStates.Running;
        }

        private NodeStates DoTailHit()
        {
            if (states.attacking)
            {
                states.attackElapsed += Timer.deltaTime;

                if (states.attackElapsed > stats.attacks[states.attackStage].delay &&
                    states.attackElapsed < stats.attacks[states.attackStage].delay +
                    stats.attacks[states.attackStage].damageDuration)
                {
                    EvaluateHit(tailPoints, ref _currentDamageTracker, bossStats.tailBaseDamage,
                        bossStats.attackRadius);
                }
                else if (states.attackElapsed < stats.attacks[states.attackStage].delay)
                {
                    _currentDamageTracker = tailPoints.Select(x => x.position).ToArray();
                }

                if (states.attackElapsed >= stats.attacks[states.attackStage].duration)
                {
                    states.attacking = false;
                    return NodeStates.Success;
                }

                return NodeStates.Running;
            }

            states.attacking = true;
            states.attackStage = 2;
            states.attackElapsed = 0f;

            return NodeStates.Running;
        }

        private NodeStates IsPlayerNear()
        {
            var dir = PlayerDistance(transform.position);

            if (!Physics.Raycast(new Ray(transform.position, dir), dir.magnitude * 2, _physicsSettings.player | _physicsSettings.movementBlockers))
            {
                return NodeStates.Failure;
            }
            

            if (dir.magnitude < bossStats.chaseDistance)
                return NodeStates.Success;

            return NodeStates.Failure;
        }

        private NodeStates IsPlayerBehind()
        {
            var dir = PlayerDistance(transform.position);

            if (Vector3.Angle(transform.forward, dir) < bossStats.backAttackAngle)
                return NodeStates.Success;

            return NodeStates.Failure;
        }

        
        protected override void CalculateDirection()
        {
            var dir = PlayerDistance(mouth.position);

            states.direction = dir;
        }


        private Vector3 PlayerDistance(Vector3 pos)
        {
            var d = _gameManager.Player.Position - pos;
            d.y = 0;
            return d;
        }


        private void EvaluateHit(Transform[] points, ref Vector3[] lastPositions, int damage, float radius)
        {
            if (_currentDamageTracker == null || points.Length != _currentDamageTracker.Length)
            {
                _currentDamageTracker = new Vector3[points.Length];
            }

            for (int i = 0, n = points.Length; i < n; i++)
            {
                var pos = points[i].position;
                var ini = lastPositions[i];
                var dir = pos - ini;

                Debug.DrawRay(ini, dir, Color.yellow, 1);

                if (_physics.CheckSphere(ini, dir, radius, _physicsSettings.player | _physicsSettings.hittable,
                    out RaycastHit hit))
                {
                    var damageable = hit.transform.GetComponent<IDamageable>();

                    if (damageable != null)
                    {
                        _signalBus.Fire(new FX.Hit(hit.point));
                        damageable.Damage(damage);
                        break;
                    }

                    var breakable = hit.transform.GetComponent<IBreakable>();
                    if (breakable != null)
                    {
                        _signalBus.Fire(new FX.Hit(hit.point));
                        breakable.Break();
                        continue;
                    }
                }

                lastPositions[i] = pos;
            }
        }


        private void InstantiateSplash(int difficulty)
        {
            var pos = new Vector3(mouth.position.x, _gameManager.Player.Center.y, mouth.position.z);
            for (int i = 0, n = bossStats.sprayCount + bossStats.sprayCount * difficulty * 3; i < n; i++)
            {
                var step = (i / (n * 0.5f)) * Mathf.PI * 2f;
                var dir = new Vector3(Mathf.Sin(step), 0, Mathf.Cos(step));

                dir = mouth.TransformDirection(dir);
                dir.y = 0;
                dir.Normalize();

                GetNextSpit();
                _bullets[_currentSpit].Shoot(pos, dir, bossStats.spitBaseSpeed,
                    i * bossStats.stagBaseDelay * (1 / ((float) difficulty + 1)));
                Debug.DrawRay(pos, dir * 5, Color.blue, 2);
            }
        }

        private void GetNextSpit()
        {
            for (int i = 0; i < bossStats.sprayCount; i++)
            {
                _currentSpit = (_currentSpit + 1) % _bullets.Length;

                if (!_bullets[_currentSpit].Running)
                    break;
            }
        }
    }
}