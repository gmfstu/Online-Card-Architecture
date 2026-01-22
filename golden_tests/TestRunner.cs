using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GoldenTests
{
    /// <summary>
    /// Main test runner that executes all tests and reports results
    /// </summary>
    class TestRunner
    {
        static int Main(string[] args)
        {
            Console.WriteLine("====================================");
            Console.WriteLine("  Online Card Architecture Tests");
            Console.WriteLine("====================================\n");

            var allResults = new List<(string Category, string TestName, TestResult Result)>();
            var testClasses = new[]
            {
                (typeof(GameActionTests), "Game Action Tests"),
                (typeof(JSONReaderTests), "JSON Reader Tests"),
                (typeof(NetworkTests), "Network Tests"),
                (typeof(EdgeCaseTests), "Edge Case Tests")
            };

            foreach (var (testClass, categoryName) in testClasses)
            {
                Console.WriteLine($"\n--- {categoryName} ---\n");
                
                var testMethods = testClass.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(m => m.Name.StartsWith("Test") && m.ReturnType == typeof(TestResult))
                    .ToList();

                foreach (var method in testMethods)
                {
                    try
                    {
                        var result = (TestResult)method.Invoke(null, null);
                        allResults.Add((categoryName, method.Name, result));
                        
                        var status = result.Passed ? "PASS" : "FAIL";
                        var color = result.Passed ? ConsoleColor.Green : ConsoleColor.Red;
                        
                        Console.ForegroundColor = color;
                        Console.Write($"[{status}] ");
                        Console.ResetColor();
                        Console.WriteLine($"{method.Name}");
                        
                        if (!result.Passed || args.Contains("--verbose") || args.Contains("-v"))
                        {
                            Console.WriteLine($"       {result.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        var innerEx = ex.InnerException ?? ex;
                        allResults.Add((categoryName, method.Name, new TestResult(false, $"Exception: {innerEx.Message}")));
                        
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("[FAIL] ");
                        Console.ResetColor();
                        Console.WriteLine($"{method.Name}");
                        Console.WriteLine($"       Exception: {innerEx.Message}");
                    }
                }
            }

            // Summary
            Console.WriteLine("\n====================================");
            Console.WriteLine("           TEST SUMMARY");
            Console.WriteLine("====================================\n");

            var passed = allResults.Count(r => r.Result.Passed);
            var failed = allResults.Count(r => !r.Result.Passed);
            var total = allResults.Count;

            Console.WriteLine($"Total:  {total}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Passed: {passed}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Failed: {failed}");
            Console.ResetColor();

            if (failed > 0)
            {
                Console.WriteLine("\nFailed Tests:");
                foreach (var (category, testName, result) in allResults.Where(r => !r.Result.Passed))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  - [{category}] {testName}");
                    Console.ResetColor();
                    Console.WriteLine($"    {result.Message}");
                }
            }

            Console.WriteLine($"\n====================================");
            if (failed == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  ALL TESTS PASSED!");
                Console.ResetColor();
                return 0;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  {failed} TEST(S) FAILED");
                Console.ResetColor();
                return 1;
            }
        }
    }
}
