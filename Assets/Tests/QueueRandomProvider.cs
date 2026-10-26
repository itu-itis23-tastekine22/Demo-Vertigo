using System;
using System.Collections.Generic;
using VertigoWheel.Core;

namespace VertigoWheel.Tests
{
    internal sealed class QueueRandomProvider : IRandomProvider
    {
        private readonly Queue<int> _values;

        public QueueRandomProvider(params int[] values)
        {
            _values = new Queue<int>(values);
        }

        public int Range(int minInclusive, int maxExclusive)
        {
            if (_values.Count == 0)
            {
                throw new InvalidOperationException("The test requested more random values than were queued.");
            }

            return _values.Dequeue();
        }
    }
}