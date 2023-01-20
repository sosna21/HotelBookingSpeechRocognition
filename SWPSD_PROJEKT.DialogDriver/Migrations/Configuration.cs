
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace SWPSD_PROJEKT.DialogDriver.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<SWPSD_PROJEKT.DialogDriver.HotelContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
    } 
}