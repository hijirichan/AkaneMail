using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AkaneMail
{
    public static class Extentions
    {
        public static SortOrder Invert(this SortOrder compare)
        {
            switch (compare) {
                case SortOrder.Ascending:
                    return SortOrder.Descending;
                case SortOrder.Descending:
                    return SortOrder.Ascending;
                default:
                    return SortOrder.Ascending;
            }
        }
    }
}
