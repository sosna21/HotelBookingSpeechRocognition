using System;
using System.Threading.Tasks;
using SWPSD_PROJEKT.DialogDriver;
using SWPSD_PROJEKT.DialogDriver.Model;

namespace SWPSD_PROJEKT.UI;

public class Test
{
    private readonly UnitOfWork _unitOfWork;

    public Test(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddRoom()
    {
        _unitOfWork.Repository<Room>().Add(new Room{Name = "Name", Capacity = 5, Description = "Description", PricePerNight = 125});
        await _unitOfWork.Complete();
    }
    public async Task InitializeDb()
    {
        //TODO Add data to db
    }
}