using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NRustLightning.Server.Tests.Support
{
    public class TestCaseRunner
    {
        private readonly IEnumerable<Func<TestCase>> _tests;
        public TestCaseRunner(IEnumerable<Func<TestCase>> tests)
        {
            _tests = tests;
        }

        public int Run()
        {
            var results = new ConcurrentBag<string>();
            var success = true;
            var testsToRun = _tests.OrderBy(_ => Guid.NewGuid());

            Parallel.ForEach(testsToRun, new ParallelOptions {MaxDegreeOfParallelism = 4}, (testFac, state) =>
            {
                var test = testFac();
                try
                {
                    test.Execute().GetAwaiter().GetResult();
                    results.Add($"PASS {test.Name}\n{test.ServerOutput}");
                }
                catch (Exception e)
                {
                    success = false;
                    results.Add($"FAIL {test.Name}\n{e}\n{test.ServerOutput}");
                }
            });

            return success ? 0 : 1;
        }
    }
}