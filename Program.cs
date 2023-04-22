﻿using ConsoleGridEditor.Classes;
using System.Text;
using System.Xml.Linq;

namespace ConsoleGridEditor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            int selectedClass = ConsoleHelper.MultipleChoice(true, "New Grid", "Load Grid");

            if (selectedClass == 0)
            {
                CreateNewGrid();
            } 
            else if (selectedClass == 1)
            {
                throw new NotImplementedException("Load is not yet implemented");
            }
        }

        private static void CreateNewGrid()
        {
            bool useDoubleSPace;
            string strYesNo = "bla bla";
            string[] yesNoArr = { "n", "y" };
            int index = 1;
            ConsoleKey input;
            do
            {
                Console.Clear();
                Console.Write($"Double space: {yesNoArr[index]}\nUse {Emoji.Arrow_Up_Down} to change me and Tab to select");
                input = Console.ReadKey().Key;
                if (input == ConsoleKey.UpArrow)
                {
                    index = (index + 1) % 2;
                }
                else if (input == ConsoleKey.DownArrow)
                {
                    index = (index - 1 + 2) % 2;
                }
                else if (input == ConsoleKey.Tab)
                {
                    strYesNo = (index == 1) ? "true" : "false";
                }
            } while (!bool.TryParse(strYesNo, out useDoubleSPace));

            (int rows, int columns) = SelectRowsAndColumns();

            GridEditor.Editor(rows, columns, useDoubleSPace);
        }

        private static (int, int) SelectRowsAndColumns(int rows = 0, int columns = 0)
        {
            return SelectNumber("Rows:", "Columns:", 101, 101, rows, columns);
        }

        private static (int, int) SelectNumber(string text1, string text2, int maxNum1 = 100, int maxNum2 = 100, int startWith1 = 0, int startWith2 = 0)
        {
            int index1 = startWith1;
            int index2 = startWith2;
            bool selectingFirst = true;
            while (true)
            {
                Console.Clear();
                if (selectingFirst)
                    Console.WriteLine($"{Emoji.Arrow_Right_Hook} {text1} {index1}\n{text2} {index2}");
                else
                    Console.WriteLine($"{text1} {index1}\n{Emoji.Arrow_Right_Hook} {text2} {index2}");

                Console.WriteLine($"Use {Emoji.Arrow_Up_Down} to change and press Tab or Shift + Tab to select");
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key == ConsoleKey.UpArrow)
                {
                    if (selectingFirst)
                    {
                        index1 = (index1 + 1) % maxNum1;
                    }
                    else
                    {
                        index2 = (index2 + 1) % maxNum2;
                    }
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    if (selectingFirst)
                    {
                        index1 = (index1 - 1 + maxNum1) % maxNum1;
                    }
                    else
                    {
                        index2 = (index2 - 1 + maxNum2) % maxNum2;
                    }
                }
                else if ((key.Modifiers & ConsoleModifiers.Shift) == ConsoleModifiers.Shift && key.Key == ConsoleKey.Tab)
                {
                    selectingFirst = true;
                }
                else if (key.Key == ConsoleKey.Tab)
                {
                    if (selectingFirst)
                    {
                        selectingFirst = false;
                    }
                    else
                    {
                        break;
                    }
                }

            }
            return (index1, index2);
        }
    }
}