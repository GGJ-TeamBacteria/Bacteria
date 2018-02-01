using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CPUPerformanceSpoiler : MonoBehaviour
{
    public int iterations = 1000000;
    public int threadCount = 5;

    private VainCalculator[] vains;
    private bool isRunning;

    private class VainCalculator
    {
        public int iterations = 1000000;
        public bool isFinished = false;
        private Thread thread;

        public void Start()
        {
            thread = new Thread(new ThreadStart(Run));
            thread.Start();
        }

        public void Stop()
        {
            isFinished = true;
        }

        public void Run()
        {
            while (true)
            {
                float nothing = 1f;

                for (int i = 0; i < iterations; ++i)
                {
                    nothing = Mathf.Pow(nothing, 1.1f);
                }

                if (isFinished)
                {
                    break;
                }
            }
        }
    }

    void Update ()
    {
        if (Input.GetKey(KeyCode.S))
        {
            if (threadCount > 0)
            {
                //Debug.Log("Spinning threads! count: " + threadCount);

                if (!isRunning)
                {
                    isRunning = true;

                    vains = new VainCalculator[threadCount];

                    for (int i = 0; i < threadCount; ++i)
                    {
                        var calc = new VainCalculator();
                        calc.iterations = iterations;
                        calc.Start();

                        vains[i] = calc;
                    }
                }
            }
            else
            {
                Spoil();
            }
        }
        else
        {
            StopThreads();
        }
	}

    private void Spoil()
    {
        float nothing = 1f;

        for (int i = 0; i < iterations; ++i)
        {
            nothing = Mathf.Pow(nothing, 1.1f);
        }
    }

    private void StopThreads()
    {
        if (isRunning)
        {
            isRunning = false;

            for (int i = 0; i < threadCount; ++i)
            {
                vains[i].Stop();
            }
        }
    }

    void OnDestroy()
    {
        StopThreads();
    }
}
