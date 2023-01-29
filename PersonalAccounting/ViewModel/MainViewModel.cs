using GalaSoft.MvvmLight.Command;
using PersonalAccounting.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace PersonalAccounting.ViewModel
{
    internal class MainViewModel : ViewModelBase
    {
        //private Page day = new DayExpenses();
        //private Page month = new MonthExpenses();
        //private Page period= new PeriodExpenses();
        //private Page settingsPage = new SettingsPage();

        private Page currentPage = null;

        public Page CurrentPage
        {
            get => currentPage;
            set => Set(ref currentPage, value);
        }

        public ICommand OpenDayPage
        {
            get
            {
                return new RelayCommand(() => CurrentPage = new DayExpenses());
            }
        }
        public ICommand OpenMonthPage
        {
            get
            {
                return new RelayCommand(() => CurrentPage = new MonthExpenses());
            }
        }
        public ICommand OpenPeriodPage
        {
            get
            {
                return new RelayCommand(() => CurrentPage = new PeriodExpenses());
            }
        }
        public ICommand OpenSettingsPage
        {
            get
            {
                return new RelayCommand(() => CurrentPage = new SettingsPage());
            }
        }
    }
}
