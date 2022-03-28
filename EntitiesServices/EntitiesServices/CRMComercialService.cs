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
    public class CRMComercialService : ServiceBase<CRM_COMERCIAL>, ICRMComercialService
    {
        private readonly ICRMComercialRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ITipoCRMRepository _tipoRepository;
        private readonly ICRMComercialAnexoRepository _anexoRepository;
        private readonly IUsuarioRepository _usuRepository;
        private readonly ICRMOrigemRepository _oriRepository;
        private readonly IMotivoCancelamentoRepository _mcRepository;
        private readonly IMotivoEncerramentoRepository _meRepository;
        private readonly ITipoAcaoRepository _taRepository;
        private readonly ICRMComercialAcaoRepository _acaRepository;
        private readonly ICRMComercialContatoRepository _conRepository;
        private readonly ICRMComercialComentarioRepository _comRepository;
        private readonly IFilialRepository _filRepository;
        private readonly ICRMComercialItemRepository _itemRepository;

        protected ERP_CRMEntities Db = new ERP_CRMEntities();

        public CRMComercialService(ICRMComercialRepository baseRepository, ILogRepository logRepository, ITipoCRMRepository tipoRepository, ICRMComercialAnexoRepository anexoRepository, IUsuarioRepository usuRepository, ICRMOrigemRepository oriRepository, IMotivoCancelamentoRepository mcRepository, IMotivoEncerramentoRepository meRepository, ITipoAcaoRepository taRepository, ICRMComercialAcaoRepository acaRepository, ICRMComercialContatoRepository conRepository, ICRMComercialComentarioRepository comRepository, IFilialRepository filRepository, ICRMComercialItemRepository itemRepository) : base(baseRepository)
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
            _filRepository = filRepository;
            _itemRepository = itemRepository;
        }

        public CRM_COMERCIAL CheckExist(CRM_COMERCIAL tarefa, Int32 idUsu,  Int32 idAss)
        {
            CRM_COMERCIAL item = _baseRepository.CheckExist(tarefa, idUsu, idAss);
            return item;
        }

        public CRM_COMERCIAL GetItemById(Int32 id)
        {
            CRM_COMERCIAL item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<CRM_COMERCIAL> GetByDate(DateTime data, Int32 idAss)
        {
            return _baseRepository.GetByDate(data, idAss);
        }

        public USUARIO GetUserById(Int32 id)
        {
            USUARIO item = _usuRepository.GetItemById(id);
            return item;
        }

        public List<CRM_COMERCIAL> GetByUser(Int32 user)
        {
            return _baseRepository.GetByUser(user);
        }

        public List<CRM_COMERCIAL_ACAO> GetAllAcoes(Int32 idAss)
        {
            return _acaRepository.GetAllItens(idAss);
        }

        public List<CRM_COMERCIAL> GetTarefaStatus(Int32 tipo, Int32 idAss)
        {
            return _baseRepository.GetTarefaStatus(tipo, idAss);
        }

        public CRM_COMERCIAL_CONTATO GetContatoById(Int32 id)
        {
            return _conRepository.GetItemById(id);
        }

        public CRM_COMERCIAL_ACAO GetAcaoById(Int32 id)
        {
            return _acaRepository.GetItemById(id);
        }

        public List<CRM_COMERCIAL> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<CRM_COMERCIAL> GetAllItensAdm(Int32 idAss)
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

        public CRM_COMERCIAL_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public CRM_COMERCIAL_COMENTARIO_NOVA GetComentarioById(Int32 id)
        {
            return _comRepository.GetItemById(id);
        }

        public List<CRM_COMERCIAL> ExecuteFilter(Int32? status, DateTime? inicio, DateTime? final, Int32? origem, Int32? adic, String nome, String busca, Int32? estrela, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(status, inicio, final, origem, adic, nome, busca, estrela, idAss);

        }

        public List<CRM_COMERCIAL> ExecuteFilterDash(String nmr, DateTime? dtFinal, String nome, Int32? usu, Int32? status, Int32 idAss)
        {
            return _baseRepository.ExecuteFilterDash(nmr, dtFinal, nome, usu, status, idAss);
        }

        public Int32 Create(CRM_COMERCIAL item, LOG log)
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

        public Int32 Create(CRM_COMERCIAL item)
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


        public Int32 Edit(CRM_COMERCIAL item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    CRM_COMERCIAL obj = _baseRepository.GetById(item.CRMC_CD_ID);
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

        public Int32 Edit(CRM_COMERCIAL item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CRM_COMERCIAL obj = _baseRepository.GetById(item.CRMC_CD_ID);
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

        public Int32 Delete(CRM_COMERCIAL item, LOG log)
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

        public Int32 EditContato(CRM_COMERCIAL_CONTATO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CRM_COMERCIAL_CONTATO obj = _conRepository.GetById(item.CRCO_CD_ID);
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

        public Int32 CreateContato(CRM_COMERCIAL_CONTATO item)
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

        public Int32 EditAcao(CRM_COMERCIAL_ACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CRM_COMERCIAL_ACAO obj = _acaRepository.GetById(item.CRCA_CD_ID);
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

        public Int32 CreateAcao(CRM_COMERCIAL_ACAO item)
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

        public List<CRM_COMERCIAL> GetAllItensAdmUser(Int32 id, Int32 idAss)
        {
            return _baseRepository.GetAllItensAdmUser(id, idAss);
        }

        public List<CRM_COMERCIAL> GetAtrasados(Int32 idAss)
        {
            return _baseRepository.GetAtrasados(idAss);
        }

        public List<CRM_COMERCIAL> GetEncerrados(Int32 idAss)
        {
            return _baseRepository.GetEncerrados(idAss);
        }

        public List<CRM_COMERCIAL> GetCancelados(Int32 idAss)
        {
            return _baseRepository.GetCancelados(idAss);
        }

        public List<FILIAL> GetAllFilial(Int32 idAss)
        {
            return _filRepository.GetAllItens(idAss);
        }

        public Int32 CreateAcompanhamento(CRM_COMERCIAL_COMENTARIO_NOVA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _comRepository.Add(item);
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

        public CRM_COMERCIAL_ITEM GetItemCRMById(Int32 id)
        {
            CRM_COMERCIAL_ITEM item = _itemRepository.GetItemById(id);
            return item;
        }

        public Int32 EditItemCRM(CRM_COMERCIAL_ITEM item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CRM_COMERCIAL_ITEM obj = _itemRepository.GetItemById(item.CRCI_CD_ID);
                    _itemRepository.Detach(obj);
                    _itemRepository.Update(item);
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

        public Int32 CreateItemCRM(CRM_COMERCIAL_ITEM item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _itemRepository.Add(item);
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
