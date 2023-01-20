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
        var room = new RoomType{Name = "Name"};
        _unitOfWork.Repository<RoomType>().Add(room);
        await _unitOfWork.Complete();
        
    }
}