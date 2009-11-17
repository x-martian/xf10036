using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
namespace ExpenseIt
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }
        private void viewButton_Click(object sender, RoutedEventArgs args)
        {
            // View Expense Report
            ExpenseReportPage expenseReportPage = new ExpenseReportPage();
            this.NavigationService.Navigate(expenseReportPage);
        }
    }
}
