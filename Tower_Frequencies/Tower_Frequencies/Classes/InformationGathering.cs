using Microsoft.Msagl.Drawing;
using Ookii.Dialogs.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tower_Frequencies.Classes
{
    public class InformationGathering
    {
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


        // Get all cell tower information
        public static List<CellTower> GetCellTowers(string sFilePath)
        {

            // User feedback
            Console.WriteLine("\n" + new string('-', 100));
            
            // Error handling
            bool bError_Occured = false;

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

                // User feedback
                Console.WriteLine("\nLoading file contents............");

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
                bError_Occured = true;
                return lReturn;
            }
            catch (FormatException ex)
            {
                //Handels case when type conversions fail
                Console.WriteLine("\nInvalid Types for Tower info:");
                bError_Occured = true;
                return lReturn;
            }
            catch (Exception ex)
            {
                // When any other error message occurs
                Console.WriteLine("A problem occured while reading the file.");
                bError_Occured = true;
                return lReturn;
            }
            finally
            {
                //Closes Reader after its Porpuse is complte
                if (sr != null)
                {
                    sr.Close();
                }

                if(bError_Occured == false)
                {
                    // Indecate that everything worked :D (Legendary Rarity)
                    Console.WriteLine("\nThe File has Loaded");
                }

            }

            return lReturn;
        }


        // Allow the User to chose frequency range for cell towers
        public static List<int> GetFrequecyBound(List<CellTower> lCell_Towers)
        {
            // Section getting frequencies
            Console.WriteLine("\n" + new string('-', 100));

            // Inclusive bounds for the frequencies
            int frequency_lower = 0;
            int frequency_upper = 0;

            // The ratio of towers to available frequencies
            double dRatio = 0;
            double dOptimal_Ratio = 6.0 / 19.0; // Original problem optimal ratio calculation

            // Used to test input
            bool bValid_bound = false;
            bool bLow_Ratio = false;

            // User input on frequency ratio
            int iUser_Input = 0;

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
                        for (int frequency = frequency_lower; frequency <= frequency_upper; frequency++)
                        {
                            lFrequencies.Add(frequency);
                        }

                    }
                    else
                    {
                        Console.WriteLine("\nInvalid Upper Bound Frequency. Try again.");
                        bValid_bound = false;
                    }

                }
                else
                {
                    Console.WriteLine("\nInvalid Lower Bound Frequency. Try again.");
                    bValid_bound = false;
                }


                // Calculate the frequencies per tower (0.316 was calculated to be the optimal ratio given the original 19 towers and 6 frequencies)
                dRatio = (double)lFrequencies.Count / (double)lCell_Towers.Count;

                if (dRatio < dOptimal_Ratio)
                {
                    // To enable while loop usage incase of user mind change
                    bLow_Ratio = true;

                    while (bLow_Ratio)
                    {
                        // Warn user of low frequency per tower ratio
                        Console.WriteLine($"\nWarning the amout of frequencies per tower ({dRatio.ToString("F2")} < 0.316) is low!!");
                        Console.WriteLine("This can cause unintended behaviour or inadecate frequencies for some towers.");
                        Console.WriteLine($"\nDo you whish to continue with this frequency range? (1: Yes, 2: No)");

                        // Determine if user is ok with low ratio
                        if (int.TryParse(Console.ReadLine(), out iUser_Input))
                        {
                            switch (iUser_Input)
                            {
                                case 1:
                                    bLow_Ratio = false;
                                    bValid_bound = true;
                                    break;

                                case 2:
                                    // Reset old list
                                    lFrequencies = new List<int>();
                                    bLow_Ratio = false;
                                    bValid_bound = false;
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nInvalid input try again.\n");
                        }
                    }
                    
                }

            }

            // Return list of all frequencies in a range
            return lFrequencies;
        }

    }


}
