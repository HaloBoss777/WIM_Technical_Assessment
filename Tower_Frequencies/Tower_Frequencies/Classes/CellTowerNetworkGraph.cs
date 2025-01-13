using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tower_Frequencies.Classes
{
    public class CellTowerNetworkGraph
    {

        // Generate Graph and assign frequencies
        public static Graph CreateGraph(List<CellTower> lCellTowers, List<int> lFrequencies)
        {

            // Distance between towers
            double dDistance = 0;

            // User feedback
            Console.WriteLine("\n" + new string('-', 100));
            Console.WriteLine("\nCreating Graph.........");

            // Create an undirected graph that has vertices (cell towers) and edges (one double property for frequencies)
            Graph graph_Cell_Towers = new Graph("Cell_Tower_Network");

            // Edge and Nodes used for Customisation
            Node nNode = null;
            Edge eEdge = null;

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

            // Add undirected edges between all towers, while avoiding duplicates
            for (int iTowerOne = 0; iTowerOne < lCellTowers.Count; iTowerOne++)
            {
                for (int iTowerTwo = iTowerOne + 1; iTowerTwo < lCellTowers.Count; iTowerTwo++) // Used to avoid duplicate edges (undirected graph, edges directional)
                {
                    // Calculate the great-circle distance between the two towers
                    dDistance = DistanceCalculation.CalDistance(
                        lCellTowers[iTowerOne].Longitude,
                        lCellTowers[iTowerOne].Latitude,
                        lCellTowers[iTowerTwo].Longitude,
                        lCellTowers[iTowerTwo].Latitude
                    );

                    // Create a weighted edge between the two towers with the distance between the two as an edge property
                    eEdge = graph_Cell_Towers.AddEdge(lCellTowers[iTowerOne].Tower_Name, lCellTowers[iTowerTwo].Tower_Name);
                    eEdge.LabelText = dDistance.ToString("F2") + " m";
                    eEdge.Attr.Weight = (int)(dDistance * 1000.00); // Convert to millimeter as weight cannot accept a double
                }
            }

            // Return the created Cell Tower network
            Console.WriteLine("\nGraph Created!!\n");
            return graph_Cell_Towers;

        }

        // Creates a matrix of all tower distances
        public static void DistanceMatrix(Graph graph_Cell_Towers)
        {
            // Source and Targe indexes for populating matrix
            int iSource_Index = 0;
            int iTarget_Index = 0;

            // Stores the distance between source and target
            double dDistance = 0;

            // Get all nodes in the graph (cell towers)
            List<Node> lNodes = new List<Node>(graph_Cell_Towers.Nodes.OrderBy(node => (node.UserData as CellTower).Tower_Name));

            // Create the inital matrix
            int iNode_Count = lNodes.Count;
            double[,] mxDistance = new double[iNode_Count, iNode_Count]; // exmple a for 19 towers makes a 19 x 19 matrix

            // Populate the matrix with inital 0 distances
            for (int i = 0; i < iNode_Count; i++)
            {
                for (int j = 0; j < iNode_Count; j++)
                {
                    mxDistance[i, j] = 0;
                }
            }

            // Populate the matix with distance metrix
            foreach (Edge eEdge in graph_Cell_Towers.Edges)
            {
                // Gather indexes of source and target to use in matrix
                iSource_Index = lNodes.IndexOf(eEdge.SourceNode);
                iTarget_Index = lNodes.IndexOf(eEdge.TargetNode);

                // Get the distance between source and target towers
                dDistance = (eEdge.Attr.Weight / 1000.0); // Convent to meters from millimeters

                // Populate their positions in matrix (both row, column and column, row due to it being a undirected graph)
                mxDistance[iSource_Index, iTarget_Index] = dDistance;
                mxDistance[iTarget_Index, iSource_Index] = dDistance;
            }

            // Display top lables
            Console.WriteLine("\nDistance Matrix of Cell Tower Network (meters):");



            // Display the lables of each node
            Console.Write("\t");
            for (int i = 0; i < iNode_Count; i++)
            {
                Console.Write((lNodes[i].UserData as CellTower).Tower_Name + "\t");
            }

            // Close top line
            Console.WriteLine();

            // Print matrix with first row as lables
            for (int i = 0; i < iNode_Count; i++)
            {
                Console.Write((lNodes[i].UserData as CellTower).Tower_Name + "\t");

                for (int j = 0;j < iNode_Count; j++)
                {
                    Console.Write(mxDistance[i, j].ToString("F2") + "\t");
                }

                // Close line
                Console.WriteLine();

            }

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

            //Used for Information display
            bool bErrors = false;

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
                foreach (var Edge in All_Tower_Edges)
                {
                    if (Tower == Edge.SourceNode)
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
                if (lAvailable_Frequencies.Count != 0)
                {
                    // Assign a tower the first available frequency
                    (Tower.UserData as CellTower).Frequency = lAvailable_Frequencies.First();
                }
                else
                {
                    if (bErrors == false)
                    {
                        bErrors = true;
                        Console.WriteLine("\n" + new string('-', 100));
                        Console.WriteLine("\nFrequency Assignment Warnings:\n");
                    }

                    // If no frequencies are available indecate the tower that has the problem
                    Console.WriteLine($"No available frequency for {Tower.UserData as CellTower}");
                }


            }

            // Return the graph with each tower now owning a frequency
            return graph_Cell_Towers;
        }
    }
}
