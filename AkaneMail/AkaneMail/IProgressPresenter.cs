using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AkaneMail
{
    interface IProgressPresenter
    {
        bool initProgress(int min, int max);
        void updateProgress(int min, int max, int current);
        void hideProgress();
    }
}
