using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using SMS_Presentation.App_Start;
using EntitiesServices.Work_Classes;
using AutoMapper;
using ERP_CRM_Solution.ViewModels;
using System.IO;
using Correios.Net;
using Canducci.Zip;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using EntitiesServices.Attributes;
using OfficeOpenXml.Table;
using EntitiesServices.WorkClasses;
using System.Threading.Tasks;
using CrossCutting;
using EntitiesServices.DTO;

namespace ERP_CRM_Solution.Controllers
{
    public class ContaReceberController : Controller
    {
        private readonly IContaReceberAppService crApp;
        private readonly IClienteAppService cliApp;
        private readonly ILogAppService logApp;
        private readonly IContaPagarAppService cpApp;
        private readonly IFornecedorAppService forApp;
        private readonly ICentroCustoAppService ccApp;
        private readonly IContaBancariaAppService cbApp;
        private readonly IFormaPagamentoAppService fpApp;
        private readonly IPeriodicidadeAppService perApp;
        private readonly IContaReceberParcelaAppService pcApp;
        private readonly IContaPagarParcelaAppService ppApp;
        private readonly IContaReceberRateioAppService ratApp;

        private String msg;
        private Exception exception;
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao = String.Empty;
        CONTA_RECEBER objetoCR = new CONTA_RECEBER();
        CONTA_RECEBER objetoCRAntes = new CONTA_RECEBER();
        List<CONTA_RECEBER> listaCRMaster = new List<CONTA_RECEBER>();
        CONTA_PAGAR objetoCP = new CONTA_PAGAR();
        CONTA_PAGAR objetoCPAntes = new CONTA_PAGAR();
        List<CONTA_PAGAR> listaCPMaster = new List<CONTA_PAGAR>();
        CONTA_RECEBER_PARCELA objetoCRP = new CONTA_RECEBER_PARCELA();
        CONTA_RECEBER_PARCELA objetoCRPAntes = new CONTA_RECEBER_PARCELA();
        List<CONTA_RECEBER_PARCELA> listaCRPMaster = new List<CONTA_RECEBER_PARCELA>();
        CONTA_PAGAR_PARCELA objetoCPP = new CONTA_PAGAR_PARCELA();
        CONTA_PAGAR_PARCELA objetoCPPAntes = new CONTA_PAGAR_PARCELA();
        List<CONTA_PAGAR_PARCELA> listaCPPMaster = new List<CONTA_PAGAR_PARCELA>();
        CONTA_BANCO contaPadrao = new CONTA_BANCO();

        public ContaReceberController(IContaReceberAppService crApps, ILogAppService logApps, IClienteAppService cliApps, IContaPagarAppService cpApps, IFornecedorAppService forApps, ICentroCustoAppService ccApps, IContaBancariaAppService cbApps, IFormaPagamentoAppService fpApps, IPeriodicidadeAppService perApps, IContaReceberParcelaAppService pcApps, IContaPagarParcelaAppService ppApps, IContaReceberRateioAppService ratApps)
        {
            logApp = logApps;
            crApp = crApps;
            cliApp = cliApps;
            cpApp = cpApps;
            forApp = forApps;
            ccApp = ccApps;
            cbApp = cbApps;
            fpApp = fpApps;
            perApp = perApps;
            pcApp = pcApps;
            ppApp = ppApps;
            ratApp = ratApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return View();
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult VoltarGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult IncluirCliente()
        {

            Session["ClienteToCr"] = true;
            return RedirectToAction("IncluirCliente", "Cliente");
        }

        [HttpPost]
        public DTO_CR Liquidar()
        {
            Session["LiquidaCR"] = 1;
            Session["ParcialCR"] = 0;
            Session["Parcelamento"] = 0;
            DTO_CR dto = new DTO_CR();
            dto.LiquidaCR = 1;
            dto.ParcialCR = 0;
            dto.Parcelamento = 0;
            return dto;
        }

        [HttpPost]
        public DTO_CR Parcial()
        {
            Session["LiquidaCR"] = 0;
            Session["ParcialCR"] = 1;
            Session["Parcelamento"] = 0;

            DTO_CR dto = new DTO_CR();
            dto.LiquidaCR = 0;
            dto.ParcialCR = 1;
            dto.Parcelamento = 0;
            return dto;
        }

        [HttpPost]
        public DTO_CR Parcelamento()
        {
            Session["LiquidaCR"] = 0;
            Session["ParcialCR"] = 0;
            Session["Parcelamento"] = 1;

            DTO_CR dto = new DTO_CR();
            dto.LiquidaCR = 0;
            dto.ParcialCR = 0;
            dto.Parcelamento = 1;
            return dto;
        }

        [HttpPost]
        public DTO_CR Edicao()
        {
            Session["LiquidaCR"] = 0;
            Session["ParcialCR"] = 0;
            Session["Parcelamento"] = 0;

            DTO_CR dto = new DTO_CR();
            dto.LiquidaCR = 0;
            dto.ParcialCR = 0;
            dto.Parcelamento = 0;
            return dto;
        }

        public JsonResult GetRateio()
        {
            CONTA_RECEBER item = crApp.GetItemById((Int32)Session["IdVolta"]);
            List<Hashtable> result = new List<Hashtable>();

            if (item.CONTA_RECEBER_RATEIO != null && item.CONTA_RECEBER_RATEIO.Count > 0)
            {
                List<Int32> lstCC = item.CONTA_RECEBER_RATEIO.Select(x => x.CECU_CD_ID).ToList<Int32>();

                foreach (var i in lstCC)
                {
                    Hashtable id = new Hashtable();
                    id.Add("id", i);
                    result.Add(id);
                }
            }

            return Json(result);
        }

        [HttpGet]
        public ActionResult MontarTelaCR()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];


            // Carrega listas
            if (Session["ListaCR"] == null || ((List<CONTA_RECEBER>)Session["ListaCR"]).Count == 0)
            {
                listaCRMaster = crApp.GetAllItens(idAss);
                Session["ListaCR"] = listaCPMaster;
            }
            ViewBag.Listas = listaCRMaster;
            Session["Clientes"] = cliApp.GetAllItens(idAss);
            ViewBag.Clientes = new SelectList((List<CLIENTE>)Session["Clientes"], "CLIE_CD_ID", "CLIE_NM_NOME");

            // Indicadores
            List<CONTA_RECEBER> rec = crApp.GetAllItens(idAss);
            Decimal aReceberDia = (Decimal)crApp.GetVencimentoAtual(idAss).Where(x => x.CARE_IN_ATIVO == 1 && x.CARE_IN_LIQUIDADA == 0 && x.CARE_DT_VENCIMENTO.Value.Day == DateTime.Now.Day && (x.CONTA_RECEBER_PARCELA == null || x.CONTA_RECEBER_PARCELA.Count == 0)).Sum(x => x.CARE_VL_SALDO);
            aReceberDia += (Decimal)rec.Where(x => x.CARE_IN_ATIVO == 1 && x.CARE_IN_LIQUIDADA == 0 && x.CARE_DT_VENCIMENTO.Value.Day == DateTime.Now.Day && x.CONTA_RECEBER_PARCELA != null).SelectMany(x => x.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_VL_VALOR != null && x.CRPA_DT_VENCIMENTO.Value.Day == DateTime.Now.Day).Sum(x => x.CRPA_VL_VALOR);
            ViewBag.CRS = aReceberDia;
            ViewBag.Recebido = rec.Where(p => p.CARE_IN_ATIVO == 1 && p.CARE_IN_LIQUIDADA == 1 && p.CARE_DT_DATA_LIQUIDACAO.Value.Month == DateTime.Today.Date.Month).Sum(p => p.CARE_VL_VALOR_LIQUIDADO).Value;
            Decimal sumReceber = rec.Where(p => p.CARE_IN_ATIVO == 1 && p.CARE_IN_LIQUIDADA == 0 && p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month && (p.CONTA_RECEBER_PARCELA == null || p.CONTA_RECEBER_PARCELA.Count == 0)).Sum(p => p.CARE_VL_VALOR);
            sumReceber += (Decimal)rec.Where(p => p.CARE_IN_ATIVO == 1 && p.CARE_IN_LIQUIDADA == 0 && p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month && p.CONTA_RECEBER_PARCELA != null).SelectMany(p => p.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_VL_VALOR != null && x.CRPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month).Sum(p => p.CRPA_VL_VALOR);
            ViewBag.AReceber = sumReceber;
            Decimal sumAtraso = rec.Where(p => p.CARE_IN_ATIVO == 1 && p.CARE_NR_ATRASO > 0 && p.CARE_DT_VENCIMENTO < DateTime.Today.Date && (p.CONTA_RECEBER_PARCELA == null || p.CONTA_RECEBER_PARCELA.Count == 0)).Sum(p => p.CARE_VL_VALOR);
            sumAtraso += (Decimal)rec.Where(p => p.CARE_IN_ATIVO == 1 && p.CARE_NR_ATRASO > 0 && p.CARE_DT_VENCIMENTO < DateTime.Today.Date && p.CONTA_RECEBER_PARCELA != null).SelectMany(p => p.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_VL_VALOR != null && x.CRPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month).Sum(p => p.CRPA_VL_VALOR);
            ViewBag.Atrasos = sumAtraso;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            List<SelectListItem> tipoFiltro = new List<SelectListItem>();
            tipoFiltro.Add(new SelectListItem() { Text = "Somente em Aberto", Value = "1" });
            tipoFiltro.Add(new SelectListItem() { Text = "Somente Fechados", Value = "2" });
            ViewBag.Filtro = new SelectList(tipoFiltro, "Value", "Text");
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(idAss), "COBA_CD_ID", "COBA_NM_NOME");
            Session["ContasBancarias"] = cbApp.GetAllItens(idAss);
            
            if ((Int32)Session["ErroSoma"] == 2)
            {
                ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0059", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["ErroSoma"] == 3)
            {
                ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0017", CultureInfo.CurrentCulture));
            }
            Session["ErroSoma"] = 0;
            
            if (Session["MensCR"] != null && (Int32)Session["MensCR"] == 1)
            {
                ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
            }
            else if ((Int32)Session["MensVencimentoCR"] == 1)
            {
                Session["MensVencimentoCR"] = 0;
                ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0078", CultureInfo.CurrentCulture));
            }

            if (Session["MensCr"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensCR"] == 3)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0061", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCR"] == 4)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0080", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCR"] == 5)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0081", CultureInfo.CurrentCulture));
                }
            }

            if ((Int32)Session["VoltaCR"] != 0)
            {
                ViewBag.volta = Session["VoltaCR"];
                Session["VoltaCR"] = 0;
            }
            // Abre view
            objetoCR = new CONTA_RECEBER();
            return View(objetoCR);
        }

        public ActionResult RetirarFiltroCR()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaCR"] = null;
            Session["FiltroCR"] = null;
            return RedirectToAction("MontarTelaCR");
        }

        public ActionResult MostrarTudoCR()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaCRMaster = crApp.GetAllItensAdm(idAss);
            Session["FiltroCR"] = null;
            Session["ListaCR"] = listaCRMaster;
            return RedirectToAction("MontarTelaCR");
        }

        public ActionResult MostrarAtivosCR()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaCRMaster = crApp.GetAllItensAdm(idAss).Where(x => x.CARE_IN_ATIVO == 1).ToList();
            Session["FiltroCR"] = null;
            Session["ListaCR"] = listaCRMaster;
            return RedirectToAction("MontarTelaCR");
        }

        [HttpPost]
        public ActionResult FiltrarCR(CONTA_RECEBER item, DateTime? CARE_DT_VENCIMENTO_FINAL)
        {

            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executa a operação
                List<CONTA_RECEBER> listaObj = new List<CONTA_RECEBER>();
                Session["FiltroCR"] = item;
                if (CARE_DT_VENCIMENTO_FINAL != null)
                {
                    Session["vencFinal"] = CARE_DT_VENCIMENTO_FINAL.Value.ToShortDateString();
                }
                Int32 volta = crApp.ExecuteFilter(item.CLIE_CD_ID, item.CECU_CD_ID, item.CARE_DT_LANCAMENTO, item.CARE_DT_VENCIMENTO, CARE_DT_VENCIMENTO_FINAL, item.CARE_DS_DESCRICAO, item.CARE_IN_ABERTOS, item.FORMA_PAGAMENTO.COBA_CD_ID, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCR"] = 1;
                    return RedirectToAction("MontarTelaCR");
                }

                // Sucesso
                listaCRMaster = listaObj;
                Session["ListaCR"] = listaObj;
                return RedirectToAction("MontarTelaCR");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaCR");
            }
        }

        public ActionResult VoltarBaseCR()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaCRMaster = new List<CONTA_RECEBER>();
            Session["ListaCR"] = null;
            if (Session["FiltroCR"] != null)
            {
                if (Session["vencFinal"] == null)
                {
                    FiltrarCR((CONTA_RECEBER)Session["FiltroCR"], null);
                }
                else
                {
                    FiltrarCR((CONTA_RECEBER)Session["FiltroCR"], (DateTime)Session["vencFinal"]);
                }
            }
            return RedirectToAction("MontarTelaCR");
        }

        [HttpGet]
        public ActionResult VerRecebimentosMes()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<CONTA_RECEBER> rec = new List<CONTA_RECEBER>(); ;
            if (Session["ListaCRRecebimentoMes"] == null || ((List<CONTA_RECEBER>)Session["ListaCRRecebimentoMes"]).Count == 0)
            {
                rec = crApp.GetAllItens(idAss);
                Session["ListaCRRecebimentoMes"] = rec;
            }
            ViewBag.ListaCR = rec.Where(p => p.CARE_IN_LIQUIDADA == 1 && p.CARE_DT_DATA_LIQUIDACAO != null && p.CARE_DT_DATA_LIQUIDACAO.Value.Month == DateTime.Today.Date.Month).ToList();
            ViewBag.LR = rec.Count;
            ViewBag.Valor = rec.Sum(x => x.CARE_VL_VALOR_LIQUIDADO);
            ViewBag.CC = new SelectList(ccApp.GetAllItens(idAss).Where(x => x.CECU_IN_TIPO == 1).OrderBy(x => x.CECU_NM_NOME).ToList<CENTRO_CUSTO>(), "CECU_CD_ID", "CECU_NM_NOME");

            if (Session["MensRecebimentoMes"] != null && (Int32)Session["MensRecebimentoMes"] == 1)
            {
                ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0079", CultureInfo.CurrentCulture));
            }
            return View();
        }

        [HttpPost]
        public ActionResult FiltrarRecebimentoMes(CONTA_RECEBER item)
        {            
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executa a operação
                List<CONTA_RECEBER> listaObj = new List<CONTA_RECEBER>();
                Session["FiltroCRRecebimentoMes"] = item;
                Int32 volta = crApp.ExecuteFilterRecebimentoMes(item.CLIE_CD_ID, item.CECU_CD_ID, item.CARE_DS_DESCRICAO, item.CARE_DT_LANCAMENTO, item.CARE_DT_VENCIMENTO, item.CARE_DT_DATA_LIQUIDACAO, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensRecebimentoMes"] = 1;
                    return RedirectToAction("VerRecebimentosMes");
                }

                // Sucesso
                Session["ListaCRRecebimentoMes"] = listaObj;
                return RedirectToAction("VerRecebimentosMes");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("VerRecebimentosMes");
            }
        }

        [HttpGet]
        public ActionResult RetirarFiltroRecMes()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaCRRecebimentoMes"] = new List<CONTA_RECEBER>();
            return RedirectToAction("VerRecebimentosMes");
        }

        public ActionResult GerarRelatorioListaRecMes()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "RecebimentoMesLista" + "_" + data + ".pdf";
            List<CONTA_RECEBER> lista = ((List<CONTA_RECEBER>)Session["ListaCRRecebimentoMes"]).Where(p => p.CARE_IN_LIQUIDADA == 1 && p.CARE_DT_DATA_LIQUIDACAO != null && p.CARE_DT_DATA_LIQUIDACAO.Value.Month == DateTime.Today.Date.Month).ToList();
            CONTA_RECEBER filtro = (CONTA_RECEBER)Session["FiltroCRRecebimentoMes"];
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Imagens/Base/favicon_SystemBR.jpg"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Recebimentos do Mês - Listagem", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Grid
            table = new PdfPTable(8);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Itens de Conta a Receber selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 8;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Cliente", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Centro de Custo", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Conta Bancária", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Emissão", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Valor", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Descrição", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Liquidada", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Atraso", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (CONTA_RECEBER item in lista)
            {
                if (item.CLIENTE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CLIENTE.CLIE_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CENTRO_CUSTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CENTRO_CUSTO.CECU_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CONTA_BANCO != null)
                {
                    cell = new PdfPCell(new Paragraph((item.CONTA_BANCO.BANCO == null ? "" : item.CONTA_BANCO.BANCO.BANC_NM_NOME + ".") + (item.CONTA_BANCO.COBA_NM_AGENCIA == null ? "" : item.CONTA_BANCO.COBA_NM_AGENCIA + ".") + (item.CONTA_BANCO.COBA_NR_CONTA == null ? "" : item.CONTA_BANCO.COBA_NR_CONTA), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CARE_DT_LANCAMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CARE_DT_LANCAMENTO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(item.CARE_VL_VALOR.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.CARE_DS_DESCRICAO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.CARE_IN_LIQUIDADA == 1)
                {
                    cell = new PdfPCell(new Paragraph("Sim", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Não", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CARE_NR_ATRASO > 0)
                {
                    cell = new PdfPCell(new Paragraph(item.CARE_NR_ATRASO.Value.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line2);

            // Rodapé
            Chunk chunk1 = new Chunk("Parâmetros de filtro: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            String parametros = String.Empty;
            Int32 ja = 0;
            if (filtro != null)
            {
                if (filtro.CLIE_CD_ID != null)
                {
                    parametros += "Cliente: " + cliApp.GetItemById(filtro.CLIE_CD_ID.Value);
                    ja = 1;
                }
                if (filtro.CECU_CD_ID != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Plano de Contas: " + ccApp.GetItemById(filtro.CECU_CD_ID.Value);
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Plano de Contas: " + ccApp.GetItemById(filtro.CECU_CD_ID.Value);
                    }
                }
                if (filtro.CARE_DS_DESCRICAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Descrição: " + filtro.CARE_DS_DESCRICAO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Descrição: " + filtro.CARE_DS_DESCRICAO;
                    }
                }
                if (filtro.CARE_DT_LANCAMENTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data de Emissão: " + filtro.CARE_DT_LANCAMENTO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data de Emissão: " + filtro.CARE_DT_LANCAMENTO.Value.ToShortDateString();
                    }
                }
                if (filtro.CARE_DT_VENCIMENTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data de Vencimento: " + filtro.CARE_DT_VENCIMENTO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data de Vencimento: " + filtro.CARE_DT_VENCIMENTO.Value.ToShortDateString();
                    }
                }
                if (filtro.CARE_DT_DATA_LIQUIDACAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data de Liquidação: " + filtro.CARE_DT_DATA_LIQUIDACAO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data de Liquidação: " + filtro.CARE_DT_DATA_LIQUIDACAO.Value.ToShortDateString();
                    }
                }
                if (ja == 0)
                {
                    parametros = "Nenhum filtro definido.";
                }
            }
            else
            {
                parametros = "Nenhum filtro definido.";
            }
            Chunk chunk = new Chunk(parametros, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk);

            // Linha Horizontal
            Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line3);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("VerRecebimentosMes");
        }

        [HttpGet]
        public ActionResult VerAReceberMes()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<CONTA_RECEBER> rec = new List<CONTA_RECEBER>(); ;
            if (Session["ListaCRReceberMes"] == null || ((List<CONTA_RECEBER>)Session["ListaCRReceberMes"]).Count == 0)
            {
                rec = crApp.GetAllItens(idAss);
                Session["ListaCRRecebimentoMes"] = rec;
            }

            ViewBag.ListaCR = rec.Where(p => p.CARE_IN_LIQUIDADA == 0 && p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month).ToList();
            ViewBag.LR = rec.Count(x => x.CARE_IN_LIQUIDADA == 0 && x.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month);
            Decimal sumReceber = rec.Where(p => p.CARE_IN_LIQUIDADA == 0 && p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month && (p.CONTA_RECEBER_PARCELA == null || p.CONTA_RECEBER_PARCELA.Count == 0)).Sum(p => p.CARE_VL_VALOR);
            sumReceber += (Decimal)rec.Where(p => p.CARE_IN_LIQUIDADA == 0 && p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month && (p.CONTA_RECEBER_PARCELA != null || p.CONTA_RECEBER_PARCELA.Count > 0)).SelectMany(p => p.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_VL_VALOR != null && x.CRPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month).Sum(p => p.CRPA_VL_VALOR);
            ViewBag.Valor = sumReceber;
            ViewBag.CC = new SelectList(ccApp.GetAllItens(idAss).Where(x => x.CECU_IN_TIPO == 1).OrderBy(x => x.CECU_NM_NOME).ToList<CENTRO_CUSTO>(), "CECU_CD_ID", "CECU_NM_NOME");

            if (Session["MensAReceberMes"] != null && (Int32)Session["MensAReceberMes"] == 1)
            {
                ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0079", CultureInfo.CurrentCulture));
            }

            return View();
        }

        [HttpPost]
        public ActionResult FiltrarAReceberMes(CONTA_RECEBER item)
        {
            
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executa a operação
                List<CONTA_RECEBER> listaObj = new List<CONTA_RECEBER>();
                Session["FiltroCRReceberMes"] = item;
                Int32 volta = crApp.ExecuteFilterAReceberMes(item.CLIE_CD_ID, item.CECU_CD_ID, item.CARE_DS_DESCRICAO, item.CARE_DT_LANCAMENTO, item.CARE_DT_VENCIMENTO, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensAReceberMes"] = 1;
                    return RedirectToAction("VerAReceberMes");
                }

                // Sucesso
                Session["ListaCRReceberMes"] = listaObj;
                return RedirectToAction("VerAReceberMes");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("VerAReceberMes");
            }
        }

        [HttpGet]
        public ActionResult RetirarFiltroARecMes()
        {
            
            Session["ListaCRReceberMes"] = null;
            return RedirectToAction("VerAReceberMes");
        }

        public ActionResult GerarRelatorioListaARecMes()
        {
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "AReceberMesLista" + "_" + data + ".pdf";
            List<CONTA_RECEBER> lista = ((List<CONTA_RECEBER>)Session["ListaCRReceberMes"]).Where(p => p.CARE_IN_LIQUIDADA == 0 && p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month).ToList();
            CONTA_RECEBER filtro = (CONTA_RECEBER)Session["FiltroCRReceberMes"];
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Imagens/Base/favicon_SystemBR.jpg"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("A Receber no Mês - Listagem", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Grid
            table = new PdfPTable(8);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Itens de Conta a Receber selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 8;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Cliente", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Centro de Custo", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Conta Bancária", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Emissão", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Valor", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Descrição", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Atraso", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (CONTA_RECEBER item in lista)
            {
                if (item.CLIENTE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CLIENTE.CLIE_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CENTRO_CUSTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CENTRO_CUSTO.CECU_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CONTA_BANCO != null)
                {
                    cell = new PdfPCell(new Paragraph((item.CONTA_BANCO.BANCO == null ? "" : item.CONTA_BANCO.BANCO.BANC_NM_NOME + ".") + (item.CONTA_BANCO.COBA_NM_AGENCIA == null ? "" : item.CONTA_BANCO.COBA_NM_AGENCIA + ".") + (item.CONTA_BANCO.COBA_NR_CONTA == null ? "" : item.CONTA_BANCO.COBA_NR_CONTA), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CARE_DT_LANCAMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CARE_DT_LANCAMENTO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(item.CARE_VL_VALOR.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.CARE_DS_DESCRICAO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.CARE_DT_VENCIMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CARE_DT_VENCIMENTO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CARE_NR_ATRASO > 0)
                {
                    cell = new PdfPCell(new Paragraph(item.CARE_NR_ATRASO.Value.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line2);

            // Rodapé
            Chunk chunk1 = new Chunk("Parâmetros de filtro: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            String parametros = String.Empty;
            Int32 ja = 0;
            if (filtro != null)
            {
                if (filtro.CLIE_CD_ID != null)
                {
                    parametros += "Cliente: " + cliApp.GetItemById(filtro.CLIE_CD_ID.Value);
                    ja = 1;
                }
                if (filtro.CECU_CD_ID != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Plano de Contas: " + ccApp.GetItemById(filtro.CECU_CD_ID.Value);
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Plano de Contas: " + ccApp.GetItemById(filtro.CECU_CD_ID.Value);
                    }
                }
                if (filtro.CARE_DS_DESCRICAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Descrição: " + filtro.CARE_DS_DESCRICAO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Descrição: " + filtro.CARE_DS_DESCRICAO;
                    }
                }
                if (filtro.CARE_DT_LANCAMENTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data de Emissão: " + filtro.CARE_DT_LANCAMENTO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data de Emissão: " + filtro.CARE_DT_LANCAMENTO.Value.ToShortDateString();
                    }
                }
                if (filtro.CARE_DT_VENCIMENTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data de Vencimento: " + filtro.CARE_DT_VENCIMENTO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data de Vencimento: " + filtro.CARE_DT_VENCIMENTO.Value.ToShortDateString();
                    }
                }
                if (ja == 0)
                {
                    parametros = "Nenhum filtro definido.";
                }
            }
            else
            {
                parametros = "Nenhum filtro definido.";
            }
            Chunk chunk = new Chunk(parametros, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk);

            // Linha Horizontal
            Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line3);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("VerAReceberMes");
        }

        [HttpGet]
        public ActionResult VerLancamentosAtraso()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<CONTA_RECEBER> rec = new List<CONTA_RECEBER>(); ;
            if (Session["ListaCRLancAtraso"] == null || ((List<CONTA_RECEBER>)Session["ListaCRLancAtraso"]).Count == 0)
            {
                rec = crApp.GetAllItens(idAss);
                Session["ListaCRLancAtraso"] = rec;
            }

            ViewBag.ListaCR = rec.Where(p => p.CARE_NR_ATRASO > 0 && p.CARE_DT_VENCIMENTO < DateTime.Today.Date).ToList();
            ViewBag.LR =rec.Count;
            Decimal sumAtraso = rec.Where(p => p.CARE_NR_ATRASO > 0 && p.CARE_DT_VENCIMENTO < DateTime.Today.Date && (p.CONTA_RECEBER_PARCELA == null || p.CONTA_RECEBER_PARCELA.Count == 0)).Sum(p => p.CARE_VL_VALOR);
            sumAtraso += (Decimal)rec.Where(p => p.CARE_NR_ATRASO > 0 && p.CARE_DT_VENCIMENTO < DateTime.Today.Date && p.CONTA_RECEBER_PARCELA != null).SelectMany(p => p.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_VL_VALOR != null && x.CRPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month).Sum(p => p.CRPA_VL_VALOR);
            ViewBag.Valor = sumAtraso;
            ViewBag.CC = new SelectList(ccApp.GetAllItens(idAss).Where(x => x.CECU_IN_TIPO == 1).OrderBy(x => x.CECU_NM_NOME).ToList<CENTRO_CUSTO>(), "CECU_CD_ID", "CECU_NM_NOME");

            if (Session["MensCRAtraso"] != null && (Int32)Session["MensCRAtraso"] == 1)
            {
                ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0079", CultureInfo.CurrentCulture));
            }

            return View();
        }

        [HttpPost]
        public ActionResult FiltrarLancAtraso(CONTA_RECEBER item)
        {
            
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executa a operação
                List<CONTA_RECEBER> listaObj = new List<CONTA_RECEBER>();
                Session["FiltroCRLancAtraso"] = item;
                Int32 volta = crApp.ExecuteFilterCRAtrasos(item.CLIE_CD_ID, item.CECU_CD_ID, item.CARE_DS_DESCRICAO, item.CARE_DT_LANCAMENTO, item.CARE_DT_VENCIMENTO, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCRAtraso"] = 1;
                }

                // Sucesso
                Session["ListaCRLancAtraso"] = listaObj;
                return RedirectToAction("VerLancamentosAtraso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("VerLancamentosAtraso");
            }
        }

        [HttpGet]
        public ActionResult RetirarFiltroLancAtraso()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaCRLancAtraso"] = new List<CONTA_RECEBER>();
            return RedirectToAction("VerLancamentosAtraso");
        }

        public ActionResult GerarRelatorioListaLancAtraso()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "LancAtrasoLista" + "_" + data + ".pdf";

            List<CONTA_RECEBER> lista = ((List<CONTA_RECEBER>)Session["ListaCRLancAtraso"]).Where(p => p.CARE_NR_ATRASO > 0 && p.CARE_DT_VENCIMENTO < DateTime.Today.Date).ToList();
            CONTA_RECEBER filtro = (CONTA_RECEBER)Session["FiltroCRLancAtraso"];
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Imagens/Base/favicon_SystemBR.jpg"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Lançamentos em Atraso - Listagem", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Grid
            table = new PdfPTable(8);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Itens de Conta a Receber selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 8;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Cliente", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Centro de Custo", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Conta Bancária", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Emissão", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Valor", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Descrição", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Atraso", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (CONTA_RECEBER item in lista)
            {
                if (item.CLIENTE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CLIENTE.CLIE_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CENTRO_CUSTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CENTRO_CUSTO.CECU_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CONTA_BANCO != null)
                {
                    cell = new PdfPCell(new Paragraph((item.CONTA_BANCO.BANCO == null ? "" : item.CONTA_BANCO.BANCO.BANC_NM_NOME + ".") + (item.CONTA_BANCO.COBA_NM_AGENCIA == null ? "" : item.CONTA_BANCO.COBA_NM_AGENCIA + ".") + (item.CONTA_BANCO.COBA_NR_CONTA == null ? "" : item.CONTA_BANCO.COBA_NR_CONTA), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CARE_DT_LANCAMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CARE_DT_LANCAMENTO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(item.CARE_VL_VALOR.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.CARE_DS_DESCRICAO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.CARE_DT_VENCIMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CARE_DT_VENCIMENTO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CARE_NR_ATRASO > 0)
                {
                    cell = new PdfPCell(new Paragraph(item.CARE_NR_ATRASO.Value.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line2);

            // Rodapé
            Chunk chunk1 = new Chunk("Parâmetros de filtro: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            String parametros = String.Empty;
            Int32 ja = 0;
            if (filtro != null)
            {
                if (filtro.CLIE_CD_ID != null)
                {
                    parametros += "Cliente: " + cliApp.GetItemById(filtro.CLIE_CD_ID.Value);
                    ja = 1;
                }
                if (filtro.CECU_CD_ID != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Plano de Contas: " + ccApp.GetItemById(filtro.CECU_CD_ID.Value);
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Plano de Contas: " + ccApp.GetItemById(filtro.CECU_CD_ID.Value);
                    }
                }
                if (filtro.CARE_DS_DESCRICAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Descrição: " + filtro.CARE_DS_DESCRICAO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Descrição: " + filtro.CARE_DS_DESCRICAO;
                    }
                }
                if (filtro.CARE_DT_LANCAMENTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data de Emissão: " + filtro.CARE_DT_LANCAMENTO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data de Emissão: " + filtro.CARE_DT_LANCAMENTO.Value.ToShortDateString();
                    }
                }
                if (filtro.CARE_DT_VENCIMENTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data de Vencimento: " + filtro.CARE_DT_VENCIMENTO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data de Vencimento: " + filtro.CARE_DT_VENCIMENTO.Value.ToShortDateString();
                    }
                }
                if (ja == 0)
                {
                    parametros = "Nenhum filtro definido.";
                }
            }
            else
            {
                parametros = "Nenhum filtro definido.";
            }
            Chunk chunk = new Chunk(parametros, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk);

            // Linha Horizontal
            Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line3);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("VerLancamentosAtraso");
        }

        [HttpGet]
        public ActionResult VerCR(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            CONTA_RECEBER item = crApp.GetItemById(id);
            Session["ContaReceber"] = item;
            ContaReceberViewModel vm = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(item);
            Session["IdVolta"] = id;
            Session["IdCRVolta"] = 1;
            return View(vm);
        }

        [HttpPost]
        public void UploadFileToSession(IEnumerable<HttpPostedFileBase> files, String profile)
        {
            List<FileQueue> queue = new List<FileQueue>();

            foreach (var file in files)
            {
                FileQueue f = new FileQueue();
                f.Name = Path.GetFileName(file.FileName);
                f.ContentType = Path.GetExtension(file.FileName);

                MemoryStream ms = new MemoryStream();
                file.InputStream.CopyTo(ms);
                f.Contents = ms.ToArray();

                if (profile != null)
                {
                    if (file.FileName.Equals(profile))
                    {
                        f.Profile = 1;
                    }
                }

                queue.Add(f);
            }
            Session["FileQueueCR"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueLancamentoCR(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["ErroSoma"] = 4;
                return RedirectToAction("VoltarAnexoCR");
            }

            CONTA_RECEBER item = crApp.GetById((Int32)Session["IdVolta"]);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                Session["ErroSoma"] = 5;
                return RedirectToAction("VoltarAnexoCR");
            }

            String caminho = "/Imagens/" + idAss.ToString() + "/ContaReceber/" + item.CARE_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            CONTA_RECEBER_ANEXO foto = new CONTA_RECEBER_ANEXO();
            foto.CRAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.CRAN_DT_ANEXO = DateTime.Today;
            foto.CRAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            if (extensao.ToUpper() == ".PDF")
            {
                tipo = 3;
            }
            foto.CRAN_IN_TIPO = tipo;
            foto.CRAN_NM_TITULO = fileName;
            foto.CARE_CD_ID = item.CARE_CD_ID;

            // Carrega DTO
            DTO_CR dto = Edicao();

            item.CONTA_RECEBER_ANEXO.Add(foto);
            objetoCRAntes = item;
            Int32 volta = crApp.ValidateEdit(item, objetoCRAntes, usu, dto);
            if ((Int32)Session["IdCRVolta"] == 1)
            {
                return RedirectToAction("VerCR");
            }
            return RedirectToAction("VoltarAnexoCR");
        }

        [HttpPost]
        public ActionResult UploadFileLancamentoCR(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["ErroSoma"] = 4;
                return RedirectToAction("VoltarAnexoCR");
            }

            CONTA_RECEBER item = crApp.GetById((Int32)Session["IdVolta"]);

            CONTA_RECEBER itemAnx = new CONTA_RECEBER
            {
                CONTA_RECEBER_ANEXO = item.CONTA_RECEBER_ANEXO,
                CONTA_RECEBER_PARCELA = item.CONTA_RECEBER_PARCELA,
                CONTA_RECEBER_RATEIO = item.CONTA_RECEBER_RATEIO,
                CARE_CD_ID = item.CARE_CD_ID,
                ASSI_CD_ID = item.ASSI_CD_ID,
                FILI_CD_ID = item.FILI_CD_ID,
                USUA_CD_ID = item.USUA_CD_ID,
                CLIE_CD_ID = item.CLIE_CD_ID,
                PEVE_CD_ID = item.PEVE_CD_ID,
                COBA_CD_ID = item.COBA_CD_ID,
                CARE_DT_LANCAMENTO = item.CARE_DT_LANCAMENTO,
                CARE_VL_VALOR = item.CARE_VL_VALOR,
                CARE_DS_DESCRICAO = item.CARE_DS_DESCRICAO,
                CARE_IN_TIPO_LANCAMENTO = item.CARE_IN_TIPO_LANCAMENTO,
                CARE_NM_FAVORECIDO = item.CARE_NM_FAVORECIDO,
                CARE_NM_FORMA_PAGAMENTO = item.CARE_NM_FORMA_PAGAMENTO,
                CARE_IN_LIQUIDADA = item.CARE_IN_LIQUIDADA,
                CARE_IN_ATIVO = item.CARE_IN_ATIVO,
                CARE_DT_DATA_LIQUIDACAO = item.CARE_DT_DATA_LIQUIDACAO,
                CARE_VL_VALOR_LIQUIDADO = item.CARE_VL_VALOR_LIQUIDADO,
                CARE_DT_VENCIMENTO = item.CARE_DT_VENCIMENTO,
                CARE_NR_ATRASO = item.CARE_NR_ATRASO,
                FOPA_CD_ID = item.FOPA_CD_ID,
                CARE_TX_OBSERVACOES = item.CARE_TX_OBSERVACOES,
                CARE_IN_PARCELADA = item.CARE_IN_PARCELADA,
                CARE_IN_PARCELAS = item.CARE_IN_PARCELAS,
                CARE_DT_INICIO_PARCELA = item.CARE_DT_INICIO_PARCELA,
                PERI_CD_ID = item.PERI_CD_ID,
                CARE_VL_PARCELADO = item.CARE_VL_PARCELADO,
                CARE_NR_DOCUMENTO = item.CARE_NR_DOCUMENTO,
                CARE_DT_COMPETENCIA = item.CARE_DT_COMPETENCIA,
                CARE_VL_DESCONTO = item.CARE_VL_DESCONTO,
                CARE_VL_JUROS = item.CARE_VL_JUROS,
                CARE_VL_TAXAS = item.CARE_VL_TAXAS,
                CECU_CD_ID = item.CECU_CD_ID,
                CARE_VL_SALDO = item.CARE_VL_SALDO,
                CARE_IN_PAGA_PARCIAL = item.CARE_IN_PAGA_PARCIAL,
                CARE_VL_PARCIAL = item.CARE_VL_PARCIAL,
                TITA_CD_ID = item.TITA_CD_ID,
                PEVE_DT_PREVISTA = item.PEVE_DT_PREVISTA,
                CARE_IN_ABERTOS = item.CARE_IN_ABERTOS
            };

            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                Session["ErroSoma"] = 5;
                return RedirectToAction("VoltarAnexoCR");
            }

            String caminho = "/Imagens/" + idAss.ToString() + "/ContaReceber/" + itemAnx.CARE_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            CONTA_RECEBER_ANEXO foto = new CONTA_RECEBER_ANEXO();
            foto.CRAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.CRAN_DT_ANEXO = DateTime.Today;
            foto.CRAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            if (extensao.ToUpper() == ".PDF")
            {
                tipo = 3;
            }
            foto.CRAN_IN_TIPO = tipo;
            foto.CRAN_NM_TITULO = fileName;
            foto.CARE_CD_ID = itemAnx.CARE_CD_ID;

            // Carrega DTO
            DTO_CR dto = Edicao();

            itemAnx.CONTA_RECEBER_ANEXO.Add(foto);
            objetoCRAntes = item;
            Int32 volta = crApp.ValidateEdit(itemAnx, objetoCRAntes, usu, dto);
            if ((Int32)Session["IdCRVolta"] == 1)
            {
                return RedirectToAction("VerCR");
            }
            return RedirectToAction("VoltarAnexoCR");
        }

        [HttpGet]
        public ActionResult VerAnexoLancamentoCR(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            CONTA_RECEBER_ANEXO item = crApp.GetAnexoById(id);
            return View(item);
        }

        public FileResult DownloadLancamentoCR(Int32 id)
        {
            CONTA_RECEBER_ANEXO item = crApp.GetAnexoById(id);
            String arquivo = item.CRAN_AQ_ARQUIVO;
            Int32 pos = arquivo.LastIndexOf("/") + 1;
            String nomeDownload = arquivo.Substring(pos);
            String contentType = string.Empty;
            if (arquivo.Contains(".pdf"))
            {
                contentType = "application/pdf";
            }
            else if (arquivo.Contains(".jpg"))
            {
                contentType = "image/jpg";
            }
            else if (arquivo.Contains(".png"))
            {
                contentType = "image/png";
            }
            return File(arquivo, contentType, nomeDownload);
        }

        [HttpGet]
        public ActionResult ExcluirCR(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCR"] = 2;
                    return RedirectToAction("MontarTelaCR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CONTA_RECEBER item = crApp.GetItemById(id);
            objetoCRAntes = (CONTA_RECEBER)Session["ContaReceber"];
            item.CARE_IN_ATIVO = 0;
            Int32 volta = crApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["ErroSoma"] = 2;
            }
            listaCRMaster = new List<CONTA_RECEBER>();
            Session["ListaCR"] = null;
            return RedirectToAction("MontarTelaCR");
        }

        [HttpGet]
        public ActionResult ReativarCR(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCR"] = 2;
                    return RedirectToAction("MontarTelaCR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CONTA_RECEBER item = crApp.GetItemById(id);
            objetoCRAntes = (CONTA_RECEBER)Session["ContaReceber"];
            item.CARE_IN_ATIVO = 1;
            Int32 volta = crApp.ValidateReativar(item, usuario);
            listaCRMaster = new List<CONTA_RECEBER>();
            Session["ListaCR"] = null;
            return RedirectToAction("MontarTelaCR");
        }

        [HttpGet]
        public ActionResult VerParcelaCR(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            CONTA_RECEBER_PARCELA item = crApp.GetParcelaById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoVerCR()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            return RedirectToAction("VerCR", new { id = (Int32)Session["IdVolta"] });
        }

        public ActionResult VoltarAnexoCR()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EditarCR", new { id = (Int32)Session["IdVolta"] });
        }

        [HttpGet]
        public ActionResult IncluirCR()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensCR"] = 2;
                    return RedirectToAction("MontarTelaCR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(idAss), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllItens(idAss).Where(x => x.CECU_IN_TIPO == 1).OrderBy(x => x.CECU_NM_NOME).ToList<CENTRO_CUSTO>(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(idAss), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(idAss), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(idAss), "PERI_CD_ID", "PERI_NM_NOME");
            List<SelectListItem> tipoRec = new List<SelectListItem>();
            tipoRec.Add(new SelectListItem() { Text = "Recebimento Recorrente", Value = "1" });
            tipoRec.Add(new SelectListItem() { Text = "Parcelamento", Value = "2" });
            ViewBag.Pagamento = new SelectList(tipoRec, "Value", "Text");

            // Prepara view
            CONTA_RECEBER item = new CONTA_RECEBER();
            ContaReceberViewModel vm = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.CARE_DT_LANCAMENTO = DateTime.Today.Date;
            vm.CARE_IN_ATIVO = 1;
            vm.FILI_CD_ID = 1;
            vm.CARE_DT_COMPETENCIA = DateTime.Today.Date;
            vm.CARE_DT_VENCIMENTO = DateTime.Today.Date.AddDays(30);
            vm.CARE_IN_LIQUIDADA = 0;
            vm.CARE_IN_PAGA_PARCIAL = 0;
            vm.CARE_IN_PARCELADA = 0;
            vm.CARE_IN_PARCELAS = 0;
            vm.CARE_VL_SALDO = 0;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirCR(ContaReceberViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(idAss), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllItens(idAss).Where(x => x.CECU_IN_TIPO == 1).OrderBy(x => x.CECU_NM_NOME).ToList<CENTRO_CUSTO>(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(idAss), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(idAss), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(idAss), "PERI_CD_ID", "PERI_NM_NOME");
            List<SelectListItem> tipoRec = new List<SelectListItem>();
            tipoRec.Add(new SelectListItem() { Text = "Recebimento Recorrente", Value = "1" });
            tipoRec.Add(new SelectListItem() { Text = "Parcelamento", Value = "2" });
            ViewBag.Pagamento = new SelectList(tipoRec, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    Int32 recorrencia = vm.CARE_IN_RECORRENTE;
                    DateTime? data = vm.CARE_DT_INICIO_RECORRENCIA;
                    CONTA_RECEBER item = Mapper.Map<ContaReceberViewModel, CONTA_RECEBER>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = crApp.ValidateCreate(item, recorrencia, data, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        return RedirectToAction("MontarTelaCR");
                    }
                    if (volta == 2)
                    {
                        Session["MensCR"] = 3;
                        return RedirectToAction("MontarTelaCR");
                    }
                    if (volta == 3)
                    {
                        Session["MensCR"] = 4;
                        return RedirectToAction("MontarTelaCR");
                    }
                    if (volta == 4)
                    {
                        Session["MensCR"] = 5;
                        return RedirectToAction("MontarTelaCR");
                    }

                    // Acerta numero
                    if (item.CARE_NR_DOCUMENTO == null)
                    {
                        item.CARE_NR_DOCUMENTO = item.CARE_CD_ID.ToString();
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/ContaReceber/" + item.CARE_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaCRMaster = new List<CONTA_RECEBER>();
                    Session["ListaCR"] = null;
                    Session["IdVolta"] = item.CARE_CD_ID;

                    if (Session["FileQueueCR"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueCR"];

                        foreach (var file in fq)
                        {
                            UploadFileQueueLancamentoCR(file);
                        }

                        Session["FileQueueCR"] = null;
                    }

                    return RedirectToAction("MontarTelaCR");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarCR(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensCR"] = 2;
                    return RedirectToAction("MontarTelaCR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(idAss), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllItens(idAss).Where(x => x.CECU_IN_TIPO == 1).OrderBy(x => x.CECU_NM_NOME).ToList<CENTRO_CUSTO>(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(idAss), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(idAss), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(idAss), "PERI_CD_ID", "PERI_NM_NOME");
            List<SelectListItem> tipoRec = new List<SelectListItem>();
            tipoRec.Add(new SelectListItem() { Text = "Recebimento Recorrente", Value = "1" });
            tipoRec.Add(new SelectListItem() { Text = "Parcelamento", Value = "2" });
            ViewBag.Pagamento = new SelectList(tipoRec, "Value", "Text");
            ViewBag.Liquida = 0;

            CONTA_RECEBER item = crApp.GetItemById(id);
            objetoCRAntes = item;
            Session["ContaReceber"] = item;
            Session["IdVolta"] = id;
            Session["IdCrVolta"] = 2;
            ContaReceberViewModel vm = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(item);
            vm.CARE_VL_PARCELADO = vm.CARE_VL_VALOR;
            vm.CARE_DT_DATA_LIQUIDACAO = DateTime.Now;
            if (vm.CARE_IN_PAGA_PARCIAL == 1)
            {
                vm.CARE_VL_VALOR_RECEBIDO = vm.CARE_VL_SALDO;
            }
            else
            {
                vm.CARE_VL_VALOR_RECEBIDO = vm.CARE_VL_VALOR;
            }
            if (Session["ErroSoma"] != null && (Int32)Session["ErroSoma"] == 1)
            {
                ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0096", CultureInfo.CurrentCulture));
                Session["IdVoltaTab"] = 3;
            }
            if (Session["ErroSoma"] != null && (Int32)Session["ErroSoma"] == 3)
            {
                ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0097", CultureInfo.CurrentCulture));
                Session["IdVoltaTab"] = 1;
            }
            if (Session["ErroSoma"] != null && (Int32)Session["ErroSoma"] == 4)
            {
                ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0098", CultureInfo.CurrentCulture));
                Session["IdVoltaTab"] = 1;
            }

            if (Session["MensCr"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensCR"] == 10)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0082", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCR"] == 11)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0083", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCR"] == 12)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0084", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCR"] == 13)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0085", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCR"] == 14)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0086", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCR"] == 15)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0087", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCR"] == 16)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0088", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCR"] == 17)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0089", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCR"] == 18)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0090", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCR"] == 19)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0091", CultureInfo.CurrentCulture));
                }
            }
            Session["ErroSoma"] = 0;
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCR(ContaReceberViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            var vmVolta = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>((CONTA_RECEBER)Session["ContaReceber"]);

            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(idAss), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllItens(idAss).Where(x => x.CECU_IN_TIPO == 1).OrderBy(x => x.CECU_NM_NOME).ToList<CENTRO_CUSTO>(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(idAss), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(idAss), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(idAss), "PERI_CD_ID", "PERI_NM_NOME");
            List<SelectListItem> tipoRec = new List<SelectListItem>();
            tipoRec.Add(new SelectListItem() { Text = "Recebimento Recorrente", Value = "1" });
            tipoRec.Add(new SelectListItem() { Text = "Parcelamento", Value = "2" });
            ViewBag.Pagamento = new SelectList(tipoRec, "Value", "Text");
            ViewBag.Liquida = 0;
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    if (vm.CARE_DT_DATA_LIQUIDACAO == null)
                    {
                        vm.CARE_DT_DATA_LIQUIDACAO = DateTime.Now.Date;
                    }
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONTA_RECEBER item = Mapper.Map<ContaReceberViewModel, CONTA_RECEBER>(vm);
                    DTO_CR dto = Edicao();
                    Int32 volta = crApp.ValidateEdit(item, (CONTA_RECEBER)Session["ContaReceber"], usuarioLogado, dto);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCR"] = 10;
                        return View(vmVolta);
                    }
                    if (volta == 2)
                    {
                        Session["MensCR"] = 11;
                        return View(vmVolta);
                    }
                    if (volta == 3)
                    {
                        Session["MensCR"] = 12;
                        return View(vmVolta);
                    }
                    if (volta == 4)
                    {
                        Session["MensCR"] = 13;
                        return View(vmVolta);
                    }
                    if (volta == 5)
                    {
                        Session["MensCR"] = 14;
                        return View(vmVolta);
                    }
                    if (volta == 6)
                    {
                        Session["MensCR"] = 15;
                        return View(vmVolta);
                    }
                    if (volta == 7)
                    {
                        Session["MensCR"] = 16;
                        return View(vmVolta);
                    }
                    if (volta == 8)
                    {
                        Session["MensCR"] = 17;
                        return View(vmVolta);
                    }
                    if (volta == 9)
                    {
                        Session["MensCR"] = 18;
                        return View(vmVolta);
                    }
                    if (volta == 10)
                    {
                        Session["MensCR"] = 19;
                        return View(vmVolta);
                    }

                    // Sucesso
                    listaCRMaster = new List<CONTA_RECEBER>();
                    Session["ListaCR"] = null;
                    Session["VoltaCR"] = item.CARE_CD_ID;
                    if (Session["FiltroCR"] != null)
                    {
                        if (Session["vencFinal"] == null)
                        {
                            FiltrarCR((CONTA_RECEBER)Session["FiltroCR"], null);
                        }
                        else
                        {
                            FiltrarCR((CONTA_RECEBER)Session["FiltroCR"], (DateTime)Session["vencFinal"]);
                        }
                    }
                    return RedirectToAction("MontarTelaCR");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vmVolta);
                }
            }
            else
            {
                return View(vmVolta);
            }
        }

        [HttpGet]
        public ActionResult ParcelarCR(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensCR"] = 2;
                    return RedirectToAction("MontarTelaCR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(idAss), "PERI_CD_ID", "PERI_NM_NOME");
            CONTA_RECEBER item = crApp.GetItemById(id);
            objetoCRAntes = item;
            Session["ContaReceber"] = item;
            Session["IdVolta"] = id;
            ContaReceberViewModel vm = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(item);
            vm.CARE_DT_INICIO_PARCELA = DateTime.Today.Date;
            vm.CARE_IN_PARCELADA = 1;
            vm.CARE_IN_PARCELAS = 2;
            vm.CARE_VL_PARCELADO = vm.CARE_VL_VALOR;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ParcelarCR(ContaReceberViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(idAss), "PERI_CD_ID", "PERI_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Processa parcelas
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONTA_RECEBER item = Mapper.Map<ContaReceberViewModel, CONTA_RECEBER>(vm);
                    DateTime dataParcela = item.CARE_DT_INICIO_PARCELA.Value;
                    PERIODICIDADE period = perApp.GetItemById(item.PERI_CD_ID.Value);
                    if (dataParcela.Date <= DateTime.Today.Date)
                    {
                        dataParcela = dataParcela.AddMonths(1);
                    }

                    for (int i = 1; i <= item.CARE_IN_PARCELAS; i++)
                    {
                        CONTA_RECEBER_PARCELA parc = new CONTA_RECEBER_PARCELA();
                        parc.CARE_CD_ID = item.CARE_CD_ID;
                        parc.CRPA_DT_QUITACAO = null;
                        parc.CRPA_DT_VENCIMENTO = dataParcela;
                        parc.CRPA_IN_ATIVO = 1;
                        parc.CRPA_IN_QUITADA = 0;
                        parc.CRPA_NR_PARCELA = i.ToString() + "/" + item.CARE_IN_PARCELAS.Value.ToString();
                        parc.CRPA_VL_RECEBIDO = 0;
                        parc.CRPA_VL_VALOR = item.CARE_VL_PARCELADO;
                        item.CONTA_RECEBER_PARCELA.Add(parc);
                        dataParcela = dataParcela.AddDays(period.PERI_NR_DIAS);
                    }

                    item.CARE_IN_PARCELADA = 1;
                    objetoCRAntes = item;
                    DTO_CR dto = Edicao();
                    Int32 volta = crApp.ValidateEdit(item, objetoCRAntes, usuarioLogado,  dto);
                    listaCRMaster = new List<CONTA_RECEBER>();
                    Session["ListaCR"] = null;
                    return RedirectToAction("MontarTelaCR");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult DuplicarCR()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Monta novo lançamento
            CONTA_RECEBER item = crApp.GetItemById((Int32)Session["IdVolta"]);
            CONTA_RECEBER novo = new CONTA_RECEBER();
            novo.ASSI_CD_ID = idAss;
            novo.CARE_DS_DESCRICAO = "Lançamento Duplicado - " + item.CARE_DS_DESCRICAO;
            novo.CARE_DT_COMPETENCIA = item.CARE_DT_COMPETENCIA;
            novo.CARE_DT_LANCAMENTO = DateTime.Today.Date;
            novo.CARE_DT_VENCIMENTO = DateTime.Today.Date.AddDays(30);
            novo.CARE_IN_ATIVO = 1;
            novo.CARE_IN_LIQUIDADA = 0;
            novo.CARE_IN_PAGA_PARCIAL = 0;
            novo.CARE_IN_PARCELADA = 0;
            novo.CARE_IN_PARCELAS = 0;
            novo.CARE_IN_TIPO_LANCAMENTO = 0;
            novo.CARE_NM_FAVORECIDO = item.CARE_NM_FAVORECIDO;
            novo.CARE_NR_DOCUMENTO = item.CARE_NR_DOCUMENTO;
            novo.CARE_TX_OBSERVACOES = item.CARE_TX_OBSERVACOES;
            novo.CARE_VL_DESCONTO = 0;
            novo.CARE_VL_JUROS = 0;
            novo.CARE_VL_PARCELADO = 0;
            novo.CARE_VL_PARCIAL = 0;
            novo.CARE_VL_SALDO = 0;
            novo.CARE_VL_TAXAS = 0;
            novo.CARE_VL_VALOR = item.CARE_VL_VALOR;
            novo.CARE_VL_VALOR_LIQUIDADO = 0;
            novo.CECU_CD_ID = item.CECU_CD_ID;
            novo.CLIE_CD_ID = item.CLIE_CD_ID;
            novo.COBA_CD_ID = item.COBA_CD_ID;
            novo.FILI_CD_ID = item.FILI_CD_ID;
            novo.FOPA_CD_ID = item.FOPA_CD_ID;
            novo.PERI_CD_ID = item.PERI_CD_ID;
            novo.PEVE_CD_ID = item.PEVE_CD_ID;
            novo.USUA_CD_ID = item.USUA_CD_ID;

            // Grava
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            Int32 volta = crApp.ValidateCreate(novo, 0, null, usuarioLogado);

            // Cria pastas
            String caminho = "/Imagens/" + idAss.ToString() + "/ContaReceber/" + novo.CARE_CD_ID.ToString() + "/Anexos/";
            Directory.CreateDirectory(Server.MapPath(caminho));

            // Sucesso
            listaCRMaster = new List<CONTA_RECEBER>();
            Session["ListaCR"] = null;
            return RedirectToAction("MontarTelaCR");
        }

        [HttpGet]
        public ActionResult LiquidarParcelaCR(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensCR"] = 2;
                    return RedirectToAction("MontarTelaCR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            CONTA_RECEBER_PARCELA item = pcApp.GetItemById(id);
            objetoCRPAntes = item;
            Session["ContaReceberParcela"] = item;
            Session["IdVoltaCRP"] = id;
            Session["LiquidaCR"] = 0;
            Session["ParcialCR"] = 0;

            if (Session["MensCr"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensCR"] == 20)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0092", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCR"] == 21)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0095", CultureInfo.CurrentCulture));
                }
            }

            ContaReceberParcelaViewModel vm = Mapper.Map<CONTA_RECEBER_PARCELA, ContaReceberParcelaViewModel>(item);
            vm.CRPA_VL_DESCONTO = 0;
            vm.CRPA_VL_JUROS = 0;
            vm.CRPA_VL_TAXAS = 0;
            vm.CRPA_VL_RECEBIDO = vm.CRPA_VL_VALOR;
            vm.CRPA_DT_QUITACAO = DateTime.Today.Date;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LiquidarParcelaCR(ContaReceberParcelaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONTA_RECEBER_PARCELA item = Mapper.Map<ContaReceberParcelaViewModel, CONTA_RECEBER_PARCELA>(vm);
                    Int32 volta = pcApp.ValidateEdit(item, (CONTA_RECEBER_PARCELA)Session["ContaReceberParcela"], usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCR"] = 20;
                        return View(vm);
                    }
                    if (volta == 3)
                    {
                        Session["MensCR"] = 21;
                        return View(vm);
                    }

                    // Acerta saldo
                    CONTA_RECEBER rec = crApp.GetItemById((Int32)Session["IdVolta"]);
                    CONTA_RECEBER recEdit = new CONTA_RECEBER {
                        CONTA_RECEBER_ANEXO = rec.CONTA_RECEBER_ANEXO,
                        CONTA_RECEBER_PARCELA = rec.CONTA_RECEBER_PARCELA,
                        CONTA_RECEBER_RATEIO = rec.CONTA_RECEBER_RATEIO,
                        CARE_CD_ID = rec.CARE_CD_ID,
                        ASSI_CD_ID = rec.ASSI_CD_ID,
                        FILI_CD_ID = rec.FILI_CD_ID,
                        USUA_CD_ID = rec.USUA_CD_ID,
                        CLIE_CD_ID = rec.CLIE_CD_ID,
                        PEVE_CD_ID = rec.PEVE_CD_ID,
                        COBA_CD_ID = rec.COBA_CD_ID,
                        CARE_DT_LANCAMENTO = rec.CARE_DT_LANCAMENTO,
                        CARE_VL_VALOR = rec.CARE_VL_VALOR,
                        CARE_DS_DESCRICAO = rec.CARE_DS_DESCRICAO,
                        CARE_IN_TIPO_LANCAMENTO = rec.CARE_IN_TIPO_LANCAMENTO,
                        CARE_NM_FAVORECIDO = rec.CARE_NM_FAVORECIDO,
                        CARE_NM_FORMA_PAGAMENTO = rec.CARE_NM_FORMA_PAGAMENTO,
                        CARE_IN_LIQUIDADA = rec.CARE_IN_LIQUIDADA,
                        CARE_IN_ATIVO = rec.CARE_IN_ATIVO,
                        CARE_DT_DATA_LIQUIDACAO = rec.CARE_DT_DATA_LIQUIDACAO,
                        CARE_VL_VALOR_LIQUIDADO = rec.CARE_VL_VALOR_LIQUIDADO,
                        CARE_DT_VENCIMENTO = rec.CARE_DT_VENCIMENTO,
                        CARE_NR_ATRASO = rec.CARE_NR_ATRASO,
                        CARE_TX_OBSERVACOES = rec.CARE_TX_OBSERVACOES,
                        CARE_IN_PARCELADA = rec.CARE_IN_PARCELADA,
                        CARE_IN_PARCELAS = rec.CARE_IN_PARCELAS,
                        CARE_DT_INICIO_PARCELA = rec.CARE_DT_INICIO_PARCELA,
                        PERI_CD_ID = rec.PERI_CD_ID,
                        CARE_VL_PARCELADO = rec.CARE_VL_PARCELADO,
                        CARE_NR_DOCUMENTO = rec.CARE_NR_DOCUMENTO,
                        CARE_DT_COMPETENCIA = rec.CARE_DT_COMPETENCIA,
                        CARE_VL_DESCONTO = rec.CARE_VL_DESCONTO,
                        CARE_VL_JUROS = rec.CARE_VL_JUROS,
                        CARE_VL_TAXAS = rec.CARE_VL_TAXAS,
                        CECU_CD_ID = rec.CECU_CD_ID,
                        CARE_VL_SALDO = rec.CARE_VL_SALDO,
                        CARE_IN_PAGA_PARCIAL = rec.CARE_IN_PAGA_PARCIAL,
                        CARE_VL_PARCIAL = rec.CARE_VL_PARCIAL,
                        TITA_CD_ID = rec.TITA_CD_ID,
                        PEVE_DT_PREVISTA = rec.PEVE_DT_PREVISTA,
                        CARE_IN_ABERTOS = rec.CARE_IN_ABERTOS,
                        FOPA_CD_ID = rec.FOPA_CD_ID
                    };
                    recEdit.CARE_VL_SALDO = recEdit.CARE_VL_SALDO - item.CRPA_VL_RECEBIDO;                    
                    
                    // Verifica se liquidou todas
                    List<CONTA_RECEBER_PARCELA> lista = recEdit.CONTA_RECEBER_PARCELA.Where(p => p.CRPA_IN_QUITADA == 0).ToList<CONTA_RECEBER_PARCELA>();
                    if (lista.Count == 0)
                    {
                        recEdit.CARE_IN_LIQUIDADA = 1;
                        recEdit.CARE_DT_DATA_LIQUIDACAO = DateTime.Today.Date;
                        recEdit.CARE_VL_VALOR_LIQUIDADO = recEdit.CONTA_RECEBER_PARCELA.Sum(p => p.CRPA_VL_RECEBIDO);
                        recEdit.CARE_VL_SALDO = 0;
                    }

                    recEdit.CARE_VL_VALOR_LIQUIDADO = recEdit.CONTA_RECEBER_PARCELA.Where(p => p.CRPA_IN_QUITADA == 1).Sum(p => p.CRPA_VL_VALOR);
                    DTO_CR dto = Edicao();
                    volta = crApp.ValidateEdit(recEdit, rec, usuarioLogado, dto);

                    // Sucesso
                    listaCRPMaster = new List<CONTA_RECEBER_PARCELA>();
                    Session["ListaCRP"] = null;
                    return RedirectToAction("VoltarAnexoCR");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult LiquidarCR(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensCR"] = 2;
                    return RedirectToAction("MontarTelaCR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(idAss), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllItens(idAss).Where(x => x.CECU_IN_TIPO == 1).OrderBy(x => x.CECU_NM_NOME).ToList<CENTRO_CUSTO>(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(idAss), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(idAss), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(idAss), "PERI_CD_ID", "PERI_NM_NOME");
            List<SelectListItem> tipoRec = new List<SelectListItem>();
            tipoRec.Add(new SelectListItem() { Text = "Recebimento Recorrente", Value = "1" });
            tipoRec.Add(new SelectListItem() { Text = "Parcelamento", Value = "2" });
            ViewBag.Pagamento = new SelectList(tipoRec, "Value", "Text");
            ViewBag.Liquida = 1;
            Session["LiquidaCR"] = 1;

            if (Session["MensCr"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensCR"] == 10)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0082", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCR"] == 11)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0083", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCR"] == 12)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0084", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCR"] == 13)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0085", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCR"] == 14)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0086", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCR"] == 15)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0087", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCR"] == 16)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0088", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCR"] == 17)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0089", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCR"] == 18)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0090", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCR"] == 19)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0091", CultureInfo.CurrentCulture));
                }
            }

            CONTA_RECEBER item = crApp.GetItemById(id);
            objetoCRAntes = item;
            Session["ContaReceber"] = item;
            Session["IdVolta"] = id;
            ContaReceberViewModel vm = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(item);
            vm.CARE_VL_PARCELADO = vm.CARE_VL_VALOR;
            if (vm.CARE_IN_PAGA_PARCIAL == 1)
            {
                vm.CARE_VL_VALOR_LIQUIDADO = vm.CARE_VL_SALDO;
            }
            else
            {
                vm.CARE_VL_VALOR_LIQUIDADO = vm.CARE_VL_VALOR;
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LiquidarCR(ContaReceberViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(idAss), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllItens(idAss).Where(x => x.CECU_IN_TIPO == 1).OrderBy(x => x.CECU_NM_NOME).ToList<CENTRO_CUSTO>(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(idAss), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(idAss), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(idAss), "PERI_CD_ID", "PERI_NM_NOME");
            List<SelectListItem> tipoRec = new List<SelectListItem>();
            tipoRec.Add(new SelectListItem() { Text = "Recebimento Recorrente", Value = "1" });
            tipoRec.Add(new SelectListItem() { Text = "Parcelamento", Value = "2" });
            ViewBag.Pagamento = new SelectList(tipoRec, "Value", "Text");
            ViewBag.Liquida = 1;
            Session["LiquidaCR"] = 1;
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    var vmVolta = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>((CONTA_RECEBER)Session["ContaReceber"]);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONTA_RECEBER item = Mapper.Map<ContaReceberViewModel, CONTA_RECEBER>(vm);
                    DTO_CR dto = Liquidar();
                    Int32 volta = crApp.ValidateEdit(item, (CONTA_RECEBER)Session["ContaReceber"], usuarioLogado, dto);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCR"] = 10;
                        return View(vmVolta);
                    }
                    if (volta == 2)
                    {
                        Session["MensCR"] = 11;
                        return View(vmVolta);
                    }
                    if (volta == 3)
                    {
                        Session["MensCR"] = 12;
                        return View(vmVolta);
                    }
                    if (volta == 4)
                    {
                        Session["MensCR"] = 13;
                        return View(vmVolta);
                    }
                    if (volta == 5)
                    {
                        Session["MensCR"] = 14;
                        return View(vmVolta);
                    }
                    if (volta == 6)
                    {
                        Session["MensCR"] = 15;
                        return View(vmVolta);
                    }
                    if (volta == 7)
                    {
                        Session["MensCR"] = 16;
                        return View(vmVolta);
                    }
                    if (volta == 8)
                    {
                        Session["MensCR"] = 17;
                        return View(vmVolta);
                    }
                    if (volta == 9)
                    {
                        Session["MensCR"] = 18;
                        return View(vmVolta);
                    }
                    if (volta == 10)
                    {
                        Session["MensCR"] = 19;
                        return View(vmVolta);
                    }

                    // Sucesso
                    listaCRMaster = new List<CONTA_RECEBER>();
                    Session["ListaCR"] = null;
                    return RedirectToAction("MontarTelaCR");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult IncluirRateioCC(ContaReceberViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            try
            {
                // Executa a operação
                Int32? cc = vm.CECU_CD_RATEIO;
                Int32? perc = vm.CARE_VL_PERCENTUAL;
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                CONTA_RECEBER item = crApp.GetItemById(vm.CARE_CD_ID);
                Int32 volta = crApp.IncluirRateioCC(item, cc, perc, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["ErroSoma"] = 1;
                    Session["MensCR"] = 30;
                    return RedirectToAction("VoltarAnexoCR");
                }

                // Sucesso
                Session["IdVoltaTrab"] = 2;
                return RedirectToAction("VoltarAnexoCR");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("VoltarAnexoCR");
            }
        }

        [HttpGet]
        public ActionResult ExcluirRateio(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica se tem usuario logado
            CONTA_RECEBER cp = (CONTA_RECEBER)Session["ContaReceber"];
            CONTA_RECEBER_RATEIO rl = ratApp.GetItemById(id);
            Int32 volta = ratApp.ValidateDelete(rl);
            Session["IdVoltaTrab"] = 2;
            return RedirectToAction("VoltarAnexoCR");
        }

        public ActionResult GerarRelatorioLista()
        {
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "ContaReceberLista" + "_" + data + ".pdf";
            List<CONTA_RECEBER> lista = ((List<CONTA_RECEBER>)Session["ListaCR"]);
            CONTA_RECEBER filtro = (CONTA_RECEBER)Session["filtroCR"];
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Imagens/Base/favicon_SystemBR.jpg"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Contas a Receber - Listagem", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Grid
            table = new PdfPTable(7);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Itens de Conta a Receber selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 7;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Cliente", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Centro de Custo", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Emissão", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Valor", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Descrição", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Saldo", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (CONTA_RECEBER item in lista)
            {
                if (item.CLIENTE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CLIENTE.CLIE_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CENTRO_CUSTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CENTRO_CUSTO.CECU_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CARE_DT_LANCAMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CARE_DT_LANCAMENTO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(item.CARE_VL_VALOR.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.CARE_DS_DESCRICAO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.CARE_DT_VENCIMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CARE_DT_VENCIMENTO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(item.CARE_VL_SALDO.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line2);

            // Rodapé
            Chunk chunk1 = new Chunk("Parâmetros de filtro: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            String parametros = String.Empty;
            Int32 ja = 0;
            if (filtro != null)
            {
                if (filtro.CLIE_CD_ID != null)
                {
                    parametros += "Cliente: " + cliApp.GetItemById(filtro.CLIE_CD_ID.Value);
                    ja = 1;
                }
                if (filtro.CARE_DT_LANCAMENTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data da Emissão: " + filtro.CARE_DT_LANCAMENTO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data da Emissão: " + filtro.CARE_DT_LANCAMENTO.Value.ToShortDateString();
                    }
                }
                if (filtro.CARE_DT_VENCIMENTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data de Vencimento Inicial: " + filtro.CARE_DT_VENCIMENTO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data de Vencimento Inicial: " + filtro.CARE_DT_VENCIMENTO;
                    }
                }
                if (Session["vencFinal"] != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data de Vencimento Final: " + (String)Session["vencFinal"];
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data de Vencimento Final: " + (String)Session["vencFinal"];
                    }
                }
                if (filtro.CARE_DS_DESCRICAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Descrição (Histórico): " + filtro.CARE_DS_DESCRICAO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Descrição (Histórico): " + filtro.CARE_DS_DESCRICAO;
                    }
                }
                if (filtro.CARE_IN_ABERTOS != null)
                {
                    String af = filtro.CARE_IN_ABERTOS == 1 ? "Abertos" : "Fechados";
                    if (ja == 0)
                    {
                        parametros += "Abertos/Fechados: " + af;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Abertos/Fechados: " + af;
                    }
                }
                if (ja == 0)
                {
                    parametros = "Nenhum filtro definido.";
                }
            }
            else
            {
                parametros = "Nenhum filtro definido.";
            }
            Chunk chunk = new Chunk(parametros, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk);

            // Linha Horizontal
            Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line3);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("MontarTelaCR");
        }
    }
}