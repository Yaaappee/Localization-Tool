using System.Collections.Generic;

namespace Localization_Tool
{
    class DataTable
    {
        public List<Translation> Translation { get; set; }

        public DataTable()
        {
            Translation = new List<Translation>();
        }
    }
}
