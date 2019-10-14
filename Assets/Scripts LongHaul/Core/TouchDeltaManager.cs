using System;
using UnityEngine;


public class TouchDeltaManager : SimpleSingletonMono<TouchDeltaManager>
{
    TouchTracker m_TrackLeft, m_TrackRight;
    Action<Vector2> OnLeftDelta,OnRightDelta;
    Func<bool> OnCanSendDelta;
    Vector2 m_leftDelta, m_rightDelta;
    public void DoBinding(Action<Vector2> _OnLeftDelta, Action<Vector2> _OnRightDelta,Func<bool> _OnCanSendDelta)
    {
        m_leftDelta = Vector2.zero;
        m_rightDelta = Vector2.zero;
        OnLeftDelta = _OnLeftDelta;
        OnRightDelta = _OnRightDelta;
        OnCanSendDelta = _OnCanSendDelta;
    }
    private void Update()
    {
        foreach (Touch t in Input.touches)
        {
            if (t.phase == TouchPhase.Began)
            {
                TouchTracker track = new TouchTracker(t);
                if (m_TrackLeft == null && track.isLeft && track.isDown)
                {
                    m_TrackLeft = track;
                    UIT_JoyStick.Instance.OnActivate(m_TrackLeft.v2_startPos);
                    m_leftDelta = Vector2.zero;
                }
                else if (m_TrackRight == null && !track.isLeft)
                {
                    m_TrackRight = track;
                    m_rightDelta = Vector2.zero;
                }
            }
            else if (t.phase == TouchPhase.Ended||t.phase== TouchPhase.Canceled)
            {
                if (m_TrackLeft != null && t.fingerId == m_TrackLeft.m_Touch.fingerId)
                {
                    m_TrackLeft = null;
                    UIT_JoyStick.Instance.OnDeactivate();
                    m_leftDelta = Vector2.zero;
                }
                if (m_TrackRight != null && t.fingerId == m_TrackRight.m_Touch.fingerId)
                {
                    m_TrackRight = null;
                    m_rightDelta = Vector2.zero;
                }
            }
            else if (t.phase == TouchPhase.Moved)
            {
                if (m_TrackRight!=null&&t.fingerId == m_TrackRight.m_Touch.fingerId)
                {
                    m_TrackRight.Record(t);
                    m_rightDelta = t.deltaPosition;
                }
                else if (m_TrackLeft != null && t.fingerId == m_TrackLeft.m_Touch.fingerId)
                {
                    m_TrackLeft.Record(t);
                    m_leftDelta = UIT_JoyStick.Instance.OnMoved(t.position);
                }
            }
        }
        if (OnCanSendDelta==null||!OnCanSendDelta())
            return;

        OnLeftDelta(m_leftDelta);
        OnRightDelta(m_rightDelta);
    }
    
    class TouchTracker
    {
        static float f_halfHorizontal = Screen.width / 2;
        static float f_halfVertical = Screen.height / 2;
        const float f_minOffset = 50;
        public Touch m_Touch { get; private set; }
        public Vector2 v2_startPos { get; private set; }
        public bool isLeft => v2_startPos.x < f_halfHorizontal;
        public bool isDown => v2_startPos.y < f_halfVertical;
        public bool trackSuccessful;
        public TouchTracker(Touch touchTrack)
        {
            m_Touch = touchTrack;
            v2_startPos = m_Touch.position;
            trackSuccessful = false;
        }
        public void Record(Touch touchTrack)
        {
            m_Touch = touchTrack;
        }
    }
}
