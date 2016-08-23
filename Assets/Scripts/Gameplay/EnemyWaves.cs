using UnityEngine;
public static class EnemyWaves {
    public struct Wave {
        public Vector3[] _spawnPoints;
        public float _delayBetweenSpawns;
        public delegate int SpawnDelegate(int currentSpawnIndex, int waveSize, int numSpawnPoints);
        public SpawnDelegate GetSpawnpointIndex;

        public Wave(Vector3[] points, float delay) {
            _spawnPoints = points;
            _delayBetweenSpawns = delay;
            GetSpawnpointIndex = WaveSpawnPatterns.Linear;
        }

        public Vector3 GetRandSpawnPoint() {
            Vector3 temp = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
            return temp;
        }

        public Vector3 GetSpawnPoint(int currentSpawnIndex, int waveSize) {
            return _spawnPoints[GetSpawnpointIndex(currentSpawnIndex, waveSize, _spawnPoints.Length)];
        }
    }

    /*
        how it works:
        - gamemanager will choose a wave type index from the _waveTypes array, and remember that index
        - gamemanager will have an array of enemies, and loop through it
        -- call SetSpawnPosition on each enemy, passing it a position returned from GetSpawnPoint()
        --- GetSpawnPoint will return the next index in the chosen Wave's _spawnPoints array, using the iteration pattern returned from GetSpawnpointIndex
        ---- GetSpawnpointIndex will return the next index based on a pattern (loop through normally, pingpong, etc)

    */

    public static class WaveSpawnPatterns {
        // loop through the array forwards, at end wrap around to first spawn point
        public static int Linear(int currentSpawnIndex, int waveSize, int numSpawnPoints) {
            return currentSpawnIndex % numSpawnPoints;
        }

        // first half of wave goes through first half of available spawn points, second goes through second. Both looping around once reaching the end of their array
        public static int HalfAndHalf(int currentSpawnIndex, int waveSize, int numSpawnPoints) {
            return (currentSpawnIndex < (waveSize / 2)) ? currentSpawnIndex % (numSpawnPoints / 2) : currentSpawnIndex % (numSpawnPoints / 2) + numSpawnPoints / 2;
        }

        // first half of wave will get spawn point 0, rest will get 1
        public static int HalfAndHalf01(int currentSpawnIndex, int waveSize, int numSpawnPoints) {
            return (currentSpawnIndex < (waveSize / 2)) ? 0 : 1;
        }

        // first half of wave gets first spawn point, second half gets last point
        public static int HalfAndHalf0End(int currentSpawnIndex, int waveSize, int numSpawnPoints) {
            return (currentSpawnIndex < (waveSize / 2)) ? 0 : numSpawnPoints - 1;
        }

        // alternating first point, last point, first point, last point
        public static int PintPong0End(int currentSpawnIndex, int waveSize, int numSpawnPoints) {
            return (currentSpawnIndex % 2 == 0) ? 0 : numSpawnPoints - 1;
        }

        // return a pattern of 0,1,0,1,0,1...
        public static int PingPong01(int currentSpawnIndex, int waveSize, int numSpawnPoints) {
            return (currentSpawnIndex % 2 == 0) ? 0 : 1;
        }

        // a random spawn position
        public static int Random(int currentSpawnIndex, int waveSize, int numSpawnPoints) {
            return UnityEngine.Random.Range(0, numSpawnPoints);
        }
    }

    // Explicitly defined array of explicitly defined Wave structs
    private static Wave[] _waveTypes = {
        // Spawn at top at one of four random X pos
        new Wave {
            _spawnPoints = new Vector3[] {
                new Vector3(GetRandXPos(), Balance.DespawnBounds.top, 0f),
                new Vector3(GetRandXPos(), Balance.DespawnBounds.top, 0f),
                new Vector3(GetRandXPos(), Balance.DespawnBounds.top, 0f),
                new Vector3(GetRandXPos(), Balance.DespawnBounds.top, 0f)
            },
            _delayBetweenSpawns = 0.8f,
            GetSpawnpointIndex = WaveSpawnPatterns.Linear
            // GetSpawnpointIndex = WaveSpawnPatterns.HalfAndHalf ???
            // GetSpawnpointIndex = WaveSpawnPatterns.HalfAndHalf01

        },
        // Two spawn points at top
        // S wave movement pattern (implemented in Enemy.cs > MovePatterns)        
        new Wave {
            _spawnPoints = new Vector3[] {
                new Vector3(Balance.DespawnBounds.left + Balance.DespawnBounds.width * 0.2f, Balance.DespawnBounds.top, 0f),
                new Vector3(Balance.DespawnBounds.right - Balance.DespawnBounds.width * 0.2f, Balance.DespawnBounds.top, 0f)
            }, 
            _delayBetweenSpawns = 0.4f,
            // GetSpawnpointIndex = WaveSpawnPatterns.PingPong01
            GetSpawnpointIndex = WaveSpawnPatterns.HalfAndHalf01
        },
        // Two spawn points at top
        // for now just copy the same as above so it lines up with the enemy move pattern array nicely
        // probably combine this with above if we move the enemy move pattern implementations into or closer to Wave
        // this one is used for a circular move pattern
        new Wave {
            _spawnPoints = new Vector3[] {
                new Vector3(Balance.DespawnBounds.left + Balance.DespawnBounds.width * 0.2f, Balance.DespawnBounds.top, 0f),
                new Vector3(Balance.DespawnBounds.right - Balance.DespawnBounds.width * 0.2f, Balance.DespawnBounds.top, 0f)
            }, 
            _delayBetweenSpawns = 0.4f,
            GetSpawnpointIndex = WaveSpawnPatterns.PingPong01
            // GetSpawnpointIndex = WaveSpawnPatterns.HalfAndHalf01
        },
        // at 20, 40, and 60 percent down each side
        new Wave {
            _spawnPoints = new Vector3[] {
                new Vector3(Balance.DespawnBounds.left, Balance.DespawnBounds.top - Balance.DespawnBounds.height * 0.2f, 0f),
                new Vector3(Balance.DespawnBounds.left, Balance.DespawnBounds.top - Balance.DespawnBounds.height * 0.4f, 0f),
                new Vector3(Balance.DespawnBounds.left, Balance.DespawnBounds.top - Balance.DespawnBounds.height * 0.6f, 0f),
                new Vector3(Balance.DespawnBounds.right, Balance.DespawnBounds.top - Balance.DespawnBounds.height * 0.2f, 0f),
                new Vector3(Balance.DespawnBounds.right, Balance.DespawnBounds.top - Balance.DespawnBounds.height * 0.4f, 0f),
                new Vector3(Balance.DespawnBounds.right, Balance.DespawnBounds.top - Balance.DespawnBounds.height * 0.6f, 0f)
            },
            _delayBetweenSpawns = 0.3f,
            // GetSpawnpointIndex = WaveSpawnPatterns.Linear
            GetSpawnpointIndex = WaveSpawnPatterns.HalfAndHalf
            // GetSpawnpointIndex = WaveSpawnPatterns.HalfAndHalf01
            // GetSpawnpointIndex = WaveSpawnPatterns.PintPong0End
        }
    };

    public static Wave[] WaveTypes {
        get { return _waveTypes; }
    }

    // Get a random X position bound by the spawn width
    private static float GetRandXPos() {
        return Random.Range(Balance.DespawnBounds.left, Balance.DespawnBounds.right);
    }
}
