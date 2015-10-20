using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Model.Enums
{
    public enum ScenarioENUM
    {
        /// <summary>
        /// No scenario (free playing) 
        /// </summary>
        SC_NONE,

        /// <summary>
        /// Dullsville (boredom) 
        /// </summary>
        SC_DULLSVILLE,

        /// <summary>
        /// Rio (flooding) 
        /// </summary>
        SC_RIO,

        /// <summary>
        /// San francisco (earthquake) 
        /// </summary>
        SC_SAN_FRANCISCO,

        /// <summary>
        /// Hamburg (fire bombs) 
        /// </summary>
        SC_HAMBURG,

        /// <summary>
        /// ern (traffic) 
        /// </summary>
        SC_BERN,

        /// <summary>
        /// okyo (scary monster) 
        /// </summary>
        SC_TOKYO,

        /// <summary>
        /// Detroit (crime) 
        /// </summary>
        SC_DETROIT,

        /// <summary>
        /// Boston (nuclear meltdown) 
        /// </summary>
        SC_BOSTON
    }

    public class Scenario
    {
        public ScenarioENUM Type { get; set; }
        public int ID { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }

        public Scenario(ScenarioENUM type, int id, string fileName, string name)
        {
            this.Type = type;
            this.ID = id;
            this.FileName = fileName;
            this.Name = name;
        } 
    }
}
