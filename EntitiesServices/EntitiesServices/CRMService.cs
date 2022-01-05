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
    public class CRMService : ServiceBase<CRM>, ICRMService
    {
        private readonly ICRMRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ITipoCRMRepository _tipoRepository;
        private readonly ICRMAnexoRepository _anexoRepository;
        private readonly IUsuarioRepository _usuRepository;
        private readonly ICRMOrigemRepository _oriRepository;
        private readonly IMotivoCancelamentoRepository _mcRepository;
        private readonly IMotivoEncerramentoRepository _meRepository;
        private readonly ITipoAcaoRepository _taRepository;
        private readonly ICRMAcaoRepository _acaRepository;
        private readonly ICRMContatoRepository _conRepository;
        private readonly ICRMComentarioRepository _comRepository;

        protected ERP_CRMEntities Db = new ERP_CRMEntities();

        public CRMService(ICRMRepository baseRepository, ILogRepository logRepository, ITipoCRMRepository tipoRepository, ICRMAnexoRepository anexoRepository, IUsuarioRepository usuRepository, ICRMOrigemRepository oriRepository, IMotivoCancelamentoRepository mcRepository, IMotivoEncerramentoRepository meRepository, ITipoAcaoRepository taRepository, ICRMAcaoRepository acaRepository, ICRMContatoRepository conRepository, ICRMComentarioRepository comRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tipoRepository = tipoRepository;
            _anexoRepository = anexoRepository;
            _usuRepository = usuRepository;
            _oriRepository = oriRepository;
            _mcRepository = mcRepository;
            _meRepository = meRepository;
            _taRepository = taRepository;
            _acaRepository = acaRepository;
            _conRepository = conRepository;
            _comRepository = comRepository;
        }

        public CRM CheckExist(CRM tarefa, Int32 idUsu,  Int32 idAss)
        {
            CRM item = _baseRepository.CheckExist(tarefa, idUsu, idAss);
            return item;
        }

        public CRM GetItemById(Int32 id)
        {
            CRM item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<CRM> GetByDate(DateTime data, Int32 idAss)
        {
            return _baseRepository.GetByDate(data, idAss);
        }

        public USUARIO GetUserById(Int32 id)
        {
            USUARIO item = _usuRepository.GetItemById(id);
            return item;
        }

        public List<CRM> GetByUser(Int32 user)
        {
            return _baseRepository.GetByUser(user);
        }

        public List<CRM_ACAO> GetAllAcoes(Int32 idAss)
        {
            return _acaRepository.GetAllItens(idAss);
        }

        public List<CRM> GetTarefaStatus(Int32 tipo, Int32 idAss)
        {
            return _baseRepository.GetTarefaStatus(tipo, idAss);
        }

        public CRM_CONTATO GetContatoById(Int32 id)
        {
            return _conRepository.GetItemById(id);
        }

        public CRM_ACAO GetAcaoById(Int32 id)
        {
            return _acaRepository.GetItemById(id);
        }

        public List<CRM> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<CRM> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<TIPO_CRM> GetAllTipos()
        {
            return _tipoRepository.GetAllItens();
        }

        public List<TIPO_ACAO> GetAllTipoAcao(Int32 idAss)
        {
            return _taRepository.GetAllItens(idAss);
        }
        public List<CRM_ORIGEM> GetAllOrigens(Int32 idAss)
        {
            return _oriRepository.GetAllItens(idAss);
        }

        public List<MOTIVO_CANCELAMENTO> GetAllMotivoCancelamento(Int32 idAss)
        {
            return _mcRepository.GetAllItens(idAss);
        }

        public List<MOTIVO_ENCERRAMENTO> GetAllMotivoEncerramento(Int32 idAss)
        {
            return _meRepository.GetAllItens(idAss);
        }

        public List<USUARIO> GetAllUsers(Int32 idAss)
        {
            return _usuRepository.GetAllItens(idAss);
        }

        public CRM_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public CRM_COMENTARIO GetComentarioById(Int32 id)
        {
            return _comRepository.GetItemById(id);
        }

        public List<CRM> ExecuteFilter(Int32? status, DateTime? inicio, DateTime? final, Int32? origem, Int32? adic, String nome, String busca, Int32? estrela, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(status, inicio, final, origem, adic, nome, busca, estrela, idAss);

        }

        public Int32 Create(CRM item, LOG log)
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

        public Int32 Create(CRM item)
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


        public Int32 Edit(CRM item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    CRM obj = _baseRepository.GetById(item.CRM1_CD_ID);
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

        public Int32 Edit(CRM item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CRM obj = _baseRepository.GetById(item.CRM1_CD_ID);
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

        public Int32 Delete(CRM item, LOG log)
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

        public Int32 EditContato(CRM_CONTATO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CRM_CONTATO obj = _conRepository.GetById(item.CRCO_CD_ID);
                    _conRepository.Detach(obj);
                    _conRepository.Update(item);
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

        public Int32 CreateContato(CRM_CONTATO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _conRepository.Add(item);
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

        public Int32 EditAcao(CRM_ACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CRM_ACAO obj = _acaRepository.GetById(item.CRAC_CD_ID);
                    _acaRepository.Detach(obj);
                    _acaRepository.Update(item);
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

        public Int32 CreateAcao(CRM_ACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _acaRepository.Add(item);
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
