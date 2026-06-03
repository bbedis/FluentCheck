using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;

using FluentCheck.Benchmarks;

Console.WriteLine("FluentCheck Benchmarks");
Console.WriteLine("==================");
Console.WriteLine();

// Manual config: disable optimizations validator
// Avoids macOS native AOT compiler path-bug (doubles artifact dir, crashes on sub-benchmarks)
var config = new ManualConfig();
config.Options |= ConfigOptions.DisableOptimizationsValidator;
config.AddValidator(JitOptimizationsValidator.DontFailOnError);
config.AddJob(Job.Default.AsDefault());
foreach (var logger in DefaultConfig.Instance.GetLoggers()) config.AddLogger(logger);
foreach (var exporter in DefaultConfig.Instance.GetExporters()) config.AddExporter(exporter);
foreach (var column in DefaultConfig.Instance.GetColumnProviders()) config.AddColumnProvider(column);

Console.WriteLine("Running Deep Equivalency Benchmarks...");
BenchmarkRunner.Run<DeepEquivalencyBenchmarks>(config);

Console.WriteLine();
Console.WriteLine("Running String Assertions Benchmarks...");
BenchmarkRunner.Run<StringAssertionsBenchmarks>(config);

Console.WriteLine();
Console.WriteLine("Running Collection Assertions Benchmarks...");
BenchmarkRunner.Run<CollectionAssertionsBenchmarks>(config);

Console.WriteLine();
Console.WriteLine("Running Numeric Assertions Benchmarks...");
BenchmarkRunner.Run<NumericAssertionsBenchmarks>(config);

Console.WriteLine();
Console.WriteLine("Running Pass-Path Allocation Benchmarks...");
BenchmarkRunner.Run<PassPathAllocationBenchmarks>(config);

Console.WriteLine();
Console.WriteLine("All benchmarks complete.");
