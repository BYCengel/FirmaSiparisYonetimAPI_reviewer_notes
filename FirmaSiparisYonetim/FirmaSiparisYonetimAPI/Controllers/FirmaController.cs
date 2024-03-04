using FirmaSiparisYonetimAPI.DTOModels;
using FirmaSiparisYonetimAPI.Requests;
using FirmaSiparisYonetimDataLayer.Tables;
using FirmaSiparisYonetimDataLayer.UOW;
using Microsoft.AspNetCore.Mvc;

namespace FirmaSiparisYonetimAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FirmaController : Controller
    {
        /*NOTES ABOUT CODE
         * 
         * - This is not a proper implementation of N-Layered Architecture. You should learn more about it.
         * You dont have a separate Business and Entities layer and more importantly you didn't separate your concerns between layers properly.
         * For example you should have put all data operations in your data layer.
         * - You should practice SOLID principles for clean code.
         * - You should only use English in your code apart from edge cases. It's harder to read and work with when you write in other languages and its not professional.
         * - Repository pattern is not properly implemented. You might wanna research it a bit more.
         * - You should utilize appsettings.json for constants like sql server address.
         * - You should use data annotations or some other approach to better validate your user input classes.
         * - You should use a class/interface like IBaseEntity for your other entities to inherit from. This makes your code cleaner and easier to work with.
         * 
         * 
         * SOME RESOURCES TO TAKE A LOOK AT
         * -Tim Corey SOLID principles playlist on youtube
         * -Gençay yıldız mini e-ticaret playlist on youtube
         * 
         * CONCLUSION
         * You have some nice ideas and approaches to the problem at hand but you need more experience and knowledge in the techniques you use to work with.
         * 
         * * Author: Burak Yavuz Çengel
         */

        private IUnitOfWork _uow;
        public FirmaController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet("Firmalar")]
        public List<FirmaDTO> Firmalar()
        {
            //you should include Products and Orders to this return because currently we can't see them.
            //you also should use your DataLayer for these kinds of data operations. You should have functions within your repository to accomplish these
            //tasks instead of bunching up everything here. I would also implement the UnitOfWork differently for better usability and functionality.
            //you can check out this playlist on youtube for these topics: "Gençay Yıldız mini e-ticaret serisi"
            //ex: _unitOfWork.CompanyReadRepository.GetCompaniesWithProductsAndOrders().ToList();
            return _uow.GetRepository<Firma>().All().Select(f => FirmaDataToObject(f)).ToList();
        }

        [HttpPost("FirmaGuncelle")]
        public GenelYanit FirmaGuncelle(FirmaGuncelleIstek firmaGuncelVeri)
        {

            Firma? firma = _uow.GetRepository<Firma>().Find(firmaGuncelVeri.FirmaId);
            if (firma == null)
                return new GenelYanit(false, $"Firma bulunamadı!");

            firma.Onay = firmaGuncelVeri.Onay;
            firma.SiparisIzinBaslangicSaati = firmaGuncelVeri.SiparisIzinBaslangicSaati;
            firma.SiparisIzinBitisSaati = firmaGuncelVeri.SiparisIzinBitisSaati;

            _uow.SaveChanges();

            return new GenelYanit(FirmaDataToObject(firma));
        }

        [HttpPost("FirmaEkle")]
        public GenelYanit FirmaEkle(FirmaEkleIstek firmaEkleVeri)
        {
            var firma = _uow.GetRepository<Firma>().Add(new Firma
            {
                Adi = firmaEkleVeri.Adi,
                Onay = firmaEkleVeri.Onay,
                SiparisIzinBaslangicSaati = firmaEkleVeri.SiparisBaslangicSaati,
                SiparisIzinBitisSaati = firmaEkleVeri.SiparisBitisSaati
            });

            _uow.SaveChanges();

            return new GenelYanit(FirmaDataToObject(firma));
        }

        private static FirmaDTO FirmaDataToObject(Firma firma)
        {//you could use a mapper here or some other approach instead of this (implicit operators).
            return new FirmaDTO
            {
                Adi = firma.Adi,
                Id = firma.Id,
                Onay = firma.Onay,
                SiparisIzinBaslangicSaati = firma.SiparisIzinBaslangicSaati,
                SiparisIzinBitisSaati = firma.SiparisIzinBitisSaati,
                Siparisler = firma.Siparisler?.Select(s => new SiparisDTO
                {
                    Id = s.Id,
                    KisiAdi = s.KisiAdi,
                    SiparisTarihi = s.SiparisTarihi,
                    UrunId = s.UrunId,
                }).ToList(),
                Urunler = firma.Urunler?.Select(u => new UrunDTO
                {
                    Id = u.Id,
                    Adi = u.Adi,
                    Fiyat = u.Fiyat,
                    Stok = u.Stok
                }).ToList()
            };
        }
    }
}
