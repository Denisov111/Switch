using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch.ViewModels;

namespace Switch.Views
{
    public class MVVMModelTemplateView
    {
        private MVVMModelTemplateViewModel mVVMModelTemplateViewModel;

        public MVVMModelTemplateView(MVVMModelTemplateViewModel mVVMModelTemplateViewModel)
        {
            this.mVVMModelTemplateViewModel = mVVMModelTemplateViewModel;
        }
    }
}
