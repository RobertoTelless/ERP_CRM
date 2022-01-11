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
    public class MovimentoEstoqueProdutoService : ServiceBase<MOVIMENTO_ESTOQUE_PRODUTO>, IMovimentoEstoqueProdutoService
    {
        private readonly IMovimentoEstoqueProdutoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IFilialRepository _filialRepository;
        protected SystemBRDatabaseEntities Db = new SystemBRDatabaseEntities();

        public MovimentoEstoqueProdutoService(IMovimentoEstoqueProdutoRepository baseRepository, ILogRepository logRepository, IFilialRepository filialRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _filialRepository = filialRepository;
        }

        public MOVIMENTO_ESTOQUE_PRODUTO GetByProdId(Int32 prod, Int32 fili)
        {
            MOVIMENTO_ESTOQUE_PRODUTO item = _baseRepository.GetByProdId(prod, fili);
            return item;
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItens()
        {
            List<MOVIMENTO_ESTOQUE_PRODUTO> lista = _baseRepository.GetAllItens();
            return lista;
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItensAdm()
        {
            List<MOVIMENTO_ESTOQUE_PRODUTO> lista = _baseRepository.GetAllItensAdm();
            return lista;
        }

        public MOVIMENTO_ESTOQUE_PRODUTO GetItemById(Int32 id)
        {
            MOVIMENTO_ESTOQUE_PRODUTO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItensEntrada()
        {
            List<MOVIMENTO_ESTOQUE_PRODUTO> lista = _baseRepository.GetAllItensEntrada();
            return lista;
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItensSaida()
        {
            List<MOVIMENTO_ESTOQUE_PRODUTO> lista = _baseRepository.GetAllItensSaida();
            return lista;
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> ExecuteFilter(Int32? catId, Int32? subCatId, String nome, String barcode, Int32? filiId, DateTime? dtMov)
        {
            return _baseRepository.ExecuteFilter(catId, subCatId, nome, barcode, filiId, dtMov);
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> ExecuteFilterAvulso(Int32? operacao, Int32? tipoMovimento, DateTime? dtInicial, DateTime? dtFinal, Int32? filial, Int32? prod)
        {
            return _baseRepository.ExecuteFilterAvulso(operacao, tipoMovimento, dtInicial, dtFinal, filial, prod);
        }

        public Int32 Create(MOVIMENTO_ESTOQUE_PRODUTO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
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

        public Int32 Create(MOVIMENTO_ESTOQUE_PRODUTO item)
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

        public Int32 Edit (MOVIMENTO_ESTOQUE_PRODUTO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    MOVIMENTO_ESTOQUE_PRODUTO obj = _baseRepository.GetById(item.MOEP_CD_ID);
                    _baseRepository.Detach(obj);
                    _logRepository.Add(log);
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
