using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

namespace PathFinding
{
    class CollisionMap
    {
        int Width { get; set; }
        int Height { get; set; }

        public Coordinate CurrentPosition { get; set; }
        public Coordinate GoalPosition { get; set; }
        CollisionMapElement[,] BlockData { get; set; }


        public CollisionMap(int width, int height, Coordinate startPosition, Coordinate goalPosition)
        {
            this.Width = width;
            this.Height = height;
            this.CurrentPosition = startPosition;
            this.GoalPosition = goalPosition;
            this.BlockData = new CollisionMapElement[width, height];

            clearBlocks();
            assignNeighbours();
            setDistancesFromGoal();
            //printMap();
        }

        /*
         JSON to map object
         */

        private void clearBlocks()
        {
            if (this.BlockData == null)
            {
                this.BlockData = new CollisionMapElement[Height, Width];
            }

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (this.BlockData[i, j] == null)
                    {
                        this.BlockData[i, j] = new CollisionMapElement(i, j);
                    }

                    this.BlockData[i, j].Blocked = false;
                }
            }
        }

        public void setBlock(Coordinate target, bool blocked)
        {
            this.BlockData[target.Row, target.Column].Blocked = blocked;
        }

        public void setBlocks(List<Coordinate> blocks, bool blocked)
        {
            foreach (Coordinate coord in blocks)
            {
                setBlock(coord, blocked);
            }
        }

        public CollisionMapElement getElement(Coordinate target)
        {
            try
            {
                return this.BlockData[target.Row, target.Column];
            }
            catch (IndexOutOfRangeException e)
            {
                return null;
            }

        }

        private void setDistancesFromGoal()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    this.BlockData[i, j].DistanceFromGoal = GoalPosition.distanceFromCoord(i, j);
                }
            }
        }

        public void printMap()
        {
            string print = "";
            for (int i = Width-1; i >=0 ; i--)
            {
                for (int j = 0; j < Height; j++)
                {
                    string symbol = "";


                    if (CurrentPosition.Column == j && CurrentPosition.Row == i)
                    {
                        symbol = "S";
                    }
                    else if (GoalPosition.Column == j && GoalPosition.Row == i)
                    {
                        symbol = "G";
                    }
                    else
                    {
                        symbol = (this.BlockData[i, j].Blocked == true ? "B" : " ");
                    }

                    print += "[" + symbol + "]";
                }
                print += "\n";
            }
            //////Debug.Log(print);
        }

        private void assignNeighbours()
        {
            int row;
            int column;
            int i, j;

            // NORTH
            /*
             [ ][ ][ ][ ]
             [*][*][*][*]
             [*][*][*][*]
             [*][*][*][*]
             */
            row = 1;
            column = 0;
            for (i = column; i < this.Width; i++)
            {
                for (j = row; j < this.Height; j++)
                {
                    this.BlockData[i, j].addNeighbour(this.BlockData[i, j - 1]);
                }
            }

            // SOUTH
            /*
             [*][*][*][*]
             [*][*][*][*]
             [*][*][*][*]
             [ ][ ][ ][ ]
             */
            row = 0;
            column = 0;
            for (i = column; i < this.Width; i++)
            {
                for (j = row; j < this.Height - 1; j++)
                {
                    this.BlockData[i, j].addNeighbour(this.BlockData[i, j + 1]);
                }
            }


            // EAST
            /*
             [*][*][*][ ]
             [*][*][*][ ]
             [*][*][*][ ]
             [*][*][*][ ]
             */
            row = 0;
            column = 0;
            for (i = column; i < this.Width - 1; i++)
            {
                for (j = row; j < this.Height; j++)
                {
                    this.BlockData[i, j].addNeighbour(this.BlockData[i + 1, j]);
                }
            }

            // WEST
            /*
             [ ][*][*][*]
             [ ][*][*][*]
             [ ][*][*][*]
             [ ][*][*][*]
             */
            row = 0;
            column = 1;
            for (i = column; i < this.Width; i++)
            {
                for (j = row; j < this.Height; j++)
                {
                    this.BlockData[i, j].addNeighbour(this.BlockData[i - 1, j]);
                }
            }
        }

        public override bool Equals(System.Object map)
        {
            // There is no map
            if (map == null)
            {
                return false;
            }

            // Wrong type
            if (!map.GetType().IsAssignableFrom(this.GetType())){
                return false;
            }

            CollisionMap targetMap = (CollisionMap) map;

            // Player is at a different coordinate
            if (!targetMap.CurrentPosition.Equals(this.CurrentPosition))
            {
                return false;
            }

            //  Are the blocks the same?
            for (int i = 0; i >= this.Width - 1; i++)
            {
                for (int j = 0; j >= this.Height; j++)
                {
                    if (targetMap.getElement(new Coordinate(i, j)).Blocked != this.getElement(new Coordinate(i, j)).Blocked)
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }

    }
    
    
}
