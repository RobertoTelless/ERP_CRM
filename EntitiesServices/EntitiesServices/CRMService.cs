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
        private readonly ITemplatePropostaRepository _tpRepository;
        private readonly ICRMPropostaRepository _proRepository;
        private readonly ICRMPropostaComentarioRepository _pcRepository;
        private readonly ICRMPropostaAnexoRepository _aneprRepository;
        private readonly ICRMPedidoAnexoRepository _anepeRepository;
        private readonly ICRMPedidoComentarioRepository _compeRepository;
        private readonly ICRMPedidoRepository _pedRepository;
        private readonly ICRMItemPedidoRepository _itepeRepository;
        private readonly IFormaEnvioRepository _fenvRepository;
        private readonly IFormaFreteRepository _fretRepository;
        private readonly IFilialRepository _filRepository;

        protected ERP_CRMEntities Db = new ERP_CRMEntities();

        public CRMService(ICRMRepository baseRepository, ILogRepository logRepository, ITipoCRMRepository tipoRepository, ICRMAnexoRepository anexoRepository, IUsuarioRepository usuRepository, ICRMOrigemRepository oriRepository, IMotivoCancelamentoRepository mcRepository, IMotivoEncerramentoRepository meRepository, ITipoAcaoRepository taRepository, ICRMAcaoRepository acaRepository, ICRMContatoRepository conRepository, ICRMComentarioRepository comRepository, ITemplatePropostaRepository tpRepository, ICRMPropostaRepository proRepository, ICRMPropostaComentarioRepository pcRepository, ICRMPropostaAnexoRepository aneprRepository, ICRMPedidoAnexoRepository anepeRepository, ICRMPedidoComentarioRepository compeRepository, ICRMPedidoRepository pedRepository, ICRMItemPedidoRepository itepeRepository, IFormaEnvioRepository fenvRepository, IFormaFreteRepository fretRepository, IFilialRepository filRepository) : base(baseRepository)
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
            _tpRepository = tpRepository;
            _proRepository = proRepository;
            _pcRepository = pcRepository;
            _aneprRepository = aneprRepository;
            _anepeRepository = anepeRepository;
            _compeRepository = compeRepository;
            _pedRepository = pedRepository; 
            _itepeRepository = itepeRepository;
            _fenvRepository = fenvRepository;
            _fretRepository = fretRepository;
            _filRepository = filRepository;
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

        public List<CRM_PROPOSTA> GetAllPropostas(Int32 idAss)
        {
            return _proRepository.GetAllItens(idAss);
        }

        public List<CRM_PEDIDO_VENDA> GetAllPedidos(Int32 idAss)
        {
            return _pedRepository.GetAllItens(idAss);
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

        public CRM_PROPOSTA GetPropostaById(Int32 id)
        {
            return _proRepository.GetItemById(id);
        }

        public CRM_PEDIDO_VENDA GetPedidoById(Int32 id)
        {
            return _pedRepository.GetItemById(id);
        }

        public CRM_PEDIDO_VENDA GetPedidoByNumero(String num, Int32 idAss)
        {
            return _pedRepository.GetItemByNumero(num, idAss);
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

        public List<FORMA_ENVIO> GetAllFormasEnvio(Int32 idAss)
        {
            return _fenvRepository.GetAllItens(idAss);
        }

        public List<FORMA_FRETE> GetAllFormasFrete(Int32 idAss)
        {
            return _fretRepository.GetAllItens(idAss);
        }

        public List<TEMPLATE_PROPOSTA> GetAllTemplateProposta(Int32 id)
        {
            return _tpRepository.GetAllItens(id);
        }

        public TEMPLATE_PROPOSTA GetTemplateById(Int32 id)
        {
            return _tpRepository.GetItemById(id);
        }

        public List<TIPO_ACAO> GetAllTipoAcao(Int32 idAss)
        {
            return _taRepository.GetAllItens(idAss);
        }
        public List<CRM_ORIGEM> GetAllOrigens(Int32 idAss)
        {
            return _oriRepository.GetAllItens(idAss);
        }

        public List<FILIAL> GetAllFilial(Int32 idAss)
        {
            return _filRepository.GetAllItens(idAss);
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

        public CRM_PROPOSTA_ANEXO GetAnexoPropostaById(Int32 id)
        {
            return _aneprRepository.GetItemById(id);
        }

        public CRM_PEDIDO_VENDA_ANEXO GetAnexoPedidoById(Int32 id)
        {
            return _anepeRepository.GetItemById(id);
        }

        public CRM_COMENTARIO GetComentarioById(Int32 id)
        {
            return _comRepository.GetItemById(id);
        }

        public CRM_PROPOSTA_ACOMPANHAMENTO GetPropostaComentarioById(Int32 id)
        {
            return _pcRepository.GetItemById(id);
        }

        public CRM_PEDIDO_VENDA_ACOMPANHAMENTO GetPedidoComentarioById(Int32 id)
        {
            return _compeRepository.GetItemById(id);
        }

        public List<CRM> ExecuteFilter(Int32? status, DateTime? inicio, DateTime? final, Int32? origem, Int32? adic, String nome, String busca, Int32? estrela, Int32? temperatura, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(status, inicio, final, origem, adic, nome, busca, estrela, temperatura, idAss);

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
                    //item.USUARIO = null;
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

        public Int32 EditProposta(CRM_PROPOSTA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CRM_PROPOSTA obj = _proRepository.GetById(item.CRPR_CD_ID);
                    _proRepository.Detach(obj);
                    _proRepository.Update(item);
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

        public Int32 CreateProposta(CRM_PROPOSTA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _proRepository.Add(item);
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

        public Int32 EditPedido(CRM_PEDIDO_VENDA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.FILIAL = null;
                    CRM_PEDIDO_VENDA obj = _pedRepository.GetById(item.CRPV_CD_ID);
                    _pedRepository.Detach(obj);
                    _pedRepository.Update(item);
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

        public Int32 CreatePedido(CRM_PEDIDO_VENDA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _pedRepository.Add(item);
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

        public CRM_PEDIDO_VENDA_ITEM GetItemPedidoById(Int32 id)
        {
            CRM_PEDIDO_VENDA_ITEM item = _itepeRepository.GetItemById(id);
            return item;
        }

        public Int32 EditItemPedido(CRM_PEDIDO_VENDA_ITEM item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PRODUTO = null;
                    item.UNIDADE = null;    
                    CRM_PEDIDO_VENDA_ITEM obj = _itepeRepository.GetItemById(item.CRPI_CD_ID);
                    _itepeRepository.Detach(obj);
                    _itepeRepository.Update(item);
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

        public Int32 CreateItemPedido(CRM_PEDIDO_VENDA_ITEM item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _itepeRepository.Add(item);
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
