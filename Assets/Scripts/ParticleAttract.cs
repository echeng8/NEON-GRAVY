using UnityEngine;
using System.Collections;
using UnityEditor;
public class ParticleAttract : MonoBehaviour
{

    [SerializeField]
    private Transform _attractorTransform;

    private ParticleSystem _particleSystem;
    private ParticleSystem.Particle[] _particles = new ParticleSystem.Particle[1000];
    private Vector3[] _particleStartingPosition;

    public void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _particleStartingPosition = new Vector3[_particleSystem.main.maxParticles];
    }

    public void LateUpdate()
    {
        if (GetComponent<ParticleSystem>().isPlaying)
        {
            int length = _particleSystem.GetParticles(_particles);
            Vector3 attractorPosition = _attractorTransform.position;

            for (int i = 0; i < length; i++)
            {

                if(_particles[i].remainingLifetime / _particles[i].startLifetime >= .99f)
                {
                    _particleStartingPosition[i] = _particles[i].position;
                
                }
                else
                {
                    _particles[i].position = Vector3.Lerp(attractorPosition, _particleStartingPosition[i], _particles[i].remainingLifetime / _particles[i].startLifetime);
                    Debug.DrawLine(_particleStartingPosition[i], attractorPosition, new Color(i, i, 0, 1));
                }
            }

            string startingPositionText = "";
            for (int i = 0; i < _particleSystem.main.maxParticles; i++)
            {
                startingPositionText += $"{i} {_particleStartingPosition[i]}  ";
            }
            print(startingPositionText);

            _particleSystem.SetParticles(_particles, length);
        }

    }
}