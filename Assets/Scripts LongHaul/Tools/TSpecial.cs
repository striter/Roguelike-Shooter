using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AI;
namespace TTiles
{
    public enum enum_TileDirection
    {
        Invalid = -1,
        Top=0,
        Right=1,
        Bottom=2,
        Left=3,
    }
    public interface ITileAxis
    {
        TileAxis m_TileAxis { get; }
    }
    [Serializable]
    public struct TileAxis
    {
        public int m_AxisX;
        public int m_AxisY;
        public TileAxis(int _axisX, int _axisY)
        {
            m_AxisX = _axisX;
            m_AxisY = _axisY;
        }

        public TileAxis[] nearbyFourTiles => new TileAxis[4] { new TileAxis(m_AxisX - 1, m_AxisY), new TileAxis(m_AxisX + 1, m_AxisY), new TileAxis(m_AxisX, m_AxisY + 1), new TileAxis(m_AxisX, m_AxisY - 1) };
        public static bool operator ==(TileAxis a, TileAxis b) => a.m_AxisX == b.m_AxisX && a.m_AxisY == b.m_AxisY;
        public static bool operator !=(TileAxis a, TileAxis b) => a.m_AxisX != b.m_AxisX || a.m_AxisY != b.m_AxisY;
        public static TileAxis operator -(TileAxis a, TileAxis b) => new TileAxis(a.m_AxisX - b.m_AxisX, a.m_AxisY - b.m_AxisY);
        public static TileAxis operator +(TileAxis a, TileAxis b) => new TileAxis(a.m_AxisX + b.m_AxisX, a.m_AxisY + b.m_AxisY);
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public static class TTiles
    {
        public static T Get<T>(this T[,] tileArray, TileAxis axis) where T : class, ITileAxis
        {
            return axis.InRange(tileArray) ? tileArray[axis.m_AxisX, axis.m_AxisY] : null;
        }

        public static bool InRange<T>(this TileAxis tileAxis, T[,] range) where T : class, ITileAxis
        {
            return tileAxis.m_AxisX >= 0 && tileAxis.m_AxisX < range.GetLength(0) && tileAxis.m_AxisY >= 0 && tileAxis.m_AxisY < range.GetLength(1);
        }

        public static float SqrMagnitude(this TileAxis sourceAxis, TileAxis targetAxis)
        {
            return Vector2.SqrMagnitude(new Vector2(sourceAxis.m_AxisX, sourceAxis.m_AxisY) - new Vector2(targetAxis.m_AxisX, targetAxis.m_AxisY));
        }

        public static int AxisOffset(this TileAxis sourceAxis, TileAxis targetAxis)
        {
            return Mathf.Abs(sourceAxis.m_AxisX - targetAxis.m_AxisX) + Mathf.Abs(sourceAxis.m_AxisY - targetAxis.m_AxisY);
        }

        public static enum_TileDirection DirectionInverse(this enum_TileDirection direction)
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
            if (offset.m_AxisX < 0 && offset.m_AxisY == 0)
                return enum_TileDirection.Left;
            if (offset.m_AxisX > 0 && offset.m_AxisY == 0)
                return enum_TileDirection.Right;
            if (offset.m_AxisX == 0 && offset.m_AxisY > 0)
                return enum_TileDirection.Top;
            if (offset.m_AxisX == 0 && offset.m_AxisY < 0)
                return enum_TileDirection.Bottom;

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
            Debug.LogError("Invlaid Direction Detected");
            return new TileAxis(0,0);
        }

        public static readonly List<enum_TileDirection> m_AllDirections = new List<enum_TileDirection>() { enum_TileDirection.Top, enum_TileDirection.Right,enum_TileDirection.Bottom, enum_TileDirection.Left};

        public static void PathFindForClosestApproch<T>(this T[,] tileArray, T t1, T t2, List<T> tilePathsAdd,Action<T> OnEachTilePath=null, Predicate<T> stopPredicate=null, Predicate<T> invalidPredicate=null) where T:class,ITileAxis
        {
            if (!t1.m_TileAxis.InRange(tileArray) || !t2.m_TileAxis.InRange(tileArray))
                Debug.LogError("Error Tile Not Included In Array");


            tilePathsAdd.Add(t1);
            TileAxis startTile=t1.m_TileAxis;
            for (; ; )
            {
                TileAxis nextTile=startTile;
                float minDistance = startTile.SqrMagnitude(t2.m_TileAxis);
                float offsetDistance;
                TileAxis offsetTile;
                TileAxis[] nearbyFourTiles = startTile.nearbyFourTiles;
                for (int i = 0; i < nearbyFourTiles.Length; i++)
                {
                    offsetTile = nearbyFourTiles[i];
                    offsetDistance = offsetTile.SqrMagnitude(t2.m_TileAxis);
                    if (offsetTile.InRange(tileArray) && offsetDistance < minDistance)
                    {
                        nextTile = offsetTile;
                        minDistance = offsetDistance;
                    }
                }

                if (nextTile == t2.m_TileAxis||(stopPredicate!=null&&stopPredicate(tileArray.Get(nextTile))))
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

            List<enum_TileDirection> edgesRandom = new List<enum_TileDirection>(m_AllDirections) { };
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
                m_AllDirections.TraversalRandom((enum_TileDirection randomDirection) => {
                    TileAxis axis = temp.m_TileAxis.DirectionAxis(randomDirection);
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
    public static class TTime
    {
        public static int GetTimeStamp(DateTime dt)
        {
            DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            int timeStamp = Convert.ToInt32((dt - dateStart).TotalSeconds);
            return timeStamp;
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
    public class RagdollAnimationTransition : ISingleCoroutine
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

    
    public class AnimatorClippingTime
    {
        protected Animator m_Animator;
        public AnimatorClippingTime(Animator _animator)
        {
            m_Animator = _animator;
        }

    }

    //Navigation AI System Chase/Follow/Attack/Idle ETC...
    public class NavigationAgentAI<T> : SimpleMonoLifetime, ISingleCoroutine where T : MonoBehaviour
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
            this.StopSingleCoroutines(1, 2,3);
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
}
#region Extra Structs/Classes
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
public class TXmlPhrase : SingleTon<TXmlPhrase>
{
    Dictionary<Type, Func<object, string>> dic_valueToXmlData = new Dictionary<Type, Func<object, string>>();
    Dictionary<Type, Func<string, object>> dic_xmlDataToValue = new Dictionary<Type, Func<string, object>>();
    Dictionary<Type, Func<object>> dic_xmlDataDefault = new Dictionary<Type, Func<object>>();
    public TXmlPhrase()
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
    public static TXmlPhrase Phrase
    {
        get
        {
            return Instance;
        }
    }
    public object GetDefault(Type type) => dic_xmlDataDefault.ContainsKey(type) ? dic_xmlDataDefault[type]() : type.IsValueType ? Activator.CreateInstance(type) : null;
    public string this[Type type, object value]
    {
        get
        {
            StringBuilder sb_xmlData = new StringBuilder();
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
    string ValueToXmlData(Type type, object value)
    {
        if (!dic_valueToXmlData.ContainsKey(type))
            Debug.LogWarning("Xml Error Invlid Type:" + type.ToString() + " For Base Type To Phrase");
        return dic_valueToXmlData[type](value);
    }
    object XmlDataToValue(Type type, string xmlData)
    {
        if (!dic_xmlDataToValue.ContainsKey(type))
            Debug.LogWarning("Xml Error Invlid Type:" + type.ToString() + " For Xml Data To Phrase");
        return dic_xmlDataToValue[type](xmlData);
    }
}
#endregion
#region Editor Class
#if UNITY_EDITOR
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
}
#endif
#endregion