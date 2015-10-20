using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Model.Enums
{
    public static class Scenarios
    {
        public static Dictionary<ScenarioENUM, Scenario> Items { get; }

        static Scenarios()
        {
            Items = new Dictionary<ScenarioENUM, Scenario>();
            Items.Add(ScenarioENUM.SC_NONE, new Scenario(ScenarioENUM.SC_DULLSVILLE, 0, "snro.111", "None"));
            Items.Add(ScenarioENUM.SC_DULLSVILLE, new Scenario(ScenarioENUM.SC_DULLSVILLE, 1, "snro.111", "Dullsville"));
            Items.Add(ScenarioENUM.SC_RIO, new Scenario(ScenarioENUM.SC_RIO, 8, "snro.888", "Rio"));
            Items.Add(ScenarioENUM.SC_SAN_FRANCISCO, new Scenario(ScenarioENUM.SC_SAN_FRANCISCO, 2, "snro.222", "SanFrancisco"));
            Items.Add(ScenarioENUM.SC_HAMBURG, new Scenario(ScenarioENUM.SC_HAMBURG, 3, "snro.333", "Hamburg"));
            Items.Add(ScenarioENUM.SC_BERN, new Scenario(ScenarioENUM.SC_BERN, 4, "snro.444", "Bern"));
            Items.Add(ScenarioENUM.SC_TOKYO, new Scenario(ScenarioENUM.SC_TOKYO, 5, "snro.555", "Tokyo"));
            Items.Add(ScenarioENUM.SC_DETROIT, new Scenario(ScenarioENUM.SC_DETROIT, 6, "snro.666", "Detroit"));
            Items.Add(ScenarioENUM.SC_BOSTON, new Scenario(ScenarioENUM.SC_BOSTON, 7, "snro.777", "Boston"));
        }
    }
}
