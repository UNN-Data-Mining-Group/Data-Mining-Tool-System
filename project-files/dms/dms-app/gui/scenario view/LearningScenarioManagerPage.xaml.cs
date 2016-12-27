using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using dms.view_models;

namespace dms.gui
{
    /// <summary>
    /// Interaction logic for LearningScenarioManagerPage.xaml
    /// </summary>
    public partial class LearningScenarioManagerPage : UserControl
    {
        public event Action<string, UserControl> OnShowPage;

        public LearningScenarioManagerPage(LearningScenarioManagerViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            vm.requestCreateLearningScenario += () => { var t = new CreateLearningScenarioPage(); OnShowPage?.Invoke("Создание сценария обучения", t); };
            vm.requestShowLearningScenario += (p) => { var t = new LearningScenarioInfoPage(p); OnShowPage?.Invoke(p.Name, t); };
        }
    }
}
