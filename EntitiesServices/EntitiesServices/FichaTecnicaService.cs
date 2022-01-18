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
    public class FichaTecnicaService : ServiceBase<FICHA_TECNICA>, IFichaTecnicaService
    {
        private readonly IFichaTecnicaRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IFichaTecnicaDetalheRepository _detRepository;
        protected ERP_CRMEntities Db = new ERP_CRMEntities();

        public FichaTecnicaService(IFichaTecnicaRepository baseRepository, ILogRepository logRepository, IFichaTecnicaDetalheRepository detRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _detRepository = detRepository;
        }

        public FICHA_TECNICA CheckExist(FICHA_TECNICA conta, Int32 idAss)
        {
            FICHA_TECNICA item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public FICHA_TECNICA GetItemById(Int32 id)
        {
            FICHA_TECNICA item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<FICHA_TECNICA> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<FICHA_TECNICA> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public FICHA_TECNICA_DETALHE GetDetalheById(Int32 id)
        {
            return _detRepository.GetItemById(id);
        }

        public List<FICHA_TECNICA> ExecuteFilter(Int32? prodId, Int32? cat, String descricao, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(prodId, cat, descricao, idAss);

        }

        public Int32 Create(FICHA_TECNICA item, LOG log)
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

        public Int32 Create(FICHA_TECNICA item)
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


        public Int32 Edit(FICHA_TECNICA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    FICHA_TECNICA obj = _baseRepository.GetById(item.FITE_CD_ID);
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

        public Int32 Edit(FICHA_TECNICA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    FICHA_TECNICA obj = _baseRepository.GetById(item.FITE_CD_ID);
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

        public Int32 Delete(FICHA_TECNICA item, LOG log)
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

        public Int32 EditInsumo(FICHA_TECNICA_DETALHE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    if (item.PRODUTO != null)
                    {
                        item.PRODUTO = null;
                    }

                    FICHA_TECNICA_DETALHE obj = _detRepository.GetById(item.FITD_CD_ID);
                    _detRepository.Detach(obj);
                    _detRepository.Update(item);
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

        public Int32 CreateInsumo(FICHA_TECNICA_DETALHE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _detRepository.Add(item);
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
