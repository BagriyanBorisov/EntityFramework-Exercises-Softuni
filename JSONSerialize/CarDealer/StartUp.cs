using System.Globalization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;


namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext ctx = new CarDealerContext();
            //string input = File.ReadAllText(@"../../../Datasets/sales.json");

            string res = GetCarsFromMakeToyota(ctx);
            Console.WriteLine(res);

        }


        //Problem 09
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            IMapper mapper = MapConfig();


        var suppliersDto = JsonConvert.DeserializeObject<ImportSupplierDto[]>(inputJson);

            ICollection<Supplier> validSuppliers = new HashSet<Supplier>();
            foreach (var suppDto in suppliersDto)
            {
                Supplier supplier = mapper.Map<Supplier>(suppDto);

                validSuppliers.Add(supplier);
            }

            context.Suppliers.AddRange(validSuppliers);
            context.SaveChanges();
            return $"Successfully imported {validSuppliers.Count}.";
        }

        //Problem 10
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            IMapper mapper = MapConfig();

            ImportPartDto[] partsDto = JsonConvert.DeserializeObject<ImportPartDto[]>(inputJson);

            var validParts = new HashSet<Part>();
            foreach (var partDto in partsDto)
            {
                if (!context.Suppliers.Any(s => s.Id == partDto.SupplierId))
                {
                    continue;
                }

                Part part = mapper.Map<Part>(partDto);
                validParts.Add(part);
            }
            
            context.Parts.AddRange(validParts);
            context.SaveChanges();

            return $"Successfully imported {validParts.Count}.";
        }


        //Problem 11
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            IMapper mapper = MapConfig();

            var carsDto = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);

            ICollection<Car> cars = new HashSet<Car>();
            foreach (var cDto in carsDto)
            {
                Car car = mapper.Map<Car>(cDto);
                ICollection<PartCar> partCars = new HashSet<PartCar>();
                foreach (var pId in cDto.Parts.Distinct())
                {
                    if (!context.Parts.Any(p => p.Id == pId))
                    {
                        continue;
                    }

                    PartCar partCar = new PartCar() { PartId = pId, CarId = car.Id};
                    car.PartsCars.Add(partCar);
                    partCars.Add(partCar);

                }
                context.PartsCars.AddRange(partCars);
                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }

        //Problem 12
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            IMapper mapper = MapConfig();
            var customerDtos = JsonConvert.DeserializeObject<ImportCustomerDto[]>(inputJson);

            var validCustomers = new HashSet<Customer>();
            foreach (var customerDto in customerDtos)
            {
                Customer customer =mapper.Map<Customer>(customerDto);
                validCustomers.Add(customer);
            }

            context.Customers.AddRange(validCustomers);
            context.SaveChanges();
            return $"Successfully imported {validCustomers.Count}.";
        }


        //Problem 13
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            IMapper mapper = MapConfig();
            var saleDtos = JsonConvert.DeserializeObject<ImportSalesDto[]>(inputJson);

            var validSales = new HashSet<Sale>();
            foreach (var saleDto in saleDtos)
            {

                Sale sale = mapper.Map<Sale>(saleDto);
                validSales.Add(sale);
            }
            context.Sales.AddRange(validSales);
            context.SaveChanges();


            return $"Successfully imported {validSales.Count}.";
        }

        //Problem 14
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenByDescending(c => c.IsYoungDriver == false)
                .Select(c => new
                {
                    c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy",CultureInfo.InvariantCulture),
                    c.IsYoungDriver
                })
                .AsNoTracking()
                .ToArray();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }

        //Problem 15
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var toyotas = context.Cars.Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    TraveledDistance = c.TraveledDistance
                })
                .AsNoTracking()
                .ToArray();

            return JsonConvert.SerializeObject(toyotas, Formatting.Indented);
        }

        //Problem 16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    PartsCount = s.Parts.Count
                })
                .AsNoTracking()
                .ToArray();

            return JsonConvert.SerializeObject(suppliers, Formatting.Indented);
        }

        //Problem 17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,

                        TraveledDistance = c.TraveledDistance
                    },
                    parts = c.PartsCars.Select(cp => new
                    {
                        Name = cp.Part.Name,
                        Price = cp.Part.Price.ToString("f2")
                    }).ToArray()
                })
                .AsNoTracking()
                .ToArray();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

        //Problem 18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count >= 1)
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count,
                    spentMoney = 
                        c.Sales.Select(s => s.Car.PartsCars.Select(cp => cp.Part.Price).ToArray().Sum())
                })
                .AsNoTracking()
                .ToArray();

            var validCustomers = new HashSet<ExportCustomersTotalSalesDto>();
            foreach (var customer in customers.OrderByDescending(c => c.spentMoney.Sum()).ThenByDescending(c => c.boughtCars))
            {
                ExportCustomersTotalSalesDto validCust = new ExportCustomersTotalSalesDto()
                {
                    FullName = customer.fullName,
                    BoughtCars = customer.boughtCars,
                    SpentMoney = customer.spentMoney.Sum()
                };
                validCustomers.Add(validCust);
            }

            return JsonConvert.SerializeObject(validCustomers, Formatting.Indented);
        }

        public static IMapper MapConfig()
        {
            return new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));

        }
    }
}