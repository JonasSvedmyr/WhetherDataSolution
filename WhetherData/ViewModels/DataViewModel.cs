using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WhetherData.ViewModels
{
    public class DataViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region props
        public DateTime SelectedDate { get; set; } = DateTime.Parse("01/01/2016");
        public string SelectedLocation { get; set; }

        public List<string> LocationList { get; set; } = new List<string> { "Inne", "Ute" };

        private ObservableCollection<Log> _logs;

        public ObservableCollection<Log> Logs
        {
            get
            {
                return _logs;
            }
            set
            {
                _logs = value;
                NotifyPropertyChanged(nameof(Logs));
            }
        }
        #endregion

        #region Commands
        public ICommand SortByDateCommand { get; set; }
        public ICommand SortByTempretureCommand { get; set; }
        public ICommand SortByHumidityCommand { get; set; }
        public ICommand SortByMoldRiskCommand { get; set; }
        public ICommand SortByLocationCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand GetAllCommand { get; set; }
        public ICommand FirstDayOfWinterCommand { get; set; }
        public ICommand FirstDayOFallCommand { get; set; }
        public ICommand UploadCommand { get; set; }
        #endregion
        public DataViewModel()
        {
            //sets the Commands for all the buttons
            SortByDateCommand = new RelayCommand(SortByDate);
            SortByTempretureCommand = new RelayCommand(SortByTempreture);
            SortByHumidityCommand = new RelayCommand(SortByHumidity);
            SortByMoldRiskCommand = new RelayCommand(SortByMoldRisk);
            SortByLocationCommand = new RelayCommand(SortByLocation);
            GetAllCommand = new RelayCommand(GetAll);
            SearchCommand = new RelayCommand(SearchForDate);
            FirstDayOfWinterCommand = new RelayCommand(MeteorologicalWinter);
            FirstDayOFallCommand = new RelayCommand(MeteorologicalFall);
            UploadCommand = new RelayCommand(UploadFile);

            GetAll();
        }

        #region Sorting
        public void SortByDate()
        {
            Logs = Logs.OrderBy(x => x.Time).ToObservableCollection();
        }
        public void SortByTempreture()
        {
            Logs = Logs.OrderByDescending(x => x.Tempreture).ToObservableCollection();
        }
        public void SortByHumidity()
        {
            Logs = Logs.OrderBy(x => x.Humidity).ToObservableCollection();
        }
        public void SortByLocation()
        {
            Logs = Logs.OrderBy(x => x.Location).ToObservableCollection();
        }
        public void SortByMoldRisk()
        {
            Logs = Logs.OrderBy(x => x.MoldRisk).ToObservableCollection();
        }
        #endregion

        /// <summary>
        /// Gets all data from the database
        /// </summary>
        public void GetAll()
        {
            using (EFContext context = new EFContext())
            {
                var quary = context.logs
                    .GroupBy(x => new { x.Location, x.Time.Year, x.Time.Month, x.Time.Day })
                    .OrderByDescending(x => x.Average(x => x.Tempreture))
                    .Select(g => new Log
                    {
                        Humidity = g.Average(x => x.Humidity),
                        Tempreture = g.Average(x => x.Tempreture),
                        Location = g.Key.Location,
                        Time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                        MoldRisk = (g.Average(x => x.Humidity) - 78) * (g.Average(x => x.Tempreture) / 15) / 0.22
                    });
                Logs = CheckMoldRisk(quary);
            }
        }

        /// <summary>
        /// Gets a specified day from the data base
        /// </summary>
        public void SearchForDate()
        {
            using (EFContext context = new EFContext())
            {
                var quary = context.logs
                    .Where(x => x.Time.Year == SelectedDate.Year && x.Time.Month == SelectedDate.Month && x.Time.Day == SelectedDate.Day && x.Location == SelectedLocation)
                    .GroupBy(x => new { x.Location, x.Time.Year, x.Time.Month, x.Time.Day })
                    .OrderBy(x => x.Key.Year)
                    .ThenBy(x => x.Key.Month)
                    .ThenBy(x => x.Key.Day)
                    .Select(g => new Log 
                    { 
                        Humidity = g.Average(x => x.Humidity),
                        Tempreture = g.Average(x => x.Tempreture),
                        Location = g.Key.Location,
                        Time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                        MoldRisk = (g.Average(x => x.Humidity) - 78) * (g.Average(x => x.Tempreture) / 15) / 0.22
                    });
                Logs = CheckMoldRisk(quary); ;
            }
        }

        /// <summary>
        /// Gets the first day of winter
        /// </summary>
        public void MeteorologicalWinter()
        {
            //first day of winter is the first day of 5 days in a row where the tempreture is or below 0 degrees
            using (EFContext context = new EFContext())
            {
                //gets every day where the avarage tempreture is or below 0 degrees outside
                var quary = context.logs
                    .GroupBy(x => new { x.Location, x.Time.Year, x.Time.Month, x.Time.Day })
                    .Where(x => x.Average(x => x.Tempreture) <= 0 && x.Key.Location == "Ute")
                    .OrderBy(x => x.Key.Year)
                    .ThenBy(x => x.Key.Month)
                    .ThenBy(x => x.Key.Day)
                    .Select(g => new Log 
                    { 
                        Humidity = g.Average(x => x.Humidity), 
                        Tempreture = g.Average(x => x.Tempreture),
                        Location = g.Key.Location,
                        Time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                        MoldRisk = (g.Average(x => x.Humidity) - 78) * (g.Average(x => x.Tempreture) / 15) / 0.22 
                    });

                GetFirstOfFiveDays(CheckMoldRisk(quary));
            }
        }

        /// <summary>
        /// Gets the first day of fall
        /// </summary>
        public void MeteorologicalFall()
        {
            //first day of fall is the first day of 5 days in a row where the tempreture is or below 10 degrees and above 0 degrees 
            using (EFContext context = new EFContext())
            {
                //gets every day where the avarage tempreture is or below 10 degrees and above 0 outside
                var quary = context.logs
                    .GroupBy(x => new { x.Location, x.Time.Year, x.Time.Month, x.Time.Day })
                    .Where(x => x.Average(x => x.Tempreture) <= 10 && x.Average(x => x.Tempreture) > 0 && x.Key.Location == "Ute")
                    .OrderBy(x => x.Key.Year)
                    .ThenBy(x => x.Key.Month)
                    .ThenBy(x => x.Key.Day)
                    .Select(g => new Log 
                    { 
                        Humidity = g.Average(x => x.Humidity),
                        Tempreture = g.Average(x => x.Tempreture),
                        Location = g.Key.Location, Time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                        MoldRisk = (g.Average(x => x.Humidity) - 78) * (g.Average(x => x.Tempreture) / 15) / 0.22
                    });
                GetFirstOfFiveDays(CheckMoldRisk(quary));
            }
        }
        
        /// <summary>
        /// Finds the first 5 days in a row and shows it to the user
        /// </summary>
        /// <param name="quary"></param>
        private void GetFirstOfFiveDays(ObservableCollection<Log> quary)
        {
            DateTime date = quary.First().Time;
            ObservableCollection<Log> temp = new ObservableCollection<Log>();
            foreach (var day in quary)
            {
                if (date == day.Time)
                {
                    date = date.AddDays(1);
                    temp.Add(day);
                    if (temp.Count == 5)
                    {
                        Logs.Clear();
                        Logs.Add(temp.First());
                        break;
                    }
                }
                else
                {
                    date = day.Time;
                    date = date.AddDays(1);
                    temp.Clear();
                    temp.Add(day);
                }
            }
            if (temp.Count() < 5)
            {
                Logs.Clear();
            }
        }

        /// <summary>
        /// Sets mold risk to zero in condition where mold can not grow but the algoritm says it can
        /// </summary>
        private ObservableCollection<Log> CheckMoldRisk(IQueryable<Log> quary)
        {
            var Temp = quary.ToObservableCollection();
            foreach (var item in Temp)
            {
                if (item.Tempreture < 0 || item.Humidity < 75 || item.MoldRisk < 0)
                {
                    item.MoldRisk = 0;
                }
            }
            return Temp;
        }

        /// <summary>
        /// Opens the file explorer for the user and lets the user select a file and then uplads the file to the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void UploadFile()
        {
            try
            {
                DataFileReader dataFileReader = new DataFileReader();
                string filePath = dataFileReader.GetFilePath();
                dataFileReader.UploadFromFile(filePath);
                MessageBox.Show("Upload completed");
            }
            catch
            {
                MessageBox.Show("Upload failed");
            }
        }
    }
}
