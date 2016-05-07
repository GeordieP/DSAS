using UnityEngine;
public static class EnemyWaves {
    private struct Wave {
        public Vector3[] _spawnPoints;
        public float _delayBetweenSpawns;

        public Wave(Vector3[] points, float delay) {
            _spawnPoints = points;
            _delayBetweenSpawns = delay;
        }
    }

    private static Wave[] _waveTypes = {
        // Spawn at top at one of four random X pos
        new Wave {
            _spawnPoints = new Vector3[] {
                new Vector3(GetRandXPos(), Balance.ScreenBounds.top, 0f),
                new Vector3(GetRandXPos(), Balance.ScreenBounds.top, 0f),
                new Vector3(GetRandXPos(), Balance.ScreenBounds.top, 0f),
                new Vector3(GetRandXPos(), Balance.ScreenBounds.top, 0f)
            },
            _delayBetweenSpawns = 0.8f
        },
        // Two spawn points at top
        // S wave movement pattern (implemented in Enemy.cs > MovePatterns)        
        new Wave {
            _spawnPoints = new Vector3[] {
                new Vector3(Balance.ScreenBounds.left + Balance.ScreenBounds.width * 0.2f, Balance.ScreenBounds.top, 0f),
                new Vector3(Balance.ScreenBounds.right - Balance.ScreenBounds.width * 0.2f, Balance.ScreenBounds.top, 0f)
            }, 
            _delayBetweenSpawns = 0.2f
        },
        // Two spawn points at top
        // for now just copy the same as above so it lines up with the enemy move pattern array nicely
        // probably combine this with above if we move the enemy move pattern implementations into or closer to Wave
        // this one is used for a circular move pattern
        new Wave {
            _spawnPoints = new Vector3[] {
                new Vector3(Balance.ScreenBounds.left + Balance.ScreenBounds.width * 0.2f, Balance.ScreenBounds.top, 0f),
                new Vector3(Balance.ScreenBounds.right - Balance.ScreenBounds.width * 0.2f, Balance.ScreenBounds.top, 0f)
            }, 
            _delayBetweenSpawns = 0.2f
        },
        // at 20, 40, and 60 percent down each side
        new Wave {
            _spawnPoints = new Vector3[] {
                new Vector3(Balance.ScreenBounds.left, Balance.ScreenBounds.top + Balance.ScreenBounds.height * 0.2f, 0f),
                new Vector3(Balance.ScreenBounds.left, Balance.ScreenBounds.top + Balance.ScreenBounds.height * 0.4f, 0f),
                new Vector3(Balance.ScreenBounds.left, Balance.ScreenBounds.top + Balance.ScreenBounds.height * 0.6f, 0f),
                new Vector3(Balance.ScreenBounds.right, Balance.ScreenBounds.top + Balance.ScreenBounds.height * 0.2f, 0f),
                new Vector3(Balance.ScreenBounds.right, Balance.ScreenBounds.top + Balance.ScreenBounds.height * 0.4f, 0f),
                new Vector3(Balance.ScreenBounds.right, Balance.ScreenBounds.top + Balance.ScreenBounds.height * 0.6f, 0f)
            }
        }
    };

    public static Wave[] WaveTypes {
        get { return _waveTypes; }
    }

    // Get a random X position bound by the screen's width
    private static float GetRandXPos() {
        return Random.Range(Balance.ScreenBounds.left, Balance.ScreenBounds.right);
    }
}
