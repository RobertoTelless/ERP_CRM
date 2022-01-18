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
    public class ServicoService : ServiceBase<SERVICO>, IServicoService
    {
        private readonly IServicoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ICategoriaServicoRepository _tipoRepository;
        private readonly IServicoAnexoRepository _anexoRepository;
        private readonly INomencBrasServicosRepository _nbseRepository;
        protected SystemBRDatabaseEntities Db = new SystemBRDatabaseEntities();

        public ServicoService(IServicoRepository baseRepository, ILogRepository logRepository, ICategoriaServicoRepository tipoRepository, IServicoAnexoRepository anexoRepository, INomencBrasServicosRepository nbseRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tipoRepository = tipoRepository;
            _anexoRepository = anexoRepository;
            _nbseRepository = nbseRepository;
        }

        public SERVICO CheckExist(SERVICO conta)
        {
            SERVICO item = _baseRepository.CheckExist(conta);
            return item;
        }

        public SERVICO GetItemById(Int32 id)
        {
            SERVICO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<SERVICO> GetAllItens()
        {
            return _baseRepository.GetAllItens();
        }

        public List<SERVICO> GetAllItensAdm()
        {
            return _baseRepository.GetAllItensAdm();
        }

        public List<CATEGORIA_SERVICO> GetAllTipos()
        {
            return _tipoRepository.GetAllItens();
        }

        public List<NOMENCLATURA_BRAS_SERVICOS> GetAllNBSE()
        {
            return _nbseRepository.GetAllItens();
        }

        public SERVICO_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public List<SERVICO> ExecuteFilter(Int32? catId, String nome, String descricao, String referencia)
        {
            return _baseRepository.ExecuteFilter(catId, nome, descricao, referencia);

        }

        public Int32 Create(SERVICO item, LOG log)
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

        public Int32 Create(SERVICO item)
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


        public Int32 Edit(SERVICO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    SERVICO obj = _baseRepository.GetById(item.SERV_CD_ID);
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

        public Int32 Edit(SERVICO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    SERVICO obj = _baseRepository.GetById(item.SERV_CD_ID);
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

        public Int32 Delete(SERVICO item, LOG log)
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
