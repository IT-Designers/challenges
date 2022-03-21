using System;
using System.Collections.Generic;
using System.Globalization;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.Storage;

namespace SubmissionEvaluation.Server
{
    public class QueueUniqueItemFilter : JobFilterAttribute, IClientFilter, IServerFilter
    {
        private static readonly TimeSpan lockTimeout = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan fingerprintTimeout = TimeSpan.FromHours(1);

        public void OnCreating(CreatingContext filterContext)
        {
            using (filterContext.Connection.AcquireDistributedLock(GetFingerprintLock(filterContext.Job), lockTimeout))
            {
                if (IsFingerprintExisting(filterContext.Connection, filterContext.Job))
                {
                    //Job is a dublicate, cancel creation request!
                    filterContext.Canceled = true;
                }
                else
                {
                    AddFingerprint(filterContext.Connection, filterContext.Job);
                }
            }
        }

        void IClientFilter.OnCreated(CreatedContext filterContext)
        {
        }

        public void OnPerformed(PerformedContext filterContext)
        {
            using (filterContext.Connection.AcquireDistributedLock(GetFingerprintLock(filterContext.BackgroundJob.Job), lockTimeout))
            {
                RemoveFingerprint(filterContext.Connection, GetFingerprint(filterContext.BackgroundJob.Job));
            }
        }

        void IServerFilter.OnPerforming(PerformingContext filterContext)
        {
        }

        private bool IsFingerprintExisting(IStorageConnection connection, Job job)
        {
            var result = true;
            var fingerprint = connection.GetAllEntriesFromHash(GetFingerprint(job));
            if (fingerprint == null)
            {
                result = false;
            }
            if (fingerprint != null && fingerprint.ContainsKey("Timestamp") &&
                    DateTimeOffset.TryParse(fingerprint["Timestamp"], null, DateTimeStyles.RoundtripKind, out var timestamp) &&
                    DateTimeOffset.UtcNow <= timestamp.Add(fingerprintTimeout))
            {
                result = false;
            }
            return result;
        }

        private void AddFingerprint(IStorageConnection connection, Job job)
        {
            connection.SetRangeInHash(GetFingerprint(job), new Dictionary<string, string> { { "Timestamp", DateTimeOffset.UtcNow.ToString("o") } });
        }

        private void RemoveFingerprint(IStorageConnection connection, string fingerprint)
        {
            using (var transaction = connection.CreateWriteTransaction())
            {
                transaction.RemoveHash(fingerprint);
                transaction.Commit();
            }
        }

        private string GetFingerprintLock(Job job)
        {
            return $"{GetFingerprint(job)}:lock";
        }

        private string GetFingerprint(Job job)
        {
            var result = string.Empty;
            if (job.Method != null && job.Args != null)
            {
                result = $"{job.Method.Name}.{string.Join(".", job.Args)}";
            }
            return result;  
        }
    }
}
