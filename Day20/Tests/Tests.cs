using Solution;
using System;
using Xunit;

namespace Tests
{
    public class Tests
    {
        [Fact]
        public void ParseInput()
        {
            var result = Parser.ParseInput("");
        }

        [Fact]
        public void Tile_Rotate()
        {
            var tile = new Grid
            (
                new[] {
                    new[] { true, true } ,
                    new[] { false, false }
                }
            );

            tile = tile.RotateRight();
            Assert.Equal(new[] {
                    new[] { false, true } ,
                    new[] { false, true }
                }, tile.Image);

            tile = tile.RotateRight();
            Assert.Equal(new[] {
                    new[] { false, false} ,
                    new[] { true, true }
                }, tile.Image);

            tile = tile.RotateRight();
            Assert.Equal(new[] {
                    new[] { true, false} ,
                    new[] { true, false }
                }, tile.Image);

            tile = tile.RotateRight();
            Assert.Equal(new[] {
                    new[] { true, true} ,
                    new[] { false, false }
                }, tile.Image);
        }

        //[Fact]
        //public void Tile_RotateUntilSideIsInPosition()
        //{
        //    var tile = new Grid
        //    (
        //        new[] {
        //            new[] { true, true } ,
        //            new[] { false, false }
        //        }
        //    );

        //    tile.RotateUntilSideIsInPosition(0, 1);
        //    Assert.Equal(new[] {
        //            new[] { false, true } ,
        //            new[] { false, true }
        //        }, tile.Image);

        //    tile.RotateUntilSideIsInPosition(1, 0);
        //    Assert.Equal(new[] {
        //            new[] { true, true} ,
        //            new[] { false, false}
        //        }, tile.Image);
        //}

        [Fact]
        public void Tile_FlipHorizontal()
        {
            var tile = new Grid( new[] {
                    new[] { true, false } ,
                    new[] { true, false }
                }
            );

            tile = tile.FlipHorizontal();
            Assert.Equal(new[] {
                    new[] { false, true } ,
                    new[] { false, true }
                }, tile.Image);
        }

        [Fact]
        public void Tile_FlipVertical()
        {
            var tile = new Grid
            (
                new[] {
                    new[] { true, true } ,
                    new[] { false, false  }
                }
            );

            tile = tile.FlipVertical();
            Assert.Equal(new[] {
                    new[] { false, false } ,
                    new[] { true, true }
                }, tile.Image);
        }
    }
}
