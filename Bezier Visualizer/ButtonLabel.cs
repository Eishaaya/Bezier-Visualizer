using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bezier_Visualizer
{
    class ButtonLabel
    {
        public Button Button { get; set; }
        public Label Label { get; set; }

        public ButtonLabel(Button button, Label label)
        {
            Button = button;
            Label = label;
        }
    }
}
