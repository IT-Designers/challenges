using System;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Contracts.Providers
{
    public interface ILog
    {
        void Information(string msg, params object[] args);
        void Warning(string msg, params object[] args);
        void Warning(Exception exception, string msg, params object[] args);
        void Error(string msg, params object[] args);
        void Error(Exception exception, string msg, params object[] args);
        void Fatal(string msg, params object[] args);
        void Fatal(Exception exception, string msg, params object[] args);
        void Activity(string userId, ActivityType type, string reference = null);
        event Action<Activity> ActivityAdded;
    }
}
