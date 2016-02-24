using TransBus_Rozkłady.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Xml.Linq;
using System.Threading.Tasks;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace TransBus_Rozkłady
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WidokRozkladu : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        DateTime dzien = DateTime.Now;
        List<rozklad> dane = new List<rozklad>();
        int numerdnia = (int) DateTime.Today.DayOfWeek;



        public WidokRozkladu()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            pivotAutoselect();
           
        }

        

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            string name = e.NavigationParameter as string;

            if (!string.IsNullOrWhiteSpace(name))
            {
                Pivotka.Title = name;
            }
            else
            {
                Pivotka.Title = "Error!";
            }
            


            
           
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }



        /*private async void loadData()
        {
            try
            {
                Ladowanie.IsActive = true;
                
                //GridTydzien.ItemsSource = dane.FindAll(oElement => oElement.dzien == "tydzien");

                //GridSobota.ItemsSource = dane.FindAll(oElement => oElement.dzien == "sobota");
                //GridNiedziela.ItemsSource = dane.FindAll(oElement => oElement.dzien == "niedziela");
                
            }
            catch
            {

            }
            finally
            {
                Ladowanie.IsActive = false;
            }
            
        }*/

        public void pivotAutoselect()
        {
            
            if (numerdnia == 0)
            {
                Pivotka.SelectedIndex = 2;
            }
            if (numerdnia == 6)
            {
                Pivotka.SelectedIndex = 1;
            }
            
        }
        public object fillData()
        {
            XDocument xml = new XDocument();
            if (Pivotka.Title.Equals("Niepołomice-Kraków"))
            {
                xml = XDocument.Load(@"Rozklady/niepolomice-krakow.xml");
            }
            if (Pivotka.Title.Equals("Niepołomice-Chobot"))
            {
                xml = XDocument.Load(@"Rozklady/niepolomice-chobot.xml");
            }
            if (Pivotka.Title.Equals("Kraków-Chobot"))
            {
                xml = XDocument.Load(@"Rozklady/krakow-chobot.xml");
            }
            if (Pivotka.Title.Equals("Chobot-Kraków"))
            {
                xml = XDocument.Load(@"Rozklady/chobot-krakow.xml");
            }
                      

            dane = (from q in xml.Elements("rozklad").Elements("godzina")
                    select new rozklad
                    {
                        dzien = (string)q.Attribute("dzien").Value,
                        godzina = (string)q.Value,
                        ograniczenia = (string)q.Attribute("ograniczenia").Value,

                    }
                    ).ToList();
            return dane;
        }


        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
           this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion



        private async void PnPtPivot_Loaded(object sender, RoutedEventArgs e)
        {
            GridTydzien.ItemsSource = dane.FindAll(oElement => oElement.dzien == "tydzien");
            
        }

        private async void SobotaPivot_Loaded(object sender, RoutedEventArgs e)
        {
            GridSobota.ItemsSource = dane.FindAll(oElement => oElement.dzien == "sobota");
            
        }

        private async void NiedzielaPivot_Loaded(object sender, RoutedEventArgs e)
        {
            GridNiedziela.ItemsSource = dane.FindAll(oElement => oElement.dzien == "niedziela");
            
        }

        private async void Pivotka_Loading(Pivot sender, PivotItemEventArgs args)
        {
            Ladowanie.IsActive = true;
        }

        private async void Pivotka_Loaded(Pivot sender, PivotItemEventArgs args)
        {
            fillData();
            Ladowanie.IsActive = false;
        }
    
    
    
    }
}
