using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Solution
{
    public static class Directions
    {
        public static readonly Point NorthWest = new Point(0, -1);
        public static readonly Point NorthEast = new Point(1, -1);

        public static readonly Point West = new Point(-1, 0);
        public static readonly Point East = new Point(1, 0);

        public static readonly Point SouthWest = new Point(-1, 1);
        public static readonly Point SouthEast = new Point(0, 1);

        public static readonly Point[] AllDirections = new[] { NorthWest, NorthEast, West, East, SouthWest, SouthEast };
    }

    public class Program
    {
        static void Main(string[] args)
        {
            var input = Parser.ParseInput(File.ReadAllText("input.txt"));
            var part1 = Part1(input);
            var part2 = Part2(part1);

            Console.WriteLine($"Part 1: {part1.Count()} Part 2: {part2.Count}");
        }

        public static HashSet<Point> Part1(Point[][] input)
        {
            var tiles = new HashSet<Point>();

            foreach (var path in input)
            {
                var location = new Point(0, 0);

                foreach (var step in path)
                    location = location.Add(step);

                if(tiles.Contains(location))
                    tiles.Remove(location);
                else
                    tiles.Add(location);
            }

            return tiles;
        }

        public static HashSet<Point> Part2(HashSet<Point> initialState)
        {
            var state = initialState;

            for (var i=0;i<100;i++)
                state = Simulate(state);

            return state;
        }

        public static HashSet<Point> Simulate(HashSet<Point> state)
        {
            var newState = state.ToHashSet();

            state.Select(tile => (point: tile, count: CountAdjacentBlackTiles(tile, state)))
                .Where(tile => tile.count == 0 | tile.count > 2)
                .ToList()
                .ForEach(tile => newState.Remove(tile.point));

            GetWhiteTiles(state)
                .Where(tile => CountAdjacentBlackTiles(tile, state) == 2)
                .ToList()
                .ForEach(tile => newState.Add(tile));

            return newState;
        }

        public static int CountAdjacentBlackTiles(Point point, HashSet<Point> floor)
        {
            return GetAdjacentCells(point).Where(p => floor.Contains(p)).Count();
        }

        public static IEnumerable<Point> GetWhiteTiles(HashSet<Point> floor)
        {
            return floor.SelectMany(GetAdjacentCells).Distinct().Where(p => !floor.Contains(p));
        }

        public static IEnumerable<Point> GetAdjacentCells(Point p)
        {
            return Directions.AllDirections.Select(d => p.Add(d));
        }
    }

    public class Point : IEquatable<Point>
    {
        public int X { get; }
        public int Y { get; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point Add(Point p)
        {
            return new Point(X + p.X, Y + p.Y);
        }

        public override bool Equals(object obj)
        {
            if(obj is Point)
            {
                var p = (Point)obj;
                return X == p.X && Y == p.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (Y << 16) ^ X;
        }

        public bool Equals([AllowNull] Point other)
        {
            return other is Point && Equals((Object)other);
        }

        public override string ToString()
        {
            return $"{X}, {Y}";
        }
    }
}
