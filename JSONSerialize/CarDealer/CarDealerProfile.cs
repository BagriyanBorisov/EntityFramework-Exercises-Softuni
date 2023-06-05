using AutoMapper;
using CarDealer.DTOs.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            //Supplier
            this.CreateMap<ImportSupplierDto, Supplier>();

            //Part
            this.CreateMap<ImportPartDto, Part>();

            //Car
            this.CreateMap<ImportCarDto, Car>()
                .ForSourceMember(s => s.Parts,
                    opt=> opt.DoNotValidate());

            //PartCar
            this.CreateMap<ImportCarPartDto, PartCar>();

            //Customer
            this.CreateMap<ImportCustomerDto,Customer>();

            //Sales
            this.CreateMap<ImportSalesDto, Sale>();
        }
    }
}
