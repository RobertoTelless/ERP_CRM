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
