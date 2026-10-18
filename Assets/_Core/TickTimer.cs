using System;
using UnityEngine;

namespace _Core
{
    public class TickTimer : MonoBehaviour
    {
        public static event Action OnTick;
        public float tickInterval = 0.3f;
        private float _timer;

        public void Update()
        {
            _timer += Time.deltaTime;
            if (_timer > tickInterval)
            {
                _timer = 0f;
                OnTick?.Invoke();
            }
        }
    }
}