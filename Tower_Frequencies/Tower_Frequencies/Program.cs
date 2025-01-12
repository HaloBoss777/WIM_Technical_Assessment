using System;
using Tower_Frequencies.Classes;
using Ookii.Dialogs.WinForms;
using System.Windows.Forms;
using Microsoft.Msagl;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System.Security.Cryptography;
using System.Windows.Markup;

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

                    if (int.TryParse(Console.ReadLine(), out frequency_upper) && frequency_upper > frequency_lower)
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

        // Calculate the great-circle distance distance between Towers (Large using longitude(Y) and latitude(X)) (Using Haversine Formula for considering Earth's curvature)
        // Based on info from https://stackoverflow.com/questions/41621957/a-more-efficient-haversine-function and https://www.movable-type.co.uk/scripts/latlong.html
        public static double CalDistance(double dTowerOne_longitude, double dTowerOne_latitude, double dTowerTwo_longitude, double dTowerTwo_latitude)
        {
            // Final great-circle distance result in meters
            double dDistance = 0;

            // Variables used to Compute Haversine Formula
            double dHalf_Chord_Length_Squared = 0;
            double dAngular_Distance = 0;

            // Earths Radius
            const double dEarth_Radius = 6378100; // Meters

            // Convert given longitude and latitude degrees to radians (Standard unit of angular measurement --> degree * (pi / 180) )
            dTowerOne_longitude = dTowerOne_longitude * (Math.PI / 180);
            dTowerOne_latitude = dTowerOne_latitude * (Math.PI / 180);
            dTowerTwo_longitude = dTowerTwo_longitude * (Math.PI / 180);
            dTowerTwo_latitude = dTowerTwo_latitude * (Math.PI / 180);

            //Sine of half the differences between longitude and latitudes --> Sin(ΔRadian / 2)
            double shdLatitude = Math.Sin((dTowerTwo_latitude - dTowerOne_latitude) / 2); 
            double shdLongitude = Math.Sin((dTowerTwo_longitude - dTowerOne_longitude) / 2);

            // Calculated the half chord lenght squared --> sin²(Δlat / 2) + cos(lat1) * cos(lat2) * sin²(Δlon / 2)
            dHalf_Chord_Length_Squared = shdLatitude * shdLatitude + Math.Cos(dTowerOne_latitude) * Math.Cos(dTowerTwo_latitude) * shdLongitude * shdLongitude;

            // Calculate the angular distance between two points in radians --> 2 * atan2(√a, √(1-a))
            dAngular_Distance = 2 * Math.Atan2(Math.Sqrt(dHalf_Chord_Length_Squared), Math.Sqrt(1 - dHalf_Chord_Length_Squared));

            // Finaly Calculate the great-circle distance in meters --> earth_radius * angular distance
            dDistance = dEarth_Radius * dAngular_Distance;


            // Return the distance between points in meters
            return dDistance;
        }

        // Generate Graph and assign frequencies
        public static Graph CreateGraph(List<CellTower> lCellTowers, List<int> lFrequencies)
        {

            // Distance between towers
            double dDistance = 0;

            // Create an undirected graph that has vertices (cell towers) and edges (one double property for frequencies)
            Graph graph_Cell_Towers = new Graph("Cell_Tower_Network");

            // Edge and Nodes used for Customisation
            Node nNode = null;
            Edge eEdge = null;
            Microsoft.Msagl.Core.Layout.Node nGeoNode = null; // Used to assign a nodes position using using latitude and longitude
            Microsoft.Msagl.Core.Geometry.Point pPointer = new Microsoft.Msagl.Core.Geometry.Point();

            // Edge case (Only one tower)
            if (lCellTowers.Count < 2)
            {
                return graph_Cell_Towers;
            }

            // Add all cell tower objects to a Msagl graph as vertices
            for (int i = 0; i < lCellTowers.Count; i++)
            {
                nNode = graph_Cell_Towers.AddNode(lCellTowers[i].Tower_Name);
                nNode.UserData = lCellTowers[i];
                nNode.LabelText = lCellTowers[i].Tower_Name;
                nNode.Attr.Shape = Shape.Circle;
                nNode.Attr.FillColor = Color.LightCyan;
                nNode.Attr.Color = Color.Black;
            }

            //Console.WriteLine("\nTower Distances:\n");

            // Add undirected edges between all towers, while avoiding duplicates
            for (int iTowerOne = 0; iTowerOne < lCellTowers.Count; iTowerOne++)
            {
                for (int iTowerTwo = iTowerOne + 1; iTowerTwo < lCellTowers.Count; iTowerTwo++) // Used to avoid duplicate edges (undirected graph, edges directional)
                {
                    // Calculate the great-circle distance between the two towers
                    dDistance = CalDistance(
                        lCellTowers[iTowerOne].Longitude,
                        lCellTowers[iTowerOne].Latitude,
                        lCellTowers[iTowerTwo].Longitude,
                        lCellTowers[iTowerTwo].Latitude
                    );

                    // Create a weighted edge between the two towers with the distance between the two as an edge property
                    eEdge = graph_Cell_Towers.AddEdge(lCellTowers[iTowerOne].Tower_Name, lCellTowers[iTowerTwo].Tower_Name);
                    eEdge.LabelText = dDistance.ToString("F2") + " m";
                    eEdge.Attr.Weight = (int) dDistance * 1000; // Convert to millimeter as weight cannot accept a double
                    //Console.WriteLine($"Tower Distance: {lCellTowers[iTowerOne].Tower_Name} and {lCellTowers[iTowerTwo].Tower_Name} has a distance {dDistance}");


                }
            }

            // Return the created Cell Tower network
            Console.WriteLine("\nGraph Created!!\n");
            return graph_Cell_Towers;

        }

        // Visualise the created graph using Msagl.Drawing
        public static void VisualiseGraph(Graph graph_Cell_Towers)
        {
            // Used to render graph
            GViewer gViewer = new GViewer();

            // Assign the viewer a graph
            gViewer.Graph = graph_Cell_Towers;

            // Create the form to host the GVierwer
            Form fGraph_Render = new Form();

            // Ensure gViewer Control fills the form and graph visuals scales with the form
            gViewer.Dock = DockStyle.Fill;

            // Adds Viewer control to form
            fGraph_Render.Controls.Add(gViewer);

            // Form customisation
            fGraph_Render.Text = "Cell Tower Network"; // Name of form
            fGraph_Render.WindowState = FormWindowState.Maximized; // Maximises the form for better viewing

            // Show the form
            Application.Run(fGraph_Render);
            
        }

        // Calculate the frequencies of each tower
        public static Graph CalFrequencies(Graph graph_Cell_Towers, List<int> lFrequencies, int iFrequency_Consideration)
        {
            // Used as a copy of lFrequencies to determine available frequencies for the target tower
            List<int> lAvailable_Frequencies = new List<int>();
            // Used frequency of a tower
            int iUsed_Frequency = 0;

            // For each CellTower node in the graph (in Alphabetical Order)
            foreach (var Tower in graph_Cell_Towers.Nodes.OrderBy(node => (node.UserData as CellTower).Tower_Name))
            {

                // Retrive all edges of the given target tower
                var All_Tower_Edges = graph_Cell_Towers.Edges.Where(edge => edge.SourceNode == Tower || edge.TargetNode == Tower);

                // Sort all edges in desending order according to their distance from the target tower and take the closest n towers
                All_Tower_Edges = All_Tower_Edges.OrderBy(edge => edge.Attr.Weight).Take(iFrequency_Consideration);

                // Make a copy of all available frequencies
                lAvailable_Frequencies = new List<int>(lFrequencies);

                // Check all frequencies used by closest n towers to target tower
                foreach ( var Edge in All_Tower_Edges )
                {
                    if(Tower == Edge.SourceNode)
                    {
                        iUsed_Frequency = ((Edge.TargetNode.UserData) as CellTower).Frequency;
                        lAvailable_Frequencies.Remove(iUsed_Frequency);
                    }
                    else
                    {
                        iUsed_Frequency = ((Edge.SourceNode.UserData) as CellTower).Frequency;
                        lAvailable_Frequencies.Remove(iUsed_Frequency);
                    }
                    
                }

                // If not all frequencies are used assign the first available frequency to the target tower
                if(lAvailable_Frequencies.Count != 0)
                {
                    // Assign a tower the first available frequency
                    (Tower.UserData as CellTower).Frequency = lAvailable_Frequencies.First();
                }
                else
                {
                    // If no frequencies are available indecate the tower that has the problem
                    Console.WriteLine($"No available frequency for Tower: {Tower.UserData as CellTower}");
                }


            }

            // Return the graph with each tower now owning a frequency
            return graph_Cell_Towers;
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
            Graph graph_Cell_Towers = null;


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

                                // Create the inital graph with and calculate distance between towers
                                graph_Cell_Towers = CreateGraph(lCell_Towers, lFrequencies);

                                // Assigne each tower a frquency of the chosen range
                                graph_Cell_Towers = CalFrequencies(graph_Cell_Towers, lFrequencies, 5);

                                // List each tower and their new frequency
                                foreach (var tower in graph_Cell_Towers.Nodes.OrderBy(node => (node.UserData as CellTower).Tower_Name))
                                {
                                    Console.WriteLine((tower.UserData as CellTower).ToString());
                                }


                                VisualiseGraph(graph_Cell_Towers);
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