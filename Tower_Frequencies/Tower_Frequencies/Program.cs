using System;
using Tower_Frequencies.Classes;
using Ookii.Dialogs.WinForms;
using System.Windows.Forms;


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

        public static List<CellTower> GetCellTowers(string filePath)
        {

            List<CellTower> return_List = new List<CellTower>();

            // Handels case where file string is null or full of white spaces
            if (string.IsNullOrWhiteSpace(filePath))
            {
                Console.WriteLine("Invalid File Path. Cannot be null or Empty");
                return return_List;
            }

            // Decalre Stream Reader to allow close in final block
            StreamReader sr = null;

            try
            {
                // Create Stream Readed Object to read in txt file with tower info
                sr = new StreamReader(filePath);

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
                    return_List.Add(cellTower);

                }

            }
            catch (FileNotFoundException ex)
            {
                //Handels case when file is not found/chosen
                Console.WriteLine("File not found/Chosen:");
                Console.WriteLine(ex.Message + "\n");
            }
            catch (FormatException ex)
            {
                //Handels case when type conversions fail
                Console.WriteLine("Invalid Types for Tower info:");
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
                Console.WriteLine("File Loaded \n");
            }

            return return_List;
        }

        // Allow User to Choose file path
        public static string GetFilePath()
        {
            // Viarable to Store filepath
            string filePath = "";

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
                filePath = file_Dialog.FileName;
            }
            else
            {
                Console.WriteLine("No file selected.");
            }
           


            return filePath;
        }

        [STAThread] // Needed to Open File Dialog
        static void Main(string[] args)
        {
            string filePath = "";

            filePath = GetFilePath();

            List<CellTower> towers = GetCellTowers(filePath);

            foreach (var tower in towers)
            {
                Console.WriteLine(tower.ToString());
            }
        }
    }

}