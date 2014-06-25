using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniBlocker
{
    enum SeedType { Typical, Blocky, Shape, Holey, LemonDiff }
    enum CutDir { Vertical, Horizontal, LeftDiagonal, RightDiagonal }

    class MainGenerator
    {
        #region Variables
        private int[,] blockPos;
        private int[,] inputArray1;
        private int[,] inputArray2;
        private BlockGenerator blockGen;
        private ColorGenerator colorGen;

        #endregion

        #region Methods

        //***Need to convert int array to mabey a struct to hold color/health/type info***
        public MainGenerator()
        {
            blockPos = new int[15, 12];
            inputArray1 = new int[15, 12];
            inputArray2 = new int[15, 12];
            blockGen = new BlockGenerator();
            colorGen = new ColorGenerator();
        }

        public int[,] ResetBlocks()
        {
            int[,] tempArray = new int[15, 12];
            int choice1, choice2;

            choice1 = Game1.RNG.Next(0, 1);
            choice2 = Game1.RNG.Next(0, 2);

            switch (choice1)
            {
                /*ase 0:
                    inputArray1 = blockGen.Perlin();
                    break;*/
                case 0:
                    inputArray1 = blockGen.Shapes();
                    break;
                case 1:
                    inputArray1 = blockGen.Directional();
                    break;
            }
            switch (choice2)
            {
                case 0:
                    inputArray2 = blockGen.Perlin();
                    break;
                case 1:
                    inputArray2 = blockGen.Shapes();
                    break;
                case 2:
                    inputArray2 = blockGen.Directional();
                    break;
            }

            choice1 = Game1.RNG.Next(0, 2);
            choice2 = Game1.RNG.Next(0, 2);

            switch (choice1)
            {
                case 0:
                    inputArray1 = blockGen.CreateBorders(inputArray1);
                    break;
                case 1:
                    inputArray1 = blockGen.ReflectOnce(inputArray1);
                    break;
                case 2:
                    inputArray1 = blockGen.ReflectTwice(inputArray1);
                    break;
            }
            switch (choice2)
            {
                case 0:
                    inputArray2 = blockGen.CreateBorders(inputArray2);
                    break;
                case 1:
                    inputArray2 = blockGen.ReflectOnce(inputArray2);
                    break;
                case 2:
                    inputArray2 = blockGen.ReflectTwice(inputArray2);
                    break;
            }

            choice1 = Game1.RNG.Next(0, 2);

            switch(choice1)
            {
                case 0:
                    tempArray = AddArrays(inputArray1, inputArray2);
                    break;
                case 1:
                    inputArray2 = SubtractArrays(inputArray1, inputArray2);
                    break;
                case 2:
                    tempArray = inputArray1;
                    break;
            }

            return tempArray;
        }

        public int[,] ResetColors()
        {
            int[,] tempArray = new int[15, 12];
            int[,] tempArray2 = new int[15, 12];
            int colorMethod;

            colorMethod = Game1.RNG.Next(0, 5);
            switch (colorMethod)
            {
                case 0: case 1:
                    tempArray = colorGen.Walk();
                    break;
                case 2: case 3: case 4:
                    tempArray = colorGen.Stripes();
                    break;
                case 5:
                    tempArray = colorGen.RandomColor();
                    break;
            }

            colorMethod = Game1.RNG.Next(0, 2);
            switch (colorMethod)
            {
                case 0:

                    break;
                case 1:
                    tempArray = blockGen.ReflectOnce(tempArray);
                    break;
                case 2:
                    tempArray = blockGen.ReflectTwice(tempArray);
                    break;
            }

            colorMethod = Game1.RNG.Next(0, 5);
            switch (colorMethod)
            {
                case 0: case 1:
                    tempArray = colorGen.Walk();
                    break;
                case 2: case 3: case 4:
                    tempArray = colorGen.Stripes();
                    break;
                case 5:
                    tempArray = colorGen.RandomColor();
                    break;
            }

            colorMethod = Game1.RNG.Next(0, 2);
            switch (colorMethod)
            {
                case 1:
                    colorMethod = Game1.RNG.Next(0, 1);
                    if (colorMethod == 0)
                        for (int i = 0; i < 15; i++)
                        {
                            colorMethod = Game1.RNG.Next(0, 2);
                            if (colorMethod == 0)
                            {
                                for (int j = 0; j < 12; j++)
                                {
                                    tempArray[i, j] = tempArray2[i, j];
                                }
                            }
                        }
                    else
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            colorMethod = Game1.RNG.Next(0, 2);
                            if (colorMethod == 0)
                            {
                                for (int j = 0; j < 15; j++)
                                {
                                    tempArray[j, i] = tempArray2[j, i];
                                }
                            }
                        }
                    }
                    break;
            }


            return tempArray;
        }

        public int[,] AddArrays(int[,] tempArray1, int[,] tempArray2)
        {
            int[,] outputArray = new int[15,12];

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    outputArray[i, j] = tempArray1[i, j];
                }
            }
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    if (tempArray2[i, j] == 1)
                    {
                        outputArray[i, j] = 1;
                    }
                }
            }
            return outputArray;
        }

        public int[,] SubtractArrays(int[,] tempArray1, int[,] tempArray2)
        {
            int[,] outputArray = new int[15, 12];

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    outputArray[i, j] = tempArray1[i, j];
                }
            }
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    if (tempArray2[i, j] == 0)
                    {
                        outputArray[i, j] = 0;
                    }
                }
            }
            return outputArray;
        }

        #endregion
    }

    class BlockGenerator
    {
        #region Variables



        #endregion

        #region Methods
        public BlockGenerator()
        {

        }

        #region Perlin
        public int[,] Perlin()
        {
            int tileCount;
            double tempAmount;
            int[,] tileHolder = new int[15, 12];
            double[,] doubleHolder = new double[15, 12];
            double[,] neighborArrays = new double[15, 12];

            //do
            //{
                tileCount = 0;
                Array.Clear(tileHolder, 0, tileHolder.Length);
                Array.Clear(doubleHolder, 0, doubleHolder.Length);
                Array.Clear(neighborArrays, 0, neighborArrays.Length);

                for (int k = 0; k < 5; k++)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        for (int j = 0; j < 12; j++)
                        {
                            doubleHolder[i, j] = Game1.RNG.NextDouble();
                        }
                    }
                }

                //Create array adding a fifth to neighboring array elements
                for (int i = 0; i < 15; i++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        tempAmount = doubleHolder[i, j];
                        tempAmount /= 2;

                        if (i > 0)
                        {
                            neighborArrays[i - 1, j] += tempAmount;
                        }
                        if (i < 14)
                        {
                            neighborArrays[i + 1, j] += tempAmount;
                        }
                        if (j > 0)
                        {
                            neighborArrays[i, j - 1] += tempAmount;
                        }
                        if (j < 11)
                        {
                            neighborArrays[i, j + 1] += tempAmount;
                        }
                    }
                }

                //Add neighborArray to the Original Array
                for (int i = 0; i < 15; i++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        doubleHolder[i, j] += neighborArrays[i, j];
                        doubleHolder[i, j] /= 5;
                    }
                }

                //Set tile existance to one if over threshold
                for (int i = 0; i < 15; i++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        if (doubleHolder[i, j] > 0.30f)
                        {
                            tileHolder[i, j] = 1;
                            tileCount += 1;
                        }
                        else
                        {
                            tileHolder[i, j] = 0;
                        }
                    }
                }
            
            //} while (tileCount < 80 || tileCount > 140);
           
            return tileHolder;
        }
        #endregion

        #region Shapes
        public int[,] Shapes()
        {
            int[,] blockArray = new int[15, 12];
            int[,] largerArray = new int[25, 22];

            //Random RNG = new Random();
            Vector2 shapeOffset = new Vector2();
            Array.Clear(blockArray, 0, blockArray.Length);
            Array.Clear(largerArray, 0, largerArray.Length);
            List<SimpleShape> simpShap = new List<SimpleShape>();
            
            for (int i = 0; i < 20; i++)
            {
                simpShap.Add(new SimpleShape());
            }

            //Keep track of amount of field filled
            double percentNeeded;
            double percentFilled = 0;
            int blockCount;
            int xVal = 0;
            int yVal = 0;
            int cycleCount = 0;

            //**Implement code below maybe
            //percentNeeded = RNG.NextDouble();
            percentNeeded = 0.6f;

            do
            {
                //Create a simple shape to place in field

                //**Put in some exception handling

                //Pick a random location where the shape will fit on the screen
                shapeOffset.X = Game1.RNG.Next(2, 14);
                shapeOffset.Y = Game1.RNG.Next(2, 11);

                /*
                //**Put vector values into numbers, shaving off too large or too small values
                //**Try to find a method to shorten this code
                //Check if shape intersects pre-existing shapes
                xVal = (int)shapeOffset.X + (int)simpShap[0].Centre.X;
                yVal = (int)shapeOffset.Y + (int)simpShap[0].Centre.Y - simpShap[0].Space[0];
                if (xVal > 0 && xVal < 14)
                {
                    if (yVal > 0 && yVal < 11)
                    {
                        if (largerArray[xVal, yVal] == 1)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                xVal = (int)shapeOffset.X + (int)simpShap[0].Centre.X + simpShap[0].Space[1];
                yVal = (int)shapeOffset.Y + (int)simpShap[0].Centre.Y - simpShap[0].Space[1];
                if (xVal > 0 && xVal < 14)
                {
                    if (yVal > 0 && yVal < 11)
                    {
                        if (largerArray[xVal, yVal] == 1)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                xVal = (int)shapeOffset.X + (int)simpShap[0].Centre.X + simpShap[0].Space[2];
                yVal = (int)shapeOffset.Y + (int)simpShap[0].Centre.Y;
                if (xVal > 0 && xVal < 14)
                {
                    if (yVal > 0 && yVal < 11)
                    {
                        if (largerArray[xVal, yVal] == 1)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                xVal = (int)shapeOffset.X + (int)simpShap[0].Centre.X + simpShap[0].Space[3];
                yVal = (int)shapeOffset.Y + (int)simpShap[0].Centre.Y + simpShap[0].Space[3];
                if (xVal > 0 && xVal < 14)
                {
                    if (yVal > 0 && yVal < 11)
                    {
                        if (largerArray[xVal, yVal] == 1)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                xVal = (int)shapeOffset.X + (int)simpShap[0].Centre.X;
                yVal = (int)shapeOffset.Y + (int)simpShap[0].Centre.Y + simpShap[0].Space[4];
                if (xVal > 0 && xVal < 14)
                {
                    if (yVal > 0 && yVal < 11)
                    {
                        if (largerArray[xVal, yVal] == 1)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                xVal = (int)shapeOffset.X + (int)simpShap[0].Centre.X - simpShap[0].Space[5];
                yVal = (int)shapeOffset.Y + (int)simpShap[0].Centre.Y + simpShap[0].Space[5];
                if (xVal > 0 && xVal < 14)
                {
                    if (yVal > 0 && yVal < 11)
                    {
                        if (largerArray[xVal, yVal] == 1)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                xVal = (int)shapeOffset.X + (int)simpShap[0].Centre.X - simpShap[0].Space[6];
                yVal = (int)shapeOffset.Y + (int)simpShap[0].Centre.Y;
                if (xVal > 0 && xVal < 14)
                {
                    if (yVal > 0 && yVal < 11)
                    {
                        if (largerArray[xVal, yVal] == 1)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                xVal = (int)shapeOffset.X + (int)simpShap[0].Centre.X - simpShap[0].Space[7];
                yVal = (int)shapeOffset.Y + (int)simpShap[0].Centre.Y - simpShap[0].Space[7];
                if (xVal > 0 && xVal < 14)
                {
                    if (yVal > 0 && yVal < 11)
                    {
                        if (largerArray[xVal, yVal] == 1)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
                */
                
                //**fix this bloody code.. look at blockArray... shapeoffset.. x
                //Place tiles into designated area
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (simpShap[cycleCount].Tiles[i, j] == 1)
                        {
                            largerArray[(int)shapeOffset.X + i, (int)shapeOffset.Y + j] = 1;
                        }
                    }
                }

                blockCount = 0;
                for (int i = 0; i < 15; i++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        if (largerArray[i, j] == 1)
                        {
                            blockCount++;
                        }
                    }
                }
                cycleCount++;
                //percentFilled = (float)(blockCount / 180);
            } while (cycleCount < 8);
            //} while (percentFilled < percentNeeded);
            //**Needed? simpShap.Clear();

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    blockArray[i, j] = largerArray[i + 3, j + 2];
                }
            }

            return blockArray;
        }
#endregion

        #region Directional
        public int[,] Directional()
        {
            int[,] tempArray = new int[15, 12];
            DirectionalBlocks dirBlocks = new DirectionalBlocks();

            tempArray = dirBlocks.Reset();
            return tempArray;
        }
        #endregion

        #region Borders
        public int[,] CreateBorders(int[,] input)
        {
            int[,] tempArray = input;
            
            int[] borderCuts;

            double cutDir;
            int cutNumber, cutType;

            //Choose cut direction 0 = Horizontal
            cutDir = Game1.RNG.NextDouble();

            //Choose number of cuts 0 = 1 Cut
            cutNumber = Game1.RNG.Next(0, 2);

            if (cutDir > 0.5f)
            {

                switch (cutNumber)
                {
                    case 0:
                        //Choose placement of cuts
                        cutType = Game1.RNG.Next(0, 2);

                        switch (cutType)
                        {
                            case 0:
                                borderCuts = new int[] { 0 };
                                tempArray = CutBorders(cutDir, borderCuts, tempArray);
                                break;
                            case 1:
                                borderCuts = new int[] { 5, 6 };
                                tempArray = CutBorders(cutDir, borderCuts, tempArray);
                                break;
                            case 2:
                                borderCuts = new int[] { 11 };
                                tempArray = CutBorders(cutDir, borderCuts, tempArray);
                                break;
                        }

                        break;
                    case 1:
                        cutType = Game1.RNG.Next(0, 3);

                        switch (cutType)
                        {
                            case 0:
                                borderCuts = new int[] { 0, 11 };
                                tempArray = CutBorders(cutDir, borderCuts, tempArray);
                                break;
                            case 1:
                                borderCuts = new int[] { 0, 5, 6 };
                                tempArray = CutBorders(cutDir, borderCuts, tempArray);
                                break;
                            case 2:
                                borderCuts = new int[] { 5, 6, 11 };
                                tempArray = CutBorders(cutDir, borderCuts, tempArray);
                                break;
                            case 3:
                                borderCuts = new int[] { 3, 8 };
                                tempArray = CutBorders(cutDir, borderCuts, tempArray);
                                break;
                        }

                        break;
                    case 2:
                        cutType = 0;

                        borderCuts = new int[] { 0, 5, 6, 11 };
                        tempArray = CutBorders(cutDir, borderCuts, tempArray);
                        break;
                }
            }
            else
            {
                switch (cutNumber)
                {
                    case 0:
                        cutType = Game1.RNG.Next(0, 2);

                        switch (cutType)
                        {
                            case 0:
                                borderCuts = new int[] { 0 };
                                tempArray = CutBorders(cutDir, borderCuts, tempArray);
                                break;
                            case 1:
                                borderCuts = new int[] { 7 };
                                tempArray = CutBorders(cutDir, borderCuts, tempArray);
                                break;
                            case 2:
                                borderCuts = new int[] { 14 };
                                tempArray = CutBorders(cutDir, borderCuts, tempArray);
                                break;
                        }

                        break;
                    case 1:
                        cutType = Game1.RNG.Next(0, 3);
                        switch (cutType)
                        {
                            case 0:
                                borderCuts = new int[] { 0, 14 };
                                tempArray = CutBorders(cutDir, borderCuts, tempArray);
                                break;
                            case 1:
                                borderCuts = new int[] { 0, 7 };
                                tempArray = CutBorders(cutDir, borderCuts, tempArray);
                                break;
                            case 2:
                                borderCuts = new int[] { 7, 14 };
                                tempArray = CutBorders(cutDir, borderCuts, tempArray);
                                break;
                            case 3:
                                borderCuts = new int[] { 4, 10 };
                                tempArray = CutBorders(cutDir, borderCuts, tempArray);
                                break;
                        }
                        break;
                    case 2:
                        borderCuts = new int[] { 0, 7, 14 };
                        tempArray = CutBorders(cutDir, borderCuts, tempArray);
                        break;
                }
            }
            return tempArray;
        }

        public int[,] CutBorders(double direction, int[] cuts, int[,] origArray)
        {


            if (direction > 0.5f)
            {
                for (int i = 0; i < cuts.Length; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        origArray[j, cuts[i]] = 0;
                    }
                }
            }
            else
            {
                for (int i = 0; i < cuts.Length; i++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        origArray[cuts[i], j] = 0;
                    }
                }
            }
            return origArray;
        }
        #endregion

        #region Reflect
        public int[,] ReflectOnce(int[,] input)
        {
            double tempRand1, tempRand2;
            tempRand1 = Game1.RNG.NextDouble();
            tempRand2 = Game1.RNG.NextDouble();

            if (tempRand1 > 0.5f)
            {
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        if (tempRand2 > 0.5f)
                        {
                            input[14 - i, j] = input[i, j];
                        }
                        else
                        {
                            input[i, j] = input[14 - i, j];
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 14; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        if (tempRand2 > 0.5f)
                        {
                            input[i, 11 - j] = input[i, j];
                        }
                        else
                        {
                            input[i, j] = input[i, 11 - j];
                        }
                    }
                }
            }
            return input;
        }

        public int[,] ReflectTwice(int[,] input)
        {
            double tempRand;
            tempRand = Game1.RNG.NextDouble();

            if (tempRand > 0.5f)
            {
                for (int i = 5; i < 10; i++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                            input[9 - i, j] = input[i, j];
                            input[14 - (i - 5), j] = input[i, j];
                    }
                }
            }
            else
            {
                for (int i = 0; i < 14; i++)
                {
                    for (int j = 4; j < 8; j++)
                    {
                        input[i, 7 - j] = input[i, j];
                        input[i, 11 - (j - 4)] = input[i, j];
                    }
                }
            }
            return input;
        }
        #endregion

        /*
        public int[,] method(int[,] input)
        {

        }
        */
        #endregion
    }

    class ColorGenerator
    {
        #region Variables
        #endregion

        #region Methods
        public ColorGenerator()
        {

        }

        #region Walk
        public int[,] Walk()
        {
            int[,] tempArray = new int[15, 12];
            Array.Clear(tempArray, 0, tempArray.Length);
            int randX, randY, randColor, randDir;

            for (int k = 0; k < 100; k++)
            {
                //Choose a random starting position
                randX = Game1.RNG.Next(0, 14);
                randY = Game1.RNG.Next(0, 11);
                //Only set to pick the first six colors
                randColor = Game1.RNG.Next(0, 11);

                tempArray[randX, randY] = randColor;

                //Move coloring tile set number of times
                for (int i = 0; i < 25; i++)
                {
                    randDir = Game1.RNG.Next(0, 3);
                    switch (randDir)
                    {
                        case 0:
                            if (randY == 0)
                            {
                                //break;
                            }
                            else
                            {
                                randY--;
                            }
                            break;
                        case 1:
                            if (randX == 14)
                            {
                                //break;
                            }
                            else
                            {
                                randX++;
                            }
                            break;
                        case 2:
                            if (randY == 11)
                            {
                                //break;
                            }
                            else
                            {
                                randY++;
                            }
                            break;
                        case 3:
                            if (randX == 0)
                            {
                                //break;
                            }
                            else
                            {
                                randX--;
                            }
                            break;
                    }
                    tempArray[randX, randY] = randColor;
                }
            }
            return tempArray;
        }
        #endregion

        #region Stripes
        public int[,] Stripes()
        {
            int[,] tempArray = new int[15, 12];

            double HorizOrVert;
            int cols, rows;
            int colCount, colColor;

            HorizOrVert = Game1.RNG.NextDouble();
            if (HorizOrVert > 0.5f)
            {
                cols = 15;
                rows = 12;
            }
            else
            {
                cols = 12;
                rows = 15;
            }

            colCount = 0;
            colColor = 0;

            for (int i = 0; i < cols; i++)
            {
                if (colCount == 0)
                {
                    colCount = Game1.RNG.Next(1, 3);
                    colColor = Game1.RNG.Next(0, 11);
                }
                for (int j = 0; j < rows; j++)
                {
                    if (HorizOrVert > 0.5f)
                    {
                        tempArray[i, j] = colColor;
                    }
                    else
                    {
                        tempArray[j, i] = colColor;
                    }
                }
                colCount--;
            }

            return tempArray;
        }
        #endregion

        #region RandomColor
        public int[,] RandomColor()
        {
            int[,] tempArray = new int[15, 12];
            int tempRand;

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    tempRand = Game1.RNG.Next(0, 11);
                    tempArray[i, j] = tempRand;
                }
            }

            return tempArray;
        }
        #endregion

        #endregion
    }

    class SolidGenerator
    {
        #region Variables



        #endregion

        #region Methods
        public SolidGenerator()
        {

        }


        #endregion
    }

    class SimpleShape
    {
        //Number from 0-14
        int shapeChoice;
        //Number 0-3 corresponding to 0, 90, 180, 270.
        int shapeRotation;

        int[,] choiceTiles;
        Vector2 choiceCentre;
        int[] choiceSpace;

        //Setup some properties for ease of use
        public int[,] Tiles
        {
            get
            {
                if (choiceTiles == null)
                    choiceTiles = new int[5, 5];

                return choiceTiles;
            }
            set
            {
                choiceTiles = value;
            }
        }

        public Vector2 Centre
        {
            get
            {
                if (choiceCentre == null)
                    choiceCentre = new Vector2();

                return choiceCentre;
            }
            set
            {
                choiceCentre = value;
            }
        }

        public int[] Space
        {
            get
            {
                if (choiceSpace == null)
                    choiceSpace = new int[8];

                return choiceSpace;
            }
            set
            {
                choiceSpace = value;
            }
        }

        //Array keeps track of shape tiles
        int[, ,] shapeTiles = {
                        //Circles
                        {{0,1,0,0,0},{1,1,1,0,0},{0,1,0,0,0},{0,0,0,0,0},{0,0,0,0,0}},
                        {{0,1,1,0,0},{1,1,1,1,0},{1,1,1,1,0},{0,1,1,0,0},{0,0,0,0,0}},
                        {{0,0,1,0,0},{0,1,1,1,0},{1,1,1,1,1},{0,1,1,1,0},{0,0,1,0,0}},
                        //Squares
                        {{1,1,0,0,0},{1,1,0,0,0},{0,0,0,0,0},{0,0,0,0,0},{0,0,0,0,0}},
                        {{1,1,1,0,0},{1,1,1,0,0},{1,1,1,0,0},{0,0,0,0,0},{0,0,0,0,0}},
                        {{1,1,1,1,0},{1,1,1,1,0},{1,1,1,1,0},{1,1,1,1,0},{0,0,0,0,0}},
                        //Right Triangles
                        {{1,1,0,0,0},{0,1,0,0,0},{0,0,0,0,0},{0,0,0,0,0},{0,0,0,0,0}},
                        {{1,1,1,0,0},{0,1,1,0,0},{0,0,1,0,0},{0,0,0,0,0},{0,0,0,0,0}},
                        {{1,1,1,1,0},{0,1,1,1,0},{0,0,1,1,0},{0,0,0,1,0},{0,0,0,0,0}},
                        //Isosceles Triangles
                        {{0,1,0,0,0},{1,1,0,0,0},{0,1,0,0,0},{0,0,0,0,0},{0,0,0,0,0}},
                        {{0,1,0,0,0},{1,1,0,0,0},{1,1,0,0,0},{0,1,0,0,0},{0,0,0,0,0}},
                        {{0,0,1,0,0},{0,1,1,0,0},{1,1,1,0,0},{0,1,1,0,0},{0,0,1,0,0}},
                        //Miscilaneous Shapes
                        {{1,0,0,0,0},{1,0,0,0,0},{1,0,0,0,0},{1,0,0,0,0},{0,0,0,0,0}},
                        {{0,1,0,0,0},{1,1,0,0,0},{1,0,0,0,0},{0,0,0,0,0},{0,0,0,0,0}},
                        {{1,1,0,0,0},{1,0,0,0,0},{1,0,0,0,0},{1,0,0,0,0},{0,0,0,0,0}}
                              };

        //Keeps track of 'Centre of shapes'
        Vector2[] shapeCentre = {
                                    //Circles
                                    new Vector2(1,1), new Vector2(1,1), new Vector2(2,2),
                                    //Squares
                                    new Vector2(0,0), new Vector2(1,1), new Vector2(1,1),
                                    //Right Triangles
                                    new Vector2(0,1), new Vector2(0,2), new Vector2(0,3),
                                    //Isosceles Triangles
                                    new Vector2(1,1), new Vector2(1,1), new Vector2(2,2),
                                    //Miscilaneous Shapes
                                    new Vector2(0,0), new Vector2(1,1), new Vector2(0,0)};

        //Amount of Space, from centre, in 8 directions: Up, Right, Down, Left, UR, DR, DL, UL
        int[,] shapeSpace = {
                                //Circles
                                {1,1,1,1,0,0,0,0},
                                {1,2,2,1,1,1,1,0},
                                {2,2,2,2,1,1,1,1},
                                //Squares
                                {0,1,1,0,0,1,0,0},
                                {1,1,1,1,1,1,1,1},
                                {1,2,2,1,1,2,1,1},
                                //Right Triangles
                                {1,1,0,0,0,0,0,0},
                                {2,2,0,0,1,0,0,0},
                                {3,3,0,0,1,0,0,0},
                                //Isosceles Triangles
                                {1,1,0,1,0,0,0,0},
                                {1,2,0,1,1,0,0,0},
                                {2,2,0,2,1,0,0,1},
                                //Miscilaneous Shapes
                                {0,3,0,0,0,0,0,0},
                                {1,0,0,1,1,0,0,0},
                                {0,3,1,0,0,0,0,0}
                            };

        public SimpleShape()
        {
            //Chooose shape
            shapeChoice = Game1.RNG.Next(0, 14);
            shapeRotation = Game1.RNG.Next(0, 3);

            //Place shape info into seperate arrays
            choiceTiles = new int[5, 5];
            choiceSpace = new int[8];

            //Tile Info
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    choiceTiles[i, j] = shapeTiles[shapeChoice, i, j];
                }
            }
            choiceTiles = RotateTiles(shapeRotation, choiceTiles);

            //Centre Info
            choiceCentre = shapeCentre[shapeChoice];
            choiceCentre = RotateCentre(shapeRotation, choiceCentre);

            //Space Info
            for (int i = 0; i < 8; i++)
            {
                choiceSpace[i] = shapeSpace[shapeChoice, i];
            }
            choiceSpace = RotateSpace(shapeRotation, choiceSpace);
        }

        public int[,] RotateTiles(int m_Rotation, int[,] m_Tiles)
        {
            //If the shape isn't rotated, exit out of method
            if (m_Rotation == 0)
            {
                return m_Tiles;
            }

            //Setup an array to hold rotated elements
            int[,] tempTileArray = new int[5, 5];
            //Set array to all zeros, so I just need to plug in the ones.
            Array.Clear(tempTileArray, 0, tempTileArray.Length);

            //Vectors to toss around data in
            List<Vector2> vectorList = new List<Vector2>();
            Vector2 tempVector = new Vector2();
            Vector2 extraVector = new Vector2();

            //List of all tile positions
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (m_Tiles[i, j] == 1)
                    {
                        vectorList.Add(new Vector2(i, j));
                    }
                }
            }

            //Moving origin position of vectors to allow rotation
            for (int i = 0; i < vectorList.Count(); i++)
            {
                tempVector = vectorList[i];

                tempVector.X -= 2;
                tempVector.Y -= 2;

                vectorList[i] = tempVector;
            }

            //Rotating vectors dependingly about origin
            switch (m_Rotation)
            {
                case 0:

                    break;
                case 1:
                    for (int i = 0; i < vectorList.Count(); i++)
                    {
                        tempVector = vectorList[i];

                        extraVector.X = -tempVector.Y;
                        extraVector.Y = tempVector.X;

                        vectorList[i] = extraVector;
                    }
                    break;
                case 2:
                    for (int i = 0; i < vectorList.Count(); i++)
                    {
                        tempVector = vectorList[i];

                        extraVector.X = -tempVector.X;
                        extraVector.Y = -tempVector.Y;

                        vectorList[i] = extraVector;
                    }
                    break;
                case 3:
                    for (int i = 0; i < vectorList.Count(); i++)
                    {
                        tempVector = vectorList[i];

                        extraVector.X = tempVector.Y;
                        extraVector.Y = -tempVector.X;

                        vectorList[i] = extraVector;
                    }
                    break;
            }

            //Moving origin back to original position
            for (int i = 0; i < vectorList.Count(); i++)
            {
                tempVector = vectorList[i];

                tempVector.X += 2;
                tempVector.Y += 2;

                vectorList[i] = tempVector;
            }

            for (int i = 0; i < vectorList.Count(); i++)
            {
                tempVector = vectorList[i];

                tempTileArray[(int)tempVector.X, (int)tempVector.Y] = 1;
            }

            return tempTileArray;
        }

        public Vector2 RotateCentre(int m_Rotation, Vector2 m_Centre)
        {
            if (m_Rotation == 0)
            {
                return m_Centre;
            }

            //Setup vectors to handle the transition
            Vector2 tempCentre = new Vector2();
            Vector2 extraCentre = new Vector2();

            tempCentre = m_Centre;

            //Adjust origin
            tempCentre.X -= 2;
            tempCentre.Y -= 2;

            //Rotating vectors dependingly about origin
            switch (m_Rotation)
            {
                case 0:

                    break;
                case 1:
                    extraCentre.X = -tempCentre.Y;
                    extraCentre.Y = tempCentre.X;
                    break;
                case 2:
                    extraCentre.X = -tempCentre.X;
                    extraCentre.Y = -tempCentre.Y;
                    break;
                case 3:
                    extraCentre.X = tempCentre.Y;
                    extraCentre.Y = -tempCentre.X;
                    break;
            }

            //Return to original origin
            extraCentre.X += 2;
            extraCentre.Y += 2;

            return extraCentre;
        }

        public int[] RotateSpace(int m_Rotation, int[] m_Space)
        {
            if (m_Rotation == 0)
            {
                return m_Space;
            }

            //Array to temporarily hold rotated values
            int[] tempSpace = new int[8];
            //Rotation amount
            int rotAmount = 0;
            int modNum;
            int finalMod;

            switch (m_Rotation)
            {
                case 0:

                    break;
                case 1:
                    rotAmount = -2;
                    break;
                case 2:
                    rotAmount = -4;
                    break;
                case 3:
                    rotAmount = -6;
                    break;
            }

            for (int i = 0; i < 8; i++)
            {
                modNum = i + rotAmount;
                finalMod = ((modNum % 8) + 8) % 8;

                tempSpace[i] = m_Space[finalMod];
            }

            return tempSpace;
        }
    }

    class DirectionalBlocks
    {
        //General Variables
        private int[,] normalBlockArray1;
        private int[,] normalBlockArray2;
        private int[,] normalBlockArray3;
        private int[,] solidBlockArray;
        private int[,] diagArray;
        private int count1, count2, count3;

        //Debug
        private SpriteFont dbFont;
        private string dbString;
        private int[] dbArray1;
        private int[,] dbArray2;
        private int[,] dbArray3;
        private bool dbCounter1, dbCounter2;

        //For reflections
        private int[,] tempArray;
        private Texture2D normalTxr;
        private Texture2D solidTxr;
        private Vector2 offset;

        private int tempRand, tempRand1, tempRand2, tempRand3;
        private SeedType gameSeed;
        private CutDir startCut;
        //Keeps track of cuts
        private int[] cuttingEdge;
        //Angle that diagonal cuts at 1-1, 2-1, 3-1
        private int cuttingAngle;
        private Random RNG = new Random();
        //Remaining length of blocks when cutting
        private int edgeLength;
        private int remainingLength;

        //Constructor
        public DirectionalBlocks()
        {
            //Initialise General Variables
            normalBlockArray1 = new int[15, 12];
            normalBlockArray2 = new int[15, 12];
            normalBlockArray3 = new int[15, 12];

            solidBlockArray = new int[15, 12];
            diagArray = new int[27, 12];
            tempArray = new int[15, 12];
            Array.Clear(diagArray, 0, diagArray.Length);
            offset = new Vector2(50, 50);
        }

        public int[,] Reset()
        {
            /*
            Array.Clear(normalBlockArray1, 0, normalBlockArray1.Length);
            Array.Clear(normalBlockArray2, 0, normalBlockArray2.Length);
            Array.Clear(normalBlockArray3, 0, normalBlockArray3.Length);
            count1 = 0;
            count2 = 0;
            count3 = 0;
            */

            tempRand = RNG.Next(0, 4);

            gameSeed = (SeedType)tempRand;
            
            gameSeed = SeedType.Typical;
            
            switch(gameSeed)
            { 
                case SeedType.Typical:

                    do
                    {
                        Array.Clear(normalBlockArray1, 0, normalBlockArray1.Length);
                        Array.Clear(normalBlockArray2, 0, normalBlockArray2.Length);
                        Array.Clear(normalBlockArray3, 0, normalBlockArray3.Length);

                        count1 = 0;
                        count2 = 0;
                        count3 = 0;

                        TypicalCut(normalBlockArray1);
                        TypicalCut(normalBlockArray2);
                        TypicalCut(normalBlockArray3);
                        
                        for (int i = 0; i < 15; i++)
                        {
                            for (int j = 0; j < 12; j++)
                            {
                                if (normalBlockArray1[i, j] == 1)
                                {
                                    count1++;
                                }
                                if (normalBlockArray2[i, j] == 1)
                                {
                                    count2++;
                                }
                                if (normalBlockArray3[i, j] == 1)
                                {
                                    count3++;
                                }
                            }
                        }

                        if (count1 > count2 && count1 > count3)
                        {
                            if (count3 > count2)
                            {
                                SwitchArrays(ref normalBlockArray2, ref normalBlockArray3);
                            }
                            break;
                        }
                        else if (count2 > count3)
                        {
                            SwitchArrays(ref normalBlockArray2, ref normalBlockArray3);

                            if (count1 > count3)
                            {
                                SwitchArrays(ref normalBlockArray2, ref normalBlockArray3);
                            }
                        }
                        else
                        {
                            SwitchArrays(ref normalBlockArray2, ref normalBlockArray3);

                            if (count1 > count2)
                            {
                                SwitchArrays(ref normalBlockArray2, ref normalBlockArray3);
                            }
                        }
                        
                        int tempSum = 0;

                        for (int i = 0; i < 15; i++)
                        {
                            for (int j = 0; j < 12; j++)
                            {
                                tempSum = 0;

                                tempSum = normalBlockArray1[i, j] - normalBlockArray2[i, j];

                                if (tempSum == 0 || tempSum == 1)
                                {
                                    normalBlockArray1[i, j] = tempSum;
                                }
                            }
                        }

                        for (int i = 0; i < 15; i++)
                        {
                            for (int j = 0; j < 12; j++)
                            {
                                tempSum = 0;

                                tempSum = normalBlockArray1[i, j] - normalBlockArray3[i, j];

                                if (tempSum == 0 || tempSum == 1)
                                {
                                    normalBlockArray1[i, j] = tempSum;
                                }
                            }
                        }

                        count1 = 0;
                        for (int i = 0; i < 15; i++)
                        {
                            for (int j = 0; j < 12; j++)
                            {
                                if (normalBlockArray1[i, j] == 1)
                                {
                                    count1++;
                                }
                            }
                        }
                    } while (count1 > 100 || count1 < 90);
                    break;
                   
                case SeedType.Blocky:

                    break;
                   
                case SeedType.Shape:

                    break;
                   
                case SeedType.Holey:

                    break;

                case SeedType.LemonDiff:

                    break;
            }
            return normalBlockArray1;
        }

        public void SwitchArrays(ref int[,] array1, ref int[,] array2)
        {
            int[,] switchTempArray = new int[15, 12];

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    switchTempArray[i, j] = array1[i, j];
                    array1[i, j] = array2[i, j];
                    array2[i, j] = switchTempArray[i, j];
                }
            }

        }

        public void TypicalCut(int[,] blockArray)
        {


            tempRand1 = RNG.Next(0, 4);

            startCut = (CutDir)tempRand1;

            switch(startCut)
            { 
                case CutDir.Vertical:
                    cuttingEdge = new int[12]; 
                    break;
                   
                case CutDir.Horizontal:
                    cuttingEdge = new int[15];
                    break;
                   
                case CutDir.LeftDiagonal:
                    cuttingAngle = RNG.Next(1,2);
                    cuttingEdge = new int[27];
                    break;
                   
                case CutDir.RightDiagonal:
                    cuttingAngle = RNG.Next(1,2);
                    cuttingEdge = new int[27];
                    break;
            }
            
            StartCutting();
            TransferCut(normalBlockArray1);
            CreateBorders();

            

        }


        public void StartCutting()
        {
            int p = 0;

            do
            {
                if (cuttingEdge[p] == 0)
                {
                    edgeLength = cuttingEdge.Length - 1;

                    GetCutLength(p, edgeLength);

                    //Trying to give areas more space
                    
                    if (tempRand2 == 1)
                    {
                        p++;
                        tempRand2 = 0;
                    }
                    
                }
                p += 3;
            } while (p < cuttingEdge.Length);

            dbArray1 = cuttingEdge;
        }

        public void GetCutLength(int j, int m)
        {
            remainingLength = m - j;
            
            //Should this be changed to -1
            if (remainingLength > 0)
            {
                tempRand2 = RNG.Next(1,3);
                

                if (tempRand2 == 1)
                {
                    tempRand3 = 6;
                    do
                    {
                        tempRand3 = RNG.Next(1, remainingLength);
                    }
                    while (tempRand3 > 5);

                    for (int k = j; k < j + tempRand3; k++)
                    {
                        cuttingEdge[k] = 1;
                    }
                }
            }
        }

        public void TransferCut(int[,] blockArray)
        {
                    switch (startCut)
                    {
                        case CutDir.Vertical:
                            for (int i = 0; i < cuttingEdge.Length; i++)
                            {
                                if (cuttingEdge[i] == 1)
                            {
                            
                            for (int j = 0; j < 15; j++)
                            {
                                blockArray[j, i] = 1;
                            }

                            }

                            }
                            break;

                        case CutDir.Horizontal:
                            for (int i = 0; i < cuttingEdge.Length; i++)
                            {
                                if (cuttingEdge[i] == 1)
                            {
                                    
                            for (int j = 0; j < 12; j++)
                            {
                                blockArray[i, j] = 1;
                            }

                            }

                            }
                            break;

                        case CutDir.LeftDiagonal:
                            for (int i = 0; i < cuttingEdge.Length; i++)
                            {
                                if (cuttingEdge[i] == 1)
                                {

                                    for (int j = 0; j < 12; j++)
                                    {
                                        if (j <= i)
                                        {
                                            diagArray[i - j, 11 - j] = 1;
                                        }
                                    }
                                    for (int k = 0; k < 15; k++)
                                    {
                                        for (int m = 0; m < 12; m++)
                                        {
                                            blockArray[k, m] = diagArray[k, m];
                                        }
                                    }
                                }
                            }
                            Array.Clear(diagArray, 0, diagArray.Length);
                            break;

                        case CutDir.RightDiagonal:
                            //Same as left diagonal but reflected

                            for (int i = 0; i < cuttingEdge.Length; i++)
                            {
                                if (cuttingEdge[i] == 1)
                                {

                                    for (int j = 0; j < 12; j++)
                                    {
                                        if (j <= i)
                                        {
                                            diagArray[i - j, 11 - j] = 1;
                                        }
                                    }
                                    for (int k = 0; k < 15; k++)
                                    {
                                        for (int m = 0; m < 12; m++)
                                        {
                                            blockArray[k, m] = diagArray[k, m];
                                        }
                                    }
                                }
                            }
                            
                            for (int m = 0; m < 12; m++)
                            {
                                for (int n = 0; n < 15; n++)
                                {
                                    tempArray[(14 - n), m] = blockArray[n, m];
                                }
                                
                            }

                            for (int m = 0; m < 12; m++)
                            {
                                for (int n = 0; n < 15; n++)
                                {
                                    blockArray[n, m] = tempArray[n, m];
                                }

                            }
                            //blockArray = tempArray;
                            break;
                    }
        }

        //Drawing Method
        public void DrawMe(SpriteBatch sb)
        {
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    if(normalBlockArray1[i,j] == 1)
                    {
                        sb.Draw(normalTxr, new Rectangle((int)(offset.X * i), (int)(offset.Y * j), normalTxr.Width, normalTxr.Height), Color.White);
                    }
                }
            }

            /*
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    if (solidBlockArray[i, j] == 1)
                    {
                        sb.Draw(solidTxr, new Rectangle((int)(offset.X * i), (int)(offset.Y * j), solidTxr.Width, solidTxr.Height), Color.White);
                    }
                }
            }
            */

            /*
            for (int i = 0; i < dbArray1.Length; i++)
            {
                dbString = dbArray1[i].ToString();
                sb.DrawString(dbFont, dbString, new Vector2(i * 20, 10), Color.Black);
            }
            */

            //cuttingEdge
            //normalBlockArray

            //sb.DrawString(dbFont, remainingLength.ToString(), new Vector2(20, 50), Color.Black);

            
            sb.DrawString(dbFont, tempRand.ToString(), new Vector2(0, 40), Color.Black);
            sb.DrawString(dbFont, tempRand1.ToString(), new Vector2(20, 40), Color.Black);
            sb.DrawString(dbFont, tempRand2.ToString(), new Vector2(40, 40), Color.Black);
            sb.DrawString(dbFont, tempRand3.ToString(), new Vector2(60, 40), Color.Black);

            dbString = startCut.ToString();
            sb.DrawString(dbFont, dbString, new Vector2(80, 40), Color.Black);
            
            
            
            
        }

        public void CreateBorders()
        {
            int[] borderCuts;

            Random RNG = new Random();
            double cutDir;
            int cutNumber, cutType;

            //Choose cut direction 0 = Horizontal
            cutDir = RNG.NextDouble();

            //Choose number of cuts 0 = 1 Cut
            cutNumber = RNG.Next(0, 2);

            if (cutDir > 0.5f)
            {

                switch (cutNumber)
                {
                    case 0:
                        //Choose placement of cuts
                        cutType = RNG.Next(0, 2);

                        switch (cutType)
                        {
                            case 0:
                                borderCuts = new int[] { 0 };
                                CutBorders(cutDir, borderCuts);
                                break;
                            case 1:
                                borderCuts = new int[] { 5, 6 };
                                CutBorders(cutDir, borderCuts);
                                break;
                            case 2:
                                borderCuts = new int[] { 11 };
                                CutBorders(cutDir, borderCuts);
                                break;
                        }

                        break;
                    case 1:
                        cutType = RNG.Next(0, 3);

                        switch (cutType)
                        {
                            case 0:
                                borderCuts = new int[] { 0, 11 };
                                CutBorders(cutDir, borderCuts);
                                break;
                            case 1:
                                borderCuts = new int[] { 0, 5, 6 };
                                CutBorders(cutDir, borderCuts);
                                break;
                            case 2:
                                borderCuts = new int[] { 5, 6, 11 };
                                CutBorders(cutDir, borderCuts);
                                break;
                            case 3:
                                borderCuts = new int[] { 3, 8 };
                                CutBorders(cutDir, borderCuts);
                                break;
                        }

                        break;
                    case 2:
                        cutType = 0;

                        borderCuts = new int[] { 0, 5, 6, 11 };
                        CutBorders(cutDir, borderCuts);
                        break;
                }
            }
            else
            {
                switch (cutNumber)
                {
                    case 0:
                        cutType = RNG.Next(0, 2);

                        switch (cutType)
                        {
                            case 0:
                                borderCuts = new int[] { 0 };
                                CutBorders(cutDir, borderCuts);
                                break;
                            case 1:
                                borderCuts = new int[] { 7 };
                                CutBorders(cutDir, borderCuts);
                                break;
                            case 2:
                                borderCuts = new int[] { 14 };
                                CutBorders(cutDir, borderCuts);
                                break;
                        }

                        break;
                    case 1:
                        cutType = RNG.Next(0, 3);
                        switch (cutType)
                        {
                            case 0:
                                borderCuts = new int[] { 0, 14 };
                                CutBorders(cutDir, borderCuts);
                                break;
                            case 1:
                                borderCuts = new int[] { 0, 7 };
                                CutBorders(cutDir, borderCuts);
                                break;
                            case 2:
                                borderCuts = new int[] { 7, 14 };
                                CutBorders(cutDir, borderCuts);
                                break;
                            case 3:
                                borderCuts = new int[] { 4, 10 };
                                CutBorders(cutDir, borderCuts);
                                break;
                        }
                        break;
                    case 2:
                        borderCuts = new int[] { 0, 7, 14 };
                        CutBorders(cutDir, borderCuts);
                        break;
                }
            }
        }

        public void CutBorders(double direction, int[] cuts)
        {
            if (direction > 0.5f)
            {
                for (int i = 0; i < cuts.Length; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        normalBlockArray1[j, cuts[i]] = 0;
                    }
                }
            }
            else
            {
                for (int i = 0; i < cuts.Length; i++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        normalBlockArray1[cuts[i], j] = 0;
                    }
                }
            }
        }
    }
}
