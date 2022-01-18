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
    public class AtendimentoService : ServiceBase<ATENDIMENTO>, IAtendimentoService
    {
        private readonly IAtendimentoRepository _baseRepository;
        private readonly IAtendimentoAnexoRepository _anexoRepository;
        private readonly ILogRepository _logRepository;
        private readonly IClienteRepository _cliRepository;
        private readonly IProdutoRepository _prodRepository;
        private readonly ICategoriaAtendimentoRepository _tipoRepository;
        protected SystemBRDatabaseEntities Db = new SystemBRDatabaseEntities();

        public AtendimentoService(IAtendimentoRepository baseRepository, ILogRepository logRepository, IClienteRepository cliRepository, IProdutoRepository prodRepository, ICategoriaAtendimentoRepository tipoRepository, IAtendimentoAnexoRepository anexoRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _cliRepository = cliRepository;
            _prodRepository = prodRepository;
            _tipoRepository = tipoRepository;
            _anexoRepository = anexoRepository;
        }

        public ATENDIMENTO CheckExist(ATENDIMENTO conta)
        {
            ATENDIMENTO item = _baseRepository.CheckExist(conta);
            return item;
        }

        public ATENDIMENTO GetItemById(Int32 id)
        {
            ATENDIMENTO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<ATENDIMENTO> GetAllItens()
        {
            return _baseRepository.GetAllItens();
        }

        public ATENDIMENTO_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public List<ATENDIMENTO> GetAllItensAdm()
        {
            return _baseRepository.GetAllItensAdm();
        }

        public List<ATENDIMENTO> GetByCliente(Int32 id)
        {
            return _baseRepository.GetByCliente(id);
        }

        public List<CATEGORIA_ATENDIMENTO> GetAllTipos()
        {
            return _tipoRepository.GetAllItens();
        }

        public List<ATENDIMENTO> ExecuteFilter(Int32? idCat, Int32? cliente, Int32? produto, DateTime? data, Int32? status, String descricao, Int32? depto, Int32? prioridade, Int32? idUsua, Int32? idServico, Int32? sla)
        {
            return _baseRepository.ExecuteFilter(idCat, cliente, produto, data, status, descricao, depto, prioridade, idUsua, idServico, sla);

        }

        public Int32 Create(ATENDIMENTO item, LOG log)
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

        public Int32 Create(ATENDIMENTO item)
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


        public Int32 Edit(ATENDIMENTO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    ATENDIMENTO obj = _baseRepository.GetById(item.ATEN_CD_ID);
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

        public Int32 Edit(ATENDIMENTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    ATENDIMENTO obj = _baseRepository.GetById(item.ATEN_CD_ID);
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

        public Int32 Delete(ATENDIMENTO item, LOG log)
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
