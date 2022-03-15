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
using EntitiesServices.Interfaces.Repositories;

namespace ModelServices.EntitiesServices
{
    public class OrdemServicoService : ServiceBase<ORDEM_SERVICO>, IOrdemServicoService
    {
        private readonly IOrdemServicoRepository _baseRepository;
        private readonly IOrdemServicoAnexoRepository _anexoRepository;
        private readonly ILogRepository _logRepository;
        protected ERP_CRMEntities Db = new ERP_CRMEntities();

        public OrdemServicoService(IOrdemServicoRepository baseRepository, IOrdemServicoAnexoRepository anexoRepository, ILogRepository logRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _anexoRepository = anexoRepository;
            _logRepository = logRepository;
        }

        public ORDEM_SERVICO CheckExist(ORDEM_SERVICO conta, Int32 idAss)
        {
            ORDEM_SERVICO item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public ORDEM_SERVICO GetItemById(Int32 id)
        {
            ORDEM_SERVICO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<ORDEM_SERVICO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<ORDEM_SERVICO> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public ORDEM_SERVICO_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public List<ORDEM_SERVICO> ExecuteFilter(Int32? catOS, Int32? idClie, Int32? idUsu, DateTime? dtCriacao, Int32? status, Int32? idDept, Int32? idServ, Int32? idProd, Int32? idAten, Int32? idAss)
        {
            return _baseRepository.ExecuteFilter(catOS, idClie, idUsu, dtCriacao, status, idDept, idServ, idProd, idAten, idAss);
        }

        public Int32 Create(ORDEM_SERVICO item, LOG log)
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

        public Int32 Create(ORDEM_SERVICO item)
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


        public Int32 Edit(ORDEM_SERVICO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    ORDEM_SERVICO obj = _baseRepository.GetById(item.ORSE_CD_ID);
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

        public Int32 Edit(ORDEM_SERVICO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    ORDEM_SERVICO obj = _baseRepository.GetById(item.ORSE_CD_ID);
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

        public Int32 Delete(ORDEM_SERVICO item, LOG log)
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
