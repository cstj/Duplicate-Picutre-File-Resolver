/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:DuplicateFinder.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using DuplicateFinderLib;
using Microsoft.Practices.ServiceLocation;

namespace DuplicateFinder
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        System.Windows.Threading.Dispatcher dispatcher;
        public ViewModelLocator()
        {
            dispatcher = System.Windows.Application.Current.Dispatcher;
        }


        private Main _Main;
        /// <summary>
        /// Gets the Main property.
        /// </summary>
        public Main Main
        {
            get
            {
                if (_Main == null) _Main = new Main(dispatcher);
                return _Main;
            }
        }
    }
}