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
using System.Net;
using System.IO;
using System.Net.Mail;
using EntitiesServices.WorkClasses;
using System.Net.Http;

namespace ApplicationServices.Services
{
    public class PedidoCompraAppService : AppServiceBase<PEDIDO_COMPRA>, IPedidoCompraAppService
    {
        private readonly IPedidoCompraService _baseService;
        private readonly IMovimentoEstoqueProdutoService _movService;
        private readonly IUsuarioService _usuService;
        private readonly INotificacaoService _notiService;
        private readonly IConfiguracaoService _confService;
        private readonly IFornecedorService _fornService;
        private readonly IContaPagarService _cpService;
        private readonly ICentroCustoAppService _ccService;
        private readonly IContaBancariaService _cbService;
        private readonly IProdutoService _proService;
        private readonly IProdutoEstoqueFilialService _pefService;
        private readonly IProdutoTabelaPrecoService _ptpService;

        public PedidoCompraAppService(IPedidoCompraService baseService, IMovimentoEstoqueProdutoService movService, IUsuarioService usuService, INotificacaoService notiService, IConfiguracaoService confService, IFornecedorService fornService, IContaPagarService cpService, ICentroCustoAppService ccService, IContaBancariaService cbService, IProdutoService proService, IProdutoEstoqueFilialService pefService,IProdutoTabelaPrecoService ptpService) : base(baseService)
        {
            _baseService = baseService;
            _movService = movService;
            _usuService = usuService;
            _notiService = notiService;
            _confService = confService;
            _fornService = fornService;
            _cpService = cpService;
            _ccService = ccService;
            _cbService = cbService;
            _proService = proService;
            _pefService = pefService;
            _ptpService = ptpService;
        }

        public List<PEDIDO_COMPRA> GetAllItens(Int32 idAss)
        {
            List<PEDIDO_COMPRA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<PEDIDO_COMPRA> GetAllItensAdm(Int32 idAss)
        {
            List<PEDIDO_COMPRA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public List<PEDIDO_COMPRA> GetAllItensAdmUser(Int32 id, Int32 idAss)
        {
            List<PEDIDO_COMPRA> lista = _baseService.GetAllItensAdmUser(id, idAss);
            return lista;
        }

        public List<PEDIDO_COMPRA> GetAtrasados(Int32 idAss)
        {
            List<PEDIDO_COMPRA> lista = _baseService.GetAtrasados(idAss);
            return lista;
        }

        public List<PEDIDO_COMPRA> GetEncerrados(Int32 idAss)
        {
            List<PEDIDO_COMPRA> lista = _baseService.GetEncerrados(idAss);
            return lista;
        }

        public List<PEDIDO_COMPRA> GetCancelados(Int32 idAss)
        {
            List<PEDIDO_COMPRA> lista = _baseService.GetCancelados(idAss);
            return lista;
        }

        public PEDIDO_COMPRA GetItemById(Int32 id)
        {
            PEDIDO_COMPRA item = _baseService.GetItemById(id);
            return item;
        }

        public List<PEDIDO_COMPRA> GetByUser(Int32 id, Int32 idAss)
        {
            List<PEDIDO_COMPRA> item = _baseService.GetByUser(id, idAss);
            return item;
        }

        public PEDIDO_COMPRA GetByNome(String nome, Int32 idAss)
        {
            PEDIDO_COMPRA item = _baseService.GetByNome(nome, idAss);
            return item;
        }

        public PEDIDO_COMPRA CheckExist(PEDIDO_COMPRA conta, Int32 idAss)
        {
            PEDIDO_COMPRA item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<FORMA_PAGAMENTO> GetAllFormas(Int32 idAss)
        {
            List<FORMA_PAGAMENTO> lista = _baseService.GetAllFormas(idAss);
            return lista;
        }

        public List<UNIDADE> GetAllUnidades(Int32 idAss)
        {
            List<UNIDADE> lista = _baseService.GetAllUnidades(idAss);
            return lista;
        }

        public List<FILIAL> GetAllFilial(Int32 idAss)
        {
            List<FILIAL> lista = _baseService.GetAllFilial(idAss);
            return lista;
        }

        public PEDIDO_COMPRA_ANEXO GetAnexoById(Int32 id)
        {
            PEDIDO_COMPRA_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public ITEM_PEDIDO_COMPRA GetItemCompraById(Int32 id)
        {
            ITEM_PEDIDO_COMPRA lista = _baseService.GetItemCompraById(id);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? usuaId, String nome, String numero, String nf, DateTime? data, DateTime? dataPrevista, Int32? status, Int32 idAss, out List<PEDIDO_COMPRA> objeto)
        {
            try
            {
                objeto = new List<PEDIDO_COMPRA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(usuaId, nome, numero, nf, data, dataPrevista, status, idAss);
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

        public Int32 ExecuteFilterDash(String nmr, DateTime? dtFinal, String nome, Int32? usu, Int32? status, Int32 idAss, out List<PEDIDO_COMPRA> objeto)
        {
            try
            {
                objeto = new List<PEDIDO_COMPRA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterDash(nmr, dtFinal, nome, usu, status, idAss);
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

        public Int32 ValidateCreate(PEDIDO_COMPRA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.PECO_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                item.PECO_IN_STATUS = 1;
                item.USUA_CD_ID = usuario.USUA_CD_ID;               

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddPECO",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PEDIDO_COMPRA>(item)
                };

                // Persiste pedido
                Int32 volta = _baseService.Create(item, log);

                if (volta == 0)
                {
                    // Notifica comprador
                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = _usuService.GetComprador(usuario.ASSI_CD_ID).USUA_CD_ID;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Pedido de Compra";
                    noti.NOTI_TX_TEXTO = "O Pedido de Compra " + item.PECO_NM_NOME + " de número " + item.PECO_NR_NUMERO + " está aguardando processamento de cotação";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PEDIDO_COMPRA item, PEDIDO_COMPRA itemAntes, USUARIO usuario)
        {
            try
            {
                // Acerta campos
                item.PECO_DT_ALTERACAO = DateTime.Today.Date;
                
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditPECO",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PEDIDO_COMPRA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<PEDIDO_COMPRA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PEDIDO_COMPRA item, PEDIDO_COMPRA itemAntes)
        {
            try
            {
                // Acerta campos
                item.PECO_DT_ALTERACAO = DateTime.Today.Date;
                
                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(PEDIDO_COMPRA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.ITEM_PEDIDO_COMPRA.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.PECO_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelPECO",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PEDIDO_COMPRA>(item)
                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);

                if (volta == 0)
                {
                    // Recupera comprador
                    USUARIO aprov = _usuService.GetComprador(usuario.ASSI_CD_ID);
                    if (aprov == null)
                    {
                        aprov = _usuService.GetAdministrador(usuario.ASSI_CD_ID);
                    }

                    // Notifica comprador
                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = aprov.USUA_CD_ID;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Pedido de Compra";
                    noti.NOTI_TX_TEXTO = "O Pedido de Compra " + item.PECO_NM_NOME + " de número " + item.PECO_NR_NUMERO + " foi excluído";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateAcompanhamento(PEDIDO_COMPRA_ACOMPANHAMENTO item)
        {
            try
            {
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }

                item.PCAT_IN_ATIVO = 1;

                Int32 volta = _baseService.CreateAcompanhamento(item);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(PEDIDO_COMPRA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.PECO_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatPECO",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PEDIDO_COMPRA>(item)
                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);

                if (volta == 0)
                {
                    // Notifica comprador
                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = _usuService.GetComprador(usuario.ASSI_CD_ID).USUA_CD_ID;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Pedido de Compra";
                    noti.NOTI_TX_TEXTO = "O Pedido de Compra " + item.PECO_NM_NOME + " de número " + item.PECO_NR_NUMERO + " foi reativado";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditItemCompra(ITEM_PEDIDO_COMPRA item)
        {
            try
            {
                if (item.PEDIDO_COMPRA != null)
                {
                    item.PEDIDO_COMPRA = null;
                }
                if (item.PRODUTO != null)
                {
                    item.PRODUTO = null;
                }
                if (item.UNIDADE != null)
                {
                    item.UNIDADE = null;
                }

                // Persiste
                return _baseService.EditItemCompra(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDeleteItemCompra(ITEM_PEDIDO_COMPRA item)
        {
            try
            {
                if (item.PEDIDO_COMPRA != null)
                {
                    item.PEDIDO_COMPRA = null;
                }
                if (item.PRODUTO != null)
                {
                    item.PRODUTO = null;
                }
                if (item.UNIDADE != null)
                {
                    item.UNIDADE = null;
                }

                // Acerta campos
                item.ITPC_IN_ATIVO = 0;

                // Persiste
                return _baseService.EditItemCompra(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativarItemCompra(ITEM_PEDIDO_COMPRA item)
        {
            try
            {
                if (item.PEDIDO_COMPRA != null)
                {
                    item.PEDIDO_COMPRA = null;
                }
                if (item.PRODUTO != null)
                {
                    item.PRODUTO = null;
                }
                if (item.UNIDADE != null)
                {
                    item.UNIDADE = null;
                }

                // Acerta campos
                item.ITPC_IN_ATIVO = 1;

                // Persiste
                return _baseService.EditItemCompra(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateItemCompra(ITEM_PEDIDO_COMPRA item)
        {
            item.ITPC_IN_ATIVO = 1;

            try
            {
                // Persiste
                Int32 volta = _baseService.CreateItemCompra(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEnvioCotacao(PEDIDO_COMPRA item, List<AttachmentForn> anexo, String emailPersonalizado, USUARIO usuario, List<FORNECEDOR> listaForn)
        {
            try
            {
                // Preparação
                PEDIDO_COMPRA ped = _baseService.GetItemById(item.PECO_CD_ID);
                var lstFornecedores = listaForn;

                // Acerta campos
                item.PECO_IN_STATUS = 2;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelENCO",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PEDIDO_COMPRA>(item)
                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);
                ped = _baseService.GetItemById(item.PECO_CD_ID);

                if (volta == 0)
                {
                    // Notifica usuario
                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.USUA_CD_ID;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Pedido de Compra";
                    noti.NOTI_TX_TEXTO = "O Pedido de Compra " + item.PECO_NM_NOME + " de número " + item.PECO_NR_NUMERO + " está em cotação";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);

                    foreach (var f in lstFornecedores)
                    {
                        // Recupera fornecedor
                        FORNECEDOR forn = f;

                        // Recupera template e-mail
                        String header = _usuService.GetTemplate("COTFORN").TEMP_TX_CABECALHO;
                        String body = emailPersonalizado == "" || emailPersonalizado == null ? _usuService.GetTemplate("COTFORN").TEMP_TX_CORPO : emailPersonalizado;
                        String footer = _usuService.GetTemplate("COTFORN").TEMP_TX_DADOS;

                        //Prepara header
                        header = header.Replace("{NomeFornecedor}", forn.FORN_NM_NOME);

                        // Prepara corpo do e-mail  
                        String frase = String.Empty;
                        body = body.Replace("{Nome}", item.PECO_NM_NOME);
                        body = body.Replace("{Numero}", item.PECO_NR_NUMERO);
                        body = body.Replace("{Frase}", "");

                        //String table = String.Empty;

                        String table = "<table>"
                                + "<thead style=\"background-color:lightsteelblue\">"
                                + "<tr>"
                                + "<th style=\"width:30%\">Produto</th>"
                                + "<th style=\"width:60%\">Descrição</th>"
                                + "<th style=\"width: 10%;\">Quantidade</th>"
                                + "</tr>"
                                + "</thead>"
                                + "<tbody>";

                        String tableContent = String.Empty;

                        foreach (var pi in ped.ITEM_PEDIDO_COMPRA.Where(x => x.FORN_CD_ID == f.FORN_CD_ID).ToList<ITEM_PEDIDO_COMPRA>())
                        {
                            if (pi.ITPC_IN_TIPO == 1)
                            {
                                tableContent += "<tr>"
                                + "<td style=\"width:30%\">" + pi.PRODUTO.PROD_NM_NOME + "</td>"
                                + "<td style=\"width:60%\">" + pi.ITPC_TX_OBSERVACOES + "</td>"
                                + "<td style=\"width: 10%\">" + pi.ITPC_QN_QUANTIDADE + "</td>"
                                + "</tr>";
                            }
                        }
                        footer = table + tableContent + "</tbody>";

                        // Concatena
                        String emailBody = header + body + footer;
                        CONFIGURACAO conf = _confService.GetItemById(1);

                        // Gera emails
                        // Monta e-mail
                        NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                        Email mensagem = new Email();
                        mensagem.ASSUNTO = "Solicitação de Cotação";
                        mensagem.CORPO = emailBody;
                        mensagem.DEFAULT_CREDENTIALS = false;
                        mensagem.EMAIL_DESTINO = forn.FORN_NM_EMAIL;
                        mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                        mensagem.ENABLE_SSL = true;
                        mensagem.NOME_EMISSOR = _usuService.GetComprador(usuario.ASSI_CD_ID).USUA_NM_NOME;
                        mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                        mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                        mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                        mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                        mensagem.NETWORK_CREDENTIAL = net;
                        //mensagem.ATTACHMENT = new List<Attachment>();
                        //mensagem.ATTACHMENT.Add(anexo.First(x => x.FORN_CD_ID == f.FORN_CD_ID).ATTACHMENT);

                        // Envia mensagem
                        Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
                    }
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEnvioCotacao(PEDIDO_COMPRA item, String emailPersonalizado, USUARIO usuario)
        {
            try
            {
                // Preparação
                PEDIDO_COMPRA ped = _baseService.GetItemById(item.PECO_CD_ID);

                // Acerta campos
                item.PECO_IN_STATUS = 2;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ValENCO",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PEDIDO_COMPRA>(item)
                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);
                ped = _baseService.GetItemById(item.PECO_CD_ID);
                ped.FORNECEDOR = _fornService.GetById(item.FORN_CD_ID);

                if (volta == 0)
                {
                    // Notifica usuario
                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.USUA_CD_ID;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Pedido de Compra";
                    noti.NOTI_TX_TEXTO = "O Pedido de Compra " + item.PECO_NM_NOME + " de número " + item.PECO_NR_NUMERO + " está em cotação";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);

                    // Recupera template e-mail
                    String header = _usuService.GetTemplate("COTFORN").TEMP_TX_CABECALHO;
                    String body = emailPersonalizado == "" ? _usuService.GetTemplate("COTFORN").TEMP_TX_CORPO : emailPersonalizado;
                    String footer = _usuService.GetTemplate("COTFORN").TEMP_TX_DADOS;

                    //Prepara header
                    header = header.Replace("{NomeFornecedor}", ped.FORNECEDOR.FORN_NM_NOME);

                    // Prepara corpo do e-mail  
                    String frase = String.Empty;
                    body = body.Replace("{Nome}", item.PECO_NM_NOME);
                    body = body.Replace("{Numero}", item.PECO_NR_NUMERO);

                    String table = "<table>"
                            + "<thead style=\"background-color:lightsteelblue\">"
                            + "<tr>"
                            + "<th style=\"width:30%\">Produto</th>"
                            + "<th style=\"width:60%\">Descrição</th>"
                            + "<th style=\"width: 10%;\">Quantidade</th>"
                            + "</tr>"
                            + "</thead>"
                            + "<tbody>";

                    String tableContent = String.Empty;

                    //Prepara dados
                    foreach (var ipc in ped.ITEM_PEDIDO_COMPRA.Where(x => x.ITPC_IN_ATIVO == 1).ToList())
                    {
                        if (ipc.ITPC_IN_TIPO == 1)
                        {
                            tableContent += "<tr>"
                            + "<td style=\"width:30%\">" + ipc.PRODUTO.PROD_NM_NOME + "</td>"
                            + "<td style=\"width:60%\">" + ipc.ITPC_TX_OBSERVACOES + "</td>"
                            + "<td style=\"width: 10%\">" + ipc.ITPC_QN_QUANTIDADE + "</td>"
                            + "</tr>";
                        }
                    }

                    footer = table + tableContent + "</tbody>";

                    // Concatena
                    String emailBody = header + body + footer;
                    CONFIGURACAO conf = _confService.GetItemById(1);

                    // Gera emails
                    // Recupera fornecedor
                    FORNECEDOR forn = _fornService.GetItemById((Int32)ped.FORN_CD_ID);

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = "Solicitação de Cotação";
                    mensagem.CORPO = emailBody;
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_DESTINO = forn.FORN_NM_EMAIL;
                    mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                    mensagem.ENABLE_SSL = true;
                    mensagem.NOME_EMISSOR = _usuService.GetComprador(usuario.ASSI_CD_ID).USUA_NM_NOME;
                    mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                    mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                    mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                    mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                    mensagem.NETWORK_CREDENTIAL = net;

                    // Envia mensagem
                    Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCotacao(PEDIDO_COMPRA item, USUARIO usuario)
        {
            PEDIDO_COMPRA ped = _baseService.GetItemById(item.PECO_CD_ID);

            // Acerta campos
            item.PECO_IN_STATUS = 3;

            // Monta Log
            LOG log = new LOG
            {
                LOG_DT_DATA = DateTime.Now,
                USUA_CD_ID = usuario.USUA_CD_ID,
                ASSI_CD_ID = usuario.ASSI_CD_ID,
                LOG_IN_ATIVO = 1,
                LOG_NM_OPERACAO = "ValCOTA",
                LOG_TX_REGISTRO = Serialization.SerializeJSON<PEDIDO_COMPRA>(item)
            };

            // Persiste
            Int32 volta = _baseService.Edit(item, log);

            return 0;
        }

        public String ValidateCreateMensagem(FORNECEDOR item, USUARIO usuario, Int32? idAss)
        {
            try
            {
                FORNECEDOR forn = _fornService.GetById(item.FORN_CD_ID);

                // Verifica existencia prévia
                if (forn == null)
                {
                    return "1";
                }

                // Criticas
                if (forn.FORN_NM_TELEFONES == null)
                {
                    return "2";
                }

                // Monta token
                CONFIGURACAO conf = _confService.GetItemById(1);
                String text = conf.CONF_SG_LOGIN_SMS + ":" + conf.CONF_SG_SENHA_SMS;
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                String token = Convert.ToBase64String(textBytes);
                String auth = "Basic " + token;

                // Monta texto
                String texto = _usuService.GetTemplate("SMSCOTACAO").TEMP_TX_CORPO;
                texto = texto.Replace("{Fornecedor}", forn.FORN_NM_NOME);

                // inicia processo
                String resposta = String.Empty;

                // Monta destinatarios
                String listaDest = "55" + Regex.Replace(forn.FORN_NM_TELEFONES, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();

                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                httpWebRequest.Headers["Authorization"] = auth;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", texto, "\", \"from\": \"SystemBR\"}]}");

                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    resposta = result;
                }
                return resposta;
            }
            catch (Exception ex)
            {
                return "3";
            }
        }

        public Int32 ValidateEditItemCompraCotacao(ITEM_PEDIDO_COMPRA item)
        {
            try
            {
                // Persiste
                return _baseService.EditItemCompra(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEnvioAprovacao(PEDIDO_COMPRA item)
        {
            try
            {
                // Preparação
                PEDIDO_COMPRA ped = _baseService.GetItemById(item.PECO_CD_ID);

                // Acerta campos
                item.PECO_IN_STATUS = 4;

                // Recupera aprovador
                USUARIO aprov = _usuService.GetAprovador(item.ASSI_CD_ID);
                if (aprov == null)
                {
                    aprov = _usuService.GetAdministrador(item.ASSI_CD_ID);
                }

                // Persiste
                Int32 volta =  _baseService.Edit(item);
                if (volta == 0)
                {
                    // Notifica aprovador
                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = item.ASSI_CD_ID;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = aprov.USUA_CD_ID;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Pedido de Compra";
                    noti.NOTI_TX_TEXTO = "O Pedido de Compra " + item.PECO_NM_NOME + " de número " + item.PECO_NR_NUMERO + " está em aprovação";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateAprovacao(PEDIDO_COMPRA item)
        {
            try
            {
                // Preparação
                PEDIDO_COMPRA ped = _baseService.GetItemById(item.PECO_CD_ID);

                // Verificação
                List<ITEM_PEDIDO_COMPRA> ipc = ped.ITEM_PEDIDO_COMPRA.Where(a => a.ITPC_IN_ATIVO == 1).ToList();
                if (ipc.Where(a => a.ITPC_VL_PRECO_SELECIONADO == 0 || a.ITPC_VL_PRECO_SELECIONADO == null).Count() > 0)
                {
                    return 1;
                }
                if (ipc.Where(a => a.ITPC_NR_QUANTIDADE_REVISADA == 0).Count() > 0)
                {
                    return 2;
                }
                if (ipc.Where(a => a.ITPC_DT_COTACAO == null).Count() > 0)
                {
                    return 3;
                }

                // Acerta campos
                item.PECO_IN_STATUS = 5;
                item.PECO_DT_APROVACAO = DateTime.Today.Date;

                // Recupera comprador
                USUARIO aprov = _usuService.GetComprador(item.ASSI_CD_ID);
                if (aprov == null)
                {
                    aprov = _usuService.GetAdministrador(item.ASSI_CD_ID);
                }

                // Persiste
                Int32 volta =  _baseService.Edit(item);
                if (volta == 0)
                {
                    // Notifica comprador
                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = item.ASSI_CD_ID;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = aprov.USUA_CD_ID;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Pedido de Compra";
                    noti.NOTI_TX_TEXTO = "O Pedido de Compra " + item.PECO_NM_NOME + " de número " + item.PECO_NR_NUMERO + " foi aprovado";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);

                    // Notifica usuario
                    noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = item.ASSI_CD_ID;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.USUA_CD_ID;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Pedido de Compra";
                    noti.NOTI_TX_TEXTO = "O Pedido de Compra " + item.PECO_NM_NOME + " de número " + item.PECO_NR_NUMERO + " foi aprovado";

                    // Persiste notificação 
                    volta1 = _notiService.Create(noti);
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReprovacao(PEDIDO_COMPRA item)
        {
            try
            {
                // Preparação
                PEDIDO_COMPRA ped = _baseService.GetItemById(item.PECO_CD_ID);

                // Acerta campos
                item.PECO_IN_STATUS = 2;
                item.PECO_DT_APROVACAO = DateTime.Today.Date;

                // Recupera comprador
                USUARIO aprov = _usuService.GetComprador(item.ASSI_CD_ID);
                if (aprov == null)
                {
                    aprov = _usuService.GetAdministrador(item.ASSI_CD_ID);
                }

                // Persiste
                Int32 volta = _baseService.Edit(item);
                if (volta == 0)
                {
                    // Notifica comprador
                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = item.ASSI_CD_ID;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = aprov.USUA_CD_ID;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Pedido de Compra";
                    noti.NOTI_TX_TEXTO = "O Pedido de Compra " + item.PECO_NM_NOME + " de número " + item.PECO_NR_NUMERO + " foi reprovado";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);

                    // Notifica usuario
                    noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = item.ASSI_CD_ID;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.USUA_CD_ID;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Pedido de Compra";
                    noti.NOTI_TX_TEXTO = "O Pedido de Compra " + item.PECO_NM_NOME + " de número " + item.PECO_NR_NUMERO + " foi reprovado";

                    // Persiste notificação 
                    volta1 = _notiService.Create(noti);
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReceber(PEDIDO_COMPRA item)
        {
            try
            {
                // Preparação
                PEDIDO_COMPRA ped = _baseService.GetItemById(item.PECO_CD_ID);

                // Acerta campos
                item.PECO_IN_STATUS = 6;
                item.PECO_DT_FINAL = DateTime.Today.Date;

                // Persiste
                Int32 volta = _baseService.Edit(item);

                // Notifica usuario
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = item.ASSI_CD_ID;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_IN_STATUS = 1;
                noti.USUA_CD_ID = item.USUA_CD_ID;
                noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                noti.NOTI_IN_NIVEL = 1;
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Aviso de Pedido de Compra";
                noti.NOTI_TX_TEXTO = "O Pedido de Compra " + item.PECO_NM_NOME + " de número " + item.PECO_NR_NUMERO + " está com status A RECEBER";

                // Persiste notificação 
                Int32 volta1 = _notiService.Create(noti);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateRecebidoPorItem(PEDIDO_COMPRA item)
        {
            try
            {
                item.PECO_IN_STATUS = 7;
                item.PECO_DT_FINAL = DateTime.Today.Date;

                Int32 volta = _baseService.Edit(item);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateRecebido(PEDIDO_COMPRA item, USUARIO usuario)
        {
            try
            {
                PEDIDO_COMPRA ped = _baseService.GetItemById(item.PECO_CD_ID);

                // Acerta campos
                item.PECO_IN_STATUS = 7;
                item.PECO_DT_FINAL = DateTime.Today.Date;

                // Acerta estoque
                foreach (ITEM_PEDIDO_COMPRA itpc in ped.ITEM_PEDIDO_COMPRA.Where(x => x.ITPC_IN_ATIVO == 1 && x.ITPC_NR_QUANTIDADE_RECEBIDA == null).ToList())
                {
                    itpc.ITPC_NR_QUANTIDADE_RECEBIDA = itpc.ITPC_NR_QUANTIDADE_REVISADA;
                    if (itpc.ITPC_IN_TIPO == 1)
                    {
                        if (itpc.ITPC_NR_QUANTIDADE_RECEBIDA == null)
                        {
                            itpc.ITPC_NR_QUANTIDADE_RECEBIDA = itpc.ITPC_NR_QUANTIDADE_REVISADA == null ? itpc.ITPC_QN_QUANTIDADE : itpc.ITPC_NR_QUANTIDADE_REVISADA;
                        }

                        MOVIMENTO_ESTOQUE_PRODUTO mov = new MOVIMENTO_ESTOQUE_PRODUTO();
                        mov.ASSI_CD_ID = item.ASSI_CD_ID;
                        mov.FILI_CD_ID = item.FILI_CD_ID;
                        mov.MOEP_DT_MOVIMENTO = DateTime.Today.Date;
                        mov.MOEP_IN_ATIVO = 1;
                        mov.MOEP_IN_CHAVE_ORIGEM = 3;
                        mov.MOEP_IN_ORIGEM = "Compra";
                        mov.MOEP_IN_OPERACAO = 1;
                        mov.MOEP_IN_TIPO_MOVIMENTO = 0;
                        mov.MOEP_QN_QUANTIDADE = (Int32)itpc.ITPC_NR_QUANTIDADE_RECEBIDA;
                        mov.PROD_CD_ID = (Int32)itpc.PROD_CD_ID;
                        mov.USUA_CD_ID = ped.USUA_CD_ID;
                        Int32 volta2 = _movService.Create(mov);

                        PRODUTO_ESTOQUE_FILIAL pef = new PRODUTO_ESTOQUE_FILIAL();
                        pef.FILI_CD_ID = ped.FILI_CD_ID == null ? usuario.FILI_CD_ID.Value : (Int32)ped.FILI_CD_ID;
                        pef.PROD_CD_ID = (Int32)itpc.PROD_CD_ID;

                        if (_pefService.CheckExist(pef, item.ASSI_CD_ID) != null)
                        {
                            pef.PREF_CD_ID = _pefService.CheckExist(pef, item.ASSI_CD_ID).PREF_CD_ID;
                            pef.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                            pef.PREF_IN_ATIVO = 1;
                            if (_pefService.CheckExist(pef, item.ASSI_CD_ID).PREF_QN_ESTOQUE == null)
                            {
                                pef.PREF_QN_ESTOQUE = (Int32)itpc.ITPC_NR_QUANTIDADE_RECEBIDA;
                            }
                            else
                            {
                                pef.PREF_QN_ESTOQUE = _pefService.CheckExist(pef, item.ASSI_CD_ID).PREF_QN_ESTOQUE + (Int32)itpc.ITPC_NR_QUANTIDADE_RECEBIDA;
                            }

                            Int32 voltaPef = _pefService.Edit(pef);
                        }
                        else
                        {
                            pef.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                            pef.PREF_IN_ATIVO = 1;
                            pef.PREF_QN_ESTOQUE = (Int32)itpc.ITPC_NR_QUANTIDADE_RECEBIDA;

                            Int32 voltaPef = _pefService.Create(pef);
                        }

                        PRODUTO_TABELA_PRECO ptp = new PRODUTO_TABELA_PRECO();
                        ptp.PROD_CD_ID = (Int32)itpc.PROD_CD_ID;
                        ptp.FILI_CD_ID = ped.FILI_CD_ID;
                        PRODUTO_TABELA_PRECO ptpAntes = _ptpService.CheckExist(ptp);

                        if (ptpAntes == null)
                        {
                            ptp.PRTP_IN_ATIVO = 1;
                            ptp.PRTP_VL_CUSTO = itpc.ITPC_VL_PRECO_SELECIONADO;

                            Int32 voltaPtp = _ptpService.Create(ptp);
                        }
                        else
                        {
                            ptp.PRTP_IN_ATIVO = 1;
                            ptp.PRTP_VL_CUSTO = itpc.ITPC_VL_PRECO_SELECIONADO;
                            ptp.PRTP_VL_PRECO = ptpAntes.PRTP_VL_PRECO;
                            ptp.PRTP_VL_PRECO_PROMOCAO = ptp.PRTP_VL_PRECO_PROMOCAO;
                            ptp.PRTP_VL_DESCONTO_MAXIMO = ptpAntes.PRTP_VL_DESCONTO_MAXIMO;
                            ptp.PRTP_DT_DATA_REAJUSTE = ptpAntes.PRTP_DT_DATA_REAJUSTE;
                            ptp.PRTP_NR_MARKUP = ptpAntes.PRTP_NR_MARKUP;
                            ptp.PRTP_CD_ID = ptpAntes.PRTP_CD_ID;

                            Int32 voltaPtp = _ptpService.Edit(ptp);
                        }
                    }
                }

                // Persiste
                Int32 volta = _baseService.Edit(item);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateItemRecebido(ITEM_PEDIDO_COMPRA item, USUARIO usuario)
        {
            try
            {
                if (item.PRODUTO != null)
                {
                    item.PRODUTO = null;
                }
                if (item.UNIDADE != null)
                {
                    item.UNIDADE = null;
                }

                PEDIDO_COMPRA ped = GetItemById(item.PECO_CD_ID);
                ITEM_PEDIDO_COMPRA itpc = _baseService.GetItemCompraById(item.ITPC_CD_ID);

                // Acerta estoque
                if (itpc.ITPC_IN_TIPO == 1)
                {
                    MOVIMENTO_ESTOQUE_PRODUTO mov = new MOVIMENTO_ESTOQUE_PRODUTO();
                    mov.ASSI_CD_ID = item.PEDIDO_COMPRA.ASSI_CD_ID;
                    mov.FILI_CD_ID = ped.FILI_CD_ID;
                    mov.MOEP_DT_MOVIMENTO = DateTime.Today.Date;
                    mov.MOEP_IN_ATIVO = 1;
                    mov.MOEP_IN_CHAVE_ORIGEM = 3;
                    mov.MOEP_IN_OPERACAO = 1;
                    mov.MOEP_IN_TIPO_MOVIMENTO = 0;
                    mov.MOEP_QN_QUANTIDADE = (Int32)item.ITPC_NR_QUANTIDADE_RECEBIDA;
                    mov.PROD_CD_ID = (Int32)item.PROD_CD_ID;
                    mov.USUA_CD_ID = ped.USUA_CD_ID;
                    Int32 volta2 = _movService.Create(mov);

                    PRODUTO_ESTOQUE_FILIAL pef = new PRODUTO_ESTOQUE_FILIAL();
                    pef.FILI_CD_ID = ped.FILI_CD_ID == null ? usuario.FILI_CD_ID.Value : (Int32)ped.FILI_CD_ID;
                    pef.PROD_CD_ID = (Int32)item.PROD_CD_ID;

                    if (_pefService.CheckExist(pef, item.PEDIDO_COMPRA.ASSI_CD_ID) != null)
                    {
                        pef.PREF_CD_ID = _pefService.CheckExist(pef, item.PEDIDO_COMPRA.ASSI_CD_ID).PREF_CD_ID;
                        pef.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                        pef.PREF_IN_ATIVO = 1;
                        if (_pefService.CheckExist(pef, item.PEDIDO_COMPRA.ASSI_CD_ID).PREF_QN_ESTOQUE == null)
                        {
                            pef.PREF_QN_ESTOQUE = (Int32)item.ITPC_NR_QUANTIDADE_RECEBIDA;
                        }
                        else
                        {
                            pef.PREF_QN_ESTOQUE = _pefService.CheckExist(pef, item.PEDIDO_COMPRA.ASSI_CD_ID).PREF_QN_ESTOQUE + (Int32)item.ITPC_NR_QUANTIDADE_RECEBIDA;
                        }

                        Int32 voltaPef = _pefService.Edit(pef);
                    }
                    else
                    {
                        pef.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                        pef.PREF_IN_ATIVO = 1;
                        pef.PREF_QN_ESTOQUE = (Int32)item.ITPC_NR_QUANTIDADE_RECEBIDA;

                        Int32 voltaPef = _pefService.Create(pef);
                    }

                    PRODUTO_TABELA_PRECO ptp = new PRODUTO_TABELA_PRECO();
                    ptp.PROD_CD_ID = (Int32)itpc.PROD_CD_ID;
                    ptp.FILI_CD_ID = ped.FILI_CD_ID;
                    PRODUTO_TABELA_PRECO ptpAntes = _ptpService.CheckExist(ptp);

                    if (ptpAntes == null)
                    {
                        ptp.PRTP_IN_ATIVO = 1;
                        ptp.PRTP_VL_CUSTO = itpc.ITPC_VL_PRECO_SELECIONADO;

                        Int32 voltaPtp = _ptpService.Create(ptp);
                    }
                    else
                    {
                        ptp.PRTP_IN_ATIVO = 1;
                        ptp.PRTP_VL_CUSTO = itpc.ITPC_VL_PRECO_SELECIONADO;
                        ptp.PRTP_VL_PRECO = ptpAntes.PRTP_VL_PRECO;
                        ptp.PRTP_VL_PRECO_PROMOCAO = ptp.PRTP_VL_PRECO_PROMOCAO;
                        ptp.PRTP_VL_DESCONTO_MAXIMO = ptpAntes.PRTP_VL_DESCONTO_MAXIMO;
                        ptp.PRTP_DT_DATA_REAJUSTE = ptpAntes.PRTP_DT_DATA_REAJUSTE;
                        ptp.PRTP_NR_MARKUP = ptpAntes.PRTP_NR_MARKUP;
                        ptp.PRTP_CD_ID = ptpAntes.PRTP_CD_ID;

                        Int32 voltaPtp = _ptpService.Edit(ptp);
                    }
                }

                // Persiste
                Int32 volta = _baseService.EditItemCompra(item);
                Int32 conta = ped.ITEM_PEDIDO_COMPRA.Where(x => x.ITPC_IN_ATIVO == 1 && x.ITPC_NR_QUANTIDADE_RECEBIDA != null || x.ITPC_CD_ID == item.ITPC_CD_ID).Count();

                if (ped.ITEM_PEDIDO_COMPRA.Where(x => x.ITPC_IN_ATIVO == 1).Count() == conta)
                {
                    Int32 voltaItemR = ValidateRecebidoPorItem(ped);
                    return 2;
                }

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCancelamento(PEDIDO_COMPRA item)
        {
            try
            {
                // Critica
                if (String.IsNullOrEmpty(item.PECO_DS_JUSTIFICATIVA))
                {
                    return 1;
                }                
                
                // Acerta campos
                item.PECO_IN_STATUS = 8;
                item.PECO_DT_CANCELAMENTO = DateTime.Today.Date;

                // Recupera comprador
                USUARIO aprov = _usuService.GetComprador(item.ASSI_CD_ID);
                if (aprov == null)
                {
                    aprov = _usuService.GetAdministrador(item.ASSI_CD_ID);
                }

                // Persiste
                Int32 volta = _baseService.Edit(item);
                if (volta == 0)
                {
                    // Notifica comprador
                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = item.ASSI_CD_ID;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = aprov.USUA_CD_ID;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Pedido de Compra";
                    noti.NOTI_TX_TEXTO = "O Pedido de Compra " + item.PECO_NM_NOME + " de número " + item.PECO_NR_NUMERO + " foi cancelado. Justificativa: " + item.PECO_DS_JUSTIFICATIVA;

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);

                    // Notifica usuario
                    noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = item.ASSI_CD_ID;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.USUA_CD_ID;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Pedido de Compra";
                    noti.NOTI_TX_TEXTO = "O Pedido de Compra " + item.PECO_NM_NOME + " de número " + item.PECO_NR_NUMERO + " foi cancelado. Justificativa: " + item.PECO_DS_JUSTIFICATIVA;

                    // Persiste notificação 
                    volta1 = _notiService.Create(noti);
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
