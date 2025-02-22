using Arabytak.Core.Entities;
using Arabytak.Core.Repositories.Contract;
using ARABYTAK.APIS.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ARABYTAK.APIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AdvertismentController : ControllerBase
    {


        private readonly IUnitOfWork _uniteOfWork;
        private readonly IMapper _mapper;

        public AdvertismentController(IUnitOfWork uniteOfWork, IMapper mapper)
        {
            _uniteOfWork = uniteOfWork;
            _mapper = mapper;
        }



        [HttpPost("Create")]
        public async Task<ActionResult<Advertisement>> Create(AdvertismentDto advertisementDto)
        {
            var Brands =  await _uniteOfWork.Repository<Brand>().GetAllAsync();
            var Models =  await _uniteOfWork.Repository<Model>().GetAllAsync();
            var Cars   =  await _uniteOfWork.Repository<Car>().GetAllAsync();
            var AdPlans = await _uniteOfWork.Repository<AdPlan>().GetAllAsync();
            var DealerShip = await _uniteOfWork.Repository<Dealership>().GetAllAsync();



            var Flag = !Brands.Any(B => B.Name.ToLower() == advertisementDto.BrandName.ToLower());
            if (Flag)
            {
                await _uniteOfWork.Repository<Brand>().AddAsync(
                    new Brand()
                    {
                        Name = advertisementDto.BrandName,
                        Id = Brands.Max(x => x.Id) + 1

                    });
                await _uniteOfWork.CompleteAsync();
            }

            var result = !Models.Any(B => B.Name.ToLower() == advertisementDto.ModelName.ToLower());
            if (result)
            {
                await _uniteOfWork.Repository<Model>().AddAsync(
                    new Model()
                    {
                        Name = advertisementDto.ModelName,
                        Id = Models.Max(X => X.Id) + 1
                    });
                await _uniteOfWork.CompleteAsync();
            }


                await _uniteOfWork.Repository<Dealership>().AddAsync(new Dealership()
                {
                    Id = Cars.Max(x => x.Id) + 1,
                    Phone1 = advertisementDto.PhoneNumber ,
                    WhatsApp1 = advertisementDto.Whatsapp ,
                    Facebook = advertisementDto.Facebook ,
                    Instagram =advertisementDto.Instegram,
                    Name = ""
                   
                });
                await _uniteOfWork.CompleteAsync();
           



            await ConfirmCreate(advertisementDto);

            advertisementDto.CarId = Cars.Max(B => B.Id) + 1;
            advertisementDto.AdPlanId = AdPlans.Max(X => X.Id) + 1;

            Advertisement advertisementCar = _mapper.Map<Advertisement>(advertisementDto);
            await _uniteOfWork.Repository<Advertisement>().AddAsync(advertisementCar);
            var numOfRowAffected = await _uniteOfWork.CompleteAsync();
            if (numOfRowAffected > 0) return Ok(advertisementDto);
            return BadRequest();
        }



        [HttpGet("Create")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<Advertisement>> ConfirmCreate(AdvertismentDto advertisementDto)
        {

            var Brands = await _uniteOfWork.Repository<Brand>().GetAllAsync();
            var Models = await _uniteOfWork.Repository<Model>().GetAllAsync();
            var Cars = await _uniteOfWork.Repository<Car>().GetAllAsync();
            var AdPlans = await _uniteOfWork.Repository<AdPlan>().GetAllAsync();
            var DealerShip = await _uniteOfWork.Repository<Dealership>().GetAllAsync();



            if (advertisementDto.PlanType is not null)
            {
                await _uniteOfWork.Repository<AdPlan>().AddAsync(new AdPlan()
                {
                    planType = PlanType.Yearly,
                    Price = advertisementDto.Price
                });
                await _uniteOfWork.CompleteAsync();
            }

            if (Cars is not null)
            {

                await _uniteOfWork.Repository<Car>().AddAsync(new Car()
                {
                    Id = Cars.Max(x => x.Id) + 1,
                    BrandId = Brands.Where(B => B.Name == advertisementDto.BrandName).Select(B => B.Id).FirstOrDefault(),
                    ModelId = Models.Where(M => M.Name == advertisementDto.ModelName).Select(M => M.Id).FirstOrDefault(),
                    Price = advertisementDto.Price,
                    status = Status.Used,
                    DealershipId = DealerShip.Max(x => x.Id)
                });
                await _uniteOfWork.CompleteAsync();
            }

            return Ok();


        }











        [HttpPut("Update")]
        public async Task<ActionResult<Advertisement>> Update(int id, int carId, int adplanId, [FromBody] AdvertismentDto advertisementDto)
        {

            var ExsistedAd = await _uniteOfWork.Repository<Advertisement>().GetById(id);
            if (ExsistedAd is not null)
            {

                var Brands = await _uniteOfWork.Repository<Brand>().GetAllAsync();
                var Models = await _uniteOfWork.Repository<Model>().GetAllAsync();
                var Cars = await _uniteOfWork.Repository<Car>().GetAllAsync();
                var AdPLans = await _uniteOfWork.Repository<AdPlan>().GetAllAsync();
                var DealerShip = await _uniteOfWork.Repository<Dealership>().GetAllAsync();


                var Flag = !Brands.Any(B => B.Name.ToLower() == advertisementDto.BrandName.ToLower());
                if (Flag)
                {
                    await _uniteOfWork.Repository<Brand>().AddAsync(
                        new Brand()
                        {
                            Name = advertisementDto.BrandName,
                            Id = Brands.Max(x => x.Id) + 1
                        });
                    await _uniteOfWork.CompleteAsync();
                }


                var result = !Models.Any(B => B.Name.ToLower() == advertisementDto.ModelName.ToLower());
                if (result)
                {
                    await _uniteOfWork.Repository<Model>().AddAsync(
                        new Model()
                        {
                            Name = advertisementDto.ModelName,
                            Id = Models.Max(x => x.Id) + 1
                        });
                    await _uniteOfWork.CompleteAsync();
                }

               
                await ConfirmUpdate(carId, adplanId, advertisementDto);

                ExsistedAd.Description = advertisementDto.Description;
                ExsistedAd.SellerEmail = advertisementDto.SellerEmail;
                ExsistedAd.ContactInfo = advertisementDto.ContactInfo;
                ExsistedAd.Price = advertisementDto.Price;
                ExsistedAd.ExpiryDate = advertisementDto.ExpiryDate;
                ExsistedAd.AdPlanId = adplanId;
                ExsistedAd.CarId = carId;




                //advertisementDto.CarId = carId;
                //advertisementDto.AdPlanId = adplanId;

                //Advertisement UpdatedAdvertismane =  _mapper.Map<AdvertismentDto, Advertisement>(advertisementDto);
                _uniteOfWork.Repository<Advertisement>().UpdateAsync(ExsistedAd);
                //var numOfRowAffected = await _uniteOfWork.Complete();
                // if (numOfRowAffected > 0) return Ok(advertisementDto);

                await _uniteOfWork.CompleteAsync();
                return Ok(advertisementDto);
            }

            return BadRequest();
        }



        [HttpGet("Update")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<Advertisement>> ConfirmUpdate(int carId, int adPlanId, AdvertismentDto advertisementDto)
        {

            var Brands = await _uniteOfWork.Repository<Brand>().GetAllAsync();
            var Models = await _uniteOfWork.Repository<Model>().GetAllAsync();
            var Cars = await _uniteOfWork.Repository<Car>().GetAllAsync();
            var AdPlans = await _uniteOfWork.Repository<AdPlan>().GetAllAsync();
            var DealerShip = await _uniteOfWork.Repository<Dealership>().GetAllAsync(); 

            var OldCar  = Cars.Where(C => C.Id == carId).FirstOrDefault();
            var OldPlan = AdPlans.Where(A => A.Id == adPlanId).FirstOrDefault();

            var OldDealerShipId = OldCar.DealershipId;
            var OldDealerShip = DealerShip.Where(D => D.Id == OldDealerShipId).FirstOrDefault();



            OldCar.BrandId = Brands.Where(B => B.Name == advertisementDto.BrandName).Select(B => B.Id).FirstOrDefault();
            OldCar.ModelId = Models.Where(M => M.Name == advertisementDto.ModelName).Select(M => M.Id).FirstOrDefault();
            OldCar.Price = advertisementDto.Price;
            OldCar.status = Status.Used;


             OldDealerShip.Name = "";
             OldDealerShip.Phone1 = advertisementDto.PhoneNumber;
             OldDealerShip.Facebook = advertisementDto.Facebook;
             OldDealerShip.WhatsApp1 = advertisementDto.Whatsapp;
             OldDealerShip.Instagram = advertisementDto.Instegram;


            OldPlan.planType = PlanType.Monthly;
            OldPlan.Price = advertisementDto.Price;

            await _uniteOfWork.CompleteAsync();

            return Ok();
        }










        [HttpDelete("Delete")]
        public async Task<ActionResult<AdvertismentDto>> Delete(int id, int carId, int adplanId)
        {

            var advertisment = await _uniteOfWork.Repository<Advertisement>().GetById(id);
            var AdvertiseCar = await _uniteOfWork.Repository<Car>().GetById(carId);
            var AdPlan = await _uniteOfWork.Repository<AdPlan>().GetById(adplanId);

            var OldDealerShipId = AdvertiseCar.DealershipId ?? 0 ;
           
           
             var DealerShip = await _uniteOfWork.Repository<Dealership>().GetById(OldDealerShipId);
           

          

            if (advertisment is not null && AdvertiseCar is not null && AdPlan is not null && DealerShip is not null )
            {

                _uniteOfWork.Repository<Advertisement>().DeleteAsync(advertisment);
                _uniteOfWork.Repository<Car>().DeleteAsync(AdvertiseCar);
                _uniteOfWork.Repository<AdPlan>().DeleteAsync(AdPlan);
                _uniteOfWork.Repository<Dealership>().DeleteAsync(DealerShip);
            }


            int numOfRowEffected = await _uniteOfWork.CompleteAsync();
            if (numOfRowEffected > 0) return Ok();
            return BadRequest();

        }










    }
}
