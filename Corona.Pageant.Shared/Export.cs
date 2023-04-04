using System.Collections.Generic;

namespace Corona.Pageant.Shared
{
    public class Export
    {
        public List<Scripts> Scripts { get; set; } = new List<Scripts>();
        public List<Settings> Settings { get; set; } = new List<Settings>();
    }
}
