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
    public class ServicoTabelaPrecoService : ServiceBase<SERVICO_TABELA_PRECO>, IServicoTabelaPrecoService
    {
        private readonly IServicoTabelaPrecoRepository _baseRepository;
        protected SystemBRDatabaseEntities Db = new SystemBRDatabaseEntities();

        public ServicoTabelaPrecoService(IServicoTabelaPrecoRepository baseRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public SERVICO_TABELA_PRECO GetByServFilial(Int32 id, Int32 fili)
        {
            return _baseRepository.GetByServFilial(id, fili);
        }

        public Int32 Create(SERVICO_TABELA_PRECO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                if (_baseRepository.CheckExist(item.FILI_CD_ID, item.SERV_CD_ID) != null)
                {
                    return 1;
                }

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

        public Int32 Edit(SERVICO_TABELA_PRECO item, Int32 id)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    SERVICO_TABELA_PRECO obj = _baseRepository.GetById(id);
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
