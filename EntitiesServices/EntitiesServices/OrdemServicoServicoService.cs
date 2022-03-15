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
    public class OrdemServicoServicoService : ServiceBase<ORDEM_SERVICO_SERVICO>, IOrdemServicoServicoService
    {
        private readonly IOrdemServicoServicoRepository _baseRepository;
        protected ERP_CRMEntities Db = new ERP_CRMEntities();

        public OrdemServicoServicoService(IOrdemServicoServicoRepository baseRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public ORDEM_SERVICO_SERVICO CheckExist(ORDEM_SERVICO_SERVICO item)
        {
            return _baseRepository.CheckExist(item);
        }

        public List<ORDEM_SERVICO_SERVICO> GetAllByOs(Int32 id)
        {
            return _baseRepository.GetAllbyOs(id);
        }

        public Int32 Create(ORDEM_SERVICO_SERVICO item)
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

        public Int32 Edit(ORDEM_SERVICO_SERVICO item, ORDEM_SERVICO_SERVICO itemAntes)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    ORDEM_SERVICO_SERVICO obj = _baseRepository.GetById(item.OSSE_CD_ID);
                    _baseRepository.Detach(obj);
                    _baseRepository.Update(item);
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
