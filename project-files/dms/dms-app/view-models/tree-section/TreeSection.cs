using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace dms.view_models
{
    public class TreeSection : ViewmodelBase
    {
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<TreeSection> Content { get; set; }
        public TreeSection() { Title = ""; Content = null; }
        public TreeSection(string title) : this() { Title = title; }
        public TreeSection(string title, string[] content)
        {
            Title = title;
            Content = new ObservableCollection<TreeSection>();
            for (int i = 0; i < content.Length; i++)
            {
                Content.Add(new TreeSection(content[i]));
            }
        }
    }
}
