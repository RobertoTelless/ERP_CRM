using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Text.RegularExpressions;

namespace ApplicationServices.Services
{
    public class CRMAppService : AppServiceBase<CRM>, ICRMAppService
    {
        private readonly ICRMService _baseService;
        private readonly INotificacaoService _notiService;
        private readonly IClienteService _cliService;

        public CRMAppService(ICRMService baseService, INotificacaoService notiService, IClienteService cliService): base(baseService)
        {
            _baseService = baseService;
            _notiService = notiService;
            _cliService = cliService;
        }

        public List<CRM> GetAllItens(Int32 idAss)
        {
            List<CRM> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<CRM> GetAllItensAdm(Int32 idAss)
        {
            List<CRM> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public List<CRM> GetTarefaStatus(Int32 tipo, Int32 idAss)
        {
            List<CRM> lista = _baseService.GetTarefaStatus(tipo, idAss);
            return lista;
        }

        public List<CRM> GetByDate(DateTime data, Int32 idAss)
        {
            List<CRM> lista = _baseService.GetByDate(data, idAss);
            return lista;
        }

        public List<CRM> GetByUser(Int32 user)
        {
            List<CRM> lista = _baseService.GetByUser(user);
            return lista;
        }

        public CRM GetItemById(Int32 id)
        {
            CRM item = _baseService.GetItemById(id);
            return item;
        }

        public USUARIO GetUserById(Int32 id)
        {
            USUARIO item = _baseService.GetUserById(id);
            return item;
        }

        public CRM_CONTATO GetContatoById(Int32 id)
        {
            CRM_CONTATO lista = _baseService.GetContatoById(id);
            return lista;
        }

        public CRM_ACAO GetAcaoById(Int32 id)
        {
            CRM_ACAO lista = _baseService.GetAcaoById(id);
            return lista;
        }

        public CRM_PROPOSTA GetPropostaById(Int32 id)
        {
            CRM_PROPOSTA lista = _baseService.GetPropostaById(id);
            return lista;
        }

        public CRM_PEDIDO_VENDA GetPedidoById(Int32 id)
        {
            CRM_PEDIDO_VENDA lista = _baseService.GetPedidoById(id);
            return lista;
        }

        public CRM_PEDIDO_VENDA GetPedidoByNumero(String num, Int32 idAss)
        {
            CRM_PEDIDO_VENDA lista = _baseService.GetPedidoByNumero(num, idAss);
            return lista;
        }

        public CRM CheckExist(CRM tarefa, Int32 idUsu, Int32 idAss)
        {
            CRM item = _baseService.CheckExist(tarefa, idUsu, idAss);
            return item;
        }

        public List<TIPO_CRM> GetAllTipos()
        {
            List<TIPO_CRM> lista = _baseService.GetAllTipos();
            return lista;
        }

        public List<TEMPLATE_PROPOSTA> GetAllTemplateProposta(Int32 id)
        {
            List<TEMPLATE_PROPOSTA> lista = _baseService.GetAllTemplateProposta(id);
            return lista;
        }

        public TEMPLATE_PROPOSTA GetTemplateById(Int32 id)
        {
            TEMPLATE_PROPOSTA lista = _baseService.GetTemplateById(id);
            return lista;
        }

        public List<CRM_ACAO> GetAllAcoes(Int32 idAss)
        {
            List<CRM_ACAO> lista = _baseService.GetAllAcoes(idAss);
            return lista;
        }

        public List<CRM_PROPOSTA> GetAllPropostas(Int32 idAss)
        {
            List<CRM_PROPOSTA> lista = _baseService.GetAllPropostas(idAss);
            return lista;
        }

        public List<CRM_PEDIDO_VENDA> GetAllPedidos(Int32 idAss)
        {
            List<CRM_PEDIDO_VENDA> lista = _baseService.GetAllPedidos(idAss);
            return lista;
        }

        public List<TIPO_ACAO> GetAllTipoAcao(Int32 idAss)
        {
            List<TIPO_ACAO> lista = _baseService.GetAllTipoAcao(idAss);
            return lista;
        }

        public List<MOTIVO_CANCELAMENTO> GetAllMotivoCancelamento(Int32 idAss)
        {
            List<MOTIVO_CANCELAMENTO> lista = _baseService.GetAllMotivoCancelamento(idAss);
            return lista;
        }

        public List<MOTIVO_ENCERRAMENTO> GetAllMotivoEncerramento(Int32 idAss)
        {
            List<MOTIVO_ENCERRAMENTO> lista = _baseService.GetAllMotivoEncerramento(idAss);
            return lista;
        }

        public List<CRM_ORIGEM> GetAllOrigens(Int32 idAss)
        {
            List<CRM_ORIGEM> lista = _baseService.GetAllOrigens(idAss);
            return lista;
        }

        public List<FILIAL> GetAllFilial(Int32 idAss)
        {
            List<FILIAL> lista = _baseService.GetAllFilial(idAss);
            return lista;
        }

        public List<FORMA_ENVIO> GetAllFormasEnvio(Int32 idAss)
        {
            List<FORMA_ENVIO> lista = _baseService.GetAllFormasEnvio(idAss);
            return lista;
        }

        public List<FORMA_FRETE> GetAllFormasFrete(Int32 idAss)
        {
            List<FORMA_FRETE> lista = _baseService.GetAllFormasFrete(idAss);
            return lista;
        }

        public CRM_ANEXO GetAnexoById(Int32 id)
        {
            CRM_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public CRM_PROPOSTA_ANEXO GetAnexoPropostaById(Int32 id)
        {
            CRM_PROPOSTA_ANEXO lista = _baseService.GetAnexoPropostaById(id);
            return lista;
        }

        public CRM_PEDIDO_VENDA_ANEXO GetAnexoPedidoById(Int32 id)
        {
            CRM_PEDIDO_VENDA_ANEXO lista = _baseService.GetAnexoPedidoById(id);
            return lista;
        }

        public CRM_COMENTARIO GetComentarioById(Int32 id)
        {
            CRM_COMENTARIO lista = _baseService.GetComentarioById(id);
            return lista;
        }

        public CRM_PROPOSTA_ACOMPANHAMENTO GetPropostaComentarioById(Int32 id)
        {
            CRM_PROPOSTA_ACOMPANHAMENTO lista = _baseService.GetPropostaComentarioById(id);
            return lista;
        }

        public CRM_PEDIDO_VENDA_ACOMPANHAMENTO GetPedidoComentarioById(Int32 id)
        {
            CRM_PEDIDO_VENDA_ACOMPANHAMENTO lista = _baseService.GetPedidoComentarioById(id);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? status, DateTime? inicio, DateTime? final, Int32? origem, Int32? adic, String nome, String busca,  Int32? estrela, Int32? temperatura, Int32 idAss, out List<CRM> objeto)
        {
            try
            {
                objeto = new List<CRM>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(status, inicio, final, origem, adic, nome, busca, estrela, temperatura, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(CRM item, USUARIO usuario)
        {
            try
            {
                //Verifica Campos
                if (item.TIPO_CRM != null)
                {
                    item.TIPO_CRM = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }

                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.USUA_CD_ID, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.CRM1_IN_ATIVO = 1;

                // Serializa registro
                CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID);
                String serial = item.ASSI_CD_ID.ToString() + "|" + cli.CLIE_NM_NOME + "|" + item.CRM1_DS_DESCRICAO + "|" + item.CRM1_DT_CRIACAO.Value.ToShortDateString() + "|" + item.CRM1_IN_ATIVO.ToString() + "|" + item.CRM1_IN_STATUS.ToString() + "|" + item.CRM1_NM_NOME;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddCRM",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = serial
                };
                
                // Persiste
                Int32 volta = _baseService.Create(item, log);

                // Gera Notificação
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Atribuição de Processo CRM";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: O Processo CRM " + item.CRM1_NM_NOME + " do cliente " + cli.CLIE_NM_NOME + " foi colocado sob sua responsabilidade em "  + DateTime.Today.Date.ToLongDateString() + ".";
                noti.USUA_CD_ID = item.USUA_CD_ID.Value;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public Int32 ValidateEdit(CRM item, CRM itemAntes, USUARIO usuario)
        {
            try
            {
                // Verificação
                if (item.CRM1_DT_ENCERRAMENTO != null)
                {
                    if (item.CRM1_DT_ENCERRAMENTO < item.CRM1_DT_CRIACAO)
                    {
                        return 1;
                    }
                    if (item.CRM1_DT_ENCERRAMENTO > DateTime.Today.Date)
                    {
                        return 2;
                    }
                }
                if (item.CRM1_DT_CANCELAMENTO != null)
                {
                    if (item.CRM1_DT_CANCELAMENTO < item.CRM1_DT_CRIACAO)
                    {
                        return 3;
                    }
                    if (item.CRM1_DT_CANCELAMENTO > DateTime.Today.Date)
                    {
                        return 4;
                    }
                }

                // Serializa registro
                CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID);
                String serial = item.ASSI_CD_ID.ToString() + "|" + cli.CLIE_NM_NOME + "|" + item.CRM1_CD_ID.ToString() + "|" + item.CRM1_DS_DESCRICAO + "|" + item.CRM1_DT_CRIACAO.Value.ToShortDateString() + "|" + item.CRM1_IN_ATIVO.ToString() + "|" + item.CRM1_IN_STATUS.ToString() + "|" + item.CRM1_NM_NOME;
                String antes = itemAntes.ASSI_CD_ID.ToString() + "|" + cli.CLIE_NM_NOME + "|" + itemAntes.CRM1_CD_ID.ToString() + "|" + itemAntes.CRM1_DS_DESCRICAO + "|" + itemAntes.CRM1_DT_CRIACAO.Value.ToShortDateString() + "|" + itemAntes.CRM1_IN_ATIVO.ToString() + "|" + itemAntes.CRM1_IN_STATUS.ToString() + "|" + itemAntes.CRM1_NM_NOME;

                // Monta Log
                LOG log = new LOG();
                if (item.CRM1_DT_CANCELAMENTO != null)
                {
                    log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        LOG_NM_OPERACAO = "CancCRM",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = serial,
                        LOG_TX_REGISTRO_ANTES = antes
                    };
                }
                else
                {
                    log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        LOG_NM_OPERACAO = "EditCRM",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = serial,
                        LOG_TX_REGISTRO_ANTES = antes
                    };
                }

                // Persiste
                item.CLIENTE = null;
                item.CRM_ORIGEM = null;
                Int32 volta = _baseService.Edit(item, log);

                // Gera Notificação
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Alteração de Processo CRM";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: O Processo CRM " + item.CRM1_NM_NOME + " do cliente " + cli.CLIE_NM_NOME + " sob sua responsabilidade, foi alterado em " + DateTime.Today.Date.ToLongDateString() + ".";
                noti.USUA_CD_ID = usuario.USUA_CD_ID;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditSimples(CRM item, CRM itemAntes, USUARIO usuario)
        {
            try
            {
                // Verificação
                if (item.CRM1_DT_ENCERRAMENTO != null)
                {
                    if (item.CRM1_DT_ENCERRAMENTO < item.CRM1_DT_CRIACAO)
                    {
                        return 1;
                    }
                    if (item.CRM1_DT_ENCERRAMENTO > DateTime.Today.Date)
                    {
                        return 2;
                    }
                }
                if (item.CRM1_DT_CANCELAMENTO != null)
                {
                    if (item.CRM1_DT_CANCELAMENTO < item.CRM1_DT_CRIACAO)
                    {
                        return 3;
                    }
                    if (item.CRM1_DT_CANCELAMENTO > DateTime.Today.Date)
                    {
                        return 4;
                    }
                }

                // Serializa registro
                CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID);
                String serial = item.ASSI_CD_ID.ToString() + "|" + cli.CLIE_NM_NOME + "|" + item.CRM1_CD_ID.ToString() + "|" + item.CRM1_DS_DESCRICAO + "|" + item.CRM1_DT_CRIACAO.Value.ToShortDateString() + "|" + item.CRM1_IN_ATIVO.ToString() + "|" + item.CRM1_IN_STATUS.ToString() + "|" + item.CRM1_NM_NOME;
                String antes = itemAntes.ASSI_CD_ID.ToString() + "|" + cli.CLIE_NM_NOME + "|" + itemAntes.CRM1_CD_ID.ToString() + "|" + itemAntes.CRM1_DS_DESCRICAO + "|" + itemAntes.CRM1_DT_CRIACAO.Value.ToShortDateString() + "|" + itemAntes.CRM1_IN_ATIVO.ToString() + "|" + itemAntes.CRM1_IN_STATUS.ToString() + "|" + itemAntes.CRM1_NM_NOME;

                // Monta Log
                LOG log = new LOG();
                if (item.CRM1_DT_CANCELAMENTO != null)
                {
                    log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        LOG_NM_OPERACAO = "CancCRM",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = serial,
                        LOG_TX_REGISTRO_ANTES = antes
                    };
                }
                else
                {
                    log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        LOG_NM_OPERACAO = "EditCRM",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = serial,
                        LOG_TX_REGISTRO_ANTES = antes
                    };
                }

                // Persiste
                item.CLIENTE = null;
                item.CRM_ORIGEM = null;
                Int32 volta = _baseService.Edit(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CRM item, CRM itemAntes)
        {
            try
            {
                if (item.CRM1_DT_ENCERRAMENTO != null)
                {
                    if (item.CRM1_DT_ENCERRAMENTO < item.CRM1_DT_CRIACAO)
                    {
                        return 1;
                    }
                    if (item.CRM1_DT_ENCERRAMENTO > DateTime.Today.Date)
                    {
                        return 2;
                    }
                }
                if (item.CRM1_DT_CANCELAMENTO != null)
                {
                    if (item.CRM1_DT_CANCELAMENTO < item.CRM1_DT_CRIACAO)
                    {
                        return 3;
                    }
                    if (item.CRM1_DT_CANCELAMENTO > DateTime.Today.Date)
                    {
                        return 4;
                    }
                }

                // Persiste
                item.CLIENTE = null;
                item.CRM_ORIGEM = null;
                Int32 volta = _baseService.Edit(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CRM item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                // Acerta campos
                item.CRM1_IN_ATIVO = 2;

                // Verifica integridade
                List<CRM_ACAO> acao = item.CRM_ACAO.Where(p => p.CRAC_IN_STATUS == 1).ToList();
                if (acao.Count > 0)
                {
                    return 1;
                }

                // Serializa registro
                CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID);
                String serial = item.ASSI_CD_ID.ToString() + "|" + cli.CLIE_NM_NOME + "|" + item.CRM1_CD_ID.ToString() + "|" + item.CRM1_DS_DESCRICAO + "|" + item.CRM1_DT_CRIACAO.Value.ToShortDateString() + "|" + item.CRM1_IN_ATIVO.ToString() + "|" + item.CRM1_IN_STATUS.ToString() + "|" + item.CRM1_NM_NOME + "|" + item.CRM_ORIGEM.CROR_NM_NOME;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelCRM",
                    LOG_TX_REGISTRO = serial
                };

                // Persiste
                Int32 volta =  _baseService.Edit(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CRM item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CRM1_IN_ATIVO = 1;

                // Serializa registro
                CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID);
                String serial = item.ASSI_CD_ID.ToString() + "|" + cli.CLIE_NM_NOME + "|" + item.CRM1_CD_ID.ToString() + "|" + item.CRM1_DS_DESCRICAO + "|" + item.CRM1_DT_CRIACAO.Value.ToShortDateString() + "|" + item.CRM1_IN_ATIVO.ToString() + "|" + item.CRM1_IN_STATUS.ToString() + "|" + item.CRM1_NM_NOME + "|" + item.CRM_ORIGEM.CROR_NM_NOME;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCRM",
                    LOG_TX_REGISTRO = serial
                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditContato(CRM_CONTATO item)
        {
            try
            {
                // Persiste
                return _baseService.EditContato(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateContato(CRM_CONTATO item)
        {
            try
            {
                item.CRCO_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateContato(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditAcao(CRM_ACAO item)
        {
            try
            {
                // Persiste
                return _baseService.EditAcao(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateAcao(CRM_ACAO item, USUARIO usuario)
        {
            try
            {
                item.CRAC_IN_ATIVO = 1;

                // Recupera CRM
                CRM crm = _baseService.GetItemById(item.CRM1_CD_ID);

                // Persiste
                Int32 volta = _baseService.CreateAcao(item);

                // Gera Notificação
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Atribuição de Ação de Processo CRM";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: A Ação " + item.CRAC_NM_TITULO + " do processo CRM " + crm.CRM1_NM_NOME + " foi colocada sob sua responsabilidade em " + DateTime.Today.Date.ToLongDateString() + ".";
                noti.USUA_CD_ID = usuario.USUA_CD_ID;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditProposta(CRM_PROPOSTA item)
        {
            try
            {
                // Verificação
                item.CRPR_IN_STATUS = 1;

                // Persiste
                Int32 volta = _baseService.EditProposta(item);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditPedido(CRM_PEDIDO_VENDA item)
        {
            try
            {
                // Verificação

                // Persiste
                Int32 volta = _baseService.EditPedido(item);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCancelarProposta(CRM_PROPOSTA item)
        {
            try
            {
                // Verificação
                item.CRPR_IN_STATUS = 3;
                if (item.CRPR_DT_CANCELAMENTO == null)
                {
                    return 1;
                }
                if (item.CRPR_DS_CANCELAMENTO == null)
                {
                    return 2;
                }
                if (item.CRPR_DT_CANCELAMENTO < item.CRPR_DT_PROPOSTA)
                {
                    return 3;
                }
                if (item.CRPR_DT_CANCELAMENTO > DateTime.Today.Date)
                {
                    return 4;
                }

                // Persiste
                Int32 volta = _baseService.EditProposta(item);

                // Gera Notificação
                CRM crm = _baseService.GetItemById(item.CRM1_CD_ID);
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = item.ASSI_CD_ID.Value;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Cancelamento de Proposta";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: A Proposta " + item.CRPR_NR_NUMERO + " do processo CRM " + crm.CRM1_NM_NOME + " foi cancelada em " + item.CRPR_DT_CANCELAMENTO.Value.ToLongDateString() + ".";
                noti.USUA_CD_ID = item.USUA_CD_ID;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReprovarProposta(CRM_PROPOSTA item)
        {
            try
            {
                // Verificação
                item.CRPR_IN_STATUS = 4;
                if (item.CRPR_DT_REPROVACAO == null)
                {
                    return 1;
                }
                if (item.CRPR_DS_REPROVACAO == null)
                {
                    return 2;
                }
                if (item.CRPR_DT_REPROVACAO < item.CRPR_DT_PROPOSTA)
                {
                    return 3;
                }
                if (item.CRPR_DT_REPROVACAO > DateTime.Today.Date)
                {
                    return 4;
                }

                // Persiste
                Int32 volta = _baseService.EditProposta(item);

                // Gera Notificação
                CRM crm = _baseService.GetItemById(item.CRM1_CD_ID);
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = item.ASSI_CD_ID.Value;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Reprovação de Proposta";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: A Proposta " + item.CRPR_NR_NUMERO + " do processo CRM " + crm.CRM1_NM_NOME + " foi reprovada pelo cliente em " + item.CRPR_DT_REPROVACAO.Value.ToLongDateString() + ".";
                noti.USUA_CD_ID = item.USUA_CD_ID;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateAprovarProposta(CRM_PROPOSTA item)
        {
            try
            {
                // Verificação
                item.CRPR_IN_STATUS = 5;
                if (item.CRPR_DT_APROVACAO == null)
                {
                    return 1;
                }
                if (item.CRPR_DS_APROVACAO == null)
                {
                    return 2;
                }
                if (item.CRPR_DT_APROVACAO < item.CRPR_DT_PROPOSTA)
                {
                    return 3;
                }
                if (item.CRPR_DT_APROVACAO > DateTime.Today.Date)
                {
                    return 4;
                }

                // Persiste
                Int32 volta = _baseService.EditProposta(item);

                // Gera Notificação
                CRM crm = _baseService.GetItemById(item.CRM1_CD_ID);
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = item.ASSI_CD_ID.Value;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Aprovação de Proposta";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: A Proposta " + item.CRPR_NR_NUMERO + " do processo CRM " + crm.CRM1_NM_NOME + " foi aprovada pelo cliente em " + item.CRPR_DT_APROVACAO.Value.ToLongDateString() + ".";
                noti.USUA_CD_ID = item.USUA_CD_ID;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEnviarProposta(CRM_PROPOSTA item)
        {
            try
            {
                // Verificação
                item.CRPR_IN_STATUS = 2;
                if (item.CRPR_DT_ENVIO == null)
                {
                    return 1;
                }
                if (item.CRPR_DT_ENVIO < item.CRPR_DT_PROPOSTA)
                {
                    return 2;
                }
                if (item.CRPR_DT_ENVIO > DateTime.Today.Date)
                {
                    return 3;
                }
                if (item.TEPR_CD_ID == null)
                {
                    return 4;
                }
                if (item.CRPR_DS_ENVIO == null)
                {
                    return 5;
                }

                // Persiste
                Int32 volta = _baseService.EditProposta(item);

                // Gera Notificação
                CRM crm = _baseService.GetItemById(item.CRM1_CD_ID);
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = item.ASSI_CD_ID.Value;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Envio de Proposta";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: A Proposta " + item.CRPR_NR_NUMERO + " do processo CRM " + crm.CRM1_NM_NOME + " foi enviada para o cliente em " + item.CRPR_DT_ENVIO.Value.ToLongDateString() + ".";
                noti.USUA_CD_ID = item.USUA_CD_ID;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateProposta(CRM_PROPOSTA item)
        {
            try
            {
                item.CRPR_IN_ATIVO = 1;
                item.CRPR_IN_STATUS = 1;

                // Recupera CRM
                CRM crm = _baseService.GetItemById(item.CRM1_CD_ID);

                // Persiste
                Int32 volta = _baseService.CreateProposta(item);

                // Acerta processo
                crm.CRM1_IN_STATUS = 3;
                Int32 volta1 = _baseService.Edit(crm);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreatePedido(CRM_PEDIDO_VENDA item)
        {
            try
            {
                item.CRPV_IN_ATIVO = 1;
                item.CRPV_IN_STATUS = 1;

                // Recupera CRM
                CRM crm = _baseService.GetItemById(item.CRM1_CD_ID);

                // Persiste
                Int32 volta = _baseService.CreatePedido(item);

                // Acerta processo
                crm.CRM1_IN_STATUS = 3;
                Int32 volta1 = _baseService.Edit(crm);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public CRM_PEDIDO_VENDA_ITEM GetItemPedidoById(Int32 id)
        {
            CRM_PEDIDO_VENDA_ITEM lista = _baseService.GetItemPedidoById(id);
            return lista;
        }

        public Int32 ValidateCreateItemPedido(CRM_PEDIDO_VENDA_ITEM item)
        {
            item.CRPI_IN_ATIVO = 1;

            try
            {
                // Persiste
                Int32 volta = _baseService.CreateItemPedido(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditItemPedido(CRM_PEDIDO_VENDA_ITEM item)
        {
            try
            {
                // Persiste
                return _baseService.EditItemPedido(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
