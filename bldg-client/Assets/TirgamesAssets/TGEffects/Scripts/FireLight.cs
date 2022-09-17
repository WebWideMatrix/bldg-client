using System;
using UnityEngine;

namespace TirgamesAssets.Effects
{
    public class FireLight : MonoBehaviour
    {
        public Vector3 PosNoise=new Vector3(0.5f,1f,0.5f);
        private float m_Rnd;
        private Light m_Light;
        private float m_Intensity;
        private Vector3 m_Position;


        private void Start()
        {
            m_Rnd = UnityEngine.Random.value*100;
            m_Light = GetComponent<Light>();
            m_Intensity = m_Light.intensity;
            m_Position = m_Light.transform.localPosition;
        }


        private void Update()
        {
                m_Light.intensity = m_Intensity * Mathf.PerlinNoise(m_Rnd + Time.time, m_Rnd + 1 + Time.time*1);
                float x = Mathf.PerlinNoise(m_Rnd + 0 + Time.time*2, m_Rnd + 1 + Time.time*2) - 0.5f;
                float y = Mathf.PerlinNoise(m_Rnd + 2 + Time.time*2, m_Rnd + 3 + Time.time*2) - 0.5f;
                float z = Mathf.PerlinNoise(m_Rnd + 4 + Time.time*2, m_Rnd + 5 + Time.time*2) - 0.5f;
                transform.localPosition = m_Position + Vector3.Scale(new Vector3(x, y, z), PosNoise);
        }

    }
}
