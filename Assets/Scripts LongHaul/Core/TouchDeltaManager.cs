using System;
using System.Collections.Generic;
using UnityEngine;

public class TouchDeltaManager : SimpleSingletonMono<TouchDeltaManager>
{
    TouchTracker m_TrackLeft, m_TrackRight;
    Action<Vector2> OnLeftDelta,OnRightDelta;
    float f_LeftStickRadius=100f;
    public void Bind(Action<Vector2> _OnLeftDelta,Action<Vector2> _OnRightDelta)
    {
        OnLeftDelta = _OnLeftDelta;
        OnRightDelta = _OnRightDelta;
        if (UIT_JoyStick.Instance != null)  f_LeftStickRadius = UIT_JoyStick.Instance.Init().x/2;
    }
    Vector2 leftDelta = Vector2.zero;
    Vector2 rightDelta = Vector2.zero;
    private void Update()
    {
        rightDelta = Vector2.zero;
        foreach (Touch t in Input.touches)
        {
            if (t.phase == TouchPhase.Began)
            {
                TouchTracker track = new TouchTracker(t);
                if (m_TrackLeft == null && track.isLeft && track.isDown)
                {
                    m_TrackLeft = track;
                    if (UIT_JoyStick.Instance != null) UIT_JoyStick.Instance.SetPos(m_TrackLeft.v2_startPos, Vector2.zero);
                }
                else if (m_TrackRight == null && !track.isLeft)
                {
                    m_TrackRight = track;
                }
            }
            else if (t.phase == TouchPhase.Ended||t.phase== TouchPhase.Canceled)
            {
                if (m_TrackLeft != null && t.fingerId == m_TrackLeft.m_Touch.fingerId)
                    m_TrackLeft = null;
                if (m_TrackRight != null && t.fingerId == m_TrackRight.m_Touch.fingerId)
                    m_TrackRight = null;
            }
            else if (t.phase == TouchPhase.Moved)
            {
                if (m_TrackRight!=null&&t.fingerId == m_TrackRight.m_Touch.fingerId)
                {
                    m_TrackRight.Record(t);
                    rightDelta = t.deltaPosition;
                }
                else if (m_TrackLeft != null && t.fingerId == m_TrackLeft.m_Touch.fingerId)
                {
                    m_TrackLeft.Record(t);
                    Vector2 centerOffset = Vector2.Distance(t.position, m_TrackLeft.v2_startPos) > f_LeftStickRadius ? (t.position - m_TrackLeft.v2_startPos).normalized * f_LeftStickRadius : t.position - m_TrackLeft.v2_startPos;
                    leftDelta = centerOffset / f_LeftStickRadius;
                    if (UIT_JoyStick.Instance != null)  UIT_JoyStick.Instance.SetPos(m_TrackLeft.v2_startPos,centerOffset);
                }
            }
        }

        if(UIT_JoyStick.Instance!=null) UIT_JoyStick.Instance.SetActivate(m_TrackLeft!=null);

        OnLeftDelta?.Invoke(m_TrackLeft!=null?leftDelta : Vector2.zero);
        OnRightDelta?.Invoke(rightDelta);
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
