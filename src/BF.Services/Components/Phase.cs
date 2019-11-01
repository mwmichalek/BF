using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF.Services.Components {

    public enum PhaseId {
        FillStrikeWater = 0,
        HeatStrikeWater = 1,
        Mash = 2,
        MashOut = 3,
        Sparge = 4,
        Boil = 5,
        Chill = 6,

        Fermentation = 7
    }

    public class Phase {

        public Phase(PhaseId id, int durationInMinutes) {
            Id = id;
        }

        public PhaseId Id { get; private set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool IsEngaged { get; set; }
    }
}
