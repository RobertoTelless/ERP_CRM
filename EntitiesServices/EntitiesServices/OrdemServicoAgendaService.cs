using EntitiesServices.Interfaces.Repositories;
using EntitiesServices.Interfaces.Services;
using EntitiesServices.Model;
using ModelServices.EntitiesServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesServices.Services
{
    public class OrdemServicoAgendaService : ServiceBase<ORDEM_SERVICO_AGENDA>, IOrdemServicoAgendaService
    {
        private readonly IOrdemServicoAgendaRepository _baseRepository;
        protected SystemBRDatabaseEntities Db = new SystemBRDatabaseEntities();

        public OrdemServicoAgendaService(IOrdemServicoAgendaRepository baseRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public List<ORDEM_SERVICO_AGENDA> GetAgendaByOs(ORDEM_SERVICO item)
        {
            return _baseRepository.GetAgendaByOs(item);
        }

        public Int32 Create(ORDEM_SERVICO_AGENDA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
