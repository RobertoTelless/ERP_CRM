using DataServices.Repositories;
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
    public class OrdemServicoComentarioService : ServiceBase<ORDEM_SERVICO_COMENTARIOS>, IOrdemServicoComentarioService
    {
        private readonly IOrdemServicoComentarioRepository _baseRepository;
        protected ERP_CRMEntities Db = new ERP_CRMEntities();

        public OrdemServicoComentarioService(IOrdemServicoComentarioRepository baseRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public List<ORDEM_SERVICO_COMENTARIOS> GetByOs(ORDEM_SERVICO item)
        {
            return _baseRepository.GetByOs(item);
        }

        public Int32 Create(ORDEM_SERVICO_COMENTARIOS item)
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
