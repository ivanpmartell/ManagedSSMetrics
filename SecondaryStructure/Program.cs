using SecondaryStructure.Metrics;
using SecondaryStructure.Metrics.Interfaces;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace SecondaryStructure;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        var metricOption = new Option<string>(aliases: ["-m", "--metric"], description: "Name of the metric to calculate. Choices: Accuracy, SOV94, SOV99, SOVrefine, FractionalOverlap, LooseOverlap, StrictOverlap");
        var referenceOption = new Option<string>(aliases: ["-r", "--reference"], description: "Path to the reference sequence");
        var predictedOption = new Option<string>(aliases: ["-p", "--predicted"], description: "Path to the predicted sequence");
        var lambdaOption = new Option<double>(aliases: ["-l", "--lambda"], getDefaultValue: () => 1, description: "Adjustable scale parameter for SOVrefine");
        var deltaOption = new Option<bool>(aliases: ["-z", "--zeroDelta"], getDefaultValue: () => false, description: "Ignore the delta value (δ = 0)");

        metricOption.IsRequired = true;
        referenceOption.IsRequired = true;
        predictedOption.IsRequired = true;

        var cmd = new RootCommand("Secondary structure metric calculator");
        cmd.AddOption(metricOption);
        cmd.AddOption(referenceOption);
        cmd.AddOption(predictedOption);
        cmd.AddOption(lambdaOption);
        cmd.AddOption(deltaOption);
        cmd.TreatUnmatchedTokensAsErrors = true;
        cmd.Handler = CommandHandler.Create<string, string, string, double, bool>(
            (metric, reference, predicted, lambda, zeroDelta) =>
            {
                string sanitizedMetric = metric.Replace(".", "");
                string fullyQualifiedName = $"SecondaryStructure.Metrics.{sanitizedMetric}";
                Type? type = Type.GetType(fullyQualifiedName, false, true);
                if (type != null && typeof(IMetric).IsAssignableFrom(type))
                {
                    IMetric instance = (IMetric)Activator.CreateInstance(type, reference, predicted)!;
                    if (instance is SovRefine)
                        ((SovRefine)instance).Lambda = lambda;
                    if (instance is IAdjustableDelta)
                        ((IAdjustableDelta)instance).ZeroDelta = zeroDelta;
                    instance.Precomputation();
                    foreach (var secondaryStructure in instance.secondaryStructureClasses)
                    {
                        Console.WriteLine($"{sanitizedMetric.ToUpper()}_i\t{secondaryStructure}\t{instance.ComputeOneClass(secondaryStructure).ToString("#.000")}");
                    }
                    Console.WriteLine($"{sanitizedMetric.ToUpper()}\t{instance.ComputeAllClasses().ToString("#.000")}");
                }
                else
                {
                    Console.WriteLine("Metric choice is invalid");
                }
            });

        Console.OutputEncoding = Encoding.UTF8;
        var builder = new CommandLineBuilder(cmd);
        builder.UseHelp();
        builder.UseDefaults();
        builder.CancelOnProcessTermination();
        builder.UseExceptionHandler(HandleException);

        var parser = builder.Build();
        return await parser.InvokeAsync(args);
    }

    private static void HandleException(Exception exception, InvocationContext context)
    {
        if (exception is OperationCanceledException)
        {
            context.Console.Error.WriteLine("Operation canceled.");
        }
        else if (exception is TargetInvocationException tae && tae.InnerException != null)
        {
            context.Console.Error.WriteLine(tae.InnerException.Message);
        }
        else
        {
            context.Console.Error.WriteLine("Unhandled exception: ");
            context.Console.Error.WriteLine(exception.ToString());
        }
    }
}
