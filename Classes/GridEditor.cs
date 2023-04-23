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
    internal class GridEditor
    {

        static string DefaultDir { get; set; } = Path.GetFullPath(@"grids\");
        public static void Editor(int gridRows, int gridColumns, bool useDoubleSpace = false)
        {
            List<List<Grid>> gridList = PopulateEmptyGrid(gridRows, gridColumns, useDoubleSpace);

            int x = 1;
            int y = 1;

            CLearScreen();

            DrawGrid(gridRows, gridColumns, gridList, x, y);

            int oldCursorLeft = Console.CursorLeft;
            int oldCursorTop = Console.CursorTop;

            while (true)
            {
                if (gridList[x][y].DoubleSpace)
                    Console.SetCursorPosition(y * 2, x + 1);
                else
                    Console.SetCursorPosition(y, x + 1);

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
                    if (gridList[x][y].DoubleSpace)
                    {
                        if (gridList[x][y].GetSymbole() != "  ")
                            gridList[x][y].SetSymbole(" ");
                        else
                            gridList[x][y].SetSymbole("*");
                    }
                    else
                    {
                        if (gridList[x][y].GetSymbole() != " ")
                            gridList[x][y].SetSymbole(" ");
                        else
                            gridList[x][y].SetSymbole("*");
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Tab)
                {
                    FieldInfo[] fields = typeof(Emoji).GetFields(BindingFlags.Public | BindingFlags.Static);
                    // TBD: Hela den här double space grejen bör vara så att när man väljer ett tecken med double space så bör
                    // gridden ritas upp med double space och inte ett val man gör när man skapar gridden
                    if (!gridList[x][y].DoubleSpace)
                    {
                        /*List<FieldInfo> newField = new List<FieldInfo>();
                        for (int i = 0; i < fields.Length; i++)
                        {
                            if (fields[i].GetValue(null).ToString().Length == 1)
                            {
                                newField.Add(fields[i]);
                            }
                        }
                        fields = newField.ToArray();*/
                        fields = new List<FieldInfo>(fields).FindAll(f => f.GetValue(null).ToString().Length == 1).ToArray();
                    }

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
                    gridList[x][y].SetSymbole(fields[currentIndex].GetValue(null).ToString());
                    if (gridList[x][y].DoubleSpace && gridList[x][y].GetSymbole().Length == 1) gridList[x][y].Symbole += " ";
                }
                else if (keyInfo.Key == ConsoleKey.L)
                {
                    Console.SetCursorPosition(oldCursorLeft, oldCursorTop);
                    List<List<Grid>> temp = LoadFromFile();

                    gridRows = temp.Count;
                    gridColumns = temp[0].Count; // TODO: Kolla om de verkligen behöver göra så här eftersom det är en port från array
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
                    gridList = ResizeGridArray(gridList, rows, columns);
                    // TODO: Throws exception when the resize multipla times
                }

                CLearScreen();
                DrawGrid(gridRows, gridColumns, gridList, x, y);
            }
        }

        private static List<List<Grid>> ResizeGridArray(List<List<Grid>> original, int rows, int cols)
        {
            List<List<Grid>> newList = new List<List<Grid>>(rows);
            for(int row = 0; row < rows; row++)
            {
                List<Grid> newGrid = new List<Grid>(cols);
                for(int col = 0; col < cols; col++)
                {
                    Grid grid = new Grid(rows, cols);
                    if (grid.x == 0 || grid.x > cols - 2 || grid.y == 0 || grid.y > rows - 2)
                    {
                        grid.SetSymbole("*");
                    }
                    else
                    {
                        grid.Clear();
                    }
                    newGrid.Add(grid);
                }
                newList.Add(newGrid);
            }
            return newList;
        }

        private static void CLearScreen()
        {
            // Clears the screen and the scrollback buffer in xterm-compatible terminals.
            Console.Clear(); Console.WriteLine("\x1b[3J");
        }

        private static List<List<Grid>> PopulateEmptyGrid(int gridRows, int gridColumns, bool useDoubleSpace = false)
        {
            List<List<Grid>> gridList = new List<List<Grid>>(gridRows);

            for (int rows = 0; rows < gridRows; rows++)
            {
                List<Grid> newGrid = new List<Grid>(gridColumns);
                for (int columns = 0; columns < gridColumns; columns++)
                {
                    Grid grid = new Grid(rows, columns, useDoubleSpace);
                    if (grid.x == 0 || grid.x > gridRows - 2 || grid.y == 0 || grid.y > gridColumns - 2)
                    {
                        grid.SetSymbole("*");
                    }
                    else
                    {
                        grid.Clear();
                    }
                    newGrid.Add(grid);
                }
                gridList.Add(newGrid);
            }

            return gridList;
        }

        static void DrawGrid(int gridRows, int gringColumns, List<List<Grid>> editGrid, int x, int y)
        {
            string toPrint = "";
            for (int row = 0; row < gridRows; row++)
            {
                for (int col = 0; col < gringColumns; col++)
                {
                    /*if (row == x && col == y)
                    {
                        if (editGrid[row, col].DoubleSpace)
                        {
                            toPrint += "+ ";
                        }
                        else
                        {
                            toPrint += "+";
                        }
                    }
                    else
                    {
                        toPrint += editGrid[row, col].GetSymbole();
                    }*/
                    toPrint += editGrid[row][col].GetSymbole();
                }
                toPrint += "\n";
            }
            Console.WriteLine(toPrint);
            Console.WriteLine("Move with arrow keys");
            Console.WriteLine("Spacebar = add wall, Tab to select emoji");
            Console.WriteLine("L = Load from file, S = Save to file, R = Resize Grid");
        }

        static void SaveToFile(List<List<Grid>> gridList)
        {
            Console.Write("Filename: ");
            string fileName = Console.ReadLine()!;

            if (!Directory.Exists(DefaultDir)) Directory.CreateDirectory(DefaultDir);

            string jsonString = JsonSerializer.Serialize(gridList);
            File.WriteAllText(DefaultDir + fileName + ".json", jsonString);
        }

        static string[] GetJsonFiles()
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

        static List<List<Grid>> LoadFromFile()
        {
            string fileName = SelectFile();
            string jsonString = File.ReadAllText(fileName);

            List<List<Grid>> grid = JsonSerializer.Deserialize<List<List<Grid>>>(jsonString)!;
            /*Grid[,] grid = new Grid[rows.Count, rows[0].Length];
            for (int i = 0; i < rows.Count; i++)
            {
                for (int j = 0; j < rows[i].Length; j++)
                {
                    grid[i, j] = rows[i][j];
                }
            }*/

            return grid;
        }
    }
}
