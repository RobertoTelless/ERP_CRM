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
    public class ProdutoTabelaPrecoService : ServiceBase<PRODUTO_TABELA_PRECO>, IProdutoTabelaPrecoService
    {
        private readonly IProdutoTabelaPrecoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        protected ERP_CRMEntities Db = new ERP_CRMEntities();

        public ProdutoTabelaPrecoService(IProdutoTabelaPrecoRepository baseRepository, ILogRepository logRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;

        }

        public PRODUTO_TABELA_PRECO CheckExist(PRODUTO_TABELA_PRECO item, Int32 idAss)
        {
            PRODUTO_TABELA_PRECO obj = _baseRepository.CheckExist(item, idAss);
            return obj;
        }

        public PRODUTO_TABELA_PRECO GetItemById(Int32 id)
        {
            PRODUTO_TABELA_PRECO item = _baseRepository.GetItemById(id);
            return item;
        }

        public PRODUTO_TABELA_PRECO GetByProdFilial(Int32 prod, Int32 fili)
        {
            return _baseRepository.GetByProdFilial(prod, fili);
        }

        public Int32 Create(PRODUTO_TABELA_PRECO item)
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

        public Int32 Edit(PRODUTO_TABELA_PRECO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PRODUTO_TABELA_PRECO obj = _baseRepository.GetById(item.PRTP_CD_ID);
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

        public Int32 Delete(PRODUTO_TABELA_PRECO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _baseRepository.Remove(item);
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
