using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Xml.Linq;

namespace LAB.Model
{
    class RecipeSetup
    {
        // Model instances
        Process process;
        Ingredients ingredients;
        General Recipe;

        #region Constructor

        public RecipeSetup()
        {
            // Create model instances
            process = new Process();
            ingredients = new Ingredients();
            Recipe = new General();
        }

        #endregion

        #region Xml Methods

        // Get the recipe xml file
        public void Load()
        {
            // Show file dialog
            Microsoft.Win32.OpenFileDialog LoadRecipeDialog = new Microsoft.Win32.OpenFileDialog();
            LoadRecipeDialog.DefaultExt = ".xml";
            Nullable<bool> result = LoadRecipeDialog.ShowDialog();

            if (result == true)
            {
                // Open document 
                string filename = LoadRecipeDialog.FileName;
                string ext = System.IO.Path.GetExtension(filename);

                // Check if file is of type xml
                if (ext != ".xml")
                {
                    MessageBox.Show("Invalid file type selected. \n Please open a xml recipe file");
                    LoadRecipeDialog.ShowDialog();
                }
                else
                {
                    ParseRecipeFile(filename);
                }
            }
        }

        private void ParseRecipeFile(string RecipePath)
        {
            XDocument xml = XDocument.Load(RecipePath);

            // --------------------------------------------- General ------------------------------------------------------

            // Parse General Recipe Properties
            Recipe.Name = xml.Descendants("RECIPE").Elements("NAME").Single().Value;
            Recipe.Brewer = xml.Descendants("RECIPE").Elements("BREWER").Single().Value;
            Recipe.Type = xml.Descendants("RECIPE").Elements("TYPE").Single().Value;
            Recipe.Category = xml.Descendants("STYLE").Elements("CATEGORY").Single().Value;
            Recipe.BatchSize = (double)xml.Descendants("RECIPE").Elements("BATCH_SIZE").Single();

            // -------------------------------------------- Ingredients  ---------------------------------------------------

            // Get Ingredients : Malts

            foreach (var node in xml.Descendants("FERMENTABLE"))
            {
                if (node.Element("TYPE").Value == "Grain")
                {
                    ingredients.Malts.Add(new Ingredients.Malt() { Name = node.Element("NAME").Value, Quantity = (double)node.Element("AMOUNT"), SRM = node.Element("DISPLAY_COLOR").Value });
                }
            }

            // Get Ingredients : Hops

            foreach (var node in xml.Descendants("HOP"))
            {
                ingredients.Hops.Add(new Ingredients.Hop() { Name = node.Element("NAME").Value, Quantity = (double)node.Element("AMOUNT") * 1000, BoilTime = (double)node.Element("TIME") });
            }

            // Get Ingredients : Yeast

            ingredients.Yeast.Name = xml.Descendants("YEAST").Elements("NAME").Single().Value;
            ingredients.Yeast.Type = xml.Descendants("YEAST").Elements("TYPE").Single().Value;
            ingredients.Yeast.Form = xml.Descendants("YEAST").Elements("FORM").Single().Value;
            ingredients.Yeast.MinTemp = (double)xml.Descendants("YEAST").Elements("MIN_TEMPERATURE").Single();
            ingredients.Yeast.MaxTemp = (double)xml.Descendants("YEAST").Elements("MAX_TEMPERATURE").Single();

            // ----------------------------------------------- Process Steps ---------------------------------

            // Check if the adjust volume to equipment is set to true
            string AdjustForEquipment = xml.Descendants("MASH").Elements("EQUIP_ADJUST").Single().Value;
            
            double MLTDeadspace;

            if (AdjustForEquipment == "TRUE")
            {
                MLTDeadspace = (double)xml.Descendants("EQUIPMENT").Elements("LAUTER_DEADSPACE").Single();
            }
            else
            {
                MLTDeadspace = 0;
            }

            // Get Process Step : MashSteps
            double _MLTDeadspace = MLTDeadspace;
            int i = 0; 
            foreach (var node in xml.Descendants("MASH_STEP"))
            {
                if (i != 0) { _MLTDeadspace = 0; }
                process.MashSteps.Add(new Process.MashStep() { Name = node.Element("NAME").Value, Volume = (double)node.Element("INFUSE_AMOUNT") + MLTDeadspace, Temp = (double)node.Element("STEP_TEMP"), Time = (double)node.Element("STEP_TIME") });
                i++;
            }

            // Get Process Step : Strike
            double GrainAmount = 0;

            foreach (var malt in ingredients.Malts)
            {
                GrainAmount = malt.Quantity + GrainAmount;
            }

            double GrainTemp = (double)xml.Descendants("MASH").Elements("GRAIN_TEMP").Single();
            double WaterGrainRatio = process.MashSteps[0].Volume / GrainAmount;
            double FirstMashStepTemp = process.MashSteps[0].Temp;

            process.Strike.Temp = (0.2 / WaterGrainRatio) * (FirstMashStepTemp - GrainTemp) + FirstMashStepTemp;

            // Get Process Step : Boil
            process.Boil.Time = (double)xml.Descendants("RECIPE").Elements("BOIL_TIME").Single();

            // ----------------------------------------- Water Info ----------------------------------------
            // Total Water Volume Needed Calculation
            double PreBoilVol = (double)xml.Descendants("RECIPE").Elements("BOIL_SIZE").Single();
            double TotalWaterNeeded = PreBoilVol + GrainAmount + MLTDeadspace;

            process.Boil.Volume = PreBoilVol;
            process.Session.TotalWaterNeeded = TotalWaterNeeded;

            // Sparge Volume Calculations
            // Pre-Boil Vol - Mash infusion amount (add all steps) + Absorption Amount (weight of grains in kg)
            double MashVol = 0;

            foreach (var step in process.MashSteps)
            {
                MashVol = step.Volume + MashVol;
            }

            double SpargeVol = PreBoilVol - MashVol + GrainAmount;
            process.Sparge.Volume = SpargeVol;

            // Get sparge temperature
            process.Sparge.Temp = (double)xml.Descendants("MASH").Elements("SPARGE_TEMP").Single();

            // --------------------------------------  Fermentation Info -------------------------------------
            // Get fermentation temp and time
            process.Fermentation.Temp = (double)xml.Descendants("AGE_TEMP").Single();
            process.Fermentation.Age = (double)xml.Descendants("AGE").Single();

            // Get SRM color value et set SRMColorDisplay Control on side menu
            //XElement RecipeNode = xml.Descendants("RECIPE")

            // Send new Recipe and Process info to main view model
            SendRecipeInfo();
        
        }

        #endregion

        #region Send Messages

        public void SendRecipeInfo()
        {
            Messenger.Default.Send<Ingredients>(ingredients);
            Messenger.Default.Send<Process>(process);
            Messenger.Default.Send<General>(Recipe);
        }

        #endregion
    }
}
