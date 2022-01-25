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

namespace ApplicationServices.Services
{
    public class PedidoVendaAppService : AppServiceBase<PEDIDO_VENDA>, IPedidoVendaAppService
    {
        private readonly IPedidoVendaService _baseService;
        private readonly IMovimentoEstoqueProdutoService _movService;
        private readonly IUsuarioService _usuService;
        private readonly INotificacaoService _notiService;
        private readonly IConfiguracaoService _confService;
        private readonly IClienteService _cliService;
        private readonly IContaReceberService _cpService;
        private readonly ICentroCustoAppService _ccService;
        private readonly IContaBancariaService _cbService;
        private readonly IProdutoService _proService;
        private readonly IPeriodicidadeService _perService;
        private readonly IPedidoVendaParcelaService _pvpcService;

        public PedidoVendaAppService(IPedidoVendaService baseService, IMovimentoEstoqueProdutoService movService, IUsuarioService usuService, INotificacaoService notiService, IConfiguracaoService confService, IClienteService cliService, IContaReceberService cpService, ICentroCustoAppService ccService, IContaBancariaService cbService, IProdutoService proService, IPeriodicidadeService perService, IPedidoVendaParcelaService pvpcService) : base(baseService)
        {
            _baseService = baseService;
            _movService = movService;
            _usuService = usuService;
            _notiService = notiService;
            _confService = confService;
            _cliService = cliService;
            _cpService = cpService;
            _ccService = ccService;
            _cbService = cbService;
            _proService = proService;
            _perService = perService;
            _pvpcService = pvpcService;
        }

        public List<PEDIDO_VENDA> GetAllItens(Int32 idAss)
        {
            List<PEDIDO_VENDA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<PEDIDO_VENDA> GetAllItensAdm(Int32 idAss)
        {
            List<PEDIDO_VENDA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public List<PEDIDO_VENDA> GetAllItensAdmUser(Int32 id, Int32 idAss)
        {
            List<PEDIDO_VENDA> lista = _baseService.GetAllItensAdmUser(id, idAss);
            return lista;
        }

        public List<PEDIDO_VENDA> GetAtrasados(Int32 idAss)
        {
            List<PEDIDO_VENDA> lista = _baseService.GetAtrasados(idAss);
            return lista;
        }

        public List<PEDIDO_VENDA> GetEncerrados(Int32 idAss)
        {
            List<PEDIDO_VENDA> lista = _baseService.GetEncerrados(idAss);
            return lista;
        }

        public List<PEDIDO_VENDA> GetCancelados(Int32 idAss)
        {
            List<PEDIDO_VENDA> lista = _baseService.GetCancelados(idAss);
            return lista;
        }

        public PEDIDO_VENDA GetItemById(Int32 id)
        {
            PEDIDO_VENDA item = _baseService.GetItemById(id);
            return item;
        }

        public List<PEDIDO_VENDA> GetByUser(Int32 id)
        {
            List<PEDIDO_VENDA> item = _baseService.GetByUser(id);
            return item;
        }

        public PEDIDO_VENDA GetByNome(String nome, Int32 idAss)
        {
            PEDIDO_VENDA item = _baseService.GetByNome(nome, idAss);
            return item;
        }

        public PEDIDO_VENDA CheckExist(PEDIDO_VENDA conta, Int32 idAss)
        {
            PEDIDO_VENDA item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<FORMA_PAGAMENTO> GetAllFormas(Int32 idAss)
        {
            List<FORMA_PAGAMENTO> lista = _baseService.GetAllFormas(idAss);
            return lista;
        }

        public List<FORMA_ENVIO> GetAllFormaEnvio(Int32 idAss)
        {
            List<FORMA_ENVIO> lista = _baseService.GetAllFormaEnvio(idAss);
            return lista;
        }

        public List<FORMA_FRETE> GetAllFormaFrete(Int32 idAss)
        {
            List<FORMA_FRETE> lista = _baseService.GetAllFormaFrete(idAss);
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

        public PEDIDO_VENDA_ANEXO GetAnexoById(Int32 id)
        {
            PEDIDO_VENDA_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public ITEM_PEDIDO_VENDA GetItemVendaById(Int32 id)
        {
            ITEM_PEDIDO_VENDA lista = _baseService.GetItemVendaById(id);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? usuaId, String nome, String numero, DateTime? data, Int32? status, Int32 idAss, out List<PEDIDO_VENDA> objeto)
        {
            try
            {
                objeto = new List<PEDIDO_VENDA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(usuaId, nome, numero, data, status, idAss);
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

        public Int32 ValidateCreate(PEDIDO_VENDA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.PEVE_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                item.USUA_CD_ID = usuario.USUA_CD_ID;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddPEVE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PEDIDO_VENDA>(item)
                };

                // Persiste peido
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
                    noti.NOTI_NM_TITULO = "Aviso de Pedido de Venda";
                    noti.NOTI_TX_TEXTO = "O Pedido de Vemda " + item.PEVE_NM_NOME + " de número " + item.PEVE_NR_NUMERO + " está aguardando processamento";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);
                }

                if (item.PEVE_IN_PARCELAS == 1 && item.PEDIDO_VENDA_PARCELA.Count != 0)
                {
                    // Checa data
                    if (item.PEVE_DT_INICIO_PARCELAS < DateTime.Now.Date)
                    {
                        return 1;
                    }

                    // Verifica Num.Parcelas
                    if (item.PEVE_IN_NUMERO_PARCELAS < 2)
                    {
                        return 2;
                    }

                    // Acerta objeto
                    item.PEVE_IN_ATIVO = 1;
                    item.PEVE_IN_PARCELAS = 1;

                    // Gera Notificação
                    NOTIFICACAO noti3 = new NOTIFICACAO();
                    noti3.NOTI_DT_EMISSAO = DateTime.Today;
                    noti3.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    noti3.NOTI_IN_NIVEL = 1;
                    noti3.NOTI_IN_VISTA = 0;
                    noti3.NOTI_NM_TITULO = "Pedido de Venda - Parcelamento";
                    noti3.NOTI_IN_ATIVO = 1;
                    noti3.NOTI_TX_TEXTO = "O lançamento " + item.PEVE_CD_ID.ToString() + " foi parcelado em " + DateTime.Today.Date.ToLongDateString();
                    noti3.USUA_CD_ID = usuario.USUA_CD_ID;
                    noti3.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti3.CANO_CD_ID = 1;
                    noti3.NOTI_IN_STATUS = 0;

                    // Envia notificação
                    Int32 volta3 = _notiService.Create(noti3);

                    // Cria Parcelas
                    DateTime dataParcela = item.PEVE_DT_INICIO_PARCELAS.Value;
                    PERIODICIDADE period = _perService.GetItemById(item.PERI_CD_ID.Value);
                    if (dataParcela.Date <= DateTime.Today.Date)
                    {
                        dataParcela = dataParcela.AddMonths(1);
                    }

                    for (int i = 1; i <= item.PEVE_IN_NUMERO_PARCELAS; i++)
                    {
                        PEDIDO_VENDA_PARCELA parc = new PEDIDO_VENDA_PARCELA
                        {
                            PEVE_CD_ID = item.PEVE_CD_ID,
                            PVPC_DT_VENCIMENTO_PARCELA = dataParcela,
                            PVPC_IN_PARCELA = i,
                            PVPC_VL_PARCELA = item.PEVE_VL_VALOR / item.PEVE_IN_NUMERO_PARCELAS,
                            PVPC_IN_ATIVO = 1
                        };

                        Int32 voltaP = _pvpcService.Create(parc);

                        dataParcela = dataParcela.AddDays(period.PERI_NR_DIAS);
                    }
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PEDIDO_VENDA item, PEDIDO_VENDA itemAntes, USUARIO usuario)
        {
            try
            {
                if (item.ASSINANTE != null)
                { 
                    item.ASSINANTE = null; 
                }
                if (item.CENTRO_CUSTO != null)
                {
                    item.CENTRO_CUSTO = null; 
                }
                if (item.CLIENTE != null)
                {
                    item.CLIENTE = null; 
                }
                if (item.FILIAL != null)
                {
                    item.FILIAL = null; 
                }
                if (item.FORMA_PAGAMENTO != null)
                {
                    item.FORMA_PAGAMENTO = null; 
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null; 
                }
                if (item.FORMA_ENVIO != null)
                {
                    item.FORMA_ENVIO = null;
                }
                if (item.FORMA_FRETE != null)
                {
                    item.FORMA_FRETE = null;
                }

                // Acerta campos
                item.PEVE_DT_ALTERACAO = DateTime.Today.Date;

                if (item.PEVE_IN_PARCELAS == 1 && item.PEDIDO_VENDA_PARCELA.Count != 0)
                {
                    // Checa data
                    if (item.PEVE_DT_INICIO_PARCELAS < DateTime.Now.Date)
                    {
                        return 1;
                    }

                    // Verifica Num.Parcelas
                    if (item.PEVE_IN_NUMERO_PARCELAS < 2)
                    {
                        return 2;
                    }

                    // Acerta objeto
                    item.PEVE_IN_ATIVO = 1;
                    item.PEVE_IN_PARCELAS = 1;

                    // Gera Notificação
                    NOTIFICACAO noti3 = new NOTIFICACAO();
                    noti3.NOTI_DT_EMISSAO = DateTime.Today;
                    noti3.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    noti3.NOTI_IN_NIVEL = 1;
                    noti3.NOTI_IN_VISTA = 0;
                    noti3.NOTI_NM_TITULO = "Pedido de Venda - Parcelamento";
                    noti3.NOTI_IN_ATIVO = 1;
                    noti3.NOTI_TX_TEXTO = "O lançamento " + item.PEVE_CD_ID.ToString() + " foi parcelado em " + DateTime.Today.Date.ToLongDateString();
                    noti3.USUA_CD_ID = usuario.USUA_CD_ID;
                    noti3.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti3.CANO_CD_ID = 1;
                    noti3.NOTI_IN_STATUS = 0;

                    // Envia notificação
                    Int32 volta3 = _notiService.Create(noti3);

                    // Cria Parcelas
                    DateTime dataParcela = item.PEVE_DT_INICIO_PARCELAS.Value;
                    PERIODICIDADE period = _perService.GetItemById(item.PERI_CD_ID.Value);
                    if (dataParcela.Date <= DateTime.Today.Date)
                    {
                        dataParcela = dataParcela.AddMonths(1);
                    }

                    for (int i = 1; i <= item.PEVE_IN_NUMERO_PARCELAS; i++)
                    {
                        PEDIDO_VENDA_PARCELA parc = new PEDIDO_VENDA_PARCELA
                        {
                            PEVE_CD_ID = item.PEVE_CD_ID,
                            PVPC_DT_VENCIMENTO_PARCELA = dataParcela,
                            PVPC_IN_PARCELA = i,
                            PVPC_VL_PARCELA = item.PEVE_VL_VALOR / item.PEVE_IN_NUMERO_PARCELAS,
                            PVPC_IN_ATIVO = 1
                        };

                        Int32 voltaP = _pvpcService.Create(parc);

                        dataParcela = dataParcela.AddDays(period.PERI_NR_DIAS);
                    }
                }
                
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditPEVE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PEDIDO_VENDA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<PEDIDO_VENDA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PEDIDO_VENDA item, PEDIDO_VENDA itemAntes)
        {
            try
            {
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.CENTRO_CUSTO != null)
                {
                    item.CENTRO_CUSTO = null;
                }
                if (item.CLIENTE != null)
                {
                    item.CLIENTE = null;
                }
                if (item.FILIAL != null)
                {
                    item.FILIAL = null;
                }
                if (item.FORMA_PAGAMENTO != null)
                {
                    item.FORMA_PAGAMENTO = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.FORMA_ENVIO != null)
                {
                    item.FORMA_ENVIO = null;
                }
                if (item.FORMA_FRETE != null)
                {
                    item.FORMA_FRETE = null;
                }

                // Acerta campos
                item.PEVE_DT_ALTERACAO = DateTime.Today.Date;
                
                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(PEDIDO_VENDA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.ITEM_PEDIDO_VENDA.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.PEVE_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelPEVE",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PEDIDO_VENDA>(item)
                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);

                if (volta == 0)
                {
                    // Notifica responsavel
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
                    noti.NOTI_NM_TITULO = "Aviso de Pedido de Venda";
                    noti.NOTI_TX_TEXTO = "O Pedido de Venda " + item.PEVE_NM_NOME + " de número " + item.PEVE_NR_NUMERO + " foi excluído";

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


        public Int32 ValidateReativar(PEDIDO_VENDA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.PEVE_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatPEVE",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PEDIDO_VENDA>(item)
                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);

                if (volta == 0)
                {
                    // Notifica responsavel
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
                    noti.NOTI_NM_TITULO = "Aviso de Pedido de Venda";
                    noti.NOTI_TX_TEXTO = "O Pedido de Venda " + item.PEVE_NM_NOME + " de número " + item.PEVE_NR_NUMERO + " foi reativado";

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

        public Int32 ValidateEditItemVenda(ITEM_PEDIDO_VENDA item)
        {
            try
            {
                // Acerta valor no pedido

                
                
                
                
                
                // Persiste
                return _baseService.EditItemVenda(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateItemVenda(ITEM_PEDIDO_VENDA item)
        {
            try
            {
                // Completa Campos
                if (item.PROD_CD_ID != null)
                {
                    PRODUTO prod = _proService.GetItemById(item.PROD_CD_ID.Value);
                    item.UNID_CD_ID = prod.UNID_CD_ID;
                    if (item.PROD_VL_PRECO == null || item.PROD_VL_PRECO == 0)
                    {
                        item.PROD_VL_PRECO = prod.PRODUTO_TABELA_PRECO.Any(x => x.FILI_CD_ID == _baseService.GetItemById(item.PEVE_CD_ID).FILI_CD_ID) ? prod.PRODUTO_TABELA_PRECO.First(x => x.FILI_CD_ID == _baseService.GetItemById(item.PEVE_CD_ID).FILI_CD_ID).PRTP_VL_PRECO : 0;
                    }
                }

                // Persiste
                return _baseService.CreateItemVenda(item);
                
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEnvioAprovacao(PEDIDO_VENDA item, USUARIO usuario)
        {
            try
            {
                if (item.CLIENTE != null)
                {
                    item.CLIENTE = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.CENTRO_CUSTO != null)
                {
                    item.CENTRO_CUSTO = null;
                }
                if (item.FILIAL != null)
                {
                    item.FILIAL = null;
                }
                if (item.FORMA_PAGAMENTO != null)
                {
                    item.FORMA_PAGAMENTO = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.FORMA_ENVIO != null)
                {
                    item.FORMA_ENVIO = null;
                }
                if (item.FORMA_FRETE != null)
                {
                    item.FORMA_FRETE = null;
                }

                // Preparação
                PEDIDO_VENDA ped = _baseService.GetItemById(item.PEVE_CD_ID);

                // Verificação
                List<ITEM_PEDIDO_VENDA> ipc = ped.ITEM_PEDIDO_VENDA.Where(a => a.ITPE_QN_QUANTIDADE == 0).ToList();
                if (ipc.Count > 0)
                {
                    return 1;
                }

                // Recupera aprovador
                USUARIO aprov = _usuService.GetAprovador(usuario.ASSI_CD_ID);
                if (aprov == null)
                {
                    aprov = _usuService.GetAdministrador(usuario.ASSI_CD_ID);
                }

                item.PEVE_IN_STATUS = 7;
                Int32 volta = _baseService.Edit(item);

                // Notifica aprovador
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
                noti.NOTI_NM_TITULO = "Aviso de Pedido de Venda";
                noti.NOTI_TX_TEXTO = "O Pedido de Venda " + item.PEVE_NM_NOME + " de número " + item.PEVE_NR_NUMERO + " está sendo enviado para aprovação";

                // Persiste notificação 
                Int32 volta1 = _notiService.Create(noti);

                // Recupera template e-mail
                String header = _usuService.GetTemplate("ORCVENDA").TEMP_TX_CABECALHO;
                String body = _usuService.GetTemplate("ORCVENDA").TEMP_TX_CORPO;
                String data = _usuService.GetTemplate("ORCVENDA").TEMP_TX_DADOS;

                // Prepara dados do e-mail - header
                header = header.Replace("{NomeCliente}", _cliService.GetItemById(item.CLIE_CD_ID).CLIE_NM_NOME);

                // Prepara dados do e-mail - Body
                Decimal? valor = item.ITEM_PEDIDO_VENDA.Sum(a => (a.PRODUTO.PROD_VL_PRECO_VENDA * a.ITPE_QN_QUANTIDADE));

                body = body.Replace("{NumPedido}", item.PEVE_NR_NUMERO);
                body = body.Replace("{NomePedido}", item.PEVE_NM_NOME);
                body = body.Replace("{DataPedido}", item.PEVE_DT_DATA.ToShortDateString());
                if (item.PEVE_DT_VALIDADE != null)
                {
                    body = body.Replace("{DataValidade}", item.PEVE_DT_VALIDADE.Value.ToShortDateString());
                }
                else
                {
                    body = body.Replace("{DataValidade}", item.PEVE_DT_DATA.AddDays(30).ToShortDateString());
                }
                body = body.Replace("{DataPrevista}", item.PEVE_DT_PREVISTA.Value.ToShortDateString());

                // Prepara dados do e-mail - Dados
                data = data.Replace("{NumPedido}", item.PEVE_NR_NUMERO);
                data = data.Replace("{NomePedido}", item.PEVE_NM_NOME);
                data = data.Replace("{DataPedido}", item.PEVE_DT_DATA.ToShortDateString());
                data = data.Replace("{ValorTotal}", Formatters.DecimalFormatter(valor.Value));
                String itemPedido = String.Empty;
                foreach (var itemPed in item.ITEM_PEDIDO_VENDA)
                {
                    itemPedido += "Produto: " + itemPed.PRODUTO.PROD_NM_NOME + "<br />";
                    itemPedido += "Quantidade: " + itemPed.ITPE_QN_QUANTIDADE + "<br />";
                    itemPedido += "Unidade: " + itemPed.UNIDADE.UNID_NM_NOME+ "<br />";
                    //itemPedido += "Tipo de Impressão: " + itemPed.TIPO_IMPRESSAO.TIIM_NM_NOME+ "<br />";
                    //itemPedido += "Cor de Impressão: " + itemPed.COR_IMPRESSAO.COIM_NM_NOME + "<br />";
                    itemPedido += "==================================================" + "<br />";
                }
                data = data.Replace("{ItemPedido}", itemPedido);

                // Concatena
                String emailBody = header + body + data;

                // Monta e-mail
                Email mensagem = new Email();
                CONFIGURACAO conf = _usuService.CarregaConfiguracao(usuario.ASSI_CD_ID);
                mensagem.ASSUNTO = "Orçamento - Pedido Num: " + item.PEVE_NR_NUMERO;
                mensagem.CORPO = emailBody;
                mensagem.DEFAULT_CREDENTIALS = false;
                mensagem.EMAIL_DESTINO = _cliService.GetItemById(item.CLIE_CD_ID).CLIE_NM_EMAIL;
                mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                mensagem.ENABLE_SSL = true;
                mensagem.NOME_EMISSOR = "Sistema";
                mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                mensagem.SMTP = conf.CONF_NM_HOST_SMTP;

                // Envia e-mail
                Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReprovacao(PEDIDO_VENDA item, USUARIO usuario)
        {
            try
            {
                if (item.CLIENTE != null)
                {
                    item.CLIENTE = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.CENTRO_CUSTO != null)
                {
                    item.CENTRO_CUSTO = null;
                }
                if (item.FILIAL != null)
                {
                    item.FILIAL = null;
                }
                if (item.FORMA_PAGAMENTO != null)
                {
                    item.FORMA_PAGAMENTO = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.FORMA_ENVIO != null)
                {
                    item.FORMA_ENVIO = null;
                }
                if (item.FORMA_FRETE != null)
                {
                    item.FORMA_FRETE = null;
                }

                // Preparação
                PEDIDO_VENDA ped = _baseService.GetItemById(item.PEVE_CD_ID);

                // Acerta campos
                item.PEVE_IN_STATUS = 8;
                item.PEVE_DT_APROVACAO = DateTime.Today.Date;

                // Persiste
                Int32 volta = _baseService.Edit(item);
                if (volta == 0)
                {
                    // Notifica responsavel
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
                    noti.NOTI_NM_TITULO = "Aviso de Pedido de Venda";
                    noti.NOTI_TX_TEXTO = "O Pedido de Venda " + item.PEVE_NM_NOME + " de número " + item.PEVE_NR_NUMERO + " foi reprovado";

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

        public Int32 ValidateProcessamento(PEDIDO_VENDA item, USUARIO usuario)
        {
            try
            {
                if (item.CLIENTE != null)
                {
                    item.CLIENTE = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.CENTRO_CUSTO != null)
                {
                    item.CENTRO_CUSTO = null;
                }
                if (item.FILIAL != null)
                {
                    item.FILIAL = null;
                }
                if (item.FORMA_PAGAMENTO != null)
                {
                    item.FORMA_PAGAMENTO = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.FORMA_ENVIO != null)
                {
                    item.FORMA_ENVIO = null;
                }
                if (item.FORMA_FRETE != null)
                {
                    item.FORMA_FRETE = null;
                }

                // Preparação
                PEDIDO_VENDA ped = _baseService.GetItemById(item.PEVE_CD_ID);

                // Acerta campos
                item.PEVE_IN_STATUS = 10;
                item.PEVE_DT_APROVACAO = DateTime.Today.Date;

                // Persiste
                Int32 volta = _baseService.Edit(item);
                if (volta == 0)
                {
                    // Notifica responsavel
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
                    noti.NOTI_NM_TITULO = "Aviso de Pedido de Venda";
                    noti.NOTI_TX_TEXTO = "O Pedido de Venda " + item.PEVE_NM_NOME + " de número " + item.PEVE_NR_NUMERO + " foi processado";

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

        public Int32 ValidateFaturamento(PEDIDO_VENDA item, USUARIO usuario)
        {
            try
            {
                if (item.CLIENTE != null)
                {
                    item.CLIENTE = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.CENTRO_CUSTO != null)
                {
                    item.CENTRO_CUSTO = null;
                }
                if (item.FILIAL != null)
                {
                    item.FILIAL = null;
                }
                if (item.FORMA_PAGAMENTO != null)
                {
                    item.FORMA_PAGAMENTO = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.FORMA_ENVIO != null)
                {
                    item.FORMA_ENVIO = null;
                }
                if (item.FORMA_FRETE != null)
                {
                    item.FORMA_FRETE = null;
                }

                // Preparação
                PEDIDO_VENDA ped = _baseService.GetItemById(item.PEVE_CD_ID);

                // Acerta campos
                item.PEVE_IN_STATUS = 11;
                item.PEVE_DT_APROVACAO = DateTime.Today.Date;

                // Persiste
                Int32 volta = _baseService.Edit(item);
                if (volta == 0)
                {
                    // Notifica responsavel
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
                    noti.NOTI_NM_TITULO = "Aviso de Pedido de Venda";
                    noti.NOTI_TX_TEXTO = "O Pedido de Venda " + item.PEVE_NM_NOME + " de número " + item.PEVE_NR_NUMERO + " esta em faturamento";

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

        public Int32 ValidateExpedicao(PEDIDO_VENDA item, USUARIO usuario)
        {
            try
            {
                if (item.CLIENTE != null)
                {
                    item.CLIENTE = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.CENTRO_CUSTO != null)
                {
                    item.CENTRO_CUSTO = null;
                }
                if (item.FILIAL != null)
                {
                    item.FILIAL = null;
                }
                if (item.FORMA_PAGAMENTO != null)
                {
                    item.FORMA_PAGAMENTO = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.FORMA_ENVIO != null)
                {
                    item.FORMA_ENVIO = null;
                }
                if (item.FORMA_FRETE != null)
                {
                    item.FORMA_FRETE = null;
                }

                // Preparação
                PEDIDO_VENDA ped = _baseService.GetItemById(item.PEVE_CD_ID);

                // Acerta campos
                item.PEVE_IN_STATUS = 12;
                item.PEVE_DT_APROVACAO = DateTime.Today.Date;

                // Persiste
                Int32 volta = _baseService.Edit(item);
                if (volta == 0)
                {
                    // Notifica responsavel
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
                    noti.NOTI_NM_TITULO = "Aviso de Pedido de Venda";
                    noti.NOTI_TX_TEXTO = "O Pedido de Venda " + item.PEVE_NM_NOME + " de número " + item.PEVE_NR_NUMERO + " esta em expedição";

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

        public Int32 ValidateAprovacao(PEDIDO_VENDA item, USUARIO usuario)
        {
            try
            {
                if (item.CLIENTE != null)
                {
                    item.CLIENTE = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.CENTRO_CUSTO != null)
                {
                    item.CENTRO_CUSTO = null;
                }
                if (item.FILIAL != null)
                {
                    item.FILIAL = null;
                }
                if (item.FORMA_PAGAMENTO != null)
                {
                    item.FORMA_PAGAMENTO = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.FORMA_ENVIO != null)
                {
                    item.FORMA_ENVIO = null;
                }
                if (item.FORMA_FRETE != null)
                {
                    item.FORMA_FRETE = null;
                }

                // Preparação
                PEDIDO_VENDA ped = _baseService.GetItemById(item.PEVE_CD_ID);

                // Acerta campos
                item.PEVE_IN_STATUS = 9;
                item.PEVE_DT_APROVACAO = DateTime.Today.Date;

                // Persiste
                Int32 volta =  _baseService.Edit(item);
                if (volta == 0)
                {
                    // Notifica responsavel
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
                    noti.NOTI_NM_TITULO = "Aviso de Pedido de Venda";
                    noti.NOTI_TX_TEXTO = "O Pedido de Venda " + item.PEVE_NM_NOME + " de número " + item.PEVE_NR_NUMERO + " foi aprovado";

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

        public Int32 ValidateAprovacaoOportunidade(PEDIDO_VENDA item, USUARIO usuario)
        {
            try
            {
                if (item.CLIENTE != null)
                {
                    item.CLIENTE = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.CENTRO_CUSTO != null)
                {
                    item.CENTRO_CUSTO = null;
                }
                if (item.FILIAL != null)
                {
                    item.FILIAL = null;
                }
                if (item.FORMA_PAGAMENTO != null)
                {
                    item.FORMA_PAGAMENTO = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.FORMA_ENVIO != null)
                {
                    item.FORMA_ENVIO = null;
                }
                if (item.FORMA_FRETE != null)
                {
                    item.FORMA_FRETE = null;
                }

                // Preparação
                PEDIDO_VENDA ped = _baseService.GetItemById(item.PEVE_CD_ID);

                // Acerta campos
                item.PEVE_IN_STATUS = 3;
                item.PEVE_DT_APROVACAO = DateTime.Today.Date;

                // Persiste
                Int32 volta = _baseService.Edit(item);
                if (volta == 0)
                {
                    // Notifica responsavel
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
                    noti.NOTI_NM_TITULO = "Aviso de Oportunidade";
                    noti.NOTI_TX_TEXTO = "A Oportunidade " + item.PEVE_NM_NOME + " de número " + item.PEVE_NR_NUMERO + " foi aprovada";

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

        public Int32 ValidateAprovacaoProposta(PEDIDO_VENDA item, USUARIO usuario)
        {
            try
            {
                if (item.CLIENTE != null)
                {
                    item.CLIENTE = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.CENTRO_CUSTO != null)
                {
                    item.CENTRO_CUSTO = null;
                }
                if (item.FILIAL != null)
                {
                    item.FILIAL = null;
                }
                if (item.FORMA_PAGAMENTO != null)
                {
                    item.FORMA_PAGAMENTO = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.FORMA_ENVIO != null)
                {
                    item.FORMA_ENVIO = null;
                }
                if (item.FORMA_FRETE != null)
                {
                    item.FORMA_FRETE = null;
                }

                // Preparação
                PEDIDO_VENDA ped = _baseService.GetItemById(item.PEVE_CD_ID);

                // Acerta campos
                item.PEVE_IN_STATUS = 5;
                item.PEVE_DT_APROVACAO = DateTime.Today.Date;

                // Persiste
                Int32 volta = _baseService.Edit(item);
                if (volta == 0)
                {
                    // Notifica responsavel
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
                    noti.NOTI_NM_TITULO = "Aviso de Proposta";
                    noti.NOTI_TX_TEXTO = "A Proposta " + item.PEVE_NM_NOME + " de número " + item.PEVE_NR_NUMERO + " foi aprovada";

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

        public Int32 ValidateCancelamento(PEDIDO_VENDA item, USUARIO usuario)
        {
            try
            {
                if (item.CLIENTE != null)
                {
                    item.CLIENTE = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.CENTRO_CUSTO != null)
                {
                    item.CENTRO_CUSTO = null;
                }
                if (item.FILIAL != null)
                {
                    item.FILIAL = null;
                }
                if (item.FORMA_PAGAMENTO != null)
                {
                    item.FORMA_PAGAMENTO = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.FORMA_ENVIO != null)
                {
                    item.FORMA_ENVIO = null;
                }
                if (item.FORMA_FRETE != null)
                {
                    item.FORMA_FRETE = null;
                }

                // Critica
                if (String.IsNullOrEmpty(item.PEVE_DS_JUSTIFICATIVA))
                {
                    return 1;
                }                
                
                // Acerta campos
                item.PEVE_IN_STATUS = 6;
                item.PEVE_DT_CANCELAMENTO = DateTime.Today.Date;

                // Persiste
                Int32 volta = _baseService.Edit(item);
                if (volta == 0)
                {
                    // Notifica responsavel
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
                    noti.NOTI_NM_TITULO = "Aviso de Pedido de Venda";
                    noti.NOTI_TX_TEXTO = "O Pedido de Venda " + item.PEVE_NM_NOME + " de número " + item.PEVE_NR_NUMERO + " foi cancelado. Justificativa: " + item.PEVE_DS_JUSTIFICATIVA;

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

        public Int32 ValidateCancelamentoOportunidade(PEDIDO_VENDA item, USUARIO usuario)
        {
            try
            {
                if (item.CLIENTE != null)
                {
                    item.CLIENTE = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.CENTRO_CUSTO != null)
                {
                    item.CENTRO_CUSTO = null;
                }
                if (item.FILIAL != null)
                {
                    item.FILIAL = null;
                }
                if (item.FORMA_PAGAMENTO != null)
                {
                    item.FORMA_PAGAMENTO = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.FORMA_ENVIO != null)
                {
                    item.FORMA_ENVIO = null;
                }
                if (item.FORMA_FRETE != null)
                {
                    item.FORMA_FRETE = null;
                }

                // Critica
                if (String.IsNullOrEmpty(item.PEVE_DS_JUSTIFICATIVA))
                {
                    return 1;
                }

                // Acerta campos
                item.PEVE_IN_STATUS = 2;
                item.PEVE_DT_CANCELAMENTO = DateTime.Today.Date;

                // Persiste
                Int32 volta = _baseService.Edit(item);
                if (volta == 0)
                {
                    // Notifica responsavel
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
                    noti.NOTI_NM_TITULO = "Aviso de Oportunidade";
                    noti.NOTI_TX_TEXTO = "A Oportunidade " + item.PEVE_NM_NOME + " de número " + item.PEVE_NR_NUMERO + " foi cancelada. Justificativa: " + item.PEVE_DS_JUSTIFICATIVA;

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

        public Int32 ValidateCancelamentoProposta(PEDIDO_VENDA item, USUARIO usuario)
        {
            try
            {
                if (item.CLIENTE != null)
                {
                    item.CLIENTE = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.CENTRO_CUSTO != null)
                {
                    item.CENTRO_CUSTO = null;
                }
                if (item.FILIAL != null)
                {
                    item.FILIAL = null;
                }
                if (item.FORMA_PAGAMENTO != null)
                {
                    item.FORMA_PAGAMENTO = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.FORMA_ENVIO != null)
                {
                    item.FORMA_ENVIO = null;
                }
                if (item.FORMA_FRETE != null)
                {
                    item.FORMA_FRETE = null;
                }

                // Critica
                if (String.IsNullOrEmpty(item.PEVE_DS_JUSTIFICATIVA))
                {
                    return 1;
                }

                // Acerta campos
                item.PEVE_IN_STATUS = 4;
                item.PEVE_DT_CANCELAMENTO = DateTime.Today.Date;

                // Persiste
                Int32 volta = _baseService.Edit(item);
                if (volta == 0)
                {
                    // Notifica responsavel
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
                    noti.NOTI_NM_TITULO = "Aviso de Proposta";
                    noti.NOTI_TX_TEXTO = "A Proposta " + item.PEVE_NM_NOME + " de número " + item.PEVE_NR_NUMERO + " foi cancelada. Justificativa: " + item.PEVE_DS_JUSTIFICATIVA;

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

        public Int32 ValidateEncerramento(PEDIDO_VENDA item, USUARIO usuario)
        {
            try
            {
                if (item.CLIENTE != null)
                {
                    item.CLIENTE = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.CENTRO_CUSTO != null)
                {
                    item.CENTRO_CUSTO = null;
                }
                if (item.FILIAL != null)
                {
                    item.FILIAL = null;
                }
                if (item.FORMA_PAGAMENTO != null)
                {
                    item.FORMA_PAGAMENTO = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.FORMA_ENVIO != null)
                {
                    item.FORMA_ENVIO = null;
                }
                if (item.FORMA_FRETE != null)
                {
                    item.FORMA_FRETE = null;
                }

                // Preparação
                PEDIDO_VENDA ped = _baseService.GetItemById(item.PEVE_CD_ID);

                // Acerta campos
                item.PEVE_IN_STATUS = 13;
                item.PEVE_DT_FINAL = DateTime.Today.Date;
                item.FILIAL = null;
                item.PEDIDO_VENDA_ANEXO = null;
                item.USUARIO = null;
                item.CLIENTE = null;

                // Persiste
                Int32 volta = _baseService.Edit(item);

                // Acerta estoque
                ped = _baseService.GetItemById(item.PEVE_CD_ID);
                foreach (ITEM_PEDIDO_VENDA itpc in ped.ITEM_PEDIDO_VENDA)
                {
                    if (itpc.PROD_CD_ID != null)
                    {
                        MOVIMENTO_ESTOQUE_PRODUTO mov = new MOVIMENTO_ESTOQUE_PRODUTO();
                        mov.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        mov.FILI_CD_ID = item.FILI_CD_ID;
                        mov.MOEP_DT_MOVIMENTO = DateTime.Today.Date;
                        mov.MOEP_IN_ATIVO = 1;
                        mov.MOEP_IN_CHAVE_ORIGEM = 1;
                        mov.MOEP_IN_ORIGEM = "PROD";
                        mov.MOEP_IN_TIPO_MOVIMENTO = 2;
                        mov.MOEP_QN_QUANTIDADE = itpc.ITPE_QN_QUANTIDADE;
                        mov.PROD_CD_ID = itpc.PROD_CD_ID.Value;
                        mov.USUA_CD_ID = ped.USUA_CD_ID;
                        Int32 volta2 = _movService.Create(mov);

                        PRODUTO prod = _proService.GetItemById(itpc.PROD_CD_ID.Value);
                        prod.PROD_QN_ESTOQUE -= itpc.ITPE_QN_QUANTIDADE;
                        Int32 voltaProd = _proService.Edit(prod);
                    }
                }

                // Gera CR
                Decimal? valor = item.ITEM_PEDIDO_VENDA.Sum(a => (a.PRODUTO.PROD_VL_PRECO_VENDA * a.ITPE_QN_QUANTIDADE));

                CONTA_RECEBER cp = new CONTA_RECEBER();
                cp.ASSI_CD_ID = usuario.ASSI_CD_ID;
                cp.CARE_DS_DESCRICAO = "Lançamento a receber referente ao pedido de venda " + item.PEVE_NM_NOME + " de número " + item.PEVE_NR_NUMERO;
                cp.CARE_DT_COMPETENCIA = DateTime.Today.Date;
                cp.CARE_DT_LANCAMENTO = DateTime.Today.Date;
                cp.CARE_DT_VENCIMENTO = DateTime.Today.Date.AddDays(30);
                cp.CARE_IN_ATIVO = 1;
                cp.CARE_IN_LIQUIDADA = 0;
                cp.CARE_IN_PAGA_PARCIAL = 0;
                cp.CARE_IN_PARCELADA = 0;
                cp.CARE_IN_PARCELAS = 0;
                cp.CARE_IN_TIPO_LANCAMENTO = 1;
                cp.CARE_NR_DOCUMENTO = item.PEVE_NR_NUMERO;
                cp.CARE_VL_DESCONTO = 0;
                cp.CARE_VL_JUROS = 0;
                cp.CARE_VL_PARCELADO = 0;
                cp.CARE_VL_PARCIAL = 0;
                cp.CARE_VL_TAXAS = 0;
                cp.CARE_VL_VALOR_LIQUIDADO = 0;
                cp.CARE_VL_VALOR = valor.Value;
                cp.CARE_VL_SALDO = valor;
                cp.CECU_CD_ID = item.CECU_CD_ID;
                cp.FOPA_CD_ID = 1;
                cp.CLIE_CD_ID = item.CLIE_CD_ID;
                cp.PEVE_CD_ID = item.PEVE_CD_ID;
                cp.USUA_CD_ID = item.USUA_CD_ID;
                cp.COBA_CD_ID = _cbService.GetContaPadrao(usuario.ASSI_CD_ID).COBA_CD_ID;

                Int32 volta4 = _cpService.Create(cp);

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
                noti.NOTI_NM_TITULO = "Aviso de Pedido de Venda";
                noti.NOTI_TX_TEXTO = "O Pedido de Venda " + item.PEVE_NM_NOME + " de número " + item.PEVE_NR_NUMERO + " foi encerrado e os produtos estão baixados no estoque";

                // Persiste notificação 
                Int32 volta1 = _notiService.Create(noti);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 CreateResumoVenda(RESUMO_VENDA item, USUARIO usuario)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateResumoVenda(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 DeleteResumoVenda(RESUMO_VENDA item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.DeleteResumoVenda(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<RESUMO_VENDA> GetResumos(USUARIO usuario)
        {
            List<RESUMO_VENDA> lista = _baseService.GetResumos(usuario.ASSI_CD_ID);
            return lista;
        }
    }
}
