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
    public class ContaReceberParcelaAppService : AppServiceBase<CONTA_RECEBER_PARCELA>, IContaReceberParcelaAppService
    {
        private readonly IContaReceberParcelaService _baseService;
        private readonly IContaBancariaService _cbService;
        private readonly INotificacaoService _notiService;
        private readonly IFormaPagamentoService _fopaService;
        private readonly IContaReceberService _crService;

        public ContaReceberParcelaAppService(IContaReceberParcelaService baseService, IContaBancariaService cbService, INotificacaoService notiService, IFormaPagamentoService fopaService, IContaReceberService crService) : base(baseService)
        {
            _baseService = baseService;
            _cbService = cbService;
            _notiService = notiService;
            _fopaService = fopaService;
            _crService = crService;
        }

        public CONTA_RECEBER_PARCELA GetItemById(Int32 id)
        {
            CONTA_RECEBER_PARCELA item = _baseService.GetItemById(id);
            return item;
        }

        public List<CONTA_RECEBER_PARCELA> GetAllItens()
        {
            return _baseService.GetAllItens();
        }

        public Int32 ValidateCreate(CONTA_RECEBER_PARCELA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.CRPA_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddCRPA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CONTA_RECEBER_PARCELA>(item)
                };

                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CONTA_RECEBER_PARCELA item, CONTA_RECEBER_PARCELA itemAntes, USUARIO usuario)
        {
            try
            {
                //  ****** Se for Liquidação
                if (item.CRPA_IN_QUITADA == 0 & item.CRPA_VL_RECEBIDO > 0)
                {
                    // Checa data
                    if (item.CRPA_DT_QUITACAO > DateTime.Now.Date)
                    {
                        return 1;
                    }

                    // Verifica Valor
                    Decimal soma = item.CRPA_VL_VALOR.Value + item.CRPA_VL_TAXAS.Value + item.CRPA_VL_JUROS.Value - item.CRPA_VL_DESCONTO.Value;
                    if (soma != item.CRPA_VL_RECEBIDO)
                    {
                        return 2;
                    }

                    // Acerta objeto
                    item.CRPA_IN_ATIVO = 1;
                    item.CRPA_IN_QUITADA = 1;

                    // Monta lançamento bancário
                    CONTA_RECEBER cr = _crService.GetItemById(item.CARE_CD_ID);
                    if (cr.FOPA_CD_ID == null)
                    {
                        return 3;
                    }
                    FORMA_PAGAMENTO forma = _fopaService.GetItemById(cr.FOPA_CD_ID.Value);
                    CONTA_BANCO conta = _cbService.GetItemById(forma.COBA_CD_ID.Value);
                    conta.COBA_VL_SALDO_ATUAL += item.CRPA_VL_VALOR;
                    CONTA_BANCO_LANCAMENTO lanc = new CONTA_BANCO_LANCAMENTO();
                    lanc.COBA_CD_ID = conta.COBA_CD_ID;
                    lanc.CBLA_DS_DESCRICAO = item.CRPA_DS_DESCRICAO;
                    lanc.CBLA_DT_LANCAMENTO = item.CRPA_DT_QUITACAO.Value;
                    lanc.CBLA_IN_ATIVO = 1;
                    lanc.CBLA_IN_ORIGEM = 0;
                    lanc.CBLA_IN_TIPO = 1;
                    lanc.CBLA_NR_NUMERO = item.CRPA_NR_PARCELA;
                    lanc.CBLA_VL_VALOR = item.CRPA_VL_RECEBIDO.Value;
                    conta.CONTA_BANCO_LANCAMENTO.Add(lanc);

                    // Gera Notificação
                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Contas a Receber - Liquidação de Parcela";
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_TX_TEXTO = "A parcela " + item.CRPA_NR_PARCELA + " do lançamento " + item.CONTA_RECEBER.CARE_DS_DESCRICAO + " foi liquidada em " + DateTime.Today.Date.ToLongDateString();
                    noti.USUA_CD_ID = usuario.USUA_CD_ID;
                    noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti.CANO_CD_ID = 1;
                    noti.NOTI_IN_STATUS = 0;

                    // Envia notificação
                    Int32 volta = _notiService.Create(noti);

                    // Persiste lancamento Bancario
                    volta = _cbService.Edit(conta);

                    // Persiste Lancamento Parcela CR
                    volta = _baseService.Edit(item);

                    return 0;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CONTA_RECEBER_PARCELA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CRPA_IN_ATIVO = 0;

                // Monta Log
                //LOG log = new LOG
                //{
                //    LOG_DT_DATA = DateTime.Now,
                //    USUA_CD_ID = usuario.USUA_CD_ID,
                //    ASSI_CD_ID = SessionMocks.IdAssinante,
                //    LOG_IN_ATIVO = 1,
                //    LOG_NM_OPERACAO = "DelCARE",
                //    LOG_TX_REGISTRO = Serialization.SerializeJSON<CONTA_RECEBER>(item)
                //};

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CONTA_RECEBER_PARCELA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CRPA_IN_ATIVO = 1;

                // Monta Log
                //LOG log = new LOG
                //{
                //    LOG_DT_DATA = DateTime.Now,
                //    USUA_CD_ID = usuario.USUA_CD_ID,
                //    ASSI_CD_ID = SessionMocks.IdAssinante,
                //    LOG_IN_ATIVO = 1,
                //    LOG_NM_OPERACAO = "ReatCARE",
                //    LOG_TX_REGISTRO = Serialization.SerializeJSON<CONTA_RECEBER>(item)
                //};

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

   }
}
