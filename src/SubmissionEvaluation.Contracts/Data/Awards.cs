using System;
using System.Collections;
using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data
{
    public class Awards : IEnumerable<KeyValuePair<string, HashSet<Award>>>
    {
        private readonly Dictionary<string, HashSet<Award>> list = new Dictionary<string, HashSet<Award>>();

        public HashSet<Award> this[string memberId]
        {
            get
            {
                if (list.TryGetValue(memberId, out var awards))
                {
                    return awards;
                }

                return new HashSet<Award>();
            }
        }

        public IEnumerator<KeyValuePair<string, HashSet<Award>>> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AwardWith(string memberId, params string[] awards)
        {
            if (!list.ContainsKey(memberId))
            {
                list[memberId] = new HashSet<Award>();
            }

            foreach (var award in awards)
            {
                list[memberId].Add(new Award {Id = award, Date = DateTime.Now});
            }
        }

        public void AwardWith(string memberId, params Award[] awards)
        {
            if (!list.ContainsKey(memberId))
            {
                list[memberId] = new HashSet<Award>();
            }

            foreach (var award in awards)
            {
                list[memberId].Add(award);
            }
        }

        public void RemoveAwardsFor(string memberId)
        {
            if (list.ContainsKey(memberId))
            {
                list[memberId].Clear();
            }

            list.Remove(memberId);
        }
    }
}
