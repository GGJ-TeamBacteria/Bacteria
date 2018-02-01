using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BlueprintReality.UI
{
    public class SetActiveFromSelectable : MonoBehaviour
    {
        public enum Trigger
        {
            Hover, Press
        }

        public Selectable selectable;
        public Trigger trigger = Trigger.Hover;

        public List<GameObject> on = new List<GameObject>();
        public List<GameObject> off = new List<GameObject>();

        private EventTrigger.Entry startEv, endEv;

        private void OnEnable()
        {
            if (selectable == null)
                selectable = GetComponentInParent<Selectable>();

            EventTrigger evTrigger = selectable.GetComponent<EventTrigger>();
            if (evTrigger == null)
                evTrigger = selectable.gameObject.AddComponent<EventTrigger>();

            if (trigger == Trigger.Hover)
            {
                startEv = new EventTrigger.Entry();
                startEv.eventID = EventTriggerType.PointerEnter;
                startEv.callback.AddListener(HandleStart);
                evTrigger.triggers.Add(startEv);

                endEv = new EventTrigger.Entry();
                endEv.eventID = EventTriggerType.PointerExit;
                endEv.callback.AddListener(HandleEnd);
                evTrigger.triggers.Add(endEv);
            }
            if( trigger == Trigger.Press )
            {
                startEv = new EventTrigger.Entry();
                startEv.eventID = EventTriggerType.PointerDown;
                startEv.callback.AddListener(HandleStart);
                evTrigger.triggers.Add(startEv);

                endEv = new EventTrigger.Entry();
                endEv.eventID = EventTriggerType.PointerUp;
                endEv.callback.AddListener(HandleEnd);
                evTrigger.triggers.Add(endEv);
            }

            on.ForEach(g => g.SetActive(false));
            off.ForEach(g => g.SetActive(true));
        }
        private void OnDisable()
        {
            EventTrigger evTrigger = selectable.GetComponent<EventTrigger>();

            evTrigger.triggers.Remove(startEv);
            evTrigger.triggers.Remove(endEv);

            on.ForEach(g => g.SetActive(false));
            off.ForEach(g => g.SetActive(true));
        }

        private void HandleStart(BaseEventData evData)
        {
            on.ForEach(g => g.SetActive(true));
            off.ForEach(g => g.SetActive(false));
        }
        private void HandleEnd(BaseEventData evData)
        {
            on.ForEach(g => g.SetActive(false));
            off.ForEach(g => g.SetActive(true));
        }
    }
}