using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class FormaEnvioService : ServiceBase<FORMA_ENVIO>, IFormaEnvioService
    {
        private readonly IFormaEnvioRepository _baseRepository;
        protected SystemBRDatabaseEntities Db = new SystemBRDatabaseEntities();

        public FormaEnvioService(IFormaEnvioRepository baseRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public List<FORMA_ENVIO> GetAllItens()
        {
            return _baseRepository.GetAllItens();
        }
    }
}
