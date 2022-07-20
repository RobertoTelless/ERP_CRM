using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using SMS_Presentation.App_Start;
using EntitiesServices.WorkClasses;
using AutoMapper;
using ERP_CRM_Solution.ViewModels;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Web.UI.WebControls;
using System.Runtime.Caching;
using Image = iTextSharp.text.Image;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Canducci.Zip;
using CrossCutting;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ERP_CRM_Solution.Controllers
{
    public class ServicoController : Controller
    {
        private readonly IServicoAppService servApp;
        private readonly ILogAppService logApp;
        private readonly IUnidadeAppService unApp;
        private readonly ICategoriaServicoAppService csApp;
        private readonly IFilialAppService filApp;
        //private readonly IPedidoVendaAppService pedvApp;
        private readonly IServicoTabelaPrecoAppService stbApp;
        private readonly IAtendimentoAppService ateApp;
        private readonly ICategoriaAtendimentoAppService caApp;
        private readonly IClienteAppService cliApp;
        private readonly IProdutoAppService proApp;
        private readonly IUsuarioAppService usuApp;

        private String msg;
        private Exception exception;
        SERVICO objetoServ = new SERVICO();
        SERVICO objetoServAntes = new SERVICO();
        List<SERVICO> listaMasterServ = new List<SERVICO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public ServicoController(IServicoAppService servApps, ILogAppService logApps, IUnidadeAppService unApps, ICategoriaServicoAppService csApps, IFilialAppService filApps, IServicoTabelaPrecoAppService stbApps, IAtendimentoAppService ateApps, ICategoriaAtendimentoAppService caApps, IClienteAppService cliApps, IProdutoAppService proApps, IUsuarioAppService usuApps)
        {
            servApp = servApps;
            logApp = logApps;
            unApp = unApps;
            csApp = csApps;
            filApp = filApps;
            //pedvApp = pedvApps;
            stbApp = stbApps;
            ateApp = ateApps;
            caApp = caApps;
            cliApp = cliApps; 
            proApp = proApps;
            usuApp = usuApps;
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
            if ((Int32)Session["VoltaServico"] == 40)
            {
                Session["VoltaServico"] = 1;
                return RedirectToAction("VoltarAcompanhamentoCRM", "CRM");
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

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardCRMNovo", "CRM");
        }

        [HttpPost]
        public JsonResult BuscaNome(String nome)
        {
            List<Hashtable> listResult = new List<Hashtable>();
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SERVICO> lista = servApp.GetAllItens(idAss);
            Session["Servicos"] = lista;
            if (nome != null)
            {
                List<SERVICO> lstProduto = lista.Where(x => x.SERV_NM_NOME != null && x.SERV_NM_NOME.ToLower().Contains(nome.ToLower())).ToList<SERVICO>();

                if (lstProduto != null)
                {
                    foreach (var item in lstProduto)
                    {
                        Hashtable result = new Hashtable();
                        result.Add("id", item.SERV_CD_ID);
                        result.Add("text", item.SERV_NM_NOME);
                        listResult.Add(result);
                    }
                }
            }
            return Json(listResult);
        }

        [HttpGet]
        public ActionResult MontarTelaServico()
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
            if (Session["ListaServico"] == null)
            {
                listaMasterServ = servApp.GetAllItens(idAss);
                Session["ListaServico"] = listaMasterServ;
            }
            ViewBag.Listas = (List<SERVICO>)Session["ListaServico"];
            ViewBag.Title = "Serviços";
            ViewBag.Tipos = new SelectList(servApp.GetAllTipos(idAss).OrderBy(x => x.CASE_NM_NOME).ToList<CATEGORIA_SERVICO>(), "CASE_CD_ID", "CASE_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss).OrderBy(p => p.FILI_NM_NOME), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(unApp.GetAllItens(idAss).Where(p => p.UNID_IN_TIPO_UNIDADE == 2).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Nomes = new SelectList(servApp.GetAllNBSE().OrderBy(p => p.NBSE_NM_NOME), "NBSE_CD_ID", "NBSE_NM_NOME");

            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Servicos = ((List<SERVICO>)Session["ListaServico"]).Count;

            if (Session["MensServico"] != null)
            {
                if ((Int32)Session["MensServico"] == 3)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0121", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensServico"] == 2)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensServico"] == 4)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0122", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensServico"] == 50)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0124", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensServico"] = 0;
            Session["VoltaServico"] = 1;
            objetoServ = new SERVICO();
            return View(objetoServ);
        }

        public ActionResult RetirarFiltroServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaServico"] = null;
            Session["FiltroServico"] = null;
            return RedirectToAction("MontarTelaServico");
        }

        public ActionResult MostrarTudoServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterServ = servApp.GetAllItensAdm(idAss);
            Session["FiltroServico"] = null;
            Session["ListaServico"] = listaMasterServ;
            return RedirectToAction("MontarTelaServico");
        }

        [HttpPost]
        public ActionResult FiltrarServico(SERVICO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                List<SERVICO> listaObj = new List<SERVICO>();
                Session["FiltroServico"] = item;
                Int32 volta = servApp.ExecuteFilter(item.CASE_CD_ID, item.SERV_NM_NOME, item.SERV_DS_DESCRICAO, item.SERV_TX_OBSERVACOES, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    return RedirectToAction("MontarTelaServico");
                }

                // Sucesso
                listaMasterServ = listaObj;
                Session["ListaServico"] = listaObj;
                return RedirectToAction("MontarTelaServico");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaProduto");
            }
        }

        public ActionResult VoltarBaseServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaServico");
        }

        public ActionResult IncluirCategoriaServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("IncluirCategoriaServico", "TabelasAuxiliares");
        }

        [HttpGet]
        public ActionResult IncluirServico()
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

            // Verifica possibilidade
            Int32 num = servApp.GetAllItens(idAss).Count;
            if ((Int32)Session["NumServicos"] <= num)
            {
                Session["MensServico"] = 50;
                return RedirectToAction("MontarTelaServico");
            }

            // Prepara listas
            ViewBag.Tipos = new SelectList(servApp.GetAllTipos(idAss).OrderBy(x => x.CASE_NM_NOME).ToList<CATEGORIA_SERVICO>(), "CASE_CD_ID", "CASE_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss).OrderBy(p => p.FILI_NM_NOME), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(unApp.GetAllItens(idAss).Where(p => p.UNID_IN_TIPO_UNIDADE == 2).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Nomes = new SelectList(servApp.GetAllNBSE().OrderBy(p => p.NBSE_NM_NOME), "NBSE_CD_ID", "NBSE_NM_NOME");
            List<SelectListItem> local = new List<SelectListItem>();
            local.Add(new SelectListItem() { Text = "Interno", Value = "1" });
            local.Add(new SelectListItem() { Text = "Externo", Value = "2" });
            local.Add(new SelectListItem() { Text = "Interno/externo", Value = "3" });
            ViewBag.Local = new SelectList(local, "Value", "Text");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            Session["VoltaPop"] = 2;
            SERVICO item = new SERVICO();
            ServicoViewModel vm = Mapper.Map<SERVICO, ServicoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.SERV_DT_CADASTRO = DateTime.Today.Date;
            vm.SERV_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public void MontarListaCusto(SERVICO_TABELA_PRECO item)
        {
            //Task.Run(() => {
                List<SERVICO_TABELA_PRECO> listaPr = new List<SERVICO_TABELA_PRECO>();
                if (Session["ListaPrecoServico"] == null)
                {                    
                    listaPr = new List<SERVICO_TABELA_PRECO>();
                }
                else
                {
                    listaPr = (List<SERVICO_TABELA_PRECO>)Session["ListaPrecoServico"];
                }
                item.SETP_IN_ATIVO = 1;
                item.SETP_DT_DATA_REAJUSTE = DateTime.Now;
                listaPr.Add(item);
                Session["ListaPrecoServico"] = listaPr;
            //});
        }

        [HttpPost]
        public void RemovePrecoTabela(SERVICO_TABELA_PRECO item)
        {
            Task.Run(() => {
                if (Session["ListaPrecoServico"] != null)
                {
                    List<SERVICO_TABELA_PRECO> lista = (List<SERVICO_TABELA_PRECO>)Session["ListaPrecoServico"];
                    lista.RemoveAll(x => x.FILI_CD_ID == item.FILI_CD_ID);
                    Session["ListaPrecoServico"] = lista;
                }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirServico(ServicoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(servApp.GetAllTipos(idAss).OrderBy(x => x.CASE_NM_NOME).ToList<CATEGORIA_SERVICO>(), "CASE_CD_ID", "CASE_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss).OrderBy(p => p.FILI_NM_NOME), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(unApp.GetAllItens(idAss).Where(p => p.UNID_IN_TIPO_UNIDADE == 2).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Nomes = new SelectList(servApp.GetAllNBSE().OrderBy(p => p.NBSE_NM_NOME), "NBSE_CD_ID", "NBSE_NM_NOME");
            List<SelectListItem> local = new List<SelectListItem>();
            local.Add(new SelectListItem() { Text = "Interno", Value = "1" });
            local.Add(new SelectListItem() { Text = "Externo", Value = "2" });
            local.Add(new SelectListItem() { Text = "Interno/externo", Value = "3" });
            ViewBag.Local = new SelectList(local, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    SERVICO item = Mapper.Map<ServicoViewModel, SERVICO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = servApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensServico"] = 3;
                        return RedirectToAction("MontarTelaServico");
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Servicos/" + item.SERV_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    Session["IdVolta"] = item.SERV_CD_ID;
                    if (Session["FileQueueServico"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueServico"];

                        foreach (var file in fq)
                        {
                            UploadFileQueueServico(file);
                        }

                        Session["FileQueueServico"] = null;
                    }

                    if (Session["ListaPrecoServico"] != null)
                    {
                        IncluirTabelaServico(item.SERV_CD_ID);
                    }

                    // Sucesso
                    listaMasterServ = new List<SERVICO>();
                    Session["ListaServico"] = null;
                    return RedirectToAction("MontarTelaServico");
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

        public void IncluirTabelaServico(Int32 id)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SERVICO_TABELA_PRECO> lista = (List<SERVICO_TABELA_PRECO>)Session["ListaPrecoServico"];
            lista.Select(c => { c.SERV_CD_ID = id; return c; }).ToList<SERVICO_TABELA_PRECO>();

            Int32 volta = stbApp.ValidateCreateLista(lista, idAss);
            Session["ListaPrecoServico"] = null;
        }

        public ActionResult IncluirItemTabelaServico(SERVICO_TABELA_PRECO item)
        {
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];

                item.SETP_IN_ATIVO = 1;
                item.SETP_DT_DATA_REAJUSTE = DateTime.Now;

                Int32 volta = stbApp.ValidateCreate(item, idAss);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensPreco"] = 1;
                }

                return RedirectToAction("EditarServico", new { id = item.SERV_CD_ID });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("EditarServico", new { id = item.SERV_CD_ID });
            }
        }

        [HttpPost]
        public ActionResult EditarPC(SERVICO_TABELA_PRECO item, Int32 id)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];;

                item.SERV_CD_ID = id;
                item.SETP_DT_DATA_REAJUSTE = DateTime.Today.Date;
                item.SETP_IN_ATIVO = 1;

                Int32 volta = stbApp.ValidateEdit(item, id);

                return RedirectToAction("EditarServico", new { id = item.SERV_CD_ID });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction("EditarProduto", new { id = item.SERV_CD_ID });
            }
        }

        [HttpGet]
        public ActionResult ExcluirTabelaServico(Int32 id)
        {

            // Verifica se tem usuario logado
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];

            SERVICO_TABELA_PRECO itemAntes = stbApp.GetById(id);
            SERVICO_TABELA_PRECO item = new SERVICO_TABELA_PRECO();
            item.FILI_CD_ID = itemAntes.FILI_CD_ID;
            item.SERV_CD_ID = itemAntes.SERV_CD_ID;
            item.SETP_CD_ID = itemAntes.SETP_CD_ID;
            item.SETP_DT_DATA_REAJUSTE = itemAntes.SETP_DT_DATA_REAJUSTE;
            item.SETP_IN_ATIVO = 0;
            item.SETP_NR_MARKUP = itemAntes.SETP_NR_MARKUP;
            item.SETP_VL_CUSTO = itemAntes.SETP_VL_CUSTO;
            item.SETP_VL_PRECO = itemAntes.SETP_VL_PRECO;
            item.SETP_VL_PRECO_PROMOCAO = itemAntes.SETP_VL_PRECO_PROMOCAO;
            item.SETP_VL_DESCONTO_MAXIMO = itemAntes.SETP_VL_DESCONTO_MAXIMO;
            Int32 volta = stbApp.ValidateDelete(item, id);
            return RedirectToAction("VoltarAnexoServico");
        }

        [HttpGet]
        public ActionResult ReativarTabelaServico(Int32 id)
        {

            // Verifica se tem usuario logado
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];

            SERVICO_TABELA_PRECO itemAntes = stbApp.GetById(id);
            SERVICO_TABELA_PRECO item = new SERVICO_TABELA_PRECO();
            item.FILI_CD_ID = itemAntes.FILI_CD_ID;
            item.SERV_CD_ID = itemAntes.SERV_CD_ID;
            item.SETP_CD_ID = itemAntes.SETP_CD_ID;
            item.SETP_DT_DATA_REAJUSTE = itemAntes.SETP_DT_DATA_REAJUSTE;
            item.SETP_IN_ATIVO = 1;
            item.SETP_NR_MARKUP = itemAntes.SETP_NR_MARKUP;
            item.SETP_VL_CUSTO = itemAntes.SETP_VL_CUSTO;
            item.SETP_VL_PRECO = itemAntes.SETP_VL_PRECO;
            item.SETP_VL_PRECO_PROMOCAO = itemAntes.SETP_VL_PRECO_PROMOCAO;
            item.SETP_VL_DESCONTO_MAXIMO = itemAntes.SETP_VL_DESCONTO_MAXIMO;
            Int32 volta = stbApp.ValidateReativar(item, id);
            return RedirectToAction("VoltarAnexoServico");
        }

        [HttpGet]
        public ActionResult VerServico(Int32 id)
        {
            
            // Prepara view
            SERVICO item = servApp.GetItemById(id);
            //ViewBag.LstPedidos = pedvApp.GetAllItens();
            objetoServAntes = item;
            Session["Servico"] = item;
            Session["IdVolta"] = id;
            Session["VoltaPop"] = 3;
            ServicoViewModel vm = Mapper.Map<SERVICO, ServicoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarServico(Int32 id)
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

            // Prepara view
            SERVICO item = servApp.GetItemById(id);
            ViewBag.Tipos = new SelectList(servApp.GetAllTipos(idAss).OrderBy(x => x.CASE_NM_NOME).ToList<CATEGORIA_SERVICO>(), "CASE_CD_ID", "CASE_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss).OrderBy(p => p.FILI_NM_NOME), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(unApp.GetAllItens(idAss).Where(p => p.UNID_IN_TIPO_UNIDADE == 2).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Nomes = new SelectList(servApp.GetAllNBSE().OrderBy(p => p.NBSE_NM_NOME), "NBSE_CD_ID", "NBSE_NM_NOME");
            List<SelectListItem> local = new List<SelectListItem>();
            local.Add(new SelectListItem() { Text = "Interno", Value = "1" });
            local.Add(new SelectListItem() { Text = "Externo", Value = "2" });
            local.Add(new SelectListItem() { Text = "Interno/externo", Value = "3" });
            ViewBag.Local = new SelectList(local, "Value", "Text");
            //ViewBag.LstPedidos = pedvApp.GetAllItens();
            objetoServAntes = item;

            if (Session["MensPreco"] != null)
            {
                if ((Int32)Session["MensPreco"] == 1)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0108", CultureInfo.CurrentCulture));
                    Session["MensPreco"] = 0;
                }
                if ((Int32)Session["MensServico"] == 30)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensServico"] == 31)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
            }

            Session["Servico"] = item;
            Session["IdVolta"] = id;
            Session["VoltaPop"] = 3;
            ServicoViewModel vm = Mapper.Map<SERVICO, ServicoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarServico(ServicoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            ViewBag.Tipos = new SelectList(servApp.GetAllTipos(idAss).OrderBy(x => x.CASE_NM_NOME).ToList<CATEGORIA_SERVICO>(), "CASE_CD_ID", "CASE_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss).OrderBy(p => p.FILI_NM_NOME), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(unApp.GetAllItens(idAss).Where(p => p.UNID_IN_TIPO_UNIDADE == 2).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Nomes = new SelectList(servApp.GetAllNBSE().OrderBy(p => p.NBSE_NM_NOME), "NBSE_CD_ID", "NBSE_NM_NOME");
            List<SelectListItem> local = new List<SelectListItem>();
            local.Add(new SelectListItem() { Text = "Interno", Value = "1" });
            local.Add(new SelectListItem() { Text = "Externo", Value = "2" });
            local.Add(new SelectListItem() { Text = "Interno/externo", Value = "3" });
            ViewBag.Local = new SelectList(local, "Value", "Text");
            //ViewBag.LstPedidos = pedvApp.GetAllItens();
            if (ModelState.IsValid)
            {
                try
            {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    SERVICO item = Mapper.Map<ServicoViewModel, SERVICO>(vm);
                    Int32 volta = servApp.ValidateEdit(item, objetoServAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterServ = new List<SERVICO>();
                    Session["ListaServico"] = null;
                    return RedirectToAction("MontarTelaServico");
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
        public ActionResult ExcluirServico(Int32 id)
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

            // Executar
            SERVICO item = servApp.GetItemById(id);
            objetoServAntes = item;
            item.SERV_IN_ATIVO = 0;
            Int32 volta = servApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensServico"] = 4;
                return RedirectToAction("MontarTelaServico");
            }
            listaMasterServ = new List<SERVICO>();
            Session["ListaServico"] = null;
            return RedirectToAction("MontarTelaServico");
        }

        [HttpGet]
        public ActionResult ReativarServico(Int32 id)
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

            // Verifica possibilidade
            Int32 num = servApp.GetAllItens(idAss).Count;
            if ((Int32)Session["NumServicos"] <= num)
            {
                Session["MensServico"] = 50;
                return RedirectToAction("MontarTelaServico");
            }

            // Executar
            SERVICO item = servApp.GetItemById(id);
            item.SERV_IN_ATIVO = 1;
            objetoServAntes = item;
            Int32 volta = servApp.ValidateReativar(item, usuario);
            listaMasterServ = new List<SERVICO>();
            Session["ListaServico"] = null;
            return RedirectToAction("MontarTelaServico");
        }

        [HttpGet]
        public ActionResult VerAnexoServico(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            SERVICO_ANEXO item = servApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EditarServico", new { id = (Int32)Session["IdVolta"] });
        }

        public FileResult DownloadServico(Int32 id)
        {
            SERVICO_ANEXO item = servApp.GetAnexoById(id);
            String arquivo = item.SEAN_AQ_ARQUIVO;
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

            Session["FileQueueServico"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueServico(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (file == null)
            {
                Session["MensServico"] = 30;
                return RedirectToAction("VoltarAnexoServico");
            }

            SERVICO item = servApp.GetById((Int32)Session["IdVolta"]);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 100)
            {
                Session["MensServico"] = 31;
                return RedirectToAction("VoltarAnexoServico");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Servicos/" + item.SERV_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            SERVICO_ANEXO foto = new SERVICO_ANEXO();
            foto.SEAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.SEAN_DT_ANEXO = DateTime.Today;
            foto.SEAN_IN_ATIVO = 1;
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
            foto.SEAN_IN_TIPO = tipo;
            foto.SEAN_NM_TITULO = fileName;
            foto.SERV_CD_ID = item.SERV_CD_ID;

            item.SERVICO_ANEXO.Add(foto);
            objetoServAntes = item;
            Int32 volta = servApp.ValidateEdit(item, objetoServAntes);
            return RedirectToAction("VoltarAnexoServico");
        }

        [HttpPost]
        public ActionResult UploadFileServico(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (file == null)
            {
                Session["MensServico"] = 30;
                return RedirectToAction("VoltarAnexoServico");
            }

            SERVICO item = servApp.GetById((Int32)Session["IdVolta"]);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 100)
            {
                Session["MensServico"] = 31;
                return RedirectToAction("VoltarAnexoServico");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Servicos/" + item.SERV_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            SERVICO_ANEXO foto = new SERVICO_ANEXO();
            foto.SEAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.SEAN_DT_ANEXO = DateTime.Today;
            foto.SEAN_IN_ATIVO = 1;
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
            foto.SEAN_IN_TIPO = tipo;
            foto.SEAN_NM_TITULO = fileName;
            foto.SERV_CD_ID = item.SERV_CD_ID;

            item.SERVICO_ANEXO.Add(foto);
            objetoServAntes = item;
            Int32 volta = servApp.ValidateEdit(item, objetoServAntes);
            return RedirectToAction("VoltarAnexoServico");
        }

        public ActionResult GerarRelatorioLista()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "ServicoLista" + "_" + data + ".pdf";
            List<SERVICO> lista = (List<SERVICO>)Session["ListaServico"];
            SERVICO filtro = (SERVICO)Session["FiltroServico"];
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
            Image image = Image.GetInstance(Server.MapPath("~/Images/5.png"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Serviços - Listagem", meuFont2))
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
            table = new PdfPTable(new float[] { 70f, 50f, 160f, 300f, 70f, 50f, 80f, 80f});
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Serviços selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 8;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Categoria", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Código", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Nome", meuFont))
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
            cell = new PdfPCell(new Paragraph("Duração", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Duração Expressa", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Local", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Visita (R$)", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);



            foreach (SERVICO item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.CATEGORIA_SERVICO.CASE_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.SERV_CD_CODIGO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.SERV_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.SERV_DS_DESCRICAO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.SERV_NR_DURACAO.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.SERV_NR_DURACAO_EXPRESSA.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.SERV_IN_LOCAL == 1)
                {
                    cell = new PdfPCell(new Paragraph("Interno", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.SERV_IN_LOCAL == 2)
                {
                    cell = new PdfPCell(new Paragraph("Externo", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.SERV_IN_LOCAL == 3)
                {
                    cell = new PdfPCell(new Paragraph("Interno/Externo", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.SERV_VL_VISITA != null & item.SERV_IN_LOCAL > 1 )
                {
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.SERV_VL_VISITA.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.SERV_VL_VISITA == null & item.SERV_IN_LOCAL > 1 )
                {
                    cell = new PdfPCell(new Paragraph("Não Definido", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.SERV_IN_LOCAL == 1)
                {
                    cell = new PdfPCell(new Paragraph("Não se Aplica", meuFont))
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
                if (filtro.CASE_CD_ID > 0)
                {
                    parametros += "Categoria: " + filtro.CASE_CD_ID;
                    ja = 1;
                }
                if (filtro.SERV_NM_NOME != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Nome: " + filtro.SERV_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Nome: " + filtro.SERV_NM_NOME;
                    }
                }
                if (filtro.SERV_DS_DESCRICAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Descrição: " + filtro.SERV_DS_DESCRICAO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Descrição: " + filtro.SERV_DS_DESCRICAO;
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

            return RedirectToAction("MontarTelaServico");
        }

        public ActionResult GerarRelatorioDetalhe()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            SERVICO serv = servApp.GetById((Int32)Session["IdVolta"]);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "Serviço" + serv.SERV_CD_ID.ToString() + "_" + data + ".pdf";
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Images/5.png"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Serviço - Detalhes", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);

            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Dados Gerais
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Dados Gerais", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (serv.CATEGORIA_SERVICO != null)
            {
                cell = new PdfPCell(new Paragraph("Categoria: " + serv.CATEGORIA_SERVICO.CASE_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Categoria: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            //cell = new PdfPCell(new Paragraph("Filial: " + serv.FILIAL.FILI_NM_NOME, meuFont));
            //cell.Border = 0;
            //cell.Colspan = 1;
            //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //table.AddCell(cell);

            if (serv.UNIDADE != null)
            {
                cell = new PdfPCell(new Paragraph("Unidade: " + serv.UNIDADE.UNID_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Unidade: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            if (serv.NOMENCLATURA_BRAS_SERVICOS != null)
            {
                cell = new PdfPCell(new Paragraph("Cod.NBS: " + serv.NOMENCLATURA_BRAS_SERVICOS.NBSE_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Cod.NBS: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Código: " + serv.SERV_CD_CODIGO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome: " + serv.SERV_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);     
            
            cell = new PdfPCell(new Paragraph("Local: " + (serv.SERV_IN_LOCAL.Value == 1 ? "Interno" : (serv.SERV_IN_LOCAL == 2 ? "Externo" : "Interno/Externo")), meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Duração: " + serv.SERV_NR_DURACAO.ToString(), meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Duração Expressa: " + serv.SERV_NR_DURACAO_EXPRESSA.ToString(), meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (serv.SERV_VL_VISITA != null & serv.SERV_IN_LOCAL != 1)
            {
                cell = new PdfPCell(new Paragraph("Visita (R$): " + CrossCutting.Formatters.DecimalFormatter(serv.SERV_VL_VISITA.Value), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else if (serv.SERV_VL_VISITA == null & serv.SERV_IN_LOCAL != 1)
            {
                cell = new PdfPCell(new Paragraph("Visita (R$): Não Definida", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else if (serv.SERV_IN_LOCAL == 2)
            {
                cell = new PdfPCell(new Paragraph("Visita (R$): Não se Aplica", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            pdfDoc.Add(table);

            //Descirição
            Chunk chunk = new Chunk("Descrição: " + serv.SERV_DS_DESCRICAO, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Observações
            Chunk chunk1 = new Chunk("Observações: " + serv.SERV_TX_OBSERVACOES, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("VoltarAnexoServico");
        }

        [HttpGet]
        public ActionResult MontarTelaDashboardServiceDesk()
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
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(usuario);

            // Estatisticas
            List<ATENDIMENTO> listaGeral = ateApp.GetAllItens(idAss);
            Int32 atendMes = listaGeral.Where(p => p.ATEN_DT_INICIO.Value.Month == DateTime.Today.Date.Month & p.ATEN_DT_INICIO.Value.Year == DateTime.Today.Date.Year).ToList().Count;
            Int32 atendTotal = listaGeral.Count;
            Int32 encerradosTotal = listaGeral.Where(p => p.ATEN_IN_STATUS == 5).ToList().Count;
            Int32 atraso = listaGeral.Where(p => p.ATEN_IN_STATUS != 5 & p.ATEN_IN_STATUS != 3 & p.ATEN_DT_PREVISTA.Value < DateTime.Today.Date).ToList().Count;
            List<ATENDIMENTO> listaEnc = listaGeral.Where(p => p.ATEN_IN_STATUS == 5).ToList();
            Int32 encerradosMes = listaEnc.Where(p => p.ATEN_DT_ENCERRAMENTO != null & p.ATEN_DT_ENCERRAMENTO.Value.Month == DateTime.Today.Date.Month & p.ATEN_DT_ENCERRAMENTO.Value.Year == DateTime.Today.Date.Year & p.ATEN_IN_STATUS == 5).ToList().Count;
            List<ATENDIMENTO> listaSLA = listaGeral.Where(p => p.CATEGORIA_ATENDIMENTO.CAAT_IN_SLA != null).ToList();
            Int32 foraSLA = listaSLA.Where(p => p.ATEN_IN_STATUS != 5 & p.ATEN_IN_STATUS != 3 & (p.ATEN_DT_INICIO.Value.AddHours(p.CATEGORIA_ATENDIMENTO.CAAT_IN_SLA.Value) < DateTime.Today.Date)).ToList().Count;

            ViewBag.TotalMes = atendMes;
            ViewBag.Total = atendTotal;
            ViewBag.EncerradosMes = encerradosMes;
            ViewBag.Encerrados = encerradosTotal;
            ViewBag.Atraso = atraso;
            ViewBag.ForaSLA = foraSLA;

            // Atendimento por Data
            List<DateTime> datas = listaGeral.Where(m => m.ATEN_IN_STATUS != 3 & m.ATEN_DT_INICIO.Value.Month == DateTime.Today.Date.Month & m.ATEN_DT_INICIO.Value.Year == DateTime.Today.Date.Year).Select(p => p.ATEN_DT_INICIO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> listaMod = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32? conta = listaGeral.Where(p => p.ATEN_DT_INICIO.Value.Date == item).ToList().Count;
                ModeloViewModel mod1 = new ModeloViewModel();
                mod1.DataEmissao = item;
                mod1.Valor1 = conta.Value;
                listaMod.Add(mod1);
            }
            ViewBag.ListaAtendDia = listaMod;
            Session["ListaDatas"] = datas;
            Session["ListaAtendDia"] = listaMod;

            // Atendimento por Status
            Int32 sit1 = listaGeral.Where(p => p.ATEN_IN_STATUS == 1).ToList().Count;
            Int32 sit2 = listaGeral.Where(p => p.ATEN_IN_STATUS == 2).ToList().Count;
            Int32 sit3 = listaGeral.Where(p => p.ATEN_IN_STATUS == 3).ToList().Count;
            Int32 sit4 = listaGeral.Where(p => p.ATEN_IN_STATUS == 4).ToList().Count;
            Int32 sit5 = listaGeral.Where(p => p.ATEN_IN_STATUS == 5).ToList().Count;

            List<ModeloViewModel> lista1 = new List<ModeloViewModel>();
            ModeloViewModel mod = new ModeloViewModel();
            mod.Data = "Criado";
            mod.Valor = sit1;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Pendente";
            mod.Valor = sit2;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Cancelado";
            mod.Valor = sit3;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Em Execução";
            mod.Valor = sit4;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Encerrado";
            mod.Valor = sit5;
            lista1.Add(mod);
            ViewBag.ListaSituacao = lista1;
            Session["ListaSituacao"] = lista1;

            Session["CR"] = sit1;
            Session["PE"] = sit2;
            Session["CA"] = sit3;
            Session["EX"] = sit4;
            Session["EN"] = sit5;

            // Atendimento por Categoria
            List<CATEGORIA_ATENDIMENTO> cats = caApp.GetAllItens(idAss);
            Int32 num = 0;
            List<ModeloViewModel> lista2 = new List<ModeloViewModel>();
            ModeloViewModel mod3 = new ModeloViewModel();
            foreach (var item in cats)
            {
                num = listaGeral.Where(p => p.CAAT_CD_ID == item.CAAT_CD_ID).ToList().Count;
                mod3 = new ModeloViewModel();
                mod3.Data = item.CAAT_NM_NOME;
                mod3.Valor = num;
                lista2.Add(mod3);

            }
            ViewBag.ListaCategoria = lista2;
            Session["ListaCategoria"] = lista2;

            // Atendimento por Cliente
            List<CLIENTE> listaForn = cliApp.GetAllItens(idAss);
            List<Int32> forns = listaGeral.Where(m => m.ATEN_IN_STATUS != 3 & (m.CLIE_CD_ID != null & m.CLIE_CD_ID != 0)).Select(p => p.CLIE_CD_ID.Value).Distinct().ToList();
            List<ModeloViewModel> listaMod4 = new List<ModeloViewModel>();
            foreach (Int32 item in forns)
            {
                Int32? conta4 = listaGeral.Where(p => p.CLIE_CD_ID == item).ToList().Count;
                String nome4 = listaForn.First(p => p.CLIE_CD_ID == item).CLIE_NM_NOME;
                ModeloViewModel mod4 = new ModeloViewModel();
                mod4.Nome = nome4;
                mod4.Valor1 = conta4.Value;
                listaMod4.Add(mod4);
            }
            listaMod4 = listaMod4.OrderByDescending(p => p.Valor1).ToList();
            ViewBag.ListaAtendCliente = listaMod4;

            // Atendimento por Produto
            List<PRODUTO> listaProd = proApp.GetAllItens(idAss);
            List<Int32> prods = listaGeral.Where(m => m.ATEN_IN_STATUS != 3 & (m.PROD_CD_ID != null & m.PROD_CD_ID != 0)).Select(p => p.PROD_CD_ID.Value).Distinct().ToList();
            List<ModeloViewModel> listaMod5 = new List<ModeloViewModel>();
            foreach (Int32 item in prods)
            {
                Int32? conta5 = listaGeral.Where(p => p.PROD_CD_ID == item).ToList().Count;
                String nome5 = listaProd.First(p => p.PROD_CD_ID == item).PROD_NM_NOME;
                ModeloViewModel mod5 = new ModeloViewModel();
                mod5.Nome = nome5;
                mod5.Valor1 = conta5.Value;
                listaMod5.Add(mod5);
            }
            listaMod5 = listaMod5.OrderByDescending(p => p.Valor1).ToList();
            ViewBag.ListaAtendProduto = listaMod5;

            // Atendimento por Serviço
            List<SERVICO> listaServ = servApp.GetAllItens(idAss);
            List<Int32> servs = listaGeral.Where(m => m.ATEN_IN_STATUS != 3 & (m.SERV_CD_ID != null & m.SERV_CD_ID != 0)).Select(p => p.SERV_CD_ID.Value).Distinct().ToList();
            List<ModeloViewModel> listaMod6 = new List<ModeloViewModel>();
            foreach (Int32 item in servs)
            {
                Int32? conta6 = listaGeral.Where(p => p.SERV_CD_ID == item).ToList().Count;
                String nome6 = listaServ.First(p => p.SERV_CD_ID == item).SERV_NM_NOME;
                ModeloViewModel mod6 = new ModeloViewModel();
                mod6.Nome = nome6;
                mod6.Valor1 = conta6.Value;
                listaMod6.Add(mod6);
            }
            listaMod6 = listaMod6.OrderByDescending(p => p.Valor1).ToList();
            ViewBag.ListaAtendServico = listaMod6;

            // Atendimento por Atribuição
            List<USUARIO> usus = usuApp.GetAllItens(idAss);
            Int32 num7 = 0;
            List<ModeloViewModel> lista7 = new List<ModeloViewModel>();
            ModeloViewModel mod7 = new ModeloViewModel();
            foreach (var item in usus)
            {
                num7 = listaGeral.Where(p => p.USUA_CD_ID == item.USUA_CD_ID).ToList().Count;
                mod7 = new ModeloViewModel();
                mod7.Data = item.USUA_NM_NOME;
                mod7.Valor = num7;
                lista7.Add(mod7);

            }
            ViewBag.ListaUsuario = lista7;
            Session["ListaUsuario"] = lista7;

            // Atendimento por Prioridade
            Int32 pri1 = listaGeral.Where(p => p.ATEN_IN_PRIORIDADE == 1).ToList().Count;
            Int32 pri2 = listaGeral.Where(p => p.ATEN_IN_PRIORIDADE == 2).ToList().Count;
            Int32 pri3 = listaGeral.Where(p => p.ATEN_IN_PRIORIDADE == 3).ToList().Count;
            Int32 pri4 = listaGeral.Where(p => p.ATEN_IN_PRIORIDADE == 4).ToList().Count;

            List<ModeloViewModel> lista8 = new List<ModeloViewModel>();
            ModeloViewModel mod8 = new ModeloViewModel();
            mod8.Data = "Normal";
            mod8.Valor = pri1;
            lista8.Add(mod);
            mod8 = new ModeloViewModel();
            mod8.Data = "Baixa";
            mod8.Valor = pri2;
            lista8.Add(mod);
            mod8 = new ModeloViewModel();
            mod8.Data = "Alta";
            mod8.Valor = pri3;
            lista8.Add(mod);
            mod8 = new ModeloViewModel();
            mod8.Data = "Urgente";
            mod8.Valor = pri4;
            lista8.Add(mod);
            ViewBag.ListaPrioridade = lista8;
            Session["ListaPrioridade"] = lista8;

            Session["NO"] = pri1;
            Session["BA"] = pri2;
            Session["AL"] = pri3;
            Session["UR"] = pri4;

            // Atendimento por Tipo
            Int32 tipo1 = listaGeral.Where(p => p.ATEN_IN_TIPO == 1).ToList().Count;
            Int32 tipo2 = listaGeral.Where(p => p.ATEN_IN_TIPO == 2).ToList().Count;

            List<ModeloViewModel> lista9 = new List<ModeloViewModel>();
            ModeloViewModel mod9 = new ModeloViewModel();
            mod9.Data = "Interno";
            mod9.Valor = tipo1;
            lista9.Add(mod);
            mod9 = new ModeloViewModel();
            mod9.Data = "Baixa";
            mod8.Valor = tipo2;
            lista9.Add(mod);
            ViewBag.ListaTipo= lista9;
            Session["ListaTipo"] = lista9;

            Session["IN"] = tipo1;
            Session["EX"] = tipo2;

            // tempo medio de atendimento por usuario
            List<ATENDIMENTO> lista10 = listaGeral.Where(p => p.ATEN_IN_STATUS == 5).ToList();
            List<ModeloViewModel> listaMod10 = new List<ModeloViewModel>();
            List<ModeloViewModel> listaMod11 = new List<ModeloViewModel>();
            List<ModeloViewModel> listaMod12 = new List<ModeloViewModel>();
            ModeloViewModel mod10 = new ModeloViewModel();
            foreach (var item in lista10)
            {
                TimeSpan diff = item.ATEN_DT_ENCERRAMENTO.Value - item.ATEN_DT_INICIO.Value;
                double hours = diff.TotalHours;
                mod10 = new ModeloViewModel();
                mod10.ValorDouble = hours;
                mod10.Valor = item.USUA_CD_ID.Value;
                listaMod10.Add(mod10);
            }
            ModeloViewModel mod12 = new ModeloViewModel();
            foreach (var item1 in usus)
            {
                listaMod11 = listaMod10.Where(p => p.Valor == item1.USUA_CD_ID).ToList();
                Int32 conta = listaMod11.Count;
                if (conta > 0)
                {
                    Double soma = listaMod11.Sum(p => p.ValorDouble);
                    Double media = soma / conta;
                    mod12 = new ModeloViewModel();
                    mod12.ValorDouble = soma;
                    mod12.ValorDouble2 = media;
                    mod12.Data = item1.USUA_NM_NOME;
                    listaMod12.Add(mod12);
                }
            }
            ViewBag.ListaTempoUsuario = listaMod12;
            Session["ListaTempoUsuario"] = listaMod12;
            return View(vm);
        }

        public JsonResult GetDadosGraficoAtendDia()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaAtendDia"];
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);
            listaCP1 = listaCP1.OrderBy(p => p.DataEmissao).ToList();

            foreach (ModeloViewModel item in listaCP1)
            {
                dias.Add(item.DataEmissao.ToShortDateString());
                valor.Add(item.Valor1);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoSituacao()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["CR"];
            Int32 q2 = (Int32)Session["PE"];
            Int32 q3 = (Int32)Session["CA"];
            Int32 q4 = (Int32)Session["EX"];
            Int32 q5 = (Int32)Session["EN"];

            desc.Add("Criado");
            quant.Add(q1);
            cor.Add("#359E18");
            desc.Add("Pendente");
            quant.Add(q2);
            cor.Add("#FFAE00");
            desc.Add("Cancelado");
            quant.Add(q3);
            cor.Add("#FF7F00");
            desc.Add("Em Execução");
            quant.Add(q4);
            cor.Add("#744d61");
            desc.Add("Encerrado");
            quant.Add(q5);
            cor.Add("#f2e6b1");

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoCategoria()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            Int32 ind = 0;

            String[] cores = new String[5];
            cores[0] = "#359E18";
            cores[1] = "#FFAE00";
            cores[2] = "#FF7F00";
            cores[3] = "#744d61";
            cores[4] = "#f2e6b1";

            List<ModeloViewModel> lista2 = (List<ModeloViewModel>)Session["ListaCategoria"];
            foreach (var item in lista2)
            {
                desc.Add(item.Data);
                quant.Add(item.Valor);
                cor.Add(cores[ind]);
                ind++;
                if (ind > 4)
                {
                    ind = 0;
                }
            }

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoAtribuicao()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            Int32 ind = 0;

            String[] cores = new String[5];
            cores[0] = "#359E18";
            cores[1] = "#FFAE00";
            cores[2] = "#FF7F00";
            cores[3] = "#744d61";
            cores[4] = "#f2e6b1";

            List<ModeloViewModel> lista2 = (List<ModeloViewModel>)Session["ListaUsuario"];
            foreach (var item in lista2)
            {
                desc.Add(item.Data);
                quant.Add(item.Valor);
                cor.Add(cores[ind]);
                ind++;
                if (ind > 4)
                {
                    ind = 0;
                }
            }

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoPrioridade()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["NO"];
            Int32 q2 = (Int32)Session["BA"];
            Int32 q3 = (Int32)Session["AL"];
            Int32 q4 = (Int32)Session["UR"];

            desc.Add("Normal");
            quant.Add(q1);
            cor.Add("#359E18");
            desc.Add("Baixa");
            quant.Add(q2);
            cor.Add("#FFAE00");
            desc.Add("Alta");
            quant.Add(q3);
            cor.Add("#FF7F00");
            desc.Add("Urgente");
            quant.Add(q4);
            cor.Add("#744d61");

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoTipo()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["IN"];
            Int32 q2 = (Int32)Session["EX"];

            desc.Add("Interno");
            quant.Add(q1);
            cor.Add("#359E18");
            desc.Add("Externo");
            quant.Add(q2);
            cor.Add("#FFAE00");

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }


    }
}