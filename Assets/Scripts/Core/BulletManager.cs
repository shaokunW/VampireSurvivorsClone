using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vampire
{
    public class BulletManager : MonoBehaviour
    {

        public static BulletManager Instance { get; private set; }

        void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void SpawnBullet(string bulletId, Vector2 startPos, Vector2 bulletDirection)
        {
            Debug.DrawRay(startPos, bulletDirection * 5, Color.cyan, 0.1f);
        }
    }
}
