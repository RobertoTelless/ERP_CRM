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
    public class SubcategoriaProdutoService : ServiceBase<SUBCATEGORIA_PRODUTO>, ISubcategoriaProdutoService
    {
        private readonly ISubcategoriaProdutoRepository _baseRepository;
        private readonly ICategoriaProdutoRepository _gruRepository;
        private readonly ILogRepository _logRepository;
        protected ERP_CRMEntities Db = new ERP_CRMEntities();

        public SubcategoriaProdutoService(ISubcategoriaProdutoRepository baseRepository, ILogRepository logRepository, ICategoriaProdutoRepository gruRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _gruRepository = gruRepository;
        }

        public SUBCATEGORIA_PRODUTO CheckExist(SUBCATEGORIA_PRODUTO conta, Int32 idAss)
        {
            SUBCATEGORIA_PRODUTO item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public SUBCATEGORIA_PRODUTO GetItemById(Int32 id)
        {
            SUBCATEGORIA_PRODUTO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<SUBCATEGORIA_PRODUTO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<SUBCATEGORIA_PRODUTO> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<CATEGORIA_PRODUTO> GetAllCategorias(Int32 idAss)
        {
            return _gruRepository.GetAllItens(idAss);
        }

        public Int32 Create(SUBCATEGORIA_PRODUTO item, LOG log)
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

        public Int32 Create(SUBCATEGORIA_PRODUTO item)
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


        public Int32 Edit(SUBCATEGORIA_PRODUTO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    SUBCATEGORIA_PRODUTO obj = _baseRepository.GetById(item.SCPR_CD_ID);
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

        public Int32 Edit(SUBCATEGORIA_PRODUTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    SUBCATEGORIA_PRODUTO obj = _baseRepository.GetById(item.SCPR_CD_ID);
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

        public Int32 Delete(SUBCATEGORIA_PRODUTO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
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
