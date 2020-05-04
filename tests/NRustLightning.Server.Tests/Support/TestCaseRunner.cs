using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.OwnedInstances;

namespace NRustLightning.Server.Tests.Support
{
    public class TestCaseRunner
    {
        private readonly IEnumerable<Func<Owned<TestCase>>> _tests;
        public TestCaseRunner(IEnumerable<Func<Owned<TestCase>>> tests)
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
                using var ownedTest = testFac();
                var test = ownedTest.Value;
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
            foreach (var r in results) Console.WriteLine(r);

            return success ? 0 : 1;
        }
    }
}