using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace BrainParser
{
    class Program
    {
        static StreamWriter sw;
        static int pointerpos = 0;

        //Main
        static void Main(string[] args)
        {
            string pathIn = "";
            while (!File.Exists(pathIn)) {
                Console.WriteLine("Enter source-file path:  ");
                pathIn = Console.ReadLine();
                Console.Clear();
            }

            string pathOut = "";
            while (!Directory.Exists(pathOut))
            {
                Console.WriteLine("Enter result-file path:  ");
                pathOut = Console.ReadLine();
                Console.Clear();
            }
            Console.WriteLine("Enter result-file name:  ");
            pathOut += "/" +Console.ReadLine();

            StreamReader sr = new StreamReader(pathIn);
            string input = sr.ReadToEnd();
            sr.Close();

            int[,] numbers = GenerateFactors(TextToAscii(input));
            int[][] pairs = GetCloseChars(numbers);
            
            PrintArray(numbers);
            Console.WriteLine();
            PrintArray(pairs);

            sw = new StreamWriter(pathOut);
            sw.Write("++++++++ew[ew>++++ew[");
            pointerpos++;

            CalcBase(numbers, pairs, 1);

            GotoPointer(1);
            sw.Write("-ew]");

            CalcBase(numbers, pairs, 2);

            sw.Write("ew[<]<-ew]");
            pointerpos = 0;

            Output(numbers, pairs);

            sw.Close();
        }

        //Debugging
        static void PrintArray(int[,] array)
        {
            for (int y = 0; y < array.GetLength(0); y++)
            {
                for (int x = 0; x < array.GetLength(1); x++)
                {
                    Console.Write(array[y, x] + "      ");
                }
                Console.WriteLine();
            }
        }
        static void PrintArray(int[][] array)
        {
            for (int y = 0; y < array.Length; y++)
            {
                for (int x = 0; x < array[y].Length; x++)
                {
                    Console.Write(array[y][x] + "      ");
                }
                Console.WriteLine();
            }
        }

        //Navigation
        static void GotoPointer(int nextCell)
        {
            if (nextCell < 0)
                return;

            sw.Write("ew");
            int dist = nextCell - pointerpos;
            for (int i = 0; i < Math.Abs(dist); i++)
                if (dist > 0)
                    sw.Write(">");
                else if (dist < 0)
                    sw.Write("<");

            pointerpos = nextCell;
        }

        static int GetGroupIndex(int[][] pairs, int checkIndex)
        {
            bool containsIndex = false;
            int groupIndex = 0;
            for(int y = 0; y < pairs.Length; y++)
            {
                for(int x = 0; x < pairs[y].Length; x++)
                    containsIndex = pairs[y][x] == checkIndex;
                if (containsIndex)
                {
                    groupIndex = y;
                    break;
                }
            }
            if (!containsIndex)
                return checkIndex;

            return pairs[groupIndex][0];
        }

        //Output
        static void CalcBase(int[,] numbers, int[][] pairs, int factorIndex)
        {
            for (int i = 0; i < pairs.Length; i++)
            {
                int times = numbers[pairs[i][0], factorIndex];
                GotoPointer(i + 2);

                for (int x = 0; x < times; x++)
                    sw.Write("+");
            }

            List<int> ungrouped = new List<int>();
            for (int y = 0; y < numbers.GetLength(0); y++)
                if (numbers[y, 3] < 0)
                    ungrouped.Add(numbers[y, factorIndex]);

            for(int i = 0; i < ungrouped.Count; i++)
            {
                GotoPointer(pairs.Length + i + 2);
                for (int x = 0; x < ungrouped[i]; x++)
                    sw.Write("+");
            }
        }

        static void Output(int[,] numbers, int[][] pairs)
        {
            int[] memory = new int[numbers.GetLength(0) + 2];
            int groupLessIdex = 0;
            for(int i = 0; i < numbers.GetLength(0); i++)
            {
                if(numbers[i,3] >= 0)
                    GotoPointer(numbers[i, 3] + 2);
                else
                {
                    GotoPointer(pairs.Length + groupLessIdex + 2);
                    groupLessIdex++;
                }

                if (memory[pointerpos] == 0)
                    memory[pointerpos] = numbers[i, 1] * 32 + numbers[i, 2] * 8;

                int dist = numbers[i, 0] - memory[pointerpos];
                for(int x = 0; x < Math.Abs(dist); x++)
                {
                    if (dist > 0)
                        sw.Write("+");
                    if (dist < 0)
                        sw.Write("-");
                }
                memory[pointerpos] = numbers[i, 0];
                sw.Write(".");
            }
        }

        //Text to Numbers
        static int[] TextToAscii(string word)
        {
            int[] numbers = new int[word.Length];
            for(int i = 0; i< word.Length; i++)
            {
                numbers[i] = (int)word[i];
            }
            return numbers;
        }

        static int[] GetFactors(int num)
        {
            int[] factors = new int[3];
            factors[0] = num / 32;
            factors[1] = (num % 32) / 8;
            factors[2] = -1;
            return factors;
        }

        static int[,] GenerateFactors(int[] text)
        {
            int[,] newText = new int[text.Length, 4];
            for(int i = 0; i < text.Length; i++)
            {
                newText[i, 0] = text[i];
                int[] factors = GetFactors(text[i]);
                for (int f = 0; f < factors.Length; f++)
                    newText[i, f + 1] = factors[f];
            }
            return newText;
        }

        static int[][] GetCloseChars(int[,] text)
        {
            List<int[]> numbers = new List<int[]>();
            List<int> alreadyChecked = new List<int>();
            for (int i = text.GetLength(0) -1; i >= 0; i--)
            {
                if (alreadyChecked.Contains(i))
                    continue;

                int currIndex = i;
                List<int> pair = new List<int>();

                for (int ii = i; ii >= 0; ii--)
                {
                    if (text[ii, 1] != text[currIndex,1] || text[ii,2] != text[currIndex,2] || Math.Abs((text[ii,3] - text[currIndex,3])) > 8)
                        continue;

                    pair.Add(ii);
                    currIndex = ii;
                    alreadyChecked.Add(ii);
                }
                if (pair.Count <= 1)
                    continue;

                foreach (int x in pair)
                    text[x, 3] = numbers.Count;
                pair.Reverse();
                numbers.Add(pair.ToArray());
            }

            return numbers.ToArray();
        }
    }
}