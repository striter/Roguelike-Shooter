using System;
using System.Collections.Generic;
using UnityEngine;

public enum enum_Option_JoyStickMode { Invalid=-1,Retarget=1,Stational=2,}

public class TouchDeltaManager : SimpleSingletonMono<TouchDeltaManager>
{
    TouchTracker m_TrackLeft, m_TrackRight;
    Action<Vector2> OnLeftDelta,OnRightDelta;
    Vector2 leftDelta = Vector2.zero;
    Vector2 rightDelta = Vector2.zero;
    UIT_JoyStick m_Joystick;
    public enum_Option_JoyStickMode m_Mode { get; private set; } = enum_Option_JoyStickMode.Invalid;
    public void SetMode(enum_Option_JoyStickMode mode) => m_Mode = mode;
    public void SetJoystick(UIT_JoyStick joyStick, Action<Vector2> _OnLeftDelta, Action<Vector2> _OnRightDelta)
    {
        m_Joystick = joyStick;
        OnLeftDelta = _OnLeftDelta;
        OnRightDelta = _OnRightDelta;
    }
    private void Update()
    {
        if (!m_Joystick)  return;

        rightDelta = Vector2.zero;
        foreach (Touch t in Input.touches)
        {
            if (t.phase == TouchPhase.Began)
            {
                TouchTracker track = new TouchTracker(t);
                if (m_TrackLeft == null && track.isLeft && track.isDown)
                {
                    m_TrackLeft = track;
                    m_Joystick.SetPos(m_TrackLeft.v2_startPos, Vector2.zero);
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
                    switch (m_Mode)
                    {
                        case enum_Option_JoyStickMode.Retarget:
                            Vector2 centerOffset = Vector2.Distance(t.position, m_TrackLeft.v2_startPos) > m_Joystick.m_JoyStickRaidus ? (t.position - m_TrackLeft.v2_startPos).normalized * m_Joystick.m_JoyStickRaidus : t.position - m_TrackLeft.v2_startPos;
                            leftDelta = centerOffset / m_Joystick.m_JoyStickRaidus;
                            m_Joystick.SetPos(m_TrackLeft.v2_startPos, centerOffset);
                            break;
                    }
                }
            }
        }

        m_Joystick.SetActivate(m_TrackLeft!=null);

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
