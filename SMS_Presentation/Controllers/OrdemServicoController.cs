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

namespace ERP_CRM_Solution.Controllers
{
    public class OrdemServicoController : Controller
    {
        private readonly IOrdemServicoAppService baseApp;
        private readonly IClienteAppService clieApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IProdutoAppService proApp;
        private readonly IDepartamentoAppService depApp;
        private readonly IConfiguracaoAppService conApp;
        private readonly IServicoAppService servApp;
        private readonly ICategoriaOrdemServicoAppService cosApp;
        private readonly IAtendimentoAppService atenApp;
        private readonly IAgendaAppService agenApp;
        private readonly IOrdemServicoAgendaAppService osAgenApp;
        private readonly IFormaPagamentoAppService fopaApp;
        private readonly IOrdemServicoProdutoAppService ospApp;
        private readonly IOrdemServicoServicoAppService ossApp;
        private readonly IFilialAppService filiApp;
        private readonly IProdutoEstoqueFilialAppService pefApp;

        String extensao = String.Empty;
        ORDEM_SERVICO objeto = new ORDEM_SERVICO();
        ORDEM_SERVICO objetoAntes = new ORDEM_SERVICO();
        List<ORDEM_SERVICO> listaMaster = new List<ORDEM_SERVICO>();

        public OrdemServicoController(IOrdemServicoAppService baseApps, IClienteAppService clieApps, IUsuarioAppService usuApps, IProdutoAppService proApps, IDepartamentoAppService depApps, IConfiguracaoAppService conApps, IServicoAppService servApps, ICategoriaOrdemServicoAppService cosApps, IAtendimentoAppService atenApps, IAgendaAppService agenApps, IOrdemServicoAgendaAppService osAgenApps, IFormaPagamentoAppService fopaApps, IOrdemServicoProdutoAppService ospApps, IOrdemServicoServicoAppService ossApps, IFilialAppService filiApps, IProdutoEstoqueFilialAppService pefApps)
        {
            baseApp = baseApps;
            clieApp = clieApps;
            usuApp = usuApps;
            proApp = proApps;
            depApp = depApps;
            conApp = conApps;
            servApp = servApps;
            cosApp = cosApps;
            atenApp = atenApps;
            agenApp = agenApps;
            osAgenApp = osAgenApps;
            fopaApp = fopaApps;
            ospApp = ospApps;
            ossApp = ossApps;
            filiApp = filiApps;
            pefApp = pefApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
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

        public ActionResult VoltarDashboard()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult DashboardAdministracao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarAdmin", "BaseAdmin");
        }

        public ActionResult VoltarAnexoOrdemServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaOrdemServico");
        }

        public ActionResult VoltarBaseOrdemServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["IdAtendimento"] != 0)
            {
                return RedirectToAction("EditarAtendimento", "Atendimento", new { id = (Int32)Session["IdAtendimento"] });
            }

            if (Session["OSProd"] != null && (Int32)Session["OSProd"] == 1)
            {
                Session["OSProd"] = 0;
                return RedirectToAction("EditarOrdemServico", "OrdemServico", new { id = (Int32)Session["IdVolta"] });
            }

            if (Session["OSServ"] != null && (Int32)Session["OSServ"] == 1)
            {
                Session["OSServ"] = 0;
                return RedirectToAction("EditarOrdemServico", "OrdemServico", new { id = (Int32)Session["IdVolta"] });
            } 

            return RedirectToAction("MontarTelaOrdemServico");
        }

        [HttpGet]
        public ActionResult EditarAgenda(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EditarAgenda", "Agenda", new { id = id });
        }

        [HttpPost]
        public void Orcamento(Int32 id)
        {
            ORDEM_SERVICO item = baseApp.GetItemById(id);
            ORDEM_SERVICO editar = new ORDEM_SERVICO
            {
                ORSE_CD_ID = item.ORSE_CD_ID,
                ATEN_CD_ID = item.ATEN_CD_ID,
                ORSE_DT_CRIACAO = item.ORSE_DT_CRIACAO,
                ORSE_TX_INFORMACOES = item.ORSE_TX_INFORMACOES,
                ORSE_DT_PREVISTA = item.ORSE_DT_PREVISTA,
                ORSE_NR_NUMERO = item.ORSE_NR_NUMERO,
                ORSE_NR_NOTA_FISCAL = item.ORSE_NR_NOTA_FISCAL,
                ORSE_DT_CANCELAMENTO = item.ORSE_DT_CANCELAMENTO,
                ORSE_DS_MOTIVO_CANCELAMENTO = item.ORSE_DS_MOTIVO_CANCELAMENTO,
                ORSE_DT_ENCERRAMENTO = item.ORSE_DT_ENCERRAMENTO,
                ORSE_DS_ENCERRAMENTO = item.ORSE_DS_ENCERRAMENTO,
                ORSE_IN_VISITA = item.ORSE_IN_VISITA,
                ORSE_IN_ATIVO = item.ORSE_IN_ATIVO,
                ORSE_IN_STATUS = item.ORSE_IN_STATUS,
                ORSE_TX_OBSERVACOES = item.ORSE_TX_OBSERVACOES,
                ASSI_CD_ID = item.ASSI_CD_ID,
                USUA_CD_ID = item.USUA_CD_ID,
                CLIE_CD_ID = item.CLIE_CD_ID,
                PROD_CD_ID = item.PROD_CD_ID,
                SERV_CD_ID = item.SERV_CD_ID,
                DEPT_CD_ID = item.DEPT_CD_ID,
                ORSE_DT_INICIO = item.ORSE_DT_INICIO,
                ORSE_DT_FINAL = item.ORSE_DT_FINAL,
                ORSE_DS_DESCRICAO = item.ORSE_DS_DESCRICAO,
                ORSE_IN_PRIORIDADE = item.ORSE_IN_PRIORIDADE,
                CAOS_CD_ID = item.CAOS_CD_ID,
                ORSE_IN_VENDEDOR = item.ORSE_IN_VENDEDOR,
                ORSE_NM_EQUIPAMENTO = item.ORSE_NM_EQUIPAMENTO,
                ORSE_NR_EQUIPAMENTO = item.ORSE_NR_EQUIPAMENTO,
                ORSE_IN_TECNICO = item.ORSE_IN_TECNICO,
                ORSE_IN_ORCAMENTO = item.ORSE_IN_ORCAMENTO == 1 ? 0 : 1,
                FILI_CD_ID = item.FILI_CD_ID
            };

            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Session["OS"] = null;
            listaMaster = new List<ORDEM_SERVICO>();
            Session["ListaOrdemServico"] = null;

            Int32 volta = baseApp.ValidateEdit(editar, item, usuario);
        }

        [HttpPost]
        public JsonResult GetCustoProduto(Int32 id)
        {
            String preco = String.Empty;
            String promo = String.Empty;
            Hashtable result = new Hashtable();

            List<PRODUTO_TABELA_PRECO> ptp = proApp.GetItemById(id).PRODUTO_TABELA_PRECO.ToList();
            PRODUTO_TABELA_PRECO precos = ptp.First(x => x.FILI_CD_ID == (objetoAntes.FILI_CD_ID == null ? (Int32)Session["idFilial"] : objetoAntes.FILI_CD_ID));

            try
            {
                if (ptp.Count != 0 && precos != null && precos.PRTP_VL_PRECO != null)
                {
                    preco = precos.PRTP_VL_PRECO.ToString().Replace(".", ",");
                }
                else
                {
                    preco = null;
                }
                if (ptp.Count != 0 && precos != null && precos.PRTP_VL_PRECO_PROMOCAO != null)
                {
                    promo = precos.PRTP_VL_PRECO_PROMOCAO.ToString().Replace(".", ",");
                }
                else
                {
                    promo = null;
                }
            }
            finally
            {    
                result.Add("preco", preco);
                result.Add("promo", promo);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetCustoServico(Int32 id)
        {
            String preco = String.Empty;
            String promo = String.Empty;
            Hashtable result = new Hashtable();

            List<SERVICO_TABELA_PRECO> stp = servApp.GetItemById(id).SERVICO_TABELA_PRECO.ToList();
            SERVICO_TABELA_PRECO precos = stp.First(x => x.FILI_CD_ID == (objetoAntes.FILI_CD_ID == null ? (Int32)Session["idFilial"] : objetoAntes.FILI_CD_ID));

            try
            {
                if (stp.Count != 0 && precos != null && precos.SETP_VL_PRECO != null)
                {
                    preco = precos.SETP_VL_PRECO.ToString().Replace(".", ",");
                }
                else
                {
                    preco = null;
                }
                if (stp.Count != 0 && precos != null && precos.SETP_VL_PRECO_PROMOCAO != null)
                {
                    promo = precos.SETP_VL_PRECO_PROMOCAO.ToString().Replace(".", ",");
                }
                else
                {
                    promo = null;
                }
            }
            finally
            {
                result.Add("preco", preco);
                result.Add("promo", promo);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult CalculaDataPrevista(Int32? id)
        {
            Hashtable result = new Hashtable();

            if (id != null)
            {
                CATEGORIA_ORDEM_SERVICO ca = cosApp.GetItemById((Int32)id);
                result.Add("dtPrevista", DateTime.Now.AddDays(ca.CAOS_IN_SLA == null ? 0 : (Int32)ca.CAOS_IN_SLA).ToString("dd/MM/yyyy HH:mm"));
            }
            else
            {
                result.Add("dtPrevista", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetOrdemServico()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            List<ORDEM_SERVICO> lstAt = new List<ORDEM_SERVICO>();

            // Carrega listas
            if (Session["ListaOrdemServico"] == null)
            {
                listaMaster = baseApp.GetAllItens(idAss);
                Session["ListaOrdemServico"] = listaMaster;
            }

            var result = new List<Hashtable>();

            foreach (var item in lstAt)
            {
                var hash = new Hashtable();
                hash.Add("ORSE_IN_STATUS", item.ORSE_IN_STATUS);
                hash.Add("ATEN_CD_ID", item.ORSE_CD_ID);
                hash.Add("ORSE_DS_DESCRICAO", item.ORSE_DS_DESCRICAO);
                if (item.CLIENTE != null)
                {
                    hash.Add("CLIE_NM_NOME", item.CLIENTE.CLIE_NM_NOME);
                }
                hash.Add("ORSE_DT_INICIO", item.ORSE_DT_INICIO.Value.ToString("dd/MM/yyyy"));
                hash.Add("ORSE_DT_PREVISTA", item.ORSE_DT_PREVISTA == null ? " " : item.ORSE_DT_PREVISTA.Value.ToString("dd/MM/yyyy"));

                result.Add(hash);
            }

            return Json(result);
        }

        [HttpGet]
        public ActionResult MontarTelaOrdemServicoFiltro(Int32? id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (id != null)
            {
                ORDEM_SERVICO item = new ORDEM_SERVICO 
                {
                    ATEN_CD_ID = id.Value
                };
                FiltrarOrdemServico(item);
            }

            return RedirectToAction("MontarTelaOrdemServico");
        }

        [HttpGet]
        public ActionResult MontarTelaOrdemServico()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
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
            if (Session["ListaOrdemServico"] == null && Session["OS"] == null)
            {
                Session["OS"] = 1;
                listaMaster = baseApp.GetAllItens(idAss);
                Session["ListaOrdemServico"] = listaMaster;
            }

            if (Session["FiltroOrdemServico"] != null)
            {
                FiltrarOrdemServico((ORDEM_SERVICO)Session["FiltroOrdemServico"]);
            }

            if ((Int32)Session["IdOrdemServico"] != 0)
            {
                ViewBag.CodigoOS = (Int32)Session["IdOrdemServico"];
                Session["IdOrdemServico"] = 0;
            }

            ViewBag.Title = "Ordem Servico";
            ViewBag.Listas = ((List<ORDEM_SERVICO>)Session["ListaOrdemServico"]).OrderByDescending(x => x.ORSE_DT_CRIACAO).ToList<ORDEM_SERVICO>();

            // Indicadores
            ViewBag.OrdensServico = ((List<ORDEM_SERVICO>)Session["ListaOrdemServico"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            
            // Carrega listas
            ViewBag.Categorias = new SelectList(cosApp.GetAllItens(idAss), "CAOS_CD_ID", "CAOS_NM_NOME");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(x => x.USUA_NM_NOME).ToList<USUARIO>(), "USUA_CD_ID", "USUA_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Para Aprovação", Value = "1" });
            status.Add(new SelectListItem() { Text = "Finalizado", Value = "3" });
            status.Add(new SelectListItem() { Text = "Cancelado", Value = "4" });
            status.Add(new SelectListItem() { Text = "Em Aprovação", Value = "5" });
            status.Add(new SelectListItem() { Text = "Aprovado", Value = "6" });
            status.Add(new SelectListItem() { Text = "Recusada", Value = "7" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            ViewBag.Departamentos = new SelectList(depApp.GetAllItens(idAss).OrderBy(x => x.DEPT_NM_NOME).ToList<DEPARTAMENTO>(), "DEPT_CD_ID", "DEPT_NM_NOME");
            ViewBag.Produtos = new SelectList(proApp.GetAllItens(idAss).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");

            if (Session["MensOrdemServico"] != null)
            {
                if ((Int32)Session["MensOrdemServico"] == 2)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensOrdemServico"] == 3)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0026", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensOrdemServico"] == 4)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0027", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensOrdemServico"] == 50)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0081", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensOrdemServico"] == 10)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0114", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensOrdemServico"] == 11)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0072", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensOrdemServico"] == 44)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0123", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensOrdemServico"] == 70)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0118", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["IdAtendimento"] = 0;
            Session["OrdemServico"] = null;
            if (Session["FiltroOrdemServico"] != null)
            {
                objeto = (ORDEM_SERVICO)Session["FiltroOrdemServico"];
                if (objeto.ORSE_DT_CRIACAO == DateTime.MinValue)
                {
                    objeto.ORSE_DT_CRIACAO = DateTime.Now;
                }
            }
            else
            {
                objeto = new ORDEM_SERVICO();
                objeto.ORSE_DT_CRIACAO = DateTime.Now;
            }
            return View(objeto);
        }

        public ActionResult MostrarTudoOrdemServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["ListaOrdemServico"] = listaMaster;
            Session["FiltroOrdemServico"] = new ORDEM_SERVICO();
            return RedirectToAction("MontarTelaOrdemServico");
        }

        [HttpPost]
        public ActionResult FiltrarOrdemServico(ORDEM_SERVICO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executa a operação
                Session["FiltroOrdemServico"] = item;
                List<ORDEM_SERVICO> listaObj = new List<ORDEM_SERVICO>();
                Int32 volta = baseApp.ExecuteFilter(item.CAOS_CD_ID,  item.CLIE_CD_ID,  item.USUA_CD_ID,  item.ORSE_DT_CRIACAO, item.ORSE_IN_STATUS,  item.DEPT_CD_ID,  item.SERV_CD_ID, item.PROD_CD_ID, item.ATEN_CD_ID, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensOrdemServico"] = 1;
                }

                // Sucesso
                listaMaster = listaObj;
                Session["ListaOrdemServico"] = listaObj;
                return RedirectToAction("MontarTelaOrdemServico");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaOrdemServico");

            }
        }

        public ActionResult RetirarFiltroOrdemServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["OS"] = null;
            Session["ListaOrdemServico"] = null;
            Session["FiltroOrdemServico"] = null;
            return RedirectToAction("MontarTelaOrdemServico");
        }

        [HttpGet]
        public ActionResult MontarTelaOrdemServicoKanban()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            ViewBag.Title = "Ordem Servico";

            // Abre view
            objeto = new ORDEM_SERVICO();
            return View(objeto);
        }

        [HttpGet]
        public ActionResult IncluirOrdemServico()
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
                    Session["MensOrdemServico"] = 2;
                    return RedirectToAction("MontarTelaOrdemServico");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica possibilidade
            Int32 num = baseApp.GetAllItens(idAss).Count;
            if ((Int32)Session["NumOrdemServico"] <= num)
            {
                Session["MensOrdemServico"] = 70;
                return RedirectToAction("MontarTelaOrdemServico");
            }

            // Prepara listas
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Clientes = new SelectList(clieApp.GetAllItens(idAss), "CLIE_CD_ID", "CLIE_NM_NOME");
            Session["Clientes"] = clieApp.GetAllItens(idAss);
            ViewBag.Produtos = new SelectList(proApp.GetAllItens(idAss).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Categorias = new SelectList(cosApp.GetAllItens(idAss), "CAOS_CD_ID", "CAOS_NM_NOME");
            ViewBag.Depts = new SelectList(depApp.GetAllItens(idAss).OrderBy(x => x.DEPT_NM_NOME).ToList<DEPARTAMENTO>(), "DEPT_CD_ID", "DEPT_NM_NOME");
            ViewBag.Servicos = new SelectList(servApp.GetAllItens(idAss).OrderBy(x => x.SERV_NM_NOME).ToList<SERVICO>(), "SERV_CD_ID", "SERV_NM_NOME");
            ViewBag.Atendimentos = new SelectList(atenApp.GetAllItens(idAss).ToList<ATENDIMENTO>(), "ATEN_CD_ID", "ATEN_NR_NUMERO");
            ViewBag.Tecnicos = new SelectList(usuApp.GetAllTecnicos(idAss).ToList<USUARIO>(), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.FormaPagamento = new SelectList(fopaApp.GetAllItens(idAss), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Filiais = new SelectList(filiApp.GetAllItens(idAss), "FILI_CD_ID", "FILI_NM_NOME");

            // Prepara view
            CONFIGURACAO conf = conApp.GetItemById(idAss);
            ORDEM_SERVICO item = new ORDEM_SERVICO();

            if (Session["OrdemServico"] != null)
            {
                item = (ORDEM_SERVICO)Session["OrdemServico"];
                Session["OrdemServico"] = null;
            }
            Session["VoltaCliente"] = 7;
            OrdemServicoViewModel vm = Mapper.Map<ORDEM_SERVICO, OrdemServicoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.ORSE_DT_CRIACAO = DateTime.Now;
            vm.ORSE_DT_INICIO = DateTime.Now;
            vm.ORSE_IN_ATIVO = 1;
            vm.ORSE_IN_STATUS = 1;
            vm.ORSE_DT_PREVISTA = DateTime.Today.Date.AddDays(conf.CONF_NR_DIAS_ATENDIMENTO.Value).AddHours(12);
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult IncluirOrdemServico(OrdemServicoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara listas
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Clientes = new SelectList(clieApp.GetAllItens(idAss), "CLIE_CD_ID", "CLIE_NM_NOME");
            Session["Clientes"] = clieApp.GetAllItens(idAss);
            ViewBag.Produtos = new SelectList(proApp.GetAllItens(idAss).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Categorias = new SelectList(cosApp.GetAllItens(idAss), "CAOS_CD_ID", "CAOS_NM_NOME");
            ViewBag.Depts = new SelectList(depApp.GetAllItens(idAss).OrderBy(x => x.DEPT_NM_NOME).ToList<DEPARTAMENTO>(), "DEPT_CD_ID", "DEPT_NM_NOME");
            ViewBag.Servicos = new SelectList(servApp.GetAllItens(idAss).OrderBy(x => x.SERV_NM_NOME).ToList<SERVICO>(), "SERV_CD_ID", "SERV_NM_NOME");
            ViewBag.Atendimentos = new SelectList(atenApp.GetAllItens(idAss).ToList<ATENDIMENTO>(), "ATEN_CD_ID", "ATEN_NR_NUMERO");
            ViewBag.Tecnicos = new SelectList(usuApp.GetAllTecnicos(idAss).ToList<USUARIO>(), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.FormaPagamento = new SelectList(fopaApp.GetAllItens(idAss), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Filiais = new SelectList(filiApp.GetAllItens(idAss), "FILI_CD_ID", "FILI_NM_NOME");

            if (vm.PROD_CD_ID != null && vm.SERV_CD_ID != null)
            {
                ModelState.AddModelError("", "PRODUTO e SERVIÇO específicados, apenas um pode ser preenchido");
                return View(vm);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    ORDEM_SERVICO item = Mapper.Map<OrdemServicoViewModel, ORDEM_SERVICO>(vm);
                    item.ORSE_DT_CRIACAO = DateTime.Now;
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0121", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0114", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    ORDEM_SERVICO itemEdit = new ORDEM_SERVICO
                    {
                        ORSE_CD_ID = item.ORSE_CD_ID,
                        ATEN_CD_ID = item.ATEN_CD_ID,
                        ORSE_DT_CRIACAO = item.ORSE_DT_CRIACAO,
                        ORSE_TX_INFORMACOES = item.ORSE_TX_INFORMACOES,
                        ORSE_DT_PREVISTA = item.ORSE_DT_PREVISTA,
                        ORSE_NR_NUMERO = item.ORSE_CD_ID.ToString(),
                        ORSE_NR_NOTA_FISCAL = item.ORSE_NR_NOTA_FISCAL,
                        ORSE_DT_CANCELAMENTO = item.ORSE_DT_CANCELAMENTO,
                        ORSE_DS_MOTIVO_CANCELAMENTO = item.ORSE_DS_MOTIVO_CANCELAMENTO,
                        ORSE_DT_ENCERRAMENTO = item.ORSE_DT_ENCERRAMENTO,
                        ORSE_DS_ENCERRAMENTO = item.ORSE_DS_ENCERRAMENTO,
                        ORSE_IN_VISITA = item.ORSE_IN_VISITA,
                        ORSE_IN_ATIVO = item.ORSE_IN_ATIVO,
                        ORSE_IN_STATUS = item.ORSE_IN_STATUS,
                        ORSE_TX_OBSERVACOES = item.ORSE_TX_OBSERVACOES,
                        ASSI_CD_ID = item.ASSI_CD_ID,
                        USUA_CD_ID = item.USUA_CD_ID,
                        CLIE_CD_ID = item.CLIE_CD_ID,
                        PROD_CD_ID = item.ATENDIMENTO == null ? null : item.ATENDIMENTO.PROD_CD_ID,
                        SERV_CD_ID = item.ATENDIMENTO == null ? null : item.ATENDIMENTO.SERV_CD_ID,
                        DEPT_CD_ID = item.DEPT_CD_ID,
                        ORSE_DT_INICIO = item.ORSE_DT_INICIO,
                        ORSE_DT_FINAL = item.ORSE_DT_FINAL,
                        ORSE_DS_DESCRICAO = item.ORSE_DS_DESCRICAO,
                        ORSE_IN_PRIORIDADE = item.ORSE_IN_PRIORIDADE,
                        CAOS_CD_ID = item.CAOS_CD_ID,
                        ORSE_IN_VENDEDOR = item.ORSE_IN_VENDEDOR,
                        ORSE_NM_EQUIPAMENTO = item.ORSE_NM_EQUIPAMENTO,
                        ORSE_NR_EQUIPAMENTO = item.ORSE_NR_EQUIPAMENTO,
                        ORSE_IN_ORCAMENTO = item.ORSE_IN_ORCAMENTO,
                        ORSE_IN_TECNICO = item.ORSE_IN_TECNICO,
                        FOPA_CD_ID = item.FOPA_CD_ID,
                        FILI_CD_ID = item.FILI_CD_ID
                    };
                    Int32 voltaEdit = baseApp.ValidateEdit(itemEdit, item, usuarioLogado);

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/OrdemServico/" + item.ORSE_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    Session["MensOrdemServico"] = 0;
                    Session["OS"] = null;
                    listaMaster = new List<ORDEM_SERVICO>();
                    Session["ListaOrdemServico"] = null;
                    Session["IdVolta"] = item.ORSE_CD_ID;
                    Session["IdOrdemServico"] = item.ORSE_CD_ID;

                    //Anexos
                    if (Session["FileQueueOrdemServico"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueOrdemServico"];

                        foreach (var file in fq)
                        {
                            UploadFileQueueOrdemServico(file);
                        }

                        Session["FileQueueOrdemServico"] = null;
                    }

                    // retorno
                    if ((Int32)Session["IdAtendimento"] != 0)
                    {
                        return RedirectToAction("EditarAtendimento", "Atendimento", new { id = (Int32)Session["IdAtendimento"] });
                    }
                    return RedirectToAction("MontarTelaOrdemServico");
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

        [HttpPost]
        public void UploadFileToSession(IEnumerable<HttpPostedFileBase> files)
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

                queue.Add(f);
            }

            Session["FileQueueOrdemServico"] = queue;
        }


        [HttpPost]
        public ActionResult UploadFileQueueOrdemServico(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            if (file == null)
            {
                Session["MensOrdemServico"] = 50;
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoOrdemServico");
            }

            ORDEM_SERVICO item = baseApp.GetById((Int32)Session["IdVolta"]);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                Session["MensOrdemServico"] = 51;
                return RedirectToAction("VoltarAnexoOrdemServico");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/OrdemServico/" + item.ORSE_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            ORDEM_SERVICO_ANEXO foto = new ORDEM_SERVICO_ANEXO();
            foto.ORSX_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.ORSX_DT_ANEXO = DateTime.Today;
            foto.ORSX_IN_ATIVO = 1;
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
            foto.ORSX_IN_TIPO = tipo;
            foto.ORSX_NM_TITULO = fileName;
            foto.ORSE_CD_ID = item.ORSE_CD_ID;

            if (item.ORDEM_SERVICO_ANEXO == null)
            {
                item.ORDEM_SERVICO_ANEXO = new List<ORDEM_SERVICO_ANEXO>();
            }

            item.ORDEM_SERVICO_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usu);
            return RedirectToAction("VoltarAnexoOrdemServico");
        }

        [HttpPost]
        public ActionResult UploadFileOrdemServico(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            if (file == null)
            {
                Session["MensOrdemServico"] = 50;
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoOrdemServico");
            }

            ORDEM_SERVICO item = baseApp.GetById((Int32)Session["IdVolta"]);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];
            ORDEM_SERVICO itemEdit = new ORDEM_SERVICO
            {
                ORSE_CD_ID = item.ORSE_CD_ID,
                ATEN_CD_ID = item.ATEN_CD_ID,
                CAOS_CD_ID = item.CAOS_CD_ID,
                ASSI_CD_ID = item.ASSI_CD_ID,
                USUA_CD_ID = item.USUA_CD_ID,
                CLIE_CD_ID = item.CLIE_CD_ID,
                PROD_CD_ID = item.PROD_CD_ID,
                SERV_CD_ID = item.SERV_CD_ID,
                DEPT_CD_ID = item.DEPT_CD_ID,
                ORSE_DT_CRIACAO = item.ORSE_DT_CRIACAO,
                ORSE_TX_INFORMACOES = item.ORSE_TX_INFORMACOES,
                ORSE_DT_PREVISTA = item.ORSE_DT_PREVISTA,
                ORSE_NR_NUMERO = item.ORSE_NR_NUMERO,
                ORSE_NR_NOTA_FISCAL = item.ORSE_NR_NOTA_FISCAL,
                ORSE_DT_CANCELAMENTO = item.ORSE_DT_CANCELAMENTO,
                ORSE_DS_MOTIVO_CANCELAMENTO = item.ORSE_DS_MOTIVO_CANCELAMENTO,
                ORSE_DT_ENCERRAMENTO = item.ORSE_DT_ENCERRAMENTO,
                ORSE_DS_ENCERRAMENTO = item.ORSE_DS_ENCERRAMENTO,
                ORSE_IN_VISITA = item.ORSE_IN_VISITA,
                ORSE_IN_ATIVO = item.ORSE_IN_ATIVO,
                ORSE_IN_STATUS = item.ORSE_IN_STATUS,
                ORSE_TX_OBSERVACOES = item.ORSE_TX_OBSERVACOES,
                ORSE_DT_INICIO = item.ORSE_DT_INICIO,
                ORSE_DT_FINAL = item.ORSE_DT_FINAL,
                ORSE_DS_DESCRICAO = item.ORSE_DS_DESCRICAO,
                ORSE_IN_PRIORIDADE = item.ORSE_IN_PRIORIDADE,
                ORSE_IN_VENDEDOR = item.ORSE_IN_VENDEDOR,
                ORSE_NM_EQUIPAMENTO = item.ORSE_NM_EQUIPAMENTO,
                ORSE_NR_EQUIPAMENTO = item.ORSE_NR_EQUIPAMENTO,
                ORSE_IN_TECNICO = item.ORSE_IN_TECNICO,
                ORSE_IN_ORCAMENTO = item.ORSE_IN_ORCAMENTO,
                FOPA_CD_ID = item.FOPA_CD_ID,
                FILI_CD_ID = item.FILI_CD_ID
            };
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                Session["MensOrdemServico"] = 51;
                return RedirectToAction("VoltarAnexoOrdemServico");
            }

            String caminho = "/Imagens/" + idAss.ToString() + "/OrdemServico/" + item.ORSE_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            ORDEM_SERVICO_ANEXO foto = new ORDEM_SERVICO_ANEXO();
            foto.ORSX_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.ORSX_DT_ANEXO = DateTime.Today;
            foto.ORSX_IN_ATIVO = 1;
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
            foto.ORSX_IN_TIPO = tipo;
            foto.ORSX_NM_TITULO = fileName;
            foto.ORSE_CD_ID = item.ORSE_CD_ID;

            itemEdit.ORDEM_SERVICO_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usu);
            return RedirectToAction("VoltarAnexoOrdemServico");
        }

        [HttpGet]
        public ActionResult CancelarOrdemServico(Int32? id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            ORDEM_SERVICO item = new ORDEM_SERVICO();
            if (id == null)
            {
                item = baseApp.GetItemById((Int32)Session["IdVolta"]);
            }
            else
            {
                item = baseApp.GetItemById((Int32)id);
            }
            objetoAntes = item;
            Session["OrdemServivo"] = item;
            OrdemServicoViewModel vm = Mapper.Map<ORDEM_SERVICO, OrdemServicoViewModel>(item);
            vm.ORSE_DT_CANCELAMENTO = DateTime.Now;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CancelarOrdemServico(OrdemServicoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    ORDEM_SERVICO item = Mapper.Map<OrdemServicoViewModel, ORDEM_SERVICO>(vm);
                    item.ORSE_IN_STATUS = 4;
                    item.ORSE_IN_ATIVO = 1;
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    Session["OS"] = null;
                    Session["OrdemServivo"] = null;
                    listaMaster = new List<ORDEM_SERVICO>();
                    Session["ListaOrdemServivo"] = null;
                    Session["MensOrdemServico"] = 0;
                    if ((Int32)Session["VoltaOrdemServivo"] == 2)
                    {
                        return RedirectToAction("EditarOrdemServico", new { id = (Int32)Session["IdVolta"] });
                    }
                    return RedirectToAction("MontarTelaOrdemServico");
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
        public ActionResult EncerrarOrdemServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            ORDEM_SERVICO item = baseApp.GetItemById((Int32)Session["IdVolta"]);
            objetoAntes = item;
            Session["OrdemServico"] = item;
            OrdemServicoViewModel vm = Mapper.Map<ORDEM_SERVICO, OrdemServicoViewModel>(item);
            vm.ORSE_DT_ENCERRAMENTO = DateTime.Now;
            vm.ORSE_IN_STATUS = 3;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EncerrarOrdemServico(OrdemServicoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    ORDEM_SERVICO item = Mapper.Map<OrdemServicoViewModel, ORDEM_SERVICO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    Session["OS"] = null;
                    Session["OrdemServico"] = null;
                    Session["MensOrdemServico"] = 0;
                    listaMaster = new List<ORDEM_SERVICO>();
                    Session["ListaOrdemServico"] = null;
                    return RedirectToAction("MontarTelaOrdemServico");
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
        public ActionResult ExcluirOrdemServico(Int32 id)
        {
            // Prepara view
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            ORDEM_SERVICO item = baseApp.GetItemById(id);
            OrdemServicoViewModel vm = Mapper.Map<ORDEM_SERVICO, OrdemServicoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ExcluirOrdemServico(OrdemServicoViewModel vm)
        {
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                ORDEM_SERVICO item = Mapper.Map<OrdemServicoViewModel, ORDEM_SERVICO>(vm);
                Int32 volta = baseApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensOrdemServico"] = 44;
                    return RedirectToAction("MontarTelaOrdemServico");
                }

                // Sucesso
                Session["MensOrdemServico"] = 0;
                listaMaster = new List<ORDEM_SERVICO>();
                Session["ListaOrdemServico"] = null;
                return RedirectToAction("MontarTelaOrdemServico");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult ReativarOrdemServico(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            ORDEM_SERVICO item = baseApp.GetItemById(id);
            OrdemServicoViewModel vm = Mapper.Map<ORDEM_SERVICO, OrdemServicoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ReativarOrdemServico(OrdemServicoViewModel vm)
        {

            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                ORDEM_SERVICO item = Mapper.Map<OrdemServicoViewModel, ORDEM_SERVICO>(vm);
                Int32 volta = baseApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMaster = new List<ORDEM_SERVICO>();
                Session["ListaOrdemServico"] = null;
                return RedirectToAction("MontarTelaOrdemServico");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult EditarOrdemServico(Int32 id)
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
                    Session["MensOrdemServico"] = 2;
                    return RedirectToAction("MontarTelaOrdemServico");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Clientes = new SelectList(clieApp.GetAllItens(idAss), "CLIE_CD_ID", "CLIE_NM_NOME");
            Session["Clientes"] = clieApp.GetAllItens(idAss);
            ViewBag.Produtos = new SelectList(proApp.GetAllItens(idAss).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Categorias = new SelectList(cosApp.GetAllItens(idAss), "CAOS_CD_ID", "CAOS_NM_NOME");
            ViewBag.Depts = new SelectList(depApp.GetAllItens(idAss).OrderBy(x => x.DEPT_NM_NOME).ToList<DEPARTAMENTO>(), "DEPT_CD_ID", "DEPT_NM_NOME");
            ViewBag.Servicos = new SelectList(servApp.GetAllItens(idAss).OrderBy(x => x.SERV_NM_NOME).ToList<SERVICO>(), "SERV_CD_ID", "SERV_NM_NOME");
            ViewBag.Atendimentos = new SelectList(atenApp.GetAllItens(idAss).ToList<ATENDIMENTO>(), "ATEN_CD_ID", "ATEN_NR_NUMERO");
            ViewBag.Tecnicos = new SelectList(usuApp.GetAllTecnicos(idAss).ToList<USUARIO>(), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.FormaPagamento = new SelectList(fopaApp.GetAllItens(idAss), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Filiais = new SelectList(filiApp.GetAllItens(idAss), "FILI_CD_ID", "FILI_NM_NOME");

            if (Session["ErrosAgendamento"] != null)
            {
                Hashtable erros = (Hashtable)Session["ErrosAgendamento"];

                if (erros["cadastro"] != null)
                {
                    ModelState.AddModelError("", erros["cadastro"].ToString());
                }
                else
                {
                    if (erros["categoria"] != null)
                    {
                        ModelState.AddModelError("", erros["categoria"].ToString());
                    }
                    if (erros["data"] != null)
                    {
                        ModelState.AddModelError("", erros["data"].ToString());
                    }
                    if (erros["hora"] != null)
                    {
                        ModelState.AddModelError("", erros["hora"].ToString());
                    }
                    if (erros["titulo"] != null)
                    {
                        ModelState.AddModelError("", erros["titulo"].ToString());
                    }
                }

                Session["ErrosAgendamento"] = null;
            }

            if (Session["MensOrdemServico"] != null)
            {
                if ((Int32)Session["MensOrdemServico"] == 3)
                {
                    ModelState.AddModelError("", "PRODUTO já cadastrado na ordem");
                }
                if ((Int32)Session["MensOrdemServico"] == 4)
                {
                    ModelState.AddModelError("", "SERVICO já cadastrado na ordem");
                }
                if ((Int32)Session["MensOrdemServico"] == 5)
                {
                    ModelState.AddModelError("", "Campo PRODUTO/SERVICO obrigatorio");
                }

                Session["MensOrdemServico"] = 0;
            }

            ORDEM_SERVICO item = baseApp.GetItemById(id);

            Decimal valorTotal = 0;
            valorTotal += (Decimal)item.ORDEM_SERVICO_PRODUTO.Where(x => x.OSPR_IN_ATIVO == 1).Sum(x => (x.OSPR_VL_PRECO_PROMO == null ? x.OSPR_VL_PRECO_NOVO : x.OSPR_VL_PRECO_PROMO) * x.OSPR_IN_QUANTIDADE);
            valorTotal += (Decimal)item.ORDEM_SERVICO_SERVICO.Where(x => x.OSSE_IN_ATIVO == 1).Sum(x => (x.OSSE_VL_PRECO_PROMO == null ? x.OSSE_VL_PRECO_NOVO : x.OSSE_VL_PRECO_PROMO) * x.OSSE_IN_QUANTIDADE);
            ViewBag.ValorTotal = valorTotal;

            objetoAntes = item;
            OrdemServicoViewModel vm = Mapper.Map<ORDEM_SERVICO, OrdemServicoViewModel>(item);
            Session["VoltaOrdemServico"] = 1;
            Session["IdOrdemServico"] = item.ORSE_CD_ID;
            Session["IdVolta"] = item.ORSE_CD_ID;
            Session["idOrdemServico"] = id;
            Session["idAtendimento"] = null;
            Session["vlrTotalOS"] = valorTotal;
            Session["OSProd"] = 0;
            Session["OSServ"] = 0;

            if (Session["TabP"] != null && (Int32)Session["TabP"] == 1)
            {
                ViewBag.TabProd = "active";
                ViewBag.TabServ = "";
                ViewBag.TabGeral = "";
                ViewBag.TabAcomp = "";
                ViewBag.TabAgend = "";
                ViewBag.Comments = "";
            }
            else if (Session["TabS"] != null && (Int32)Session["TabS"] == 1)
            {
                ViewBag.TabProd = "";
                ViewBag.TabServ = "active";
                ViewBag.TabGeral = "";
                ViewBag.TabAcomp = "";
                ViewBag.TabAgend = "";
                ViewBag.Comments = "";
            }
            else if (Session["TabAcom"] != null && (Int32)Session["TabAcom"] == 1)
            {
                ViewBag.TabProd = "";
                ViewBag.TabServ = "";
                ViewBag.TabGeral = "";
                ViewBag.TabAcomp = "active";
                ViewBag.TabAgend = "";
                ViewBag.Comments = "";
            }
            else if (Session["TabAgen"] != null && (Int32)Session["TabAgen"] == 1)
            {
                ViewBag.TabProd = "";
                ViewBag.TabServ = "";
                ViewBag.TabGeral = "";
                ViewBag.TabAcomp = "";
                ViewBag.TabAgend = "active";
                ViewBag.Comments = "";
            }
            else if (Session["TabComments"] != null && (Int32)Session["TabComments"] == 1)
            {
                ViewBag.TabProd = "";
                ViewBag.TabServ = "";
                ViewBag.TabGeral = "";
                ViewBag.TabAcomp = "";
                ViewBag.TabAgend = "";
                ViewBag.Comments = "active";
            }
            else
            {
                ViewBag.TabProd = "";
                ViewBag.TabServ = "";
                ViewBag.TabGeral = "active";
                ViewBag.TabAcomp = "";
                ViewBag.TabAgend = "";
                ViewBag.Comments = "";
            }

            var listaAA = osAgenApp.GetAgendaByOs(item);
            List<AGENDA> lstAgenda = new List<AGENDA>();
            foreach (var aa in listaAA)
            {
                lstAgenda.Add(agenApp.GetById(aa.AGEN_CD_ID));
            }
            ViewBag.Agendamentos = lstAgenda;

            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditarOrdemServico(OrdemServicoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara listas
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Clientes = new SelectList(clieApp.GetAllItens(idAss), "CLIE_CD_ID", "CLIE_NM_NOME");
            Session["Clientes"] = clieApp.GetAllItens(idAss);
            ViewBag.Produtos = new SelectList(proApp.GetAllItens(idAss).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Categorias = new SelectList(cosApp.GetAllItens(idAss), "CAOS_CD_ID", "CAOS_NM_NOME");
            ViewBag.Depts = new SelectList(depApp.GetAllItens(idAss).OrderBy(x => x.DEPT_NM_NOME).ToList<DEPARTAMENTO>(), "DEPT_CD_ID", "DEPT_NM_NOME");
            ViewBag.Servicos = new SelectList(servApp.GetAllItens(idAss).OrderBy(x => x.SERV_NM_NOME).ToList<SERVICO>(), "SERV_CD_ID", "SERV_NM_NOME");
            ViewBag.Atendimentos = new SelectList(atenApp.GetAllItens(idAss).ToList<ATENDIMENTO>(), "ATEN_CD_ID", "ATEN_NR_NUMERO");
            ViewBag.Tecnicos = new SelectList(usuApp.GetAllTecnicos(idAss).ToList<USUARIO>(), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.FormaPagamento = new SelectList(fopaApp.GetAllItens(idAss), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Filiais = new SelectList(filiApp.GetAllItens(idAss), "FILI_CD_ID", "FILI_NM_NOME");

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    ORDEM_SERVICO item = Mapper.Map<OrdemServicoViewModel, ORDEM_SERVICO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    Session["OS"] = null;
                    Session["OrdemServico"] = null;
                    Session["MensOrdemServico"] = 0;
                    listaMaster = new List<ORDEM_SERVICO>();
                    Session["ListaOrdemServico"] = null;
                    Session["IdOrdemServico"] = item.ORSE_CD_ID;
                    if ((Int32)Session["IdAtendimento"] != 0)
                    {
                        return RedirectToAction("EditarAtendimento", "Atendimento", new { id = (Int32)Session["IdAtendimento"] });
                    }

                    if (Session["FiltroOrdemServico"] != null)
                    {
                        FiltrarOrdemServico((ORDEM_SERVICO)Session["FiltroOrdemServico"]);
                    }
                    return RedirectToAction("MontarTelaOrdemServico");
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
        public ActionResult VerOrdemServico(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara listas
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.FormaPagamento = new SelectList(fopaApp.GetAllItens(idAss), "FOPA_CD_ID", "FOPA_NM_NOME");
            ORDEM_SERVICO item = baseApp.GetItemById(id);

            Decimal valorTotal = 0;
            valorTotal += (Decimal)item.ORDEM_SERVICO_PRODUTO.Where(x => x.OSPR_IN_ATIVO == 1).Sum(x => x.OSPR_VL_PRECO_NOVO * x.OSPR_IN_QUANTIDADE);
            valorTotal += (Decimal)item.ORDEM_SERVICO_SERVICO.Where(x => x.OSSE_IN_ATIVO == 1).Sum(x => x.OSSE_VL_PRECO_NOVO * x.OSSE_IN_QUANTIDADE);
            ViewBag.ValorTotal = valorTotal;

            objetoAntes = item;
            OrdemServicoViewModel vm = Mapper.Map<ORDEM_SERVICO, OrdemServicoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult VerAnexoOrdemServico(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            ORDEM_SERVICO_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult IncluirAcompanhamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            ORDEM_SERVICO item = baseApp.GetItemById((Int32)Session["IdVolta"]);
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            ORDEM_SERVICO_ACOMPANHAMENTO coment = new ORDEM_SERVICO_ACOMPANHAMENTO();
            OrdemServicoAcompanhamentoViewModel vm = Mapper.Map<ORDEM_SERVICO_ACOMPANHAMENTO, OrdemServicoAcompanhamentoViewModel>(coment);
            vm.ORSA_DT_ACOMPANHAMENTO = DateTime.Now;
            vm.ORSA_IN_ATIVO = 1;
            vm.ORSE_CD_ID = item.ORSE_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirAcompanhamento(OrdemServicoAcompanhamentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            Session["VoltaAcompanhamento"] = true;
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    ORDEM_SERVICO_ACOMPANHAMENTO item = Mapper.Map<OrdemServicoAcompanhamentoViewModel, ORDEM_SERVICO_ACOMPANHAMENTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    ORDEM_SERVICO not = baseApp.GetItemById((Int32)Session["IdVolta"]);

                    if (item.USUARIO != null)
                    {
                        item.USUARIO = null;
                    }

                    objetoAntes = not;
                    Int32 volta = baseApp.ValidateCreateAcompanhamento(item);

                    // Verifica retorno
                    Session["TabAcom"] = 1;

                    // Sucesso
                    return RedirectToAction("EditarOrdemServico", new { id = (Int32)Session["IdVolta"] });
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

        public ActionResult IncluirComentario()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            ORDEM_SERVICO item = baseApp.GetItemById((Int32)Session["IdVolta"]);
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            ORDEM_SERVICO_COMENTARIOS coment = new ORDEM_SERVICO_COMENTARIOS();
            OrdemServicoComentarioViewModel vm = Mapper.Map<ORDEM_SERVICO_COMENTARIOS, OrdemServicoComentarioViewModel>(coment);
            vm.ORSC_DT_CRIACAO = DateTime.Now;
            vm.ORSE_CD_ID = item.ORSE_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirComentario(OrdemServicoComentarioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["VoltaComentario"] = true;
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    ORDEM_SERVICO_COMENTARIOS item = Mapper.Map<OrdemServicoComentarioViewModel, ORDEM_SERVICO_COMENTARIOS>(vm);
                    Int32 volta = baseApp.ValidateCreateComentario(item);

                    Session["TabComments"] = 1;

                    // Sucesso
                    return RedirectToAction("EditarOrdemServico", new { id = (Int32)Session["IdVolta"] });
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

        [HttpPost]
        public ActionResult IncluirAgendamento(AGENDA agenda, Int32 ORSE_CD_ID)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            Hashtable error = new Hashtable();
            ORDEM_SERVICO_AGENDA osa = new ORDEM_SERVICO_AGENDA();
            osa.ORSE_CD_ID = ORSE_CD_ID;
            osa.OSAG_IN_ATIVO = 1;

            USUARIO user = usuario;
            agenda.ASSI_CD_ID = idAss;
            agenda.USUA_CD_ID = user.USUA_CD_ID;
            agenda.AGEN_IN_ATIVO = 1;
            agenda.AGEN_IN_STATUS = 1;
            Int32 id = ORSE_CD_ID;

            if (agenda.CAAG_CD_ID == null)
            {
                error.Add("categoria", "Campo CATEGORIA obrigatorio");
            }

            if (agenda.AGEN_DT_DATA == DateTime.MinValue)
            {
                agenda.AGEN_DT_DATA = DateTime.Now.Date;
                //error.Add("data", "Campo DATA obrigatorio");
                if (agenda.AGEN_HR_HORA == TimeSpan.Zero)
                {
                    error.Add("hora", "Campo HORA obrigatorio");
                }
            }

            if (agenda.AGEN_NM_TITULO == null)
            {
                error.Add("titulo", "Campo TÍTULO obrigatorio");
            }

            Int32? volta = null;
            if (error.Count == 0)
            {
                volta = agenApp.ValidateCreate(agenda, user);

                // Cria pastas
                String caminho = "/Imagens/" + idAss.ToString() + "/Agenda/" + agenda.AGEN_CD_ID.ToString() + "/Anexos/";
                Directory.CreateDirectory(Server.MapPath(caminho));
                Session["ListaAgenda"] = null;
            }
            else
            {
                Session["ErrosAgendamento"] = error;
                return RedirectToAction("EditarOrdemServico", "OrdemServico", new { id = id });
            }

            if (volta != 0)
            {
                error.Add("cadastro", "Agendamento não incluído, verifique e tente novamente");
                Session["ErrosAgendamento"] = error;
            }

            osa.AGEN_CD_ID = agenda.AGEN_CD_ID;
            Int32 voltaAA = osAgenApp.ValidateCreate(osa);

            if (voltaAA != 0)
            {
                error.Add("cadastro", "Agendamento não incluído, verifique e tente novamente");
                Session["ErrosAgendamento"] = error;
            }

            Session["agenLista"] = 1;
            return RedirectToAction("EditarOrdemServico", "OrdemServico", new { id = id });
        }

        [HttpGet]
        public ActionResult ExcluirOSProduto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["OSProd"] = 1;
            ORDEM_SERVICO_PRODUTO item = ospApp.GetById(id);
            return View(item);
        }

        [HttpPost]
        public ActionResult ExcluirOSProduto(ORDEM_SERVICO_PRODUTO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                item = ospApp.GetById(item.OSPR_CD_ID);
                ORDEM_SERVICO_PRODUTO itemEdit = new ORDEM_SERVICO_PRODUTO
                {
                    OSPR_CD_ID = item.OSPR_CD_ID,
                    ORSE_CD_ID = item.ORSE_CD_ID,
                    PROD_CD_ID = item.PROD_CD_ID,
                    OSPR_IN_QUANTIDADE = item.OSPR_IN_QUANTIDADE,
                    OSPR_VL_PRECO_NOVO = item.OSPR_VL_PRECO_NOVO,
                    OSPR_IN_ATIVO = item.OSPR_IN_ATIVO,
                    OSPR_DS_OBSERVACAO = item.OSPR_DS_OBSERVACAO
                };

                Int32 volta = ospApp.ValidateDelete(itemEdit, item);

                return RedirectToAction("EditarOrdemServico", new { id = (Int32)Session["IdVolta"] });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(item);
            }
        }

        [HttpGet]
        public ActionResult ReativarOSProduto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["OSProd"] = 1;
            ORDEM_SERVICO_PRODUTO item = ospApp.GetById(id);
            return View(item);
        }

        [HttpPost]
        public ActionResult ReativarOSProduto(ORDEM_SERVICO_PRODUTO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                item = ospApp.GetById(item.OSPR_CD_ID);
                ORDEM_SERVICO_PRODUTO itemEdit = new ORDEM_SERVICO_PRODUTO
                {
                    OSPR_CD_ID = item.OSPR_CD_ID,
                    ORSE_CD_ID = item.ORSE_CD_ID,
                    PROD_CD_ID = item.PROD_CD_ID,
                    OSPR_IN_QUANTIDADE = item.OSPR_IN_QUANTIDADE,
                    OSPR_VL_PRECO_NOVO = item.OSPR_VL_PRECO_NOVO,
                    OSPR_IN_ATIVO = item.OSPR_IN_ATIVO,
                    OSPR_DS_OBSERVACAO = item.OSPR_DS_OBSERVACAO

                };

                Int32 volta = ospApp.ValidateReativar(itemEdit, item);

                return RedirectToAction("EditarOrdemServico", new { id = (Int32)Session["IdVolta"] });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(item);
            }
        }

        [HttpGet]
        public ActionResult ExcluirOSServico(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["OSServ"] = 1;
            ORDEM_SERVICO_SERVICO item = ossApp.GetById(id);
            return View(item);
        }

        [HttpPost]
        public ActionResult ExcluirOSServico(ORDEM_SERVICO_SERVICO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                item = ossApp.GetById(item.OSSE_CD_ID);
                ORDEM_SERVICO_SERVICO itemEdit = new ORDEM_SERVICO_SERVICO
                {
                    OSSE_CD_ID = item.OSSE_CD_ID,
                    ORSE_CD_ID = item.ORSE_CD_ID,
                    SERV_CD_ID = item.SERV_CD_ID,
                    OSSE_IN_QUANTIDADE = item.OSSE_IN_QUANTIDADE,
                    OSSE_VL_PRECO_NOVO = item.OSSE_VL_PRECO_NOVO,
                    OSSE_DS_OBSERVACOES = item.OSSE_DS_OBSERVACOES,
                    OSSE_IN_ATIVO = item.OSSE_IN_ATIVO

                };

                Int32 volta = ossApp.ValidateDelete(itemEdit, item);

                return RedirectToAction("EditarOrdemServico", new { id = (Int32)Session["IdVolta"] });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(item);
            }
        }

        [HttpGet]
        public ActionResult ReativarOSServico(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["OSServ"] = 1;
            ORDEM_SERVICO_SERVICO item = ossApp.GetById(id);
            return View(item);
        }

        [HttpPost]
        public ActionResult ReativarOSServico(ORDEM_SERVICO_SERVICO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                item = ossApp.GetById(item.OSSE_CD_ID);
                ORDEM_SERVICO_SERVICO itemEdit = new ORDEM_SERVICO_SERVICO
                {
                    OSSE_CD_ID = item.OSSE_CD_ID,
                    ORSE_CD_ID = item.ORSE_CD_ID,
                    SERV_CD_ID = item.SERV_CD_ID,
                    OSSE_IN_QUANTIDADE = item.OSSE_IN_QUANTIDADE,
                    OSSE_VL_PRECO_NOVO = item.OSSE_VL_PRECO_NOVO,
                    OSSE_DS_OBSERVACOES = item.OSSE_DS_OBSERVACOES,
                    OSSE_IN_ATIVO = item.OSSE_IN_ATIVO
                };

                Int32 volta = ossApp.ValidateReativar(itemEdit, item);

                return RedirectToAction("EditarOrdemServico", new { id = (Int32)Session["IdVolta"] });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(item);
            }
        }

        [HttpPost]
        public JsonResult IncluirProdutoOS_Inline(Int32? PROD_CD_ID, Int32 qtde, Decimal preco, String item_obs, Decimal promo)
        {
            if (PROD_CD_ID != null)
            {
                ORDEM_SERVICO_PRODUTO item = new ORDEM_SERVICO_PRODUTO
                {
                    ORSE_CD_ID = (Int32)Session["IdVolta"],
                    PROD_CD_ID = PROD_CD_ID.Value,
                    OSPR_IN_QUANTIDADE = qtde,
                    OSPR_VL_PRECO_NOVO = preco,
                    OSPR_IN_ATIVO = 1,
                    OSPR_DS_OBSERVACAO = item_obs,
                    OSPR_VL_PRECO_PROMO = promo
                };

                Int32 volta = ospApp.ValidateCreate(item);
                if (volta == 1)
                {
                    Session["MensOrdemServico"] = 3;
                }
                Session["TabP"] = 1;

                var os = baseApp.GetItemById((Int32)Session["IdVolta"]).ORDEM_SERVICO_PRODUTO;
                Decimal vlrTotal = os.Sum(x => (Int32)x.OSPR_IN_QUANTIDADE * (Decimal)(x.OSPR_VL_PRECO_PROMO == null ? x.OSPR_VL_PRECO_NOVO : x.OSPR_VL_PRECO_PROMO));

                var result = new Hashtable();
                result.Add("id", item.OSPR_CD_ID);
                result.Add("vlrTotal", vlrTotal);
                return Json(result);
            }
            else
            {
                Session["MensOrdemServico"] = 5;
                return Json(3);
            }
        }

        [HttpPost]
        public JsonResult IncluirServicoOS_Inline(Int32? SERV_CD_ID, Int32 qtde, Decimal preco, String item_obs, Decimal promo)
        {
            if (SERV_CD_ID != null)
            {
                ORDEM_SERVICO_SERVICO item = new ORDEM_SERVICO_SERVICO
                {
                    ORSE_CD_ID = (Int32)Session["IdVolta"],
                    SERV_CD_ID = SERV_CD_ID.Value,
                    OSSE_IN_QUANTIDADE = qtde,
                    OSSE_VL_PRECO_NOVO = preco,
                    OSSE_DS_OBSERVACOES = item_obs,
                    OSSE_IN_ATIVO = 1
                };

                Int32 volta = ossApp.ValidateCreate(item);
                if (volta == 1)
                {
                    Session["MensOrdemServico"] = 4;
                }
                Session["TabS"] = 1;

                var os = baseApp.GetItemById((Int32)Session["IdVolta"]).ORDEM_SERVICO_SERVICO;
                Decimal vlrTotal = os.Sum(x => (Int32)x.OSSE_IN_QUANTIDADE * (Decimal)(x.OSSE_VL_PRECO_PROMO == null ? x.OSSE_VL_PRECO_NOVO : x.OSSE_VL_PRECO_PROMO));

                var result = new Hashtable();
                result.Add("id", item.OSSE_CD_ID);
                result.Add("vlrTotal", vlrTotal);
                return Json(result);
            }
            else
            {
                Session["MensOrdemServico"] = 5;
                return Json(3);
            }
        }

        [HttpGet]
        public ActionResult EnviarAprovacaoOrdemServico(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            ORDEM_SERVICO item = baseApp.GetItemById(id);
            objetoAntes = item;
            return View(item);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EnviarAprovacaoOrdemServico(ORDEM_SERVICO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    item.ORSE_IN_STATUS = 5;
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    Session["OS"] = null;
                    Session["OrdemServico"] = null;
                    listaMaster = new List<ORDEM_SERVICO>();
                    Session["ListaOrdemServico"] = null;
                    return RedirectToAction("MontarTelaOrdemServico");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(item);
                }
            }
            else
            {
                return View(item);
            }
        }

        [HttpGet]
        public ActionResult AprovarOrdemServico(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["OSServ"] = 1;
            ORDEM_SERVICO item = baseApp.GetItemById(id);
            objetoAntes = item;
            return View(item);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AprovarOrdemServico(ORDEM_SERVICO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    item.ORSE_IN_STATUS = 6;
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        foreach (var prod in item.ORDEM_SERVICO_PRODUTO)
                        {
                            try
                            {
                                PRODUTO_ESTOQUE_FILIAL pef = pefApp.GetByProdFilial(item.PROD_CD_ID.Value, item.FILI_CD_ID == null ? (Int32)Session["IdFilial"] : item.FILI_CD_ID.Value);
                                PRODUTO_ESTOQUE_FILIAL pefEdit = new PRODUTO_ESTOQUE_FILIAL
                                {
                                    PREF_CD_ID = pef.PREF_CD_ID,
                                    PROD_CD_ID = pef.PROD_CD_ID,
                                    FILI_CD_ID = pef.FILI_CD_ID,
                                    PREF_QN_ESTOQUE = pef.PREF_QN_ESTOQUE - prod.OSPR_IN_QUANTIDADE,
                                    PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now,
                                    PREF_IN_ATIVO = pef.PREF_IN_ATIVO,
                                    PREF_QN_QUANTIDADE_ALTERADA = prod.OSPR_IN_QUANTIDADE,
                                    PREF_DS_JUSTIFICATIVA = prod.OSPR_DS_OBSERVACAO,
                                    PREF_NR_MARKUP = pef.PREF_NR_MARKUP
                                };

                                Int32 voltaEstoque = pefApp.ValidateEdit(pefEdit, pef, usuarioLogado);
                            }
                            catch (Exception ex)
                            {

                            }
                        } 
                    }

                    // Sucesso
                    Session["OS"] = null;
                    Session["OrdemServico"] = null;
                    listaMaster = new List<ORDEM_SERVICO>();
                    Session["ListaOrdemServico"] = null;
                    return RedirectToAction("MontarTelaOrdemServico");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(item);
                }
            }
            else
            {
                return View(item);
            }
        }

        [HttpGet]
        public ActionResult RecusarOrdemServico(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            ORDEM_SERVICO item = baseApp.GetItemById(id);
            objetoAntes = item;
            return View(item);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult RecusarOrdemServico(ORDEM_SERVICO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    item.ORSE_IN_STATUS = 7;
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    Session["OS"] = null;
                    Session["OrdemServico"] = null;
                    listaMaster = new List<ORDEM_SERVICO>();
                    Session["ListaOrdemServico"] = null;
                    return RedirectToAction("MontarTelaOrdemServico");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(item);
                }
            }
            else
            {
                return View(item);
            }
        }
    }
}