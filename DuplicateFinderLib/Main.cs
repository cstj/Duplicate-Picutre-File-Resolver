using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateFinderLib
{
    public class Main
    {
        private System.Windows.Threading.Dispatcher _dispatcher;

        public Main(System.Windows.Threading.Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        private ViewModel.MainViewModel _MainViewModel;
        public ViewModel.MainViewModel MainViewModel
        {
            get
            {
                if (_MainViewModel == null) _MainViewModel = new ViewModel.MainViewModel(_dispatcher);
                return _MainViewModel;
            }
        }
    }
}
