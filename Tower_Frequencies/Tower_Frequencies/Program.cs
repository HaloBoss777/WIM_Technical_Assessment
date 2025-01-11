using System;
using Tower_Frequencies.Classes;
using Ookii.Dialogs.WinForms;
using System.Windows.Forms;
using QuikGraph;
using QuikGraph.Graphviz;
using QuikGraph.Graphviz.Dot;


/*
 * Project By Dewald Oosthuizen for WIM Technologies
 * 
 * Main Purpose of this project is to create a program that can effectively assign frequency signals to Cell Phone Towers to minimize potential interferince between towers.
 * 
 * Program: Console Application.
 * Cell Tower info: Read from chosen file.
 * Frequency Range: 
 * 
*/


namespace Tower_Frequencies
{
        
    class Program
    {

        // Get all cell tower information
        public static List<CellTower> GetCellTowers(string sFilePath)
        {

            List<CellTower> lReturn = new List<CellTower>();

            // Handels case where file string is null or full of white spaces
            if (string.IsNullOrWhiteSpace(sFilePath))
            {
                Console.WriteLine("Invalid File Path. Cannot be null or Empty");
                return lReturn;
            }

            // Decalre Stream Reader to allow close in final block
            StreamReader sr = null;

            try
            {
                // Create Stream Readed Object to read in txt file with tower info
                sr = new StreamReader(sFilePath);

                //Variable To keep file line info
                string sline = "";

                //Array to keep seperated Tower info (5 default size)
                string[] arrTower_Info = new string[5];

                while (!sr.EndOfStream)
                {
                    sline = sr.ReadLine(); // Read Tower Info
                    arrTower_Info = sline.Split(";"); // Split info on ";" delimiter

                    // Create a Tower Object for each tower in the Txt file
                    CellTower cellTower = new CellTower()
                    {
                        Tower_Name = arrTower_Info[0],
                        Easting = double.Parse(arrTower_Info[1]),
                        Northing = double.Parse(arrTower_Info[2]),
                        Longitude = double.Parse(arrTower_Info[3]),
                        Latitude = double.Parse(arrTower_Info[4]),
                        Frequency = 0
                    };

                    // Add Tower Object to final return list
                    lReturn.Add(cellTower);

                }

            }
            catch (FileNotFoundException ex)
            {
                //Handels case when file is not found/chosen
                Console.WriteLine("\nFile not found/Chosen:");
                Console.WriteLine(ex.Message + "\n");
            }
            catch (FormatException ex)
            {
                //Handels case when type conversions fail
                Console.WriteLine("\nInvalid Types for Tower info:");
                Console.WriteLine(ex.Message + "\n");
            }
            finally
            {
                //Closes Reader after its Porpuse is complte
                if (sr != null)
                {
                    sr.Close();
                }

                // Indecate that everything worked :D (Legendary Rarity)
                Console.WriteLine("\nThe File has Loaded\n");
            }

            return lReturn;
        }

        // Allow User to Choose file path
        public static string GetFilePath()
        {
            // Viarable to Store filepath
            string sFilePath = "";

            // Use third party file chooser, as Windows.Forms does not work.
            VistaOpenFileDialog file_Dialog = new VistaOpenFileDialog();
            //Change the title
            file_Dialog.Title = "Select a Text file with Cell Tower information";
            //Restrict to only *.txt files (Description|Extension_Type)
            file_Dialog.Filter = "Text Files (*.txt)|*.txt";

            // Prompts the user to select a text file
            DialogResult choose_file = file_Dialog.ShowDialog();

            if (choose_file == DialogResult.OK)
            {
                //Retrive file path of chosen file
                sFilePath = file_Dialog.FileName;
            }
            else
            {
                Console.WriteLine("No file selected.");
            }
           

            // Return chosen file path
            return sFilePath;
        }

        // Allow the User to chose frequency range for cell towers
        public static List<int> GetFrequecyBound()
        {
            // Inclusive bounds for the frequencies
            int frequency_lower = 0;
            int frequency_upper = 0;

            // Used to test input
            bool bValid_bound = false;

            // Used to store all possible frequencies in a range
            List<int> lFrequencies = new List<int>();

            // Continue until correct input is given
            while (bValid_bound == false)
            {
                // Prompt for lower range
                Console.WriteLine("\nPlease enter lower bound for possible frequencies (Inclusive): ");

                if (int.TryParse(Console.ReadLine(), out frequency_lower))
                {

                    // Prompt for upper range
                    Console.WriteLine("\nPlease enter upper bound for possible frequencies (Inclusive): ");

                    if (int.TryParse(Console.ReadLine(), out frequency_upper))
                    {
                        bValid_bound = true;

                        // Add each frequecny to a list
                        for(int frequency = frequency_lower; frequency <= frequency_upper; frequency++)
                        {
                            lFrequencies.Add(frequency);
                        }

                    }
                    else
                    {
                        Console.WriteLine("Invalid Upper Bound Frequency. Try again.");
                        bValid_bound = false;
                    }

                }
                else
                {
                    Console.WriteLine("Invalid Lower Bound Frequency. Try again.");
                    bValid_bound = false;
                }
            }

            // Return list of all frequencies in a range
            return lFrequencies;
        }

        // Calculate Euclidean Distance between Towers (Small are using Easting and Northing)
        public static double CalDistance( double dTowerOne_Easting, double dTowerOne_Northing, double dTowerTwo_Easting, double dTowerTwo_Northing)
        {
            // Final Euclidean Result in meters
            double dDistance = 0;

            // Calculated the Euclidean distance using sqrt((E2 - E1)^2 + (N2 - N1)^2)
            dDistance = Math.Sqrt(
                (dTowerOne_Easting - dTowerTwo_Easting) * (dTowerOne_Easting - dTowerTwo_Easting) +
                (dTowerOne_Northing - dTowerTwo_Northing) * (dTowerOne_Northing - dTowerTwo_Northing)
            );

            // Return the distance between points in meters
            return dDistance;
        }

        // Generate Graph and assign frequencies
        public static UndirectedGraph<CellTower, TaggedEdge<CellTower, double>> CreateGraph(List<CellTower> lCellTowers, List<int> lFrequencies)
        {

            // Distance between towers
            double dDistance = 0;

            // Create an undirected graph that has vertices (cell towers) and edges (one double property for frequencies)
            UndirectedGraph<CellTower, TaggedEdge<CellTower, double>> graph_Cell_Towers = new UndirectedGraph<CellTower, TaggedEdge<CellTower, double>>();

            // Edge case (Only one tower)
            if (lCellTowers.Count < 2)
            {
                return graph_Cell_Towers;
            }

            // Add all cell tower objects to a QuickGraph undirectedgraph as vertices
            for (int i = 0; i < lCellTowers.Count; i++)
            {
                graph_Cell_Towers.AddVertex(lCellTowers[i]);
            }

            // Add undirected tagged edges between all nodes, while avoiding duplicates
            for (int iTowerOne = 0; iTowerOne < lCellTowers.Count; iTowerOne++)
            {
                for (int iTowerTwo = iTowerOne + 1; iTowerTwo < lCellTowers.Count; iTowerTwo++) // Used to avoid duplicate edges (undirected graph, edges directional)
                {
                    // Calculate the Euclidean distance between the two towers
                    dDistance = CalDistance(
                        lCellTowers[iTowerOne].Easting,
                        lCellTowers[iTowerOne].Northing,
                        lCellTowers[iTowerTwo].Easting,
                        lCellTowers[iTowerTwo].Northing
                    );

                    // Create a weighted edge between the two towers with the distance between the two as an edge property
                    graph_Cell_Towers.AddEdge(new TaggedEdge<CellTower, double>(lCellTowers[iTowerOne], lCellTowers[iTowerTwo], dDistance));
                }
            }

            // Return the created Cell Tower network
            return graph_Cell_Towers;

        }

        // Visualise the created graph using Graphiz (Redo)
        public static void VisualiseGraph(UndirectedGraph<CellTower, TaggedEdge<CellTower, double>> graph_Cell_Towers, string sFilePath)
        {
            // Create 
            var graphviz = new GraphvizAlgorithm<CellTower, TaggedEdge<CellTower, double>>(graph_Cell_Towers);

            // Customize Graph appearance
            graphviz.FormatVertex += (sender, args) =>
            {
                args.VertexFormat.Label = args.Vertex.Tower_Name;
            };

            graphviz.FormatEdge += (sender, args) =>
            {
                args.EdgeFormat.Label = new GraphvizEdgeLabel() {
                    Value = $" {args.Edge.Tag:F2} m"
                };
            };

            string dotFormat = graphviz.Generate();

            // Save the DOT file
            File.WriteAllText(sFilePath, dotFormat);

            // Output the DOT content to the console (optional)
            Console.WriteLine(dotFormat);

            // Inform the user
            Console.WriteLine($"DOT file saved to {sFilePath}");

        }

        [STAThread] // Needed to Open File Dialog
        static void Main(string[] args)
        {
            // Used to handel program continues excecution
            bool bContinue_Program = true;
            int iChoise = 0;

            // Stores users chosen txt file path
            string sFilePath = "";
            
            // Stores a list of all possible integer frequencies in a given range
            List<int> lFrequencies = new List<int>();

            // Stores a list of all cell tower information each in a cell tower object
            List<CellTower> lCell_Towers = new List<CellTower>();

            // Graph of cell tower network
            UndirectedGraph<CellTower, TaggedEdge<CellTower, double>> graph_Cell_Towers = null;


            while ( bContinue_Program )
            {
                Console.WriteLine("\nPlease enter a number (1: read file and calculate tower frequencies, 2: exit program)");

                if (int.TryParse(Console.ReadLine(), out iChoise)){

                    switch (iChoise)
                    {
                        case 1:

                            // Get file path
                            sFilePath = GetFilePath();

                            if(!(string.IsNullOrWhiteSpace(sFilePath)))
                            {
                                // Get list of frequencies
                                lFrequencies = GetFrequecyBound();

                                // Get list of Cell Tower infomration
                                lCell_Towers = GetCellTowers(sFilePath);

                                graph_Cell_Towers = CreateGraph(lCell_Towers, lFrequencies);

                                VisualiseGraph(graph_Cell_Towers, @"C:\Users\User\OneDrive\Company Technical Assessments\WIM\Tower_Frequencies\WIM_Technical_Assessment\Tower_Frequencies\graph.dot");
                            }

                            break;

                        case 2:
                            Console.WriteLine("Thank you, have a greate day!");
                            bContinue_Program = false;
                            break;

                        default:
                            continue;
                    }

                }
            }

        }
    }

}