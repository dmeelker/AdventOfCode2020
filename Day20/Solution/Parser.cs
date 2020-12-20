using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Solution
{
    public static class Parser
    {
        public static Tile[] ParseInput(string input)
        {
            var tiles = new List<Tile>();
            var sections = input.Replace("\r", "").Split("\n\n").ToArray();

            foreach(var section in sections)
            {
                var lines = section.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToArray();
                var header = ParseHeader(lines[0]);
                var image = lines.Skip(1).Select(line => line.ToCharArray()
                        .Select(chr => chr switch
                        {
                            '#' => true,
                            _ => false
                        }).ToArray()
                    ).ToArray();

                tiles.Add(new Tile(header, new Grid(image)));
            }

            return tiles.ToArray();
        }

        public static int ParseHeader(string line)
        {
            var match = Regex.Match(line, "Tile (\\d+)");
            return int.Parse(match.Groups[1].Value);
        }
    }

    public class Tile
    {
        public int Id { get; set; }

        public Grid Base { get; set; }
        public List<Grid> Permutations { get; set; }

        public Tile(int id, Grid image)
        {
            Id = id;
            Base = image;
            Permutations = RemoveDuplicates(GeneratePermutations(image)).ToList();
        }

        private IEnumerable<Grid> RemoveDuplicates(IEnumerable<Grid> grids)
        {
            var x = grids.Select(g => g.ToString()).ToArray();

            return grids.Select(g => (grid: g, str: g.ToString()))
                .GroupBy(g => g.str, g => g.grid)
                .Select(g => g.First());
        }

        public IEnumerable<Grid> GeneratePermutations(Grid image)
        {
            //yield return image;
            var b = image;

            for (var i = 0; i < 3; i++)
            {
                yield return b;
                yield return b.FlipHorizontal();
                yield return b.FlipVertical();

                b = b.RotateRight();
            }
        }

        public HashSet<string> GetAllSidePermutations()
        {
            return new HashSet<string>(Permutations.SelectMany(p => p.Sides));
        }

        public bool ContainsSide(string side)
        {
            return Permutations.Any(permutation => permutation.Sides.Any(s => s == side));
        }

        public IEnumerable<Grid> FindConnectablePermutations(string side, int sideIndex)
        {
            return Permutations.Where(p => p.Sides[sideIndex] == side);
        }
    }

    public class Grid
    {
        public bool[][] Image { get; set; }
        public string[] Sides { get; }

        public Grid(bool[][] image)
        {
            Image = image;
            Sides = GetSides();
        }

        public Grid RotateRight()
        {
            var size = Image.Length;
            var result = new bool[size][];
            for (var y = 0; y < size; y++)
                result[y] = new bool[size];

            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    result[i][j] = Image[size - j - 1][i];
                }
            }

            return new Grid(result);
        }

        public Grid FlipHorizontal()
        {
            var size = Image.Length;
            var result = new bool[size][];
            for (var y = 0; y < size; y++)
                result[y] = new bool[size];

            for (int y = 0; y < size; ++y)
            {
                for (int x = 0; x < size; ++x)
                {
                    result[y][size - 1 - x] = Image[y][x];
                }
            }

            return new Grid(result);
        }

        public Grid FlipVertical()
        {
            var size = Image.Length;
            var result = new bool[size][];
            for (var y = 0; y < size; y++)
                result[y] = new bool[size];

            for (int y = 0; y < size; ++y)
            {
                for (int x = 0; x < size; ++x)
                {
                    result[size - 1 - y][x] = Image[y][x];
                }
            }

            return new Grid(result);
        }

        public void RotateUntilSideIsInPosition(int sideIndex, int desiredIndex)
        {
            while (sideIndex != desiredIndex)
            {
                RotateRight();
                sideIndex++;

                if (sideIndex > 3)
                    sideIndex -= 4;
            }
        }

        public string GetSide(int index)
        {
            return ToString(index switch
            {
                0 => ReadRow(0), //ReadRow(0),
                1 => ReadColumn(Image.Length - 1), //ReadColumn(Image.Length - 1),
                2 => ReadRow(Image.Length - 1),
                3 => ReadColumn(0),
                _ => throw new Exception()
            });
        }

        public string[] GetSides()
        {
            return Enumerable.Range(0, 4)
                .Select(side => GetSide(side))
                .ToArray();
        }

        public IEnumerable<bool> ReadRow(int row)
        {
            return Image[row];
        }

        public IEnumerable<bool> ReadColumn(int column)
        {
            for (var i = 0; i < Image.Length; i++)
            {
                yield return Image[i][column];
            }
        }

        public string ToString(IEnumerable<bool> data)
        {
            return data.Select(value => value ? "#" : ".")
                .Aggregate((v1, v2) => v1 + v2);
        }

        public override string ToString()
        {
            return Image.Select(line => ToString(line)).Aggregate((l1, l2) => l1 + Environment.NewLine + l2);
        }

        public bool Equals(Grid other)
        {
            return ToString() == other.ToString();
        }
    }

    public class TilePermutation
    {

    }
}
