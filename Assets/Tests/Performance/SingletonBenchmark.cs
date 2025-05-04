/*---------------------------------------------------------------------------------------------
 *  Copyright (c) OpenGamesForges. All rights reserved.
 *  Licensed under the MIT License. See LICENSE-MIT file in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

// Note: to launch the test, you need to attach the script to a GameObject in the scene and run the scene.
namespace Common.Utils
{
    public class SingletonBenchmark : MonoBehaviour
    {
        private class TestSingleton : Singleton<TestSingleton> { }
        private class TestLockSingleton : LockSingleton<TestLockSingleton> { }
        private class TestLazySingleton : LazySingleton<TestLazySingleton> { }

        private const int WARMUP_ITERATIONS = 1000;
        private const int TEST_ITERATIONS = 1000000;
        private const string LOG_FILE_FORMAT = "SingletonBenchmark_{0:MM-dd_HH-mm}.log";

        private StringBuilder logBuilder = new StringBuilder();
        private string timestamp;

        private void Start()
        {
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            logBuilder.AppendLine($"=== Singleton Performance Tests Start - {timestamp} ===");
            Log("=== Singleton Performance Tests Start ===");
            
            // Initialization test
            TestSingletonInitialization();
            
            // Access test
            TestSingletonAccess();
            
            Log("=== Singleton Performance Tests End ===");
            logBuilder.AppendLine("=== Singleton Performance Tests End ===");
            
            // Save results
            SaveResults();
        }

        private void Log(string message)
        {
            UnityEngine.Debug.Log(message);
            logBuilder.AppendLine(message);
        }

        private void SaveResults()
        {
            try
            {
                string fileName = string.Format(LOG_FILE_FORMAT, DateTime.Now);
                string path = Path.Combine(Application.persistentDataPath, fileName);
                File.WriteAllText(path, logBuilder.ToString());
                UnityEngine.Debug.Log($"Results saved to: {path}");
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Error while saving results: {e.Message}");
            }
        }

        private void TestSingletonInitialization()
        {
            Log("\nSingleton Initialization Tests:");
            
            // Standard Singleton test
            var result = Benchmark.Run(() => {
                var instance = TestSingleton.Instance;
            }, WARMUP_ITERATIONS);
            Log($"Standard Singleton: {result} per initialization");

            // LockSingleton test
            result = Benchmark.Run(() => {
                var instance = TestLockSingleton.Instance;
            }, WARMUP_ITERATIONS);
            Log($"Lock Singleton: {result} per initialization");

            // LazySingleton test
            result = Benchmark.Run(() => {
                var instance = TestLazySingleton.Instance;
            }, WARMUP_ITERATIONS);
            Log($"Lazy Singleton: {result} per initialization");
        }

        private void TestSingletonAccess()
        {
            Log("\nSingleton Access Tests (after initialization):");
            
            // Initialize instances
            var standardInstance = TestSingleton.Instance;
            var lockInstance = TestLockSingleton.Instance;
            var lazyInstance = TestLazySingleton.Instance;

            // Standard Singleton test
            var result = Benchmark.Run(() => {
                var instance = TestSingleton.Instance;
            }, TEST_ITERATIONS);
            Log($"Standard Singleton: {result} per access");

            // LockSingleton test
            result = Benchmark.Run(() => {
                var instance = TestLockSingleton.Instance;
            }, TEST_ITERATIONS);
            Log($"Lock Singleton: {result} per access");

            // LazySingleton test
            result = Benchmark.Run(() => {
                var instance = TestLazySingleton.Instance;
            }, TEST_ITERATIONS);
            Log($"Lazy Singleton: {result} per access");
        }
    }
} 