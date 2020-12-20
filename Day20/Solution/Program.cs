using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Solution
{
    public class Program
    {
        static void Main(string[] args)
        {
            var input = Parser.ParseInput(File.ReadAllText("input.txt"));
            var part1 = Part1(input);
            var part2 = Part2(input);

            Console.WriteLine($"Part 1: {part1} Part 2: {part2}");
        }

        public static long Part1(Tile[] input)
        {
            var totalPermutations = input.Sum(tile => tile.Permutations.Count());
            var tree = GenerateSearchTree(input);
            var solutions = FindSolutions(tree).Where(p => p[0].IsSquare()).ToArray();

            var sol = solutions[0][0];
            return sol.GetScore();

            //var str = solutions.Select(s => s[0].ToDebugString()).ToArray();

            //foreach(var s in str)
            //{
            //    Console.WriteLine(s);
            //}

            //return 0;
        }

        public static List<TreeNode[]> FindSolutions(TreeNode root)
        {
            var path = new Stack<TreeNode>();
            var paths = new List<TreeNode[]>();

            FindSolutions(root, path, paths);
            return paths;
        }

        public static void FindSolutions(TreeNode node, Stack<TreeNode> path, List<TreeNode[]> paths)
        {
            path.Push(node);

            if (node.Children.Count == 0)
                paths.Add(path.ToArray());

            foreach (var child in node.Children)
            {
                FindSolutions(child, path, paths);
            }

            path.Pop();
        }

        public static TreeNode GenerateSearchTree(Tile[] input)
        {
            var remainingTiles = input.Skip(1).ToList();
            TreeNode root = new TreeNode();
            root.PlacedTiles.Add(new PlacedTile(0, 0, input[0].Id, input[0].Permutations[0]));

            GenerateSearchTree(root, remainingTiles, 1);
            return root;
        }

        public static void GenerateSearchTree(TreeNode node, List<Tile> remainingTiles, int depth)
        {
            if (depth == 144)
            {
                Console.WriteLine(node.ToDebugString());

                if (node.IsSquare())
                {
                    Console.WriteLine(node.GetScore());
                    Environment.Exit(0);
                }
            }

            foreach (var availableSide in node.GetAvailableSides())
            {
                foreach (var remainingTile in remainingTiles.ToArray())
                {
                    var sidePermutations = remainingTile.Permutations.Where(p => node.CanPlace(availableSide.freeLocation, p)).ToArray();
                    foreach (var permutation in sidePermutations)
                    {
                        var newNode = new TreeNode { 
                            PlacedTiles = node.PlacedTiles.ToList()
                        };

                        newNode.PlacedTiles.Add(new PlacedTile(availableSide.freeLocation.x, availableSide.freeLocation.y, remainingTile.Id, permutation));
                        remainingTiles.Remove(remainingTile);
                        GenerateSearchTree(newNode, remainingTiles, depth+1);
                        remainingTiles.Add(remainingTile);

                        node.Children.Add(newNode);
                    }
                }
            }
        }

        public static int GetOppositeSideIndex(int index)
        {
            var result = index + 2;

            if (result > 3)
                result -= 4;
            return result;
        }

        //public static (int x, int y) GetAdjacentCellCoordinates((int x, int y) location, int sideIndex)
        //{
        //    return sideIndex switch { 
        //        0 => (location.x, location.y -1),
        //        1 => (location.x +1, location.y),
        //        2 => (location.x, location.y+1),
        //        3 => (location.x - 1, location.y),
        //        _ => throw new Exception()
        //    };
        //}

        //public static (PlacedTile tile, int sideIndex)? FindPlacedTileWithSide(string side, List<PlacedTile> placedTiles)
        //{
        //    var tile = placedTiles.FirstOrDefault(tile => tile.Tile.GetSides().Contains(side));

        //    //var tile = sideMapping[side].Where(s => s != except).FirstOrDefault();

        //    if (tile == null)
        //        return null;


        //    var sideIndex = tile.Tile.GetSides().Select((value, index) => (value, index))
        //        .Where(s => s.value == side)
        //        .Single()
        //        .index;

        //    return (tile, sideIndex);
        //}





        public static int Part2(Tile[] input)
        {
            return 0;
        }
    }

    public class PlacedTile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int TileId { get; set; }
        public Grid Image { get; set; }

        public PlacedTile(int x, int y, int tileId, Grid image)
        {
            X = x;
            Y = y;
            TileId = tileId;
            Image = image;
        }

        public override int GetHashCode()
        {
            return X * Y * Image.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PlacedTile))
                return false;

            var tile = (PlacedTile)obj;
            return X == tile.X && Y == tile.Y && TileId == tile.TileId && Image == tile.Image;
        }
    }

    public class TreeNode
    {
        public List<TreeNode> Children { get; } = new List<TreeNode>();

        public List<PlacedTile> PlacedTiles { get; set; } = new List<PlacedTile>();

        public IEnumerable<(PlacedTile tile, int sideIndex, string side, (int x, int y) freeLocation)> GetAvailableSides()
        {
            foreach(var tile in PlacedTiles)
            {
                if (SideAvailable(tile, 0)) yield return (tile, 0, tile.Image.Sides[0], (tile.X, tile.Y-1));
                if (SideAvailable(tile, 1)) yield return (tile, 1, tile.Image.Sides[1], (tile.X+1, tile.Y));
                if (SideAvailable(tile, 2)) yield return (tile, 2, tile.Image.Sides[2], (tile.X, tile.Y+1));
                if (SideAvailable(tile, 3)) yield return (tile, 3, tile.Image.Sides[3], (tile.X-1, tile.Y));
            }
        }

        public bool SideAvailable(PlacedTile tile, int side)
        {
            var location = GetAdjacentCellCoordinates((tile.X, tile.Y), side);
            return GetTileAtLocation(location) == null;
        }

        public PlacedTile GetTileAtLocation((int x, int y) location)
        {
            return PlacedTiles.FirstOrDefault(t => t.X == location.x && t.Y == location.y);
        }

        public static (int x, int y) GetAdjacentCellCoordinates((int x, int y) location, int sideIndex)
        {
            return sideIndex switch
            {
                0 => (location.x, location.y - 1),
                1 => (location.x + 1, location.y),
                2 => (location.x, location.y + 1),
                3 => (location.x - 1, location.y),
                _ => throw new Exception()
            };
        }

        public bool CanPlace((int x, int y) location, Grid grid)
        {
            return
                Check(location.x, location.y - 1, 0) &&
                Check(location.x, location.y + 1, 2) &&
                Check(location.x - 1, location.y, 3) &&
                Check(location.x + 1, location.y, 1);

            bool Check(int x, int y, int sideIndex)
            {
                var tile = GetTileAtLocation((x, y));
                if (tile == null)
                    return true;

                return tile.Image.Sides[Program.GetOppositeSideIndex(sideIndex)] == grid.Sides[sideIndex];
            }
        }

        public bool IsSquare()
        {
            if (Width != Height)
                return false;

            var minX = PlacedTiles.Min(t => t.X);
            var maxX = PlacedTiles.Max(t => t.X);
            var minY = PlacedTiles.Min(t => t.Y);
            var maxY = PlacedTiles.Max(t => t.Y);

            var summaryString = new StringBuilder();
            for (var y = minY; y <= maxY; y++)
            {
                for (var x = minX; x <= maxX; x++)
                {
                    var grid = GetTileAtLocation((x, y));

                    if (grid == null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public int Width
        {
            get
            {
                return 1 + PlacedTiles.Select(t => t.X).Max() - PlacedTiles.Select(t => t.X).Min();
            }
        }

        public int Height
        {
            get
            {
                return 1 + PlacedTiles.Select(t => t.Y).Max() - PlacedTiles.Select(t => t.Y).Min();
            }
        }

        public long GetScore()
        {
            var minX = PlacedTiles.Min(t => t.X);
            var maxX = PlacedTiles.Max(t => t.X);
            var minY = PlacedTiles.Min(t => t.Y);
            var maxY = PlacedTiles.Max(t => t.Y);

            return (long)GetTileAtLocation((minX, minY)).TileId *
                GetTileAtLocation((maxX, minY)).TileId *
                GetTileAtLocation((maxX, maxY)).TileId *
                GetTileAtLocation((minX, maxY)).TileId;
        }

        public string ToDebugString()
        {
            var gridSize = PlacedTiles[0].Image.Image.Length;
            var str = new StringBuilder();
            var minX = PlacedTiles.Min(t => t.X);
            var maxX = PlacedTiles.Max(t => t.X);
            var minY = PlacedTiles.Min(t => t.Y);
            var maxY = PlacedTiles.Max(t => t.Y);

            var summaryString = new StringBuilder();
            for (var y = minY; y <= maxY; y++)
            {
                for (var x = minX; x <= maxX; x++)
                {
                    var grid = GetTileAtLocation((x, y));

                    if (grid != null)
                    {
                        summaryString.AppendLine($"{x}, {y}: {grid.TileId}");
                    }
                }
            }

            for (var y = minY; y <= maxY; y++)
            {
                for (var line = 0; line < gridSize; line++)
                {

                    for (var x = minX; x <= maxX; x++)
                    {
                        var grid = GetTileAtLocation((x, y));

                        if (grid == null)
                        {
                            str.Append(new string(' ', gridSize));
                        }
                        else
                        {
                            for(int c=0; c<gridSize; c++)
                            {
                                str.Append(grid.Image.Image[line][c] ? '#' : '.');
                            }
                        }

                        str.Append(' ');
                    }
                    str.AppendLine();
                }

                str.AppendLine();
            }


            return summaryString + Environment.NewLine + Environment.NewLine + str.ToString();
        }
    }
}
