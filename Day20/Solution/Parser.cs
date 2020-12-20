using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                var image = ParseGrid(lines.Skip(1));

                tiles.Add(new Tile(header, new Grid(image)));
            }

            return tiles.ToArray();
        }

        public static int ParseHeader(string line)
        {
            var match = Regex.Match(line, "Tile (\\d+)");
            return int.Parse(match.Groups[1].Value);
        }

        public static bool[][] ParseGrid(IEnumerable<string> lines)
        {
            return lines.Select(line => line.ToCharArray()
                        .Select(chr => chr switch
                        {
                            '#' => true,
                            _ => false
                        }).ToArray()
                    ).ToArray();
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
            Permutations = image.GeneratePermutations().ToList();
        }

        public IEnumerable<string> UniqueSides => Permutations.SelectMany(p => p.GetNormalizedSides()).Distinct();

        public bool IsCorner(Dictionary<string, int> sideCounts)
        {
            return UniqueSides.Count(side => sideCounts[side] == 1) == 2*2;
        }

        public Grid GetTopLeftCornerPermutation(Dictionary<string, int> sideCounts)
        {
            return Permutations.First(p => sideCounts[p.Sides[0]] == 1 && sideCounts[p.Sides[3]] == 1);
        }
    }

    public class Grid
    {
        public bool[,] Image { get; set; }
        public string[] Sides { get; }

        public Grid(bool[][] image)
        {
            Image = new bool[image.GetLength(0), image.GetLength(0)];
            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(0); y++)
                {
                    Image[y, x] = image[y][x];
                }
            }

            Sides = GetSides();
        }

        public Grid(bool[,] image)
        {
            Image = image;
            //Image = new bool[image.GetLength(0)][];
            //for (int i = 0; i < image.GetLength(0); i++)
            //    Image[i] = new bool[image.GetLength(0)];

            //for(int x=0; x< image.GetLength(0); x++)
            //{
            //    for (int y = 0; y < image.GetLength(0); y++)
            //    {
            //        Image[x][y] = image[x, y];
            //    }
            //}

            Sides = GetSides();
        }

        public IEnumerable<Grid> GeneratePermutations()
        {
            return RemoveDuplicates(GetPermutations());
        }

        public IEnumerable<Grid> GetPermutations()
        {
            var grid = this;

            for (var i = 0; i < 3; i++)
            {
                yield return grid;
                yield return grid.FlipHorizontal();
                yield return grid.FlipVertical();

                grid = grid.RotateRight();
            }
        }

        public IEnumerable<Grid> RemoveDuplicates(IEnumerable<Grid> grids)
        {
            var x = grids.Select(g => g.ToString()).ToArray();

            return grids.Select(g => (grid: g, str: g.ToString()))
                .GroupBy(g => g.str, g => g.grid)
                .Select(g => g.First());
        }

        public Grid RotateRight()
        {
            var result = new bool[Size, Size];

            for (int i = 0; i < Size; ++i)
            {
                for (int j = 0; j < Size; ++j)
                {
                    result[i,j] = Image[Size - j - 1,i];
                }
            }

            return new Grid(result);
        }

        public Grid FlipHorizontal()
        {
            var result = new bool[Size, Size];

            for (int y = 0; y < Size; ++y)
            {
                for (int x = 0; x < Size; ++x)
                {
                    result[y, Size - 1 - x] = Image[y, x];
                }
            }

            return new Grid(result);
        }

        public Grid FlipVertical()
        {
            var result = new bool[Size, Size];

            for (int y = 0; y < Size; ++y)
            {
                for (int x = 0; x < Size; ++x)
                {
                    result[Size - 1 - y, x] = Image[y, x];
                }
            }

            return new Grid(result);
        }

        public string GetSide(int index)
        {
            return ToString(index switch
            {
                0 => ReadRow(0),
                1 => ReadColumn(Width - 1),
                2 => ReadRow(Height - 1),
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

        public string GetNormalizedSide(int index)
        {
            return ToString(index switch
            {
                0 => ReadRow(0),
                1 => ReadColumn(Width - 1),
                2 => ReadRow(Height - 1).Reverse(),
                3 => ReadColumn(0).Reverse(),
                _ => throw new Exception()
            });
        }

        public string[] GetNormalizedSides()
        {
            return Enumerable.Range(0, 4)
                .Select(side => GetNormalizedSide(side))
                .ToArray();
        }

        public IEnumerable<bool> ReadRow(int row)
        {
            for (var x = 0; x < Width; x++)
            {
                yield return Image[row, x];
            }
        }

        public IEnumerable<bool> ReadColumn(int column)
        {
            for (var i = 0; i < Height; i++)
            {
                yield return Image[i,column];
            }
        }

        public string ToString(IEnumerable<bool> data)
        {
            return data.Select(value => value ? "#" : ".")
                .Aggregate((v1, v2) => v1 + v2);
        }

        public override string ToString()
        {
            var str = new StringBuilder();

            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    str.Append(Get(x, y) ? '#' : '.');
                }
                str.AppendLine();
            }

            return str.ToString();
        }

        public bool Contains(int x, int y)
        {
            return x >= 0 && x < Width &&
                y >= 0 && y < Height;
        }

        public bool Get(int x, int y)
        {
            return Image[y, x];
        }

        public IEnumerable<bool> AllValues
        {
            get 
            {
                for (var x = 0; x < Size; x++)
                {
                    for (var y = 0; y < Size; y++)
                    {
                        yield return Get(x, y);
                    }
                }
            }
        }

        public int Width => Image.GetLength(1);
        public int Height => Image.GetLength(0);
        public int Size => Width;
    }
}
