using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using LAB.Model;
using System;

namespace LAB.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SidePanelViewModel : ViewModelBase
    {
        // Model Definitions

        Process process;
        Ingredients ingredients;
        General recipe;

        // Command Definitions
        public RelayCommand RecipeExpandCommand { get; private set;}
        public RelayCommand ProcessExpandCommand { get; private set; }
        public RelayCommand IngredientsExpandCommand { get; private set; }

        #region Bindable Properties

        // Property Names
        public const string IngredientsSubMenuVisibilityPropertyName = "IngredientsSubMenuVisibility";
        public const string RecipeSubMenuVisibilityPropertyName = "RecipeSubMenuVisibility";
        public const string ProcessSubMenuVisibilityPropertyName = "ProcessSubMenuVisibility";
        public const string IngredientsArrowRotationPropertyName = "IngredientsArrowRotation";
        public const string RecipeArrowRotationPropertyName = "RecipeArrowRotation";
        public const string ProcessArrowRotationPropertyName = "ProcessArrowRotation";
        public const string MaltsPropertyName = "Malts";
        public const string HopsPropertyName = "Hops";
        public const string YeastPropertyName = "Yeast";
        public const string RecipePropertyName = "Recipe";

        // Local properties
        private bool recipeSubMenuVisible;
        private bool processSubMenuVisible;
        private bool ingredientsSubMenuVisible;

        // Visibility Properties
        public string RecipeSubMenuVisibility
        {
            get
            {
                if(recipeSubMenuVisible) { return "Visible"; }
                return "Collapsed";
            }
        }

        public string ProcessSubMenuVisibility
        {
            get
            {
                if(processSubMenuVisible) { return "Visible"; }
                return "Collapsed";
            }
        }

        public string IngredientsSubMenuVisibility
        {
            get
            {
                if (ingredientsSubMenuVisible) { return "Visible"; }
                return "Collapsed";
            }
        }

        // Arrow Rotation Properties
        public double IngredientsArrowRotation
        {
            get
            {
                if(ingredientsSubMenuVisible) { return 90; }
                return 0;
            }
        }

        public double RecipeArrowRotation
        {
            get
            {
                if(recipeSubMenuVisible) { return 90;}
                return 0;
            }
        }

        public double ProcessArrowRotation
        {
            get
            {
                if(processSubMenuVisible) { return 90; }
                return 0;
            }
        }

        // Ingredients properties
        public List<Ingredients.Malt> Malts
        {
            get{ return ingredients.Malts; }
        }

        public List<Ingredients.Hop> Hops
        {
            get { return ingredients.Hops; }
        }

        public Ingredients.yeast Yeast
        {
            get { return ingredients.Yeast; }
        }

        // Recipe Properties
        public General Recipe
        {
            get { return recipe; }
        }
        
        #endregion

        #region Constructor

        public SidePanelViewModel()
        {
            // List Initialisation
            recipe = new General();
            process = new Process();
            ingredients = new Ingredients();
            ingredients.Yeast.Name = "N/A";
            ingredients.Yeast.Type = "N/A";
            ingredients.Yeast.Form = "N/A";

            // Definition of Command Instances
            RecipeExpandCommand = new RelayCommand(recipeExpandCommand);
            ProcessExpandCommand = new RelayCommand(processExpandCommand);
            IngredientsExpandCommand = new RelayCommand(ingredientsExpandCommand);

            // Registration to messages
            Messenger.Default.Register<Process>(this, Process_MessageReceived);
            Messenger.Default.Register<Ingredients>(this, Ingredients_MessageReceived);
            Messenger.Default.Register<General>(this, Recipe_MessageReceived);
        }

        #endregion

        #region Commands

        private void ingredientsExpandCommand()
        {
            // Set process submenu visibility
            ingredientsSubMenuVisible = !ingredientsSubMenuVisible;
            RaisePropertyChanged(IngredientsSubMenuVisibilityPropertyName);

            // Set rotation transform of arrow icon
            RaisePropertyChanged(IngredientsArrowRotationPropertyName);
        }

        private void processExpandCommand()
        {
            // Set process submenu visibility
            processSubMenuVisible = !processSubMenuVisible;
            RaisePropertyChanged(ProcessSubMenuVisibilityPropertyName);

            // Set rotation transform of arrow icon
            RaisePropertyChanged(ProcessArrowRotationPropertyName);
        }

        private void recipeExpandCommand()
        {
            // Set recipe submenu visility
            recipeSubMenuVisible = !recipeSubMenuVisible;
            RaisePropertyChanged(RecipeSubMenuVisibilityPropertyName);

            // Set rotation transform of arrow icon
            RaisePropertyChanged(RecipeArrowRotationPropertyName);
        }

        #endregion

        #region Message Received Methods

        private void Process_MessageReceived(Process _process)
        {
            // Assign received instance to local process instance
            process = _process;

            // Raise concerned properties
        }
        
        private void Ingredients_MessageReceived(Ingredients _ingredients)
        {
            // Assign received instance to local ingredients instance
            ingredients = _ingredients;
            
            // Format the data for display purposes
            // Malt Formating
            foreach(Ingredients.Malt malt in ingredients.Malts)
            {
                malt.Quantity = Math.Round(malt.Quantity, 2);
                malt.Quantity.ToString();
            }
            
            // Hop Formating
            foreach(Ingredients.Hop hop in ingredients.Hops)
            {
                hop.Quantity = Math.Round(hop.Quantity, 2);
                hop.Quantity.ToString();
            }

            // Yeast Formating

            ingredients.Yeast.MinTemp = Math.Round(ingredients.Yeast.MinTemp, 2);
            ingredients.Yeast.MaxTemp = Math.Round(ingredients.Yeast.MaxTemp, 2);

            ingredients.Yeast.MinTemp.ToString();
            ingredients.Yeast.MaxTemp.ToString();

            // Raise concerned properties
            RaisePropertyChanged(MaltsPropertyName);
            RaisePropertyChanged(HopsPropertyName);
            RaisePropertyChanged(YeastPropertyName);
        }

        private void Recipe_MessageReceived(General _recipe)
        {
            recipe = _recipe;
            RaisePropertyChanged(RecipePropertyName);
        }

        #endregion

    }
}