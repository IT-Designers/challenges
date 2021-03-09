using System;
using System.Collections.Generic;
using System.Globalization;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.Storage;
using SubmissionEvaluation.Server.Classes.JekyllHandling;

namespace SubmissionEvaluation
{
    // TODO: See https://gist.github.com/odinserj/a8332a3f486773baa009
    public class DisableMultipleQueuedItemsFilter : JobFilterAttribute, IClientFilter, IServerFilter
    {
        private static readonly TimeSpan LockTimeout = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan FingerprintTimeout = TimeSpan.FromHours(1);

        public bool IncludeArguments { get; set; } = true;

        public void OnCreating(CreatingContext filterContext)
        {
            if (!AddFingerprintIfNotExists(filterContext.Connection, filterContext.Job))
            {
                filterContext.Canceled = true;
            }
        }

        void IClientFilter.OnCreated(CreatedContext filterContext)
        {
        }

        public void OnPerformed(PerformedContext filterContext)
        {
            RemoveFingerprint(filterContext.Connection, filterContext.BackgroundJob.Job);
        }

        void IServerFilter.OnPerforming(PerformingContext filterContext)
        {
        }

        private bool AddFingerprintIfNotExists(IStorageConnection connection, Job job)
        {
            using (connection.AcquireDistributedLock(GetFingerprintLockKey(job), LockTimeout))
            {
                var fingerprint = connection.GetAllEntriesFromHash(GetFingerprintKey(job));

                if (fingerprint != null && fingerprint.ContainsKey("Timestamp") &&
                    DateTimeOffset.TryParse(fingerprint["Timestamp"], null, DateTimeStyles.RoundtripKind, out var timestamp) &&
                    DateTimeOffset.UtcNow <= timestamp.Add(FingerprintTimeout))
                {
                    // Actual fingerprint found, returning.
                    JekyllHandler.Log.Warning("Job {name} bereits gequeued", job.Method.Name);
                    return false;
                }

                // Fingerprint does not exist, it is invalid (no `Timestamp` key),
                // or it is not actual (timeout expired).
                connection.SetRangeInHash(GetFingerprintKey(job), new Dictionary<string, string> {{"Timestamp", DateTimeOffset.UtcNow.ToString("o")}});

                return true;
            }
        }

        private void RemoveFingerprint(IStorageConnection connection, Job job)
        {
            using (connection.AcquireDistributedLock(GetFingerprintLockKey(job), LockTimeout))
            using (var transaction = connection.CreateWriteTransaction())
            {
                transaction.RemoveHash(GetFingerprintKey(job));
                transaction.Commit();
            }
        }

        private string GetFingerprintLockKey(Job job)
        {
            return string.Format("{0}:lock", GetFingerprintKey(job));
        }

        private string GetFingerprintKey(Job job)
        {
            return string.Format("fingerprint:{0}", GetFingerprint(job));
        }

        private string GetFingerprint(Job job)
        {
            if (job.Type == null || job.Method == null)
            {
                return string.Empty;
            }

            var parameters = string.Empty;
            if (IncludeArguments && job.Args != null)
            {
                parameters = "." + string.Join(".", job.Args);
            }

            return $"{job.Type.FullName}.{job.Method.Name}{parameters}";
        }
    }
}
