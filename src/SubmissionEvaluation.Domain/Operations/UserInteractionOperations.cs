using System;

namespace SubmissionEvaluation.Domain.Operations
{
    internal static class UserInteractionOperations
    {
        internal static void AskUserToContinue(string message, Action onContinue, Action onAbort)
        {
            Console.WriteLine(message);
            if (Console.ReadKey().Key != ConsoleKey.J)
            {
                onAbort();
            }
            else
            {
                onContinue();
            }
        }
    }
}
