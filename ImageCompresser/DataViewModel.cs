using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageCompresser
{
    class DataViewModel:INotifyPropertyChanged
    {
        public int CompressValue
        {
            set
            {
                CompressedFileSize = FileSize / value;
            }
        }
        private long compressedfilesize;
        public String Title { get; set; }
        public String FilePath { get; set; }
        public long FileSize { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public long CompressedFileSize
        { get
            {
                return compressedfilesize;
            }
            set{
                compressedfilesize = value;
                OnPropertyChanged("CompressedFileSize");
            } 
        }
    }
}
