using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleGridEditor.Classes
{
    internal static class GridEditor
    {

        static string DefaultDir { get; set; } = Path.GetFullPath(@"grids\");
        public static void RunEditor(this Grid<string> gridList, bool useDoubleSpace = false)
        {

            int gridRows = gridList.RowCount();
            int gridColumns = gridList.ColumnCount();

            int x = 1;
            int y = 1;

            CLearScreen();

            DrawGrid(gridList, x, y);

            int oldCursorLeft = Console.CursorLeft;
            int oldCursorTop = Console.CursorTop;

            while (true)
            {
                if (useDoubleSpace)
                    Console.SetCursorPosition(y * 2, x + 1);
                else
                    Console.SetCursorPosition(y, x + 1);

                //Console.SetCursorPosition(y * 2, x + 1);

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    if (x <= 0) x = gridRows - 1;
                    else x = Math.Max(0, x - 1);
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    if (x >= gridRows - 1) x = 0;
                    else x = Math.Min(gridRows - 1, x + 1);
                }
                else if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    if (y <= 0) y = gridColumns - 1;
                    else y = Math.Max(0, y - 1);
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    if (y >= gridColumns - 1) y = 0;
                    else y = Math.Min(gridColumns - 1, y + 1);
                }
                else if (keyInfo.Key == ConsoleKey.Spacebar)
                {
                    if (useDoubleSpace)
                    {
                        if (gridList.GetValue(x, y) != "  ")
                        {
                            gridList.SetValue(x, y, "  ");
                        }
                        else
                        {
                            gridList.SetValue(x, y, "* ");
                        }
                    } 
                    else
                    {
                        if (gridList.GetValue(x, y) != " ")
                        {
                            gridList.SetValue(x, y, " ");
                        }
                        else
                        {
                            gridList.SetValue(x, y, "*");
                        }
                    }
                    
                }
                else if (keyInfo.Key == ConsoleKey.Tab)
                {
                    FieldInfo[] fields = typeof(Emoji).GetFields(BindingFlags.Public | BindingFlags.Static);
                    // TBD: Hela den här double space grejen bör vara så att när man väljer ett tecken med double space så bör
                    // gridden ritas upp med double space och inte ett val man gör när man skapar gridden
                    //if (!gridList[x][y].DoubleSpace)
                    //{
                        /*List<FieldInfo> newField = new List<FieldInfo>();
                        for (int i = 0; i < fields.Length; i++)
                        {
                            if (fields[i].GetValue(null).ToString().Length == 1)
                            {
                                newField.Add(fields[i]);
                            }
                        }
                        fields = newField.ToArray();*/
                        //fields = new List<FieldInfo>(fields).FindAll(f => f.GetValue(null).ToString().Length == 1).ToArray();
                    //}

                    int currentIndex = 0;
                    int totalFields = fields.Length;

                    while (true)
                    {
                        Console.Clear();

                        FieldInfo currentField = fields[currentIndex];
                        Console.WriteLine(currentField.Name.Replace('_', ' '));
                        Console.WriteLine(currentField.GetValue(null));

                        ConsoleKeyInfo key = Console.ReadKey();

                        if (key.Key == ConsoleKey.UpArrow)
                        {
                            currentIndex = (currentIndex + 1) % totalFields;
                        }
                        else if (key.Key == ConsoleKey.DownArrow)
                        {
                            currentIndex = (currentIndex - 1 + totalFields) % totalFields;
                        }
                        else if (key.Key == ConsoleKey.Enter)
                        {
                            break;
                        }
                    }
                    gridList.SetValue(x, y, fields[currentIndex].GetValue(null).ToString());
                    if (useDoubleSpace && gridList.GetValue(x, y).Length == 1) gridList.AppendValue(x, y, " ");
                }
                else if (keyInfo.Key == ConsoleKey.L)
                {
                    Console.SetCursorPosition(oldCursorLeft, oldCursorTop);
                    string fileName = SelectFile();
                    Grid<string> temp = LoadFromFile(fileName);

                    gridRows = temp.RowCount();
                    gridColumns = temp.ColumnCount();
                    gridList = PopulateEmptyGrid(gridRows, gridColumns, useDoubleSpace);
                    gridList = temp;
                }
                else if (keyInfo.Key == ConsoleKey.S)
                {
                    Console.SetCursorPosition(oldCursorLeft, oldCursorTop);
                    SaveToFile(gridList);
                }
                else if (keyInfo.Key == ConsoleKey.R)
                {
                    Console.SetCursorPosition(oldCursorLeft, oldCursorTop);
                    string strColumns;
                    int columns;
                    do
                    {
                        Console.Write("Columns: ");
                        strColumns = Console.ReadLine();
                    } while (!int.TryParse(strColumns, out columns));

                    string strRows;
                    int rows;
                    do
                    {
                        Console.Write("Rows: ");
                        strRows = Console.ReadLine();
                    } while (!int.TryParse(strRows, out rows));

                    oldCursorLeft += Math.Min(0, Math.Abs(columns - gridColumns));
                    oldCursorTop += Math.Max(0, rows - gridRows);

                    gridRows = rows;
                    gridColumns = columns;
                    gridList = ResizeGridList(gridList, rows, columns, useDoubleSpace);
                }

                CLearScreen();
                DrawGrid(gridList, x, y);
            }
        }

        private static Grid<string> ResizeGridList(Grid<string> original, int rows, int cols, bool useDoubleSpace)
        {
            Grid<string> newList = new StringGrid(rows, cols);
            for(int row = 0; row < rows; row++)
            {
                for(int col = 0; col < cols; col++)
                {
                    if (row == 0 || row > rows - 2 || col == 0 || col > cols - 2)
                    {
                        if (useDoubleSpace) newList.SetValue(row, col, "* ");
                        else newList.SetValue(row, col, "*");
                    }
                    else
                    {
                        if (useDoubleSpace) newList.SetValue(row, col, "  ");
                        else newList.SetValue(row, col, " ");
                    }
                }
            }

            int minRows = Math.Min(rows, original.RowCount());
            int minCols = Math.Min(cols, original.ColumnCount());
            Debug.WriteLine($"mr = {minRows}, mc = {minCols}");
            for (int i = 0; i < minRows; i++)
            {
                for (int j = 0; j < minCols; j++)
                { 
                    if (useDoubleSpace && newList.GetValue(i, j) != "* ")
                        newList.SetValue(i, j, original.GetValue(i, j));
                    else if (newList.GetValue(i, j) != "*") newList.SetValue(i, j, original.GetValue(i, j));
                }
                    
            }

            return newList;
        }

        private static void CLearScreen()
        {
            // Clears the screen and the scrollback buffer in xterm-compatible terminals.
            Console.Clear(); Console.WriteLine("\x1b[3J");
        }

        public static Grid<string> PopulateEmptyGrid(int gridRows, int gridColumns, bool useDoubleSpace = false)
        {
            Grid<string> gridList = new StringGrid(gridRows, gridColumns);

            for (int row = 0; row < gridRows; row++)
            {
                for (int col = 0; col < gridColumns; col++)
                {
                    if (row == 0 || row > gridRows - 2 || col == 0 || col > gridColumns - 2)
                    {
                        if (useDoubleSpace) gridList.SetValue(row, col, "* ");
                        else gridList.SetValue(row, col, "*");
                    }
                    else
                    {
                        if (useDoubleSpace) gridList.SetValue(row, col, "  ");
                        else gridList.SetValue(row, col, " ");
                    }
                }
            }

            return gridList;
        }

        static void DrawGrid(Grid<string> gridList, int x, int y)
        {
            string toPrint = "";
            for (int row = 0; row < gridList.RowCount(); row++)
            {
                for (int col = 0; col < gridList.ColumnCount(); col++)
                {
                    toPrint += gridList.GetValue(row, col);
                }
                toPrint += "\n";
            }
            Console.WriteLine(toPrint);
            Console.WriteLine("Move with arrow keys");
            Console.WriteLine("Spacebar = add wall, Tab to select emoji");
            Console.WriteLine("L = Load from file, S = Save to file, R = Resize Grid");
        }

        static void SaveToFile(Grid<string> gridList)
        {
            Console.Write("Filename: ");
            string fileName = Console.ReadLine()!;

            if (!Directory.Exists(DefaultDir)) Directory.CreateDirectory(DefaultDir);

            string jsonString = JsonSerializer.Serialize(gridList);
            File.WriteAllText(DefaultDir + fileName + ".json", jsonString);
        }

        public static string[] GetJsonFiles()
        {
            return Directory.GetFiles(DefaultDir, "*.json", SearchOption.AllDirectories);
        }

        static string SelectFile()
        {
            string[] allJsonFIles = GetJsonFiles();

            int numberOfFiles = allJsonFIles.Length;
            int index = 0;

            while (true)
            {
                Console.Clear();

                Console.WriteLine($"File {Emoji.Arrow_Right_Hook} {Path.GetFileName(allJsonFIles[index]).Replace(".json", "")}");
                ConsoleKeyInfo key = Console.ReadKey();

                if (key.Key == ConsoleKey.UpArrow)
                {
                    index = (index - 1 + numberOfFiles) % numberOfFiles;
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    index = (index + 1) % numberOfFiles;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }
            }

            return allJsonFIles[index];
        }

        public static Grid<string> LoadFromFile(string fileName)
        {
            string jsonString = File.ReadAllText(fileName);

            Grid<string> grid = JsonSerializer.Deserialize<Grid<string>>(jsonString)!;

            return grid;
        }
    }
}
