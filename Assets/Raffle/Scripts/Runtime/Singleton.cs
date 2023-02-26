using UnityEngine;

namespace Raffle
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T m_Instance;
        public static T Instance
        {
            get => m_Instance;
            protected set
            {
                if (m_Instance == null)
                {
                    m_Instance = value;
                }
                else if (m_Instance != value)
                {
                    Destroy(value);
                }
            }
        }

        protected virtual void Awake()
        {
            Instance = gameObject.GetComponent<T>();
        }
    }
}
