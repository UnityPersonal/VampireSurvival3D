using System;
using System.Collections.Generic;

namespace DebugToolkit.ReportForm.Trello
{
    [Serializable]
    public class TrelloLabel
    {
        public string id;
        public string idBoard;
        public string name;
        public string color;
    }

    [Serializable]
    public class LabelList
    {
        public List<TrelloLabel> labels;
    }
}
