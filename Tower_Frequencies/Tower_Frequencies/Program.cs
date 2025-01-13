using System;
using Tower_Frequencies.Classes;
using Ookii.Dialogs.WinForms;
using System.Windows.Forms;
using Microsoft.Msagl;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

/*
 * Project By Dewald Oosthuizen for WIM Technologies
 * 
 * Main Purpose of this project is to create a program that can effectively assign frequency signals to Cell Phone Towers to minimize potential interferince between towers.
 * 
*/


namespace Tower_Frequencies
{
        
    class Program
    {

        [STAThread] // Needed to Open File Dialog
        static void Main(string[] args)
        {
            // Used to handel program continues excecution
            bool bContinue_Program = true;
            int iChoise = 0;
            bool bValid_Closest_Towers = true;

            // The number of closest towers to consider when assigning frequencies
            int iNum_Closest_Towers = 5; // The default is the optimal found for original problem

            // Stores users chosen txt file path
            string sFilePath = "";
            
            // Stores a list of all possible integer frequencies in a given range
            List<int> lFrequencies = new List<int>();

            // Stores a list of all cell tower information each in a cell tower object
            List<CellTower> lCell_Towers = new List<CellTower>();

            // Graph of cell tower network
            Graph graph_Cell_Towers = null;


            while ( bContinue_Program )
            {
                Console.Clear();
                Console.WriteLine(new string('-', 100));
                Console.WriteLine("\nOptions available in the program:");
                Console.WriteLine("1: read file and calculate tower frequencies");
                Console.WriteLine("2: Change number of closest towers to consider when assigning frequencies");
                Console.WriteLine("3: exit program\n");
                Console.WriteLine("Enter a number:");

                if (int.TryParse(Console.ReadLine(), out iChoise)){

                    switch (iChoise)
                    {
                        case 1:

                            // Get file path
                            sFilePath = InformationGathering.GetFilePath();

                            if(!(string.IsNullOrWhiteSpace(sFilePath)))
                            {
                                // Get list of Cell Tower infomration
                                lCell_Towers = InformationGathering.GetCellTowers(sFilePath);

                                // Handles file erros and skips the rest of the case
                                if(lCell_Towers == null || lCell_Towers.Count <= 0)
                                {
                                    Console.WriteLine("\nEnter a key to continue and try selecting a file again.");
                                    Console.ReadKey();
                                    continue;
                                }

                                // Get list of frequencies
                                lFrequencies = InformationGathering.GetFrequecyBound(lCell_Towers);

                                // Create the inital graph with and calculate distance between towers
                                graph_Cell_Towers = CellTowerNetworkGraph.CreateGraph(lCell_Towers, lFrequencies);

                                // Assigne each tower a frquency of the chosen range
                                graph_Cell_Towers = CellTowerNetworkGraph.CalFrequencies(graph_Cell_Towers, lFrequencies, iNum_Closest_Towers);

                                // List each tower and their new frequency
                                Console.WriteLine("\n" + new string('-', 100));
                                Console.WriteLine("\nTower information:");

                                foreach (var tower in graph_Cell_Towers.Nodes.OrderBy(node => (node.UserData as CellTower).Tower_Name))
                                {
                                    Console.WriteLine((tower.UserData as CellTower).ToString());
                                }


                                CellTowerNetworkGraph.VisualiseGraph(graph_Cell_Towers);

                                CellTowerNetworkGraph.DistanceMatrix(graph_Cell_Towers);

                                Console.WriteLine("\nEnter a key to continue.");
                                Console.ReadKey();
                            }

                            break;

                        case 2:

                            // Used to handle invalid input
                            bValid_Closest_Towers = true;
                            
                            // Asks user to enter new 
                            while (bValid_Closest_Towers)
                            {
                                Console.WriteLine($"\nThe current number of closest towers to consider is {iNum_Closest_Towers}.");
                                Console.WriteLine("Enter the new number of closest towers to consider:");

                                if (int.TryParse(Console.ReadLine(), out iNum_Closest_Towers))
                                {
                                    bValid_Closest_Towers = false;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid number. Please enter a valid integer.");
                                }

                            }

                            break;


                        case 3:
                            Console.WriteLine("\n" + new string('-', 100));
                            Console.WriteLine("\nThank you, have a greate day!");
                            bContinue_Program = false;
                            Console.ReadKey();
                            break;

                        default:
                            continue;
                    }

                }
            }

        }
    }

}