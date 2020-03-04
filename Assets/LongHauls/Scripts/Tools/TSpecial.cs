using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.U2D;

namespace TTiles
{
    public enum enum_TileDirection
    {
        Invalid = -1,
        Top=0,
        Right =1,
        Bottom =2,
        Left =3,
    }
    public interface ITileAxis
    {
        TileAxis m_Axis { get; }
    }
    [System.Serializable]
    public struct TileAxis
    {
        public int X;
        public int Y;
        public TileAxis(int _axisX, int _axisY)
        {
            X = _axisX;
            Y = _axisY;
        }

        public TileAxis[] nearbyFourTiles => new TileAxis[4] { new TileAxis(X - 1, Y), new TileAxis(X + 1, Y), new TileAxis(X, Y + 1), new TileAxis(X, Y - 1) };
        public static TileAxis operator -(TileAxis a) => new TileAxis(-a.X, -a.Y);
        public static bool operator ==(TileAxis a, TileAxis b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(TileAxis a, TileAxis b) => a.X != b.X || a.Y != b.Y;
        public static TileAxis operator -(TileAxis a, TileAxis b) => new TileAxis(a.X - b.X, a.Y - b.Y);
        public static TileAxis operator +(TileAxis a, TileAxis b) => new TileAxis(a.X + b.X, a.Y + b.Y);
        public static TileAxis operator *(TileAxis a, TileAxis b) => new TileAxis(a.X * b.X, a.Y * b.Y);
        public static TileAxis operator /(TileAxis a, TileAxis b) => new TileAxis(a.X / b.X, a.Y / b.Y);

        public static TileAxis operator *(TileAxis a, int b) => new TileAxis(a.X*b,a.Y*b);
        public static TileAxis operator /(TileAxis a, int b) => new TileAxis(a.X/b,a.Y/b);
        public TileAxis Inverse() => new TileAxis(Y, X);
        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode()=> base.GetHashCode();
        public override string ToString()=> X + "," + Y;
        public int SqrMagnitude => X * X + Y * Y;
        public int SqrLength => Mathf.Abs(X) + Mathf.Abs(Y);
        public static readonly TileAxis Zero = new TileAxis(0, 0);
        public static readonly TileAxis One = new TileAxis(1, 1);
        public static readonly TileAxis NegativeOne = new TileAxis(-1,-1);
        public static readonly TileAxis Back = new TileAxis(0, -1);
        public static readonly TileAxis Right = new TileAxis(1, 0);
        public static readonly TileAxis Forward = new TileAxis(0, 1);
    }
    public struct TileBounds
    {
        public TileAxis m_Origin { get; private set; }
        public TileAxis m_Size { get; private set; }
        public TileAxis m_End { get; private set; }
        public bool Contains(TileAxis axis) => axis.X >= m_Origin.X && axis.X <= m_End.X && axis.Y>=m_Origin.Y &&axis.Y<=m_End.Y;
        public override string ToString() => m_Origin.ToString() + "|" + m_Size.ToString();
        public bool Intersects(TileBounds targetBounds)
        {
            TileAxis[] sourceAxies = new TileAxis[] { m_Origin,  m_End,m_Origin+new TileAxis(m_Size.X,0),m_Origin+new TileAxis(0,m_Size.Y) };
            for (int i = 0; i < sourceAxies.Length; i++)
                if (targetBounds.Contains(sourceAxies[i]))
                    return true;
            TileAxis[] targetAxies = new TileAxis[] {  targetBounds.m_Origin, targetBounds.m_End, targetBounds.m_Origin + new TileAxis(targetBounds.m_Size.X, 0), targetBounds.m_Origin + new TileAxis(0, targetBounds.m_Size.Y) };
            for (int i = 0; i < targetAxies.Length; i++)
                if (Contains(targetAxies[i]))
                    return true;
            return false;
        }

        public TileBounds(TileAxis origin,TileAxis size)
        {
            m_Origin = origin;
            m_Size = size;
            m_End = m_Origin + m_Size;
        }
    }
    public static class TileTools
    {
        static int AxisDimensionTransformation(int x, int y, int width) => x + y * width;
        public static bool InRange<T>(this TileAxis axis, T[,] range)  => axis.X >= 0 && axis.X < range.GetLength(0) && axis.Y >= 0 && axis.Y < range.GetLength(1);
        public static bool InRange<T>(this TileAxis originSize, TileAxis sizeAxis, T[,] range) => InRange<T>(originSize + sizeAxis, range);
        public static int Get1DAxisIndex(TileAxis axis, int width) => AxisDimensionTransformation(axis.X, axis.Y, width);
        public static TileAxis GetAxisByIndex(int index, int width) => new TileAxis(index%width,index/width);
        public static T Get<T>(this T[,] tileArray, TileAxis axis) where T:class => axis.InRange(tileArray) ? tileArray[axis.X, axis.Y] :null;
        public static bool Get<T>(this T[,] tileArray,TileAxis axis,  TileAxis size, ref List<T> tileList)  where T:class
        {
            tileList.Clear();
            for (int i = 0; i < size.X; i++)
                for (int j = 0; j < size.Y; j++)
                {
                    if (!InRange( axis + new TileAxis(i, j), tileArray))
                        return false;
                    tileList.Add(tileArray.Get(axis + new TileAxis(i, j)));
                }
            return true;
        }

        public static List<TileAxis> GetAxisRange(int width,int height, TileAxis start, TileAxis end)
        {
            List<TileAxis> axisList = new List<TileAxis>();
            for (int i = start.X; i <= end.X; i++)
                for (int j = start.Y; j <= end.Y; j++)
                {
                    if (i < 0 ||j < 0 || i >= width || j >= height)
                        continue;
                    axisList.Add(new TileAxis(i, j));
                }
            return axisList;
        }

        public static List<TileAxis> GetAxisRange(int width, int height, TileAxis centerAxis,int radius)
        {
            List<TileAxis> axisList = new List<TileAxis>();
            int sqrRadius = radius * radius;
            for (int i =0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    if ((centerAxis - new TileAxis(i, j)).SqrMagnitude >sqrRadius)
                        continue;
                    axisList.Add(new TileAxis(i, j));
                }
            return axisList;
        }

        public static bool CheckIsEdge<T>(this T[,] tileArray, TileAxis axis) where T : class, ITileAxis => axis.X == 0 || axis.X == tileArray.GetLength(0) - 1 || axis.Y == 0 || axis.Y == tileArray.GetLength(1) - 1;
        
        public static TileAxis GetDirectionedSize(TileAxis size, enum_TileDirection direction) => (int)direction % 2 == 0 ? size : size.Inverse();
        public static Vector3 GetUnitScaleBySizeAxis(TileAxis directionedSize,int tileSize) => new Vector3(directionedSize.X, 1, directionedSize.Y) * tileSize;
        public static Vector3 GetLocalPosBySizeAxis(TileAxis directionedSize) => new Vector3(directionedSize.X, 0, directionedSize.Y);
        public static Quaternion ToRotation(this enum_TileDirection direction) => Quaternion.Euler(0, (int)direction * 90, 0);

        public static enum_TileDirection Next(this enum_TileDirection direction)
        {
            direction++;
            if ((int)direction > 3)
                direction = 0;
            return direction;
        }
        public static enum_TileDirection Inverse(this enum_TileDirection direction)
        {
            switch (direction)
            {
                case enum_TileDirection.Top:
                    return enum_TileDirection.Bottom;
                case enum_TileDirection.Bottom:
                    return enum_TileDirection.Top;
                case enum_TileDirection.Right:
                    return enum_TileDirection.Left;
                case enum_TileDirection.Left:
                    return enum_TileDirection.Right;
                default:
                    Debug.LogError("Error Direction Here");
                    return enum_TileDirection.Invalid;
            }
        }
        
        public static enum_TileDirection OffsetDirection(this TileAxis sourceAxis, TileAxis targetAxis)
        {
            TileAxis offset = targetAxis - sourceAxis;
            if (offset.Y == 0) return offset.X < 0 ? enum_TileDirection.Left : enum_TileDirection.Right;
            if (offset.X == 0) return offset.Y > 0 ? enum_TileDirection.Top : enum_TileDirection.Bottom;
            return enum_TileDirection.Invalid;
        }

        public static TileAxis DirectionAxis(this TileAxis sourceAxis,enum_TileDirection direction)
        {
            switch (direction)
            {
                case enum_TileDirection.Bottom:
                    return sourceAxis + new TileAxis(0,-1);
                case enum_TileDirection.Top:
                    return sourceAxis + new TileAxis(0, 1);
                case enum_TileDirection.Left:
                    return sourceAxis + new TileAxis(-1, 0);
                case enum_TileDirection.Right:
                    return sourceAxis + new TileAxis(1, 0);
            }
            Debug.LogError("Invlaid Direction Detected:"+direction);
            return new TileAxis(0,0);
        }

        public static readonly List<enum_TileDirection> m_FourDirections = new List<enum_TileDirection>() { enum_TileDirection.Top, enum_TileDirection.Right,enum_TileDirection.Bottom, enum_TileDirection.Left};
        
        public static void PathFindForClosestApproch<T>(this T[,] tileArray, T t1, T t2, List<T> tilePathsAdd,Action<T> OnEachTilePath=null, Predicate<T> stopPredicate=null, Predicate<T> invalidPredicate=null) where T:class,ITileAxis       //Temporary Solution, Not Required Yet
        {
            if (!t1.m_Axis.InRange(tileArray) || !t2.m_Axis.InRange(tileArray))
                Debug.LogError("Error Tile Not Included In Array");


            tilePathsAdd.Add(t1);
            TileAxis startTile=t1.m_Axis;
            for (; ; )
            {
                TileAxis nextTile=startTile;
                float minDistance = (startTile-t2.m_Axis).SqrMagnitude;
                float offsetDistance;
                TileAxis offsetTile;
                TileAxis[] nearbyFourTiles = startTile.nearbyFourTiles;
                for (int i = 0; i < nearbyFourTiles.Length; i++)
                {
                    offsetTile = nearbyFourTiles[i];
                    offsetDistance = (offsetTile-t2.m_Axis).SqrMagnitude;
                    if (offsetTile.InRange(tileArray) && offsetDistance < minDistance)
                    {
                        nextTile = offsetTile;
                        minDistance = offsetDistance;
                    }
                }

                if (nextTile == t2.m_Axis||(stopPredicate!=null&&stopPredicate(tileArray.Get(nextTile))))
                {
                    tilePathsAdd.Add(tileArray.Get(nextTile));
                    break;
                }

                if (invalidPredicate != null && invalidPredicate(tileArray.Get(nextTile)))
                {
                    tilePathsAdd.Clear();
                    break;
                }
                startTile = nextTile;
                T tilePath = tileArray.Get(startTile);
                OnEachTilePath?.Invoke(tilePath);
                tilePathsAdd.Add(tilePath);

                if (tilePathsAdd.Count > tileArray.Length) {
                    Debug.LogError("Error Path Found Failed");
                    break;
                }
            }
        }

        public static T TileEdgeRandom<T>(this T[,] tileArray ,  System.Random randomSeed=null, Predicate<T> predicate=null, List<enum_TileDirection> edgeOutcluded = null, int predicateTryCount=-1) where T : class, ITileAxis        //Target Edges Random Tile
        {
            if (edgeOutcluded != null && edgeOutcluded.Count > 3)
                Debug.LogError("Can't Outclude All Edges!");

            if (predicateTryCount == -1) predicateTryCount = int.MaxValue;

            List<enum_TileDirection> edgesRandom = new List<enum_TileDirection>(m_FourDirections) { };
            if (edgeOutcluded!=null) edgesRandom.RemoveAll(p=>edgeOutcluded.Contains(p));
            
            int axisX=-1,axisY=-1;
            int tileWidth = tileArray.GetLength(0), tileHeight = tileArray.GetLength(1);
            T targetTile = null;
            for (int i = 0; i < predicateTryCount; i++)
            {
                enum_TileDirection randomDirection = edgesRandom.RandomItem(randomSeed);
                switch (randomDirection)
                {
                    case enum_TileDirection.Bottom:
                        axisX = randomSeed.Next(tileWidth-1)+1;
                        axisY = 0;
                        break;
                    case enum_TileDirection.Top:
                        axisX = randomSeed.Next(tileWidth-1);
                        axisY = tileHeight - 1;
                        break;
                    case enum_TileDirection.Left:
                        axisX = 0;
                        axisY = randomSeed.Next(tileHeight-1);
                        break;
                    case enum_TileDirection.Right:
                        axisX = tileWidth - 1;
                        axisY = randomSeed.Next(tileHeight-1)+1;
                        break;
                }
                targetTile = tileArray[axisX, axisY];
                if (predicate == null || predicate(targetTile))
                {
                    if(edgeOutcluded!=null) edgeOutcluded.Add(randomDirection);
                    break;
                }
            }
            return targetTile;
        }

        public static bool ArrayNearbyContains<T>(this T[,] tileArray, TileAxis origin, Predicate<T> predicate) where T : class,ITileAxis
        {
            TileAxis[] nearbyTiles = origin.nearbyFourTiles;
            for (int i = 0; i < nearbyTiles.Length; i++)
            {
                if (origin.InRange(tileArray)&&!predicate(tileArray.Get(nearbyTiles[i])))
                    return false;
            }
            return true;
        }

        public static List<T> TileRandomFill<T>(this T[,] tileArray,System.Random seed,TileAxis originAxis,Action<T> OnEachFill,Predicate<T> availableAxis,int fillCount) where T:class,ITileAxis
        {
            List<T> targetList = new List<T>();
            T targetAdd = tileArray.Get(originAxis);
            OnEachFill(targetAdd);
            targetList.Add(targetAdd);
            for (int i = 0; i < fillCount; i++)
            {
                T temp = targetList[i];
                m_FourDirections.TraversalRandomBreak((enum_TileDirection randomDirection) => {
                    TileAxis axis = temp.m_Axis.DirectionAxis(randomDirection);
                    if (axis.InRange(tileArray))
                    {
                        targetAdd= tileArray.Get(axis);
                        if (availableAxis(targetAdd))
                        {
                            OnEachFill(targetAdd);
                            targetList.Add(targetAdd);
                            return true;
                        }
                    }
                    return false;
                }, seed);
            }
            return targetList;
        }
    }
}
namespace TTime
{
    public static class TTimeTools
    {
        public static int GetTimeStampNow() => GetTimeStamp(DateTime.Now);
        public static int GetTimeStamp(DateTime dt)
        {
            DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            int timeStamp = Convert.ToInt32((dt - dateStart).TotalSeconds);
            return timeStamp;
        }

        public static string GetHMS(int stamp)
        {
            int hours = stamp / 3600;
            int minutes = (stamp - hours * 3600) / 60;
            int seconds = stamp - hours * 3600 - minutes * 60;
            return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, + minutes, seconds);
        }

        public static DateTime GetDateTime(int timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = ((long)timeStamp * 10000000);
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime targetDt = dtStart.Add(toNow);
            return targetDt;
        }

        public static DateTime GetDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
    }
}
namespace TSpecialClasses          //Put Some Common Shits Into Specifical Classes, Tightly Attached To CoroutineManager Cause Not Using Monobehaviour
{
    //Translate Ragdoll To Animation Or Animatoion To Ragdoll
    public class RagdollAnimationTransition : ICoroutineHelperClass
    {
        class BodyPart
        {
            public Rigidbody m_rigidBody { get; private set; }
            public Quaternion m_startRot { get; private set; }
            public Vector3 m_startPos { get; private set; }
            public BodyPart(Rigidbody rb)
            {
                m_rigidBody = rb;
                m_startRot = rb.transform.localRotation;
                m_startPos = rb.transform.localPosition;
            }
        }
        Animator m_Animator;
        BodyPart m_TransitionCenter;
        List<BodyPart> m_BodyParts = new List<BodyPart>();
        public RagdollAnimationTransition(Animator animator, Rigidbody[] rigidbodys, Transform transitionCenter)
        {
            m_Animator = animator;
            TCommon.Traversal(rigidbodys, (Rigidbody rb) => {
                BodyPart part = new BodyPart(rb);
                m_BodyParts.Add(part);
                if (rb.transform == transitionCenter)
                    m_TransitionCenter = part;
            });
        }
        public void Reset(bool isAnimation)
        {

            this.StopSingleCoroutine(1);
            if (isAnimation)
            {
                SetAnimActivate(false);
                SetKinematic(true);
                TCommon.Traversal(m_BodyParts, (BodyPart part) =>
                {
                    part.m_rigidBody.transform.localPosition = part.m_startPos;
                    part.m_rigidBody.transform.localRotation = part.m_startRot;
                });
                SetAnimActivate(true);
            }
            else
            {
                SetAnimActivate(false);
                SetKinematic(false);
            }

        }
        public void SetState(bool isAnimation)
        {
            if (isAnimation)
            {
                SetAnimActivate(false);
                SetKinematic(true);
                Vector3 offset = m_TransitionCenter.m_rigidBody.position - m_Animator.transform.position;
                m_Animator.transform.position += offset;
                this.StartSingleCoroutine(1, TIEnumerators.ChangeValueTo((float value) =>
                {
                    TCommon.Traversal(m_BodyParts, (BodyPart part) => {
                        part.m_rigidBody.transform.localPosition = Vector3.Lerp(part.m_rigidBody.transform.localPosition, part.m_startPos, value);
                        part.m_rigidBody.transform.localRotation = Quaternion.Lerp(part.m_rigidBody.transform.localRotation, part.m_startRot, value);
                    });
                }, 0, 1, .5f, () =>
                {
                    SetAnimActivate(true);
                }));
            }
            else
            {
                this.StopSingleCoroutine(1);
                SetKinematic(false);
                SetAnimActivate(false);
            }
        }

        void SetAnimActivate(bool active)
        {
            m_Animator.enabled = active;
        }
        void SetKinematic(bool active)
        {
            TCommon.Traversal(m_BodyParts, (BodyPart part) =>
            {
                part.m_rigidBody.isKinematic = active;
            });
        }
    }

    public class AnimatorControlBase
    {
        public Animator m_Animator { get; private set; }
        public AnimatorControlBase(Animator _animator)
        {
            m_Animator = _animator;
        }
    }

    public class AnimationControlBase
    {
        public Animation m_Animation { get; private set; }
        public AnimationControlBase(Animation _animation,bool startFromOn=true)
        {
            m_Animation = _animation;
            m_Animation.playAutomatically = false;
            SetPlayPosition(startFromOn);
        }
        public void SetPlayPosition(bool forward) => m_Animation.clip.SampleAnimation(m_Animation.gameObject, forward ? 0 : m_Animation.clip.length);

        public void Play(bool forward)
        {
            m_Animation[m_Animation.clip.name].speed = forward ? 1 : -1;
            m_Animation[m_Animation.clip.name].normalizedTime = forward ? 0 : 1;
            m_Animation.Play(m_Animation.clip.name);
        }

        public void Stop()
        {
            m_Animation.Stop();
        }

    }

    public class ParticleControlBase
    {
        public Transform transform { get; private set; }
        public ParticleSystem[] m_Particles { get; private set; }
        public ParticleControlBase(Transform _transform)
        {
            transform = _transform;
            m_Particles = transform?transform.GetComponentsInChildren<ParticleSystem>():new ParticleSystem[0];
        }
        public void Play()
        {
            m_Particles.Traversal((ParticleSystem particle) => {
                particle.Simulate(0, true, true);
                particle.Play(true);
                ParticleSystem.MainModule main = particle.main;
                main.playOnAwake = true;
            });
        }
        public void Stop()
        {
            m_Particles.Traversal((ParticleSystem particle) => {
                particle.Stop(true);
                ParticleSystem.MainModule main = particle.main;
                main.playOnAwake = false;
            });
        }
        public void Clear()
        {
            m_Particles.Traversal((ParticleSystem particle) => { particle.Clear(); });
        }
        public void SetActive(bool active)
        {
            m_Particles.Traversal((ParticleSystem particle) => { particle.transform.SetActivate(active); });
        }
    }

    //Navigation AI System Chase/Follow/Attack/Idle ETC...
    public class NavigationAgentAI<T> : SimpleMonoLifetime, ICoroutineHelperClass where T : MonoBehaviour
    {
        public enum enum_AIState
        {
            Invalid = -1,
            Idle,           //Same As Stop
            WalkAround,
            Follow,
        }

        public const float CF_AICheckTime = .3f;
        protected float F_MoveSpeed;
        protected float F_DetectRange;
        protected float F_AttackRange;
        protected float F_FollowRange;
        protected int i_targetCastLayer;          //Cast Layer Used For Optimizing
        protected Func<T,bool, bool> TargetAvailable;
        protected Func<T, float> OnAttackTargetCoolDown;
        protected Action OnAITick;
        protected NavMeshAgent m_Agent;
        protected NavMeshObstacle m_Obstacle;
        protected Transform transform;
        public T m_AttackTarget { get; private set; }
        public T m_FollowTarget { get; private set; }
        public enum_AIState E_CurrentAI { get; private set; } = enum_AIState.Invalid;
        public bool B_Walking { get; private set; } = false;
        public bool B_Aggressive { get; private set; } = false;
        public bool B_Attacking { get; private set; } = false; virtual 
        public bool B_HaveAttackTarget => m_AttackTarget != null;
        public bool B_HaveFollowTarget => m_FollowTarget != null;
        public bool TargetInRange(T target,float range)
        {
            return  TCommon.GetXZDistance(transform.position, target.transform.position) < range ;
        }
        public bool B_AgentEnabled
        {
            get
            {
                return m_Agent.enabled;
            }
            set
            {
                if (value)
                {
                    m_Obstacle.enabled = false;
                    m_Agent.enabled = true;
                    m_Agent.isStopped = false;
                }
                else
                {
                    if (m_Agent.enabled)
                    {
                        m_Agent.isStopped = true;
                        m_Agent.enabled = false;
                    }
                    m_Obstacle.enabled = true;
                }
            }
        }
        public NavigationAgentAI(Transform _transform, int _castLayers,float _moveSpeed, float _detectRange,float _attackRange,float _followRange, Func<T,bool, bool> _TargetValiable, Func<T, float> _OnAttackTargetCooldown,Action _OnAITick=null)
        {
            transform = _transform;
            m_Agent = transform.GetComponent<NavMeshAgent>();
            m_Obstacle = transform.GetComponent<NavMeshObstacle>();
            F_DetectRange = _detectRange;
            F_AttackRange = _attackRange;
            F_FollowRange = _followRange;
            F_MoveSpeed = _moveSpeed;
            i_targetCastLayer = _castLayers;
            TargetAvailable = _TargetValiable;
            OnAttackTargetCoolDown = _OnAttackTargetCooldown;
            OnAITick = _OnAITick;
            m_Agent.speed = F_MoveSpeed;
            B_AgentEnabled = false;
            m_AttackTarget = null;
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            this.StopAllSingleCoroutines();
        }
        public void OnDead()
        {
            OnDestroy();
            B_AgentEnabled = false;
        }

        public void Start(enum_AIState state, bool aggressive,T followTarget)
        {
            E_CurrentAI = state;
            B_Aggressive = aggressive;
            m_FollowTarget = followTarget;
            this.StartSingleCoroutine(1, IE_MainAI());
        }

        public void SetTarget(T target)
        {
            m_AttackTarget = target;
            this.StopSingleCoroutine(1);
            this.StopSingleCoroutine(3);
            this.StartSingleCoroutine(2, Sub_ChaseAttack());
        }

        bool OverwatchNearby()
        {
            Collider[] targets = Physics.OverlapSphere(transform.position, F_DetectRange, i_targetCastLayer);
            for (int i = 0; i < targets.Length; i++)
            {
                T temp = targets[i].GetComponent<T>();
                if (TargetAvailable(temp,false))
                {
                    SetTarget( temp);
                    return true;
                }
            }
            return false;
        }


        #region AI Behaviours 
        IEnumerator IE_MainAI()
        {
            B_AgentEnabled = false;
            for (; ; )
            {
                OnAITick?.Invoke();

                if (B_Aggressive && OverwatchNearby())
                    yield break;

                switch(E_CurrentAI)
                {
                    default:
                        break;
                    case enum_AIState.Follow:
                        {
                            B_AgentEnabled = m_FollowTarget != null &&TargetAvailable(m_FollowTarget,true) && !TargetInRange(m_FollowTarget,F_FollowRange);

                            if (B_AgentEnabled)
                                m_Agent.SetDestination(m_FollowTarget.transform.position);
                        }
                        break;
                    case enum_AIState.Idle:
                        {
                            
                        }
                        break;
                    case enum_AIState.WalkAround:
                        {
                            
                        }
                        break;
                }
                yield return new WaitForSeconds(CF_AICheckTime);
            }
        }
        
        IEnumerator Sub_ChaseAttack()
        {
            B_AgentEnabled = true;
            for (; ; )
            {
                OnAITick?.Invoke();
                B_Attacking = false;
                this.StopSingleCoroutine(3);
                if (m_AttackTarget == null || !TargetAvailable(m_AttackTarget,false))
                {
                    m_AttackTarget = null;
                    Start(E_CurrentAI, B_Aggressive,m_FollowTarget);
                    yield break;
                }

                B_AgentEnabled = !TargetInRange(m_AttackTarget,F_AttackRange);

                if (B_AgentEnabled)
                {
                    m_Agent.SetDestination(m_AttackTarget.transform.position);
                    yield return new WaitForSeconds(CF_AICheckTime);
                }
                else
                {
                    B_Attacking = true;
                    this.StartSingleCoroutine(3, Sub_LookAt(m_AttackTarget));
                    yield return new WaitForSeconds(OnAttackTargetCoolDown(m_AttackTarget));
                }
            }
        }
        IEnumerator Sub_LookAt(T target)
        {
            for (; ; )
            {
                if (!B_Attacking||B_AgentEnabled)
                    yield break;

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(TCommon.GetXZLookDirection(transform.position, target.transform.position), Vector3.up), .1f);
                yield return null;
            }
        }
        #endregion
    }
    #region UI
    public class ValueLerpBase
    {
        float m_check;
        float m_duration;
        protected float m_value { get; private set; }
        protected float m_previousValue { get; private set; }
        protected float m_targetValue { get; private set; }
        Action<float> OnValueChanged;
        public ValueLerpBase(float startValue, Action<float> _OnValueChanged)
        {
            m_targetValue = startValue;
            m_previousValue = startValue;
            m_value = m_targetValue;
            OnValueChanged = _OnValueChanged;
            OnValueChanged(m_value);
        }

        protected virtual void ChangeValue(float value,float duration)
        {
            if (value == m_targetValue)
                return;
            m_duration = duration;
            m_check = m_duration;
            m_previousValue = m_value;
            m_targetValue = value;
        }
        public void TickDelta(float deltaTime)
        {
            if (m_check <= 0)
                return;
            m_check -= deltaTime;
            m_value = GetValue(m_check / m_duration);
            OnValueChanged(m_value);
        }
        protected virtual float GetValue(float checkLeftParam)
        {
            Debug.LogError("Override This Please");
            return 0;
        }
    }

    public class ValueLerpSeconds: ValueLerpBase
    {
        float m_perSecondValue;
        float m_maxDuration;
        float m_maxDurationValue;
        public ValueLerpSeconds(float startValue, float perSecondValue,float maxDuration, Action<float> _OnValueChanged) :base(startValue,_OnValueChanged)
        {
            m_perSecondValue = perSecondValue;
            m_maxDuration = maxDuration;
            m_maxDurationValue = m_perSecondValue * maxDuration;
        }

        public void ChangeValue(float value)=> base.ChangeValue(value,Mathf.Abs(value-m_value)> m_maxDurationValue? m_maxDuration : Mathf.Abs((value - m_value)) / m_perSecondValue);
        
        protected override float GetValue(float checkLeftParam) => Mathf.Lerp(m_previousValue, m_targetValue, 1 - checkLeftParam);
    }


    public class ValueChecker<T>
    {
        public T check1 { get; private set; }
        public ValueChecker(T _check)
        {
            check1 = _check;
        }

        public bool Check(T target)
        {
            if (check1.Equals(target))
                return false;
            check1 = target;
            return true;
        }
    }

    public class ValueChecker<T, Y> : ValueChecker<T>
    {
        public Y check2 { get; private set; }
        public ValueChecker(T temp1, Y temp2) : base(temp1)
        {
            check2 = temp2;
        }

        public bool Check(T target1, Y target2)
        {
            bool check1 = Check(target1);
            bool check2= Check(target2);
            return check1 || check2;
        }
        public bool Check(Y target2)
        {
            if (check2.Equals(target2))
                return false;
            check2 = target2;
            return true;
        }
    }
    #endregion
}
#region Extra Structs/Classes

public class TimeCounter
{
    float m_duration;
    public bool m_Timing => m_timeCheck > 0;
    public float m_timeCheck { get; private set; } = -1;
    public float m_TimeLeftScale =>m_duration==0?0:m_timeCheck / m_duration;
    public TimeCounter(float duration = 0) { SetTimer(duration); }
    public void SetTimer(float duration)
    {
        m_duration = duration;
        m_timeCheck = m_duration;
    }
    public void Reset() => m_timeCheck = m_duration;
    public void Tick(float deltaTime)
    {
        if (m_timeCheck <= 0)
            return;
        m_timeCheck -= deltaTime;
    }
}

[Serializable]
public struct RangeFloat
{
    public float start;
    public float length;
    public float end => start + length;
    public RangeFloat(float _start, float _length)
    {
        start = _start;
        length = _length;
    }
}

[Serializable]
public struct RangeInt
{
    public int start;
    public int length;
    public int end => start + length;
    public RangeInt(int _start, int _length)
    {
        start = _start;
        length = _length;
    }
}
public interface IXmlPhrase
{
     string ToXMLData();
}
public class TXmlPhrase : SingleTon<TXmlPhrase>
{
    Dictionary<Type, Func<object, string>> dic_valueToXmlData = new Dictionary<Type, Func<object, string>>();
    Dictionary<Type, Func<string, object>> dic_xmlDataToValue = new Dictionary<Type, Func<string, object>>();
    Dictionary<Type, Func<object>> dic_xmlDataDefault = new Dictionary<Type, Func<object>>();
    public TXmlPhrase()     //Common Smaller Forms Translate/Retranslate
    {
        dic_valueToXmlData.Add(typeof(int), (object target) => { return target.ToString(); });
        dic_xmlDataToValue.Add(typeof(int), (string xmlData) => { return int.Parse(xmlData); });
        dic_xmlDataDefault.Add(typeof(int), () => { return -1; });
        dic_valueToXmlData.Add(typeof(long), (object target) => { return target.ToString(); });
        dic_xmlDataToValue.Add(typeof(long), (string xmlData) => { return long.Parse(xmlData); });
        dic_xmlDataDefault.Add(typeof(long), () => { return -1; });
        dic_valueToXmlData.Add(typeof(double), (object target) => { return target.ToString(); });
        dic_xmlDataToValue.Add(typeof(double), (string xmlData) => { return double.Parse(xmlData); });
        dic_xmlDataDefault.Add(typeof(double), () => { return -1; });
        dic_valueToXmlData.Add(typeof(float), (object target) => { return target.ToString(); });
        dic_xmlDataToValue.Add(typeof(float), (string xmlData) => { return float.Parse(xmlData); });
        dic_xmlDataDefault.Add(typeof(float), () => { return -1f; });
        dic_valueToXmlData.Add(typeof(string), (object target) => { return target as string; });
        dic_xmlDataToValue.Add(typeof(string), (string xmlData) => { return xmlData; });
        dic_xmlDataDefault.Add(typeof(string), () => { return "Invalid"; });
        dic_valueToXmlData.Add(typeof(bool), (object data) => { return (((bool)data ? 1 : 0)).ToString(); });
        dic_xmlDataToValue.Add(typeof(bool), (string xmlData) => { return int.Parse(xmlData) == 1; });
        dic_xmlDataDefault.Add(typeof(bool), () => { return false; });
        dic_valueToXmlData.Add(typeof(RangeInt), (object data) => { return ((RangeInt)data).start.ToString() + "," + ((RangeInt)data).length.ToString(); });
        dic_xmlDataToValue.Add(typeof(RangeInt), (string xmlData) => { string[] split = xmlData.Split(','); return new RangeInt(int.Parse(split[0]), int.Parse(split[1])); });
        dic_xmlDataDefault.Add(typeof(RangeInt), () => { return new RangeInt(-1, 0); });
        dic_valueToXmlData.Add(typeof(RangeFloat), (object data) => { return ((RangeFloat)data).start.ToString() + "," + ((RangeFloat)data).length.ToString(); });
        dic_xmlDataToValue.Add(typeof(RangeFloat), (string xmlData) => { string[] split = xmlData.Split(','); return new RangeFloat(float.Parse(split[0]), float.Parse(split[1])); });
        dic_xmlDataDefault.Add(typeof(RangeFloat), () => { return new RangeFloat(-1, 0); });
    }
    public static TXmlPhrase Phrase=>Instance;
    public string this[object value]
    {
        get
        {
            StringBuilder sb_xmlData = new StringBuilder();
            Type type = value.GetType();
            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type listType = type.GetGenericArguments()[0];
                    foreach (object obj in value as IEnumerable)
                    {
                        sb_xmlData.Append(ValueToXmlData(listType, obj));
                        sb_xmlData.Append(";");
                    }
                    if(sb_xmlData.Length!=0)
                        sb_xmlData.Remove(sb_xmlData.Length - 1, 1);
                }
                else if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    Type keyType = type.GetGenericArguments()[0];
                    Type valueType = type.GetGenericArguments()[1];
                    foreach (DictionaryEntry obj in (IDictionary)value)
                    {
                        sb_xmlData.Append(ValueToXmlData(keyType, obj.Key));
                        sb_xmlData.Append(":");
                        sb_xmlData.Append(ValueToXmlData(valueType, obj.Value));
                        sb_xmlData.Append(";");
                    }
                    if (sb_xmlData.Length != 0)
                        sb_xmlData.Remove(sb_xmlData.Length - 1, 1);
                }
            }
            else
            {
                sb_xmlData.Append(ValueToXmlData(type, value));
            }
            return sb_xmlData.ToString();
        }
    }
    public object this[Type type, string xmlData]
    {
        get
        {
            object obj_target = null;
            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type listType = type.GetGenericArguments()[0];
                    IList iList_Target = (IList)Activator.CreateInstance(type);
                    string[] as_split = xmlData.Split(';');
                    if (as_split.Length != 1 || as_split[0] != "")
                        for (int i = 0; i < as_split.Length; i++)
                        {
                            iList_Target.Add(XmlDataToValue(listType, as_split[i]));
                        }
                    obj_target = iList_Target;
                }
                else if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    Type keyType = type.GetGenericArguments()[0];
                    Type valueType = type.GetGenericArguments()[1];
                    IDictionary iDic_Target = (IDictionary)Activator.CreateInstance(type);
                    string[] as_split = xmlData.Split(';');
                    if (as_split.Length != 1 || as_split[0] != "")
                        for (int i = 0; i < as_split.Length; i++)
                        {
                            string[] as_subSplit = as_split[i].Split(':');
                            iDic_Target.Add(XmlDataToValue(keyType, as_subSplit[0])
                                , XmlDataToValue(valueType, as_subSplit[1]));
                        }
                    obj_target = iDic_Target;
                }
            }
            else
            {
                obj_target = XmlDataToValue(type, xmlData);
            }
            return obj_target;
        }
    }
    public object GetDefault(Type type) => dic_xmlDataDefault.ContainsKey(type) ? dic_xmlDataDefault[type]() : type.IsValueType ? Activator.CreateInstance(type) : null;
    string ValueToXmlData(Type type, object value)
    {
        if (typeof(IXmlPhrase).IsAssignableFrom(type))
            return ((IXmlPhrase)value).ToXMLData(); 

        if (type.IsEnum)
            return value.ToString();
        if (!dic_valueToXmlData.ContainsKey(type))
            Debug.LogWarning("Xml Error Invlid Type:" + type.ToString() + " For Base Type To Phrase");
        return dic_valueToXmlData[type](value);
    }
    object XmlDataToValue(Type type, string xmlData)
    {
        if (typeof(IXmlPhrase).IsAssignableFrom(type))
            return Activator.CreateInstance(type,xmlData);

        if (type.IsEnum)
            return Enum.Parse(type,xmlData);

        if (!dic_xmlDataToValue.ContainsKey(type))
            Debug.LogWarning("Xml Error Invlid Type:" + type.ToString() + " For Xml Data To Phrase");
        
        return dic_xmlDataToValue[type](xmlData);
    }
}
public static class Physics_Extend
{
    public static RaycastHit[] BoxCastAll(Vector3 position,Vector3 forward,Vector3 up,Vector3 boxBounds,int layerMask=-1)
    {
        float castBoxLength = .1f;
        return Physics.BoxCastAll(position + forward* castBoxLength / 2f, new Vector3(boxBounds.x / 2, boxBounds.y / 2, castBoxLength/2), forward, Quaternion.LookRotation(forward, up), boxBounds.z - castBoxLength, layerMask);
    }

    public static RaycastHit[] TrapeziumCastAll(Vector3 position, Vector3 forward,Vector3 up,Vector4 trapeziumInfo,int layerMask=-1,int castCount=8)
    {
        List<RaycastHit> hitsList = new List<RaycastHit>();
        float castLength = trapeziumInfo.z / castCount;
        for (int i = 0; i < castCount; i++)
        {
            Vector3 boxPos = position + forward * castLength * i;
            Vector3 boxInfo = new Vector3(trapeziumInfo.x+(trapeziumInfo.w-trapeziumInfo.x)*i/castCount,trapeziumInfo.y,castLength);
            RaycastHit[] hits = BoxCastAll(boxPos,forward,up,boxInfo,layerMask);
            for (int j = 0; j < hits.Length; j++)
            {
                bool b_continue=false;

                for (int k = 0; k < hitsList.Count; k++)
                    if (hitsList[k].collider == hits[j].collider)
                        b_continue = true;

                if (b_continue)
                    continue;

                hitsList.Add(hits[j]);
            }
        }
        return hitsList.ToArray();
    }
}
#endregion
#region UI Classes
public class AtlasLoader
{
    protected Dictionary<string, Sprite> m_SpriteDic { get; private set; } = new Dictionary<string, Sprite>();
    public bool Contains(string name) => m_SpriteDic.ContainsKey(name);
    public Sprite this[string name]
    {
        get
        {
            if (!m_SpriteDic.ContainsKey(name))
            {
                Debug.LogWarning("Null Sprites Found |" + name + "|");
                return m_SpriteDic.Values.First();
            }
            return m_SpriteDic[name];
        }
    }
    public AtlasLoader(SpriteAtlas atlas)
    {
        Sprite[] allsprites=new Sprite[atlas.spriteCount];
        atlas.GetSprites(allsprites);
        allsprites.Traversal((Sprite sprite)=> { string name = sprite.name.Replace("(Clone)", ""); m_SpriteDic.Add(name, sprite); });
    }
}

public class AtlasAnim:AtlasLoader
{
    int animIndex=0;
    List<Sprite> m_Anims;
    public AtlasAnim(SpriteAtlas atlas):base(atlas)
    {
        m_Anims = m_SpriteDic.Values.ToList();
        m_Anims.Sort((a,b) =>
        {
            int index1 = int.Parse(System.Text.RegularExpressions.Regex.Replace(a.name, @"[^0-9]+", ""));
            int index2 = int.Parse(System.Text.RegularExpressions.Regex.Replace(b.name, @"[^0-9]+", ""));
            return   index1- index2;
        });
    }

    public Sprite Reset()
    {
        animIndex = 0;
        return m_Anims[animIndex];
    }

    public Sprite Tick()
    {
        animIndex++;
        if (animIndex == m_Anims.Count)
            animIndex = 0;
        return m_Anims[animIndex];
    }
}
#endregion

#if UNITY_EDITOR
#region Editor Class

public static class Gizmos_Extend
{
    public static void DrawWireCapsule(Vector3 _pos, Quaternion _rot, Vector3 _scale, float _radius, float _height)
    {
        using (new UnityEditor.Handles.DrawingScope(Gizmos.color, Matrix4x4.TRS(_pos, _rot, _scale)))
        {
            if (_height > _radius*2)
            {
                Vector3 offsetPoint =  Vector3.up * (_height - (_radius * 2)) / 2;

                UnityEditor.Handles.DrawWireArc(offsetPoint, Vector3.forward, Vector3.right, 180, _radius);
                UnityEditor.Handles.DrawWireArc(offsetPoint, Vector3.right, Vector3.forward, -180, _radius);
                UnityEditor.Handles.DrawWireArc(-offsetPoint, Vector3.forward, Vector3.right, -180, _radius);
                UnityEditor.Handles.DrawWireArc(-offsetPoint, Vector3.right, Vector3.forward, 180, _radius);

                UnityEditor.Handles.DrawWireDisc(offsetPoint, Vector3.up, _radius);
                UnityEditor.Handles.DrawWireDisc(-offsetPoint, Vector3.up, _radius);

                UnityEditor.Handles.DrawLine(offsetPoint + Vector3.left * _radius, -offsetPoint + Vector3.left * _radius);
                UnityEditor.Handles.DrawLine(offsetPoint - Vector3.left * _radius, -offsetPoint - Vector3.left * _radius);
                UnityEditor.Handles.DrawLine(offsetPoint + Vector3.forward * _radius, -offsetPoint + Vector3.forward * _radius);
                UnityEditor.Handles.DrawLine(offsetPoint - Vector3.forward * _radius, -offsetPoint - Vector3.forward * _radius);
            }
            else
            {
                UnityEditor.Handles.DrawWireDisc(Vector3.zero, Vector3.up, _radius);
                UnityEditor.Handles.DrawWireDisc(Vector3.zero, Vector3.right, _radius);
                UnityEditor.Handles.DrawWireDisc(Vector3.zero, Vector3.forward, _radius);
            }
        }
    }
    public static void DrawWireCube(Vector3 _pos, Quaternion _rot, Vector3 _cubeSize)
    {
        using (new UnityEditor.Handles.DrawingScope(Gizmos.color, Matrix4x4.TRS(_pos, _rot, UnityEditor.Handles.matrix.lossyScale)))
        {
            float halfWidth, halfHeight, halfLength;
            halfWidth = _cubeSize.x/2;
            halfHeight = _cubeSize.y/2;
            halfLength = _cubeSize.z/2;

            UnityEditor.Handles.DrawLine(new Vector3(halfWidth, halfHeight, halfLength), new Vector3(-halfWidth, halfHeight, halfLength));
            UnityEditor.Handles.DrawLine(new Vector3(halfWidth, halfHeight, -halfLength), new Vector3(-halfWidth, halfHeight, -halfLength));
            UnityEditor.Handles.DrawLine(new Vector3(halfWidth, -halfHeight, halfLength), new Vector3(-halfWidth, -halfHeight, halfLength));
            UnityEditor.Handles.DrawLine(new Vector3(halfWidth, -halfHeight, -halfLength), new Vector3(-halfWidth, -halfHeight, -halfLength));

            UnityEditor.Handles.DrawLine(new Vector3(halfWidth, halfHeight, halfLength), new Vector3(halfWidth, -halfHeight, halfLength));
            UnityEditor.Handles.DrawLine(new Vector3(-halfWidth, halfHeight, halfLength), new Vector3(-halfWidth, -halfHeight, halfLength));
            UnityEditor.Handles.DrawLine(new Vector3(halfWidth, halfHeight, -halfLength), new Vector3(halfWidth, -halfHeight, -halfLength));
            UnityEditor.Handles.DrawLine(new Vector3(-halfWidth, halfHeight, -halfLength), new Vector3(-halfWidth, -halfHeight, -halfLength));

            UnityEditor.Handles.DrawLine(new Vector3(halfWidth,halfHeight,halfLength), new Vector3(halfWidth, halfHeight, -halfLength));
            UnityEditor.Handles.DrawLine(new Vector3(-halfWidth, halfHeight, halfLength), new Vector3(-halfWidth, halfHeight, -halfLength));
            UnityEditor.Handles.DrawLine(new Vector3(halfWidth, -halfHeight, halfLength), new Vector3(halfWidth, -halfHeight, -halfLength));
            UnityEditor.Handles.DrawLine(new Vector3(-halfWidth, -halfHeight, halfLength), new Vector3(-halfWidth, -halfHeight, -halfLength));
        }
    }
    public static void DrawArrow(Vector3 _pos, Quaternion _rot, Vector3 _arrowSize)
    {
        using (new UnityEditor.Handles.DrawingScope(Gizmos.color, Matrix4x4.TRS(_pos, _rot, UnityEditor.Handles.matrix.lossyScale)))
        {
            Vector3 capBottom = Vector3.forward * _arrowSize.z/2;
            Vector3 capTop = Vector3.forward * _arrowSize.z;
            float rootRadius = _arrowSize.x / 4;
            float capBottomSize = _arrowSize.x/2;
            UnityEditor.Handles.DrawWireDisc(Vector3.zero, Vector3.forward, rootRadius);
            UnityEditor.Handles.DrawWireDisc(capBottom, Vector3.forward, rootRadius);
            UnityEditor.Handles.DrawLine(Vector3.up*rootRadius,capBottom+Vector3.up*rootRadius);
            UnityEditor.Handles.DrawLine(-Vector3.up * rootRadius, capBottom - Vector3.up * rootRadius);
            UnityEditor.Handles.DrawLine(Vector3.right * rootRadius, capBottom + Vector3.right * rootRadius);
            UnityEditor.Handles.DrawLine(-Vector3.right * rootRadius, capBottom - Vector3.right * rootRadius);

            UnityEditor.Handles.DrawWireDisc(capBottom, Vector3.forward, capBottomSize);
            UnityEditor.Handles.DrawLine(capBottom+Vector3.up * capBottomSize, capTop);
            UnityEditor.Handles.DrawLine(capBottom - Vector3.up * capBottomSize, capTop);
            UnityEditor.Handles.DrawLine(capBottom+Vector3.right * capBottomSize, capTop);
            UnityEditor.Handles.DrawLine(capBottom +- Vector3.right * capBottomSize, capTop);
        }
    }
    public static void DrawCylinder(Vector3 _pos, Quaternion _rot, float _radius, float _height)
    {
        using (new UnityEditor.Handles.DrawingScope(Gizmos.color, Matrix4x4.TRS(_pos, _rot, UnityEditor.Handles.matrix.lossyScale)))
        {
            Vector3 top = Vector3.forward * _height;

            UnityEditor.Handles.DrawWireDisc(Vector3.zero, Vector3.forward, _radius);
            UnityEditor.Handles.DrawWireDisc(top, Vector3.forward, _radius);

            UnityEditor.Handles.DrawLine(Vector3.right*_radius,top+Vector3.right*_radius);
            UnityEditor.Handles.DrawLine(-Vector3.right * _radius, top - Vector3.right * _radius);
            UnityEditor.Handles.DrawLine(Vector3.up * _radius, top + Vector3.up * _radius);
            UnityEditor.Handles.DrawLine(-Vector3.up * _radius, top - Vector3.up * _radius);
        }
    }
    public static void DrawTrapezium(Vector3 _pos, Quaternion _rot, Vector4 trapeziumInfo)
    {
        using (new UnityEditor.Handles.DrawingScope(Gizmos.color, Matrix4x4.TRS(_pos, _rot,UnityEditor.Handles.matrix.lossyScale)))
        {
                Vector3 backLeftUp = -Vector3.right * trapeziumInfo.x / 2 + Vector3.forward * trapeziumInfo.y / 2 - Vector3.up * trapeziumInfo.z / 2;
                Vector3 backLeftDown = -Vector3.right * trapeziumInfo.x / 2 - Vector3.forward * trapeziumInfo.y / 2 - Vector3.up * trapeziumInfo.z / 2;
                Vector3 backRightUp = Vector3.right * trapeziumInfo.x / 2 + Vector3.forward * trapeziumInfo.y / 2 - Vector3.up * trapeziumInfo.z / 2;
                Vector3 backRightDown = Vector3.right * trapeziumInfo.x / 2 - Vector3.forward * trapeziumInfo.y / 2 - Vector3.up * trapeziumInfo.z / 2;

                Vector3 forwardLeftUp = -Vector3.right * trapeziumInfo.w / 2 + Vector3.forward * trapeziumInfo.y / 2 + Vector3.up * trapeziumInfo.z / 2;
                Vector3 forwardLeftDown = -Vector3.right * trapeziumInfo.w / 2 - Vector3.forward * trapeziumInfo.y / 2 + Vector3.up * trapeziumInfo.z / 2;
                Vector3 forwardRightUp = Vector3.right * trapeziumInfo.w / 2 + Vector3.forward * trapeziumInfo.y / 2 + Vector3.up * trapeziumInfo.z / 2;
                Vector3 forwardRightDown = Vector3.right * trapeziumInfo.w / 2 - Vector3.forward * trapeziumInfo.y / 2 + Vector3.up * trapeziumInfo.z / 2;

                UnityEditor.Handles.DrawLine(backLeftUp, backLeftDown);
                UnityEditor.Handles.DrawLine(backLeftDown, backRightDown);
                UnityEditor.Handles.DrawLine(backRightDown, backRightUp);
                UnityEditor.Handles.DrawLine(backRightUp, backLeftUp);

                UnityEditor.Handles.DrawLine(forwardLeftUp, forwardLeftDown);
                UnityEditor.Handles.DrawLine(forwardLeftDown, forwardRightDown);
                UnityEditor.Handles.DrawLine(forwardRightDown, forwardRightUp);
                UnityEditor.Handles.DrawLine(forwardRightUp, forwardLeftUp);

                UnityEditor.Handles.DrawLine(backLeftUp, forwardLeftUp);
                UnityEditor.Handles.DrawLine(backLeftDown, forwardLeftDown);
                UnityEditor.Handles.DrawLine(backRightUp, forwardRightUp);
                UnityEditor.Handles.DrawLine(backRightDown, forwardRightDown);
        }
    }
}
#endregion
#endif