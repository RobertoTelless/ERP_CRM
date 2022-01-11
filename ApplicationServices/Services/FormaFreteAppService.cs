using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace ApplicationServices.Services
{
    public class FormaFreteAppService : AppServiceBase<FORMA_FRETE>, IFormaFreteAppService
    {
        private readonly IFormaFreteService _baseService;

        public FormaFreteAppService(IFormaFreteService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<FORMA_FRETE> GetAllItens()
        {
            return _baseService.GetAllItens();
        }
    }
}
