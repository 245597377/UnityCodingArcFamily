using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STFEngine.Module
{
    public class STF_TouchSystem : MonoBehaviour
    {
        // Start is called before the first frame update
        public enum TouchState
        {
            Begin = 0,
            Touching = 1,
            End = 2
        }
        private delegate void DZTouchEvent(TouchState pState);
        private DZTouchEvent _touchEvent;
        private void BindTouchEvent()
        {

        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
