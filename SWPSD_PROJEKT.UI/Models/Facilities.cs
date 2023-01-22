using System.Collections.Generic;
using System.Linq;
using SWPSD_PROJEKT.DialogDriver;
using SWPSD_PROJEKT.DialogDriver.Model;

namespace SWPSD_PROJEKT.UI.Models;

public class Facilities
{
    public bool SingleBed { get; set; }
    public bool DoubleBed { get; set; }
    public bool Breakfast { get; set; }
    public bool Pets { get; set; }
    public bool AlcoholBar { get; set; }
    public bool ExtraBedForChild { get; set; }

    public IEnumerable<Facility> GetDbFacilities(IRepository<Facility> facilityRepo)
    {
        
        if (DoubleBed)
        {
            yield return facilityRepo.GetQueryable().SingleOrDefault(x => x.Name == nameof(DoubleBed));
        }

        if (Breakfast)
        {
            yield return facilityRepo.GetQueryable().SingleOrDefault(x => x.Name == nameof(Breakfast));
        }

        if (Pets)
        {
            yield return facilityRepo.GetQueryable().SingleOrDefault(x => x.Name == nameof(Pets));
        }

        if (AlcoholBar)
        {
            yield return facilityRepo.GetQueryable().SingleOrDefault(x => x.Name == nameof(AlcoholBar));
        }
        if (ExtraBedForChild)
        {
            yield return facilityRepo.GetQueryable().SingleOrDefault(x => x.Name == nameof(ExtraBedForChild));
        }
    }
}