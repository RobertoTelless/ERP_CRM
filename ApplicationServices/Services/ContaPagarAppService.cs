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

namespace ApplicationServices.Services
{
    public class ContaPagarAppService : AppServiceBase<CONTA_PAGAR>, IContaPagarAppService
    {
        private readonly IContaPagarService _baseService;
        private readonly INotificacaoService _notiService;
        private readonly IUsuarioService _usuService;
        private readonly IContaBancariaService _cbService;
        private readonly IPeriodicidadeService _perService;
        private readonly IContaPagarRateioService _ratService;
        private readonly IFormaPagamentoService _fpService;

        public ContaPagarAppService(IContaPagarService baseService, INotificacaoService notiService, IUsuarioService usuService, IContaBancariaService cbService, IPeriodicidadeService perService, IContaPagarRateioService ratService, IFormaPagamentoService fpService): base(baseService)
        {
            _baseService = baseService;
            _notiService = notiService;
            _usuService = usuService;
            _cbService = cbService;
            _perService = perService;
            _ratService = ratService;
            _fpService = fpService;
        }

        public List<CONTA_PAGAR> GetItensAtraso(Int32 idAss)
        {
            List<CONTA_PAGAR> lista = _baseService.GetItensAtraso(idAss);
            return lista;
        }

        public CONTA_PAGAR_PARCELA GetParcelaById(Int32 id)
        {
            CONTA_PAGAR_PARCELA lista = _baseService.GetParcelaById(id);
            return lista;
        }

        public List<CONTA_PAGAR> GetItensAtrasoFornecedor(Int32 idAss)
        {
            return _baseService.GetItensAtrasoFornecedor(idAss);
        }

        public List<CONTA_PAGAR> GetPagamentosMes(DateTime mes, Int32 idAss)
        {
            return _baseService.GetPagamentosMes(mes, idAss);
        }

        public List<CONTA_PAGAR> GetAPagarMes(DateTime mes, Int32 idAss)
        {
            return _baseService.GetAPagarMes(mes, idAss);
        }

        public CONTA_PAGAR_ANEXO GetAnexoById(Int32 id)
        {
            CONTA_PAGAR_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public CONTA_PAGAR GetItemById(Int32 id)
        {
            CONTA_PAGAR item = _baseService.GetItemById(id);
            return item;
        }

        public List<CONTA_PAGAR> GetAllItens(Int32 idAss)
        {
            return _baseService.GetAllItens(idAss);
        }

        public List<CONTA_PAGAR> GetAllItensAdm(Int32 idAss)
        {
            return _baseService.GetAllItensAdm(idAss);
        }

        public Decimal GetTotalPagoMes(DateTime mes, Int32 idAss)
        {
            return _baseService.GetTotalPagoMes(mes, idAss);
        }

        public Decimal GetTotalAPagarMes(DateTime mes, Int32 idAss)
        {
            return _baseService.GetTotalAPagarMes(mes, idAss);
        }
        
        public Int32 ExecuteFilter(Int32? forId, Int32? ccId, DateTime? data, String descricao, Int32? aberto, DateTime? vencimento, DateTime? vencFinal, DateTime? quitacao, Int32? atraso, Int32? conta, Int32 idAss, out List<CONTA_PAGAR> objeto)
        {
            try
            {
                objeto = new List<CONTA_PAGAR>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(forId, ccId, data, descricao, aberto, vencimento, vencFinal, quitacao, atraso, conta, idAss);
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

        public Int32 ExecuteFilterAtraso(String nome, DateTime? vencimento, Int32 idAss, out List<CONTA_PAGAR> objeto)
        {
            try
            {
                objeto = new List<CONTA_PAGAR>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterAtraso(nome, vencimento, idAss);
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

        public Int32 ValidateCreate(CONTA_PAGAR item, Int32 recorrente, DateTime? data, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.CAPA_IN_ATIVO = 1;
                item.CAPA_VL_DESCONTO = 0;
                item.CAPA_VL_JUROS = 0;
                item.CAPA_VL_PARCELADO = 0;
                item.CAPA_VL_PARCIAL = 0;
                item.CAPA_VL_SALDO = item.CAPA_VL_VALOR;
                item.CAPA_VL_TAXAS = 0;
                item.CAPA_VL_VALOR_PAGO = 0;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                if (item.CAPA_IN_PARCELAS > 1)
                {
                    item.CAPA_IN_PARCELADA = 1;
                    item.CAPA_VL_PARCELADO = item.CAPA_VL_VALOR;
                }
                else
                {
                    item.CAPA_IN_PARCELADA = 0;
                    item.CAPA_VL_PARCELADO = 0;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCAPA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CONTA_PAGAR>(item)
                };

                // Critica de parcelamento
                if (item.CAPA_DT_INICIO_PARCELAS != null)
                {
                    // Checa data
                    if (item.CAPA_DT_INICIO_PARCELAS < DateTime.Now.Date)
                    {
                        return 1;
                    }

                    // Verifica Num.Parcelas
                    if (item.CAPA_IN_PARCELAS < 2)
                    {
                        return 2;
                    }

                    // Verifica se é recorrente
                    if (recorrente > 0)
                    {
                        return 3;
                    }

                    // Acerta objeto
                    item.CAPA_IN_PARCELADA = 1;
                    item.CAPA_VL_PARCELADO = item.CAPA_VL_VALOR;
                }
                else
                {
                    if (item.CAPA_IN_PARCELAS > 1)
                    {
                        return 5;
                    }
                }

                // Pagamento recorrente
                if (recorrente > 0)
                {
                    if (item.CAPA_DT_INICIO_PARCELAS != null)
                    {
                        return 3;
                    }
                    if (data == null)
                    {
                        return 4;
                    }
                }

                //Gera lançamentos
                Int32 volta = 0;
                if (recorrente > 0)
                {
                    // Gera Notificação
                    NOTIFICACAO noti2 = new NOTIFICACAO();
                    noti2.NOTI_DT_EMISSAO = DateTime.Now;
                    noti2.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    noti2.NOTI_IN_NIVEL = 1;
                    noti2.NOTI_IN_VISTA = 0;
                    noti2.NOTI_NM_TITULO = "Contas a Pagar - Criação do Lançamento Recorrente";
                    noti2.NOTI_IN_ATIVO = 1;
                    noti2.NOTI_TX_TEXTO = "O lançamento recorrente " + item.CAPA_NR_DOCUMENTO + " foi criado em " + DateTime.Today.Date.ToLongDateString() + " com " + recorrente.ToString() +  " ocorrências,  sob sua responsabilidade.";
                    noti2.USUA_CD_ID = item.USUA_CD_ID.Value;
                    noti2.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti2.CANO_CD_ID = 1;
                    noti2.NOTI_IN_STATUS = 0;

                    // Persiste notificação
                    Int32 volta2 = _notiService.Create(noti2);

                    // Gera ocorrencias
                    DateTime dataParcela = data.Value;
                    if (dataParcela.Date <= DateTime.Today.Date)
                    {
                        dataParcela = dataParcela.AddMonths(1);
                    }

                    for (int i = 1; i <= recorrente; i++)
                    {
                        CONTA_PAGAR cp = new CONTA_PAGAR();
                        cp.CAPA_DS_DESCRICAO = item.CAPA_DS_DESCRICAO + " - Ocorrência " + i.ToString();
                        cp.CAPA_DT_COMPETENCIA = item.CAPA_DT_COMPETENCIA;
                        cp.CAPA_DT_INICIO_PARCELAS = item.CAPA_DT_INICIO_PARCELAS;
                        cp.CAPA_DT_LANCAMENTO = item.CAPA_DT_LANCAMENTO;
                        cp.CAPA_DT_LIQUIDACAO = item.CAPA_DT_LIQUIDACAO;
                        cp.CAPA_DT_VENCIMENTO = dataParcela;
                        cp.CAPA_IN_ABERTOS = item.CAPA_IN_ABERTOS;
                        cp.CAPA_IN_CHEQUE = item.CAPA_IN_CHEQUE;
                        cp.CAPA_IN_FECHADOS = item.CAPA_IN_FECHADOS;
                        cp.CAPA_IN_LIQUIDADA = item.CAPA_IN_LIQUIDADA;
                        cp.CAPA_IN_PAGA_PARCIAL = item.CAPA_IN_PAGA_PARCIAL;
                        cp.CAPA_IN_PARCELAS = item.CAPA_IN_PARCELAS;
                        cp.CAPA_IN_TIPO_LANCAMENTO = item.CAPA_IN_TIPO_LANCAMENTO;
                        cp.CAPA_NM_FAVORECIDO = item.CAPA_NM_FAVORECIDO;
                        cp.CAPA_NM_FORMA_PAGAMENTO = item.CAPA_NM_FORMA_PAGAMENTO;
                        cp.CAPA_NR_CHEQUE = item.CAPA_NR_CHEQUE;
                        cp.CAPA_NR_DOCUMENTO = item.CAPA_NR_DOCUMENTO;
                        cp.CAPA_TX_OBSERVACOES = item.CAPA_TX_OBSERVACOES;
                        cp.CECU_CD_ID = item.CECU_CD_ID;
                        cp.COBA_CD_ID = item.COBA_CD_ID;
                        cp.COBA_CD_ID_1 = item.COBA_CD_ID_1;
                        cp.FOPA_CD_ID = item.FOPA_CD_ID;
                        cp.FORN_CD_ID = item.FORN_CD_ID;
                        cp.PECO_CD_ID = item.PECO_CD_ID;
                        cp.PERI_CD_ID = item.PERI_CD_ID;
                        cp.USUA_CD_ID = item.USUA_CD_ID;
                        cp.CAPA_VL_VALOR = item.CAPA_VL_VALOR;
                        cp.CAPA_IN_ATIVO = 1;
                        cp.CAPA_VL_DESCONTO = 0;
                        cp.CAPA_VL_JUROS = 0;
                        cp.CAPA_VL_PARCELADO = item.CAPA_VL_VALOR;
                        cp.CAPA_VL_PARCIAL = 0;
                        cp.CAPA_VL_SALDO = item.CAPA_VL_VALOR;
                        cp.CAPA_VL_TAXAS = 0;
                        cp.CAPA_VL_VALOR_PAGO = 0;
                        cp.CAPA_IN_PARCELADA = 0;
                        cp.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        volta = _baseService.Create(cp);
                        dataParcela = dataParcela.AddDays(30);
                    }
                }
                else
                {
                    // Gera Notificação
                    NOTIFICACAO noti2 = new NOTIFICACAO();
                    noti2.NOTI_DT_EMISSAO = DateTime.Now;
                    noti2.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    noti2.NOTI_IN_NIVEL = 1;
                    noti2.NOTI_IN_VISTA = 0;
                    noti2.NOTI_NM_TITULO = "Contas a Pagar - Criação do Lançamento";
                    noti2.NOTI_IN_ATIVO = 1;
                    noti2.NOTI_TX_TEXTO = "O lançamento " + item.CAPA_NR_DOCUMENTO + " foi criado em " + DateTime.Today.Date.ToLongDateString() + " sob sua responsabilidade.";
                    noti2.USUA_CD_ID = item.USUA_CD_ID.Value;
                    noti2.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti2.CANO_CD_ID = 1;
                    noti2.NOTI_IN_STATUS = 0;

                    // Persiste Lancamento CP
                    volta = _baseService.Create(item);

                    // Persiste notificação
                    Int32 volta2 = _notiService.Create(noti2);

                    // Cria Parcelas
                    if (item.CAPA_IN_PARCELADA == 1)
                    {
                        CONTA_PAGAR rec = _baseService.GetItemById(item.CAPA_CD_ID);
                        DateTime dataParcela = rec.CAPA_DT_INICIO_PARCELAS.Value;
                        PERIODICIDADE period = _perService.GetItemById(item.PERI_CD_ID.Value);
                        if (dataParcela.Date <= DateTime.Today.Date)
                        {
                            dataParcela = dataParcela.AddMonths(1);
                        }

                        for (int i = 1; i <= rec.CAPA_IN_PARCELAS; i++)
                        {
                            CONTA_PAGAR_PARCELA parc = new CONTA_PAGAR_PARCELA();
                            parc.CAPA_CD_ID = item.CAPA_CD_ID;
                            parc.CPPA_DT_QUITACAO = null;
                            parc.CPPA_DT_VENCIMENTO = dataParcela;
                            parc.CPPA_IN_ATIVO = 1;
                            parc.CPPA_IN_QUITADA = 0;
                            parc.CPPA_NR_PARCELA = i.ToString() + "/" + item.CAPA_IN_PARCELAS.Value.ToString();
                            parc.CPPA_VL_VALOR_PAGO = 0;
                            parc.CPPA_VL_VALOR = item.CAPA_VL_PARCELADO / item.CAPA_IN_PARCELAS;
                            parc.CPPA_IN_PARCELA = i;
                            parc.CPPA_DS_DESCRICAO = rec.CAPA_DS_DESCRICAO;
                            rec.CONTA_PAGAR_PARCELA.Add(parc);
                            dataParcela = dataParcela.AddDays(period.PERI_NR_DIAS);
                        }
                        volta = _baseService.Edit(rec);
                    }
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateExpressa(CONTA_PAGAR item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.CAPA_IN_ATIVO = 1;
                item.CAPA_VL_DESCONTO = 0;
                item.CAPA_VL_JUROS = 0;
                item.CAPA_VL_PARCELADO = 0;
                item.CAPA_VL_PARCIAL = 0;
                item.CAPA_VL_SALDO = item.CAPA_VL_VALOR;
                item.CAPA_VL_TAXAS = 0;
                item.CAPA_VL_VALOR_PAGO = 0;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                if (item.CAPA_IN_PARCELAS > 1)
                {
                    item.CAPA_IN_PARCELADA = 1;
                    item.CAPA_VL_PARCELADO = item.CAPA_VL_VALOR;
                }
                else
                {
                    item.CAPA_IN_PARCELADA = 0;
                    item.CAPA_VL_PARCELADO = 0;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCAPA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CONTA_PAGAR>(item)
                };

                // Critica de parcelamento
                if (item.CAPA_DT_INICIO_PARCELAS != null)
                {
                    // Checa data
                    if (item.CAPA_DT_INICIO_PARCELAS < DateTime.Now.Date)
                    {
                        return 1;
                    }

                    // Verifica Num.Parcelas
                    if (item.CAPA_IN_PARCELAS < 2)
                    {
                        return 2;
                    }

                    // Acerta objeto
                    item.CAPA_IN_PARCELADA = 1;
                    item.CAPA_VL_PARCELADO = item.CAPA_VL_VALOR;
                }
                else
                {
                    if (item.CAPA_IN_PARCELAS > 1)
                    {
                        return 5;
                    }
                }

                //Gera lançamentos
                Int32 volta = 0;

                // Gera Notificação
                NOTIFICACAO noti2 = new NOTIFICACAO();
                noti2.NOTI_DT_EMISSAO = DateTime.Now;
                noti2.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti2.NOTI_IN_NIVEL = 1;
                noti2.NOTI_IN_VISTA = 0;
                noti2.NOTI_NM_TITULO = "Contas a Pagar - Criação do Lançamento";
                noti2.NOTI_IN_ATIVO = 1;
                noti2.NOTI_TX_TEXTO = "O lançamento " + item.CAPA_NR_DOCUMENTO + " foi criado em " + DateTime.Today.Date.ToLongDateString() + " sob sua responsabilidade.";
                noti2.USUA_CD_ID = item.USUA_CD_ID.Value;
                noti2.ASSI_CD_ID = usuario.ASSI_CD_ID;
                noti2.CANO_CD_ID = 1;
                noti2.NOTI_IN_STATUS = 0;

                // Persiste Lancamento CP
                volta = _baseService.Create(item);

                // Persiste notificação
                Int32 volta2 = _notiService.Create(noti2);

                // Cria Parcelas
                if (item.CAPA_IN_PARCELADA == 1)
                {
                    CONTA_PAGAR rec = _baseService.GetItemById(item.CAPA_CD_ID);
                    DateTime dataParcela = rec.CAPA_DT_INICIO_PARCELAS.Value;
                    PERIODICIDADE period = _perService.GetItemById(item.PERI_CD_ID.Value);
                    if (dataParcela.Date <= DateTime.Today.Date)
                    {
                        dataParcela = dataParcela.AddMonths(1);
                    }

                    for (int i = 1; i <= rec.CAPA_IN_PARCELAS; i++)
                    {
                        CONTA_PAGAR_PARCELA parc = new CONTA_PAGAR_PARCELA();
                        parc.CAPA_CD_ID = item.CAPA_CD_ID;
                        parc.CPPA_DT_QUITACAO = null;
                        parc.CPPA_DT_VENCIMENTO = dataParcela;
                        parc.CPPA_IN_ATIVO = 1;
                        parc.CPPA_IN_QUITADA = 0;
                        parc.CPPA_NR_PARCELA = i.ToString() + "/" + item.CAPA_IN_PARCELAS.Value.ToString();
                        parc.CPPA_VL_VALOR_PAGO = 0;
                        parc.CPPA_VL_VALOR = item.CAPA_VL_PARCELADO / item.CAPA_IN_PARCELAS;
                        parc.CPPA_IN_PARCELA = i;
                        parc.CPPA_DS_DESCRICAO = rec.CAPA_DS_DESCRICAO;
                        rec.CONTA_PAGAR_PARCELA.Add(parc);
                        dataParcela = dataParcela.AddDays(period.PERI_NR_DIAS);
                    }
                    volta = _baseService.Edit(rec);
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CONTA_PAGAR item, CONTA_PAGAR itemAntes, USUARIO usuario, Int32 liquida, Int32 eParcela)
        {
            try
            {
                //Verificação
                if (item.CENTRO_CUSTO != null)
                {
                    item.CENTRO_CUSTO = null;
                }
                if (item.CONTA_BANCO != null)
                {
                    item.CONTA_BANCO = null;
                }
                if (item.CONTA_BANCO1 != null)
                {
                    item.CONTA_BANCO1 = null;
                }
                if (item.FORMA_PAGAMENTO != null)
                {
                    item.FORMA_PAGAMENTO = null;
                }
                if (item.FORNECEDOR != null)
                {
                    item.FORNECEDOR = null;
                }
                //if (item.PEDIDO_COMPRA != null)
                //{
                //    item.PEDIDO_COMPRA = null;
                //}
                if (item.PERIODICIDADE != null)
                {
                    item.PERIODICIDADE = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }


                //  ****** Se for Liquidação
                if (liquida == 1)
                {
                    // Verifica se é parcelada
                    if (item.CAPA_IN_PARCELADA == 1)
                    {
                        return 8;
                    }                  
                    
                    // Checa data
                    if (item.CAPA_DT_LIQUIDACAO > DateTime.Now.Date)
                    {
                        return 1;
                    }

                    // Verifica Valor
                    Decimal soma = 0;
                    if (item.CAPA_IN_PAGA_PARCIAL == 0)
                    {
                        soma = item.CAPA_VL_VALOR.Value + item.CAPA_VL_TAXAS.Value + item.CAPA_VL_JUROS.Value - item.CAPA_VL_DESCONTO.Value;
                    }
                    else
                    {
                        soma = (item.CAPA_VL_SALDO == null ? 0 : item.CAPA_VL_SALDO.Value) + item.CAPA_VL_TAXAS.Value + item.CAPA_VL_JUROS.Value - item.CAPA_VL_DESCONTO.Value;
                    }
                    if (soma != item.CAPA_VL_VALOR_PAGO)
                    {
                        return 2;
                    }

                    // Acerta objeto
                    item.CAPA_IN_ATIVO = 1;
                    item.CAPA_IN_LIQUIDADA = 1;
                    item.CAPA_VL_SALDO = 0;
                    if (item.CAPA_IN_PAGA_PARCIAL == 1)
                    {
                        item.CAPA_VL_VALOR_PAGO += item.CAPA_VL_PARCIAL;
                    }

                    // Monta lançamento bancário
                    FORMA_PAGAMENTO forma = _fpService.GetItemById(item.FOPA_CD_ID.Value);
                    CONTA_BANCO conta = _cbService.GetItemById(forma.COBA_CD_ID.Value);
                    conta.COBA_VL_SALDO_ATUAL -= item.CAPA_VL_VALOR_PAGO;
                    CONTA_BANCO_LANCAMENTO lanc = new CONTA_BANCO_LANCAMENTO();
                    lanc.COBA_CD_ID = forma.COBA_CD_ID.Value;
                    lanc.CBLA_DS_DESCRICAO = item.CAPA_DS_DESCRICAO;
                    lanc.CBLA_DT_LANCAMENTO = item.CAPA_DT_LIQUIDACAO.Value;
                    lanc.CBLA_IN_ATIVO = 1;
                    lanc.CBLA_IN_ORIGEM = 0;
                    lanc.CBLA_IN_TIPO = 2;
                    lanc.CBLA_NR_NUMERO = item.CAPA_NR_DOCUMENTO;
                    if (item.CAPA_IN_PAGA_PARCIAL == 0)
                    {
                        lanc.CBLA_VL_VALOR = item.CAPA_VL_VALOR_PAGO.Value;
                    }
                    else
                    {
                        lanc.CBLA_VL_VALOR = item.CAPA_VL_SALDO.Value;
                    }
                    conta.CONTA_BANCO_LANCAMENTO.Add(lanc);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        LOG_NM_OPERACAO = "LiquiCAPA",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<CONTA_PAGAR>(item),
                    };

                    // Gera Notificação
                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.NOTI_DT_EMISSAO = DateTime.Now;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Contas a Pagar - Liquidação do Lançamento";
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_TX_TEXTO = "O lançamento " + item.CAPA_NR_DOCUMENTO + " foi liquidado em " + DateTime.Today.Date.ToLongDateString();
                    noti.USUA_CD_ID = usuario.USUA_CD_ID;
                    noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti.CANO_CD_ID = 1;
                    noti.NOTI_IN_STATUS = 0;

                    // Envia notificação
                    Int32 volta = _notiService.Create(noti);

                    // Persiste lancamento Bancario
                    volta = _cbService.Edit(conta);

                    // Persiste Lancamento CP
                    item.FORMA_PAGAMENTO = null;
                    volta = _baseService.Edit(item);

                    //SessionMocks.liquidaCP = 0;
                    return 0;
                }

                //  ****** Se for Pagamento parcial
                if (item.CAPA_IN_PAGA_PARCIAL == 1 && item.CAPA_VL_PARCIAL > 0)
                {
                    // Verifica se é parcelada
                    if (item.CAPA_IN_PARCELADA == 1)
                    {
                        return 9;
                    }
                    
                    // Checa data
                    if (item.CAPA_DT_LIQUIDACAO > DateTime.Now.Date)
                    {
                        return 1;
                    }

                    // Verifica se é parcelada
                    if (item.CAPA_IN_PARCELADA == 1)
                    {
                        return 3;
                    }

                    // Checa valor
                    if (item.CAPA_VL_PARCIAL > item.CAPA_VL_VALOR)
                    {
                        return 4;
                    }
                    if (item.CAPA_VL_PARCIAL == item.CAPA_VL_VALOR)
                    {
                        return 5;
                    }
                    
                    // Acerta objeto
                    item.CAPA_IN_ATIVO = 1;
                    item.CAPA_IN_PAGA_PARCIAL = 1;
                    item.CAPA_VL_SALDO = item.CAPA_VL_VALOR - item.CAPA_VL_PARCIAL.Value;
                    item.CAPA_VL_VALOR_PAGO = item.CAPA_VL_PARCIAL.Value;

                    // Monta lançamento bancário
                    FORMA_PAGAMENTO forma = _fpService.GetItemById(item.FOPA_CD_ID.Value);
                    CONTA_BANCO conta = _cbService.GetItemById(forma.COBA_CD_ID.Value);
                    conta.COBA_VL_SALDO_ATUAL -= item.CAPA_VL_PARCIAL;
                    CONTA_BANCO_LANCAMENTO lanc = new CONTA_BANCO_LANCAMENTO();
                    lanc.COBA_CD_ID = forma.COBA_CD_ID.Value;
                    lanc.CBLA_DS_DESCRICAO = "Pagamento Parcial - " + item.CAPA_DS_DESCRICAO;
                    lanc.CBLA_DT_LANCAMENTO = item.CAPA_DT_LIQUIDACAO.Value;
                    lanc.CBLA_IN_ATIVO = 1;
                    lanc.CBLA_IN_ORIGEM = 0;
                    lanc.CBLA_IN_TIPO = 1;
                    lanc.CBLA_NR_NUMERO = item.CAPA_NR_DOCUMENTO;
                    lanc.CBLA_VL_VALOR = item.CAPA_VL_PARCIAL.Value;
                    conta.CONTA_BANCO_LANCAMENTO.Add(lanc);

                    // Monta Log
                    LOG log1 = new LOG()
                    {
                        LOG_DT_DATA = DateTime.Now,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        LOG_NM_OPERACAO = "ParcCAPA",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<CONTA_PAGAR>(item),
                    };

                    // Gera Notificação
                    NOTIFICACAO noti1 = new NOTIFICACAO();
                    noti1.NOTI_DT_EMISSAO = DateTime.Now;
                    noti1.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    noti1.NOTI_IN_NIVEL = 1;
                    noti1.NOTI_IN_VISTA = 0;
                    noti1.NOTI_NM_TITULO = "Contas a Pagar - Pagamento Parcial";
                    noti1.NOTI_IN_ATIVO = 1;
                    noti1.NOTI_TX_TEXTO = "O lançamento " + item.CAPA_NR_DOCUMENTO + " foi parcialmente liquidado em " + DateTime.Today.Date.ToLongDateString();
                    noti1.USUA_CD_ID = usuario.USUA_CD_ID;
                    noti1.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti1.CANO_CD_ID = 1;
                    noti1.NOTI_IN_STATUS = 0;

                    // Envia notificação
                    Int32 volta1 = _notiService.Create(noti1);

                    // Persiste lancamento Bancario
                    volta1 = _cbService.Edit(conta);

                    // Persiste Lancamento CR
                    volta1 = _baseService.Edit(item);

                    // Grava parcela
                    CONTA_PAGAR rec = _baseService.GetItemById(item.CAPA_CD_ID);
                    DateTime dataParcela = DateTime.Today.Date;
                    CONTA_PAGAR_PARCELA parc = new CONTA_PAGAR_PARCELA();
                    parc.CAPA_CD_ID = item.CAPA_CD_ID;
                    parc.CPPA_DT_QUITACAO = DateTime.Today.Date;
                    parc.CPPA_DT_VENCIMENTO = dataParcela;
                    parc.CPPA_IN_ATIVO = 1;
                    parc.CPPA_IN_QUITADA = 1;
                    parc.CPPA_NR_PARCELA = "1/1";
                    parc.CPPA_VL_VALOR_PAGO = rec.CAPA_VL_PARCIAL;
                    parc.CPPA_VL_VALOR = rec.CAPA_VL_PARCIAL;
                    parc.CPPA_NR_PARCELA = "1";
                    parc.CPPA_DS_DESCRICAO = rec.CAPA_DS_DESCRICAO;
                    rec.CONTA_PAGAR_PARCELA.Add(parc);
                    volta1 = _baseService.Edit(rec);
                    return 0;
                }

                //  ****** Se for Parcelamento
                if (item.CAPA_IN_PARCELADA == 0 && item.CAPA_DT_INICIO_PARCELAS !=  null)
                {
                    // Veriica se é parcial
                    if (item.CAPA_IN_PAGA_PARCIAL == 1)
                    {
                        return 10;
                    }
                    
                    // Checa data
                    if (item.CAPA_DT_INICIO_PARCELAS < DateTime.Now.Date)
                    {
                        return 6;
                    }

                    // Verifica Num.Parcelas
                    if (item.CAPA_IN_PARCELAS < 2)
                    {
                        return 7;
                    }

                    // Acerta objeto
                    item.CAPA_IN_ATIVO = 1;
                    item.CAPA_IN_PARCELADA = 1;

                    // Monta Log
                    //LOG log3 = new LOG
                    //{
                    //    LOG_DT_DATA = DateTime.Now,
                    //    USUA_CD_ID = usuario.USUA_CD_ID,
                    //    ASSI_CD_ID = SessionMocks.IdAssinante,
                    //    LOG_NM_OPERACAO = "ParceCARE",
                    //    LOG_IN_ATIVO = 1,
                    //    LOG_TX_REGISTRO = Serialization.SerializeJSON<CONTA_RECEBER>(item),
                    //    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CONTA_RECEBER>(itemAntes)
                    //};

                    // Gera Notificação
                    NOTIFICACAO noti3 = new NOTIFICACAO();
                    noti3.NOTI_DT_EMISSAO = DateTime.Now;
                    noti3.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    noti3.NOTI_IN_NIVEL = 1;
                    noti3.NOTI_IN_VISTA = 0;
                    noti3.NOTI_NM_TITULO = "Contas a Pagar - Parcelamento";
                    noti3.NOTI_IN_ATIVO = 1;
                    noti3.NOTI_TX_TEXTO = "O lançamento " + item.CAPA_NR_DOCUMENTO + " foi parcelado em " + DateTime.Today.Date.ToLongDateString();
                    noti3.USUA_CD_ID = usuario.USUA_CD_ID;
                    noti3.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti3.CANO_CD_ID = 1;
                    noti3.NOTI_IN_STATUS = 0;

                    // Envia notificação
                    Int32 volta3 = _notiService.Create(noti3);

                    // Persiste Lancamento CP
                    volta3 =  _baseService.Edit(item);

                    // Cria Parcelas
                    CONTA_PAGAR rec = _baseService.GetItemById(item.CAPA_CD_ID);
                    DateTime dataParcela = rec.CAPA_DT_INICIO_PARCELAS.Value;
                    PERIODICIDADE period = _perService.GetItemById(item.PERI_CD_ID.Value);
                    if (dataParcela.Date <= DateTime.Today.Date)
                    {
                        dataParcela = dataParcela.AddMonths(1);
                    }

                    for (int i = 1; i <= rec.CAPA_IN_PARCELAS; i++)
                    {
                        CONTA_PAGAR_PARCELA parc = new CONTA_PAGAR_PARCELA();
                        parc.CAPA_CD_ID = item.CAPA_CD_ID;
                        parc.CPPA_DT_QUITACAO = null;
                        parc.CPPA_DT_VENCIMENTO = dataParcela;
                        parc.CPPA_IN_ATIVO = 1;
                        parc.CPPA_IN_QUITADA = 0;
                        parc.CPPA_NR_PARCELA = i.ToString() + "/" + item.CAPA_IN_PARCELAS.Value.ToString();
                        parc.CPPA_VL_VALOR_PAGO = 0;
                        parc.CPPA_VL_VALOR = item.CAPA_VL_PARCELADO / item.CAPA_IN_PARCELAS;
                        parc.CPPA_IN_PARCELA = i;
                        parc.CPPA_DS_DESCRICAO = rec.CAPA_DS_DESCRICAO;
                        rec.CONTA_PAGAR_PARCELA.Add(parc);
                        dataParcela = dataParcela.AddDays(period.PERI_NR_DIAS);
                    }
                    volta3 = _baseService.Edit(rec);
                    return 0;
                }

                // ***** Se for Alteração comum
                // Acerta objeto
                item.CAPA_IN_ATIVO = 1;
                if (eParcela == 0)
                {
                    item.CAPA_VL_SALDO = item.CAPA_VL_VALOR;
                }

                // Monta Log
                //LOG log2 = new LOG
                //{
                //    LOG_DT_DATA = DateTime.Now,
                //    USUA_CD_ID = usuario.USUA_CD_ID,
                //    ASSI_CD_ID = SessionMocks.IdAssinante,
                //    LOG_NM_OPERACAO = "EditCARE",
                //    LOG_IN_ATIVO = 1,
                //    LOG_TX_REGISTRO = Serialization.SerializeJSON<CONTA_RECEBER>(item),
                //    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CONTA_RECEBER>(itemAntes)
                //};

                // Gera Notificação
                NOTIFICACAO noti2 = new NOTIFICACAO();
                noti2.NOTI_DT_EMISSAO = DateTime.Now;
                noti2.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti2.NOTI_IN_NIVEL = 1;
                noti2.NOTI_IN_VISTA = 0;
                noti2.NOTI_NM_TITULO = "Contas a Pagar - Alteração do Lançamento";
                noti2.NOTI_IN_ATIVO = 1;
                noti2.NOTI_TX_TEXTO = "O lançamento " + item.CAPA_NR_DOCUMENTO + " foi alterado em " + DateTime.Today.Date.ToLongDateString();
                noti2.USUA_CD_ID = usuario.USUA_CD_ID;
                noti2.ASSI_CD_ID = usuario.ASSI_CD_ID;
                noti2.CANO_CD_ID = 1;
                noti2.NOTI_IN_STATUS = 0;

                // Persiste notificação
                Int32 volta2 = _notiService.Create(noti2);

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditPagar(CONTA_PAGAR item, CONTA_PAGAR itemAntes, USUARIO usuario)
        {
            try
            {
                // Acerto do objeto
                item.CAPA_IN_LIQUIDADA = 1;

                // gera lancamento conta bancaria
                FORMA_PAGAMENTO forma = _fpService.GetItemById(item.FOPA_CD_ID.Value);
                CONTA_BANCO conta = _cbService.GetItemById(forma.COBA_CD_ID.Value);

                CONTA_BANCO_LANCAMENTO lanc = new CONTA_BANCO_LANCAMENTO();
                lanc.CBLA_DS_DESCRICAO = item.CAPA_DS_DESCRICAO;
                lanc.CBLA_DT_LANCAMENTO = DateTime.Today.Date;
                lanc.CBLA_IN_ATIVO = 1;
                lanc.CBLA_IN_ORIGEM = 2;
                lanc.CBLA_IN_TIPO = 2;
                lanc.CBLA_NR_NUMERO = item.CAPA_CD_ID.ToString();
                lanc.CBLA_VL_VALOR = item.CAPA_VL_VALOR_PAGO;
                lanc.COBA_CD_ID = conta.COBA_CD_ID;
                item.CONTA_BANCO.CONTA_BANCO_LANCAMENTO.Add(lanc);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditCAPA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CONTA_PAGAR>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CONTA_PAGAR>(itemAntes)
                };

                // Gera Notificação
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.NOTI_DT_EMISSAO = DateTime.Now;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_NIVEL = 1;
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Contas a Pagar - Pagamento";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "O lançamento " + item.CAPA_NR_DOCUMENTO + " com vencimento em " + item.CAPA_DT_VENCIMENTO.Value.ToLongDateString() + " de " + item.CAPA_NM_FAVORECIDO + " foi liquidado em " + item.CAPA_DT_LIQUIDACAO.Value.ToLongDateString() + " sendo pago R$" + item.CAPA_VL_VALOR_PAGO.Value.ToString() + " - Valor original a pagar R$" + item.CAPA_VL_VALOR.ToString();
                noti.USUA_CD_ID = usuario.USUA_CD_ID;
                noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                noti.CANO_CD_ID = 1;
                noti.NOTI_IN_STATUS = 0;

                // Persiste notificação
                Int32 volta = _notiService.Create(noti);

                // Recupera template e-mail
                String header = _usuService.GetTemplate("PAGCPAG").TEMP_TX_CABECALHO;
                String body = _usuService.GetTemplate("PAGCPAG").TEMP_TX_CORPO;
                String footer = _usuService.GetTemplate("PAGCPAG").TEMP_TX_DADOS;

                // Prepara corpo do e-mail  
                header = header.Replace("{Nome}", item.USUARIO.USUA_NM_NOME);
                body = body.Replace("{Emissor}", usuario.ASSINANTE.ASSI_NM_NOME);
                footer = footer.Replace("{DataLanc}", item.CAPA_DT_LANCAMENTO.Value.ToLongDateString());
                footer = footer.Replace("{Valor}", item.CAPA_VL_VALOR.ToString());
                footer = footer.Replace("{DataVenc}", item.CAPA_DT_VENCIMENTO.Value.ToLongDateString());
                footer = footer.Replace("{DataPag}", item.CAPA_DT_LIQUIDACAO.Value.ToLongDateString());
                footer = footer.Replace("{Desc}", item.CAPA_DS_DESCRICAO);
                footer = footer.Replace("{Numero}", item.CAPA_CD_ID.ToString());

                // Concatena
                String emailBody = header + body + footer;

                // Monta e-mail
                CONFIGURACAO conf = _baseService.CarregaConfiguracao(usuario.ASSI_CD_ID);
                NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                Email mensagem = new Email();
                mensagem.ASSUNTO = "Pagamento de Lançamento - Conta a Pagar";
                mensagem.CORPO = emailBody;
                mensagem.DEFAULT_CREDENTIALS = false;
                mensagem.EMAIL_DESTINO = item.USUARIO.USUA_NM_EMAIL;
                mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                mensagem.ENABLE_SSL = true;
                mensagem.NOME_EMISSOR = usuario.USUA_NM_NOME;
                mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                mensagem.IS_HTML = true;
                mensagem.NETWORK_CREDENTIAL = net;

                // Envia mensagem
                Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);

                // Monta token
                String text = conf.CONF_SG_LOGIN_SMS + ":" + conf.CONF_SG_SENHA_SMS;
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                String token = Convert.ToBase64String(textBytes);
                String auth = "Basic " + token;

                // Prepara texto
                String texto = _usuService.GetTemplate("SMSCPAG").TEMP_TX_CORPO;;
                texto = texto.Replace("{Nome}", item.USUARIO.USUA_NM_NOME);
                texto = texto.Replace("{Numero}", item.CAPA_CD_ID.ToString());
                texto = texto.Replace("{Emissor}", usuario.ASSINANTE.ASSI_NM_NOME);
                String smsBody = texto;
                String erro = null;

                // inicia processo
                String resposta = String.Empty;

                // Monta destinatarios
                try
                {
                    String listaDest = "55" + Regex.Replace(item.USUARIO.USUA_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                    httpWebRequest.Headers["Authorization"] = auth;
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    String customId = Cryptography.GenerateRandomPassword(8);
                    String data = String.Empty;
                    String json = String.Empty;

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", texto, "\", \"customId\": \"" + customId + "\", \"from\": \"ERPSys\"}]}");
                        streamWriter.Write(json);
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        resposta = result;
                    }
                }
                catch (Exception ex)
                {
                    erro = ex.Message;
                }

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CONTA_PAGAR item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CAPA_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelCAPA",
                    LOG_TX_REGISTRO = "Exclusão de lançamento: " + item.CAPA_NR_DOCUMENTO
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CONTA_PAGAR item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CAPA_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCAPA",
                    LOG_TX_REGISTRO = "Reativação de lançamento: " + item.CAPA_NR_DOCUMENTO
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 IncluirRateioCC(CONTA_PAGAR item, Int32? cc, Int32? perc, USUARIO usuario)
        {
            try
            {
                // Verifica soma
                List<CONTA_PAGAR_RATEIO> lista = item.CONTA_PAGAR_RATEIO.ToList();
                Int32 soma = lista.Sum(p => p.CRPA_NR_PERCENTUAL);
                Int32 total = soma + perc.Value;

                if (total > 100)
                {
                    return 1;
                }
                
                // Monta rateio
                CONTA_PAGAR_RATEIO rat = new CONTA_PAGAR_RATEIO();
                rat.CAPA_CD_ID = item.CAPA_CD_ID;
                rat.CECU_CD_ID = cc.Value;
                rat.CRPA_IN_ATIVO = 1;
                rat.CRPA_NR_PERCENTUAL = perc.Value;

                // Persiste
                return _ratService.Create(rat);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
