using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsoleGridEditor.Classes
{

    // TBD: Kolla om StringGrid är ett alternatic. Mycket snabbare enligt mina tester. Till och med snabbare än Grin[,]
    // INFO: https://codereview.stackexchange.com/a/192737
    // INFO: Tester i ConsoleGameEditor__Slask projekter

    internal class Grid
    {
        /// <summary>
        /// Row
        /// </summary>
        public int x { get; set; }

        /// <summary>
        /// Column
        /// </summary>
        public int y { get; set; }

        public string Symbole { get; set; }

        // TBD: Undra om inte hela iden med double space bör skrotas. Efteroms det blir förvirrande och så gör det koden svårhanterad.
        // TBD: Antingen så skrotas alla emojis med två tecken eller så är det alltid double space
        // TBD: Om inte annat så bör denna varibel inte sättas här
        public bool DoubleSpace { get; set; }

        /// <summary>
        /// Grid constructor
        /// </summary>
        /// <param name="X">Row</param>
        /// <param name="Y">Column</param>
        /// <param name="doubleSpace">Use double space or not</param>
        [JsonConstructor]
        public Grid(int X, int Y, bool doubleSpace = false)
        {
            x = X;
            y = Y;
            DoubleSpace = doubleSpace;
        }

        public void SetSymbole(string symbole)
        {
            if (DoubleSpace && symbole.Length == 1)
                Symbole = $"{symbole} ";
            else
                Symbole = symbole;
        }

        public string GetSymbole()
        {
            return Symbole;
        }

        public void Clear()
        {
            if (DoubleSpace)
                Symbole = "  ";
            else
                Symbole = " ";
        }

        public void SetDoubleSpace(bool spacing)
        {
            DoubleSpace = spacing;
        }
    }
}
