using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Solution
{
    public class Program
    {
        static void Main(string[] args)
        {
            var input = Parser.ParseInput(File.ReadAllText("input.txt"));
            var part1 = Solve(input, 2020);
            var part2 = Solve(input, 30000000);

            Console.WriteLine($"Part 1: {part1} Part 2: {part2}");
        }

        public static long Solve(long[] input, long target)
        {
            return GenerateNumbers(input).SkipWhile(pair => pair.Item2 < target)
                .First().Item1;
        }

        public static IEnumerable<(long, int)> GenerateNumbers(long[] input)
        {
            var numbers = input.Select((number, index) => (number, index)).ToDictionary(value => value.number, value => new NumberKnowledge(value.number, value.index));
            var lastNumber = input.Last();
            var numberCount = input.Length;

            while(true)
            {
                long number = numbers[lastNumber].SecondToLastIndex.HasValue ? numbers[lastNumber].Difference : 0;

                AddOrUpdateNumber(number);

                lastNumber = number;
                numberCount++;

                yield return (number, numberCount);
            }

            void AddOrUpdateNumber(long number)
            {
                if (numbers.ContainsKey(number))
                    numbers[number].Update(numberCount);
                else
                    numbers[number] = new NumberKnowledge(number, numberCount);
            }
        }
    }

    public class NumberKnowledge
    {
        public long Number { get; }
        public int? SecondToLastIndex { get; set; }
        public int LastIndex { get; set; }
        public int Difference => LastIndex - SecondToLastIndex.Value;

        public NumberKnowledge(long number, int lastIndex)
        {
            Number = number;
            LastIndex = lastIndex;
        }

        public void Update(int lastIndex)
        {
            SecondToLastIndex = LastIndex;
            LastIndex = lastIndex;
        }
    }
}
