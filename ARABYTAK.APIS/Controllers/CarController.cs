using Arabytak.Core.Entities;
using Arabytak.Core.Repositories.Contract;
using Arabytak.Core.Specification.CarSpecification;
using ARABYTAK.APIS.DTOs;
using ARABYTAK.APIS.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace ARABYTAK.APIS.Controllers
{
        //[Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]

    public class CarController : BaseApiController
    {
       // private readonly IGenericRepository<Car> _genericRepository;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Brand> _brandRepo;
        private readonly IGenericRepository<Dealership> _dealRepo;
        private readonly IGenericRepository<RescueCompany> _rescueRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CarController(/*IGenericRepository<Car> genericRepository*/ IMapper mapper
            /*IGenericRepository<Brand> brandRepo*/ /*IGenericRepository<Dealership> dealRepo*/
            /*IGenericRepository<RescueCompany> RescueRepo*/ ,IUnitOfWork unitOfWork)
        {
           // _genericRepository = genericRepository;

            _mapper = mapper;
           //brandRepo = brandRepo;
           //dealRepo = dealRepo;
           // _rescueRepo = RescueRepo;
            _unitOfWork = unitOfWork;
        }
        
        //public async Task<ActionResult<IReadOnlyList<CarDto>>> GetCarsBySearchAndPagination([FromQuery] CarSpecParams SpecParams)
        //{
        //    var spec =new CarWithBrandAndCategoryAndPicUrlAndSpecSpecification(SpecParams);
        //    var cars=await _genericRepository.GetAllWithSpecAsync(spec);
        //    var data = _mapper.Map<IReadOnlyList<Car>,IReadOnlyList<CarDto>>(cars);
        //    foreach (var carDto in data)
        //    {
        //        var car = cars.FirstOrDefault(c => c.Id == carDto.Id);
        //        if (car != null)
        //        {
        //            // إضافة اسم السيارة (Brand + Model)
        //            carDto.CarName = $"{car.brand.Name} {car.model.Name}".Trim();

        //            // تحديد المواصفات بناءً على الحالة
        //            if (car.status == Status.New && car.specNewCar != null)
        //            {
        //                carDto.Specifications = _mapper.Map<SpecNewCarDto>(car.specNewCar);
        //            }
        //            else if (car.status == Status.Used && car.specUsedCar != null)
        //            {
        //                carDto.Specifications = _mapper.Map<SpecUsedCarDto>(car.specUsedCar);
        //            }
        //        }
        //    }

        //    var countSpec = new CarWithFilterationForCountSpec(SpecParams);
        //    var count= await _genericRepository.GetCountAsync(countSpec);
        //    return Ok(new Pagination<CarDto>(SpecParams.PageSize, SpecParams.PageIndex, count,data));

        //}



        [HttpGet("{status}/{CarId}")]
        public async Task<ActionResult<CarDto>> GetCarDetails(int CarId, [FromQuery] Status status)
        {
            var spec = new CarWithBrandAndCategoryAndPicUrlAndSpecSpecification(CarId, status);
            var car = await _unitOfWork.Repository<Car>().GetByIdWithSpecAsync(spec);
            if (car == null)
            {
                return NotFound();
            }
            var carDto = _mapper.Map<Car, CarDto>(car);
            carDto.CarName = $"{car.brand.Name} {car.model.Name}";
            if (status == Status.New)
            {
                var specDto = _mapper.Map<SpecNewCar, SpecNewCarDto>(car.specNewCar);
                carDto.Specifications = specDto;
            }
            else if (status == Status.Used)
            {
                var specDto = _mapper.Map<SpecUsedCar, SpecUsedCarDto>(car.specUsedCar);
                carDto.Specifications = specDto;
            }
            // carDto.DealershipName = car.dealership.Name;

            return Ok(carDto);
        }


        [HttpGet("{status}") ]
        public async Task<ActionResult<CarListDto>> GetListOfCar([FromQuery] Status status)
        {
           
            var spec = new CarWithBrandAndCategoryAndPicUrlAndSpecSpecification(status);
            var car= await _unitOfWork.Repository<Car>().GetAllWithSpecAsync(spec);
            if (car == null)
            {
                return NotFound();
            }
            var carsDto = _mapper.Map<IReadOnlyList<Car>, IReadOnlyList<CarListDto>>(car);
          return Ok(carsDto);

        }

        [HttpGet("Brands")]
        public async Task<ActionResult<IReadOnlyList<Brand>>> GetBrand()
        {
            var brands= await _unitOfWork.Repository<Brand>().GetAllAsync();
            return Ok(brands);
        }
        [HttpGet("Dealership")]
        public async Task<ActionResult<IReadOnlyList<Dealership>>> GetDealerShip()
        {
            var dealerships= await _unitOfWork.Repository<Dealership>().GetAllAsync();
            return Ok(dealerships);
        }
        [HttpGet("RescueCompany")]
        public async Task<ActionResult<IReadOnlyList<RescueCompany>>> GetRescueCompany()
        {
            var RescueCompany=  await _unitOfWork.Repository<RescueCompany>().GetAllAsync();
            return Ok(RescueCompany);

        }
      
    }
}
