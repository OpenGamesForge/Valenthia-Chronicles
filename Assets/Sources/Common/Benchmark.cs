/*---------------------------------------------------------------------------------------------
 *  Copyright (c) OpenGamesForges. All rights reserved.
 *  Licensed under the MIT License. See LICENSE-MIT file in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

// Credit: https://github.com/FlaShG/Benchmark.cs

using System;
using System.Diagnostics;

/// <summary>
/// Simple benchmark class to measure average time consumption of code.
/// </summary>
public static class Benchmark
{
    /// <summary>
    /// A benchmark's time result.
    /// Use ToString() to display it.
    /// </summary>
    public readonly struct Result
    {
        private static readonly string[] units = new string[] { "s", "ms", "µs", "ns" };

        private readonly long elapsedMilliseconds;
        private readonly long iterationCount;
        public float elapsedSeconds
        {
            get { return ((float)elapsedMilliseconds) / (iterationCount * 1000); }
        }

        public Result(long elapsedMilliseconds, long iterationCount)
        {
            this.elapsedMilliseconds = elapsedMilliseconds;
            this.iterationCount = iterationCount;
        }

        public float CompareTo(Result other)
        {
            var elapsed = elapsedSeconds;
            var otherElapsed = other.elapsedSeconds;
            return elapsed / otherElapsed;
        }

        public override string ToString()
        {
            var elapsedTime = elapsedSeconds;

            var order = 0;
            while (elapsedTime < 1 && order < units.Length - 1)
            {
                elapsedTime *= 1000;
                order++;
            }
            return elapsedTime + units[order];
        }
    }

    public const long OneMillion = 1000000;
    public const long TenMillion = 10000000;
    public const long HundredMillion = 100000000;

    /// <summary>
    /// Returns the time it took on average to perform action.
    /// </summary>
    /// <param name="action">The action to benchmark.</param>
    /// <param name="iterationCount">The amount of times to perform the action.</param>
    /// <returns>The time it took on average to perform action.</returns>
    public static Result Run(Action action, long iterationCount = TenMillion)
    {
        var stopwatch = new Stopwatch();

        stopwatch.Start();
        for (var l = 0L; l < iterationCount; l++)
        {
            action();
        }
        stopwatch.Stop();
        return new Result(stopwatch.ElapsedMilliseconds, iterationCount);
    }

    /// <summary>
    /// Returns the time it took on average to perform action. Allows an action to be performed each time without being measured.
    /// </summary>
    /// <param name="action">The action to benchmark.</param>
    /// <param name="actionBefore">The action to be performed before each cycle without being measured.</param>
    /// <param name="iterationCount">The amount of times to perform the action.</param>
    /// <returns>The time it took on average to perform action.</returns>
    public static Result Run(Action action, Action actionBefore, long iterationCount = TenMillion)
    {
        var stopwatch = new Stopwatch();

        for (var l = 0L; l < iterationCount; l++)
        {
            actionBefore();
            stopwatch.Start();
            action();
            stopwatch.Stop();
        }
        return new Result(stopwatch.ElapsedMilliseconds, iterationCount);
    }
}

/* --- Example ---
using UnityEngine;

public class BenchmarkExample : MonoBehaviour
{
    private void Start()
    {
        BenchmarkResult result;
        
        result = Benchmark.Run(() => Something.SomeStuff());
        Debug.Log("SomeStuff takes " + result);
        
        result = Benchmark.Run(() => Something.OtherStuff());
        Debug.Log("OtherStuff takes " + result);
    }
}
*/
