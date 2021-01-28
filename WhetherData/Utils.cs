using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhetherData
{
    public static class Utils
    {
        /// <summary>
        /// Creates a ObservableCollection<Log> from an IEnumerable<Log>
        /// </summary>
        /// <param name="logs"></param>
        /// <returns></returns>
        public static ObservableCollection<Log> ToObservableCollection(this IEnumerable<Log> logs)
        {
            ObservableCollection<Log> temp = new ObservableCollection<Log>();
            foreach (var item in logs)
            {
                temp.Add(item);
            }
            return temp;
        }
    }
}
