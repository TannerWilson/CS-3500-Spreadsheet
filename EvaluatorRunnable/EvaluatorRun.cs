using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormulaEvaluator;

namespace FormulaEvaluator_runable
{
    class EvaluateRun
    {
        static void Main(string[] args)
        {
            while(true)
            {
                Console.WriteLine("Enter an equation to solve");
                String input = Console.ReadLine();
                if (input == "Exit" || input == "exit") return;
                int result = FormulaEvaluator.Evaluator.Evaluate(input, varlookup);
                Console.WriteLine("The answer is: " + result);
            }
           
        }

        private static int varlookup(string varible)
        {
            if (varible == "A")
            {
                return 5;
            }
            else if (varible == "B")
            {
                return 2;
            }
            else throw new ArgumentException("Varible not found");
        }
    }
}

