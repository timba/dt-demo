using System;
using System.Collections.Generic;
using System.Linq;

namespace DTDemo.DealProcessing.Csv
{
    public static class LinkedListTrimExtension
    {
        public static IEnumerable<char> Trim(this LinkedList<char> target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (!target.Any())
                return target;
                
            return target
                .Trim(target.First, current => current.Next)
                .Trim(target.Last, current => current.Previous);
        }

        private static LinkedList<char> Trim(
            this LinkedList<char> source, 
            LinkedListNode<char> start, 
            Func<LinkedListNode<char>, LinkedListNode<char>> nextF)
        {
            var current = start;
            while (true)
            {
                if (current == null)
                {
                    break;
                }

                var next = nextF(current);

                if (char.IsWhiteSpace(current.Value))
                {
                    source.Remove(current);
                }
                else
                {
                    break;
                }

                current = next;
            }

            return source;
        }
    }
}